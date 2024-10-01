using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Infrastructure.Validation
{

    public abstract class BCAttributeAdapterBase<TAttribute> : AttributeAdapterBase<TAttribute>  where TAttribute : ValidationAttribute
    {
        private readonly IStringLocalizer? stringLocalizer;
        public BCAttributeAdapterBase(
            TAttribute attribute, IStringLocalizer? stringLocalizer)
            : base(attribute, stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
        }
        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            var errorMessage = !string.IsNullOrEmpty(Attribute.ErrorMessage) ? Attribute.ErrorMessage : "BCAttributeErrorMessage";
            return stringLocalizer != null ? stringLocalizer[errorMessage] : errorMessage;
        }
        public string GetLocalizedMessage(string? message)
        {
            return stringLocalizer != null ? stringLocalizer[message??string.Empty] : message ?? string.Empty;
        }
    }
}
