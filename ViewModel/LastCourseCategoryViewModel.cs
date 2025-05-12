using ZA_PLACE.Models;
using ZAPLACE.ViewModel;
namespace ZAPLACE.ViewModel
{
    public class LastCourseCategoryViewModel
    {
        public IEnumerable<Course> LastCourse { get; set; }
        public IEnumerable<Category> LastCategory { get; set; }
    }
}

