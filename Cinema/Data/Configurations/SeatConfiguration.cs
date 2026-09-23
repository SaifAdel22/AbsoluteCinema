using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class SeatConfiguration : IEntityTypeConfiguration<Seat>
{
    public void Configure(EntityTypeBuilder<Seat> builder)
    {
        builder.HasKey(s => s.Id);

        builder.HasIndex(s => new { s.MovieId, s.SeatNumber }).IsUnique();

        builder.HasOne(s => s.Movie)
              .WithMany(m => m.Seats)   
              .HasForeignKey(s => s.MovieId)
              .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Cart)
              .WithMany(c => c.Seats)
              .HasForeignKey(s => s.CartId)
              .OnDelete(DeleteBehavior.SetNull);
    }
}