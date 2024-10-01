using BlueCardPortal.Core.Contracts;
using BlueCardPortal.Infrastructure.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace BlueCardPortal.Controllers
{
    public class ReportController : BaseController
    {
        private readonly INomenclatureService nomenclatureService;
        private readonly IApplicationService applicationService;
        private readonly IStringLocalizer localizer;
        public ReportController(
            INomenclatureService _nomenclatureService,
            IApplicationService _applicationService,
            IStringLocalizer _localizer)
        {
            nomenclatureService = _nomenclatureService;
            localizer = _localizer;
            applicationService = _applicationService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult IndexComplaint()
        {
            return View();
        }
        public async Task<IActionResult> IndexOnStatus()
        {
            await nomenclatureService.SetViewBagApplicationFilter(ViewBag);
            return View();
        }

        public async Task<IActionResult> IndexOnForeigner()
        {
            await nomenclatureService.SetViewBagApplicationFilter(ViewBag);
            return View();
        }
        public async Task<IActionResult> ListComplaint([FromBody] GridRequestModel request)
        {
            var model = await applicationService.GetComplaintList(request.page, request.size);
            return new JsonResult(model);
        }

        public async Task<IActionResult> ListApplicationOnStatus([FromBody] GridRequestModel request)
        {
            var data = await applicationService.GetApplicationList();
            var model = await applicationService.FilterApplicationList(request.data, data, request.page, request.size);

            return new JsonResult(model);
        }

        public async Task<IActionResult> ListApplicationOnForeigner([FromBody] GridRequestModel request)
        {
            var data = await applicationService.GetApplicationList();
            var model = await applicationService.FilterApplicationList(request.data, data, request.page, request.size);

            return new JsonResult(model);
        }
    }
}
