using log4net;
using Newtonsoft.Json;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Web.Mvc;
using Vacaciones.Models.ModelosGenerales;
using Vacaciones.Models.ModelosMotorDeReglas;
using Vacaciones.Models.ModelosRespuestaSAP;
using Vacaciones.Utilities;
using Vacaciones.Utilities.IntegracionesServicios;

namespace Vacaciones.Controllers
{
    public class VacacionesIntranetController : Controller
    {
        // Variable para almacenar los Log's
        private static readonly ILog Logger = LogManager.GetLogger(Environment.MachineName);

        // GET: VacacionesIntranet
        public ActionResult Index()
        {
            ViewBag.UsuarioIntranet = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            return View();
        }

        // GET: VacacionesIntranet/Details/5
        public JsonResult ConsultarUserDA(string NombreUsuario)
        {
            PersonaModels oPersona = new PersonaModels();
            ConsumoDA oConsumoDA = new ConsumoDA();
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();

            try
            {
                //Se guarda respuesta del API del DA
                oMensajeRespuesta = oConsumoDA.ConsultarUserDA("popcano");
                //Se retornan los valores
                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);

            }
            catch (Exception Ex)
            {
                Logger.Error("Ocurrió un error consultando la información del Directorio Activo " +
                               "Nombre del usuario: " + NombreUsuario +
                               ". Exception " + Ex);

                oPersona.Codigo = -3;
                oPersona.Respuesta = "Ocurrió un error en el API del Directorio Activo.";


                oMensajeRespuesta.Codigo = oPersona.Codigo.ToString();
                oMensajeRespuesta.Mensaje = oPersona.Respuesta;
                oMensajeRespuesta.Resultado = Json(JsonConvert.SerializeObject(oPersona, Formatting.Indented), JsonRequestBehavior.AllowGet);

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult ConsultarUserSAP(string UserDA)
        {
            ConsumoAPISAP oConsumoAPISAP = new ConsumoAPISAP();
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
            RespuestaSAPModels oRespuestaSAP = new RespuestaSAPModels
            {
                Details = new List<DetailsModels>(),
                Exception = new List<ExceptionModels>()
            };

            try
            {
                oMensajeRespuesta = oConsumoAPISAP.ConsultarUserSAP(JsonConvert.DeserializeObject<PersonaModels>(UserDA).Identificacion);
                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                Logger.Error("Ocurrió un error interno en el consumo del API de SAP con el " +
                             "Nro. Documento: " + JsonConvert.DeserializeObject<PersonaModels>(UserDA).Identificacion +
                             "Exception: " + Ex);

                oRespuestaSAP.Exception[0].ID = "-3";
                oRespuestaSAP.Exception[0].MESSAGE = "Ocurrió un error inesperado en la consulta de la información. Contacte al administrador del sistema.";

                oMensajeRespuesta.Codigo = oRespuestaSAP.Exception[0].ID = "-3";
                oMensajeRespuesta.Mensaje = oRespuestaSAP.Exception[0].MESSAGE;
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

                oRespuestaSap.Exception[0].ID = "-3";
                oRespuestaSap.Exception[0].MESSAGE = "Ocurrió un error inesperado en la consulta de la información. Contacte al administrador del sistema.";

                oMensajeRespuesta.Codigo = oRespuestaSap.Exception[0].ID = "-3";
                oMensajeRespuesta.Mensaje = oRespuestaSap.Exception[0].MESSAGE;
                oMensajeRespuesta.Resultado = Json(JsonConvert.SerializeObject(oMensajeRespuesta, Formatting.Indented), JsonRequestBehavior.AllowGet);

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
