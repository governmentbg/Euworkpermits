using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Infrastructure.Data.Models.Application
{
    /// <summary>
    /// Данни раздел от заявление
    /// </summary>
    [Comment("Подписани данни от саоотказ/жалба/промяна изпратени към коре системата")]
    public class UpdateMessage
    {
        // <summary>
        /// Идентификатор
        /// </summary>
        [Key]
        [Comment("Идентификатор")]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Тип на източник
        /// </summary>
        [Comment("Тип източник")]
        public int SourceTypeId { get; set; }


        /// <summary>
        /// Идентификатор на източник
        /// </summary>
        [Comment("Идентификатор на източник")]
        public Guid SourceId { get; set; }

        /// <summary>
        /// Данни за раздел сериализирани в json
        /// </summary>
        [Comment("Данни за заявление сериализирани в json")]
        [Column("registration_data", TypeName = "jsonb")]
        public string? RegistrationData { get; set; }

        /// <summary>
        /// Отговор
        /// </summary>
        [Comment("Отговор")]
        public string? ResponseStatus { get; set; }

        /// <summary>
        /// TimeStamp
        /// </summary>
        public byte[]? RegistrationTimeStamp { get; set; }

        /// <summary>
        /// Signature
        /// </summary>
        public string? RegistrationDataSignature { get; set; }

        /// <summary>
        /// Отговор
        /// </summary>
        [Comment("Отговор")]
        public string? ResponseMessage { get; set; }

        /// <summary>
        /// Дата на последна промяна
        /// </summary>
        [Comment("Дата на последна промяна")]
        public DateTime DateWrt { get; set; }

    }
}
