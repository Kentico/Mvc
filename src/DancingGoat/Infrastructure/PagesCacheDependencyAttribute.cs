using System;

namespace DancingGoat.Infrastructure
{
    /// <summary>
    /// Provides cache dependency settings for methods retrieving pages.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class PagesCacheDependencyAttribute : CacheDependencyAttribute
    {
        /// <summary>
        /// Cache key format.
        /// </summary>
        internal const string KEY_FORMAT = "nodes|{0}|{1}|all";

        private readonly string mPageTypeClassName;


        /// <summary>
        /// Initializes a new instance of the <see cref="PagesCacheDependencyAttribute"/> class.
        /// </summary>
        /// <param name="pageTypeClassName">Class name of the page type on which the result of the decorated method depends.</param>
        /// <exception cref="ArgumentException">Thrown when the given key format is null or empty.</exception>
        public PagesCacheDependencyAttribute(string pageTypeClassName)
            : base(KEY_FORMAT)
        {
            mPageTypeClassName = pageTypeClassName;
        }


        /// <summary>
        /// Returns a resolved dummy cache key for dependency on pages.
        /// </summary>
        /// <param name="siteName">Site name representing context of a site.</param>
        /// <param name="methodArguments">Array of values for the method arguments.</param>
        internal override string ResolveKey(string siteName, object[] methodArguments)
        {
            return string.Format(KEY_FORMAT, siteName, mPageTypeClassName);
        }
    }
}