using AbsoluteCinema.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbsoluteCinema.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.TotalPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(o => o.Status)
                .IsRequired();

            builder.HasOne(o => o.ApplicationUser)
                .WithMany() 
                .HasForeignKey(o => o.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.Movie)
                .WithMany() 
                .HasForeignKey(o => o.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

           
            builder.HasMany(o => o.Seats)
                .WithOne() 
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}