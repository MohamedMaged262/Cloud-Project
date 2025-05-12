using Microsoft.AspNetCore.Mvc;
using System.Linq;
using ZA_PLACE.Models;
using ZA_PLACE.ViewModel;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ZA_PLACE.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace ZA_PLACE.Controllers
{
    [Authorize]
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IEmailSender _emailSender;

        // Constructor for dependency injection of ApplicationDbContext and IHostingEnvironment
        public NewsController(ApplicationDbContext db, IHostingEnvironment hostingEnvironment, IEmailSender emailSender)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
            _emailSender = emailSender;
        }

        // Action method for Index view
        public IActionResult Index()
        {
            // Retrieve all news items from the database
            IEnumerable<News> NewsList = _db.News.ToList();

            // Pass the list of news to the view
            return View(NewsList);
        }


        // Action method to show CreateNews form
        public IActionResult CreateNews()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNews(NewsViewModel args)
        {
            if (ModelState.IsValid)
            {
                string fileName = string.Empty;
                string ImagePath = string.Empty;

                if (args.clientFile != null) // Check if a file was uploaded
                {
                    // Define the folder to store the uploaded files
                    string uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "DatabaseFolder");

                    // Ensure the folder exists
                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    fileName = args.clientFile.FileName;
                    string fullPath = Path.Combine(uploadFolder, fileName);

                    // Copy the file to the specified path
                    using (var fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        await args.clientFile.CopyToAsync(fileStream);
                    }

                    // Store the relative path in the database
                    args.NewsPhotoPath = $"/DatabaseFolder/{fileName}";
                    ImagePath = args.NewsPhotoPath;
                }
                else
                {
                    ImagePath = "/DatabaseFolder/6.png"; // Corrected path
                }

                // Create a new News item using the view model data
                var newItem = new News
                {
                    NewsId = Guid.NewGuid(),
                    NewsTitle = args.NewsTitle,
                    NewsDescription = args.NewsDescription,
                    NewsPhotoPath = ImagePath,
                    CreatedOn = DateTime.UtcNow, // Use UTC for consistency
                    UpdatedOn = null,
                };

                // Add the news item to the database
                _db.News.Add(newItem);
                await _db.SaveChangesAsync(); // Use async save

                // Fetch all registered users' emails from the database
                var userEmails = await _db.Users
                    .Select(u => u.Email)
                    .ToListAsync();

                // Send notifications to all users (if you have an email sender set up)
                foreach (var userEmail in userEmails)
                {
                    await _emailSender.SendEmailAsync(userEmail, "New News Item",
                        $"A new news item titled '{newItem.NewsTitle}' has been created.");
                }

                // Redirect to Index page after successful creation
                return RedirectToAction("Index");
            }

            // If the model is not valid, return the form with the existing data
            return View(args);
        }


        [HttpGet]
        public IActionResult EditNews(Guid id)
        {
            var item = _db.News.FirstOrDefault(n => n.NewsId == id);

            if (item == null)
            {
                return NotFound();
            }

            // Map the News model to the NewsViewModel
            var viewModel = new NewsViewModel
            {
                NewsId = item.NewsId,
                NewsTitle = item.NewsTitle,
                NewsDescription = item.NewsDescription,
                NewsPhotoPath = item.NewsPhotoPath,
                CreatedOn = item.CreatedOn,
                UpdatedOn = item.UpdatedOn
            };

            return View(viewModel);
        }


        // POST method to update the news
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditNews(NewsViewModel args)
        {
            if (ModelState.IsValid)
            {
                var existingItem = _db.News.FirstOrDefault(n => n.NewsId == args.NewsId);
                if (existingItem != null)
                {
                    // Update the news properties
                    existingItem.NewsTitle = args.NewsTitle;
                    existingItem.NewsDescription = args.NewsDescription;
                    existingItem.UpdatedOn = DateTime.Now;

                    // Handle file update if a new file is uploaded
                    if (args.clientFile != null && args.clientFile.Length > 0)
                    {
                        var validFileTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                        if (!validFileTypes.Contains(args.clientFile.ContentType))
                        {
                            ModelState.AddModelError("clientFile", "Invalid file type. Only images are allowed.");
                            return View(args);
                        }

                        string fileName = Path.GetFileName(args.clientFile.FileName);
                        string uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "DatabaseFolder");
                        string fullPath = Path.Combine(uploadFolder, fileName);

                        using (var fileStream = new FileStream(fullPath, FileMode.Create))
                        {
                            args.clientFile.CopyTo(fileStream);
                        }
                        existingItem.NewsPhotoPath = $"/DatabaseFolder/{fileName}";
                    }
                    else
                    {
                        // Keep the existing photo path if no new file is uploaded
                        existingItem.NewsPhotoPath = existingItem.NewsPhotoPath;
                    }

                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else
            {
                // Log validation errors for debugging
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            // If we reach here, something went wrong
            return View(args);
        }

        // POST method to delete the news item
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteNews(Guid id)
        {
            // Find the news item by ID
            var Item = _db.News.FirstOrDefault(n => n.NewsId == id);

            if (Item != null)
            {
                // Check if the news has an associated image file
                if (!string.IsNullOrEmpty(Item.NewsPhotoPath))
                {
                    // Get the full path of the image file
                    string filePath = Path.Combine(_hostingEnvironment.WebRootPath, Item.NewsPhotoPath.TrimStart('/'));

                    // Check if the file exists and delete it
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                // Remove the news item from the database
                _db.News.Remove(Item);
                _db.SaveChanges();

                return RedirectToAction("Index");   
            }

            return NotFound();
        }


        // News User
        public IActionResult NewsPage()
        {
            // Retrieve all news items from the database
            IEnumerable<News> ItemList = _db.News.ToList();

            // Pass the list of news to the view
            return View(ItemList);
        }
    }
}
