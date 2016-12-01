using System;
using System.Collections.Concurrent;

using CMS.DataEngine;
using CMS.DocumentEngine;

namespace DancingGoat.Infrastructure
{
    /// <summary>
    /// Provides information about pages and info objects using their runtime type.
    /// This class is thread-safe and class names and object types are normalized, i.e. they are converted to lowercase.
    /// </summary>
    public sealed class ContentItemMetadataProvider : IContentItemMetadataProvider
    {
        private readonly ConcurrentDictionary<Type, String> mClassNames = new ConcurrentDictionary<Type, String>();
        private readonly ConcurrentDictionary<Type, String> mObjectTypes = new ConcurrentDictionary<Type, String>();


        /// <summary>
        /// Returns a class name of a page.
        /// </summary>
        /// <param name="type">Runtime type that represents pages, i.e. it is derived from the <see cref="TreeNode"/> class.</param>
        /// <returns>Lowercase class name of a page.</returns>
        public string GetClassNameFromPageRuntimeType(Type type)
        {
            return mClassNames.GetOrAdd(type, x => ((TreeNode)Activator.CreateInstance(type)).ClassName.ToLowerInvariant());
        }


        /// <summary>
        /// Returns a class name of a page.
        /// </summary>
        /// <typeparam name="T">Runtime type that represents pages, i.e. it is derived from the <see cref="TreeNode"/> class.</typeparam>
        /// <returns>Lowercase class name of a page.</returns>
        public string GetClassNameFromPageRuntimeType<T>() where T : TreeNode, new()
        {
            return mClassNames.GetOrAdd(typeof(T), x => new T().ClassName.ToLowerInvariant());
        }


        /// <summary>
        /// Returns an object type of an info object.
        /// </summary>
        /// <param name="type">Runtime type that represents info objects, i.e. it is derived from the <see cref="AbstractInfo{TInfo}"/> class.</param>
        /// <returns>Lowercase object type of an info object.</returns>
        public string GetObjectTypeFromInfoObjectRuntimeType(Type type)
        {
            return mObjectTypes.GetOrAdd(type, x => ((BaseInfo)Activator.CreateInstance(type)).TypeInfo.ObjectType.ToLowerInvariant());
        }


        /// <summary>
        /// Returns an object type of an info object.
        /// </summary>
        /// <typeparam name="T">Runtime type that represents info objects, i.e. it is derived from the <see cref="AbstractInfo{TInfo}"/> class.</typeparam>
        /// <returns>Lowercase object type of an info object.</returns>
        public string GetObjectTypeFromInfoObjectRuntimeType<T>() where T : AbstractInfo<T>, new()
        {
            return mObjectTypes.GetOrAdd(typeof(T), x => new T().TypeInfo.ObjectType.ToLowerInvariant());
        }
    }
}