using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vacaciones.Models;
using System.Data;
using Newtonsoft.Json;

namespace Vacaciones.Controllers
{
    public class AnotadorController : Controller
    {
     
        List<EmpleadoModels> oLstEmpleados = new List<EmpleadoModels>();

        // GET: Anotador
        public ActionResult Index()
        {
            ViewBag.NombreEmpleado = "Carlos Andres Ospina Estrada";
            ViewBag.NumeroDias = 12;
            return View();
        }

        public JsonResult AgregarEmpleado(string Cedula, string NumeroDias, string FechaInicio, string FechaFin, string DataActual)
        {

            //var result = HttpContext.Current.Session["Products"] as IList<ProductViewModel>;
            oLstEmpleados = JsonConvert.DeserializeObject<List<EmpleadoModels>>(DataActual);

            if (!string.IsNullOrEmpty(Cedula))
            {
                EmpleadoModels os = new EmpleadoModels
                {
                    Cedula = Cedula,
                    NumeroDias = Convert.ToInt32(NumeroDias),
                    FechaInicio = FechaInicio,
                    FechaFin = FechaFin

                };
                oLstEmpleados.Add(os);

            }

            return Json(oLstEmpleados, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ConsultarListaEmpleado()
        {
            return Json(oLstEmpleados, JsonRequestBehavior.AllowGet);
        }

        // GET: Anotador/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Anotador/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Anotador/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Anotador/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Anotador/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Anotador/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Anotador/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
