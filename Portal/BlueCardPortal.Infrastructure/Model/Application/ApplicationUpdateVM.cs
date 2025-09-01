using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlueCardPortal.Infrastructure.Model.Application;

namespace BlueCardPortal.Infrastructure.Model.Complaint
{
    public class ApplicationUpdateVM
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ApplicationNumber { get; set; } = default!;
        public ApplicantVM Applicant { get; set; } = new();
        public DocumentsVM Documents { get; set; } = new();

        public Guid ApplicationId { get { return Id; } }

    }
}
