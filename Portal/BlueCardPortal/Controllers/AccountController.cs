using BlueCardPortal.Infrastructure.Constants;
using BlueCardPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static EAuthIntegration.Models.EAuthIntegrationDefaults;

namespace BlueCardPortal.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ILogger logger;
        private readonly IConfiguration config;

        public AccountController(
            ILogger<AccountController> _logger,
            IConfiguration _config)
        {
            logger = _logger;
            config = _config;
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string? returnUrl = null, string? error = null)
        {
            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };


            if (!string.IsNullOrEmpty(error))
            {
                ViewBag.errorMessage = error;
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> LogOff()
        {

            return LocalRedirect("/");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied(string returnUrl)
        {
            TempData[MessageConstant.ErrorMessage] = MessageConstant.Values.Unauthorized;

            return LocalRedirect("/");
        }
        
        
    }
}
