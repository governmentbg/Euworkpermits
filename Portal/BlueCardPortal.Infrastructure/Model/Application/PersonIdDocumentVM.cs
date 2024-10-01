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
    /// Документи за идентификация на чужденец
    /// </summary>
    public class PersonIdDocumentVM
    {
        /// <summary>
        /// Номер поред
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Тип на документ
        /// </summary>
        [Display(Name = "PersonIdDocumentType")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        public string Type { get; set; } = default!;
       
        /// <summary>
        /// Серия
        /// </summary>
        [Display(Name = "PersonIdDocumentSeries")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        public string Series { get; set; } = default!;

        /// <summary>
        /// Номер на документ
        /// </summary>
        [Display(Name = "PersonIdDocumentIdentifier")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        public string Identifier { get; set; } = default!;

        /// <summary>
        /// Дата на издаване
        /// </summary>
        [Display(Name = "IssueDate")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        public DateTime? IssueDate { get; set; } 

        /// <summary>
        /// Дата на валидност
        /// </summary>
        [Display(Name = "ExpirationDate")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// Издател
        /// </summary>
        [Display(Name = "IssuedBy")]
        [Required(ErrorMessage = "RequiredErrorMessage")] 
        public string IssuedBy { get; set; } = default!;

    }
}
