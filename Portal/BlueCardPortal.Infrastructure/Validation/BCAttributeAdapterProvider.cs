using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;

namespace BlueCardPortal.Infrastructure.Validation;

// <snippet_Class>
public class BCAttributeAdapterProvider : IValidationAttributeAdapterProvider
{
    private readonly IValidationAttributeAdapterProvider baseProvider =
        new ValidationAttributeAdapterProvider();

    public IAttributeAdapter? GetAttributeAdapter(
        ValidationAttribute attribute, IStringLocalizer? stringLocalizer)
    {
        if (attribute is BCRangeTodayDateAttribute bcRequiredToTodayAttribute)
        {
            return new BCRangeTodayDateAttributeAdapter(bcRequiredToTodayAttribute, stringLocalizer);
        }
        if (attribute is BCIdentifierAttribute bcIdentifierAttribute)
        {
            return new BCIdentifierAttributeAdapter(bcIdentifierAttribute, stringLocalizer);
        }
        if (attribute is BCAddressAttribute bcAddressAttribute)
        {
            return new BCAddressAttributeAdapter(bcAddressAttribute, stringLocalizer);
        }
        if (attribute is BCAddressListAttribute bcAddressListAttribute)
        {
            return new BCAddressListAttributeAdapter(bcAddressListAttribute, stringLocalizer);
        }
        if (attribute is BCContactPhoneAttribute bcContactPhoneAttribute)
        {
            return new BCContactPhoneAttributeAdapter(bcContactPhoneAttribute, stringLocalizer);
        }
        if (attribute is BCContactEmailAttribute bcContactEmailAttribute)
        {
            return new BCContactEmailAttributeAdapter(bcContactEmailAttribute, stringLocalizer);
        }
        if (attribute is BCContactListAttribute bcContactListAttribute)
        {
            return new BCContactListAttributeAdapter(bcContactListAttribute, stringLocalizer);
        }
        if (attribute is BCPersonIdDocumentListAttribute bcPersonIdDocumentListAttribute)
        {
            return new BCPersonIdDocumentListAttributeAdapter(bcPersonIdDocumentListAttribute, stringLocalizer);
        }
        if (attribute is BCDocumentsAttribute bcDocumentsAttribute)
        {
            return new BCDocumentsAttributeAdapter(bcDocumentsAttribute, stringLocalizer);
        }
        if (attribute is BCForeignerSmallListAttribute bcForeignerSmallListAttribute)
        {
            return new BCForeignerSmallListAttributeAdapter(bcForeignerSmallListAttribute, stringLocalizer);
        }
        if (attribute is BCDurationOfEmploymentAttribute bcDurationOfEmploymentAttribute)
        {
            return new BCDurationOfEmploymentAttributeAdapter(bcDurationOfEmploymentAttribute, stringLocalizer);
        }
        if (attribute is BCEmployeeCountAttribute bcEmployeeCountAttribute)
        {
            return new BCEmployeeCountAttributeAdapter(bcEmployeeCountAttribute, stringLocalizer);
        }
        if (attribute is BCEmploymentDurationMonthAttribute bcEmploymentDurationMonthAttribute)
        {
            return new BCEmploymentDurationMonthAttributeAdapter(bcEmploymentDurationMonthAttribute, stringLocalizer);
        }
        return baseProvider.GetAttributeAdapter(attribute, stringLocalizer);
    }
}
// </snippet_Class>
