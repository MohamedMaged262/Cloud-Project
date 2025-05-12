using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZA_PLACE.Models
{
    public class CategoryViewModel
    {
        [Key]
        public Guid CategoryId { get; set; }

        [Required(ErrorMessage = "Please Add Category Title")]
        [StringLength(100, ErrorMessage = "The Category Name must be at least 1 and at most 100 characters long.")]
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; }

        [StringLength(5000)]
        [Display(Name = "Category Image Path")]
        public string? CategoryImagePath { get; set; }

        [Required]
        public bool CategoryStatus { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        public DateTime? UpdatedOn { get; set; }
        public ICollection<Course>? Courses { get; set; } = new List<Course>();

        [NotMapped] 
        public IFormFile? clientFile { get; set; }
    }
}

