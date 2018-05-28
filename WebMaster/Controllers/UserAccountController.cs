using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WebMaster.Data;

namespace WebMaster.Controllers
{
    [Route("[controller]/[action]")]
    public class UserAccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;

        public UserAccountController(SignInManager<ApplicationUser> signInManager, ILogger<UserAccountController> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User Signed Out.");
            return RedirectToPage("/Index");
        }
    }
}
