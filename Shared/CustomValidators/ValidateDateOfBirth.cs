using System.ComponentModel.DataAnnotations;
namespace Project.CustomValidators
{
    public class ValidateDateOfBirth : ValidationAttribute
    {

        public ValidateDateOfBirth(): base ( ()=> "Your Age must be 18 or more!")
            { }
        public override bool IsValid(object value)
        {
           DateTime birthDate = (DateTime)value;
            var minimumBirthDate = DateTime.UtcNow.AddYears(-18);

            return birthDate<=minimumBirthDate;
        }
    }
}
