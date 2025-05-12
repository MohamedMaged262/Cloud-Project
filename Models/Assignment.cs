using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZA_PLACE.Models
{
    public class Assignment
    {
        [Key]
        public Guid AssignmentId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Assignment Name")]
        public string AssignmentName { get; set; }

        [Required]
        [StringLength(2500)]
        [Display(Name = "Assignment Description")]
        public string AssignmentDescription { get; set; }

        [Required]
        [Display(Name = "Assignment Link")]
        public string AssignmentLink { get; set; }

        [Required]
        [Display(Name = "Assignment Status")]
        public bool AssignmentStatus { get; set; }

        [Required]
        [Display(Name = "Assignment DueDate")]
        public DateOnly AssignmentDueDate { get; set; }

        [Required]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Required]
        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }

        // Foreign Key to Course
        [Required]
        [Display(Name = "Course")]
        [ForeignKey("Course")]
        public Guid CourseId { get; set; }

        // Navigation property
        public Course? Course { get; set; }
    }
}
