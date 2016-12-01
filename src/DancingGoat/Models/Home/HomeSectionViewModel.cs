using System.Linq;

using CMS.DocumentEngine.Types.DancingGoatMvc;
using Kentico.Content.Web.Mvc;

namespace DancingGoat.Models.Home
{
    public class HomeSectionViewModel
    {
        public string BackgroundImagePath { get; set; }
        public string Heading { get; set; }
        public string Text { get; set; }
        public string MoreButtonText { get; set; }
        public string MoreButtonUrl { get; set; }

        public static HomeSectionViewModel GetViewModel(HomeSection homeSection)
        {
            return homeSection == null ? null : new HomeSectionViewModel 
            {
                BackgroundImagePath = homeSection.Fields.Image.GetPath(),
                Heading = homeSection.Fields.Heading,
                Text = homeSection.Fields.Text,
                MoreButtonText = homeSection.Fields.LinkText,
                MoreButtonUrl = homeSection.Fields.Link.FirstOrDefault()?.RelativeURL
            };
        }
    }
}