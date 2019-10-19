using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft;
using Newtonsoft.Json;
using Vacaciones.Models;
using Vacaciones.Utilities;

namespace Vacaciones.Controllers
{
    public class EjecutivosYPlantaController : Controller
    {

        MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
        // GET: EjecutivosYPlanta
        public ActionResult Index(String Valores)
        {
            Persona ols = new Persona();
            ols = JsonConvert.DeserializeObject<Persona>(Valores);

            ViewBag.NombreEmpleado = ols.NombrePersona;
            ViewBag.NumeroDias = ols.NumeroDias;

            return View();
        }

        public JsonResult ValidarCantidadDias(int NumeroDias, float NumDiasDisponibles)
        {
            if (NumeroDias < 6)
            {
                oMensajeRespuesta = new MensajeRespuesta
                {
                    Codigo = "1",
                    Mensaje = "La cantidad de dias debe ser superior a 6",
                    Resultado = Json("", JsonRequestBehavior.AllowGet)
                };
            }

            if (NumeroDias > NumDiasDisponibles)
            {
                oMensajeRespuesta = new MensajeRespuesta
                {
                    Codigo = "1",
                    Mensaje = "La cantidad de dias debe ser menor o igual al numero de dias disponibles",
                    Resultado = Json("", JsonRequestBehavior.AllowGet)
                };
            }

            return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
        }



        // GET: EjecutivosYPlanta/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: EjecutivosYPlanta/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EjecutivosYPlanta/Create
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

        // GET: EjecutivosYPlanta/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: EjecutivosYPlanta/Edit/5
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

        // GET: EjecutivosYPlanta/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EjecutivosYPlanta/Delete/5
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
