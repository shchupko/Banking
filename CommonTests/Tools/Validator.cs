using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Moq;


namespace Banking.CommonTests
{
    public class ValidatorException : Exception
    {
        public ValidationAttribute Attribute { get; private set; }

        public ValidatorException(ValidationException ex, ValidationAttribute attribute)
            : base(attribute.GetType().Name, ex)
        {
            Attribute = attribute;
        }
    }

    /// <summary>
    /// Instance of a controller for testing things that use controller methods i.e. controller.TryValidateModel(model)
    /// </summary>
    public class ModelStateTestController : Controller
    {
        public ModelStateTestController()
        {
            ControllerContext = (new Mock<ControllerContext>()).Object;
        }

        public bool TestTryValidateModel(object model)
        {
            return TryValidateModel(model);
        }
    }

    public class ValidatorTool
    {
        public static IList<ValidationResult> GetValidationErrors(object model)
        {
            var validationContext = new ValidationContext(model, null, null);
            var validationResults = new List<ValidationResult>();

            // fix of: MVC Controller post method unit test: ModelState.IsValid always true
            //TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(POSViewModel), typeof(POSViewModel)), typeof(POSViewModel));

            System.ComponentModel.DataAnnotations.Validator.TryValidateObject(model, validationContext, validationResults);
            return validationResults;
        }

        public static void ValidateObject<T>(T obj)
        {
            var type = typeof(T);
            var meta = type.GetCustomAttributes(false).OfType<MetadataTypeAttribute>().FirstOrDefault();
            if (meta != null)
            {
                type = meta.MetadataClassType;
            }

            var typeAttributes = type.GetCustomAttributes(typeof(ValidationAttribute), true).OfType<ValidationAttribute>();
            var validationContext = new ValidationContext(obj);
            foreach (var attribute in typeAttributes)
            {
                try
                {
                    attribute.Validate(obj, validationContext);
                }
                catch (ValidationException ex)
                {
                    throw new ValidatorException(ex, attribute);
                }
            }

            var propertyInfo = type.GetProperties();
            foreach (var info in propertyInfo)
            {
                var attributes = info.GetCustomAttributes(typeof(ValidationAttribute), true).OfType<ValidationAttribute>();
                foreach (var attribute in attributes)
                {
                    var objPropInfo = obj.GetType().GetProperty(info.Name);
                    try
                    {
                        attribute.Validate(objPropInfo.GetValue(obj, null), validationContext);
                    }
                    catch (ValidationException ex)
                    {
                        throw new ValidatorException(ex, attribute);
                    }
                }
            }
        }
    }
}
