using BlueCardPortal.Infrastructure.Integrations.BlueCardCore.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Infrastructure.Model.Application
{
    /// <summary>
    /// Тип заявление
    /// </summary>
    public class ApplicationTypeVM
    {
        /// <summary>
        /// Вид на заявление
        /// </summary>
        [Display(Name = "ApplicationType")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        public string ApplicationTypeCode { get; set; } = default!;
 
        /// <summary>
        /// Разрешение
        /// </summary>
        [Display(Name = "PermitType")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        public string PermitType { get; set; } = default!;
    }
}
