// Copyright (C) Information Services. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using BlueCardPortal.Infrastructure.Model.Application;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace BlueCardPortal.Infrastructure.Validation
{
    public class BCPersonIdDocumentListAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (validationContext.ObjectInstance == null)
                return new ValidationResult(ErrorMessage);
            var personIdDocumentList = (PersonIdDocumentListVM)validationContext.ObjectInstance;
            return ValidationResult.Success;
        }
 
    }
}
