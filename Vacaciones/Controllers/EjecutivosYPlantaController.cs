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
        List<EmpleadoModels> oLstEmpleados = new List<EmpleadoModels>();

        // GET: EjecutivosYPlanta
        public ActionResult Index(String Valores)
        {
            Persona ols = new Persona();
            ols = JsonConvert.DeserializeObject<Persona>(Valores);

            ViewBag.NombreEmpleado = ols.NombrePersona;
            ViewBag.NumeroDias = ols.NumeroDias;
            ViewBag.Documento = ols.Documento;
            return View();
        }

        public JsonResult ValidarCantidadDias(int NumeroDias, float NumDiasDisponibles)
        {
            if (NumeroDias < 6)
            {
                oMensajeRespuesta = new MensajeRespuesta
                {
                    Codigo = "1",
                    Mensaje = "La cantidad de días debe ser superior a 6.",
                    Resultado = Json("", JsonRequestBehavior.AllowGet)
                };
            }

            if (NumeroDias > NumDiasDisponibles)
            {
                oMensajeRespuesta = new MensajeRespuesta
                {
                    Codigo = "1",
                    Mensaje = "La cantidad de días debe ser menor o igual al número de días disponibles.",
                    Resultado = Json("", JsonRequestBehavior.AllowGet)
                };
            }

            return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
        }


        public JsonResult AgregarEmpleado(string Cedula, string NumeroDias, string FechaInicio, string FechaFin)
        {
            oMensajeRespuesta = new MensajeRespuesta
            {
                Codigo = "1",
                Mensaje = "Su solicitud ha sido enviada exitosamente.",
                Resultado = Json("", JsonRequestBehavior.AllowGet)
            };

            return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);

        }



    }
}
