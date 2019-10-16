using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Vacaciones.Controllers
{
    public class AnotadorController : Controller
    {
        // GET: Anotador
        public ActionResult Index()
        {
            ViewBag.NombreEmpleado = "Carlos Andres Ospina Estrada";
            ViewBag.NumeroDias = 12;
            return View();
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
