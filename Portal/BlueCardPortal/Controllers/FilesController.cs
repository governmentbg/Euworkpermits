using BlueCardPortal.Core.Contracts;
using BlueCardPortal.Core.Helper;
using BlueCardPortal.Core.Services;
using BlueCardPortal.Extensions;
using BlueCardPortal.Infrastructure.Constants;
using BlueCardPortal.Infrastructure.Model.Application;
using IO.SignTools.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace BlueCardPortal.Controllers
{
    public class FilesController : BaseController
    {
        private readonly IApplicationService applicationService;
        private readonly IStringLocalizer localizer;
        private readonly IIOSignToolsService signToolsService;
        private readonly INomenclatureService nomenclatureService;
        public FilesController(
            IApplicationService _applicationService,
            IStringLocalizer _localizer,
            IIOSignToolsService _signToolsService,
            INomenclatureService _nomenclatureService)
        {
            applicationService = _applicationService;
            localizer = _localizer;
            signToolsService = _signToolsService;
            nomenclatureService = _nomenclatureService;
        }

        [HttpPost]
        public async Task<PartialViewResult> UploadFileView([FromBody] DocumentVM model)
        {
            model.MaxFileSize = await nomenclatureService.UploadFileSize();
            model.FileFormats = await nomenclatureService.UploadFileFormats();
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
                model.MaxFileSize = await nomenclatureService.UploadFileSize();
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
                var fileFormats = await nomenclatureService.UploadFileFormats();
                if (!$"{fileFormats},".Contains($"{fileExt},"))
                {
                    /// message = $"Недопустим файлов формат {fileExt}, разрешено е качване на файлов във формат {fileFormats} "
                    result.Error = localizer["ErrorFileExt"];
                    return Json(result);
                }
                model.MimeType = FileHelper.GetMimeType(fileExt);
                var fileHash = string.Empty;
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    model.FileContent = Convert.ToBase64String(ms.ToArray());
                    fileHash = signToolsService.GetHash(ms.ToArray(), HashAlgorithmName.SHA256);
                }
                model.FileName = file.FileName;
                result = await applicationService.UploadFile(model);

                if (result.IsOk)
                {
                    result.FileHash = fileHash;
                    model.CmisId = result.CmisId!;
                    model.FileHash = fileHash;
                    await applicationService.SaveDocument(model.ApplicationId, model);
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

        public async Task<IActionResult> RemoveFile(Guid applicationId, Guid portalId, bool remove)
        {
            await applicationService.RemoveDocument(applicationId, portalId, remove);
            return Json(new { state = "OK" });
        }
    }
}
