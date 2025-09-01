// Copyright (C) Information Services. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using BlueCardPortal.Infrastructure.Constants;
using BlueCardPortal.Infrastructure.Model.Application;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace BlueCardPortal.Infrastructure.Validation
{
    public class BCEmploymentDurationMonthAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (validationContext.ObjectInstance == null)
                return new ValidationResult(ErrorMessage);
            var employment = (EmploymentVM)validationContext.ObjectInstance;


            (int from, int to) = employment.EmploymentPermitType switch
             {
                 PERMIT_TYPE.BlueCard => (24, 60),
                 PERMIT_TYPE.UnifiedWorkPermit => (1, 36),
                 PERMIT_TYPE.SeasonalWorkerPermit => (3, 9),
                 PERMIT_TYPE.IntracorporateTransfer => (1, 36),
                 _ => (0,0)
             };
            if ( from <= employment.DurationOfEmploymentMonth && employment.DurationOfEmploymentMonth <= to)
                return ValidationResult.Success;
            return new ValidationResult(string.Format(ErrorMessage?? string.Empty, from, to));
        }
    }
}
