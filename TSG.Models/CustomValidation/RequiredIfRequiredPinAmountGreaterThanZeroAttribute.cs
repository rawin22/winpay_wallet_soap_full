using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TSG.Models.CustomValidation
{
    public class RequiredIfRequiredPinAmountGreaterThanZeroAttribute : ValidationAttribute //, IClientValidatable
    {
        private readonly string _otherProperty;
        public RequiredIfRequiredPinAmountGreaterThanZeroAttribute()
        {
        }
        public RequiredIfRequiredPinAmountGreaterThanZeroAttribute(string otherProperty)
        {
            _otherProperty = otherProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(_otherProperty);
            
            if (property == null)
            {
                return new ValidationResult(string.Format(
                    CultureInfo.CurrentCulture,
                    "Unknown property {0}",
                    new[] { _otherProperty }
                ));
            }
            var otherPropertyValue = property.GetValue(validationContext.ObjectInstance, null) as Decimal?;

            if (otherPropertyValue != null && otherPropertyValue.HasValue && otherPropertyValue.Value > 0)
            {
                if (value == null || value as string == string.Empty)
                {
                    return new ValidationResult(string.Format(
                        CultureInfo.CurrentCulture,
                        FormatErrorMessage(validationContext.DisplayName),
                        new[] { _otherProperty }
                    ));
                }
            }

            return null;
        }

        //public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        //{
        //    var rule = new ModelClientValidationRule
        //    {
        //        ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
        //        ValidationType = "requiredif",
        //    };
        //    rule.ValidationParameters.Add("other", _otherProperty);
        //    yield return rule;
        //}
    }
}