using Centric.Learning.Smoelenboek.Business;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Centric.Learning.Smoelenboek.Storage.People.TableStorage
{
    class PersonEntity : TableEntity
    {
        public int PersonId { get; set; }
        public string Name { get; set; }
        public string Nationality { get; set; }
        public int Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public string Image { get; set; }

        public PersonEntity()
        {

        }

        public PersonEntity(Person person)
        {
            this.PersonId = person.PersonId;
            this.Name = person.Name;
            this.Nationality = person.Nationality;
            this.Gender = (int)person.Gender;
            this.BirthDate = person.BirthDate;
            this.Image = person.Image;

            this.PartitionKey = "people";
            this.RowKey = this.PersonId.ToString();
        }

        public Person ToPerson()
        {
            return new Person
            {
                PersonId = this.PersonId,
                Name = this.Name,
                Nationality = this.Nationality,
                Gender = (Gender)this.Gender,
                BirthDate = this.BirthDate,
                Image = this.Image
            };
        }
    }

}
