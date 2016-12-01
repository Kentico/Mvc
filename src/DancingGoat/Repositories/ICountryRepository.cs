using System.Collections.Generic;

using CMS.Globalization;

using Kentico.Core.DependencyInjection;

namespace DancingGoat.Repositories
{
    /// <summary>
    /// Represents a contract for a collection of countries and states.
    /// </summary>
    public interface ICountryRepository : IRepository
    {
        /// <summary>
        /// Returns all available countries.
        /// </summary>
        /// <returns>Collection of all available countries</returns>
        IEnumerable<CountryInfo> GetAllCountries();

        
        /// <summary>
        /// Returns the country with the specified code name.
        /// </summary>
        /// <param name="countryName">The code name of the country.</param>
        /// <returns>The country with the specified code name, if found; otherwise, null.</returns>
        CountryInfo GetCountry(string countryName);


        /// <summary>
        /// Returns the country with the specified ID.
        /// </summary>
        /// <param name="countryId">The identifier of the country.</param>
        CountryInfo GetCountry(int countryId);


        /// <summary>
        /// Returns the state with the specified code name.
        /// </summary>
        /// <param name="stateName">The code name of the state.</param>
        /// <returns>The state with the specified code name, if found; otherwise, null.</returns>
        StateInfo GetState(string stateName);


        /// <summary>
        /// Returns all states in country with given ID.
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <returns>Collection of all states in county.</returns>
        IEnumerable<StateInfo> GetCountryStates(int countryId);
    }
}