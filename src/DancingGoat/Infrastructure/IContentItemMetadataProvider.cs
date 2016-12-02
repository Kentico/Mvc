using System;

using CMS.DataEngine;
using CMS.DocumentEngine;

namespace DancingGoat.Infrastructure
{
    /// <summary>
    /// Represents a contract for objects that provide information about pages and info objects using their runtime type.
    /// The objects are thread-safe and class names and object types are normalized, i.e. they are converted to lowercase.
    /// </summary>
    public interface IContentItemMetadataProvider
    {
        /// <summary>
        /// Returns a class name of a page.
        /// </summary>
        /// <param name="type">Runtime type that represents pages, i.e. it is derived from the <see cref="TreeNode"/> class.</param>
        /// <returns>Lowercase class name of a page.</returns>
        string GetClassNameFromPageRuntimeType(Type type);


        /// <summary>
        /// Returns a class name of a page.
        /// </summary>
        /// <typeparam name="T">Runtime type that represents pages, i.e. it is derived from the <see cref="TreeNode"/> class.</typeparam>
        /// <returns>Lowercase class name of a page.</returns>
        string GetClassNameFromPageRuntimeType<T>() where T : TreeNode, new();


        /// <summary>
        /// Returns an object type of an info object.
        /// </summary>
        /// <param name="type">Runtime type that represents info objects, i.e. it is derived from the <see cref="AbstractInfo{TInfo}"/> class.</param>
        /// <returns>Lowercase object type of an info object.</returns>
        string GetObjectTypeFromInfoObjectRuntimeType(Type type);


        /// <summary>
        /// Returns an object type of an info object.
        /// </summary>
        /// <typeparam name="T">Runtime type that represents info objects, i.e. it is derived from the <see cref="AbstractInfo{TInfo}"/> class.</typeparam>
        /// <returns>Lowercase object type of an info object.</returns>
        string GetObjectTypeFromInfoObjectRuntimeType<T>() where T : AbstractInfo<T>, new();
    }
}