using log4net;
using Newtonsoft.Json;
using System;
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

        MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();

        // GET: VacacionesIntranet
        public ActionResult Index()
        {
            ViewBag.UsuarioIntranet = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            return View();
        }

        // GET: VacacionesIntranet/Details/5
        public JsonResult ConsultarUserDA(string NombreUsuario)
        {
            PersonaModels oPersonaCodigo = new PersonaModels();
            ConsumoDA oConsumoDA = new ConsumoDA();

            try
            {
                //Se guarda respuesta del API del DA
                oMensajeRespuesta = oConsumoDA.ConsultarUserDA("popcano");
                //Se retornan los valores
                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);

            }
            catch (Exception Ex)
            {
                Logger.Error("Se presento un error consultando al usuario: " + NombreUsuario + ". " + Ex);

                oPersonaCodigo.Codigo = -3;
                oPersonaCodigo.Respuesta = "Ocurrió un error inesperado en la consulta de la información. Contacte al administrador del sistema.";

                oMensajeRespuesta.Codigo = oPersonaCodigo.Codigo.ToString();
                oMensajeRespuesta.Mensaje = oPersonaCodigo.Respuesta;
                oMensajeRespuesta.Resultado = Json(JsonConvert.SerializeObject(oPersonaCodigo, Formatting.Indented), JsonRequestBehavior.AllowGet);

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult ConsultarUserSAP(string UserDA)
        {
            ConsumoAPISAP oConsumoAPISAP = new ConsumoAPISAP();
            RespuestaMotorModels oRespuestaMotor = new RespuestaMotorModels
            {
                Escenario = new List<EscenarioModels>(),
                Reglas = new List<ReglaModels>(),
                Error = new ErrorModels()
            };

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
                Logger.Error("Se presento un error consultando al usuario: " + UserDA + ". " + Ex);

                oRespuestaSAP.Exception[0].ID = "-3";
                oRespuestaSAP.Exception[0].MESSAGE = "Ocurrió un error inesperado en la consulta de la información.Contacte al administrador del sistema.";

                oMensajeRespuesta.Codigo = oRespuestaSAP.Exception[0].ID = "-3";
                oMensajeRespuesta.Mensaje = oRespuestaSAP.Exception[0].MESSAGE;
                oMensajeRespuesta.Resultado = Json(JsonConvert.SerializeObject(oMensajeRespuesta, Formatting.Indented), JsonRequestBehavior.AllowGet);

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ConsultaMotorDeReglas(string RespuestaSAP)
        {
            ConsumoAPIMotorDeReglas oConsumoAPIMotorDeReglas = new ConsumoAPIMotorDeReglas();
            RespuestaSAPModels oRespuestaSap = new RespuestaSAPModels();
            RespuestaMotorModels oRespuestaMotor = new RespuestaMotorModels();

            try
            {
                //oMensajeRespuesta = oConsumoAPIMotorDeReglas.ConsultarExcenarioYReglas(JsonConvert.DeserializeObject<PersonaModels>(RespuestaSAP).Identificacion);
                oMensajeRespuesta = oConsumoAPIMotorDeReglas.ConsultarEscenarioYReglas(RespuestaSAP);
                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                //Logger.Error("Se presento un error consultando al usuario: " + UserDA + ". " + Ex);

                oRespuestaSap.Exception[0].ID = "-3";
                oRespuestaSap.Exception[0].MESSAGE = "Ocurrió un error inesperado en la consulta de la información.Contacte al administrador del sistema.";

                oMensajeRespuesta.Codigo = oRespuestaSap.Exception[0].ID = "-3";
                oMensajeRespuesta.Mensaje = oRespuestaSap.Exception[0].MESSAGE;
                oMensajeRespuesta.Resultado = Json(JsonConvert.SerializeObject(oMensajeRespuesta, Formatting.Indented), JsonRequestBehavior.AllowGet);

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }
        }



    }
}
