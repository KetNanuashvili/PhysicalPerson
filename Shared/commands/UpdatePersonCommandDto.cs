using Project.Models;
using System.ComponentModel.DataAnnotations;

namespace Shared.commands
{
    public class UpdatePersonCommandDto
    {

        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public DateTime? BirthDate { get; set; }

        public Gender? Gender { get; set; }

        public string PersonalId { get; set; }

        public UpdateCityDbo? City { get; set; }

        public List<UpdatePhoneNumberDto> PhoneNumbers { get; set; } = new List<UpdatePhoneNumberDto>();

        public string ImagePath { get; set; }
        public List<RelatedPersonDto> RelatedPersons { get; set; } = new List<RelatedPersonDto>(); 

    }

    public class UpdateCityDbo
    {
        public string Name { get; set; }
    }

    public class UpdatePhoneNumberDto
    {
        [Required]
        [Phone]
        public string Number { get; set; }

        [Required]
        public PhoneNumberType Type { get; set; }
    }

    public class RelatedPersonDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Relationship { get; set; }
    }
}
