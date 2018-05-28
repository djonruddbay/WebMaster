using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebMaster.Data;
using WebMaster.Models;

namespace WebMaster.Pages.WebSites
{
    [Authorize(Roles = "WebMastr")]

    public class WebSitesDeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public WebSitesDeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public WebSitesModel WebSitesModel { get; set; }

        public async Task<IActionResult> OnGetAsync(
            string userId,
            int webSitesMenuId,
            int profileId,
            string profileName,
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
            if (WebSitesModel == null)
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

            if (WebSitesModel.WebSiteURL.Length > 35)
            {
                TempData["ShortWebSiteURL"] = WebSitesModel.WebSiteURL.Substring(0, 35) + " ...";
            }
            else
            {
                TempData["ShortWebSiteURL"] = WebSitesModel.WebSiteURL;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(
            string userId,
            int webSitesMenuId,
            int profileId,
            string profileName,
            string webSitesMenuDescription,
            int webSiteId,
            string returnUrl)
        {
            if (string.IsNullOrEmpty(webSitesMenuId.ToString()) ||
               string.IsNullOrEmpty(profileId.ToString()) ||
               string.IsNullOrEmpty(profileName) ||
               string.IsNullOrEmpty(webSitesMenuDescription) ||
               string.IsNullOrEmpty(webSiteId.ToString()) ||
               string.IsNullOrEmpty(returnUrl))
            {
                return NotFound();
            }

            WebSitesModel = await _context.WebSites.FindAsync(webSiteId);

            if (WebSitesModel == null)
            {
                return RedirectToPage("/WebSites/WebSitesIndex",
                   new { userId, profileId, profileName, webSitesMenuId, webSitesMenuDescription, returnUrl, deletedWebSite = "true" });
            }

            _context.WebSites.Remove(WebSitesModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("/WebSites/WebSitesIndex", 
                new { userId, profileId, profileName, webSitesMenuId, webSitesMenuDescription, returnUrl });
        }
    }
}
