using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using ZA_PLACE.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ZA_PLACE.Controllers
{
    [Authorize]
    public class ResourceController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;

        // Constructor for dependency injection
        public ResourceController(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: /Resource/CreateResource
        public ActionResult CreateResource(Guid courseId)
        {
            // Fetch contents with Status 1 from the database
            var contentListForViewBag = _db.Contents
                                           .Where(c => c.ContentStatus == true)
                                           .ToList();  // Filter where Status is 1

            // Populate ViewBag with SelectList for contents
            ViewBag.CategoryList = new SelectList(contentListForViewBag, "ContentId", "ContentName");

            // Pass the courseId to the view
            ViewBag.CourseId = courseId;

            return View();
        }

        // POST: /Resource/CreateResource
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateResource(Resource resource)
        {
            // Fetch contents with Status 1 from the database
            var contentListForPost = _db.Contents
                                        .Where(c => c.ContentStatus == true)
                                        .ToList();  // Filter where Status is 1

            if (ModelState.IsValid)
            {
                // Check if the content exists and has Status 1
                var existingContent = await _db.Contents
                                               .Where(c => c.ContentStatus == true && c.ContentId == resource.ContentId)
                                               .FirstOrDefaultAsync();  // Ensure Status is 1

                if (existingContent == null)
                {
                    ModelState.AddModelError("ContentId", "Selected Content does not exist or is inactive.");
                    ViewBag.CategoryList = new SelectList(contentListForPost, "ContentId", "ContentName");
                    ViewBag.CourseId = resource.ContentId; // Retain the courseId
                    return View(resource);
                }

                if (resource.clientFile != null) // Check if a file is uploaded
                {
                    // Define the folder to store the uploaded files
                    string uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "DatabaseFolder");

                    // Ensure the folder exists
                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    // Save the file
                    string fileName = Path.GetFileName(resource.clientFile.FileName);
                    string fullPath = Path.Combine(uploadFolder, fileName);

                    using (var fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        await resource.clientFile.CopyToAsync(fileStream);
                    }

                    // Store the relative path in the database
                    resource.ResourceFile = $"/DatabaseFolder/{fileName}";
                }

                // Save the resource to the database
                _db.Resources.Add(resource);
                await _db.SaveChangesAsync();

                // Redirect back to the original page with the same courseId
                return RedirectToAction("CoursePage", "Courses", new { id = existingContent.CourseId });
            }

            // If the model is not valid, return the form with existing data
            ViewBag.CategoryList = new SelectList(contentListForPost, "ContentId", "ContentName");
            ViewBag.CourseId = resource.ContentId; // Retain the courseId
            return View(resource);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteResource(Guid id)
        {
            // Fetch the resource from the database by id
            var resource = await _db.Resources.FindAsync(id);

            if (resource == null)
            {
                return Json(new { success = false, message = "Resource not found" });
            }

            try
            {
                // If a file is associated with the resource, delete it from the server
                if (!string.IsNullOrEmpty(resource.ResourceFile))
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", resource.ResourceFile);

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                // Remove the resource from the database
                _db.Resources.Remove(resource);
                await _db.SaveChangesAsync();

                // Return a success response
                return Json(new { success = true, message = "Resource deleted successfully" });
            }
            catch (Exception ex)
            {
                // Handle any errors and return a failure response
                return Json(new { success = false, message = $"Error deleting resource: {ex.Message}" });
            }
        }

    }
}
