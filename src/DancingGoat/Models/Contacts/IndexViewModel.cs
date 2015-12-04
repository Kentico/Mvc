using System.Collections.Generic;

namespace DancingGoat.Models.Contacts
{
    public class IndexViewModel
    {
        public MessageModel Message
        {
            get;
            set;
        }


        public bool MessageSent
        {
            get;
            set;
        }


        public ContactModel CompanyContact
        {
            get;
            set;
        }


        public List<ContactModel> CompanyCafes
        {
            get;
            set;
        }
    }
}