using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using WebMaster.Pages.Shared;

namespace WebMaster.Pages
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public IndexModel(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IActionResult> OnGet()
        {

            if (!await _roleManager.RoleExistsAsync(UserRoles.WebMastr))
            {
                return RedirectToPage("/UserAccount/Register");
            }
            else
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return RedirectToPage("/UserAccount/SignIn");
                }
                return Page();
            }
        }
    }
}