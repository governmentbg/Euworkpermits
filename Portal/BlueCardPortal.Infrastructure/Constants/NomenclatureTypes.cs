using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Infrastructure.Constants
{
    public static class NomenclatureTypes
    {
        public const string APPLICATION_TYPE = nameof(APPLICATION_TYPE);
        public const string PERMIT_TYPE = nameof(PERMIT_TYPE);
        public const string MARITAL_STATUS = nameof(MARITAL_STATUS);
        public const string GENDER = nameof(GENDER);
        public const string EDUCATION = nameof(EDUCATION);
        public const string RESOLUTION_DECISION_TYPE = nameof(RESOLUTION_DECISION_TYPE);
        public const string ENTITY_TYPE = nameof(ENTITY_TYPE);
        public const string FOREIGNER_TYPE_IDENTIFIER = nameof(FOREIGNER_TYPE_IDENTIFIER);
        public const string IDENTIFIER = nameof(IDENTIFIER);
        public const string CONTACT_INFO_TYPE = nameof(CONTACT_INFO_TYPE);
        public const string ADDRESSE_TYPE = nameof(ADDRESSE_TYPE);
        public const string RESOLUTION_TYPE = nameof(RESOLUTION_TYPE);
        public const string LEGAL_FORM_TYPE = nameof(LEGAL_FORM_TYPE);
        public const string REGION = nameof(REGION);
        public const string COUNTRIES = nameof(COUNTRIES);
        public const string NKPD_CODE = nameof(NKPD_CODE);
        public const string VISA_TYPE = nameof(VISA_TYPE);
        public const string TYPE_IDENTIFICATION_DOCUMENT = nameof(TYPE_IDENTIFICATION_DOCUMENT);
        public const string EXTERNAL_STATUS = nameof(EXTERNAL_STATUS);
        public const string INTERNAL_STATUS = nameof(INTERNAL_STATUS);
        public const string BIRTH_DATE_TYPE_INPUT = nameof(BIRTH_DATE_TYPE_INPUT);

    }
    public static class APPLICATION_TYPE
    {
        public const string Permanent = "1";
        public const string Temporary = "2";
    }
    public static class PERMIT_TYPE
    {
        public const string Temporary = "5";
    }
    public static class ENTITY_TYPE
    {
        /// <summary>
        /// Упълномощено лице
        /// </summary>
        public const string AuthorizedPerson = "1";
        /// <summary>
        /// Представител
        /// </summary>
        public const string Representative = "2";

        /// <summary>
        /// Чужденец
        /// </summary>
        public const string Foreigner = "3";
    }
    public static class FOREIGNER_TYPE_IDENTIFIER
    {
        /// <summary>
        /// лицето е в страната
        /// </summary>
        public const string InBg = "1";
        /// <summary>
        /// лицето не е в страната
        /// </summary>
        public const string External = "2";

    }
    public static class ADDRESSE_TYPE
    {
        /// <summary>
        /// ПОСТОЯНЕН АДРЕС
        /// </summary>
        public const string Permanent = "1";

        /// <summary>
        /// НАСТОЯЩ АДРЕС
        /// </summary>
        public const string Current = "2";

        /// <summary>
        /// АДРЕС ЗА КОРЕСПОДЕНЦИЯ
        /// </summary>
        public const string Correspondence = "3";

        /// <summary>
        /// СЕДАЛИЩЕ / АДРЕС НА УПРАВЛЕНИЕ
        /// </summary>
        public const string Head = "4";

    }
    public static class YESNO_TYPE
    {
        public const string Yes = "YES";
        public const string No = "NO";
    }
    public static class ApplicationStatus
    {
        public const int None = 0;
        public const int Draft = 1;
        public const int Send = 10;
    }
    public static class ApplicationRole
    {
        public const string Person = "1";
        public const string Representative = "2";
    }
    public static class IDENTIFIER
    {
        public const string Egn = "1";
        public const string Lnch = "2";
        public const string Eik = "3";
    }
    public static class COUNTRIES
    {
        public const string Bulgaria = "BGR";
    }
    public static class BIRTH_DATE_TYPE_INPUT
    {
        public const string YYYYMMDD = "1";
    }

    public static class UicTypes
    {
        public const int EGN = 1;
        public const int LNCH = 2;
        public const int EIK = 3;
    }

    public class EXTERNAL_STATUS
    {
        /// <summary>
        /// 5 Очаква допълнителни документи от заявител
        /// </summary>
        public const string WaitDocuments = "5";
        public const string Denial = "6";
        public const string Termination = "7";
        public const string Withdrawal = "8";
        public static string[] ForComplaint = new []{ Denial, Termination, Withdrawal};
    }
    public class INTERNAL_STATUS
    {
        public const string DenialIssued = "9";
        public const string TerminationIssued = "10";
        public const string WithdrawalIssued = "11";
        public const string AwaitingVisa = "12";
        public const string IssuanceVisa = "13";
        public const string PermitIssued = "14";
        // "Самоотказ",
        public const string SelfDenial = "50";
        public static string[] NoSelfDenial = new[] {
            DenialIssued, 
            TerminationIssued,
            WithdrawalIssued, 
            AwaitingVisa,
            IssuanceVisa,
            PermitIssued
        };
    }

    public static class DocumentType
    {
        public const string Other = "99";
    }
    //6 Отказ
    //7 Прекратено производство
    //8 Отнемане

    //'Прекратяване' , 'Отнемане', 'Отказана жалба'.


    /*    
        1 Регистрирано 
        2 В обработка от ДМ
        3 В обработка от АЗ
        4 В обработка от АЗ/ДАНС
        5 Очаква допълнителни документи от заявител
        6 Отказ
        7 Прекратено производство
        8 Отнемане 
        9 Кандидатстване за виза
        10 Предоставяне на виза и заплащане
        11 Издадено разрешение
        12 Информиран жалбоподател за издадено становище

    1 Регистрирано
    2 В обработка от ДМ
    3 В обработка от АЗ
    4 В обработка от АЗ/ДАНС
    5 Издадено становище АЗ
    6 Издадено становище АЗ/ДАНС
    7 Върнат с указания от  ДМ
    8 Върнат с указания от  АЗ
    9 Издаден отказ
    10 Издадено прекратяване
    11 Издадено отнемане
    12 Очаква предоставяне на виза на гише
    13 Предоставяне на виза в ДМ
    14 Издадено разрешение
    15 Информиран жалбоподател */

}
