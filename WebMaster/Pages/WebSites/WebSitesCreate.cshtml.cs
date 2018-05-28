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

    public class WebSitesCreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public WebSitesCreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(
            string userId,
            int profileId,
            string profileName,
            int webSitesMenuId,
            string webSitesMenuDescription,
            string returnUrl)
        {
            if (string.IsNullOrEmpty(userId) ||
                string.IsNullOrEmpty(profileId.ToString()) ||
                string.IsNullOrEmpty(profileName) ||
                string.IsNullOrEmpty(webSitesMenuId.ToString()) ||
                string.IsNullOrEmpty(webSitesMenuDescription) ||
                string.IsNullOrEmpty(returnUrl))
            {
                return NotFound();
            }

            var webSitesMenu = await _context.WebSitesMenu
              .SingleOrDefaultAsync(m => m.Id == webSitesMenuId);
            if (webSitesMenu == null)
            {
                return NotFound();
            }

            TempData["UserId"] = userId;
            TempData["WebSitesMenuId"] = webSitesMenuId;
            TempData["ProfileId"] = profileId;
            TempData["ProfileName"] = profileName;
            TempData["WebSitesMenuDescription"] = webSitesMenuDescription;
            TempData["ReturnUrl"] = returnUrl;

            return Page();
        }

        [BindProperty]
        public WebSitesModel WebSitesModel { get; set; }

        public async Task<IActionResult> OnPostAsync(
              string userId,
              int profileId,
              string profileName,
              int webSitesMenuId,
              string webSitesMenuDescription,
              string returnUrl)

        {
            if (string.IsNullOrEmpty(userId) ||
                string.IsNullOrEmpty(profileId.ToString()) ||
                string.IsNullOrEmpty(profileName) ||
                string.IsNullOrEmpty(webSitesMenuId.ToString()) ||
                string.IsNullOrEmpty(webSitesMenuDescription) ||
                string.IsNullOrEmpty(returnUrl))
            {
                return NotFound();
            }

            TempData["UserId"] = userId;
            TempData["WebSitesMenuId"] = webSitesMenuId;
            TempData["ProfileId"] = profileId;
            TempData["ProfileName"] = profileName;
            TempData["WebSitesMenuDescription"] = webSitesMenuDescription;
            TempData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            WebSitesModel.WebSitesMenuId = webSitesMenuId;
            _context.WebSites.Add(WebSitesModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("/WebSites/WebSitesIndex", new { userId, profileId, webSitesMenuId, profileName, webSitesMenuDescription, returnUrl });
        }
    }
}
