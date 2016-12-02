using System.Collections.Generic;

using DancingGoat.Models.Contacts;

namespace DancingGoat.Models.Cafes
{
    public class IndexViewModel
    {
        public IEnumerable<CafeModel> CompanyCafes { get; set; }


        public Dictionary<string, List<ContactModel>> PartnerCafes { get; set; }
    }
}