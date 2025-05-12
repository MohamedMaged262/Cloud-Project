using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using ZA_PLACE.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ZA_PLACE.Helpers;
using System.Security.Claims; // For ClaimTypes
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services; // For UserManager


namespace ZA_PLACE.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IEmailSender _emailSender;

        public CoursesController(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment, IEmailSender emailSender)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
            _emailSender = emailSender;
        }

        public ActionResult Index()
        {
            List<Course> coursesList;

            if (User.IsInRole(clsRoles.roleAdmin))
            {
                // Admin can view all courses
                coursesList = _db.Courses.ToList();
            }
            else if (User.IsInRole(clsRoles.roleTeacher))
            {
                // Teachers can view only active courses they created
                string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); 

                coursesList = _db.Courses
                                 .Where(c => c.CourseStatus == true && c.CreatedBy == currentUserId)
                                 .ToList();
            }
            else
            {
                // Other users can view only the courses they are enrolled in
                string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                coursesList = (from c in _db.Courses
                               join e in _db.EnrollementCourses
                               on c.CourseId equals e.CourseId
                               where e.UserId == Guid.Parse(currentUserId)
                               select c).ToList();
            }

            return View(coursesList);
        }

        public ActionResult CreateCourses()
        {
            // Fetch categories and ages from the database where status is true
            var categories = _db.Categories.Where(c => c.CategoryStatus == true).ToList();
            var ages = _db.Ages.Where(a => a.AgeStatus == true).ToList();

            // Check if categories and ages are null
            if (categories == null || ages == null)
            {
                throw new InvalidOperationException("Categories or Ages are null.");
            }

            // Populate ViewBag with SelectList for categories and ages
            ViewBag.CategoryList = new SelectList(categories, "CategoryId", "CategoryName");
            ViewBag.AgeList = new SelectList(ages, "AgeId", "AgeFromTO");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateCourses(Course args)
        {
            // Get the current user's ID from claims
            args.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
            args.CourseStatus = true; // Assuming status is an int
            args.CreatedOn = DateTime.Now;

            if (ModelState.IsValid)
            {
                string fileName = string.Empty;
                string videoFileName = string.Empty;
                string uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "DatabaseFolder");
                string imagePath = string.Empty;

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                if (args.clientFile != null && args.clientFile.Length > 0)
                {
                    fileName = args.clientFile.FileName;
                    string fullPath = Path.Combine(uploadFolder, fileName);

                    using (var fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        await args.clientFile.CopyToAsync(fileStream);
                    }

                    args.CourseImagePath = $"/DatabaseFolder/{fileName}";
                }
                else
                {
                    imagePath = $"/DatabaseFolder/5.png";
                    args.CourseImagePath = imagePath;
                }

                if (args.clientVideo != null && args.clientVideo.Length > 0)
                {
                    videoFileName = args.clientVideo.FileName;
                    string videoFullPath = Path.Combine(uploadFolder, videoFileName);

                    using (var videoStream = new FileStream(videoFullPath, FileMode.Create))
                    {
                        await args.clientVideo.CopyToAsync(videoStream);
                    }

                    args.CourseVideoPath = $"/DatabaseFolder/{videoFileName}";
                }

                args.UpdatedOn = null;

                _db.Courses.Add(args);
                await _db.SaveChangesAsync();

                // Fetch the user's email address from the database
                var userEmail = await _db.Users
                    .Where(u => u.Id == args.CreatedBy)
                    .Select(u => u.Email)
                    .FirstOrDefaultAsync();

                await _emailSender.SendEmailAsync(userEmail, "Course Creation", $"{userEmail} has been successfully Created a {args.CourseName} Coruse in Za Place.");
                return RedirectToAction("Index");
            }

            // Fetch categories and ages from the database
            var categories = _db.Categories.ToList();
            var ages = _db.Ages.ToList();

            // Populate ViewBag with SelectList for categories and ages
            ViewBag.CategoryList = new SelectList(categories, "CategoryId", "CategoryName");
            ViewBag.AgeList = new SelectList(ages, "AgeId", "AgeFromTO");

            return View(args);
        }

        [Route("Course/EditCourse/{id}")]
        public async Task<ActionResult> EditCourses(Guid id)
        {
            var item = await _db.Courses
                .Include(c => c.Category) // Assuming you need to include related data
                .Include(c => c.Age) // Assuming you need to include related data
                .FirstOrDefaultAsync(n => n.CourseId == id);

            if (item == null)
            {
                return NotFound();
            }

            // Fetch categories and ages from the database
            var categories = await _db.Categories.ToListAsync();
            var ages = await _db.Ages.ToListAsync();

            // Populate ViewBag with SelectList for categories and ages
            ViewBag.CategoryList = new SelectList(categories, "CategoryId", "CategoryName", item.CategoryId);
            ViewBag.AgeList = new SelectList(ages, "AgeId", "AgeFromTO", item.AgeId);

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Course/EditCourse/{id}")]
        public async Task<ActionResult> EditCourses(Course args)
        {
            if (!ModelState.IsValid)
            {
                // Fetch categories and ages from the database
                var categoryList = await _db.Categories.ToListAsync();
                var ageList = await _db.Ages.ToListAsync();

                // Populate ViewBag with SelectList for categories and ages
                ViewBag.CategoryList = new SelectList(categoryList, "CategoryId", "CategoryName", args.CategoryId);
                ViewBag.AgeList = new SelectList(ageList, "AgeId", "AgeFromTO", args.AgeId);

                return View(args);
            }

            var existingCourse = await _db.Courses.FindAsync(args.CourseId);

            if (existingCourse != null)
            {
                existingCourse.CourseName = args.CourseName;
                existingCourse.CourseDescription = args.CourseDescription;
                existingCourse.CourseMeetingLink = args.CourseMeetingLink;
                existingCourse.CourseStatus = true;
                existingCourse.UpdatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                existingCourse.UpdatedOn = DateTime.Now;
                existingCourse.CategoryId = args.CategoryId;
                existingCourse.AgeId = args.AgeId;

                if (args.clientFile != null && args.clientFile.Length > 0)
                {
                    string fileName = args.clientFile.FileName;
                    string uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "DatabaseFolder");
                    string fullPath = Path.Combine(uploadFolder, fileName);

                    using (var fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        await args.clientFile.CopyToAsync(fileStream);
                    }

                    existingCourse.CourseImagePath = $"/DatabaseFolder/{fileName}";
                }

                if (args.clientVideo != null && args.clientVideo.Length > 0)
                {
                    string videoFileName = args.clientVideo.FileName;
                    string uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "DatabaseFolder");
                    string fullVideoPath = Path.Combine(uploadFolder, videoFileName);

                    using (var videoStream = new FileStream(fullVideoPath, FileMode.Create))
                    {
                        await args.clientVideo.CopyToAsync(videoStream);
                    }

                    existingCourse.CourseVideoPath = $"/DatabaseFolder/{videoFileName}";
                }

                await _db.SaveChangesAsync();

                // Fetch the user's email address from the database
                var userEmail = await _db.Users
                    .Where(u => u.Id == existingCourse.UpdatedBy)
                    .Select(u => u.Email)
                    .FirstOrDefaultAsync();

                await _emailSender.SendEmailAsync(userEmail, "Course Creation", $"{userEmail} has been successfully Updated a {args.CourseName} Coruse in Za Place.");
                return RedirectToAction("Index");
            }

            // If we reach here, something went wrong
            ModelState.AddModelError("", "An error occurred while updating the course.");

            // Fetch categories and ages from the database
            var categoriesList = await _db.Categories.ToListAsync();
            var agesList = await _db.Ages.ToListAsync();

            // Populate ViewBag with SelectList for categories and ages
            ViewBag.CategoryList = new SelectList(categoriesList, "CategoryId", "CategoryName", args.CategoryId);
            ViewBag.AgeList = new SelectList(agesList, "AgeId", "AgeFromTO", args.AgeId);

            return View(args);
        }

        [HttpPost]
        [Route("Course/DeleteCourse/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            try
            {
                var course = await _db.Courses.FindAsync(id);
                if (course == null)
                {
                    return NotFound("Course not found");  // More detailed error message
                }

                // Set course status to false to mark as "inactive"
                course.CourseStatus = false;

                await _db.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");  // Include error details in the response
            }
        }

        [Route("/Course/ChangeStatus")]
        [HttpPost]
        public async Task<IActionResult> ChangeStatus([FromBody] StatusChangeModel model)
        {
            if (model == null)
            {
                return BadRequest("Model cannot be null");
            }
            try
            {
                // Retrieve the course by ID
                var course = await _db.Courses.FindAsync(model.CourseId);
                if (course == null)
                {
                    return NotFound("Course not found");
                }

                // Log the course details for debugging
                Console.WriteLine($"Course fetched: CourseId = {course.CourseId}, Current Status: {course.CourseStatus}");

                // Update the course status
                course.CourseStatus = model.Status;

                // Save changes to the database
                await _db.SaveChangesAsync();

                // Log success message
                Console.WriteLine("Course status successfully updated.");

                return Ok(); // Return success response
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                return StatusCode(500, $"An error occurred: {ex.Message}"); // Return error response with details
            }
        }
        public class StatusChangeModel
        {
            public Guid CourseId { get; set; }
            public bool Status { get; set; } // Ensure this is a boolean type
        }

        [Authorize]
        [Route("Course/CoursePage/{id}")]
        public ActionResult CoursePage(Guid id)
        {
            var course = _db.Courses
                .Include(c => c.Age)
                .Include(c => c.Contents)
                .FirstOrDefault(n => n.CourseId == id);

            if (course == null)
            {
                return NotFound();
            }

            // Get the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if the current user is enrolled in the course
            var isEnrolled = _db.EnrollementCourses
                .Any(e => e.CourseId == id && e.UserId == Guid.Parse(userId));

            var contents = _db.Contents.Where(c => c.CourseId == id).ToList();
            var resources = _db.Resources.Where(r => contents.Select(c => c.ContentId).Contains(r.ContentId)).ToList();
            var payments = _db.Payments.ToList();

            var viewModel = new CourseViewModel
            {
                Course = course,
                Contents = contents,
                Resources = resources,
                IsUserEnrolled = isEnrolled,
                Payments = payments
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnrollNow(Guid courseId)
        {
            if (courseId == Guid.Empty)
            {
                Console.WriteLine("No valid CourseId passed.");
                return BadRequest("CourseId is missing or invalid.");
            }

            // Get the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Create a new EnrollmentCourse object
            var enrollment = new EnrollementCourse
            {
                EnrollementCourseId = Guid.NewGuid(),
                CourseId = courseId,
                UserId = Guid.Parse(userId),
                EnrollementStatus = true,
                EnrolledOn = DateTime.Now,
            };

            // Add the enrollment to the database
            _db.EnrollementCourses.Add(enrollment);
            await _db.SaveChangesAsync();

            // Fetch the user's email address who enrolled in the course
            var enrolledUserEmail = await _db.Users
                .Where(u => u.Id == userId)
                .Select(u => u.Email)
                .FirstOrDefaultAsync();

            // Fetch the course creator's email address
            var courseCreatorId = (await _db.Courses.FindAsync(courseId))?.CreatedBy; // Assuming you have CreatedBy in your Course model
            var courseCreatorEmail = await _db.Users
                .Where(u => u.Id == courseCreatorId)
                .Select(u => u.Email)
                .FirstOrDefaultAsync();

            // Send email to the user who enrolled in the course
            await _emailSender.SendEmailAsync(enrolledUserEmail, "Enrollment Confirmation",
                $"You have successfully enrolled in the course with ID {courseId}.");

            // Send email to the course creator
            if (courseCreatorEmail != null)
            {
                await _emailSender.SendEmailAsync(courseCreatorEmail, "Course Enrollment Notification",
                    $"{enrolledUserEmail} has successfully enrolled in your course with ID {courseId}.");
            }

            // Redirect back to the CoursePage action with the courseId
            return RedirectToAction("CoursePage", new { id = courseId });
        }
    }
}
