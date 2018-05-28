using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebMaster.Data;
using WebMaster.Models;

namespace WebMaster.Pages.WebSitesMenu
{
    [Authorize(Roles = "WebMastr")]

    public class WebSitesMenuEditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public WebSitesMenuEditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public WebSitesMenuModel WebSitesMenuModel { get; set; }

        public async Task<IActionResult> OnGetAsync( string userId, int profileId, string profileName, int webSitesMenuId, string returnUrl)
        {
            if (string.IsNullOrEmpty(userId) ||               
                string.IsNullOrEmpty(profileId.ToString()) ||
                string.IsNullOrEmpty(profileName) ||
                string.IsNullOrEmpty(webSitesMenuId.ToString()) ||
                string.IsNullOrEmpty(returnUrl))
            {
                return NotFound();
            }

            TempData["UserId"] = userId;
            TempData["ProfileId"] = profileId;
            TempData["ProfileName"] = profileName;
            TempData["ReturnUrl"] = returnUrl;

            WebSitesMenuModel = await _context.WebSitesMenu
                .SingleOrDefaultAsync(m => m.Id == webSitesMenuId);

            if (WebSitesMenuModel == null)
            {
                return RedirectToPage("./WebSitesMenuIndex", new { userId, profileId, profileName, returnUrl, deletedMenuItem = "true" });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string userId, int profileId, string profileName, string returnUrl)
        {
            if (string.IsNullOrEmpty(userId) ||
                string.IsNullOrEmpty(profileId.ToString()) ||
                string.IsNullOrEmpty(profileName) ||
                string.IsNullOrEmpty(returnUrl))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(WebSitesMenuModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WebSitesMenuModelExists(WebSitesMenuModel.Id))
                {
                    return RedirectToPage("./WebSitesMenuIndex", new { userId, profileId, profileName, returnUrl, deletedMenuItem = "true" });
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./WebSitesMenuIndex", new { userId, profileId, profileName, returnUrl });
        }

        private bool WebSitesMenuModelExists(int id)
        {
            return _context.WebSitesMenu.Any(e => e.Id == id);
        }
    }
}
