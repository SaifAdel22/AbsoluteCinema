using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbsoluteCinema.Models
{
    public class Seat
    {
        public int Id { get; set; }

        [Required]
        public string SeatNumber { get; set; } 

        public bool IsBooked { get; set; } = false;
        public int MovieId { get; set; }
        [ForeignKey("MovieId")]
        public Movie Movie { get; set; }

        public int? CartId { get; set; }
        [ForeignKey("CartId")]
        public Cart? Cart { get; set; }

        public string? ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser? ApplicationUser { get; set; }
    }
}