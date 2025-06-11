using Datas.ViewModels.Product;
using System.ComponentModel.DataAnnotations;

namespace Datas.Extensions.Validations
{
    public class PriceGreaterThanPromotionPriceAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var model = (CreateProductViewModel)validationContext.ObjectInstance;

            if (model.Promotion_Price.HasValue && model.Price <= model.Promotion_Price.Value)
            {
                return new ValidationResult("Promotional price must be less than price!");
            }

            return ValidationResult.Success;
        }
    }
}
