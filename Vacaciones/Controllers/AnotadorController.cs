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
        Persona oPersona = new Persona();
        // GET: Anotador
        public ActionResult Index(string Valores)
        {
            Persona ols = new Persona();
            ols = JsonConvert.DeserializeObject<Persona>(Valores);

            ViewBag.NombreEmpleado = ols.NombrePersona;
            ViewBag.NumeroDias = ols.NumeroDias;
            ViewBag.Documento = ols.Documento;
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

        public JsonResult ConsultarEmpleadosAnotador(string Cedula)
        {
            try
            {

                bool Encontro = false;


                #region Escenario 1 planta ejecutiva


                if (Cedula == "98714393" && !Encontro)
                {
                    oMensajeRespuesta.Codigo = "1";
                    oPersona.Documento = Cedula;
                    oPersona.NombrePersona = "NELSON ENRIQUE USUGA MESA";
                    oPersona.NumeroDias = 19.42;
                    Encontro = true;
                }

                if (Cedula == "1045138486" && !Encontro)
                {
                    oMensajeRespuesta.Codigo = "1";
                    oPersona.Documento = Cedula;
                    oPersona.NombrePersona = "JOHN FREDIS CORDOBA VASQUEZ";
                    oPersona.NumeroDias = 19.67;
                    Encontro = true;
                }

                #endregion



                if (!Encontro)
                {
                    oMensajeRespuesta.Codigo = "2";
                    oMensajeRespuesta.Mensaje = "El documento ingresado no se encontro en el sistema. Verifiquelo e intentelo de nuevo.";
                    oMensajeRespuesta.Resultado = Json("", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    oMensajeRespuesta.Mensaje = "";
                    oMensajeRespuesta.Resultado = Json(oPersona, JsonRequestBehavior.AllowGet);
                }

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                oMensajeRespuesta.Codigo = "-1";
                oMensajeRespuesta.Mensaje = "Ocurrio un error al consultar el documento. Contacte al administrador del sistema";
                oMensajeRespuesta.Resultado = Json("", JsonRequestBehavior.AllowGet);

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }
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
