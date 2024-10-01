using BlueCardPortal.Infrastructure.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Infrastructure.Model.Application
{
    /// <summary>
    /// Адрес
    /// </summary>
    public class AddressVM
    {
        /// <summary>
        /// Номер поред
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Тип
        /// </summary>
        [Required(ErrorMessage = "RequiredErrorMessage")]
        [Display(Name = "AddressKind")]
        public string Kind { get; set; } = default!;
        /// <summary>
        /// Допустими типове 
        /// </summary>
        public List<SelectListItem>? KindDDL { get; set; }

        /// <summary>
        /// Област
        /// </summary>
        [Required(ErrorMessage = "RequiredErrorMessage")]
        [Display(Name = "Region")]
        public string Region { get; set; } = default!;

        /// <summary>
        /// Населено място
        /// </summary>
        [Required(ErrorMessage = "RequiredErrorMessage")]
        [Display(Name = "City")]
        public string City { get; set; } = default!;

        /// <summary>
        /// Пощенски код
        /// </summary>
        [Range(1000, 9999, ErrorMessage = "RangeErrorMessage")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        [Display(Name = "PostalCode")]
        public int PostalCode { get; set; }

        /// <summary>
        /// Улица
        /// </summary>
        [Display(Name = "Street")]
        [BCAddress(ErrorMessage = "BCAddressErrorMessage")]// "Задължително да бъде попълнена следната комбинация : Улица и Номер ИЛИ Жилищен квартал и Сграда номер")] // TODO: "BCAddressErrorMessage"
        public string? Street { get; set; } 

        /// <summary>
        /// Номер
        /// </summary>
        [Display(Name = "StreetNo")]
        public string? StreetNo { get; set; }

        /// <summary>
        /// Квартал / жк
        /// </summary>
        [Display(Name = "Quarter")]
        public string? Quarter { get; set; }

        /// <summary>
        /// Блок
        /// </summary>
        [Display(Name = "BuildingNo")]
        public string? BuildingNo { get; set; }

        /// <summary>
        /// Вход
        /// </summary>
        [Display(Name = "Entrance")]
        public string? Entrance { get; set; }

        /// <summary>
        /// Етаж
        /// </summary>
        [Display(Name = "Floor")]
        public string? Floor { get; set; }

        /// <summary>
        /// Апартамент
        /// </summary>
        [Display(Name = "Apartment")]
        public string? Apartment { get; set; }
    }
}
