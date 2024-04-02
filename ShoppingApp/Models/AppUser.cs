using Microsoft.AspNetCore.Identity;

namespace ShoppingApp.Models
{
    public class AppUser:IdentityUser
    {
        public string Occupation {  get; set; }

    }
}
