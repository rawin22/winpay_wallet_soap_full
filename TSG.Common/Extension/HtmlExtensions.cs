using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WinstantPay.Common.Extension
{
    public class LocalizedDisplayNameAttribute : DisplayNameAttribute
    {
        private readonly PropertyInfo nameProperty;

        public LocalizedDisplayNameAttribute(string displayNameKey, Type resourceType = null)
            : base(displayNameKey)
        {
            if (resourceType != null)
            {
                nameProperty = resourceType.GetProperty(base.DisplayName,
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            }
        }

        public override string DisplayName
        {
            get
            {
                if (nameProperty == null)
                {
                    return base.DisplayName;
                }
                return (string)nameProperty.GetValue(nameProperty.DeclaringType, null);
            }
        }
    }

    public static class HtmlExtensions
    {
        public static string IsSelected(this HtmlHelper html, string controller = null, string action = null)
        {
            string cssClass = "active open";
            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            string currentController = (string)html.ViewContext.RouteData.Values["controller"];

            if (String.IsNullOrEmpty(controller))
                controller = currentController;

            if (String.IsNullOrEmpty(action))
                action = currentAction;

            return controller == currentController && action == currentAction ?
                cssClass : String.Empty;
        }
        public static string PageClass(this HtmlHelper html)
        {
            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            return currentAction;
        }

        public static HtmlString EnumDisplayNameFor(this Enum item)
        {
            var type = item.GetType();
            var member = type.GetMember(item.ToString());
            DisplayAttribute displayName = (DisplayAttribute)member[0].GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();

            if (displayName != null)
            {
                return new HtmlString(displayName.Name);
            }

            return new HtmlString(item.ToString());
        }

        //[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
        //public class IgnoreValidationAttribute : FilterAttribute, IAuthorizationFilter
        //{
        //    // TODO: Try to put it on another more appropriate method such as OnActionExcecuting.
        //    // Looks like - This is the earliest method we can interpret before an action. I really dont like this!
        //    public void OnAuthorization(AuthorizationContext filterContext)
        //    {
        //        //TODO: filterContext != null && filterContext.httpContext != null
        //        var itemKey = this.CreateKey(filterContext.ActionDescriptor);
        //        if (!filterContext.HttpContext.Items.Contains(itemKey))
        //        {
        //            filterContext.HttpContext.Items.Add(itemKey, true);
        //        }
        //    }

        //    private string CreateKey(ActionDescriptor actionDescriptor)
        //    {
        //        var action = actionDescriptor.ActionName.ToLower();
        //        var controller = actionDescriptor.ControllerDescriptor.ControllerName.ToLower();
        //        return string.Format("IgnoreValidation_{0}_{1}", controller, action);
        //    }
        //}

        //public class IgnoreValidationModelMetaData : DataAnnotationsModelMetadata
        //{
        //    public IgnoreValidationModelMetaData(DataAnnotationsModelMetadataProvider provider, Type containerType,
        //        Func<object> modelAccessor, Type modelType, string propertyName,
        //        DisplayColumnAttribute displayColumnAttribute) :
        //        base(provider, containerType, modelAccessor, modelType, propertyName, displayColumnAttribute)
        //    {
        //    }

        //    public override IEnumerable<ModelValidator> GetValidators(ControllerContext context)
        //    {
        //        var itemKey = this.CreateKey(context.RouteData);

        //        if (context.HttpContext.Items[itemKey] != null && bool.Parse(context.HttpContext.Items[itemKey].ToString()) == true)
        //        {
        //            return Enumerable.Empty<ModelValidator>();
        //        }

        //        return base.GetValidators(context);
        //    }

        //    private string CreateKey(RouteData routeData)
        //    {
        //        var action = (routeData.Values["action"] ?? null).ToString().ToLower();
        //        var controller = (routeData.Values["controller"] ?? null).ToString().ToLower();
        //        return string.Format("IgnoreValidation_{0}_{1}", controller, action);
        //    }
        //}
        //public class IgnoreValidationModelMetaDataProvider : DataAnnotationsModelMetadataProvider
        //{
        //    protected override ModelMetadata CreateMetadata(IEnumerable<Attribute> attributes,
        //        Type containerType, Func<object> modelAccessor, Type modelType, string propertyName)
        //    {
        //        var displayColumnAttribute = new List<Attribute>(attributes).OfType<DisplayColumnAttribute>().FirstOrDefault();

        //        var baseMetaData = base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);

        //        // is there any other good strategy to copy the properties?
        //        return new IgnoreValidationModelMetaData(this, containerType, modelAccessor, modelType, propertyName, displayColumnAttribute)
        //        {
        //            TemplateHint = baseMetaData.TemplateHint,
        //            HideSurroundingHtml = baseMetaData.HideSurroundingHtml,
        //            DataTypeName = baseMetaData.DataTypeName,
        //            IsReadOnly = baseMetaData.IsReadOnly,
        //            NullDisplayText = baseMetaData.NullDisplayText,
        //            DisplayFormatString = baseMetaData.DisplayFormatString,
        //            ConvertEmptyStringToNull = baseMetaData.ConvertEmptyStringToNull,
        //            EditFormatString = baseMetaData.EditFormatString,
        //            ShowForDisplay = baseMetaData.ShowForDisplay,
        //            ShowForEdit = baseMetaData.ShowForEdit,
        //            Description = baseMetaData.Description,
        //            ShortDisplayName = baseMetaData.ShortDisplayName,
        //            Watermark = baseMetaData.Watermark,
        //            Order = baseMetaData.Order,
        //            DisplayName = baseMetaData.DisplayName,
        //            IsRequired = baseMetaData.IsRequired
        //        };
        //    }
        //}

        public class NoValidateAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid
                (object value, ValidationContext validationContext)
            {
                return ValidationResult.Success;
            }
        }

        public static string MapPathReverse(string fullServerPath)
        {
            return @"~\" + fullServerPath.Replace(HttpContext.Current.Request.PhysicalApplicationPath ?? string.Empty, String.Empty);
        }
    }
}