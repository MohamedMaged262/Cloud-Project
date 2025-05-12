using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace ZA_PLACE.ViewModel

{
    public class NewsViewModel
    {
        [Key]
        public Guid NewsId { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Please Add News Title")]
        [StringLength(255, ErrorMessage = "The Full Name must be at least 1 and at most 255 characters long.")]
        public string? NewsTitle { get; set; }

        [Required(ErrorMessage = "Please Add News Description")]
        [StringLength(5000, ErrorMessage = "The Full Name must be at least 1 and at most 5000 characters long.")]
        public string? NewsDescription { get; set; }

        [StringLength(5000, ErrorMessage = "The Full Name must be at least 1 and at most 5000 characters long.")]
        public string? NewsPhotoPath { get; set; }

        [Required]
        public DateTime? CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }
        [NotMapped]
        public IFormFile? clientFile { get; set; }
    }
}
