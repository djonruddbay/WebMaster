using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using WebMaster.Data;

namespace WebMaster.Pages.UserAccount
{
    [Authorize(Roles = "WebMastr")]

    public class UserEditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<EMailUpdateModel> _logger;

        public UserEditModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<EMailUpdateModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        [TempData]
        public int ProfileId { get; set; }
        [TempData]
        public bool IsAppUser { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "A User Id is Required!")]
            [StringLength(20, ErrorMessage = "User Id Must be {2} to {1} Characters!", MinimumLength = 5)]
            [Display(Prompt = "User Id!")]
            public string UserName { get; set; }

            public bool EmailConfirmed { get; set; }

            [Required(ErrorMessage = "An E-Mail Address is Required!")]
            [EmailAddress(ErrorMessage = "E-Mail Address is not Valid!")]
            [Display(Prompt = "E-Mail Address!")]
            public string Email { get; set; }

            public bool PhoneNumberConfirmed { get; set; }

            [StringLength(14, ErrorMessage = "Please Enter a Valid Phone Number!", MinimumLength = 14)]
            [Display(Prompt = "(999) 999-9999")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "Security Color is Required!")]
            [StringLength(10)]
            public string SecurityColorAnswer { get; set; }

            [Required(ErrorMessage = "Security Symbol is Required!")]
            [StringLength(10)]
            public string SecuritySymbolAnswer { get; set; }

            public int AccessFailedCount { get; set; }

            public bool LockoutEnabled { get; set; }

            public DateTimeOffset LockoutEnd { get; set; }

            public bool TwoFactorEnabled { get; set; }

            public string NormalizedUserName { get; set; }

            public string NormalizedEmail { get; set; }

            public string PasswordHash { get; set; }

            public string SecurityStamp { get; set; }

            public string ConcurrencyStamp { get; set; }

            public string Id { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string userId, bool deletedProfile, string UpdateSuccessfulMessageHidden)
        {
            var user = await _context.ApplicationUser.SingleOrDefaultAsync(m => m.Id == userId);

            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            TempData["DeletedProfile"] = deletedProfile ? "true" : "false";
            TempData["UpdateSuccessfulMessageHidden"] = String.IsNullOrEmpty(UpdateSuccessfulMessageHidden) ? "hidden" : "";

            ProfileId = 0;

            var Profile = await _context.Profiles
                      .SingleOrDefaultAsync(m => m.UserId == userId);

            ProfileId = Profile == null ? 0 : Profile.Id;

            Input = new InputModel
            {
                UserName = user.UserName,
                EmailConfirmed = user.EmailConfirmed,
                Email = user.Email,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                PhoneNumber = user.PhoneNumber,
                SecurityColorAnswer = user.SecurityColorAnswer,
                SecuritySymbolAnswer = user.SecuritySymbolAnswer,
                AccessFailedCount = user.AccessFailedCount,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd ?? DateTimeOffset.MinValue,
                TwoFactorEnabled = user.TwoFactorEnabled,
                NormalizedUserName = user.NormalizedUserName,
                NormalizedEmail = user.NormalizedEmail,
                PasswordHash = user.PasswordHash,
                SecurityStamp = user.SecurityStamp,
                ConcurrencyStamp = user.ConcurrencyStamp,
                Id = user.Id
            };

            IsAppUser = await _userManager.IsInRoleAsync(user, "AppUser");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _context.ApplicationUser.SingleOrDefaultAsync(m => m.Id == Input.Id);

            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                TempData["UpdateSuccessfulMessageHidden"] = "hidden";
                return Page();
            }

            user.Id = Input.Id;
            user.UserName = Input.UserName;
            user.NormalizedUserName = Input.UserName.ToUpper();
            user.Email = Input.Email;
            user.NormalizedEmail = Input.Email.ToUpper();
            user.EmailConfirmed = Input.EmailConfirmed;
            user.PhoneNumber = Input.PhoneNumber;
            user.PhoneNumberConfirmed = Input.PhoneNumberConfirmed;
            user.SecuritySymbolAnswer = Input.SecuritySymbolAnswer;
            user.SecurityColorAnswer = Input.SecurityColorAnswer;
            user.AccessFailedCount = Input.AccessFailedCount;
            user.LockoutEnabled = Input.LockoutEnabled;
            user.LockoutEnd = Input.LockoutEnd;
            user.TwoFactorEnabled = Input.TwoFactorEnabled;

            _context.Attach(user).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            _logger.LogInformation("WebMaster ~ ApplicationUser successfully updated.");

            return RedirectToPage("/UserAccount/UserEdit",
                        new { userId = Input.Id, UpdateSuccessfulMessageHidden = "show" });
        }
    }
}
