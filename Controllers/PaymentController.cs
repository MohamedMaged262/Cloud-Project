using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using ZA_PLACE.Models;
using System.Linq;
using ZA_PLACE.Helpers;
using Stripe;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace ZA_PLACE.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IOptions<StripeSettings> _stripeSettings;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _db;

        public PaymentController(ApplicationDbContext db, IOptions<StripeSettings> stripeSettings, IEmailSender emailSender)
        {
            _db = db;
            _stripeSettings = stripeSettings;
            _emailSender = emailSender;
        }

        // Method to save payment details to the database
        private IActionResult PaymentDatabase(string contentName, bool paymentStatus)
        {
            var newItem = new Payment
            {
                PaymentId = Guid.NewGuid(),
                PaymentReason = contentName,
                PaymentType = "Credit Card",
                PaymentStatus = paymentStatus,
                CreatedOn = DateTime.Now,
                CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            // Save the newItem to the database
            _db.Payments.Add(newItem);
            int result = _db.SaveChanges(); // This returns the number of affected rows

            return Json(new { success = result > 0 });
        }

        // Method to handle checkout process
        public IActionResult Checkout(Guid contentId)
        {
            var content = _db.Contents.FirstOrDefault(c => c.ContentId == contentId);

            if (content == null)
            {
                return NotFound(); // Handle the case where content is not found
            }

            decimal price = Convert.ToDecimal(content.ContentPrice);
            var priceInCents = (int)(price * 100); // Convert price to cents for Stripe

            if (priceInCents < 500)
            {
                return BadRequest("The minimum amount must be ج.م50.00.");
            }

            // Create Stripe Checkout session
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "egp",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = content.ContentName,
                            },
                            UnitAmount = priceInCents,
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = "https://localhost:7049/Payment/Success",
                CancelUrl = "https://localhost:7049/Payment/Cancel",
                Metadata = new Dictionary<string, string>
                {
                    { "contentId", contentId.ToString() },
                    { "userEmail", User.FindFirstValue(ClaimTypes.Email) } // Include user email in metadata
                }
            };

            var service = new SessionService();
            Session session = service.Create(options);

            ViewBag.StripePublishableKey = _stripeSettings.Value.PublishableKey;
            ViewBag.SessionId = session.Id;

            // Save payment to the database (initial entry)
            PaymentDatabase(content.ContentName, true);

            return View();
        }

        // Action for successful payment
        public async Task<IActionResult> Success()
        {
            // Retrieve payment details from the metadata and send confirmation email
            var contentId = Request.Query["contentId"].ToString();
            var userEmail = Request.Query["userEmail"].ToString();

            // Call PaymentDatabase function with actual payment status
            PaymentDatabase(contentId, true);

            // Send a confirmation email to the user
            await _emailSender.SendEmailAsync(userEmail, "Payment Successful",
                $"Your payment for the content has been successfully processed. Thank you!");

            return View();
        }

        // Action for canceled payment
        public IActionResult Cancel()
        {
            return View();
        }

        // Index action to display payment records
        public IActionResult Index()
        {
            List<Payment> paymentList;

            if (User.IsInRole(clsRoles.roleAdmin))
            {
                paymentList = _db.Payments.ToList();
            }
            else
            {
                string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                paymentList = _db.Payments.Where(c => c.CreatedBy == currentUserId).ToList();
            }

            return View(paymentList);
        }

        // Method for handling cash payments
        public IActionResult CashPayment()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CashPayment(Payment args)
        {
            // Handle cash payment logic here
            return View();
        }
    }
}
