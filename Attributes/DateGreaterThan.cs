using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MVCBookingFinal_YARAB_.Attributes
{
	public class DateGreaterThanAttribute: ValidationAttribute
	{
		private readonly DateTime _comparedto;

		public DateGreaterThanAttribute(DateTime comparedto)
		{
			_comparedto = comparedto;

		}
		protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
		{
			if (value == null || !(value is DateTime currentDate))
			{
				return new ValidationResult("The current date must be a valid DateTime.");
			}

			
			//var comparisonPropertyInfo = validationContext.ObjectType.GetProperty(_comparedto);
			//if (comparisonPropertyInfo == null)
			//{
			//	return new ValidationResult($"Unknown property: {_comparedto}");
			//}

			//var comparisonDate = comparisonPropertyInfo.GetValue(validationContext.ObjectInstance) as DateTime?;

			// Ensure the comparison date is valid and not null
			//if (!comparisonDate.HasValue)
			//{
			//	return new ValidationResult("The comparison date must be a valid DateTime.");
			//}

			// If currentDate is less than or equal to the comparisonDate, validation fails
			if (currentDate <= _comparedto)
			{
				return new ValidationResult(ErrorMessage);
			}

			// If everything is valid, return Success
			return ValidationResult.Success;
		}
	}
}
