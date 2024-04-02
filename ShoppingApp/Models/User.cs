using System.ComponentModel.DataAnnotations;

namespace ShoppingApp.Models
{
    public class User
    {
        public string Id { get; set; }
        [Required,MinLength(2,ErrorMessage ="Minimun length is 2")]
        [Display(Name ="UserName")]
        public string UserName { get; set; }
        [Required,EmailAddress]
        public string EmailAddress { get; set; }
        [DataType(DataType.Password),Required,MinLength(4,ErrorMessage = "Minimun length is 4")]
        public string Password { get; set; }
    }
}
