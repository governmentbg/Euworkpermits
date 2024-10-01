using BlueCardPortal.Infrastructure.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlueCardPortal.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        public void SetSuccessMessage(string message)
        {
            TempData[MessageConstant.SuccessMessage] = message;
        }
        public void SetErrorMessage(string message)
        {
            TempData[MessageConstant.ErrorMessage] = message;
        }
        public void SetWarningMessage(string message)
        {
            TempData[MessageConstant.WarningMessage] = message;
        }
    }
}
