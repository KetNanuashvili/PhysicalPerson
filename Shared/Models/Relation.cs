namespace Project.Models
{
    public class Relation
    {
        public int FromId { get; set; }
        public int ToId { get; set; }
        public RelationType RelationType { get; set; }
        public virtual PhysicalPerson RelatedFrom { get; set; }
        public virtual PhysicalPerson RelatedTo { get; set; }

    }
    public enum RelationType
    {
        Colleague ,
        Friend,
        Relative,
        Other
    }
}