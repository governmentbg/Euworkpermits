using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Infrastructure.Model.Application
{
    /// <summary>
    /// Резултат от записа в core системата
    /// </summary>
    public class DocumentResultVM
    {
        /// <summary>
        /// Резултат
        /// </summary>
        public bool IsOk { get; set; }

        /// <summary>
        /// Грешка
        /// </summary>
        public string? Error { get; set; }

        /// <summary>
        /// Идентификатор Cmis
        /// </summary>
        public string? CmisId { get; set; }

        /// <summary>
        /// Идентификатор 
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Идентификатор 
        /// </summary>
        public Guid PortalId { get; set; }
        /// <summary>
        /// Url за преглед
        /// </summary>
        public string? FileUrl { get; set; }

        /// <summary>
        /// Име на файл
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// Тип
        /// </summary>
        public string? MimeType { get; set; }

        /// <summary>
        /// Съдържание на файла
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// Множество файлове за този тип
        /// </summary>
        public bool HasMultipleFile { get; set; }

        /// <summary>
        /// Title на файла
        /// </summary>
        public bool HasTitle { get; set; }

        // <summary>
        /// Title 
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Тип на документа
        /// </summary>
        public string DocumentTypeCode { get; set; } = default!;

        public string ? View { get; set; }
    }
}
