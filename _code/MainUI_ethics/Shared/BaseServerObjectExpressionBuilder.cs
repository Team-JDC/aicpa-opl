using System;
using System.Web;
using System.Web.Compilation;
using System.CodeDom;
using System.ComponentModel;
using System.Configuration;

namespace skmExpressionBuilders
{
    public abstract class BaseServerObjectExpressionBuilder : ExpressionBuilder
    {
        protected abstract object GetValue(string key);
        public abstract string SourceObjectName { get; }

        public override object EvaluateExpression(object target, System.Web.UI.BoundPropertyEntry entry, object parsedData, ExpressionBuilderContext context)
        {
            return GetRequestedValue(entry.Expression.Trim(), target.GetType(), entry.PropertyInfo.Name);
        }

        public override CodeExpression GetCodeExpression(System.Web.UI.BoundPropertyEntry entry, object parsedData, ExpressionBuilderContext context)
        {
            CodeExpression[] inputParams = new CodeExpression[] { new CodePrimitiveExpression(entry.Expression.Trim()), 
                                                                             new CodeTypeOfExpression(entry.DeclaringType), 
                                                                             new CodePrimitiveExpression(entry.PropertyInfo.Name) };

            // Return a CodeMethodInvokeExpression that will invoke the GetRequestedValue method using the specified input parameters
            return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(this.GetType()),
                                                  "Instance().GetRequestedValue",
                                                  inputParams);
        }

        public object GetRequestedValue(string key, Type targetType, string propertyName)
        {
            // First make sure that the server object will be available
            if (HttpContext.Current == null)
                return null;

            // Get the value
            object value = this.GetValue(key);

            // Make sure that the value exists
            if (value == null)
            {
                if (key.Equals("CustomThemeLocation"))
                {
                    value = ConfigurationManager.AppSettings["CustomThemeLocation"];
                    HttpContext.Current.Session["CustomThemeLocation"] = ConfigurationManager.AppSettings["CustomThemeLocation"];
                }
                else
                    return null;// throw new InvalidOperationException(string.Format("{0} field '{1}' not found.", this.SourceObjectName, key));
            }

            // If the value is being assigned to a control property we may need to convert it
            if (targetType != null)
            {
                PropertyDescriptor propDesc = TypeDescriptor.GetProperties(targetType)[propertyName];
                if (propDesc != null && propDesc.PropertyType != value.GetType())
                {
                    // Type mismatch - make sure that the value can be converted
                    if (propDesc.Converter.CanConvertFrom(value.GetType()) == false)
                        throw new InvalidOperationException(string.Format("{0} value '{1}' cannot be converted to type {2}.", this.SourceObjectName, key, propDesc.PropertyType.ToString()));
                    else
                        return propDesc.Converter.ConvertFrom(value);
                }
            }

            // If we reach here, no type mismatch - return the value
            return value;
        }

        public override bool SupportsEvaluate
        {
            get
            {
                return true;
            }
        }
    }
}
