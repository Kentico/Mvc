using System;
using System.Collections.Concurrent;

namespace Kentico.Web.Mvc
{
    /// <summary>
    /// Represents a set of features available for the current request.
    /// </summary>
    internal sealed class FeatureSet : IFeatureSet
    {
        private readonly ConcurrentDictionary<Type, object> mFeaturesByType = new ConcurrentDictionary<Type, object>();


        /// <summary>
        /// Adds or replaces a feature of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the feature.</typeparam>
        /// <param name="feature">The feature to add or replace.</param>
        public void SetFeature<T>(object feature)
        {
            mFeaturesByType.AddOrUpdate(typeof(T), feature, (key, value) => feature);
        }


        /// <summary>
        /// Returns a feature of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the feature.</typeparam>
        /// <returns>A feature of the specified type, if available; otherwise, null.</returns>
        public T GetFeature<T>()
        {
            object feature;
            if (mFeaturesByType.TryGetValue(typeof(T), out feature))
            {
                return (T)feature;
            }

            return default(T);
        }


        /// <summary>
        /// Removes a feature of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the feature.</typeparam>
        public void RemoveFeature<T>()
        {
            object feature;
            mFeaturesByType.TryRemove(typeof(T), out feature);
        }
    }
}