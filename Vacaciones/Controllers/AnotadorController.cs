using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vacaciones.Models;
using System.Data;
using Newtonsoft.Json;
using Vacaciones.Utilities;

namespace Vacaciones.Controllers
{
    public class AnotadorController : Controller
    {
        List<EmpleadoModels> oLstEmpleados = new List<EmpleadoModels>();
        MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
        // GET: Anotador
        public ActionResult Index()
        {
            ViewBag.NombreEmpleado = "Carlos Andres Ospina Estrada";
            ViewBag.NumeroDias = 12;
            return View();
        }

        public JsonResult AgregarEmpleado(string Cedula, string NumeroDias, string FechaInicio, string FechaFin, string DataActual)
        {

            oLstEmpleados = JsonConvert.DeserializeObject<List<EmpleadoModels>>(DataActual);

            EmpleadoModels oEmpleado = new EmpleadoModels
            {
                Cedula = Cedula,
                NumeroDias = Convert.ToInt32(NumeroDias),
                FechaInicio = FechaInicio,
                FechaFin = FechaFin
            };

            //Se valida si ya la cedula ha sido agregada
            int Existe = oLstEmpleados
                .Where(w => w.Cedula == oEmpleado.Cedula).Count();
            if (Existe == 0)
            {
                oLstEmpleados.Add(oEmpleado);

                oMensajeRespuesta = new MensajeRespuesta
                {
                    Codigo = "1",
                    Mensaje = "Empleado agregado correctamente.",
                    Resultado = Json(oLstEmpleados, JsonRequestBehavior.AllowGet)

                };
            }
            else
            {
                oMensajeRespuesta = new MensajeRespuesta
                {
                    Codigo = "2",
                    Mensaje = "El empleado ya ha sido agregado a la lista.",
                    Resultado = Json("", JsonRequestBehavior.AllowGet)
                };


            }

            return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);

        }

        public JsonResult ConsultarListaEmpleado()
        {
            if (oLstEmpleados == null)
            {
                oLstEmpleados = new List<EmpleadoModels>();
            }

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
