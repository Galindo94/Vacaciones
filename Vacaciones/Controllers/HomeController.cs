using System.Collections.Generic;
using System.Web.Mvc;
using Vacaciones.Models;

namespace Vacaciones.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            ViewBag.NumeroDias = 22;


            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Detalles()
        {
            return PartialView();
        }

    }
}
