using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebMaster.Data;

namespace WebMaster.Pages.UserAccount
{
    [AllowAnonymous]
    public class SignInModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<SignInModel> _logger;

        public SignInModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<SignInModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "User Id is Required!")]
            [Display(Name = "User-Id", Prompt = "Enter Your User-Id!")]
            public string UserName { get; set; }

            [Required(ErrorMessage = "Password is Required!")]
            [Display(Name = "Password", Prompt = "Enter Your Password!")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember Me:")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    //  User.IsInRole(UserRoles.????) doesn't work here
                    //  apparently "User" isn't loaded at this time

                    var user = await _userManager.FindByNameAsync(Input.UserName);
                
                    if (_userManager.GetRolesAsync(user).Result.ToList().Contains("WebMastr"))
                    {
                        return LocalRedirect(Url.GetLocalUrl(returnUrl));                    
                    }
                    else
                    {
                        await _signInManager.SignOutAsync();
                        return RedirectToPage("./LogInNotAllowed");
                    }
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    ModelState.AddModelError(string.Empty, "Account is Temporary Locked!");
                    ModelState.AddModelError(string.Empty, "Please Try Again Later.");
                    return Page();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt!");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
