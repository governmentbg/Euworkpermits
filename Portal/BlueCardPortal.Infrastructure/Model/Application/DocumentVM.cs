using BlueCardPortal.Infrastructure.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Infrastructure.Model.Application
{
    /// <summary>
    /// Прикачен документ
    /// </summary>
    public class DocumentVM
    {
        /// <summary>
        /// Индекс
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Максимален индекс
        /// </summary>
        public int IndexAll { get; set; }

        /// <summary>
        /// Ид на заявление 
        /// </summary>
        public Guid ApplicationId { get; set; }

        /// <summary>
        /// Идентификатор
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Идентификатор Cmis
        /// </summary>
        public string CmisId { get; set; } = default!;

        /// <summary>
        /// Задължителен ли е документа
        /// </summary>
        public bool IsMandatory { get; set; }

        /// <summary>
        /// Тип на документа
        /// </summary>
        public string DocumentTypeCode { get; set; } = default!;

        /// <summary>
        /// Тип на документа
        /// </summary>
        [Display(Name = "DocumentType")]
        public string DocumentType { get; set; } = default!;

        /// <summary>
        /// Тип на документа за грид
        /// </summary>
        [Display(Name = "DocumentType")]
        public string? DocumentTypeWithTitle
        {
            get
            {
                return DocumentType + (DocumentTypeCode == Constants.DocumentType.Other && !string.IsNullOrEmpty(Title) ? $": {Title}" : string.Empty);
            }
        }

        /// <summary>
        /// Категория на документа
        /// </summary>
        public string? DocumentCategoryCode { get; set; }

        /// <summary>
        /// Категория на документа
        /// </summary>
        [Display(Name = "DocumentCategory")]
        public string? DocumentCategory { get; set; }

        /// <summary>
        /// Име на файл
        /// </summary>
        [Display(Name = "FileName")]
        public string FileName { get; set; } = default!;

        /// <summary>
        /// Оригинал
        /// </summary>
        [Display(Name = "IsOriginal")]
        public bool IsOriginal { get; set; }

        /// <summary>
        /// Максимален размер на файл
        /// </summary>
        public int MaxFileSize { get; set; }

        /// <summary>
        /// Допустими типове файлове
        /// </summary>
        public string? FileFormats { get; set; }

        /// <summary>
        /// Съдържание на файла
        /// </summary>
        public string? FileContent { get; set; }

        /// <summary>
        /// Mime type
        /// </summary>
        public string MimeType { get; set; } = default!;

        /// <summary>
        /// Идентификатор 
        /// </summary>
        public Guid PortalId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Url за преглед
        /// </summary>
        public string? FileUrl { get; set; }

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
        [Display(Name = "DocumentTitle")]
        public string? Title { get; set; }

        public string? FileHash { get; set; }
        public string? ForeignerLabel { get; set; }
        public Guid? ForeignerSmallId { get; set; }
        [Display(Name = "FileName")]
        public string FileNameLabel
        {
            get
            {
                return DocumentType != Title && !string.IsNullOrEmpty(Title) ? $"{Title}{Environment.NewLine}{FileName}" : FileName;
            }
        }
    }
}
