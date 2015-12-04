using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Kentico.Web.Mvc
{
    /// <summary>
    /// Provides extension methods related to Kentico ASP.NET MVC integration features.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Transparently handles preview URLs and also disables output caching in preview mode.
        /// </summary>
        /// <param name="builder">The application builder.</param>
        public static void UsePreview(this ApplicationBuilder builder)
        {
            var module = new PreviewFeatureModule();
            builder.RegisterModule(module);
        }


        /// <summary>
        /// Enables localization of ASP.NET model meta-data based on data annotation attributes.
        /// Display names or validation results declared with data annotation attributes can contain keys of Kentico resource strings that will be resolved automatically using Kentico localization API.
        /// The localization uses a custom model metadata provider based on data annotations and is therefore not compatible with other providers or their customizations.
        /// </summary>
        /// <param name="builder">The application builder.</param>
        public static void UseDataAnnotationsLocalization(this ApplicationBuilder builder)
        {
            ModelMetadataProviders.Current = new LocalizedDataAnnotationsModelMetadataProvider();

            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(RangeAttribute), typeof(LocalizedRangeAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(RegularExpressionAttribute), typeof(LocalizedRegularExpressionAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(RequiredAttribute), typeof(LocalizedRequiredAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(StringLengthAttribute), typeof(LocalizedStringLengthAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(MaxLengthAttribute), typeof(LocalizedMaxLengthAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(MinLengthAttribute), typeof(LocalizedMinLengthAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapterFactory(typeof(CreditCardAttribute), (metadata, context, attribute) => new LocalizedDataTypeAttributeAdapter(metadata, context, (DataTypeAttribute)attribute, "creditcard"));
            DataAnnotationsModelValidatorProvider.RegisterAdapterFactory(typeof(EmailAddressAttribute), (metadata, context, attribute) => new LocalizedDataTypeAttributeAdapter(metadata, context, (DataTypeAttribute)attribute, "email"));
            DataAnnotationsModelValidatorProvider.RegisterAdapterFactory(typeof(PhoneAttribute), (metadata, context, attribute) => new LocalizedDataTypeAttributeAdapter(metadata, context, (DataTypeAttribute)attribute, "phone"));
            DataAnnotationsModelValidatorProvider.RegisterAdapterFactory(typeof(UrlAttribute), (metadata, context, attribute) => new LocalizedDataTypeAttributeAdapter(metadata, context, (DataTypeAttribute)attribute, "url"));
        }


        /// <summary>
        /// Displays the <c>~/Views/Shared/NotFound.cshtml</c> view when a controller is not found, an action is not found, or the action returns the <see cref="HttpNotFoundResult"/> view result.
        /// </summary>
        /// <param name="builder">The application builder.</param>
        public static void UseNotFoundHandler(this ApplicationBuilder builder)
        {
            var currentFactory = ControllerBuilder.Current.GetControllerFactory();
            var currentFactoryWrapper = new ControllerFactoryWrapper(currentFactory);
            ControllerBuilder.Current.SetControllerFactory(currentFactoryWrapper);
            GlobalFilters.Filters.Add(new HandleNotFoundErrorAttribute());
        }
    }
}
