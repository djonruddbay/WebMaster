using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebMaster.Data;
using WebMaster.Models;

namespace WebMaster.Pages.WebSitesMenu
{
    [Authorize(Roles = "WebMastr")]

    public class WebSitesMenuCreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public WebSitesMenuCreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(string userId, int? profileId, string profileName, string returnUrl)
        {
            if (string.IsNullOrEmpty(userId) ||
                string.IsNullOrEmpty(profileId.ToString()) ||
                string.IsNullOrEmpty(profileName) ||
                string.IsNullOrEmpty(returnUrl))
            {
                return NotFound();
            }

            var profile = await _context.Profiles
                .Include(p => p.ApplicationUser).SingleOrDefaultAsync(m => m.Id == profileId);

            if (profile == null)
            {
                return NotFound();
            }

            TempData["UserId"] = userId;
            TempData["ProfileId"] = profileId;
            TempData["ProfileName"] = profileName;
            TempData["ReturnUrl"] = returnUrl;

            return Page();
        }

        [BindProperty]
        public WebSitesMenuModel WebSitesMenuModel { get; set; }

        public async Task<IActionResult> OnPostAsync(string userId, int profileId, string profileName, string returnUrl)
        {
            if (string.IsNullOrEmpty(userId) ||
                string.IsNullOrEmpty(profileId.ToString()) ||
                string.IsNullOrEmpty(profileName) ||
                string.IsNullOrEmpty(returnUrl))
            {
                return NotFound();
            }

            TempData["UserId"] = userId;
            TempData["ProfileId"] = profileId;
            TempData["ProfileName"] = profileName;
            TempData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            WebSitesMenuModel.ProfileId = profileId;
            _context.WebSitesMenu.Add(WebSitesMenuModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./WebSitesMenuIndex", new { userId, profileId, profileName, returnUrl });
        }
    }
}