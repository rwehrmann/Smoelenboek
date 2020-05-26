using System;
using Xunit;

namespace Centric.Learning.Smoelenboek.Business.Tests
{
    public class PersonTests
    {
        public class PersonIdTests : PersonTests
        {
            [Theory(DisplayName = "PersonId")]
            [InlineData(int.MinValue)]
            [InlineData(-10)]
            [InlineData(-1)]
            [InlineData(0)]
            [InlineData(1)]
            [InlineData(10)]
            [InlineData(int.MaxValue)]
            public void WhenSet_ReturnsSetValue(int value)
            {
                // arrange
                var sut = new Person { PersonId = value };

                // act
                var result = sut.PersonId;

                // assert
                Assert.Equal(value, result);
            }
        }

        public class NameTests : PersonTests
        {
            [Theory(DisplayName = "Name")]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("testline")]
            [InlineData("123456789101112131415161718192021222324252627282930313233343536373839404142")]
            public void WhenSet_ReturnsSetValue(string value)
            {
                // arrange
                var sut = new Person { Name = value };

                // act
                var result = sut.Name;

                // assert
                Assert.Equal(value, result);
            }
        }

        public class NatonalityTests : PersonTests
        {
            [Theory(DisplayName = "Nationality")]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("testline")]
            [InlineData("123456789101112131415161718192021222324252627282930313233343536373839404142")]
            public void WhenSet_ReturnsSetValue(string value)
            {
                // arrange
                var sut = new Person { Nationality = value };

                // act
                var result = sut.Nationality;

                // assert
                Assert.Equal(value, result);
            }
        }

        public class BirthDateTests : PersonTests
        {
            /// <summary>
            /// edge cases for datetime, since we cannot supply these using inlineData
            /// </summary>
            public static TheoryData<DateTime> MemberData = new TheoryData<DateTime>
            {
                DateTime.MinValue,
                new DateTime(1782,9,5),
                DateTime.UnixEpoch,
                new DateTime(1900,1,1),
                new DateTime(2000,1,1),
                DateTime.Today,
                DateTime.MaxValue
            };

            [Theory(DisplayName = "BirthDate")]
            [MemberData(nameof(MemberData))]
            public void WhenSet_ReturnsSetValue(DateTime value)
            {
                // arrange
                var sut = new Person { BirthDate = value };

                // act
                var result = sut.BirthDate;

                // assert
                Assert.Equal(value, result);
            }
        }

        public class GenderTests : PersonTests
        {
            [Theory(DisplayName = "Gender")]
            [InlineData(Gender.Female)]
            [InlineData(Gender.Male)]
            [InlineData(Gender.Neutral)]
            public void WhenSet_ReturnsSetValue(Gender value)
            {
                // arrange
                var sut = new Person { Gender = value };

                // act
                var result = sut.Gender;

                // assert
                Assert.Equal(value, result);
            }
        }

        public class ImageTests : PersonTests
        {
            [Theory(DisplayName = "Image")]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("testline")]
            [InlineData("123456789101112131415161718192021222324252627282930313233343536373839404142")]
            public void WhenSet_ReturnsSetValue(string value)
            {
               // arrange
               var sut = new Person { Image = value };

               // act
               var result = sut.Image;

               // assert
               Assert.Equal(value, result);
            }
        }

        public class ImageUrlTests : PersonTests
        {
            [Theory(DisplayName = "ReturnsPhotoUrl")]
            [InlineData(int.MinValue)]
            [InlineData(-10)]
            [InlineData(-1)]
            [InlineData(0)]
            [InlineData(1)]
            [InlineData(10)]
            [InlineData(int.MaxValue)]
            public void WhenCalled_ReturnsRelativePhotoUrlContainingPersonId(int id)
            {
                // arrange
                var expectedResult = $"/Home/Photos/{id}";
                var sut = new Person { PersonId = id };

                // act
                var result = sut.ImageUrl;

                // assert
                Assert.Equal(expectedResult, result);
            }
        }
    }
}
