using BlueCardPortal.Infrastructure.Constants;
using BlueCardPortal.Infrastructure.Integrations.BlueCardCore.Contracts;
using BlueCardPortal.Infrastructure.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Infrastructure.Model.Application
{
    /// <summary>
    /// Работодател
    /// </summary>
    public class EmployerVM
    {
        /// <summary>
        /// ЕИК
        /// </summary>
        [Display(Name = "EmployerIdentifier")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        [BCIdentifier(UicType = UicTypes.EIK, ErrorMessage = "BCIdentifierErrorMessageEIK")]
        public string Identifier { get; set; } = default!;

        /// <summary>
        /// Наименование на юридическото лице
        /// </summary>
        [Display(Name = "EmployerName")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        public string Name { get; set; } = default!;

        /// <summary>
        /// Брой служители
        /// </summary>
        [Display(Name = "EmployeeCount")]
        [Range(1, 99999, ErrorMessage = "EmployeeRangeErrorMessage")]
        public int EmployeeCount { get; set; } = default!;

        /// <summary>
        /// Брой чуждестранни служители
        /// </summary>
        [Display(Name = "ForeignEmployeeCount")]
        [Range(1, 99999, ErrorMessage = "EmployeeRangeErrorMessage")]
        [BCEmployeeCount(ErrorMessage = "EmployeeCountErrorMessage")]
        public int ForeignEmployeeCount { get; set; } = default!;

        /// <summary>
        /// Правна форма
        /// </summary>
        [Display(Name = "LegalForm")]
        [BCEmployeeCount(ErrorMessage = "EmployeeCountErrorMessage")]
        public string LegalForm { get; set; } = default!;

        /// <summary>
        /// Седалище/Адрес на управление
        /// </summary>
        public AddressVM Address { get; set; } = new AddressVM { Kind = ADDRESSE_TYPE.Head };

        /// <summary>
        /// Адрес на месторабота
        /// Съвпада с адреса на седалището на работодателя
        /// </summary>
        [Display(Name = "ContactAddressIsSame")]
        [Required(ErrorMessage = "RequiredErrorMessage")]

        public string ContactAddressIsSame { get; set; } = default!;

        /// <summary>
        /// Адрес за контакт
        /// </summary>
        public AddressVM ContactAddress { get; set; } = new AddressVM { Kind = ADDRESSE_TYPE.Correspondence };

        /// <summary>
        /// Данни за контакт с работодателя
        /// </summary>
        public ContactListVM Contacts { get; set; } = new();

        public AddressListVM GetAddresses()
        {
            var result = new AddressListVM();
            result.Items.Add(Address);
            if (ContactAddressIsSame == YESNO_TYPE.No)
            {
                result.Items.Add(ContactAddress);
            }
            else
            {
                var contactAddress = new AddressVM
                {
                    Kind = ADDRESSE_TYPE.Correspondence,
                    Region = Address.Region,
                    City = Address.City,    
                    PostalCode = Address.PostalCode,
                    Street = Address.Street,
                    Quarter = Address.Quarter,
                    StreetNo = Address.StreetNo,
                    BuildingNo = Address.BuildingNo,
                    Apartment = Address.Apartment,
                    Entrance = Address.Entrance,
                    Floor = Address.Floor,  
                };
                result.Items.Add(contactAddress);
            }
            return result;
        }
    }
}
