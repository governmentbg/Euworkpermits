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
    [Comment("Подписани данни от заявление изпратени към коре системата")]
    public class ApplicationMessage
    {
        // <summary>
        /// Идентификатор
        /// </summary>
        [Key]
        [Comment("Идентификатор")]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Идентификатор на заявление
        /// </summary>
        [Comment("Идентификатор на заявление")]
        public Guid ApplicationId { get; set; }

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

        /// <summary>
        /// Заявление
        /// </summary>
        [Comment("Заявление")]
        [ForeignKey(nameof(ApplicationId))]
        public virtual Application? Application { get; set; }

    }
}
