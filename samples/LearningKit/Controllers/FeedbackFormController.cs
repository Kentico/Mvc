using System.Web.Mvc;

using CMS.OnlineForms.Types;

using LearningKit.Models.Form;

namespace LearningKit.Controllers
{
    public class FeedbackFormController : Controller
    {
        /// <summary>
        /// Basic action that displays the feedback form.
        /// </summary>
        public ActionResult Fill()
        {
            return View("FormFill");
        }

        /// <summary>
        /// Inserts the form data to the connected database when the feedback form is submitted.
        /// Accepts parameters posted from the feedback form via the FeedbackFormMessageModel.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendFeedback(FeedbackFormMessageModel model)
        {
            // Validates the received form data based on the view model
            if (!ModelState.IsValid)
            {
                return View("FormFill", model);
            }

            // Inserts the collected form data to the connected database
            InsertFeedbackFormItem(model);
        
            return View("FormSendSuccess");
        }

        // Inserts the collected data to the connected database (helper method)
        private void InsertFeedbackFormItem(FeedbackFormMessageModel feedback)
        {
            var item = new FeedbackFormItem
            {
                UserName = feedback.FirstName,
                UserLastName = feedback.LastName,
                UserEmail = feedback.Email,
                UserFeedback = feedback.MessageText,
            };

            item.Insert();
        }
    }
}