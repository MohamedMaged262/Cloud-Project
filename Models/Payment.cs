using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZA_PLACE.Models
{
    public class Payment
    {
        [Key]
        public Guid PaymentId { get; set; }
        [Required]
        [StringLength(255)]
        [Display(Name = "Payment Reason")]
        public string PaymentReason { get; set; }
        [Required]
        [StringLength(255)]
        [Display(Name = "Payment Type")]
        public string PaymentType { get; set; }
        [Required]
        [Display(Name = "Payment Status")]
        public bool PaymentStatus { get; set; }
        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }
        [Required]
        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string? PaymentImgPath { get; set; }
        [NotMapped]
        public IFormFile? clientFile { get; set; }
    }
}
