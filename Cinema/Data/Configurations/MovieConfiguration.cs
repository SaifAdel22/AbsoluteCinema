using AbsoluteCinema.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbsoluteCinema.Data.Configurations
{
	public class MovieConfiguration : IEntityTypeConfiguration<Movie>
	{
		public void Configure(EntityTypeBuilder<Movie> builder)
		{
			builder.HasKey(m => m.Id);
			builder.Property(m => m.Name).IsRequired().HasMaxLength(200);
			builder.Property(m => m.Price).HasColumnType("decimal(18,2)");

			builder.HasOne(m => m.Category)
				   .WithMany(c => c.Movies)
				   .HasForeignKey(m => m.CategoryId);

			builder.HasOne(m => m.Cinema)
				   .WithMany(c => c.Movies)
				   .HasForeignKey(m => m.CinemaId);
		}
	}
}