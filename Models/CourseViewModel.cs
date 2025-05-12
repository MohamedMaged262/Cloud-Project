using System.Collections.Generic;

namespace ZA_PLACE.Models
{
    public class CourseViewModel
    {
        public Course Course { get; set; }
        public IEnumerable<Content> Contents { get; set; }
        public IEnumerable<Resource> Resources { get; set; }
        public List<Payment> Payments { get; set; }
        public bool IsUserEnrolled { get; set; }
    }
}
