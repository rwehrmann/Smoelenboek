using Centric.Learning.Smoelenboek.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Centric.Learning.Smoelenboek.Business.Storage
{
    /// <summary>
    /// People Repository interface
    /// </summary>
    public interface IPeopleRepository
    {
        /// <summary>
        /// Get or search persons by their name
        /// </summary>
        /// <param name="search">search term</param>
        /// <returns>List of Persons</returns>
        Task<List<Person>> GetPeopleAsync(string search = null);

        /// <summary>
        /// Get a person by its id
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>Person</returns>
        Task<Person> GetPersonAsync(int id);

        /// <summary>
        /// Add a person (id not filled)
        /// </summary>
        /// <param name="person">Person</param>
        /// <returns></returns>
        Task AddPersonAsync(Person person);

        /// <summary>
        /// Edit / update as person (id filled)
        /// </summary>
        /// <param name="person">Person</param>
        /// <returns></returns>
        Task EditPersonAsync(Person person);

        /// <summary>
        /// Delete a person (id filled)
        /// </summary>
        /// <param name="person">Person</param>
        /// <returns></returns>
        Task DeletePersonAsync(Person person);
    }
}