using BlueCardPortal.Core.Contracts;
using BlueCardPortal.Core.Extensions;
using BlueCardPortal.Extensions;
using BlueCardPortal.Infrastructure.Constants;
using BlueCardPortal.Infrastructure.Data.Models.UserContext;
using BlueCardPortal.Infrastructure.Model;
using BlueCardPortal.Infrastructure.Model.Application;
using BlueCardPortal.Infrastructure.Model.ApplicationList;
using BlueCardPortal.Infrastructure.Model.Complaint;
using BlueCardPortal.Infrastructure.Model.SelfDenial;
using BlueCardPortal.Models;
using BlueCardPortal.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Security.AccessControl;
using System.Web;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BlueCardPortal.Controllers
{
    public class ApplicationController : BaseController
    {
        private readonly INomenclatureService nomenclatureService;
        private readonly IApplicationService applicationService;
        private readonly IStringLocalizer localizer;
        public ApplicationController(
            INomenclatureService _nomenclatureService,
            IApplicationService _applicationService,
            IStringLocalizer _localizer)
        {
            nomenclatureService = _nomenclatureService;
            localizer = _localizer;
            applicationService = _applicationService;
        }

        public async Task<IActionResult> ListApplication([FromBody] GridRequestModel request)
        {
            var data = await applicationService.GetApplicationList();
            var model = await applicationService.FilterApplicationList(request.data, data, request.page, request.size);
           
            return new JsonResult(model);
        }
        public async Task<IActionResult> ListApplicationDraft([FromBody] GridRequestModel request)
        {
            var model = await applicationService.GetApplicationListDraft(request.page, request.size);
            return new JsonResult(model);
        }
       
        public async Task<IActionResult> Add(string? permitType)
        {
            var applicationId = Guid.NewGuid();
            if (!string.IsNullOrEmpty(permitType))
            {
                var application = await applicationService.GetApplication(applicationId);
                var appType = application.GetApplicationType()!;
                appType.PermitTypeInit = permitType;
                appType.PermitType = permitType;
                appType.ApplicationTypeCode = permitType == PERMIT_TYPE.Temporary ? APPLICATION_TYPE.Temporary : APPLICATION_TYPE.Permanent;
                await applicationService.SaveApplicationType(applicationId, appType);
            }
            return RedirectToAction(nameof(Edit), new { applicationId  });
        }
        public async Task<IActionResult> Edit(Guid applicationId)
        {
            var application = await applicationService.GetApplication(applicationId);
            await nomenclatureService.SetViewBagApplication(ViewBag, application.GetApplicationTypeCode());
            application.GetEmployer()?.SetIsCompanyAddress(true);
            application.GetApplicant()?.Person?.Employer?.SetIsCompanyAddress(true);
            return View(nameof(Edit), application);
        }
        public async Task<IActionResult> AddAddress(int index, string prefix)
        {
            var ddl = await nomenclatureService.GetNomenclatureDDL(NomenclatureTypes.ADDRESSE_TYPE, false);
            if (prefix.Contains("Employer."))
            {
                ddl = ddl.Where(x => x.Value != ADDRESSE_TYPE.Permanent && x.Value != ADDRESSE_TYPE.Current).ToList();
            }
            else
            {
                ddl = ddl.Where(x => x.Value != ADDRESSE_TYPE.Head).ToList();
            }
            var address = new AddressVM
            {
                Index = index,
                KindDDL = ddl
            };
            ViewData.TemplateInfo.HtmlFieldPrefix = $"{prefix}.Items[{index}]";
            await nomenclatureService.SetViewBagAddress(ViewBag);
            return PartialView("_Address", address);
        }
        public async Task<IActionResult> AddContact(int index, string prefix)
        {
            var contact = new ContactVM
            {
                Index = index,
            };
            ViewData.TemplateInfo.HtmlFieldPrefix = $"{prefix}.Items[{index}]";
            await nomenclatureService.SetViewBagContact(ViewBag);
            return PartialView("_Contact", contact);
        }
        public async Task<IActionResult> AddPersonIdDocument(int index, string prefix)
        {
            var personIdDocument = new PersonIdDocumentVM
            {
                Index = index,
            };
            ViewData.TemplateInfo.HtmlFieldPrefix = $"{prefix}.Items[{index}]";
            await nomenclatureService.SetViewBagPersonIdDocument(ViewBag);
            return PartialView("_PersonIdDocument", personIdDocument);
        }

        private void RemoveErrorForNotUsed(string startWith)
        {
            var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                                     .Select(x => new { x.Key, x.Value.Errors })
                                     .ToList();
            foreach (var error in errors)
            {
                if (error.Key.StartsWith(startWith))
                {
                    ModelState.Remove(error.Key);
                }
            }
        }
        private JsonResult SaveResult(string state, string message)
        {
            return Json(new { state, message });
        }

        private JsonResult SaveErrorValidation()
        {
            var message = "Невалидни данни";
            foreach (var value in ModelState.Where(x => x.Value != null && x.Value?.Errors.Count > 0))
            {
                foreach (var error in value.Value!.Errors) {
                    message += Environment.NewLine + error.ErrorMessage;
                }
            }
            return SaveResult("Error", message);
        }
        private JsonResult SaveOk(string message = "")
        {
            return SaveResult("OK", message);
        }
        [HttpPost]
        [RequestFormLimits(ValueCountLimit = 99999999)]
        public async Task<JsonResult> SaveApplicationType(Guid applicationId, [Bind(Prefix = "ApplicationType")] ApplicationTypeVM model)
        {
            if (model.ApplicationTypeCode == APPLICATION_TYPE.Temporary)
            {
                RemoveErrorForNotUsed("ApplicationType.PermitType");
                model.PermitType = PERMIT_TYPE.Temporary;

            }

            if (!ModelState.IsValid)
            {
                return SaveErrorValidation();
            }
            await applicationService.SaveApplicationType(applicationId, model);
            return SaveOk();
        }
        [HttpPost]
        [RequestFormLimits(ValueCountLimit = 99999999)]

        public async Task<JsonResult> SaveApplicant(Guid applicationId, [Bind(Prefix = "Applicant")] ApplicantVM model)
        {
            RemoveApplicantValidation(model);
            if (!ModelState.IsValid)
            {
                return SaveErrorValidation();
            }
            await applicationService.SaveApplicant(applicationId, model);
            return SaveOk();
        }
        public void RemoveDocumentValidation(DocumentsVM documents)
        {
            foreach (var document in documents.Documents)
            {
                if (!document.IsMandatory && string.IsNullOrEmpty(document.CmisId))
                {
                    RemoveErrorForNotUsed($"Documents.Documents[{document.Index}]");
                }
            }
        }

        private void RemoveApplicantValidation(ApplicantVM model)
        {
            if (model.ApplicantType == ENTITY_TYPE.Foreigner)
            {
                model.Foreigner.Identifier = model.Lnch;
                model.Foreigner.TypeIdentifier = FOREIGNER_TYPE_IDENTIFIER.InBg;
                RemoveErrorForNotUsed("Applicant.Person.Address.Kind");
                RemoveErrorForNotUsed("Applicant.Foreigner.Fid");
                RemoveErrorForNotUsed("Applicant.Egn");
                RemoveErrorForNotUsed("Applicant.Foreigner.Identifier");
                RemoveErrorForNotUsed("Applicant.Foreigner.TypeIdentifier");
                RemoveErrorForNotUsed("Applicant.Person");
            }
            if (model.ApplicantType == ENTITY_TYPE.AuthorizedPerson || model.ApplicantType == ENTITY_TYPE.Representative)
            {
                model.Person.Address.Kind = ADDRESSE_TYPE.Correspondence;
                RemoveErrorForNotUsed("Applicant.Person.Address.Kind");
                if (model.UicType == "EGN")
                {
                    RemoveErrorForNotUsed("Applicant.Lnch");
                }
                if (model.UicType == "LNCH")
                {
                    RemoveErrorForNotUsed("Applicant.Egn");
                }
                RemoveErrorForNotUsed("Applicant.Foreigner");
                if (model.Person.ApplicantRole != ApplicationRole.Representative)
                {
                    RemoveErrorForNotUsed("Applicant.Person.Employer");
                }
                if (model.Person.ApplicantRole == ApplicationRole.Representative) {
                    RemoveErrorForNotUsed("Applicant.Person.Employer.EmployeeCount");
                    RemoveErrorForNotUsed("Applicant.Person.Employer.ForeignEmployeeCount");
                    if (model.Person.Employer.ContactAddressIsSame != YESNO_TYPE.No)
                    {
                        RemoveErrorForNotUsed("Applicant.Person.Employer.ContactAddress");
                    }
                    if (model.Person.ApplicantContactAddressIsSame != YESNO_TYPE.No)
                    {
                        RemoveErrorForNotUsed("Applicant.Person.Address");
                    }
                }
            }
        }

        [HttpPost]
        [RequestFormLimits(ValueCountLimit = 99999999)]
        public async Task<JsonResult> SaveForeigner(Guid applicationId, [Bind(Prefix = "Foreigner")] ForeignerVM model)
        {
            if (model.TypeIdentifier == FOREIGNER_TYPE_IDENTIFIER.External)
            {
                RemoveErrorForNotUsed("Foreigner.Identifier");
                RemoveErrorForNotUsed("Foreigner.EntryDate");
                RemoveErrorForNotUsed("Foreigner.EntryPoint");
                RemoveErrorForNotUsed("Foreigner.VisaType");
                RemoveErrorForNotUsed("Foreigner.VisaSerialNumber");
                RemoveErrorForNotUsed("Foreigner.VisaExpirationDate");
            }
            if (!ModelState.IsValid)
            {
                return SaveErrorValidation();
            }
            await applicationService.SaveForeigner(applicationId, model);
            return SaveOk();
        }

        [HttpPost]
        [RequestFormLimits(ValueCountLimit = 99999999)]
        public async Task<JsonResult> SaveForeignerSmallList(Guid applicationId, [Bind(Prefix = "ForeignerSmallList")] ForeignerSmallListVM model)
        {
            if (!ModelState.IsValid)
            {
                return SaveErrorValidation();
            }
            await applicationService.SaveForeignerSmallList(applicationId, model);
            return SaveOk();
        }

        [HttpPost]
        [RequestFormLimits(ValueCountLimit = 99999999)]
        public async Task<JsonResult> SaveEmployer(Guid applicationId, [Bind(Prefix = "Employer")] EmployerVM model)
        {
            var application = await applicationService.GetApplication(applicationId);
            var appType = application.GetApplicationType();
            if (appType?.ApplicationTypeCode == APPLICATION_TYPE.Temporary)
            {
                RemoveErrorForNotUsed("Employer.EmployeeCount");
                RemoveErrorForNotUsed("Employer.ForeignEmployeeCount");
            }
            if (model.ContactAddressIsSame != YESNO_TYPE.No)
            {
                RemoveErrorForNotUsed("Employer.ContactAddress");
            }

            if (!ModelState.IsValid)
            {
                return SaveErrorValidation();
            }
            await applicationService.SaveEmployer(applicationId, model);
            return SaveOk();
        }
        [HttpPost]
        [RequestFormLimits(ValueCountLimit = 99999999)]
        public async Task<JsonResult> SaveEmployment(Guid applicationId, [Bind(Prefix = "Employment")] EmploymentVM model)
        {
            if (model.AddressIsSame == YESNO_TYPE.Yes)
            {
                RemoveErrorForNotUsed("Employment.Address");
            }
            var application = await applicationService.GetApplication(applicationId);
            var appType = application.GetApplicationType();
            if (appType?.ApplicationTypeCode == APPLICATION_TYPE.Temporary)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                                    .Select(x => new { x.Key, x.Value.Errors })
                                    .ToList();

                foreach (var error in errors)
                {
                    if (!error.Key.StartsWith("Employment.Address"))
                    {
                        ModelState.Remove(error.Key);
                    }
                }
            }
            if (!ModelState.IsValid)
            {
                return SaveErrorValidation();
            }
            await applicationService.SaveEmployment(applicationId, model);
            return SaveOk();
        }
        public async Task<JsonResult> SaveApplicationInfo(Guid applicationId, [Bind(Prefix = "ApplicationInfo")] ApplicationInfoVM model)
        {
            await applicationService.SaveApplicationInfo(applicationId, model);
            return SaveOk();
        }

        [HttpPost]
        [RequestFormLimits(ValueCountLimit = 99999999)]
        public async Task<JsonResult> SaveDocuments(Guid applicationId, [Bind(Prefix = "Documents")] DocumentsVM model)
        {
            await applicationService.SaveDocuments(applicationId, model);
            return SaveOk();
        }

        [HttpGet]
        public async Task<JsonResult> GetCities(string region)
        {
            var ddl = await nomenclatureService.GetRegionCities(region);
            return Json(ddl);
        }
        [HttpGet]
        public async Task<IActionResult> AddDocuments(Guid applicationId, string permitType, string applicantType)
        {
            ViewData.TemplateInfo.HtmlFieldPrefix = "Documents";
            var model = await applicationService.GetDocumentTypes(applicationId, permitType, applicantType);
            return PartialView("_Documents", model);
        }
        [HttpGet]
        public async Task<IActionResult> AddForeigner(Guid applicationId)
        {
            ViewData.TemplateInfo.HtmlFieldPrefix = "Foreigner";
            await nomenclatureService.SetViewBagApplication(ViewBag, null);
            var model = await applicationService.GetApplication(applicationId);
            var foreigner = model.GetApplicant()?.Foreigner;
            foreigner = foreigner ?? new ForeignerVM();
            return PartialView("_Foreigner", foreigner);

        }

        [HttpGet]
        public async Task<IActionResult> AddEmployer(Guid applicationId)
        {
            ViewData.TemplateInfo.HtmlFieldPrefix = "Employer";
            await nomenclatureService.SetViewBagApplication(ViewBag, null);
            var model = await applicationService.GetApplication(applicationId);
            var employer = model.GetEmployer();
            if (string.IsNullOrEmpty(employer?.Identifier))
               employer = model.GetApplicant()?.Person?.Employer;
            employer = employer ?? new EmployerVM();
            return PartialView("_Employer", employer);
        }

        [HttpGet]
        public async Task<JsonResult> SendApplication(Guid applicationId)
        {
            (var state, var message) = await applicationService.SendApplication(applicationId);
            return SaveResult(state, message);
        }

        [HttpGet]
        public async Task<JsonResult> SetStatusDraft(Guid applicationId)
        {
            await applicationService.SetStatusDraft(applicationId);
            return SaveOk();
        }

        [HttpGet]
        public async Task<JsonResult> SetStatusNone(Guid applicationId)
        {
            await applicationService.SetStatusNone(applicationId);
            return SaveOk();
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployer(string uic, string prefix)
        {
            var employer = await applicationService.GetEmployer(uic);
            ViewData.TemplateInfo.HtmlFieldPrefix = prefix;
            await nomenclatureService.SetViewBagApplication(ViewBag, null);

            if (prefix == "Applicant.Person.Employer")
            {
                ViewData["ApplicantRole"] = "2";
                ViewData["ApplicantType"] = ENTITY_TYPE.AuthorizedPerson;
                employer.SetIsCompanyAddress(true);
                return PartialView("_ApplicantEmployer", employer);
            }
            employer.SetIsCompanyAddress(true);
            return PartialView("_Employer", employer);
        }
        public async Task<IActionResult> AddForeignerSmall(int index)
        {
            var model = new ForeignerSmallVM { Index = index };
            ViewBag.Countries_ddl = await nomenclatureService.GetNomenclatureDDL(NomenclatureTypes.COUNTRIES, true);
            return PartialView("_ForeignerSmallEdit", model);
        }
        public async Task<IActionResult> EditForeignerSmall(ForeignerSmallVM model)
        {
            model.IsEdit = true;
            ViewBag.Countries_ddl = await nomenclatureService.GetNomenclatureDDL(NomenclatureTypes.COUNTRIES, true);
            return PartialView("_ForeignerSmallEdit", model);
        }

        public async Task<IActionResult> ForeignerSmallEdit(ForeignerSmallVM model)
        {
            ViewData.TemplateInfo.HtmlFieldPrefix = $"ForeignerSmallList.Items[{model.Index}]";
            model.Nationality = await nomenclatureService.GetNomenclatureText(NomenclatureTypes.COUNTRIES, model.NationalityCode);
            return PartialView("_ForeignerSmall", model);
        }
        public async Task<IActionResult> Index()
        {
            var model = new ApplicationFilterVM();
            await nomenclatureService.SetViewBagApplicationFilter(ViewBag);
            return View(model);
        }
        public IActionResult IndexDraft()
        {
            return View();
        }
       
        public async Task<IActionResult> AddComplaint(string applicationNumber)
        {
            await nomenclatureService.SetViewBagApplication(ViewBag, null);
            var model = applicationService.InitNewComplaint(applicationNumber);
            return View("EditComplaint", model);
        }
        public async Task<IActionResult> SaveComplaint(ComplaintVM model)
        {
            RemoveApplicantValidation(model.Applicant);
            if (!ModelState.IsValid)
            {
                await nomenclatureService.SetViewBagApplication(ViewBag, null);
                return View("EditComplaint", model);
            }
            (var result, var message) = await applicationService.SaveComplaint(model);
            if (result)
            {
                SetSuccessMessage(message, true);
                return RedirectToAction("Index", "Home");
            }
            await nomenclatureService.SetViewBagApplication(ViewBag, null);
            SetErrorMessage(message, true);
            return View("EditComplaint", model);
        }

        public async Task<IActionResult> ApplicationUpdate(string applicationNumber)
        {
            await nomenclatureService.SetViewBagApplication(ViewBag, null);
            var model = applicationService.InitNewApplicationUpdate(applicationNumber);
            return View("ApplicationUpdate", model);
        }
        public async Task<IActionResult> SaveApplicationUpdate(ApplicationUpdateVM model)
        {
            RemoveApplicantValidation(model.Applicant);
            RemoveDocumentValidation(model.Documents);
            if (!ModelState.IsValid)
            {
                await nomenclatureService.SetViewBagApplication(ViewBag, null);
                return View("ApplicationUpdate", model);
            }
            (var result, var message) = await applicationService.SaveApplicationUpdate(model);
            if (result)
            {
                SetSuccessMessage(message);
                return RedirectToAction("Index", "Home");
            }
            await nomenclatureService.SetViewBagApplication(ViewBag, null);
            SetErrorMessage(message);
            return View("ApplicationUpdate", model);
        }

        public async Task<IActionResult> AddSelfDenial(string applicationNumber)
        {
            await nomenclatureService.SetViewBagApplication(ViewBag, null);
            var model = applicationService.InitNewSelfDenial(applicationNumber);
            return View("EditSelfDenial", model);
        }

        public async Task<IActionResult> SaveSelfDenial(SelfDenialVM model)
        {
            RemoveApplicantValidation(model.Applicant);
            if (!ModelState.IsValid)
            {
                await nomenclatureService.SetViewBagApplication(ViewBag, null);
                return View("EditSelfDenial", model);
            }
            (var result, var message) = await applicationService.SaveSelfDenial(model);
            if (result)
            {
                SetSuccessMessage(message);
                return RedirectToAction("Index", "Home");
            }
            await nomenclatureService.SetViewBagApplication(ViewBag, null);
            SetErrorMessage(message);
            return View("EditSelfDenial", model);
        }
        public async Task<IActionResult> GetDocument(string cmisId, string fileName)
        {
            var result = await applicationService.DownloadFile(cmisId);
            var bytes = Convert.FromBase64String(result.Content);
            return File(bytes, result.MimeType, Uri.EscapeDataString(result.FileName));
        }
        
    }
}
