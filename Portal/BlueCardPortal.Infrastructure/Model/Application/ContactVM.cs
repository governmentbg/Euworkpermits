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
    /// Данни за контакт
    /// </summary>
    public  class ContactVM
    {
        /// <summary>
        /// Индекс
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Тип
        /// </summary>
        [Display(Name = "ContactType")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        public string Type { get; set; } = default!;

        /// <summary>
        /// Контакт
        /// </summary>
        [Display(Name = "ContactDescription")]
//        [Required(ErrorMessage = "RequiredErrorMessage")]
        [BCContactEmail(RegEx = "^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,16}$", ErrorMessage = "ErrorEmailMessage")]
        [BCContactPhone(RegEx= "^[\\+]?[(]?[0-9]{3}[)]?[-\\s\\.]?[0-9]{3}[-\\s\\.]?[0-9]{4,6}$", ErrorMessage = "ErrorPhoneMessage")]
        public string? Description { get; set; } 

        /// <summary>
        /// Предпочитан контакт
        /// </summary>
        [Display(Name = " ")]
        public string? IsPreferedContract { get; set; }
    }
}
