using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZA_PLACE.Models;

namespace ZA_PLACE.ViewModel
{
    public class AssignmentViewModel
    {
        [Key]
        public Guid AssignmentId { get; set; }
        [Required(ErrorMessage = "Please Enter Assignment Name.")]
        [StringLength(100, ErrorMessage = "The Assignment Name must be at least 1 and at most 100 characters long.")]
        public string AssignmentName { get; set; }
        [Required(ErrorMessage = "Please Enter Assignment Description.")]
        [StringLength(2500, ErrorMessage = "The Assignment Description must be at least 1 and at most 100 characters long.")]
        public string AssignmentDescription { get; set; }
        [Required(ErrorMessage = "Please Enter Assignment Link.")]
        public string AssignmentLink { get; set; }
        [Required]
        public bool AssignmentStatus { get; set; }
        [Required(ErrorMessage = "Please Enter The Assignment DueDate.")]
        public DateOnly AssignmentDueDate { get; set; }
        public string? CreatedBy { get; set; }
        [Required]
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
