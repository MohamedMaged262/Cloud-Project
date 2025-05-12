using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using ZA_PLACE.Models;
using System.Linq;
using System.IO;
using ZA_PLACE.Repository.Base;
using Microsoft.AspNetCore.Authorization;
using ZA_PLACE.Helpers;

namespace ZA_PLACE.Controllers
{
    [Authorize(Roles = clsRoles.roleAdmin)]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public CategoryController(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
        }

        public ActionResult Index()
        {
            var CategoryList = _db.Categories.ToList();
            if (CategoryList != null)
            {

                return View(CategoryList);
            }

            return View();
        }

        public ActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCategory(CategoryViewModel args)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string fileName = string.Empty;
                    var ImagePath = string.Empty;

                    if (args.clientFile != null)
                    {
                        // Define the folder to store the uploaded files
                        string uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "DatabaseFolder");

                        // Ensure the folder exists
                        if (!Directory.Exists(uploadFolder))
                        {
                            Directory.CreateDirectory(uploadFolder);
                        }

                        // Generate a unique filename
                        fileName = $"{Guid.NewGuid()}_{args.clientFile.FileName}";
                        string fullPath = Path.Combine(uploadFolder, fileName);

                        // Copy the file to the specified path
                        using (var fileStream = new FileStream(fullPath, FileMode.Create))
                        {
                            args.clientFile.CopyTo(fileStream);
                        }

                        // Store the relative path in the database
                        ImagePath = $"/DatabaseFolder/{fileName}";
                    }
                    else
                    {
                        // If the ModelState is invalid, return the same view with the view model
                        ImagePath = $"/DatabaseFolder/Science.png";
                    }

                    // Map CategoryViewModel to Category
                    var newItem = new Category
                    {
                        CategoryId = Guid.NewGuid(),
                        CategoryName = args.CategoryName,
                        CategoryImagePath = ImagePath,
                        CategoryStatus = true,
                        CreatedOn = DateTime.Now,
                        UpdatedOn = null,
                        Courses = null
                    };

                    // Add the Category item to the database
                    _db.Categories.Add(newItem);
                    _db.SaveChanges();

                    // Redirect to the Index page after successful creation
                    return RedirectToAction("Index");
                }
                                
                return View(args); 
            }
            catch (Exception ex)
            {
                // Log or display the exception message for debugging purposes
                TempData["Error"] = $"There was an error in creating the category: {ex.Message}";

                // Return the view with the CategoryViewModel to show form inputs
                return View(args);
            }
        }

        [HttpGet]
        public ActionResult EditCategory(Guid id)
        {
            var existingItem = _db.Categories.FirstOrDefault(n => n.CategoryId == id);
            if (existingItem != null)
            {
                var viewModel = new CategoryViewModel
                {
                    CategoryId = existingItem.CategoryId,
                    CategoryName = existingItem.CategoryName,
                    CategoryImagePath = existingItem.CategoryImagePath,
                    // Add other properties as needed
                };

                return View(viewModel);
            }
            return NotFound(); // or handle it as needed
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCategory(CategoryViewModel args, IFormFile? clientFile) // Make clientFile nullable
        {
            args.CategoryStatus = true; // You can set the category status as needed
            if (ModelState.IsValid)
            {
                var existingItem = _db.Categories.FirstOrDefault(n => n.CategoryId == args.CategoryId);
                if (existingItem != null)
                {
                    // Update the category properties
                    existingItem.CategoryName = args.CategoryName;
                    existingItem.UpdatedOn = DateTime.Now;

                    // Handle file update if a new file is uploaded
                    if (clientFile != null && clientFile.Length > 0)
                    {
                        string fileName = clientFile.FileName;
                        string uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "DatabaseFolder");
                        string fullPath = Path.Combine(uploadFolder, fileName);

                        using (var fileStream = new FileStream(fullPath, FileMode.Create))
                        {
                            clientFile.CopyTo(fileStream);
                        }
                        existingItem.CategoryImagePath = $"/DatabaseFolder/{fileName}";
                    }
                    // If no file is uploaded, keep the existing image path
                    // You might also consider adding logic to set a default image or leave it unchanged.

                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else
            {
                // Log or inspect the errors for debugging
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            TempData["Error"] = "There was an error in updating the category. Please check the inputs.";
            return View(args);
        }


        [HttpPost]
        [Route("Category/ChangeStatus")]
        public JsonResult ChangeStatus([FromBody] ChangeStatusRequest request)
        {
            if (request?.CategoryId == null)
            {
                return Json(new { success = false, message = "Invalid Age ID." });
            }

            var item = _db.Categories.Find(request.CategoryId);

            if (item != null)
            {
                // Toggle the AgeStatus
                item.CategoryStatus = !item.CategoryStatus;

                _db.Categories.Update(item);
                _db.SaveChanges();

                return Json(new { success = true, message = "Status updated successfully." });
            }

            return Json(new { success = false, message = "Age not found." });
        }

        public class ChangeStatusRequest
        {
            public Guid CategoryId { get; set; }
            public int Status { get; set; }
        }

        public IActionResult CategoryPage() {
            var itemList = _db.Categories.Where(c => c.CategoryStatus == true).ToList();
            return View(itemList);
        }
    }
}
