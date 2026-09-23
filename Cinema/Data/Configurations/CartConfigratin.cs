using AbsoluteCinema.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbsoluteCinema.Data.Configurations
{
    public class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.ApplicationUserId)
                .IsRequired();

            builder.Property(c => c.CurrentPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // العلاقات
            builder.HasOne(c => c.Movie)
                .WithMany()
                .HasForeignKey(c => c.MovieId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}