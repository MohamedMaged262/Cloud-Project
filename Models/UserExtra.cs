using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZA_PLACE.Models
{
    public class UserExtra : IdentityUser
    {
        //[Key]
        //public Guid UserId { get; set; }
        [Required]
        [StringLength(250)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public bool Gender { get; set; }

        [Required]
        [Display(Name = "User Status")]
        public bool UserStatus { get; set; }

        [Required]
        [Display(Name = "Date Of Birth")]
        public DateTime DoB { get; set; }

        [Display(Name = "Profile Picture")]
        public string? ProfilePicture { get; set; }

        [Display(Name = "About Description")]
        [StringLength(2500)]
        public string? AboutDescription { get; set; }

        [Display(Name = "Parent Phone Number")]
        [StringLength(12)]
        public string? ParentPhoneNumber { get; set; }

        [EmailAddress]
        [StringLength(250)]
        [Display(Name = "Parent Email Address")]
        public string? ParentEmailAddress { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        [Required]
        public int Permision { get; set; }
        public string? RoleName { get; set; }
        [NotMapped]
        public IFormFile? clientFile { get; set; }
    }
}