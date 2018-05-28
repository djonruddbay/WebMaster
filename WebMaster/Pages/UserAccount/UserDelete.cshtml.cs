using WebMaster.Data;
using WebMaster.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;

namespace WebMaster.Pages.User
{
    [Authorize(Roles = "WebMastr")]

    public class UserDeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public UserDeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ApplicationUser ApplicationUser { get; set; }
        [BindProperty]
        public ProfilesModel Profile { get; set; }
        [BindProperty]
        public WebSitesMenuModel WebSitesMenu { get; set; }

        [TempData]
        public bool AccountEstablished { get; set; }
        [TempData]
        public string ShortUserId { get; set; }
        [TempData]
        public string ShortEmail { get; set; }
        [TempData]
        public string NOFMsg { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId)
        {
            if (userId == null)
            {
                return NotFound();
            }

            ApplicationUser = await _context.ApplicationUser.SingleOrDefaultAsync(m => m.Id == userId);

            if (ApplicationUser == null)
            {
                return NotFound();
            }

            Profile = await _context.Profiles
                .Include(a => a.ApplicationUser).SingleOrDefaultAsync(m => m.UserId == userId);

            AccountEstablished = Profile != null ? true : false;

            ShortUserId = ApplicationUser.Id;
            if (ShortUserId.Length > 20)
            {
                ShortUserId = ShortUserId.Substring(0, 20) + " ...";
            }
            ShortEmail = ApplicationUser.Email;
            if (ShortEmail.Length > 16)
            {
                ShortEmail = ShortEmail.Substring(0, 16) + " ...";
            }

            NOFMsg = "Not on File!";
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string userId)
        {
            if (userId == null)
            {
                return NotFound();
            }

            ApplicationUser = await _context.ApplicationUser.FindAsync(userId);

            if (ApplicationUser != null)
            {
                _context.ApplicationUser.Remove(ApplicationUser);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./UserIndex");
        }
    }
}
