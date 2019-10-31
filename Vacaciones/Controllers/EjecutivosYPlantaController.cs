using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Vacaciones.Models.ModelosGenerales;
using Vacaciones.Utilities;

namespace Vacaciones.Controllers
{
    public class EjecutivosYPlantaController : Controller
    {

        MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
        List<EmpleadoModels> oLstEmpleados = new List<EmpleadoModels>();

        // GET: EjecutivosYPlanta
        public ActionResult Index(string oDatosPersona, string oDatosFormulario, string oDatosSAP)
        {
            PersonaModels oPersona = new PersonaModels();
            oPersona = JsonConvert.DeserializeObject<PersonaModels>(oDatosPersona);

            ViewBag.NombreEmpleado = oPersona.Nombres + oPersona.Apellidos;
            ViewBag.NumeroDias = oPersona.NumeroDias;
            ViewBag.Documento = oPersona.Identificacion;


            // Se obtienen las fechas de los festivos, sabados y domingos (Si se envía true incluira los sábados, si se envía false no incluirá los sábados, según criterio)
            string DiasFestivosSabadosDomingos = FestivosColombia.DiasFestivoSabadosDomingosConcatenado(DateTime.Now.Year, true);
            ViewBag.DiasFestivosSabadosDomingos = DiasFestivosSabadosDomingos;

            // Fecha donde el usuario puede iniciar sus vacaciones (Se le agregan los días según condicion, 1 o 15 días)
            ViewBag.InicioFecha = DateTime.Now.AddDays(15);


            // Fecha maxima que se le mostrará al usuario para pedir sus vacaciones (Esta para 60 días se puede cambiar según criterio)
            ViewBag.FinFecha = DateTime.Now.AddDays(60);
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
