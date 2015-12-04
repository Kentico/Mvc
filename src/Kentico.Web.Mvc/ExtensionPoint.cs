using System;

namespace Kentico.Web.Mvc
{
    /// <summary>
    /// Represents a point for extension methods.
    /// </summary>
    /// <typeparam name="T">The type of the class that is a target of extension methods.</typeparam>
    public sealed class ExtensionPoint<T> where T : class
    {
        private readonly T mTarget;


        internal T Target
        {
            get
            {
                return mTarget;
            }
        }


        internal ExtensionPoint(T target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            mTarget = target;
        }
    }
}
