using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Vacaciones.Controllers
{
    public class FinVacacionesController : Controller
    {
        // GET: FinVacaciones
        public ActionResult Index()
        {
            return View();
        }

        // GET: FinVacaciones/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: FinVacaciones/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FinVacaciones/Create
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

        // GET: FinVacaciones/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: FinVacaciones/Edit/5
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

        // GET: FinVacaciones/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: FinVacaciones/Delete/5
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
