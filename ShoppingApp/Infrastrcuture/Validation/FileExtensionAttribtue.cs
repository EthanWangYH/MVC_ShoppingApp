using System.ComponentModel.DataAnnotations;

namespace ShoppingApp.Infrastrcuture.Validation
{
    public class FileExtensionAttribtue : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName);
                string[] extensions = { "jpg", "png" };
                bool result = extensions.Any(x => extension.EndsWith(x));
                if (!result)
                {
                    return new ValidationResult("Allowed extensions are jpg and png");
                }
            }
            return ValidationResult.Success;
        }
    }
}
