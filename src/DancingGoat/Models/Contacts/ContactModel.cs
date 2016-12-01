namespace DancingGoat.Models.Contacts
{
    public class ContactModel
    {
        public string Name { get; set; }


        public string Phone { get; set; }


        public string Email { get; set; }


        public string ZIP { get; set; }


        public string Street { get; set; }


        public string City { get; set; }


        public string Country { get; set; }


        public string CountryCode { get; set; }


        public string State { get; set; }


        public string StateCode { get; set; }


        public ContactModel()
        {
        }


        public ContactModel(IContact contact)
        {
            Name = contact.Name;
            Phone = contact.Phone;
            Email = contact.Email;
            ZIP = contact.ZIP;
            Street = contact.Street;
            City = contact.City;
        }
    }
}