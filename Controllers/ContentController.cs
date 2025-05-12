using Microsoft.AspNetCore.Mvc;
using ZA_PLACE.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using ZA_PLACE.Helpers;

namespace ZA_PLACE.Controllers
{
    [Authorize(Roles = clsRoles.roleAdmin + "," + clsRoles.roleTeacher)]
    public class ContentController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ContentController> _logger;

        // Constructor for dependency injection
        public ContentController(ApplicationDbContext db, ILogger<ContentController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // GET: /Course/CreateContent/{id}
        [HttpGet]
        [Route("Course/CreateContent/{id}")]
        public ActionResult CreateContent(Guid id)
        {
            ViewBag.CourseId = id; // Passing the course ID to the view
            return View();
        }

        [HttpPost]
        [Route("/Content/AddContent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddContent(Content content)
        {
            content.CreatedBy = "Admin"; 
            content.CreatedOn = DateTime.Now;
            content.ContentStatus = true;

            if (ModelState.IsValid)
            {
                _db.Contents.Add(content);
                await _db.SaveChangesAsync();
                return RedirectToAction("CoursePage", "Courses", new { id = content.CourseId });
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                _logger.LogError("Model state is invalid: {Errors}", string.Join(", ", errors));
                return View("CreateContent", content);
            }
        }

        // GET: /Content/Edit/{id}
        [HttpGet]
        [Route("/Content/EditContent/{id}")]
        public async Task<IActionResult> EditContent(Guid id)
        {
            var content = await _db.Contents.FindAsync(id);

            if (content == null)
            {
                return NotFound();
            }

            // Pass the CourseId to the view via ViewBag if needed
            ViewBag.CourseId = content.CourseId;

            return View(content);
        }

        [HttpPost]
        [Route("/Content/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Content content)
        {
            if (ModelState.IsValid)
            {
                var existingContent = await _db.Contents.FindAsync(content.ContentId);

                if (existingContent == null)
                {
                    return NotFound();
                }

                // Update the properties
                existingContent.ContentName = content.ContentName;
                existingContent.ContentDescription = content.ContentDescription;
                existingContent.ContentPrice = content.ContentPrice;
                existingContent.ContentMeetingLink = content.ContentMeetingLink;
                existingContent.UpdatedBy = "Admin"; // Populate UpdatedBy
                existingContent.UpdatedOn = DateTime.Now;
                existingContent.ContentStatus = true;

                _db.Contents.Update(existingContent);
                await _db.SaveChangesAsync();

                return RedirectToAction("CoursePage", "Courses", new { id = existingContent.CourseId });
            }

            return View(content);
        }

        // POST: /Course/DeleteCourse/{id}
        [HttpPost]
        [Route("Content/DeleteContent/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteContent(Guid id)
        {
            try
            {
                // Retrieve the course by ID
                var course = await _db.Contents.FindAsync(id);
                if (course == null)
                {
                    return NotFound();
                }

                // Update the course status to 0 (assuming 0 means "deleted" or inactive)
                course.ContentStatus = false;

                // Save changes
                await _db.SaveChangesAsync();

                return Ok(); // Return success response
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, ex.Message); // Return error response
            }
        }

        [Authorize(Roles = clsRoles.roleAdmin)]
        // POST: /Course/ChangeStatus
        [HttpPost]
        [Route("Content/ChangeStatus")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus([FromBody] StatusChangeModel model)
        {
            try
            {
                // Retrieve the course by ID
                var course = await _db.Contents.FindAsync(model.CourseId);
                if (course == null)
                {
                    return NotFound();
                }

                // Toggle the course status
                course.ContentStatus = (course.ContentStatus == false) ? true : false;

                // Save changes
                await _db.SaveChangesAsync();

                return Ok(); // Return success response
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, ex.Message); // Return error response
            }
        }
        public class StatusChangeModel
        {
            public Guid CourseId { get; set; }
            public bool Status { get; set; }
        }
    }
}
