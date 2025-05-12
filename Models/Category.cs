using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZA_PLACE.Models
{
    public class Category
    {
        [Key]
        public Guid CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; }

        [Required]
        [StringLength(5000)]
        [Display(Name = "Category Image Path")]
        public string CategoryImagePath { get; set; }

        [Required]
        public bool CategoryStatus { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        public DateTime? UpdatedOn { get; set; }

        // Initialize as empty collection if required
        public ICollection<Course>? Courses { get; set; } = new List<Course>();
    }
}