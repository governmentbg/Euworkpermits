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
    public class BCContactListAttributeAdapter : BCAttributeAdapterBase<BCContactListAttribute>
    {
        public BCContactListAttributeAdapter(
            BCContactListAttribute attribute, IStringLocalizer? stringLocalizer)
            : base(attribute, stringLocalizer)
        {

        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-bccontact-list", GetErrorMessage(context));
            MergeAttribute(context.Attributes, "data-val-bccontact-list-error-count", GetLocalizedMessage(Attribute.ErrorMessage));
            MergeAttribute(context.Attributes, "data-val-bccontact-list-error-prefered", GetLocalizedMessage(Attribute.ErrorMessagePrefered));

        }
    }
}
