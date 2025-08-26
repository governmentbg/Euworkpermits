using BlueCardPortal.Infrastructure.Constants;
using BlueCardPortal.Infrastructure.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Infrastructure.Model.Application
{
    public class EmploymentVM
    {

        /// <summary>
        /// Продължителност на заетостта от
        /// <br/>Полето се използва за :Заявление за разрешение за краткосрочна заетост на хартиен носител (Приложимо за Заявление №5)
        /// </summary>
        [Display(Name = "DurationOfEmploymentFrom")]
        // [Required(ErrorMessage = "RequiredErrorMessage")]
        public DateTime? DurationOfEmploymentFrom { get; set; } //= default!;

        /// <summary>
        /// Продължителност на заетостта до
        /// Полето се използва за :Заявление за разрешение за краткосрочна заетост на хартиен носител (Приложимо за Заявление №5)
        /// </summary>
        /// 
        /// <summary>
        /// Продължителност (месеци)
        /// </summary>
        [Display(Name = "DurationOfEmploymentMonth")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        [BCEmploymentDurationMonth(ErrorMessage = "BCEmploymentDurationMonthMessage")]
        public int DurationOfEmploymentMonth { get; set; }

        /// <summary>
        /// Продължаване на договор
        /// </summary>
        [Display(Name = "EmploymentType")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        public string Type { get; set; } = default!;

        /// <summary>
        /// Смяна на работодател
        /// </summary>
        [Display(Name = "EmployerChange")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        public string EmployerChange { get; set; } = default!;

        /// <summary>
        /// Длъжност при заявление 5 - временна заетост
        /// </summary>
        [Display(Name = "EmploymentPosition")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        [RegularExpression(@"^[А-Яа-я](?:[А-Яа-я -]*[А-Яа-я])$", ErrorMessage = "PositionCyrilicRegularExpression")]
        public string Position { get; set; } = default!;

        /// <summary>
        /// Квалификация
        /// </summary>
        [Display(Name = "QualificationVM")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        [RegularExpression(@"^[А-Яа-я](?:[А-Яа-я -]*[А-Яа-я])$", ErrorMessage = "PositionCyrilicRegularExpression")]
        public string Qualification { get; set; } = default!;

        /// <summary>
        /// Цел на наемане
        /// </summary>
        [Display(Name = "EmploymentReason")]
        // [Required(ErrorMessage = "RequiredErrorMessage")]
        public string? EmploymentReason { get; set; } //= default!;

        /// <summary>
        /// Степен на образование
        /// </summary>
        [Display(Name = "EducationTypeVM")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        public string EducationType { get; set; } = default!;

        /// <summary>
        /// Специалност
        /// </summary>
        [Display(Name = "SpecialityVM")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        [RegularExpression(@"^[А-Яа-я](?:[А-Яа-я -]*[А-Яа-я])$", ErrorMessage = "PositionCyrilicRegularExpression")]
        public string Speciality { get; set; } = default!;

        /// <summary>
        /// Професия по НКПД
        /// </summary>
        [Display(Name = "SpecialityCodeVM")]
        [Required(ErrorMessage = "RequiredErrorMessage")] 
        public string SpecialityCode { get; set; } = default!;

        /// <summary>
        /// Адрес на месторабота
        /// Съвпада с адреса на седалището на работодателя
        /// </summary>
        [Display(Name = "AddressIsSame")]
        [Required(ErrorMessage = "RequiredErrorMessage")]

        public string AddressIsSame { get; set; } = default!;

        /// <summary>
        /// Обект на месторабота
        /// </summary>
        [Display(Name = "AddressObject")]
        public string? AddressObject { get; set; } 

        /// <summary>
        /// За валидация
        /// </summary>
        public string? EmploymentPermitType { get; set; }

        /// <summary>
        /// Адрес на месторабота
        /// </summary>
        public AddressVM Address { get; set; } = new AddressVM
        {
            Kind = ADDRESSE_TYPE.Head,
            IsCompanyAddress = true,
        };

    }
}
