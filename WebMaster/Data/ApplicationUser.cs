using Microsoft.AspNetCore.Identity;
using WebMaster.Models;

namespace WebMaster.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string SecurityColorAnswer { get; set; }
        public string SecuritySymbolAnswer { get; set; }

        public ProfilesModel Profiles { get; set; }
    }
}
