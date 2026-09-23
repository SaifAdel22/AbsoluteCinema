using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbsoluteCinema.Models
{
    public class Promotion
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Promotion code is required.")]
        [StringLength(50)]
        [Display(Name = "Promo Code")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Discount is required.")]
        [Range(1, 100, ErrorMessage = "Discount must be between 1 and 100.")]
        [Display(Name = "Discount (%)")]
        public decimal Discount { get; set; }

        [Display(Name = "Max Usage Limit")]
        public int MaxUsage { get; set; }

        [Display(Name = "Times Used")]
        public int TimesUsed { get; set; } = 0;

        // اكتفينا بـ ValidTo فقط كما طلبته
        [Required]
        [Display(Name = "Valid To")]
        public DateTime ValidTo { get; set; } = DateTime.Now.AddMonths(1);

        [Display(Name = "Applicable Movie")]
        public int? MovieId { get; set; }
        [ForeignKey("MovieId")]
        public Movie? Movie { get; set; }

        [Display(Name = "Is Active")]
        public bool Status { get; set; } = true;
    }
}