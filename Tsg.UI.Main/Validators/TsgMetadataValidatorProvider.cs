using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tsg.UI.Main.Attributes;

namespace Tsg.UI.Main.Validators
{
    public class TsgMetadataValidatorProvider : DataAnnotationsModelValidatorProvider
    {
        protected override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context,
            IEnumerable<Attribute> attributes)
        {
            List<ModelValidator> vals = base.GetValidators(metadata, context, attributes).ToList();

            var baseTypeAttribute = attributes.FirstOrDefault(a => a.GetType().Equals(typeof(BaseTypeAttribute)))
                as BaseTypeAttribute;

            if (baseTypeAttribute != null)
            {
                // get our parent model
                var parentMetaData = ModelMetadataProviders.Current.GetMetadataForProperties(
                    context.Controller.ViewData.Model,
                    metadata.ContainerType);

                // get the concrete type
                var concreteType = parentMetaData.FirstOrDefault(p => p.PropertyName == "ConcreteType").Model;
                if (concreteType != null)
                {
                    var concreteMetadata = ModelMetadataProviders.Current.GetMetadataForProperties(
                        context.Controller.ViewData.Model,
                        Type.GetType(concreteType.ToString()));

                    var concretePropertyMetadata =
                        concreteMetadata.FirstOrDefault(p => p.PropertyName == metadata.PropertyName);

                    vals = base.GetValidators(concretePropertyMetadata, context, attributes).ToList();
                }
            }
            return vals.AsEnumerable();
        }
    }
}