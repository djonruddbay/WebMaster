using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMaster.Data;
using WebMaster.Models;

namespace WebMaster.Pages.WebSites
{
    [Authorize(Roles = "WebMastr")]

    public class WebSitesIndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public WebSitesIndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<WebSitesModel> WebSites { get; set; }

        public async Task<IActionResult> OnGetAsync(
            string userId,
            int profileId,
            string profileName,
            int webSitesMenuId,
            string webSitesMenuDescription,
            string returnUrl,
            bool deletedWebSite)
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
            TempData["ProfileId"] = profileId;
            TempData["ProfileName"] = profileName;
            TempData["WebSitesMenuId"] = webSitesMenuId;
            TempData["WebSitesMenuDescription"] = webSitesMenuDescription;
            TempData["ReturnUrl"] = returnUrl;
            TempData["DeletedWebSiteMsgHidden"] = deletedWebSite ? "" : "hidden";

            WebSites = await _context.WebSites
                .Where(w => w.WebSitesMenuId == webSitesMenuId)
                .OrderBy(o => o.Description)
                .AsNoTracking()
                .ToListAsync();

            return Page();
        }
    }
}
