using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using ZA_PLACE.Models;
using ZA_PLACE.ViewModel;
using ZAPLACE.ViewModel;

namespace ZA_PLACE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            // Fetch the last 8 rows from the Courses table where CourseStatus is true
            var lastCourses = _db.Courses
                                 .Where(c => c.CourseStatus == true) // Filter by active courses
                                 .OrderByDescending(c => c.CourseId)
                                 .Take(8)
                                 .ToList();

            // Fetch the last 4 rows from the Categories table where CategoryStatus is true
            var lastCategories = _db.Categories
                                    .Where(cat => cat.CategoryStatus == true) // Filter by active categories
                                    .OrderByDescending(cat => cat.CategoryId)
                                    .Take(4)
                                    .ToList();

            // Pass the data to the view using the ViewModel
            var model = new LastCourseCategoryViewModel
            {
                LastCourse = lastCourses,
                LastCategory = lastCategories
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Extra Pages
        public IActionResult AboutUsExtra()
        {
            return View();
        }

        public IActionResult ContactUsExtra()
        {
            return View();
        }

        public IActionResult InstructorExtra()
        {
            return View();
        }

        public IActionResult StudentExtra()
        {
            return View();
        }

        // Category Page
        public IActionResult AllCategory()
        {
            // Fetch categories where CategoryStatus is true
            var activeCategories = _db.Categories
                                      .Where(c => c.CategoryStatus == true)
                                      .ToList();

            return View(activeCategories);
        }
        // Courses Page
        public IActionResult AllCourses()
        {
            // Fetch Courses where CategoryStatus is true
            var activeCourses = _db.Courses
                                      .Where(c => c.CourseStatus == true)
                                      .ToList();

            return View(activeCourses);
        }
        // News Page
        public IActionResult AllNews()
        {
            // Fetch Courses where CategoryStatus is true
            var activeNews = _db.News.ToList();

            return View(activeNews);
        }
    }
}
