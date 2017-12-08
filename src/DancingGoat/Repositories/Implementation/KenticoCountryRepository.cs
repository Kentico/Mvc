using System.Collections.Generic;

using CMS.Globalization;

namespace DancingGoat.Repositories.Implementation
{
    /// <summary>
    /// Represents a collection of countries and states.
    /// </summary>
    public class KenticoCountryRepository : ICountryRepository
    {
        /// <summary>
        /// Returns all available countries.
        /// </summary>
        /// <returns>Collection of all available countries</returns>
        public IEnumerable<CountryInfo> GetAllCountries()
        {
            return CountryInfoProvider.GetCountries();
        }


        /// <summary>
        /// Returns the country with the specified ID.
        /// </summary>
        /// <param name="countryId">The identifier of the country.</param>
        /// <returns>The country with the specified ID, if found; otherwise, null.</returns>
        public CountryInfo GetCountry(int countryId)
        {
            return CountryInfoProvider.GetCountryInfo(countryId);
        }


        /// <summary>
        /// Returns the country with the specified code name.
        /// </summary>
        /// <param name="countryName">The code name of the country.</param>
        /// <returns>The country with the specified code name, if found; otherwise, null.</returns>
        public CountryInfo GetCountry(string countryName)
        {
            return CountryInfoProvider.GetCountryInfo(countryName);
        }

        
        /// <summary>
        /// Returns all states in country with given ID.
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <returns>Collection of all states in county.</returns>
        public IEnumerable<StateInfo> GetCountryStates(int countryId)
        {
            return StateInfoProvider.GetStates().WhereEquals("CountryID", countryId);
        }


        /// <summary>
        /// Returns the state with the specified code name.
        /// </summary>
        /// <param name="stateName">The code name of the state.</param>
        /// <returns>The state with the specified code name, if found; otherwise, null.</returns>
        public StateInfo GetState(string stateName)
        {
            return StateInfoProvider.GetStateInfo(stateName);
        }
    }
}