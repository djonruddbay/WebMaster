using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using WebMaster.Data;
using WebMaster.Pages.Shared;
using WebMaster.Services;

namespace WebMaster.Pages.UserAccount
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        public string ReturnUrl { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "A User Id is Required!")]
            [StringLength(20, ErrorMessage = "User Id Must be {2} to {1} Characters!", MinimumLength = 5)]
            [Display(Name = "User-Id", Prompt = "User Id!")]
            public string UserName { get; set; }

            public bool IsWebMastr { get; set; }

            [Required(ErrorMessage = "A Password is Required!")]
            [DataType(DataType.Password)]
            [Display(Name = "Password", Prompt = "Password!")]
            public string Password { get; set; }

            [Compare("Password", ErrorMessage = "The Confirmation Password Does Not Match!")]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm Password", Prompt = "Re-Enter Password!")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "An E-Mail Address is Required!")]
            [EmailAddress(ErrorMessage = "E-Mail Address is not Valid!")]
            [Display(Name = "E-Mail", Prompt = "E-Mail Address!")]
            public string Email { get; set; }

            [Compare("Email", ErrorMessage = "Confirmation E-Mail Must Match E-Mail!")]
            [EmailAddress(ErrorMessage = "Confirmation E-Mail Must Match E-Mail!")]
            [Display(Name = "Confirm E-Mail", Prompt = "Re-Enter E-Mail Address!")]
            public string ConfirmEmail { get; set; }

            [StringLength(14, ErrorMessage = "Please Enter a Valid Phone Number!", MinimumLength = 14)]
            [Display(Name = "Cell Phone", Prompt = "(999) 999-9999")]       
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "Security Color is Required!")]
            [StringLength(10)]
            public string SecurityColorAnswer { get; set; }

            [Required(ErrorMessage = "Security Symbol is Required!")]
            [StringLength(10)]
            public string SecuritySymbolAnswer { get; set; }
        }

        public void OnGet(string returnUrl)
        { 
            ViewData["ReturnUrl"] = returnUrl;
            @TempData["SecurityColorSelection"] = "";
            @TempData["SecuritySymbolSelection"] = "";
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            @TempData["SecurityColorSelection"] = Input.SecurityColorAnswer;
            @TempData["SecuritySymbolSelection"] = Input.SecuritySymbolAnswer;

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.UserName,
                    PhoneNumber = Input.PhoneNumber,
                    Email = Input.Email,
                    SecurityColorAnswer = Input.SecurityColorAnswer,
                    SecuritySymbolAnswer = Input.SecuritySymbolAnswer
                };

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync(UserRoles.WebMastr))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(UserRoles.WebMastr));
                        Input.IsWebMastr = true;
                    }

                    if (!await _roleManager.RoleExistsAsync(UserRoles.AppUser))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(UserRoles.AppUser));
                    }
                              
                    if (Input.IsWebMastr)
                    {
                        await _userManager.AddToRoleAsync(user, UserRoles.WebMastr);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, UserRoles.AppUser);
                    }

                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await _emailSender.SendEmailConfirmationAsync(Input.Email, callbackUrl);

                    return LocalRedirect(Url.GetLocalUrl(returnUrl));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
