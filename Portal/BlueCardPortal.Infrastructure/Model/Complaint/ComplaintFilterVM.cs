using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Infrastructure.Model.Complaint
{
    public class ComplaintFilterVM
    {
        /// <summary>
        /// Номер на жалба
        /// </summary>
        [Display(Name = "ComplaintNumber")]
        public string? ComplaintNumber { get; set; }

        /// <summary>
        /// От дата
        /// </summary>
        [Display(Name = "ComplaintFilterFromDate")]
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// До дата
        /// </summary>
        [Display(Name = "ComplaintFilterToDate")]
        public DateTime? ToDate { get; set; }
    }
}
