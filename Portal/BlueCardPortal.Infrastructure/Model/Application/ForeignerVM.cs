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
    public partial class ForeignerVM
    {
        /// <summary>
        /// Дали е заявител
        /// </summary>
        public bool IsApplicant { get; set; }

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
        /// Идентификатор на чужденец
        /// </summary>
        [Display(Name = "ForeignerIdentifier")]
        [Required(ErrorMessage = "RequiredErrorMessage")] 
        public string Identifier { get; set; } = default!;
        
        /// <summary>
        /// Вид идентификатор 
        /// Ако пребивава в страната ЛНЧ
        /// Ако не друг
        /// </summary>
        [Display(Name = "ForeignerTypeIdentifier")]
        [Required(ErrorMessage = "RequiredErrorMessage")] 
        public string TypeIdentifier { get; set; } = default!;

        /// <summary>
        /// Адрес на чужденеца в Република България
        /// Адрес в Република България
        /// </summary>
        //public AddressListVM Addresses { get; set; } =new ();

        /// <summary>
        /// Настоящ адрес
        /// </summary>
        public AddressVM Address { get; set; } = new AddressVM { Kind = ADDRESSE_TYPE.Current };

        /// <summary>
        /// Дата на раждане
        /// </summary>
        [Display(Name = "BirthDate")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        [BCRangeTodayDate(FromYear = -100, ToYear = -10, ErrorMessage = "BCRangeDateErrorMessage")]
        public DateTime BirthDate { get; set; } = default!;

        /// <summary>
        /// Формат дата на раждане
        /// </summary>
        [Display(Name = "BirthDateTypeInput")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        public string BirthDateTypeInput { get; set; } = default!;

        /// <summary>
        /// Дата на раждане
        /// </summary>
        [Display(Name = "BirthMonth")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        [BCRangeTodayDate(FromYear = -100, ToYear = -10, ErrorMessage = "BCRangeDateErrorMessage")]
        public DateTime BirthMonth { get; set; } 

        /// <summary>
        /// Гражданство
        /// </summary>
        [Display(Name = "Nationality")]
        [Required(ErrorMessage = "RequiredErrorMessage")] 
        public string Nationality { get; set; } = default!;
        
        /// <summary>
        /// Семеен стаус
        /// </summary>
        [Display(Name = "MaritalStatus")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        public string MaritalStatus { get; set; } = default!;
        
        /// <summary>
        /// Пол
        /// </summary>
        [Display(Name = "Gender")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        public string Gender { get; set; } = default!;

   
        /// <summary>
        /// Място на раждане
        /// </summary>
        [Display(Name = "CityОfBirth")]
        [Required(ErrorMessage = "RequiredErrorMessage")] 
        [RegularExpression(@"^[А-Яа-я](?:[А-Яа-я' -]*[А-Яа-я])$", ErrorMessage = "CityOfBirthRegularExpression")]
        public string CityОfBirth { get; set; } = default!;
        
        /// <summary>
        /// Виза
        /// </summary>
        [Display(Name = "VisaType")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        public string VisaType { get; set; } = default!;

        /// <summary>
        /// Номер на виза
        /// </summary>
        [Display(Name = "VisaSerialNumber")]
        [Required(ErrorMessage = "RequiredErrorMessage")] 
        public string VisaSerialNumber { get; set; } = default!;

        /// <summary>
        /// Дата на валидност на визата
        /// </summary>
        [Display(Name = "VisaExpirationDate")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        public DateTime? VisaExpirationDate { get; set; } 

        /// <summary>
        /// Дата на влизане в страната
        /// </summary>
        [Display(Name = "EntryDate")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        [BCRangeTodayDate(FromYear = -100, ToYear = 100, ErrorMessage = "BCRangeDateErrorMessage")]
        public DateTime? EntryDate { get; set; }

        /// <summary>
        /// Входна точка на влизане в страната
        /// </summary>
        [Display(Name = "EntryPoint")]
        [Required(ErrorMessage = "RequiredErrorMessage")]
        public string EntryPoint { get; set; } = default!;

        ///// <summary>
        ///// цел на влизането в страната
        ///// </summary>
        //[Display(Name = "PurposeOfEnteringCountry")]
        //[Required(ErrorMessage = "RequiredErrorMessage")]
        //public string PurposeOfEnteringCountry { get; set; } = default!;

        ///// <summary>
        ///// срок на пребиваване
        ///// </summary>
        //public int TermОfResidence { get; set; } = default!;

        ///// <summary>
        ///// валидност на разрешението за пребиваване от първа държава членка
        ///// </summary>
        //public DateTime ValidityResidencePermitFromFirstMemberState { get; set; } = default!;


        /// <summary>
        /// Данни за контакт със заявител
        /// </summary>
         public ContactListVM Contacts { get; set; } = new();

        /// <summary>
        /// Документи за идентификация
        /// </summary>
        public PersonIdDocumentListVM PersonIdDocuments { get; set; } = new ();
        public AddressListVM GetAddresses()
        {
            return new AddressListVM
            {
                Items = new List<AddressVM>
                {
                    Address
                }
            };
        }
    }
}
