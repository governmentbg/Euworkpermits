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
    /// Тип раздел от заявление
    /// </summary>
    [Comment("Тип раздел от заявление")]
    public class ApplicationItemType
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Key]
        [Comment("Идентификатор")]
        public int Id { get; set; }
        /// <summary>
        /// Наименование
        /// </summary>
        [Comment("Наименование")]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Наименование en
        /// </summary>
        [Comment("Наименование en")]
        public string NameEn { get; set; } = string.Empty;

        /// <summary>
        /// Тип на модела
        /// </summary>
        [Comment("Тип на модела")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Име на view
        /// </summary>
        [Comment("Име на view")]
        public string ViewName { get; set; } = string.Empty;
        /// <summary>
        /// HtmlPrefix
        /// </summary>
        [Comment("Html Prefix")]
        public string HtmlPrefix { get; set; } = string.Empty;

        /// <summary>
        /// Наименование при преглед
        /// </summary>
        [Comment("Наименование при преглед")]
        public string PreviewName { get; set; } = string.Empty;

        /// <summary>
        /// Наименование en при преглед
        /// </summary>
        [Comment("Наименование en при преглед")]
        public string PreviewNameEn { get; set; } = string.Empty;

        /// <summary>
        /// Име на view за преглед
        /// </summary>
        [Comment("Име на view за преглед")]
        public string PreviewNameView { get; set; } = string.Empty;

        /// <summary>
        /// Номер на стъпка
        /// </summary>
        [Comment("Номер на стъпка")]
        public int StepNum { get; set; }
    }
}
