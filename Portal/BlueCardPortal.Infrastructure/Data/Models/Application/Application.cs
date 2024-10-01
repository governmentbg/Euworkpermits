using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Infrastructure.Data.Models.Application
{
    /// <summary>
    /// Заявления
    /// </summary>
    [Comment("Заявления")]
    public class Application
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
        public string? ApplyNumber { get; set; }

        /// <summary>
        /// Раздели
        /// </summary>
        public virtual ICollection<ApplicationItem> ApplicationItems { get; set; } = new List<ApplicationItem>();
    }
}
