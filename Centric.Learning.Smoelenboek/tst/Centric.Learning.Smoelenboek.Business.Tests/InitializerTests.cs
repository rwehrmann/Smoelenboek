using Centric.Learning.Smoelenboek.Business.Storage;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Centric.Learning.Smoelenboek.Business.Tests
{
    public class InitializerTests
    {
        public class InitialiseTests : InitializerTests
        {
            private List<Person> GetPeopleContainingPerson(string name)
            {
                return new List<Person>
                {
                    new Person
                    {
                        Name = name
                    }
                };
            }

            [Theory(DisplayName = "PeopleNotPresentAreAdded")]
            [InlineData("Hans")]
            [InlineData("Dick")]
            public void WhenPersonIsNotPresent_PersonIsAddedWithJpegImage(string name)
            {
                // arrange
                var peopleMock = new Mock<IPeopleRepository>();
                var photoMock = new Mock<IPhotoRepository>();
                var personIsAdded = false;
                var correctPhotoIsUsed = false;

                peopleMock.Setup(p => p.GetPeopleAsync(It.IsAny<string>())).ReturnsAsync(GetPeopleContainingPerson(""));
                peopleMock.Setup(p => p.AddPersonAsync(It.IsAny<Person>())).Callback((Person np) => personIsAdded |= np.Name == name);
                photoMock.Setup(p => p.UploadPhotoAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>())).Callback((string imageName, Stream f, string contentType) => correctPhotoIsUsed |= contentType == "image/jpg" && imageName.Contains(name, StringComparison.InvariantCultureIgnoreCase));

                // act
                var result = Initializer.Initialise(peopleMock.Object, photoMock.Object);

                // assert
                Assert.True(personIsAdded, $"{name} should have been added.");
                Assert.True(correctPhotoIsUsed, $"Photo should have contained {name}.");
            }

            [Theory(DisplayName = "PeoplePresentAreNotAdded")]
            [InlineData("Hans")]
            [InlineData("Dick")]
            public void WhenPersonIsPresent_DoNotAddPerson(string name)
            {
                // arrange
                var peopleMock = new Mock<IPeopleRepository>();
                var photoMock = new Mock<IPhotoRepository>();
                var personIsAdded = false;

                peopleMock.Setup(p => p.GetPeopleAsync(It.IsAny<string>())).ReturnsAsync(GetPeopleContainingPerson(name));
                peopleMock.Setup(p => p.AddPersonAsync(It.IsAny<Person>())).Callback((Person np) => personIsAdded |= np.Name == name);

                // act
                var result = Initializer.Initialise(peopleMock.Object, photoMock.Object);

                // assert
                Assert.False(personIsAdded, $"{name} should not have been added");
            }
        }
    }
}
