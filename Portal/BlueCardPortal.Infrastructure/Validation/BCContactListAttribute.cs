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
    public class BCContactListAttribute : ValidationAttribute
    {
        public string? ErrorMessagePrefered { get; set; }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (validationContext.ObjectInstance == null)
                return new ValidationResult(ErrorMessage);
            var contactList = (ContactListVM)validationContext.ObjectInstance;
            if (contactList.Items == null || contactList.Items.Count == 0)
                return new ValidationResult(ErrorMessage);
            if (!contactList.Items.Any(x => !string.IsNullOrEmpty(x.IsPreferedContract)))
                return new ValidationResult(ErrorMessagePrefered);
            return ValidationResult.Success;
        }
    }
}
