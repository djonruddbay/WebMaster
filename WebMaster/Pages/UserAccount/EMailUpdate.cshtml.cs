using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using WebMaster.Data;

namespace WebMaster.Pages.UserAccount
{
    [Authorize(Roles = "WebMastr")]
    public class EMailUpdateModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<EMailUpdateModel> _logger;

        public EMailUpdateModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<EMailUpdateModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }
        public bool IsEmailConfirmed { get; set; }

        public class InputModel
        {
            [EmailAddress]
            public string CurrentEmail { get; set; }

            [Required(ErrorMessage = "An E-Mail Address is Required!")]
            [EmailAddress(ErrorMessage = "E-Mail Address is not a Valid Address!")]
            [Display(Name = "E-Mail Address", Prompt = "Enter Your E-Mail Address Here!")]
            public string Email { get; set; }

            [Compare("Email", ErrorMessage = "Confirmation E-Mail Must Match E-Mail!")]
            [EmailAddress(ErrorMessage = "Confirmation E-Mail Must Match E-Mail!")]
            [Display(Name = "Confirm E-Mail", Prompt = "Re-Enter Your E-Mail Address Here!")]
            public string ConfirmEmail { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            Input = new InputModel
            {
                CurrentEmail = user.Email,
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            Input.CurrentEmail = user.Email;

            if (!ModelState.IsValid)
            {
                StatusMessage = "";
                return Page();
            }
          
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            Input.CurrentEmail = user.Email;
            if (Input.Email != user.Email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);

                if (!setEmailResult.Succeeded)
                {
                    foreach (var error in setEmailResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return Page();
                }

                StatusMessage = "E-Mail Address Successfully Updated!";
                _logger.LogInformation("User changed their E-Mail successfully.");
            }
            else
            {
                StatusMessage = "E-Mail Address Not Changed!";
            }
                
            return RedirectToPage("/Index");
        }
    }
}
