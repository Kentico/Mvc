using System;

namespace DancingGoat.Infrastructure
{
    /// <summary>
    /// Provides cache dependency settings for methods.
    /// Allows to specify the cache dependency key containing values from method arguments.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CacheDependencyAttribute : Attribute
    {
        private readonly string mDependencyKeyFormat;


        /// <summary>
        /// Initializes a new instance of the <see cref="CacheDependencyAttribute"/> class.
        /// </summary>
        /// <example>
        /// The following example shows how to use the <see cref="CacheDependencyAttribute"/> with the first method argument.
        /// <code>
        /// [CacheDependency("nodeid|{0}")]
        /// public TreeNode GetTreeNode(int nodeID)
        /// {
        ///     ...
        /// }
        /// </code>
        /// </example>
        /// <param name="dependencyKeyFormat">Format of the cache dependency key. Values in curly braces will be replaced by method arguments.</param>
        /// <exception cref="ArgumentException">Thrown when the given key format is null or empty.</exception>
        public CacheDependencyAttribute(string dependencyKeyFormat)
        {
            if (String.IsNullOrEmpty(dependencyKeyFormat))
            {
                throw new ArgumentException(nameof(dependencyKeyFormat));
            }

            mDependencyKeyFormat = dependencyKeyFormat;
        }


        /// <summary>
        /// Returns a resolved dummy cache key for dependency.
        /// </summary>
        /// <param name="siteName">Site name representing the context of a site.</param>
        /// <param name="methodArguments">Array of values for the method arguments.</param>
        internal virtual string ResolveKey(string siteName, object[] methodArguments)
        {
            return string.Format(mDependencyKeyFormat, methodArguments);
        }
    }
}