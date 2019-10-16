using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Vacaciones.Controllers
{
    public class EjecutivosYPlantaController : Controller
    {
        // GET: EjecutivosYPlanta
        public ActionResult Index()
        {
            ViewBag.NombreEmpleado = "Jose Daniel Sepulveda Galindo";
            ViewBag.NumeroDias = 9;
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
