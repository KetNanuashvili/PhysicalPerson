using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project.Infrastructure
{
    public class PhysicalPersonDbContext : DbContext
    {
        public PhysicalPersonDbContext(DbContextOptions<PhysicalPersonDbContext> options) : base(options)
        {
        }

        public DbSet<PhysicalPerson> PhysicalPerson { get; set; }
        public DbSet<City> Cities { get; internal set; }
        public DbSet<Relation> Relations { get; set; }
        public DbSet<PhoneNumber> PhoneNumbers { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Relation>()
                .HasKey(e => new { e.FromId, e.ToId });

            modelBuilder.Entity<Relation>()
        .HasOne(e => e.RelatedFrom)
        .WithMany(e => e.RelatedTo)
        .HasForeignKey(e => e.FromId)
        //როცა წაიშლება RelatedFrom ეს არ გაანადგურებს RelatedTo-ს
        .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Relation>()
                .HasOne(e => e.RelatedTo)
                .WithMany(e => e.RelatedFrom)
                .HasForeignKey(e => e.ToId)

                 //როცა წაიშლება RelatedTo ეს არ გაანადგურებს დაკავშირებული RelatedFrom-იც წაიშლება.
                 .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PhoneNumber>()
     .HasOne(p => p.PhysicalPerson)
     .WithMany(b => b.PhoneNumbers)
     .HasForeignKey(p => p.PhysicalPersonId)

//თუ პირიზკური პირი წაიშლება, მისი ტელეფონიც ავტომატურად წაიშლება
     .OnDelete(DeleteBehavior.Cascade); 

        }
    }
}
