using Microsoft.AspNetCore.Identity;

namespace PlantShop.Models
{
    public class Users : IdentityUser
    {
        public string FullName { get; set; }
    }
}
