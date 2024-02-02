using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TKC.Areas.Identity.Pages.Account
{
	public class AssignRolesModel : PageModel
    {
        public List<IdentityUser> Users { get; set; }
        public List<IdentityRole> RoleNames { get; set; }

        [BindProperty]
        public string SelectedUserEmail { get; set; }

        [BindProperty]
        public string SelectedRoleName { get; set; }

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<LoginModel> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AssignRolesModel(ILogger<LoginModel> logger, IServiceProvider serviceProvider
            , RoleManager<IdentityRole> roleManager
            , UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _roleManager = roleManager;
            _userManager = userManager;
            RoleNames = new List<IdentityRole>();
            Users = new List<IdentityUser>();
        }

        public void OnGet()
        {
            RoleNames = _roleManager.Roles.ToList();
            Users = _userManager.Users.ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                await AssignRole(SelectedUserEmail, SelectedRoleName);
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) return false;

            if (!await _userManager.IsInRoleAsync(user, roleName))
            {
                await _userManager.AddToRoleAsync(user, roleName);
            }
            return true;
        }
    }
}
