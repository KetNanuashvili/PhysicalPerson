using Project.CustomValidators;
using Project.Models;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Shared.Interface;
namespace Project.commands
{
    public class CreatePersonCommandDto
    {
        [MinLength(2, ErrorMessage = "Minimum length for the Name is 2 characters"), MaxLength(50, ErrorMessage = "Custom Error Message")]
        [RegularExpression(@"^[a-zA-Z]*$", ErrorMessage = "Only Latin symbols allowed")]
        public string Name { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }

        [RegularExpression(@"^\d{11}$", ErrorMessage = "Personal ID must contain only numbers and be at most 11 digits.")]
        public string PersonalId { get; set; }

        [ValidateDateOfBirth]
        public DateTime BirthDate { get; set; }
        public CreateCityDbo? City { get; set; }


        public List<PhoneNumberDto>? PhoneNumbers { get; set; }
    }

    public class CreateCityDbo
    {
        public string Name { get; set; }
    }

    public class PhoneNumberDto
    {
        public PhoneNumberType Type { get; set; }
        public string Number { get; set; }
    }
}