using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace ZA_PLACE.Models
{
    public class Resource
    {
        [Key]
        public Guid ResourceId { get; set; }

        [Required(ErrorMessage = "Please Enter Resource Name")]
        [StringLength(100, ErrorMessage = "The Resource Name must be at least 1 and at most 2500 characters long.")]
        [Display(Name = "Resource Name")]
        public string ResourceName { get; set; }

        public string? ResourceFile { get; set; }

        public string? ResourceLink { get; set; }

        public DateTime? CreatedOn { get; set; } = DateTime.Now;

        [Required]
        [DisplayName("Content")]
        [ForeignKey("Content")]
        public Guid ContentId { get; set; }

        public Content? Content { get; set; }

        [NotMapped]
        public IFormFile? clientFile { get; set; }
    }
}
