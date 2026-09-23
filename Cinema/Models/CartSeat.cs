using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbsoluteCinema.Models
{
    public class CartSeat
    {
        public int Id { get; set; }

        [Required]
        public int CartId { get; set; }
        public Cart Cart { get; set; }

        [Required]
        public int SeatId { get; set; }
        public Seat Seat { get; set; }

       
        [Required]
        public int MovieId { get; set; }
    }
}