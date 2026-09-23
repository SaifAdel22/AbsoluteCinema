using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbsoluteCinema.Models
{
    public enum OrderStatus
    {
        Paid = 1,        
        Pending = 0,   
        Cancelled = 2,  
        Refunded = 3      
    }

    public class Order : Audit
    {
        public int Id { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
        public string? SessionId { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    }
}