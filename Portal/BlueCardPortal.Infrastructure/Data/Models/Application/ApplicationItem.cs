﻿using Microsoft.EntityFrameworkCore;
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
    [Comment("Данни раздел от заявление")]
    public class ApplicationItem
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
        /// Тип
        /// </summary>
        [Comment("Тип")]
        public int ItemTypeId { get; set; }

        /// <summary>
        /// Данни за раздел сериализирани в json
        /// </summary>
        [Comment("Данни за раздел сериализирани в json")]
        [Column("data_content", TypeName = "jsonb")]
        public string? DataContent { get; set; }

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

        /// <summary>
        /// Тип елемент
        /// </summary>
        [Comment("Тип елемент")]
        [ForeignKey(nameof(ItemTypeId))]
        public virtual ApplicationItemType ItemType { get; set; } = default!;
    }
}
