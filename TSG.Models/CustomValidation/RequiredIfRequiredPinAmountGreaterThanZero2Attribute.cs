using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TSG.Models.ServiceModels.LimitPayment;

namespace TSG.Models.CustomValidation
{
    public class RequiredIfRequiredPinAmountGreaterThanZero2Attribute : ValidationAttribute //, IClientValidatable
    {
        public RequiredIfRequiredPinAmountGreaterThanZero2Attribute()
        {
        }
        //public RequiredIfRequiredPinAmountGreaterThanZeroAttribute(string otherProperty)
        //{
        //    _otherProperty = otherProperty;
        //}

        //protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        //{
        //    var model = (DaPayLimitsSo)validationContext.ObjectInstance;
        //    string _pin = Convert.ToString(value);
        //    //decimal _requiredPinAmount = Convert.ToDecimal(model.DaPayLimits_RequiredPinAmount);

        //    //if (_requiredPinAmount > 0 && String.IsNullOrEmpty(_pin))
        //    //{
        //    //    return new ValidationResult
        //    //        ("PIN is required if limit amount is greater than zero");
        //    //}
        //    ////else if (_lastDeliveryDate > DateTime.Now)
        //    ////{
        //    ////    return new ValidationResult
        //    ////        ("Last Delivery Date can not be greater than current date.");
        //    ////}
        //    //else
        //    //{
        //    //    return ValidationResult.Success;
        //    //}
        //}

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