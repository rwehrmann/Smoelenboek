using Centric.Learning.Smoelenboek.Business;
using Centric.Learning.Smoelenboek.Business.Storage;
using Centric.Learning.Smoelenboek.Storage.People.TableStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Centric.Learning.Smoelenboek.Storage
{
    /// <summary>
    /// Table storage implementation of IPeopleRepository
    /// </summary>
    public class TableStoragePeopleRepository : IPeopleRepository, IDisposable
    {
        private CloudTable table = null;
        public TableStoragePeopleRepository(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("StorageConnection");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            table = tableClient.GetTableReference("People");

            if (!table.ExistsAsync().Result)
            {
                table.CreateIfNotExistsAsync().Wait();
            }
        }

        public async Task AddPersonAsync(Person person)
        {
            if (person.PersonId == 0)
            {
                List<Person> persons = await GetPeopleAsync();
                person.PersonId = persons.Count == 0 ? 1 : persons.Max(p => p.PersonId) + 1;
            }
            TableOperation operation = TableOperation.Insert(new PersonEntity(person));
            await table.ExecuteAsync(operation);
        }

        public Task DeletePersonAsync(Person person)
        {
            TableOperation operation = TableOperation.Delete(new PersonEntity(person));
            throw new NotImplementedException();
        }

        public async Task EditPersonAsync(Person person)
        {
            TableOperation operation = TableOperation.Replace(new PersonEntity(person));
            await table.ExecuteAsync(operation);
        }

        public async Task<List<Person>> GetPeopleAsync(string search = null)
        {
            TableQuery<PersonEntity> query = new TableQuery<PersonEntity>();

            IList<PersonEntity> peopleEntities = await table.ExecuteQueryAsync<PersonEntity>(query);
            List<Person> people = peopleEntities.Select(e => e.ToPerson()).ToList();
            if (!string.IsNullOrWhiteSpace(search))
            {
                people = people.Where(p => p.Name.ToLower().Contains(search.ToLower())).ToList();
            }

            return people;
        }

        public async Task<Person> GetPersonAsync(int id)
        {
            TableOperation operation = TableOperation.Retrieve<PersonEntity>("people", id.ToString());
            TableResult result = await table.ExecuteAsync(operation);
            if (result.Result == null)
            {
                return null;
            }
            PersonEntity person = result.Result as PersonEntity;
            return person.ToPerson();
        }

        public void Dispose()
        {
            // throw new NotImplementedException();
        }
    }
}
