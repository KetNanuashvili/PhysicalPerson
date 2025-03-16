using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class PhoneNumber
    {
        public int Id { get; set; }
        public PhoneNumberType Type { get; set; }
        public string Number { get; set; }

        public int PhysicalPersonId { get; set; }

        public PhysicalPerson PhysicalPerson { get; set; }

    }
    public enum PhoneNumberType
    {
        Home ,
        Work,
        Mobile
    }
}