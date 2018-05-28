using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebMaster.Data;
using WebMaster.Models;

namespace WebMaster.Pages.WebSites
{
    [Authorize(Roles = "WebMastr")]

    public class WebSitesEditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public WebSitesEditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public WebSitesModel WebSitesModel { get; set; }

        public async Task<IActionResult> OnGetAsync(
            string userId,
            int profileId,
            string profileName,
            int webSitesMenuId,
            string webSitesMenuDescription,
            int webSiteId,
            string returnUrl)
        {
            if (string.IsNullOrEmpty(userId) ||
                string.IsNullOrEmpty(profileId.ToString()) ||
                string.IsNullOrEmpty(profileName) ||
                string.IsNullOrEmpty(webSitesMenuId.ToString()) ||
                string.IsNullOrEmpty(webSitesMenuDescription) ||
                string.IsNullOrEmpty(webSiteId.ToString()) ||
                string.IsNullOrEmpty(returnUrl))
            {
                return NotFound();
            }

            WebSitesModel = await _context.WebSites
                .SingleOrDefaultAsync(m => m.Id == webSiteId);

            if(WebSitesModel == null)
            {
                return RedirectToPage("/WebSites/WebSitesIndex",
                    new { userId, profileId, profileName, webSitesMenuId, webSitesMenuDescription, returnUrl, deletedWebSite = "true" });
            }
            TempData["UserId"] = userId;         
            TempData["ProfileId"] = profileId;
            TempData["ProfileName"] = profileName;
            TempData["WebSitesMenuId"] = webSitesMenuId;
            TempData["WebSitesMenuDescription"] = webSitesMenuDescription;
            TempData["ReturnUrl"] = returnUrl;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(
            string userId,
            int profileId,
            string profileName,
            int webSitesMenuId,
            string webSitesMenuDescription,
            int webSiteId,
            string returnUrl)
        {
            if (string.IsNullOrEmpty(userId) ||
                string.IsNullOrEmpty(profileId.ToString()) ||
                string.IsNullOrEmpty(profileName) ||
                string.IsNullOrEmpty(webSitesMenuId.ToString()) ||
                string.IsNullOrEmpty(webSitesMenuDescription) ||
                string.IsNullOrEmpty(webSiteId.ToString()) ||
                string.IsNullOrEmpty(returnUrl))
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(WebSitesModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WebSitesModelExists(WebSitesModel.Id))
                {
                    return RedirectToPage("/WebSites/WebSitesIndex",
                        new { userId, profileId, profileName, webSitesMenuId, webSitesMenuDescription, returnUrl, deletedWebSite = "true" });
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("/WebSites/WebSitesIndex", 
                new { userId, profileId, profileName, webSitesMenuId, webSitesMenuDescription, returnUrl });
        }

        private bool WebSitesModelExists(int id)
        {
            return _context.WebSites.Any(e => e.Id == id);
        }
    }
}
