using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Centric.Learning.Smoelenboek.Business
{
    /// <summary>
    /// A person
    /// </summary>
    public class Person
    {
        /// <summary>
        /// Identifier
        /// </summary>
        public int PersonId { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [DisplayName("Name")]
        public string Name { get; set; }
        /// <summary>
        /// Nationality
        /// </summary>
        [DisplayName("Nationality")]
        public string Nationality { get; set; }
        /// <summary>
        /// Birthdate
        /// </summary>
        [DisplayName("Date of Birth")]
        [DisplayFormat(DataFormatString = "{0:d-M-yyyy}")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        /// <summary>
        /// Gender
        /// </summary>
        [DisplayName("Gender")]
        public Gender Gender { get; set; }
        /// <summary>
        /// Image name
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// Url to image, created by using IPhotoRepository
        /// </summary>
        [NotMapped]
        public string ImageUrl
        {
            get
            {
                return $"/Home/Photos/{PersonId}";
            }
        }

    }
}