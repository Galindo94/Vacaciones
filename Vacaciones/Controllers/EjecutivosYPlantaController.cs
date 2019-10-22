using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vacaciones.Utilities;

namespace Vacaciones.Controllers
{
    public class EjecutivosYPlantaController : Controller
    {
        // GET: EjecutivosYPlanta
        public ActionResult Index()
        {
            ViewBag.NombreEmpleado = "Jose Daniel Sepulveda Galindo";
            ViewBag.NumeroDias = 9;


            // Se obtienen las fechas de los festivos, sabados y domingos (Si se envía true incluira los sábados, si se envía false no incluirá los sábados, según criterio)
            string DiasFestivosSabadosDomingos = FestivosColombia.DiasFestivoSabadosDomingosConcatenado(DateTime.Now.Year, true);
            ViewBag.DiasFestivosSabadosDomingos = DiasFestivosSabadosDomingos;

            // Fecha donde el usuario puede iniciar sus vacaciones (Se le agregan los días según condicion, 1 o 15 días)
            ViewBag.InicioFecha = DateTime.Now.AddDays(15);
            

            // Fecha maxima que se le mostrará al usuario para pedir sus vacaciones (Esta para 60 días se puede cambiar según criterio)
            ViewBag.FinFecha = DateTime.Now.AddDays(60);
            return View();
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
