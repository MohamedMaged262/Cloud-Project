using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZA_PLACE.Models
{
    public class Course
    {
        [Key]
        public Guid CourseId { get; set; }

        [Required(ErrorMessage = "Please Add Course Name")]
        [StringLength(100, ErrorMessage = "The Course Name must be at least 1 and at most 100 characters long.")]
        [Display(Name = "Course Name")]
        public string CourseName { get; set; }

        [Required(ErrorMessage = "Please Add Course Description")]
        [StringLength(2500, ErrorMessage = "The Course Description must be at least 1 and at most 2500 characters long.")]
        [Display(Name = "Course Description")]
        public string CourseDescription { get; set; }
        [Display(Name = "Course Meeting Link")]
        public string? CourseMeetingLink { get; set; }
        [Display(Name = "Course Image")]
        public string? CourseImagePath { get; set; }
        [Display(Name = "Course Video")]
        public string? CourseVideoPath { get; set; }

        [Required]
        [Display(Name = "Course Status")]
        public bool CourseStatus { get; set; }
        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }

        [Required]
        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        [Display(Name = "Updated By")]
        public string? UpdatedBy { get; set; }
        [Display(Name = "Updated On")]
        public DateTime? UpdatedOn { get; set; }

        [Required]
        [DisplayName("Category")]
        [ForeignKey("Category")]
        public Guid CategoryId { get; set; }

        public Category? Category { get; set; }

        [Required]
        [DisplayName("Age")]
        [ForeignKey("Age")]
        public Guid AgeId { get; set; }

        public Age? Age { get; set; }

        [NotMapped]
        public IFormFile? clientVideo { get; set; }

        [NotMapped]
        public IFormFile? clientFile { get; set; }
        public ICollection<Content>? Contents { get; set; } 
    }
}
