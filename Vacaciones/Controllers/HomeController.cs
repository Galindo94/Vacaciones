using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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



        public JsonResult GetCiudadList()
        {
            List<ListaEmpleadosModels> oLstEmpleados = new List<ListaEmpleadosModels>
            {
                new ListaEmpleadosModels
                {
                    Cedula = "1152694826",
                    NumeroDias=7,
                    FechaInicio = "16/10/2019",
                    FechaFin = "25/10/2019"
                },
                 new ListaEmpleadosModels
                {
                    Cedula = "1152694826",
                    NumeroDias=7,
                    FechaInicio = "16/10/2019",
                    FechaFin = "25/10/2019"
                },
                  new ListaEmpleadosModels
                {
                    Cedula = "1152694826",
                    NumeroDias=7,
                    FechaInicio = "16/10/2019",
                    FechaFin = "25/10/2019"
                },
                   new ListaEmpleadosModels
                {
                    Cedula = "1152694826",
                    NumeroDias=7,
                    FechaInicio = "16/10/2019",
                    FechaFin = "25/10/2019"
                },
                    new ListaEmpleadosModels
                {
                    Cedula = "1152694826",
                    NumeroDias=7,
                    FechaInicio = "16/10/2019",
                    FechaFin = "25/10/2019"
                },
                     new ListaEmpleadosModels
                {
                    Cedula = "1152694826",
                    NumeroDias=7,
                    FechaInicio = "16/10/2019",
                    FechaFin = "25/10/2019"
                },
                      new ListaEmpleadosModels
                {
                    Cedula = "1152694826",
                    NumeroDias=7,
                    FechaInicio = "16/10/2019",
                    FechaFin = "25/10/2019"
                },
                       new ListaEmpleadosModels
                {
                    Cedula = "1152694826",
                    NumeroDias=7,
                    FechaInicio = "16/10/2019",
                    FechaFin = "25/10/2019"
                },
            };

            return Json(oLstEmpleados, JsonRequestBehavior.AllowGet);
        }
    }
}