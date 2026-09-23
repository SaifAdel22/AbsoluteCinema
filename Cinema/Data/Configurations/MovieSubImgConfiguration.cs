using AbsoluteCinema.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbsoluteCinema.Data.Configurations
{
    public class MovieSubImgConfiguration : IEntityTypeConfiguration<MovieSubImg>
    {
        public void Configure(EntityTypeBuilder<MovieSubImg> builder)
        {
            builder.HasKey(img => img.Id);

            builder.HasOne(img => img.Movie)
                   .WithMany(m => m.SubImgs)
                   .HasForeignKey(img => img.MovieId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}