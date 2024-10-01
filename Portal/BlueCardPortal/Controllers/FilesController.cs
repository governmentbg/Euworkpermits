using BlueCardPortal.Core.Contracts;
using BlueCardPortal.Core.Helper;
using BlueCardPortal.Core.Services;
using BlueCardPortal.Extensions;
using BlueCardPortal.Infrastructure.Constants;
using BlueCardPortal.Infrastructure.Model.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace BlueCardPortal.Controllers
{
    public class FilesController : BaseController
    {
        private readonly IApplicationService applicationService;
        private readonly IStringLocalizer localizer;
        public FilesController(
            IApplicationService _applicationService,
            IStringLocalizer _localizer)
        {
            applicationService = _applicationService;
            localizer = _localizer;
        }

        [HttpPost]
        public PartialViewResult UploadFileView([FromBody] DocumentVM model)
        {
            return PartialView("_FileUpload", model);
        }
        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadFile(ICollection<IFormFile> files, DocumentVM model)
        {
            var result = new DocumentResultVM
            {
                Error = localizer["ErrorNoFIle"]
            };
            if (files != null && files.Count() > 0)
            {
                var file = files.FirstOrDefault();
               
                if (model.MaxFileSize > 0)
                {
                    long maxSize = (long)model.MaxFileSize * 1024 * 1024;
                    if (files.Any(x => x.Length > maxSize))
                    {
                        result.Error = localizer["ErrorFileMaxSIze"];
                        return Json(result);
                    }
                }
                var fileExt = Path.GetExtension(file.FileName).Replace(".", "").ToLower();
                string[] acceptFileExts = { "doc", "docx", "rtf", "pdf", "html" };
                if (!acceptFileExts.Contains(fileExt))
                {
                    result.Error = "file_ext";
                    return Json(result);
                }
                model.MimeType = FileHelper.GetMimeType(fileExt);

                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    model.FileContent = Convert.ToBase64String(ms.ToArray());
                }
                model.FileName = file.FileName;
                result = await applicationService.UploadFile(model);
                if (result.IsOk)
                {
                    model.CmisId = result.CmisId!;
                    ViewData.TemplateInfo.HtmlFieldPrefix = $"Documents.Documents[{model.Index}]";
                    result.View = await this.RenderViewAsync("_Document", model, true);
                    if (model.HasMultipleFile)
                    {
                        ViewData.TemplateInfo.HtmlFieldPrefix = $"Documents.Documents[{model.IndexAll}]";
                        var blank = new DocumentVM
                        {
                            Index = model.IndexAll,
                            HasMultipleFile = true,
                            ApplicationId = model.ApplicationId,
                            DocumentCategoryCode = model.DocumentCategoryCode,
                            DocumentCategory = model.DocumentCategory,
                            DocumentType = model.DocumentType,
                            DocumentTypeCode = model.DocumentTypeCode,
                            HasTitle=model.HasTitle,
                            IsMandatory=model.IsMandatory,
                            MaxFileSize=model.MaxFileSize,
                            
                        };
                        result.View += await this.RenderViewAsync("_Document", blank, true);
                    }
                }
            }
            return Json(result);
        }
    }
}
