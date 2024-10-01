using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Infrastructure.Data.Models.Application
{
    /// <summary>
    /// Жалби
    /// </summary>
    [Comment("Жалби")]
    public class ApplicationUpdate
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Key]
        [Comment("Идентификатор")]
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор на потребител
        /// </summary>
        [Comment("Идентификатор на потребител")]
        public Guid UserId { get; set; }
        /// <summary>
        /// Статус
        /// </summary>
        [Comment("Статус")]
        public int Status { get; set; }

        /// <summary>
        /// Грешка върната от core системата
        /// </summary>
        [Comment("Грешка върната от core системата")]
        public string? Error { get; set; }

        /// <summary>
        /// Дата на последна промяна
        /// </summary>
        [Comment("Дата на последна промяна")]
        public DateTime DateWrt { get; set; }

        /// <summary>
        /// Номер заявление от core системата
        /// </summary>
        [Comment("Номер заявление от core системата")]
        public string? ApplyNumberFrom { get; set; }

     
        /// <summary>
        /// Данни за  жалба сериализирани в json
        /// </summary>
        [Comment("Данни за  жалба сериализирани в json")]
        [Column("data_content", TypeName = "jsonb")]
        public string? DataContent { get; set; }
    }
}
