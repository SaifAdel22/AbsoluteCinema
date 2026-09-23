using AbsoluteCinema.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbsoluteCinema.Data.Configurations
{
    public class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
    {
        public void Configure(EntityTypeBuilder<Promotion> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Discount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(p => p.ValidTo)
                .IsRequired();

            // علاقة اختياري مع الفيلم (لو البروموشن خاص بفيلم معين أو عام)
            builder.HasOne(p => p.Movie)
                .WithMany()
.HasForeignKey(p => p.MovieId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}