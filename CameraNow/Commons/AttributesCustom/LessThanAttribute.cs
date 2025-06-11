using System.ComponentModel.DataAnnotations;

namespace Commons.AttributesCustom
{
    public class LessThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public Type? ErrorResourceType { get; set; }

        public LessThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
           if (value != null)
            {
                var currentValue = (decimal)value;

                var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

                if (property == null)
                    throw new ArgumentException("Property with this name not found");

                var comparisonValue = (decimal)property.GetValue(validationContext.ObjectInstance);

                if (currentValue >= comparisonValue)
                    return new ValidationResult(ErrorMessage = ErrorMessageString);
            }

            return ValidationResult.Success;
        }



    }
}
