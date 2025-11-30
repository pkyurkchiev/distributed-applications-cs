using Microsoft.AspNetCore.Identity;

namespace PlantShop.Models
{
    public class EditUserRolesViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }

        public List<IdentityRole> Roles { get; set; }
        public IList<string> UserRoles { get; set; }

        public string SelectedRole { get; set; }
    }
}
