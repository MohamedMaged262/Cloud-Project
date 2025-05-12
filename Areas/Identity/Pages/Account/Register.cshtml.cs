using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Identity.UI.Services;
using ZA_PLACE.Models;
using ZA_PLACE.Helpers;

public class RegisterModel : PageModel
{
    private readonly SignInManager<UserExtra> _signInManager;
    private readonly UserManager<UserExtra> _userManager;
    private readonly IUserStore<UserExtra> _userStore;
    private readonly IUserEmailStore<UserExtra> _emailStore;
    private readonly ILogger<RegisterModel> _logger;
    private readonly IEmailSender _emailSender;

    public RegisterModel(
        UserManager<UserExtra> userManager,
        IUserStore<UserExtra> userStore,
        SignInManager<UserExtra> signInManager,
        ILogger<RegisterModel> logger,
        IEmailSender emailSender)
    {
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
        _signInManager = signInManager;
        _logger = logger;
        _emailSender = emailSender;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public string ReturnUrl { get; set; }
    public IList<AuthenticationScheme> ExternalLogins { get; set; }

    public class InputModel
    {
        [Required]
        [StringLength(250)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public bool Gender { get; set; }

        [Required]
        [Display(Name = "User Status")]
        public bool UserStatus { get; set; }

        [Required]
        [Display(Name = "Date Of Birth")]
        public DateTime DoB { get; set; }

        [Display(Name = "Profile Picture")]
        public string? ProfilePicture { get; set; }

        [Display(Name = "About Description")]
        [StringLength(2500)]
        public string? AboutDescription { get; set; }

        [Display(Name = "Parent Phone Number")]
        [StringLength(12)]
        public string? ParentPhoneNumber { get; set; }

        [EmailAddress]
        [StringLength(250)]
        [Display(Name = "Parent Email Address")]
        public string? ParentEmailAddress { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        [Required]
        public int Permision { get; set; }
        public string? RoleName { get; set; }
    }

    public async Task OnGetAsync(string returnUrl = null)
    {
        ReturnUrl = returnUrl;
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        var userRole = string.Empty;
        if (ModelState.IsValid)
        {
            var user = CreateUser();

            await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
            user.EmailConfirmed = true;
            user.FullName = Input.FullName;
            user.PhoneNumber = Input.PhoneNumber;
            user.Gender = Input.Gender;
            user.UserStatus = true;
            user.DoB = Input.DoB;
            user.ProfilePicture = Input.ProfilePicture;
            user.AboutDescription = Input.AboutDescription;
            user.ParentPhoneNumber = Input.ParentPhoneNumber;
            user.ParentEmailAddress = Input.ParentEmailAddress;
            user.CreatedOn = Input.CreatedOn;
            user.UpdatedBy = Input.UpdatedBy;
            user.UpdatedOn = Input.UpdatedOn;
            user.Permision = Input.Permision;

            userRole = user.Permision == 1 ? clsRoles.roleStudent :
                       user.Permision == 2 ? clsRoles.roleTeacher :
                       clsRoles.roleStudent;

            user.RoleName = userRole;

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                // Check if the role exists before assigning
                var roleExists = await _userManager.GetRolesAsync(user);
                if (!roleExists.Contains(userRole))
                {
                    await _userManager.AddToRoleAsync(user, userRole);
                }

                var userId = await _userManager.GetUserIdAsync(user);

                await _emailSender.SendEmailAsync(Input.Email, "Registration Successful", $"{Input.Email} has been successfully registered in Za Place.");
                // Redirect to the Profile action of the User controller with the user ID
                return RedirectToAction("Profile", "User", new { id = userId });

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                }
                else
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return Page();
    }

    private UserExtra CreateUser()
    {
        try
        {
            return Activator.CreateInstance<UserExtra>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(UserExtra)}'. " +
                $"Ensure that '{nameof(UserExtra)}' is not an abstract class and has a parameterless constructor.");
        }
    }

    private IUserEmailStore<UserExtra> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<UserExtra>)_userStore;
    }
}
