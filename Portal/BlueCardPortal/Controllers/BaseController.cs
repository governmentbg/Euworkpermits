using BlueCardPortal.Infrastructure.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlueCardPortal.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        public void SetSuccessMessage(string message, bool swal = false)
        {
            if (swal)
            {
                TempData[MessageConstant.SwalSuccessMessage] = message;
            }
            else
            {
                TempData[MessageConstant.SuccessMessage] = message;
            }
        }
        public void SetErrorMessage(string message, bool swal = false)
        {
            if (swal)
            {
                TempData[MessageConstant.SwalErrorMessage] = message;
            }
            else
            {
                TempData[MessageConstant.ErrorMessage] = message;
            }
        }
        public void SetWarningMessage(string message, bool swal = false)
        {
            if (swal)
            {
                TempData[MessageConstant.SwalWarningMessage] = message;
            }
            else
            {
                TempData[MessageConstant.WarningMessage] = message;
            }
        }
    }
}
