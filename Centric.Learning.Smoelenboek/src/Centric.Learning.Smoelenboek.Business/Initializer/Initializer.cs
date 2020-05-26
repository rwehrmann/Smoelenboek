using Centric.Learning.Smoelenboek.Business.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Centric.Learning.Smoelenboek.Business
{
    public class Initializer
    {
        public static async Task Initialise(IPeopleRepository peopleRepository, IPhotoRepository photoRepository)
        {
            List<Person> people = await peopleRepository.GetPeopleAsync();
            if (!people.Any(p => p.Name == "Hans"))
            {
                // add Hans
                Person person = new Person
                {
                    Name = "Hans",
                    Nationality = "Netherlands",
                    Gender = Gender.Male,
                    BirthDate = new DateTime(1967, 11, 27),
                    Image = "hans.jpg"
                };
                await peopleRepository.AddPersonAsync(person);
                var thisAssembly = Assembly.GetExecutingAssembly();
                var resourcename = $"Centric.Learning.Smoelenboek.Business.Initializer.Photos.{person.Image}";
                using (var stream = thisAssembly.GetManifestResourceStream(resourcename))
                {
                    await photoRepository.UploadPhotoAsync(person.Image, stream, "image/jpg");
                }
            }

            if (!people.Any(p => p.Name == "Dick"))
            {
                // add Dick
                Person person = new Person
                {
                    Name = "Dick",
                    Nationality = "Netherlands",
                    Gender = Gender.Male,
                    BirthDate = new DateTime(1969, 7, 1),
                    Image = "dick.jpg"
                };
                await peopleRepository.AddPersonAsync(person);
                var thisAssembly = Assembly.GetExecutingAssembly();
                var resourcename = $"Centric.Learning.Smoelenboek.Business.Initializer.Photos.{person.Image}";
                using (var stream = thisAssembly.GetManifestResourceStream(resourcename))
                {
                    await photoRepository.UploadPhotoAsync(person.Image, stream, "image/jpg");
                }
            }
        }
    }
}
