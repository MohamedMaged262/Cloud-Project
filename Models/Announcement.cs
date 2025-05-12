using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZA_PLACE.Models
{
    public class Announcement
    {
        [Key]
        public Guid AnnouncementId { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "Announcement Title")]
        public string AnnouncementTitle { get; set; }
        [Required]
        [StringLength(2500)]
        [Display(Name = "Announcement Description")]
        public string AnnouncementDescription { get; set; }
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
