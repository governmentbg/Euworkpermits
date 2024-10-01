using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Infrastructure.Model.ApplicationList
{
    public class ApplicationListItemVM
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public string? ApplicationId { get; set; }
        /// <summary>
        /// Номер заявление
        /// </summary>
        public string ApplicationNumber { get; set; } = default!;
        /// <summary>
        /// Име чужденец
        /// </summary>
        public string ForeignerName { get; set; } = default!;
        /// <summary>
        /// ЛНЧ чужденец
        /// </summary>
        public string? ForeignerLNCH { get; set; }
        /// <summary>
        /// Дата на раждане чужденец
        /// </summary>
        public string? ForeignerBirthDate { get; set; }
        /// <summary>
        /// Националност на чужденец
        /// </summary>
        public string? ForeignerNationality { get; set; }
        public string? ForeignerNationalityCode { get; set; }
        /// <summary>
        /// Статус
        /// </summary>
        public string Status { get; set; } = default!;
        public string StatusCode { get; set; } = default!;
        /// <summary>
        /// Вид разрешително
        /// </summary>
        public string PermitType { get; set; } = default!;
        public string PermitTypeCode { get; set; } = default!;
        /// <summary>
        /// Дата на влизане в страната
        /// </summary>
        public DateTime? EntryDate { get; set; }
        /// <summary>
        /// Допустимо ли е подаване на жалба в този статус
        /// </summary>
        public bool ForComplaint { get; set; }
        /// <summary>
        /// Допустимо ли е само отказ
        /// </summary>
        public bool ForSelfDenial { get; set; }
        /// <summary>
        /// Заявлението е за редакция
        /// </summary>
        public bool ForUpdate { get; set; }
    }
}
