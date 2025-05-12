using System.ComponentModel.DataAnnotations;

namespace ZA_PLACE.Models
{
    public class Age
    {
        [Key]
        public Guid AgeId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Age From To")]
        public string AgeFromTO { get; set; }

        [Required]
        [Display(Name = "Age Status")]
        public bool AgeStatus { get; set; }

        [Required]
        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "Updated On")]
        public DateTime? UpdatedOn { get; set; }

        [Display(Name = "Course Link Id")]
        public ICollection<Course>? Courses { get; set; }
    }
}
