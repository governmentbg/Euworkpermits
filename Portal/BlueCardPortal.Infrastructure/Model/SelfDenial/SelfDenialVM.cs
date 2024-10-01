using BlueCardPortal.Infrastructure.Model.Application;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Infrastructure.Model.SelfDenial
{
    public class SelfDenialVM
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ApplicationNumber { get; set; } = default!;
        public ApplicantVM Applicant { get; set; } = new();
        /// <summary>
        /// Причина за отказ
        /// </summary>
        [Display(Name = "RejectionInfo")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        public string RejectionInfo { get; set; } = default!;
    }
}
