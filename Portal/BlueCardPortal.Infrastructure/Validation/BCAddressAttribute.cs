// Copyright (C) Information Services. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using BlueCardPortal.Infrastructure.Model.Application;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace BlueCardPortal.Infrastructure.Validation
{
    public class BCAddressAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (validationContext.ObjectInstance == null)
                return new ValidationResult(ErrorMessage);
            var address = (AddressVM)validationContext.ObjectInstance;
            if (!string.IsNullOrEmpty(address.Street) && !string.IsNullOrEmpty(address.StreetNo))
                return ValidationResult.Success;
            if (!string.IsNullOrEmpty(address.Quarter) && !string.IsNullOrEmpty(address.BuildingNo))
                return ValidationResult.Success;
            if (address.IsCompanyAddress)
            {
                if (!string.IsNullOrEmpty(address.Street))
                    return ValidationResult.Success;
                if (!string.IsNullOrEmpty(address.Quarter))
                    return ValidationResult.Success;
            }
            return new ValidationResult(ErrorMessage);
        }
    }
}
