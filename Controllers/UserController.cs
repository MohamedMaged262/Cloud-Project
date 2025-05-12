using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IO;
using System.Linq;
using System;
using System.Threading.Tasks;
using ZA_PLACE.Models;
using Microsoft.IdentityModel.Tokens;
using ZA_PLACE.Helpers;

namespace ZA_PLACE.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;

        // Constructor for dependency injection of ApplicationDbContext and IWebHostEnvironment
        public UserController(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
        }

        public ActionResult Index()
        {
            // Retrieve all users and roles
            var users = _db.Users.ToList();
            var userRoles = _db.UserRoles.ToList(); // Contains user and role mappings
            var roles = _db.Roles.ToList(); // Contains roles information

            // Use Select to map the users and roles
            var userListWithRoles = users.Select(user => new UserExtra
            {
                Id = user.Id,
                FullName = user.FullName,
                ProfilePicture = user.ProfilePicture,
                PhoneNumber = user.PhoneNumber,
                UserStatus = user.UserStatus,
                CreatedOn = user.CreatedOn,
                // Use the roles from userRoles and roles lists to get the role name
                RoleName = user.RoleName
            }).ToList();

            return View(userListWithRoles);
        }
        public IActionResult Profile(Guid id)
        {
            // Convert the Guid to string if needed
            var userId = id.ToString();

            // Fetch the user from the database using the provided ID
            var user = _db.Users.FirstOrDefault(n => n.Id == userId);

            // Check if the user exists
            if (user == null)
            {
                // Handle the case where the user is not found (optional)
                return NotFound(); // or return RedirectToAction("Index", "Home") to go back to a safe page
            }

            // Return the view with the user data
            return View(user);
        }

        public IActionResult ProfileByUserId() // Changed the name of this action
        {
            var userProfileId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var item = _db.Users.FirstOrDefault(n => n.Id == userProfileId);

            if (item == null)
            {
                TempData["Error"] = "There was an error retrieving the user profile. Please check the inputs.";
                return View(new UserExtra());
            }

            return View(item);
        }

        // GET: Edit User
        public ActionResult EditUser(Guid id)
        {
            var newid = id.ToString();
            var item = _db.Users.FirstOrDefault(n => n.Id == newid);

            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Edit User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(UserExtra args)
        {
            if (!ModelState.IsValid)
            {
                return View(args); // Return the model with validation errors
            }

            var existingUser = _db.Users.FirstOrDefault(n => n.Id == args.Id);
            if (existingUser != null)
            {
                existingUser.FullName = args.FullName;
                existingUser.PhoneNumber = args.PhoneNumber;
                existingUser.Gender = args.Gender; // Ensure Gender is updated correctly
                existingUser.UserStatus = args.UserStatus; // Assuming argsStatus was a typo
                existingUser.DoB = args.DoB;
                existingUser.ProfilePicture = args.ProfilePicture;
                existingUser.AboutDescription = args.AboutDescription;
                existingUser.ParentPhoneNumber = args.ParentPhoneNumber;
                existingUser.ParentEmailAddress = args.ParentEmailAddress;
                existingUser.UpdatedBy = User.FindFirstValue(ClaimTypes.Name); // Assuming the logged-in user
                existingUser.UpdatedOn = DateTime.Now;
                existingUser.UserStatus = true;

                _db.Users.Update(existingUser);
                _db.SaveChanges(); // Save changes to the database

                TempData["Success"] = "User updated successfully.";
                return RedirectToAction("Index"); // Redirect to the index page or any other page
            }

            ModelState.AddModelError("", "User not found.");
            return View(args); // Return model with error message
        }

        [HttpPost]
        [Route("User/ChangeStatus")]
        public JsonResult ChangeStatus([FromBody] ChangeStatusRequest request)
        {
            if (request?.UserId == Guid.Empty) // Check for empty GUID
            {
                return Json(new { success = false, message = "Invalid User ID." });
            }

            var newUserid = request?.UserId.ToString();

            var item = _db.Users.Find(newUserid);

            if (item != null)
            {
                // Toggle the UserStatus
                item.UserStatus = !item.UserStatus;

                _db.Users.Update(item);
                _db.SaveChanges();

                return Json(new { success = true, message = "Status updated successfully." });
            }

            return Json(new { success = false, message = "User not found." });
        }

        // Model for request
        public class ChangeStatusRequest
        {
            public Guid UserId { get; set; } // Only UserId is needed
        }

    }
}