using System;
using System.Globalization;
using System.Text;
using System.Web.Mvc;

using CMS.DocumentEngine;
using CMS.Helpers;

namespace Kentico.Web.Mvc
{
    /// <summary>
    /// Provides methods to generate fully qualified URLs to attachments.
    /// </summary>
    public static class UrlHelperAttachmentMethods
    {
        /// <summary>
        /// Generates a fully qualified URL to the specified attachment.
        /// </summary>
        /// <param name="instance">The object that provides methods to build URLs to Kentico content.</param>
        /// <param name="attachment">The attachment.</param>
        /// <param name="options">Options that affect the attachment URL.</param>
        /// <returns>The fully qualified URL to the attachment.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> or <paramref name="attachment"/> is null.</exception>
        public static string Attachment(this ExtensionPoint<UrlHelper> instance, Attachment attachment, AttachmentUrlOptions options = null)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            if (attachment == null)
            {
                throw new ArgumentNullException("attachment");
            }

            return GenerateAttachmentUrl(instance, attachment, SizeConstraint.Empty, options);
        }


        /// <summary>
        /// Generates a fully qualified URL to the specified attachment. If the attachment is an image, the URL points to a resized version.
        /// </summary>
        /// <param name="instance">The object that provides methods to build URLs to Kentico content.</param>
        /// <param name="attachment">The attachment.</param>
        /// <param name="constraint">The size constraint that is enforced on image when resizing.</param>
        /// <param name="options">Options that affect the attachment URL.</param>
        /// <returns>The fully qualified URL to the attachment.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> or <paramref name="attachment"/> is null.</exception>
        public static string Attachment(this ExtensionPoint<UrlHelper> instance, Attachment attachment, SizeConstraint constraint, AttachmentUrlOptions options = null)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            if (attachment == null)
            {
                throw new ArgumentNullException("attachment");
            }

            return GenerateAttachmentUrl(instance, attachment, constraint, options);
        }


        private static string GenerateAttachmentUrl(ExtensionPoint<UrlHelper> instance, Attachment attachment, SizeConstraint constraint, AttachmentUrlOptions options)
        {
            var fileName = GetFileName(attachment);
            var builder = new StringBuilder().AppendFormat("~/getattachment/{0:D}/{1}", attachment.GUID, GetFileNameForUrl(fileName));
            var referenceLength = builder.Length;
            Action<string, object> append = (name, value) =>
            {
                builder.Append(builder.Length == referenceLength ? '?' : '&').Append(name).Append('=').Append(value);
            };

            if (constraint.WidthComponent > 0)
            {
                append("width", constraint.WidthComponent);
            }

            if (constraint.HeightComponent > 0)
            {
                append("height", constraint.HeightComponent);
            }

            if (constraint.MaxWidthOrHeightComponent > 0)
            {
                append("maxsidesize", constraint.MaxWidthOrHeightComponent);
            }

            // Prevent Kentico from using site settings
            if (!constraint.IsEmpty)
            {
                append("resizemode", "force");
            }

            if ((options != null) && options.AttachmentContentDisposition)
            {
                append("disposition", "attachment");
            }

            var url = builder.ToString();

            if (attachment.VersionID > 0)
            {
                url = GetAttachmentPreviewUrl(url);
            }

            return instance.Target.Content(url);
        }


        private static string GetAttachmentPreviewUrl(string url)
        {
            if (VirtualContext.IsPreviewLinkInitialized)
            {
                // Add hash of attachment URL for validation
                url = VirtualContext.AddPreviewHash(url);

                // Add prefix of virtual context
                url = url.Insert(1, VirtualContext.CurrentURLPrefix);
            }

            return url;
        }


        private static string GetFileName(Attachment attachment)
        {
            if (!String.IsNullOrEmpty(attachment.Name))
            {
                return attachment.Name;
            }

            return "attachment";
        }


        private static string GetFileNameForUrl(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            var builder = new StringBuilder();

            // Remove characters that prevent normalization and use compatibility normalization to decompose certain code points, e.g. ligatures into the constituent letters.
            fileName = RemoveCharacters(fileName, UnicodeCategory.OtherNotAssigned, builder).Normalize(NormalizationForm.FormKD);

            // Remove characters that modify base characters, such as accents, and recompose.
            fileName = RemoveCharacters(fileName, UnicodeCategory.NonSpacingMark, builder).Normalize(NormalizationForm.FormKC);

            builder.Clear();
            var separatorFlag = false;

            foreach (var character in fileName)
            {
                if (((character >= 'a') && (character <= 'z')) || ((character >= '0') && (character <= '9')) || (character == '.') || (character == '_'))
                {
                    builder.Append(character);
                    separatorFlag = false;
                }
                else if ((character >= 'A') && (character <= 'Z'))
                {
                    builder.Append(Char.ToLowerInvariant(character));
                    separatorFlag = false;
                }
                else
                {
                    if (!separatorFlag && (builder.Length > 0))
                    {
                        builder.Append('-');
                        separatorFlag = true;
                    }
                }
            }

            // Remove the ending hyphen, if present.
            if (separatorFlag)
            {
                builder.Length = builder.Length - 1;
            }

            return builder.ToString();
        }


        private static string RemoveCharacters(string text, UnicodeCategory category, StringBuilder builder)
        {
            builder.Clear();
            foreach (var character in text)
            {
                if (Char.GetUnicodeCategory(character) != category)
                {
                    builder.Append(character);
                }
            }

            return builder.ToString();
        }
    }
}
