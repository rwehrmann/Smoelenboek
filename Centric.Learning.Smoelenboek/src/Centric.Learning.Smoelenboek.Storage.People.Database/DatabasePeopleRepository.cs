using Centric.Learning.Smoelenboek.Business;
using Centric.Learning.Smoelenboek.Business.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Centric.Learning.Smoelenboek.Storage
{
    /// <summary>
    /// SQL Server implementation of IPeopleRepository
    /// </summary>
    public class DatabasePeopleRepository : IPeopleRepository, IDisposable
    {
        private PeopleDbContext _dbContext;

        public DatabasePeopleRepository(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            DbContextOptionsBuilder<PeopleDbContext> builder = new DbContextOptionsBuilder<PeopleDbContext>();
            builder.UseSqlServer(connectionString);
            _dbContext = new PeopleDbContext(builder.Options);
            _dbContext.Database.Migrate();
        }

        public DatabasePeopleRepository(string connectionString)
        {
            DbContextOptionsBuilder<PeopleDbContext> builder = new DbContextOptionsBuilder<PeopleDbContext>();
            builder.UseSqlServer(connectionString);
            _dbContext = new PeopleDbContext(builder.Options);
            _dbContext.Database.Migrate();
        }

        public async Task AddPersonAsync(Person person)
        {
            _dbContext.People.Add(person);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeletePersonAsync(Person person)
        {
            _dbContext.People.Remove(person);
            await _dbContext.SaveChangesAsync();
        }

        public async Task EditPersonAsync(Person person)
        {
            _dbContext.Entry(person).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Person> GetPersonAsync(int id)
        {
            Person person = await _dbContext.People.FindAsync(id);
            return person;
        }

        public async Task<List<Person>> GetPeopleAsync(string search = null)
        {
            var people = _dbContext.People.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                people = people.Where(p => p.Name.ToLower().Contains(search.ToLower()));
            }

            var peopleList = await people.ToListAsync();
            return peopleList;
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}