using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using ZA_PLACE.Models;
using Newtonsoft.Json.Serialization;

namespace ZA_PLACE.Models
{
    public class Content
    {
        [Key]
        public Guid ContentId { get; set; }
        [Required(ErrorMessage="Please Enter Content Name")]
        [StringLength(100, ErrorMessage= "The Content Name must be at least 1 and at most 100 characters long.")]
        [Display(Name = "Content Name")]
        public string ContentName { get; set; }
        [Required(ErrorMessage = "Please Enter Content Description")]
        [StringLength(2500, ErrorMessage = "The Content Description must be at least 1 and at most 2500 characters long.")]
        [Display(Name = "Content Description")]
        public string ContentDescription { get; set; }
        [Required(ErrorMessage = "Please Enter Content Price")]
        [StringLength(10000, ErrorMessage = "The Content Price must be in Egp and not exceed 10000")]
        [Display(Name = "Content Price")]
        public string ContentPrice { get; set; } // Ensure this is a string
        [Required(ErrorMessage = "Please Enter Content Meeting Link")]
        [StringLength(2500, ErrorMessage = "The Content Meeting Link must be at least 1 and at most 2500 characters long.")]
        [Display(Name = "Content Meeting Link")]
        public string ContentMeetingLink { get; set; }
        
        public string? ContentDate { get; set; }

        [Required]
        [Display(Name = "Content Status")]
        public bool ContentStatus { get; set; }
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
        [DisplayName("Course")]
        [ForeignKey("Course")]
        public Guid CourseId { get; set; }
        public Course? Course { get; set; }
        public ICollection<Resource>? Resources { get; set; }
    }
}
