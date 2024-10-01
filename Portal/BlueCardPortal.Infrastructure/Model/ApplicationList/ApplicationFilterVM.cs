using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BlueCardPortal.Infrastructure.Model.ApplicationList
{
    

    /// <summary>
    /// Филтър заявления
    /// </summary>
    public class ApplicationFilterVM
    {
        /// <summary>
        /// Номер на заявление
        /// </summary>
        [Display(Name = "ApplicationNumber")]
        public string? ApplicationNumber { get; set; }

        /// <summary>
        /// Име чужденец
        /// </summary>
        [Display(Name = "ForeignerNameFilter")]
        public string? ForeignerName { get; set; }
        /// <summary>
        /// Статус
        /// </summary>
        [Display(Name = "Status")]
        public string? Status { get; set; }
        /// <summary>
        /// Вид разрешително
        /// </summary>
        [Display(Name = "PermitTypeFilter")]
        public string? PermitType { get; set; }

        /// <summary>
        /// Национална идентичност
        /// </summary>
        [Display(Name = "CountryFilter")]
        public string? Country { get; set; }
        /// <summary>
        /// ЛНЧ
        /// </summary>
        [Display(Name = "LNCHFilter")]
        public string? LNCH { get; set; }


        /// <summary>
        /// От дата
        /// </summary>
        [Display(Name = "ApplicationFilterFromDate")]
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// Дата на раждане
        /// </summary>
        [Display(Name = "BirthDateFilter")]
        public DateTime? BirthDate { get; set; }
        
        /// <summary>
        /// До дата
        /// </summary>
        [Display(Name = "ApplicationFilterToDate")]
        public DateTime? ToDate { get; set; }
    }
}
