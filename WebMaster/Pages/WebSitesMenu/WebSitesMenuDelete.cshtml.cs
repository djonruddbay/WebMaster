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

    public class WebSitesMenuDeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public WebSitesMenuDeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public WebSitesMenuModel WebSitesMenuModel { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, int? profileId, string profileName, int? webSitesMenuId, string returnUrl)
        {
            if (string.IsNullOrEmpty(userId) || 
                string.IsNullOrEmpty(webSitesMenuId.ToString()) ||
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

            WebSitesMenuModel = await _context.WebSitesMenu
                 .SingleOrDefaultAsync(m => m.Id == webSitesMenuId);

            if (WebSitesMenuModel == null)
            {
                return RedirectToPage("./WebSitesMenuIndex", new { userId, profileId, profileName, returnUrl, deletedMenuItem="true" });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string userId, int? id, int profileId, string profileName, string returnUrl)
        {
            if (string.IsNullOrEmpty(userId) || 
                string.IsNullOrEmpty(id.ToString()) ||
                string.IsNullOrEmpty(profileId.ToString()) ||
                string.IsNullOrEmpty(profileName) ||
                string.IsNullOrEmpty(returnUrl))
            {
                return RedirectToPage("./WebSitesMenuIndex", new { userId, profileId, profileName, returnUrl});
            }

            WebSitesMenuModel = await _context.WebSitesMenu.FindAsync(id);

            if (WebSitesMenuModel == null)
            {
                return RedirectToPage("./WebSitesMenuIndex", new { userId, profileId, profileName, returnUrl, deletedMenuItem="true" });
            }

            _context.WebSitesMenu.Remove(WebSitesMenuModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./WebSitesMenuIndex", new { userId, profileId, profileName, returnUrl });
        }
    }
}
