using BlueCardPortal.Core.Contracts;
using BlueCardPortal.Infrastructure.Constants;
using BlueCardPortal.Infrastructure.Model.ApplicationList;
using BlueCardPortal.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BlueCardPortal.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> logger;
        private readonly INomenclatureService nomenclatureService;

        public HomeController(
            ILogger<HomeController> _logger,
            INomenclatureService _nomenclatureService)
        {
            nomenclatureService = _nomenclatureService;
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var error = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error.Message;
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
    }
}
