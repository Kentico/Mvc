using System;

namespace Kentico.Content.Web.Mvc
{
    /// <summary>
    /// Represents a size constraint that can be enforced on image when resizing.
    /// </summary>
    public struct SizeConstraint
    {
        private readonly int mWidth;
        private readonly int mHeight;
        private readonly int mMaxWidthOrHeight;
        

        /// <summary>
        /// Gets a <see cref="SizeConstraint"/> structure that has width, height and maximum width or height of 0.
        /// </summary>
        public static readonly SizeConstraint Empty = new SizeConstraint();


        /// <summary>
        /// Gets the desired image width, in pixels. 
        /// </summary>
        public int WidthComponent
        {
            get
            {
                return mWidth;
            }
        }

        
        /// <summary>
        /// Gets the desired image height, in pixels.
        /// </summary>
        public int HeightComponent
        {
            get
            {
                return mHeight;
            }
        }


        /// <summary>
        /// Gets the desired maximum image width or height.
        /// </summary>
        public int MaxWidthOrHeightComponent
        {
            get
            {
                return mMaxWidthOrHeight;
            }
        }


        /// <summary>
        /// Tests whether this structure has width, height and maximum width or height of 0.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return ((mWidth == 0) && (mHeight == 0) && (mMaxWidthOrHeight == 0));
            }
        }

        
        private SizeConstraint(int width = 0, int height = 0, int maxWidthOrHeight = 0)
        {
            mWidth = width;
            mHeight = height;
            mMaxWidthOrHeight = maxWidthOrHeight;
        }


        /// <summary>
        /// Creates a new size constraint that makes the image resize to the specified width. This constraint maintains the aspect ratio. The resized image is never made larger than the original.
        /// </summary>
        /// <param name="width">The width of the resized image.</param>
        /// <returns>The new size constraint that makes the image resize to the specified width.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Width is less than or equal to 0.</exception>
        public static SizeConstraint Width(int width)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Width must be greater than 0.");
            }

            return new SizeConstraint(width: width);
        }

        
        /// <summary>
        /// Creates a new size constraint that makes the image resize to the specified height. This constraint maintains the aspect ratio. The resized image is never made larger than the original.
        /// </summary>
        /// <param name="height">The height of the resized image.</param>
        /// <returns>The new size constraint that makes the image resize to the specified height.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Height is less than or equal to 0.</exception>
        public static SizeConstraint Height(int height)
        {
            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height), "Height must be greater than 0.");
            }

            return new SizeConstraint(height: height);
        }


        /// <summary>
        /// Creates a new size constraint that makes the image resize to the specified width and height. This constraint does not maintain the aspect ratio. The resized image is never made larger than the original.
        /// </summary>
        /// <param name="width">The width of the resized image.</param>
        /// <param name="height">The height of the resized image.</param>
        /// <returns>The new size constraint that makes the image resize to the specified width and height.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Width or height is less than or equal to 0.</exception>
        public static SizeConstraint Size(int width, int height)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Width must be greater than 0.");
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height), "Height must be greater than 0.");
            }
            
            return new SizeConstraint(width: width, height: height);
        }


        /// <summary>
        /// Creates a new size constraint that makes the image resize to the width and height that do not exceed the specified value. This constraint maintains the aspect ratio. The resized image is never made larger than the original.
        /// </summary>
        /// <param name="maxWidthOrHeight">Maximum width or height of the resized image.</param>
        /// <returns>The new size constraint that makes the image resize to the width and height that do not exceed the specified value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Maximum width or height is less than or equal to 0.</exception>
        public static SizeConstraint MaxWidthOrHeight(int maxWidthOrHeight)
        {
            if (maxWidthOrHeight <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxWidthOrHeight), "Maximum width or height must be greater than 0.");
            }

            return new SizeConstraint(maxWidthOrHeight: maxWidthOrHeight);
        }


        /// <summary>
        /// Determines whether two <see cref="SizeConstraint"/> structures are equal.
        /// </summary>
        /// <param name="x">The <see cref="SizeConstraint"/> structure on the left side of the equality operator.</param>
        /// <param name="y">The <see cref="SizeConstraint"/> structure on the right side of the equality operator.</param>
        /// <returns>True if <paramref name="x"/> and <paramref name="y"/> have equal width, height and maximum width or height; otherwise, false.</returns>
        public static bool operator ==(SizeConstraint x, SizeConstraint y)
        {
            return ((x.mWidth == y.mWidth) && (x.mHeight == y.mHeight) && (x.mMaxWidthOrHeight == y.mMaxWidthOrHeight));
        }


        /// <summary>
        /// Determines whether two <see cref="SizeConstraint"/> structures are different.
        /// </summary>
        /// <param name="x">The <see cref="SizeConstraint"/> structure on the left side of the inequality operator.</param>
        /// <param name="y">The <see cref="SizeConstraint"/> structure on the right side of the inequality operator.</param>
        /// <returns>True if <paramref name="x"/> and <paramref name="y"/> differ in width or height or maximum width or height; otherwise, true.</returns>
        public static bool operator !=(SizeConstraint x, SizeConstraint y)
        {
            return ((x.mWidth != y.mWidth) || (x.mHeight != y.mHeight) || (x.mMaxWidthOrHeight != y.mMaxWidthOrHeight));
        }

        
        /// <summary>
        /// Determines whether the specified object is a <see cref="SizeConstraint"/> structure with the same width, height and maximum width or height as this <see cref="SizeConstraint"/> structure.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        /// <returns>True if <paramref name="other"/> is a <see cref="SizeConstraint"/> and has the same width, height and maximum width or height; otherwise, false.</returns>
        public override bool Equals(object other)
        {
            if (!(other is SizeConstraint))
            {
                return false;
            }

            var constraint = (SizeConstraint)other;

            return ((mWidth == constraint.mWidth) && (mHeight == constraint.mHeight) && (mMaxWidthOrHeight == constraint.mMaxWidthOrHeight));
        }

        
        /// <summary>
        /// Returns a hash code for this <see cref="SizeConstraint"/> structure.
        /// </summary>
        /// <returns>A hash code for this <see cref="SizeConstraint"/> structure.</returns>
        public override int GetHashCode()
        {
            return mWidth ^ mHeight ^ mMaxWidthOrHeight;
        }

        
        /// <summary>
        /// Creates a human-readable string that represents this <see cref="SizeConstraint"/> structure.
        /// </summary>
        /// <returns>A human-readable string that represents this <see cref="SizeConstraint"/> structure.</returns>
        public override string ToString()
        {
            return String.Format("Width={0:D}, Height={1:D}, MaxWidthOrHeight={2:D}", mWidth, mHeight, mMaxWidthOrHeight);
        }
    }
}
