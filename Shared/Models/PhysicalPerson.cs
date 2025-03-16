using System.Reflection;

namespace Project.Models
{
    public class PhysicalPerson
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        public string PersonalId { get; set; }
        public DateTime BirthDate { get; set; }
        public City? City { get; set; }
        public List<PhoneNumber> PhoneNumbers { get; set; } = new List<PhoneNumber>();
        public string? ImagePath { get; set; }
        public ICollection<Relation> RelatedFrom { get; set; } = new List<Relation>();
        public ICollection<Relation> RelatedTo { get; set; } = new List<Relation>();

    }



public enum Gender
    {
        Male,
        Female
    }


}