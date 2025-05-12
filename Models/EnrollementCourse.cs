using System.ComponentModel.DataAnnotations;

namespace ZA_PLACE.Models
{
    public class EnrollementCourse
    {
        [Key]
        public Guid EnrollementCourseId { get; set; }
        [Required]
        public Guid CourseId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public bool EnrollementStatus { get; set; }
        public DateTime EnrolledOn { get; set; }
    }
}
