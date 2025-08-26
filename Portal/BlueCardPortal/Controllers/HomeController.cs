using BlueCardPortal.Core.Contracts;
using BlueCardPortal.Core.Services;
using BlueCardPortal.Infrastructure.Constants;
using BlueCardPortal.Infrastructure.Model.ApplicationList;
using BlueCardPortal.Infrastructure.Model.Common;
using BlueCardPortal.Infrastructure.Model.Statistics;
using BlueCardPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg.Sig;
using System.Diagnostics;

namespace BlueCardPortal.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> logger;
        private readonly INomenclatureService nomenclatureService;
        private readonly IApplicationService applicationService;

        public HomeController(
            ILogger<HomeController> _logger,
            INomenclatureService _nomenclatureService,
            IApplicationService _applicationService)
        {
            nomenclatureService = _nomenclatureService;
            applicationService = _applicationService;
            logger = _logger;
        }
      
        public async Task<IActionResult> Index()
        {
            var model = new ApplicationFilterVM();
            ViewBag.PermitType_ddl = await nomenclatureService.GetNomenclatureDDL(NomenclatureTypes.PERMIT_TYPE, true);
            ViewBag.Status_ddl = await nomenclatureService.GetNomenclatureDDL(NomenclatureTypes.EXTERNAL_STATUS, true);
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult Blanks()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            DashboardFilter model = new();
            await DashboardViewBag();
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Dashboard(DashboardFilter model)
        {
            await DashboardViewBag();
            return View(model);
        }

        private async Task DashboardViewBag()
        {
            ViewBag.PermitType_ddl = await nomenclatureService.GetNomenclatureDDL(NomenclatureTypes.PERMIT_TYPE, false);
            ViewBag.Countries_ddl = await nomenclatureService.GetNomenclatureDDL(NomenclatureTypes.COUNTRIES, false);
            ViewBag.Year_ddl = nomenclatureService.GetStatisticsYear();
        }


        [AllowAnonymous]
        public IActionResult EService()
        {
            return View();
        }
        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var error = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error.Message;
            string traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            logger.LogError(error, "TraceId: {0}", traceId);

            if (error != null && error.StartsWith("ErrorAjax:"))
            {
                return PartialView("ErrorAjax", new ErrorViewModel { Message = error.Replace("ErrorAjax:", string.Empty), RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
            //return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            return PartialView("ErrorAjax", new ErrorViewModel { Message = "Възникна непредвидена грешка", RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> PermitsSeries([FromBody] DashboardFilter model)
        {
            var result = await applicationService.GetPermitData(model);
            return Json(result);
        }

        [AllowAnonymous]
        public IActionResult AccessabilityPolicy()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult UserHelp()
        {
            return View();
        }

        public async Task<IActionResult> FeedBack()
        {
            var model = new FeedBackVM();
            ViewBag.TypeId_ddl = await nomenclatureService.GetNomenclatureCodeableDDL(NomenclatureTypesCodeable.FeedBackType);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FeedBack(FeedBackVM model)
        {
            await nomenclatureService.SaveFeedBack(model);
            TempData[MessageConstant.SuccessMessage] = "Успешен запис";
            return RedirectToAction("Index", "Home");
        }
    }
}
