using Centric.Learning.Smoelenboek.Business;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Centric.Learning.Smoelenboek.Storage
{
    public class PeopleDbContext: DbContext
    {
        public PeopleDbContext()
            : base()
        {
            // Database.SetInitializer(new MigrateDatabaseToLatestVersion<PeopleDbContext, AzureWorkshop.Smoelenboek.Web.Migrations.Configuration>());
        }

        public PeopleDbContext(DbContextOptions<PeopleDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // only to enable migrations
            if (optionsBuilder.Options.FindExtension<Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal.SqlServerOptionsExtension>() == null)
            {
                optionsBuilder
                    .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=aspnet-Centric.Learning.Smoelenboek-A4CDB26E-944A-4258-8BD3-C24C4271BC91;Trusted_Connection=True;MultipleActiveResultSets=true");
            }

            optionsBuilder
                .EnableSensitiveDataLogging();
        }

        public DbSet<Person> People { get; set; }
    }
}