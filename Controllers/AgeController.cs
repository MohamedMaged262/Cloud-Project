using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using ZA_PLACE.Models;
using System.Linq;
using System.IO;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Authorization;
using ZA_PLACE.Helpers;

namespace ZA_PLACE.Controllers
{
    [Authorize(Roles = clsRoles.roleAdmin)]
    public class AgeController : Controller
    {
        private readonly ApplicationDbContext _db;

        // Constructor for dependency injection of ApplicationDbContext and IWebHostEnvironment
        public AgeController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: CategoryController
        public ActionResult Index()
        {
            // Retrieve all Category items from the database
            var AgeList = _db.Ages.ToList();

            // Pass the list of Category to the view
            return View(AgeList);
        }

        // GET: CategoryController/Create
        public ActionResult CreateAge()
        {
            return View();
        }

        // POST: CategoryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAge(AgeViewModel args)
        {
            if (ModelState.IsValid)
            {
                var newItem = new Age
                {
                    AgeId = Guid.NewGuid(), // Ensure AgeId is generated
                    AgeFromTO = args.AgeFromTO,
                    AgeStatus = true,
                    CreatedOn = DateTime.Now,
                    UpdatedOn = null,
                };

                // No need to set newItem to null
                _db.Ages.Add(newItem);
                _db.SaveChanges();

                return RedirectToAction("Index");
            }

            TempData["Error"] = "There was an error in creating the age. Please check the inputs.";
            return View(args);
        }

        // GET: AgeController/Edit/5
        public ActionResult EditAge(Guid id)
        {
            var item = _db.Ages.FirstOrDefault(n => n.AgeId == id);

            if (item == null)
            {
                TempData["Error"] = "There was an error retrieving the age. Please check the inputs.";
                return View(new AgeViewModel()); // Return a new AgeViewModel
            }

            // Convert Age to AgeViewModel
            var viewModel = new AgeViewModel
            {
                AgeId = item.AgeId,
                AgeFromTO = item.AgeFromTO
            };

            return View(viewModel);
        }

        // POST: CategoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAge(AgeViewModel args)
        {
            if (ModelState.IsValid)
            {
                var existingItem = _db.Ages.FirstOrDefault(n => n.AgeId == args.AgeId);
                if (existingItem != null)
                {
                    // Update the age properties
                    existingItem.AgeFromTO = args.AgeFromTO;
                    existingItem.AgeStatus = true; 
                    existingItem.UpdatedOn = DateTime.Now;

                    _db.Ages.Update(existingItem);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Error"] = "Age not found. Please try again.";
                }
            }

            // If we got this far, something failed; redisplay form
            return View(args); // Return the view with the model to show validation errors
        }

        [HttpPost]
        [Route("Age/ChangeStatus")]
        public JsonResult ChangeStatus([FromBody] ChangeStatusRequest request)
        {
            if (request?.AgeId == null)
            {
                return Json(new { success = false, message = "Invalid Age ID." });
            }

            var item = _db.Ages.Find(request.AgeId);

            if (item != null)
            {
                // Toggle the AgeStatus
                item.AgeStatus = !item.AgeStatus;

                _db.Ages.Update(item);
                _db.SaveChanges();

                return Json(new { success = true, message = "Status updated successfully." });
            }

            return Json(new { success = false, message = "Age not found." });
        }

        // Model for request
        public class ChangeStatusRequest
        {
            public Guid AgeId { get; set; }
            public bool Status { get; set; }
        }

    }
}
