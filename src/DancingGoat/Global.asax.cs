using System;
using System.Configuration;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using Kentico.Activities;
using Kentico.ContactManagement;
using Kentico.Core.DependencyInjection;
using Kentico.Web.Mvc;
using Kentico.Newsletters;
using Kentico.Search;
using Kentico.Ecommerce;

using Autofac;
using Autofac.Extras.DynamicProxy2;
using Autofac.Integration.Mvc;

using CMS.Personas;

using DancingGoat.Infrastructure;
using Kentico.Content.Web.Mvc;

namespace DancingGoat
{
    public class DancingGoatApplication : HttpApplication
    {
        public const string CACHE_VARY_BY_PERSONA = "Persona";

        private const string MEDIA_LIBRARY_NAME = "CoffeeGallery";


        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            
            // Enable and configure selected Kentico ASP.NET MVC integration features
            ApplicationConfig.RegisterFeatures(ApplicationBuilder.Current);

            BundleConfig.RegisterBundles(BundleTable.Bundles);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // Provide custom ASP.NET MVC dependency resolver using Autofac
            ConfigureDependencyResolver();
        }


        private void ConfigureDependencyResolver()
        {
            var builder = new ContainerBuilder();

            ConfigureDependencyResolverForMvcApplication(builder);

            ConfigureDependencyResolverForLibraries(builder);

            // Set the ASP.NET MVC dependency resolver
            DependencyResolver.SetResolver(new AutofacDependencyResolver(builder.Build()));
        }


        private void ConfigureDependencyResolverForMvcApplication(ContainerBuilder builder)
        {
            // Enable property injection in view pages
            builder.RegisterSource(new ViewRegistrationSource());

            // Register web abstraction classes
            builder.RegisterModule<AutofacWebTypesModule>();

            // Register controllers
            builder.RegisterControllers(typeof(DancingGoatApplication).Assembly);

            // Register repositories
            builder.RegisterAssemblyTypes(typeof(DancingGoatApplication).Assembly)
                .Where(x => x.IsClass && !x.IsAbstract && typeof(IRepository).IsAssignableFrom(x))
                .AsImplementedInterfaces()
                .WithParameter("mediaLibraryName", MEDIA_LIBRARY_NAME)
                .WithParameter((parameter, context) => parameter.Name == "cultureName", (parameter, context) => CultureInfo.CurrentUICulture.Name)
                .WithParameter((parameter, context) => parameter.Name == "latestVersionEnabled", (parameter, context) => IsPreviewEnabled())
                .EnableInterfaceInterceptors().InterceptedBy(typeof(CachingRepositoryDecorator))
                .InstancePerRequest();

            // Register services
            builder.RegisterAssemblyTypes(typeof(DancingGoatApplication).Assembly)
                .Where(x => x.IsClass && !x.IsAbstract && typeof(IService).IsAssignableFrom(x))
                .AsImplementedInterfaces()
                .InstancePerRequest();

            // Register providers of additional information about content items
            builder.RegisterType<ContentItemMetadataProvider>()
                .AsImplementedInterfaces()
                .SingleInstance();

            // Register factory for product view models
            builder.RegisterType<TypedProductViewModelFactory>()
                .SingleInstance();

            // Register factory for full-text search product view models
            builder.RegisterType<TypedSearchItemViewModelFactory>()
                .InstancePerRequest();

            // Register caching decorator for repositories
            builder.Register(context => new CachingRepositoryDecorator(GetCacheItemDuration(), context.Resolve<IContentItemMetadataProvider>(), IsCacheEnabled()))
                .InstancePerRequest();

            // Enable declaration of output cache dependencies in controllers
            builder.Register(context => new OutputCacheDependencies(context.Resolve<HttpResponseBase>(), context.Resolve<IContentItemMetadataProvider>(), IsCacheEnabled()))
                .AsImplementedInterfaces()
                .InstancePerRequest();
        }


        private void ConfigureDependencyResolverForLibraries(ContainerBuilder builder)
        {
            // Register repositories
            builder.RegisterAssemblyTypes(typeof(KenticoCustomerAddressRepository).Assembly)
                .Where(x => x.IsClass && !x.IsAbstract && typeof(IRepository).IsAssignableFrom(x))
                .AsImplementedInterfaces()
                .EnableInterfaceInterceptors().InterceptedBy(typeof(CachingRepositoryDecorator))
                .InstancePerRequest();

            // Register services
            builder.RegisterAssemblyTypes(
                typeof(ShoppingService).Assembly,
                typeof(ContactTrackingService).Assembly,
                typeof(SearchService).Assembly,
                typeof(NewsletterSubscriptionService).Assembly,
                typeof(MembershipActivitiesLogger).Assembly)
                .Where(x => x.IsClass && !x.IsAbstract && typeof(IService).IsAssignableFrom(x))
                .AsImplementedInterfaces()
                .InstancePerRequest();
        }


        public override string GetVaryByCustomString(HttpContext context, string custom)
        {
            var contactTrackingService = DependencyResolver.Current.GetService<IContactTrackingService>();

            if (custom == "User")
            {
                return $"User={context.User.Identity.Name}";
            }

            if (custom == CACHE_VARY_BY_PERSONA)
            {
                var existingContact = contactTrackingService.GetExistingContactAsync().Result;
                var contactPersonaID = existingContact?.ContactPersonaID;
                return $"{CACHE_VARY_BY_PERSONA}={contactPersonaID}|{context.User.Identity.Name}";
            }

            return base.GetVaryByCustomString(context, custom);
        }


        private static bool IsCacheEnabled()
        {
            return !IsPreviewEnabled();
        }


        private static bool IsPreviewEnabled()
        {
            return HttpContext.Current.Kentico().Preview().Enabled;
        }


        private static TimeSpan GetCacheItemDuration()
        {
            var value = ConfigurationManager.AppSettings["RepositoryCacheItemDuration"];
            var seconds = 0;

            if (Int32.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out seconds) && seconds > 0)
            {
                return TimeSpan.FromSeconds(seconds);
            }

            return TimeSpan.Zero;
        }
    }
}