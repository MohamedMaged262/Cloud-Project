using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZA_PLACE.Models;

namespace ZA_PLACE.ViewModel
{
    public class AnnouncementViewModel
    {
        [Key]
        public Guid AnnouncementId { get; set; }
        [Required(ErrorMessage = "Please Enter Announcement Title")]
        [StringLength(100, ErrorMessage = "The Announcement Title must be at least 1 and at most 100 characters long.")]
        public string AnnouncementTitle { get; set; }
        [Required(ErrorMessage = "Please Enter Announcement Description")]
        [StringLength(2500, ErrorMessage = "The Announcement Description must be at least 1 and at most 100 characters long.")]
        public string AnnouncementDescription { get; set; }
        public string? CreatedBy { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        // Foreign Key to Course
        [Required]
        [Display(Name = "Course")]
        [ForeignKey("Course")]
        public Guid CourseId { get; set; }

        // Navigation property
        public Course? Course { get; set; }
    }
}
