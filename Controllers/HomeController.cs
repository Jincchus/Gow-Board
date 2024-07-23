using GowBoard.Models.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GowBoard.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBoardService _boardService;
        public ActionResult Index()
        {
            return View();
        }

    }
}