using BlueCardPortal.Infrastructure.Constants;
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
    /// Чужденец
    /// </summary>
    public partial class ForeignerSmallVM
    {
        /// <summary>
        /// Индекс
        /// </summary>
        public int Index { get; set; }

        public Guid Id  { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Име
        /// </summary>
        [Display(Name = "ForeignerName")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        [RegularExpression(@"^[a-zA-Z](?:[a-zA-Z' -]*[a-zA-Z])?$", ErrorMessage = "ForeignerNameRegularExpression")] // ^[A-Za-z]+(?:[\-\'][A-Za-z]+)$
        public string Name { get; set; } = default!;

        /// <summary>
        /// Име на кирилица
        /// </summary>
        [RegularExpression(@"^[А-Яа-я](?:[А-Яа-я' -]*[А-Яа-я])$", ErrorMessage = "ForeignerNameCyrilicRegularExpression")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        [Display(Name = "ForeignerNameCyrilic")]
        public string NameCyrilic { get; set; } = default!;

        /// <summary>
        /// Дата на раждане
        /// </summary>
        [Display(Name = "BirthDate")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        [BCRangeTodayDate(FromYear = -100, ToYear = -10, ErrorMessage = "BCRangeDateErrorMessage")]
        public DateTime? BirthDate { get; set; } = default!;

        /// <summary>
        /// Номер на задгр. паспорт
        /// </summary>
        [Display(Name = "PassportNumber")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        public string PassportNumber { get; set; } = default!;

        /// <summary>
        /// Гражданство
        /// </summary>
        [Display(Name = "Nationality")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        public string NationalityCode { get; set; } = default!;

        /// <summary>
        /// Гражданство
        /// </summary>
        [Display(Name = "Nationality")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        public string Nationality { get; set; } = default!;

        [Display(Name = "SmallDurationOfEmploymentFrom")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        [BCDurationOfEmployment(PeriodDays = 90, ErrorMessage = "BCDurationOfEmploymentErrorMessage")]
        public DateTime? DurationOfEmploymentFrom { get; set; }

        [Display(Name = "SmallDurationOfEmploymentTo")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        [BCDurationOfEmployment(PeriodDays = 90, ErrorMessage = "BCDurationOfEmploymentErrorMessage")]
        public DateTime? DurationOfEmploymentTo { get; set; }

        /// <summary>
        /// Длъжност при заявление 5 - временна заетост
        /// </summary>
        [Display(Name = "SmallEmploymentPosition")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        public string Position { get; set; } = default!;

        [Display(Name = "SmallDurationOfEmploymentPeriod")]
        public string DurationOfEmploymentPeriod
        {
            get
            {
                return DurationOfEmploymentFrom?.ToString(FormattingConstant.NormalDateFormat) + " - " +
                       DurationOfEmploymentTo?.ToString(FormattingConstant.NormalDateFormat);
            }
        }

        public bool IsEdit { get; set; }
    }
}
