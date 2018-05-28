using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using WebMaster.Data;
using WebMaster.Models;

namespace WebMaster.Pages.Profile
{
    public class ProfileDeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileDeleteModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public ProfilesModel Profile { get; set; }

        [TempData]
        public string ShortFirstName { get; set; }
        [TempData]
        public string ShortLastName { get; set; }

        public async Task<IActionResult> OnGetAsync(int? profileId)
        {
            if (profileId == null)
            {
                return NotFound();
            }

            Profile = await _context.Profiles
                .Include(p => p.ApplicationUser).SingleOrDefaultAsync(m => m.Id == profileId);

            if (Profile == null)
            {
                return RedirectToPage("./ProfileIndex", new { userId = Profile.UserId, deletedProfile = true });
            }

            ShortFirstName = Profile.FirstName;
            if (ShortFirstName.Length > 15)
            {
                ShortFirstName = ShortFirstName.Substring(0, 15) + " ...";
            }
            ShortLastName = Profile.LastName;
            if (ShortLastName.Length > 15)
            {
                ShortLastName = ShortLastName.Substring(0, 15) + " ...";
            }

            var WebSitesMenuOnFile = await _context.WebSitesMenu
                .FirstOrDefaultAsync(m => m.ProfileId == Profile.Id);

            TempData["WebSitesOnFileMsg"] = WebSitesMenuOnFile == null ? "" : "Warning...WebSites on File!";

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? profileId)
        {
            if (profileId == null)
            {
                return NotFound();
            }

            Profile = await _context.Profiles.FindAsync(profileId);
            if (Profile == null)
            {
                return RedirectToPage("./ProfileIndex", new { userId = Profile.UserId, deletedProfile = true });
            }

            _context.Profiles.Remove(Profile);
            await _context.SaveChangesAsync();

            return RedirectToPage("./ProfileIndex", new { userId = Profile.UserId });

        }
    }
}
