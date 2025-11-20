using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tsg.UI.Main.Attributes;

namespace Tsg.UI.Main.Providers
{
    public class TsgMetaModelProvider : DataAnnotationsModelMetadataProvider
    {
        protected override ModelMetadata CreateMetadata(
            IEnumerable<Attribute> attributes,
            Type containerType,
            Func<object> modelAccessor,
            Type modelType,
            string propertyName)
        {
            if (attributes.FirstOrDefault(a => a.GetType() == typeof(BaseTypeAttribute)) is BaseTypeAttribute attribute && modelAccessor != null)
            {
                var target = modelAccessor.Target;
                var containerField = target.GetType().GetField("container");
                if (containerField == null)
                {
                    var vdi = target.GetType().GetField("vdi").GetValue(target) as ViewDataInfo;
                    var concreteType = vdi.Container.GetType();
                    return base.CreateMetadata(attributes, concreteType, modelAccessor, modelType, propertyName);
                }
                else
                {
                    var container = containerField.GetValue(target);
                    var concreteType = container.GetType();
                    var propertyField = target.GetType().GetField("property");
                    if (propertyField == null)
                    {
                        concreteType = base.GetMetadataForProperties(container, containerType)
                            .FirstOrDefault(p => p.PropertyName == "ConcreteType")
                            ?.Model as System.Type;
                        if (concreteType != null)
                            return base.GetMetadataForProperties(container, concreteType)
                                .FirstOrDefault(pr => pr.PropertyName == propertyName);
                    }
                    return base.CreateMetadata(attributes, concreteType, modelAccessor, modelType, propertyName);
                }
            }
            return base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);
        }
    }
}