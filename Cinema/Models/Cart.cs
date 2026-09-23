using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbsoluteCinema.Models
{
    public class Cart
    {
        public int Id { get; set; }

        [Required]
        public string ApplicationUserId { get; set; } = string.Empty;

        [Required]
        public int MovieId { get; set; }
        [ForeignKey("MovieId")]
        public Movie Movie { get; set; } = null!;

        [Required]
        [Range(1, 100, ErrorMessage = "Number of tickets must be at least 1.")]
        public int Count { get; set; } = 1;

        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentPrice { get; set; }

        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    }
}