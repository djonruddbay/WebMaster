using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebMaster.Data;

namespace WebMaster.Pages.UserAccount
{
    [Authorize(Roles = "WebMastr")]

    public class UserIndexModel : PageModel
    {    
        private readonly ApplicationDbContext _context;

        public UserIndexModel(ApplicationDbContext context)
        {
            _context = context;
        }
        public PaginatedList<ApplicationUser> ApplicationUser { get; set; }

        public string UserIdSort { get; set; }
        public string EmailSort { get; set; }
        public string PhoneSort { get; set; }
        public string SearchBy { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public async Task OnGetAsync(
            string sortOrder, 
            string currentFilter, 
            string searchBy, 
            string searchString,
            int? pageIndex)
        {           
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;
            SearchBy = searchBy;
            CurrentSort = sortOrder;

            UserIdSort = String.IsNullOrEmpty(sortOrder) ? "UserId_desc" : "";
            EmailSort = sortOrder == "Email" ? "Email_desc" : "Email";
            PhoneSort = sortOrder == "Phone" ? "Phone_desc" : "Phone";

            IQueryable<ApplicationUser>
                applicationUserIQ = from u in _context.ApplicationUser
                                    select u;

            if (!String.IsNullOrEmpty(searchString))
            {
                if (searchBy == "UserId")
                {
                    applicationUserIQ = applicationUserIQ
                        .Where(u => u.UserName.ToLower().Contains(searchString.ToLower()));
                }
                else if (searchBy == "EMail")
                {
                    applicationUserIQ = applicationUserIQ
                        .Where(u => u.Email.ToLower().Contains(searchString.ToLower()));
                }
                else if (searchBy == "Phone")
                {
                    applicationUserIQ = applicationUserIQ
                        .Where(u => u.PhoneNumber.Contains(searchString));
                }
            }

            switch (sortOrder)
            {
                case "UserId_desc":
                    applicationUserIQ = applicationUserIQ.OrderByDescending(u => u.UserName);
                    break;
                case "Email_desc":
                    applicationUserIQ = applicationUserIQ.OrderByDescending(u => u.Email);
                    break;
                case "Email":
                    applicationUserIQ = applicationUserIQ.OrderBy(u => u.Email);
                    break;
                case "Phone_desc":
                    applicationUserIQ = applicationUserIQ.OrderByDescending(u => u.PhoneNumber);
                    break;
                case "Phone":
                    applicationUserIQ = applicationUserIQ.OrderBy(u => u.PhoneNumber);
                    break;
                default:
                    applicationUserIQ = applicationUserIQ.OrderBy(u => u.UserName);
                    break;
            }
            int pageSize = 5;
            ApplicationUser = await PaginatedList<ApplicationUser>.CreateAsync(
                applicationUserIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}
