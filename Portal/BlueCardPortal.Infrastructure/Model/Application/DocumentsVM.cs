using BlueCardPortal.Infrastructure.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Infrastructure.Model.Application
{
    /// <summary>
    /// Документи
    /// </summary>
    public class DocumentsVM
    {
        /// <summary>
        /// Вид разрешително
        /// </summary>
        public string? PermitType { get; set; }

        /// <summary>
        /// Вид заявител
        /// </summary>
        public string? ApplicantType { get; set; }

        /// <summary>
        /// Документи
        /// </summary>
        public List<DocumentVM> Documents { get; set; } = new();

        [BCDocuments(ErrorMessage = "BCDocumentsErrorMessage")]
        public string Validation { get; set; } = "DocumentsValidation";
    }
}
