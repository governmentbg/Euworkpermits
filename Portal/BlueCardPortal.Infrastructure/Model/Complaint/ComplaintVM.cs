using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlueCardPortal.Infrastructure.Model.Application;

namespace BlueCardPortal.Infrastructure.Model.Complaint
{
    public class ComplaintVM
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ApplicationNumber { get; set; } = default!;
        public ApplicantVM Applicant { get; set; } = new();
        /// <summary>
        /// Обоснование
        /// </summary>
        [Display(Name = "ComplaintInfo")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        public string ComplaintInfo { get; set; } = default!;
    }
}
