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
    public class AssignmentController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;

        public AssignmentController(ApplicationDbContext db, IEmailSender emailSender)
        {
            _db = db;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            List<Assignment> AssignmentList;

            if (User.IsInRole(clsRoles.roleAdmin))
            {
                AssignmentList = _db.Assignments.ToList();
            }
            else if (User.IsInRole(clsRoles.roleTeacher))
            {
                // Teachers can view only active courses they created
                string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                AssignmentList = _db.Assignments
                                 .Where(c => c.CreatedBy == currentUserId)
                                 .ToList();
            }
            else
            {
                // Other users can view only the courses they are enrolled in
                string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                AssignmentList = (from c in _db.Assignments
                                    join e in _db.EnrollementCourses
                                    on c.CourseId equals e.CourseId
                                    where e.UserId == Guid.Parse(currentUserId)
                                    select c).ToList();
            }
            return View(AssignmentList);
        }

        // GET: Announcement/Create
        public IActionResult Create(Guid courseId)
        {
            // Pass CourseId to the View, or populate it in the AnnouncementViewModel
            var model = new AssignmentViewModel
            {
                CourseId = courseId
            };

            return View(model);
        }

        // POST: Assignment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AssignmentViewModel args)
        {
            if (ModelState.IsValid)
            {
                var assignment = new Assignment
                {
                    AssignmentId = Guid.NewGuid(),
                    AssignmentName = args.AssignmentName,
                    AssignmentDescription = args.AssignmentDescription,
                    AssignmentLink = args.AssignmentLink,
                    AssignmentStatus = false,
                    CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    CreatedOn = DateTime.UtcNow,
                    CourseId = args.CourseId
                };

                // Add to database
                _db.Assignments.Add(assignment);
                await _db.SaveChangesAsync();

                // Fetch the current user's email address (the one creating the assignment)
                var creatorEmail = await _db.Users
                    .Where(u => u.Id == assignment.CreatedBy)
                    .Select(u => u.Email)
                    .FirstOrDefaultAsync();

                // Fetch all enrolled users' emails in the course
                var enrolledUsersEmails = await _db.EnrollementCourses
                    .Where(e => e.CourseId == args.CourseId && e.EnrollementStatus)
                    .Select(e => e.UserId)
                    .ToListAsync();

                // Send email to the assignment creator
                if (creatorEmail != null)
                {
                    await _emailSender.SendEmailAsync(creatorEmail, "Assignment Created",
                        $"Your assignment titled '{assignment.AssignmentName}' has been successfully created.");
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
                        await _emailSender.SendEmailAsync(userEmail, "New Assignment",
                            $"A new assignment titled '{assignment.AssignmentName}' has been created for your course.");
                    }
                }

                // Redirect to the CreatePage action in the CourseController, passing CourseId
                return RedirectToAction("CoursePage", "Course", new { id = args.CourseId });
            }

            return View(args);
        }
    }
}
