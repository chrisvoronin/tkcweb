using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace TKC.Areas.Identity.Pages.Account
{
    //(Roles = "Admin")
    [Authorize]
    public class ManageRolesModel : PageModel
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<LoginModel> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty]
        public string NewRoleName { get; set; }

        public List<IdentityRole> RoleNames { get; set; }

        public ManageRolesModel(ILogger<LoginModel> logger, IServiceProvider serviceProvider
            , RoleManager<IdentityRole> roleManager
            , UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _roleManager = roleManager;
            _userManager = userManager;
            RoleNames = new List<IdentityRole>();
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            await CreateRole(roleName);

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) return false;

            if (!await _userManager.IsInRoleAsync(user, roleName))
            {
                await _userManager.AddToRoleAsync(user, roleName);
            }
            return true;
        }

        public async Task<List<IdentityRole>> GetAllRolesAsync()
        {
            return await _roleManager.Roles.ToListAsync();
        }

        public async Task CreateRole(string roleName)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            RoleNames = await GetAllRolesAsync();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                await CreateRole(NewRoleName);
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
