using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace ZA_PLACE.Models
{
    public class News
    {
        [Key]
        public Guid NewsId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(255)]
        [Display(Name = "News Title")]
        public string? NewsTitle { get; set; }

        [Required]
        [StringLength(5000)]
        [Display(Name = "News Description")]
        public string? NewsDescription { get; set; }
        [Required]
        [StringLength(2000)]
        [Display(Name = "News Photo Path")]
        public string? NewsPhotoPath { get; set; }

        [Required]
        [Display(Name = "Created On")]
        public DateTime? CreatedOn { get; set; } = DateTime.UtcNow;
        [Display(Name = "Updated On")]
        public DateTime? UpdatedOn { get; set; }
    }
}
