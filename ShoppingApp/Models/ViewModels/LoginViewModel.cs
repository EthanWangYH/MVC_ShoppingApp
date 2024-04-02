using System.ComponentModel.DataAnnotations;

namespace ShoppingApp.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required, MinLength(2, ErrorMessage = "Minimum length is 2")]
        [Display(Name = "UserName")]
        public string UserName { get; set; }
        [Required,DataType(DataType.Password),MinLength(4,ErrorMessage = "Minimum length is 4")]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}
