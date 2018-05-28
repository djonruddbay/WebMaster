using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebMaster.Data;
using WebMaster.Models;

namespace WebMaster.Pages.Profile
{
    [Authorize(Roles = "WebMastr")]

    public class ProfileIndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ProfileIndexModel(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public PaginatedList<ProfilesModel> ProfilesModel { get; set; }

        public string IdSort { get; set; }      
        public string NameSort { get; set; }
        public string AccountNumberSort { get; set; }
        public string DateCreateSort { get; set; }
        public string DateLastUpdateSort { get; set; }
        public string UserIdSort { get; set; }
        public string SearchBy { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public async Task OnGetAsync(
            string sortOrder, 
            string currentFilter,
            string searchBy,
            string searchString,
            bool deletedProfile,
            int? pageIndex)
        {
            TempData["DeletedProfileMsgHidden"] = deletedProfile ? "" : "hidden";

            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = String.IsNullOrEmpty(searchString) ? "" : searchString.ToUpper();
            SearchBy = searchBy;
            CurrentSort = sortOrder;

            IdSort = String.IsNullOrEmpty(sortOrder) ? "Id_desc" : "";
            NameSort = sortOrder == "Name" ? "Name_desc" : "Name";
            AccountNumberSort = sortOrder == "AccountNumber" ? "AccountNumber_desc" : "AccountNumber";
            DateCreateSort = sortOrder == "DateCreate" ? "DateCreate_desc" : "DateCreate";
            DateLastUpdateSort = sortOrder == "DateLastUpdate" ? "DateLastUpdate_desc" : "DateLastUpdate";
            UserIdSort = sortOrder == "UserId" ? "UserId_desc" : "UserId";

            IQueryable<ProfilesModel> profileIQ = from p in _context.Profiles  select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                if (searchBy == "Id")
                {
                    profileIQ = profileIQ
                        .Where(u => u.Id.ToString().Contains(searchString));
                }
                else if (searchBy == "LastName")
                {
                    profileIQ = profileIQ
                        .Where(u => u.LastName.ToUpper().Contains(searchString) || u.FirstName.ToUpper().Contains(searchString));
                }
                else if (searchBy == "UserId")
                {
                    profileIQ = profileIQ
                       .Where(u => u.UserId.Contains(searchString));
                }
            }

            switch (sortOrder)
            {
                case "Id_desc":
                    profileIQ = profileIQ.OrderByDescending(s => s.Id);
                    break;
                case "Name":
                    profileIQ = profileIQ.OrderBy(s => s.LastName + s.FirstName);
                    break;
                case "Name_desc":
                    profileIQ = profileIQ.OrderByDescending(s => s.LastName + s.FirstName);
                    break;
                case "AccountNumber":
                    profileIQ = profileIQ.OrderBy(s => s.AccountNumber);
                    break;
                case "AccountNumber_desc":
                    profileIQ = profileIQ.OrderByDescending(s => s.AccountNumber);
                    break;
                case "DateCreate":
                    profileIQ = profileIQ.OrderBy(s => s.DateCreate);
                    break;
                case "DateCreate_desc":
                    profileIQ = profileIQ.OrderByDescending(s => s.DateCreate);
                    break;
                case "DateLastUpdate":
                    profileIQ = profileIQ.OrderBy(s => s.DateLastUpdate);
                    break;
                case "DateLastUpdate_desc":
                    profileIQ = profileIQ.OrderByDescending(s => s.DateLastUpdate);
                    break;
                case "UserId":
                    profileIQ = profileIQ.OrderBy(s => s.UserId);
                    break;
                case "UserId_desc":
                    profileIQ = profileIQ.OrderByDescending(s => s.UserId);
                    break;
                default:
                    profileIQ = profileIQ.OrderBy(s => s.Id);
                    break;
            }

            int pageSize = 5;

            ProfilesModel = await PaginatedList<ProfilesModel>.CreateAsync(
                profileIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}
