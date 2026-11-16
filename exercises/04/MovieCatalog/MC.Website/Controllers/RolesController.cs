using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MC.Website.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager ?? throw new NullReferenceException(nameof(roleManager));
            _userManager = userManager ?? throw new NullReferenceException(nameof(userManager));
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        public IActionResult Create()
        {
            return View(new IdentityRole());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IdentityRole role)
        {
            if (ModelState.IsValid)
            {
                await _roleManager.CreateAsync(role);
                return RedirectToAction("Index");
            }

            return View(role);
        }

        [HttpGet]
        public ActionResult ManageRoleToUser()
        {
            ViewData["Roles"] = new SelectList(_roleManager.Roles, "Name", "Name");
            return View();
        }

        [HttpPost, ActionName("ManageRoleToUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRoleToUserConfirm(string userEmail, string roleName)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email!.Contains(userEmail));

            if(user ==null)
            {
                return NotFound();
            }

            await _userManager.AddToRoleAsync(user, roleName);

            return RedirectToAction("Index", "Roles");
        }
    }
}
