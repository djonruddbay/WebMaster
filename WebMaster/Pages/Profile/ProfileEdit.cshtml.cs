using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebMaster.Data;
using WebMaster.Models;
using WebMaster.Pages.UserAccount;

namespace WebMaster.Pages.Profile
{
    [Authorize(Roles = "WebMastr")]

    public class ProfileEditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EMailUpdateModel> _logger;

        public ProfileEditModel(
            ApplicationDbContext context,
            ILogger<EMailUpdateModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public ProfilesModel Profile { get; set; }
        [TempData]
        public string CreateDate { get; set; }
        [TempData]
        public string LastUpdate { get; set; }

public async Task<IActionResult> OnGetAsync(
            string userId,
            int? profileId,
            string returnUrl,
            string UpdateSuccessfulMessageHidden,
            string UpdateUnSuccessfulMessageHidden)
        {
            if (string.IsNullOrEmpty(userId) ||
                 string.IsNullOrEmpty(profileId.ToString()) ||
                 string.IsNullOrEmpty(returnUrl))
            {
                return NotFound();
            }

            Profile = await _context.Profiles
                .SingleOrDefaultAsync(m => m.Id == profileId);
            if (Profile == null)
            {
                return RedirectToPage(returnUrl, new { userId, deletedProfile = true });
            }

            CreateDate = Profile.DateCreate.ToString();
            LastUpdate = Profile.DateLastUpdate.ToString();

            TempData["UserId"] = userId;
            TempData["ProfileId"] = Profile.Id;
            TempData["ReturnUrl"] = returnUrl;
            TempData["ProfileName"] = Profile.FirstName + "  " + Profile.LastName;
            TempData["UpdateSuccessfulMessageHidden"] = String.IsNullOrEmpty(UpdateSuccessfulMessageHidden) ? "hidden" : "";
            TempData["UpdateUnSuccessfulMessageHidden"] = String.IsNullOrEmpty(UpdateUnSuccessfulMessageHidden) ? "hidden" : "";

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string userId, int? profileId, string returnUrl)
        {
            if (string.IsNullOrEmpty(userId) ||
                string.IsNullOrEmpty(profileId.ToString()) ||
                 string.IsNullOrEmpty(returnUrl))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                TempData["UpdateSuccessfulMessageHidden"] = "hidden";
                TempData["UpdateUnSuccessfulMessageHidden"] = "hidden";
                return Page();
            }

            var ProfileToUpdate = await _context.Profiles
                 .FirstOrDefaultAsync(m => m.Id == profileId);

            if (ProfileToUpdate == null)
            {
                return RedirectToPage(returnUrl, new { userId, deletedProfile = true });
            }

            _context.Entry(ProfileToUpdate)
                .Property("RowVersion").OriginalValue = Profile.RowVersion;

            if (await TryUpdateModelAsync(
               ProfileToUpdate, "Profile",
               p => p.ProfileImage, p => p.AccountNumber, 
               p => p.FirstName, p => p.LastName,
               p => p.AddressLine1, p => p.AddressLine2, p => p.City, p => p.State, p => p.ZipCode))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("WebMaster ~ Profile successfully updated.");

                    return RedirectToPage("/Profile/ProfileEdit",
                        new { userId, profileId = ProfileToUpdate.Id, returnUrl, UpdateSuccessfulMessageHidden = "show" });
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    var profileValues = (ProfilesModel)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        return RedirectToPage(returnUrl, new { userId, deletedProfile = true });
                    }

                    var profileInputValues = (ProfilesModel)databaseEntry.ToObject();
                    await SetChangeMessages(profileInputValues, profileValues, _context);

                    Profile.RowVersion = (byte[])profileInputValues.RowVersion;
                    ModelState.Remove("Profile.RowVersion");
                }
            }

            TempData["UserId"] = userId;
            TempData["ProfileId"] = profileId;
            TempData["ReturnUrl"] = returnUrl;
            TempData["ProfileName"] = Profile.FirstName + "  " + Profile.LastName;
            TempData["UpdateSuccessfulMessageHidden"] = "hidden";
            TempData["UpdateUnSuccessfulMessageHidden"] = "";

            return Page();

        }
        private Task SetChangeMessages(ProfilesModel profileInputValues,
             ProfilesModel profileValues, ApplicationDbContext context)
        {
            if (profileInputValues.ProfileImage != Profile.ProfileImage)
            {
                ModelState.AddModelError("Profile.ProfileImage",
                $"Current: { profileInputValues.ProfileImage}");
            }

            if (profileInputValues.AccountNumber != Profile.AccountNumber)
            {
                ModelState.AddModelError("Profile.AccountNumber",
                $"Current: { profileInputValues.AccountNumber}");
            }

            if (profileInputValues.FirstName != Profile.FirstName)
            {
                ModelState.AddModelError("Profile.FirstName",
                $"Current: { profileInputValues.FirstName}");
                profileInputValues.FirstName = "profileInputValues";
                Profile.FirstName = "ProfileFirstname";
            }

            if (profileInputValues.LastName != Profile.LastName)
            {
                ModelState.AddModelError("Profile.LastName",
                $"Current: { profileInputValues.LastName}");
            }

            if (profileInputValues.AddressLine1 != Profile.AddressLine1)
            {
                ModelState.AddModelError("Profile.AddressLine1",
                $"Current: { profileInputValues.AddressLine1}");
            }

            if (profileInputValues.AddressLine2 != Profile.AddressLine2)
            {
                ModelState.AddModelError("Profile.AddressLine2",
                $"Current: { profileInputValues.AddressLine2}");
            }
            if (profileInputValues.City != Profile.City)
            {
                ModelState.AddModelError("Profile.City",
                $"Current: { profileInputValues.City}");
            }

            if (profileInputValues.State != Profile.State)
            {
                ModelState.AddModelError("Profile.State",
                $"Current: { profileInputValues.State}");
            }

            if (profileInputValues.ZipCode != Profile.ZipCode)
            {
                ModelState.AddModelError("Profile.ZipCode",
                $"Current: { profileInputValues.ZipCode}");
            }

            return Task.CompletedTask;
        }
    }
}
