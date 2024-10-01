using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Infrastructure.Model.Application
{
    /// <summary>
    /// Данни за заявител български гражданин
    /// </summary>
    public class ApplicantPersonVM
    {
        /// <summary>
        /// Физическо лице
        /// Служител на юридическо лице
        /// </summary>
        [Display(Name = "ApplicantRole")]
        public string ApplicantRole { get; set; } = default!;

        /// <summary>
        /// Име на заявител
        /// </summary>
        [Display(Name = "ApplicantName")]
        public string Name { get; set; } = default!;

        /// <summary>
        /// Адрес за кореспонденция на заявител
        /// </summary>
        public AddressVM Address { get; set; } = new();

        /// <summary>
        /// Данни за контакт със заявител
        /// </summary>
        public ContactListVM Contacts { get; set; } = new();

        /// <summary>
        /// Физическо лице
        /// Служител на юридическо лице
        /// </summary>
        public EmployerVM Employer { get; set; } = new();

    }
}
