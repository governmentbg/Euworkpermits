using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Infrastructure.Model.Application
{
    /// <summary>
    /// Допълнителна информация
    /// </summary>
    public class ApplicationInfoVM
    {
        /// <summary>
        /// Допълнителна информация, свързана със заявлението
        /// </summary>; 
        [Display(Name = "ApplicationAdditionalInfo")]
        public string? AdditionalInfo { get; set; } 
    }
}
