﻿using BlueCardPortal.Core.Contracts;
using BlueCardPortal.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace BlueCardPortal.Controllers
{
    public class ApplicationPreviewController : BaseController
    {
        private readonly IApplicationService applicationService;
        private readonly INomenclatureService nomenclatureService;
        private readonly IStringLocalizer localizer;

        public ApplicationPreviewController(
            IApplicationService _applicationService,
            INomenclatureService _nomenclatureService,
            IStringLocalizer _localizer
            )
        {
            applicationService = _applicationService;
            nomenclatureService = _nomenclatureService;
            localizer = _localizer;
        }
        public async Task<IActionResult> PreviewLocal(Guid applicationId)
        {
            await nomenclatureService.SetViewBagApplication(ViewBag);
            var model = await applicationService.GetApplication(applicationId, true);
            var list = model.ApplicationItems.Where(x => !string.IsNullOrEmpty(x.Label)).ToList();
            return PartialView("_Preview", list);
        }
        public async Task<IActionResult> GetApplicationRemote(string applicationId)
        {
            await nomenclatureService.SetViewBagApplication(ViewBag);
            var model = await applicationService.GetApplicationRemote(applicationId);
            var list = model.ApplicationItems.Where(x => !string.IsNullOrEmpty(x.Label)).ToList();
            return PartialView("_Preview", list);
        }

    }
}
