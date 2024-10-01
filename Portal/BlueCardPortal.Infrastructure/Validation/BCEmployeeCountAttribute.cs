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
    public class BCEmployeeCountAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (validationContext.ObjectInstance == null)
                return new ValidationResult(ErrorMessage);
            var employer = (EmployerVM)validationContext.ObjectInstance;
            if (employer.ForeignEmployeeCount > employer.EmployeeCount)
                return new ValidationResult(ErrorMessage);
            return ValidationResult.Success;
        }
    }
}
