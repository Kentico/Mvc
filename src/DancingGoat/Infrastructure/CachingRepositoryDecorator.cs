using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;

using Castle.DynamicProxy;
using DancingGoat.Infrastructure;

namespace DancingGoat
{
    /// <summary>
    /// Provides caching for repository methods that return a single content item or a collection of content items. 
    /// Only results of methods starting with 'Get' are cached. A minimum set of cache dependencies is also specified 
    /// so when a content item changes the cached result is invalidated.
    /// </summary>
    public sealed class CachingRepositoryDecorator : IInterceptor
    {
        private readonly string mSiteName;
        private readonly TimeSpan mCacheItemDuration;
        private readonly IContentItemMetadataProvider mContentItemMetadataProvider;
        private readonly bool mCacheEnabled;


        /// <summary>
        /// Initializes a new instance of the <see cref="CachingRepositoryDecorator"/> class.
        /// </summary>
        /// <param name="siteName">The code name of a site.</param>
        /// <param name="cacheItemDuration">The time duration during which the repository method result is cached.</param>
        /// <param name="contentItemMetadataProvider">The object that provides information about pages and info objects using their runtime type.</param>
        /// <param name="cacheEnabled">Indicates whether caching is enabled.</param>
        public CachingRepositoryDecorator(string siteName, TimeSpan cacheItemDuration, IContentItemMetadataProvider contentItemMetadataProvider, bool cacheEnabled)
        {
            mSiteName = siteName.ToLowerInvariant();
            mCacheItemDuration = cacheItemDuration;
            mContentItemMetadataProvider = contentItemMetadataProvider;
            mCacheEnabled = cacheEnabled;
        }


        /// <summary>
        /// Returns the cached result for the specified method invocation, if possible; otherwise proceeds with the invocation and caches the result.
        /// Only results of methods starting with 'Get' are affected.
        /// </summary>
        /// <param name="invocation">The method invocation.</param>
        public void Intercept(IInvocation invocation)
        {
            if (!mCacheEnabled || !invocation.Method.Name.StartsWith("Get"))
            {
                invocation.Proceed();

                return;
            }

            var returnType = invocation.Method.ReturnType;

            if (typeof(TreeNode).IsAssignableFrom(returnType))
            {
                invocation.ReturnValue = GetCachedResult(invocation, GetDependencyCacheKeyForPage(returnType));
            }
            else if (typeof(IEnumerable<TreeNode>).IsAssignableFrom(returnType))
            {
                invocation.ReturnValue = GetCachedResult(invocation, GetDependencyCacheKeyForPage(returnType.GenericTypeArguments[0]));
            }
            else if (typeof(BaseInfo).IsAssignableFrom(returnType))
            {
                invocation.ReturnValue = GetCachedResult(invocation, GetDependencyCacheKeyForObject(returnType));
            }
            else if (typeof(IEnumerable<BaseInfo>).IsAssignableFrom(returnType))
            {
                invocation.ReturnValue = GetCachedResult(invocation, GetDependencyCacheKeyForObject(returnType.GenericTypeArguments[0]));
            }
            else
            {
                invocation.Proceed();
            }
        }


        private object GetCachedResult(IInvocation invocation, string dependencyCacheKey)
        {
            var cacheKey = GetCacheItemKey(invocation);
            var cacheSettings = CreateCacheSettings(cacheKey, dependencyCacheKey);
            Func<Object> provideData = () =>
            {
                invocation.Proceed();
                return invocation.ReturnValue;
            };

            return CacheHelper.Cache(provideData, cacheSettings);
        }


        private string GetCacheItemKey(IInvocation invocation)
        {
            var builder = new StringBuilder(127)
                .Append("MvcDemo|Data")
                .Append("|").Append(invocation.TargetType.FullName)
                .Append("|").Append(invocation.Method.Name)
                .Append("|").Append(CultureInfo.CurrentUICulture.Name);

            foreach (var value in invocation.Arguments)
            {
                builder.AppendFormat(CultureInfo.InvariantCulture, "|{0}", value);
            }

            return builder.ToString();
        }


        private CacheSettings CreateCacheSettings(string cacheKey, string dependencyCacheKey)
        {
            return new CacheSettings(mCacheItemDuration.TotalMinutes, cacheKey)
            {
                GetCacheDependency = () => CacheHelper.GetCacheDependency(dependencyCacheKey)
            };
        }


        private string GetDependencyCacheKeyForPage(Type type)
        {
            return String.Format("nodes|{0}|{1}|all", mSiteName, mContentItemMetadataProvider.GetClassNameFromPageRuntimeType(type));
        }


        private string GetDependencyCacheKeyForObject(Type type)
        {
            return String.Format("{0}|all", mContentItemMetadataProvider.GetObjectTypeFromInfoObjectRuntimeType(type));
        }
    }
}