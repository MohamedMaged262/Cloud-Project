using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ZA_PLACE.ViewModel;
using ZA_PLACE.Models;
using System.Security.Claims;
using ZA_PLACE.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;


namespace ZA_PLACE.Controllers
{
    [Authorize]
    public class AnnouncementController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;

        public AnnouncementController(ApplicationDbContext db, IEmailSender emailSender)
        {
            _db = db;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            List<Announcement> AnnouncementList;

            if (User.IsInRole(clsRoles.roleAdmin))
            {
                AnnouncementList = _db.Announcements.ToList();
            }
            else if (User.IsInRole(clsRoles.roleTeacher))
            {
                // Teachers can view only active courses they created
                string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                AnnouncementList = _db.Announcements
                                 .Where(c => c.CreatedBy == currentUserId)
                                 .ToList();
            }
            else
            {
                // Other users can view only the courses they are enrolled in
                string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                AnnouncementList = (from c in _db.Announcements
                                    join e in _db.EnrollementCourses
                                    on c.CourseId equals e.CourseId
                                    where e.UserId == Guid.Parse(currentUserId)
                                    select c).ToList();
            }
            return View(AnnouncementList);
        }

        // GET: Announcement/Create
        public IActionResult Create(Guid courseId)
        {
            // Pass CourseId to the View, or populate it in the AnnouncementViewModel
            var model = new AnnouncementViewModel
            {
                CourseId = courseId
            };

            return View(model);
        }

        // POST: Announcement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AnnouncementViewModel args)
        {
            if (ModelState.IsValid)
            {
                var announcement = new Announcement
                {
                    AnnouncementId = Guid.NewGuid(),
                    AnnouncementTitle = args.AnnouncementTitle,
                    AnnouncementDescription = args.AnnouncementDescription,
                    CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    CreatedOn = DateTime.UtcNow,
                    CourseId = args.CourseId // Assuming CourseId exists in the ViewModel
                };

                // Add to database
                _db.Announcements.Add(announcement);
                await _db.SaveChangesAsync();

                // Fetch the current user's email address (the one creating the announcement)
                var creatorEmail = await _db.Users
                    .Where(u => u.Id == announcement.CreatedBy)
                    .Select(u => u.Email)
                    .FirstOrDefaultAsync();

                // Optionally, you can fetch the emails of all users enrolled in the course
                var enrolledUsersEmails = await _db.EnrollementCourses
                    .Where(e => e.CourseId == args.CourseId && e.EnrollementStatus)
                    .Select(e => e.UserId)
                    .ToListAsync();

                // Send email to the announcement creator
                if (creatorEmail != null)
                {
                    await _emailSender.SendEmailAsync(creatorEmail, "Announcement Created",
                        $"Your announcement titled '{announcement.AnnouncementTitle}' has been successfully created.");
                }

                // Send email to all enrolled users in the course
                foreach (var userId in enrolledUsersEmails)
                {
                    var changeUser = userId.ToString();

                    var userEmail = await _db.Users
                        .Where(u => u.Id == changeUser)
                        .Select(u => u.Email)
                        .FirstOrDefaultAsync();

                    if (userEmail != null)
                    {
                        await _emailSender.SendEmailAsync(userEmail, "New Announcement",
                            $"A new announcement titled '{announcement.AnnouncementTitle}' has been created for your course.");
                    }
                }

                // Redirect to the CreatePage action in the CourseController, passing CourseId
                return RedirectToAction("CoursePage", "Course", new { id = args.CourseId });
            }

            return View(args);
        }
    }
}