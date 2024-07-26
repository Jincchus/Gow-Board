using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GowBoard.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index(int statusCode = 500)
        {
            ViewBag.StatusCode = statusCode;
            ViewBag.ErrorMessage = GetErrorMessage(statusCode);
            ViewBag.ErrorDescription = GetErrorDescription(statusCode);
            return View("Error");
        }

        private string GetErrorMessage(int statusCode)
        {
            switch (statusCode)
            {
                case 400:
                    return "잘못된 요청입니다.";
                case 401:
                    return "인증이 필요합니다.";
                case 403:
                    return "접근이 금지되었습니다.";
                case 404:
                    return "페이지를 찾을 수 없습니다.";
                case 500:
                    return "서버 오류입니다.";
                default:
                    return "오류가 발생했습니다.";
            }
        }

        private string GetErrorDescription(int statusCode)
        {
            switch (statusCode)
            {
                case 400:
                    return "요청이 잘못되었습니다. 입력값을 확인하고 다시 시도해 주세요.";
                case 401:
                    return "이 페이지에 접근하려면 로그인이 필요합니다.";
                case 403:
                    return "이 페이지에 대한 접근 권한이 없습니다.";
                case 404:
                    return "요청하신 페이지를 찾을 수 없습니다. URL을 확인하시고 다시 시도해 주세요.";
                case 500:
                    return "서버에서 오류가 발생했습니다. 잠시 후 다시 시도해 주세요.";
                default:
                    return "예기치 않은 오류가 발생했습니다. 다시 시도해 주세요.";
            }
        }
    }
}