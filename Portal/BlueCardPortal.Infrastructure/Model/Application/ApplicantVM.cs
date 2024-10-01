using BlueCardPortal.Infrastructure.Constants;
using BlueCardPortal.Infrastructure.Integrations.BlueCardCore.Contracts;
using BlueCardPortal.Infrastructure.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Infrastructure.Model.Application
{
    /// <summary>
    /// Заявител
    /// </summary>
    public class ApplicantVM
    {
        /// <summary>
        /// Тип заявител
        /// </summary>
        [Display(Name = "ApplicantType")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        public string ApplicantType { get; set; } = default!;

        /// <summary>
        /// ЕГН
        /// </summary>
        [Display(Name = "ApplicantEgn")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        [BCIdentifier(UicType = UicTypes.EGN, ErrorMessage = "BCIdentifierErrorMessageEGN")]
        public string Egn { get; set; } = default!;

        /// <summary>
        /// ЛНЧ
        /// </summary>
        [Display(Name = "ForeignerFid")]
        public string Lnch { get; set; } = default!;

        /// <summary>
        /// Данни за лице
        /// </summary>
        public ApplicantPersonVM Person { get; set; } = new();

        /// <summary>
        /// Данни за заявител чужденец
        /// </summary>
        public ForeignerVM Foreigner { get; set; } = new ForeignerVM{IsApplicant = true, TypeIdentifier = FOREIGNER_TYPE_IDENTIFIER.InBg };
    }
}
