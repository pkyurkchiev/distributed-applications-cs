using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlantShop.Models;

[Authorize(Roles = "Admin")]
public class RolesController : Controller
{
    private readonly UserManager<Users> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public RolesController(UserManager<Users> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // лист с всички потребители
    public IActionResult Index()
    {
        var users = _userManager.Users.ToList();
        return View(users);
    }

    // GET: Промяна на роли
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        var model = new EditUserRolesViewModel
        {
            UserId = user.Id,
            Email = user.Email,
            Roles = _roleManager.Roles.ToList(),
            UserRoles = await _userManager.GetRolesAsync(user)
        };

        return View(model);
    }

    // POST: Запазване на новата роля
    [HttpPost]
    public async Task<IActionResult> Edit(EditUserRolesViewModel model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);

        var currentUserId = _userManager.GetUserId(User);
        var currentUserRoles = await _userManager.GetRolesAsync(user);

        // Admin-ът не може сам да премахва собствената си роля
        if (user.Id == currentUserId && model.SelectedRole != "Admin")
        {
            ModelState.AddModelError("", "You cannot demote your own administrator role.");

            // Премахваме ненужни валидации
            ModelState.Remove("Email");
            ModelState.Remove("Roles");
            ModelState.Remove("UserRoles");

            model.Roles = _roleManager.Roles.ToList();
            model.UserRoles = currentUserRoles;
            return View(model);
        }

        // Проверка - трябва винаги поне един Admin да има
        var allAdmins = await _userManager.GetUsersInRoleAsync("Admin");

        if (allAdmins.Count == 1 && allAdmins.First().Id == user.Id && model.SelectedRole != "Admin")
        {
            ModelState.AddModelError("", "Error.");
            model.Roles = _roleManager.Roles.ToList();
            model.UserRoles = currentUserRoles;
            return View(model);
        }

        // Премахване на всички роли
        await _userManager.RemoveFromRolesAsync(user, currentUserRoles);

        // Добавяне на новата роля
        await _userManager.AddToRoleAsync(user, model.SelectedRole);

        return RedirectToAction("Index");
    }


    // GET: Roles/Delete/id
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        return View(user);
    }

    // POST: Roles/DeleteConfirmed
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var currentUserId = _userManager.GetUserId(User);

        // Не може да изтрие себе си
        if (user.Id == currentUserId)
        {
            ModelState.AddModelError("", "You cannot delete your own account.");
            return View(user);
        }

        // Не може да изтрие последния администратор
        var admins = await _userManager.GetUsersInRoleAsync("Admin");
        if (admins.Count == 1 && admins.First().Id == user.Id)
        {
            ModelState.AddModelError("", "You cannot delete the last administrator.");
            return View(user);
        }

        await _userManager.DeleteAsync(user);

        return RedirectToAction("Index");
    }
}
