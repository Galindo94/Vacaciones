using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Vacaciones.Models.ModelosMotorDeReglas;
using Vacaciones.Models.ModelosRespuestaSAP;
using Vacaciones.Utilities;
using Vacaciones.Utilities.IntegracionesServicios;

namespace Vacaciones.Controllers
{
    public class HomeController : Controller
    {
        // Variable para almacenar los Log's
        private static readonly ILog Logger = LogManager.GetLogger(Environment.MachineName);

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ConsultarUserSAP(int NroDocumento)
        {
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
            ConsumoAPISAP oConsumoAPISAP = new ConsumoAPISAP();
            try
            {
                oMensajeRespuesta = oConsumoAPISAP.ConsultarUserSAP(NroDocumento);

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);

            }
            catch (Exception Ex)
            {
                Logger.Error("Ocurrió un error interno en el consumo del API de SAP con el " +
                                             "Nro. Documento: " + NroDocumento +
                                             "Exception: " + Ex);

                oMensajeRespuesta.Codigo = "3";
                oMensajeRespuesta.Mensaje = "Ocurrió un error inesperado en la consulta de la información. Contacte al administrador del sistema.";
                oMensajeRespuesta.Resultado = Json(JsonConvert.SerializeObject(oMensajeRespuesta, Formatting.Indented), JsonRequestBehavior.AllowGet);

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ConsultaMotorDeReglas(string RespuestaSAP)
        {
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
            ConsumoAPIMotorDeReglas oConsumoAPIMotorDeReglas = new ConsumoAPIMotorDeReglas();
            RespuestaSAPModels oRespuestaSap = new RespuestaSAPModels();
            RespuestaMotorModels oRespuestaMotor = new RespuestaMotorModels
            {
                Escenario = new List<EscenarioModels>(),
                Reglas = new List<ReglaModels>(),
                Error = new ErrorModels()
            };

            try
            {
                oRespuestaSap = JsonConvert.DeserializeObject<RespuestaSAPModels>(RespuestaSAP);
                oMensajeRespuesta = oConsumoAPIMotorDeReglas.ConsultarEscenarioYReglas(oRespuestaSap.Details[0].Clasificacion, oRespuestaSap.Details[0].IdGestor);
                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {

                Logger.Error("Ocurrió un error interno en el consumo del API del motor de reglas. " +
                            "Exception: " + Ex);

                oMensajeRespuesta.Codigo = "-3";
                oMensajeRespuesta.Mensaje = "Ocurrió un error inesperado en la consulta de la información. Contacte al administrador del sistema.";
                oMensajeRespuesta.Resultado = Json(JsonConvert.SerializeObject(oMensajeRespuesta, Formatting.Indented), JsonRequestBehavior.AllowGet);

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
