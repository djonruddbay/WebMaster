using WebMaster.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMaster.Pages.UserAccount
{
    [Authorize(Roles = "WebMastr")]
    public class SecuritySelectionsUpdateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<SecuritySelectionsUpdateModel> _logger;

        public SecuritySelectionsUpdateModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<SecuritySelectionsUpdateModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;

        }

        [BindProperty]
        public ApplicationUser ApplicationUser { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }
        [TempData]
        public string CurrentColorName { get; set; }
        [TempData]
        public string CurrentColorSelection { get; set; }
        [TempData]
        public string CurrentSymbolName { get; set; }
        [TempData]
        public string CurrentSymbolSelection { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Security Color is Required!")]
            [StringLength(10)]
            public string SecurityColorAnswer { get; set; }

            [Required(ErrorMessage = "Security Symbol is Required!")]
            [StringLength(10)]
            public string SecuritySymbolAnswer { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            CurrentColorName = user.SecurityColorAnswer;
            CurrentColorSelection = CurrentColorName.ToLower();
            CurrentSymbolName = user.SecuritySymbolAnswer;
            CurrentSymbolSelection = "fas fa-" + CurrentSymbolName.ToLower();
            @TempData["SecurityColorSelection"] = "";
            @TempData["SecuritySymbolSelection"] = "";

            Input = new InputModel { };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            CurrentColorName = user.SecurityColorAnswer;
            CurrentColorSelection = CurrentColorName.ToLower();
            CurrentSymbolName = user.SecuritySymbolAnswer;
            CurrentSymbolSelection = "fas fa-" + CurrentSymbolName.ToLower();

            @TempData["SecurityColorSelection"] = Input.SecurityColorAnswer;
            @TempData["SecuritySymbolSelection"] = Input.SecuritySymbolAnswer;

            if (!ModelState.IsValid)
            {
                StatusMessage = "";
                return Page();
            }

            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (Input.SecurityColorAnswer != user.SecurityColorAnswer ||
                Input.SecuritySymbolAnswer != user.SecuritySymbolAnswer)
            {
                ApplicationUser = await _context.ApplicationUser.SingleOrDefaultAsync(m => m.Id == user.Id);

                ApplicationUser.SecurityColorAnswer = Input.SecurityColorAnswer;
                ApplicationUser.SecuritySymbolAnswer = Input.SecuritySymbolAnswer;

                _context.Attach(ApplicationUser).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationUserExists(ApplicationUser.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                StatusMessage = "Security Selections Successfully Updated!";
                _logger.LogInformation("User changed their security selections successfully.");
            }
            else
            {
                StatusMessage = "Security Selections Not Changed!";
            }

            return RedirectToPage("/Index");
        }

        private bool ApplicationUserExists(string id)
        {
            return _context.ApplicationUser.Any(e => e.Id == id);
        }
    }
}
