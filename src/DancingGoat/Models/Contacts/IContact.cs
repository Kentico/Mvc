namespace DancingGoat.Models.Contacts
{
    public interface IContact
    {
        string Name { get; }


        string Phone { get; }


        string Email { get; }


        string ZIP { get; }


        string Street { get; }


        string City { get; }


        string Country { get; }
    }
}