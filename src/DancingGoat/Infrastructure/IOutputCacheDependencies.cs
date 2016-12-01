using CMS.DataEngine;
using CMS.DocumentEngine;

namespace DancingGoat.Infrastructure
{
    /// <summary>
    /// Represents a contract for objects that create a minimum set of ASP.NET output cache dependencies for views that contain data from pages or info objects.
    /// </summary>
    public interface IOutputCacheDependencies
    {
        /// <summary>
        /// Adds a minimum set of ASP.NET output cache dependencies for a view that contains data from pages of the specified runtime type.
        /// When any page of the specified runtime type is created, updated or deleted, the corresponding output cache item is invalidated.
        /// </summary>
        /// <typeparam name="T">Runtime type that represents pages, i.e. it is derived from the <see cref="TreeNode"/> class.</typeparam>
        void AddDependencyOnPages<T>() where T : TreeNode, new();


        /// <summary>
        /// Adds a minimum set of ASP.NET output cache dependencies for a view that contains data from info objects of the specified runtime type.
        /// When any info object of the specified runtime type is created, updated or deleted, the corresponding output cache item is invalidated.
        /// </summary>
        /// <typeparam name="T">Runtime type that represents info objects, i.e. it is derived from the <see cref="AbstractInfo{TInfo}"/> class.</typeparam>
        void AddDependencyOnInfoObjects<T>() where T : AbstractInfo<T>, new();
    }
}