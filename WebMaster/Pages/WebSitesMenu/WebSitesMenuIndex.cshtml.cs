using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMaster.Data;
using WebMaster.Models;

namespace WebMaster.Pages.WebSitesMenu
{
    [Authorize(Roles = "WebMastr")]

    public class WebSitesMenuIndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public WebSitesMenuIndexModel(ApplicationDbContext context)
        {
            _context = context;
        }
 
        public IList<WebSitesMenuModel> WebSitesMenuModel { get; set; }

        public async Task<IActionResult> OnGetAsync( string userId, int profileId, string profileName, string returnUrl, bool deletedMenuItem)
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
            TempData["DeletedMenuItemMsgHidden"] = deletedMenuItem ? "" : "hidden";

            WebSitesMenuModel = await _context.WebSitesMenu
                .Where(w => w.ProfileId == profileId)
                .OrderBy(o => o.SequenceNumber)
                .AsNoTracking()
                .ToListAsync();

            return Page();
        }
    }
}
