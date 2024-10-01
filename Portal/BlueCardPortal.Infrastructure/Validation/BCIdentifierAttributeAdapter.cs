// Copyright (C) Information Services. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace BlueCardPortal.Infrastructure.Validation
{
    public class BCIdentifierAttributeAdapter : BCAttributeAdapterBase<BCIdentifierAttribute>
    {
        public BCIdentifierAttributeAdapter(
            BCIdentifierAttribute attribute, IStringLocalizer? stringLocalizer)
            : base(attribute, stringLocalizer)
        {

        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-bc-identifier", GetErrorMessage(context));
            MergeAttribute(context.Attributes, "data-val-uic-type", Attribute.UicType.ToString());
        }
    }
}
