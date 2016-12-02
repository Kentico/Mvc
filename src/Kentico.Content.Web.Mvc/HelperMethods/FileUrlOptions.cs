namespace Kentico.Content.Web.Mvc
{
    /// <summary>
    /// Represents options that affect the file URL.
    /// </summary>
    public sealed class FileUrlOptions
    {
        /// <summary>
        /// Gets or sets a value that indicates whether the web browser will require some form of action from the user to open it.
        /// </summary>
        public bool AttachmentContentDisposition
        {
            get;
            set;
        }
    }
}
