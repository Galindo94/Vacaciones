using log4net;
using Newtonsoft.Json;
using System;
using System.Web.Mvc;
using Vacaciones.Models.ModelosGenerales;
using Vacaciones.Models.ModelosRespuestaSAP;
using Vacaciones.Utilities;
using Vacaciones.Utilities.IntegracionesServicios;

namespace Vacaciones.Controllers
{
    public class VacacionesIntranetController : Controller
    {
        // Variable para almacenar los Log's
        private static readonly ILog Logger = LogManager.GetLogger(Environment.MachineName);
        PersonaModels oPersonaCodigo = new PersonaModels();
        RespuestaSAPModels oRespuestaSap = new RespuestaSAPModels();
        ConsumoDA oConsumoDA = new ConsumoDA();
        ConsumoAPISAP oConsumoAPISAP = new ConsumoAPISAP();

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
            try
            {
                //Se guarda respuesta del API del DA
                oMensajeRespuesta = oConsumoDA.ConsultarUserDA("popcano");
                //Se retornan los valores
                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);

            }
            catch (Exception Ex)
            {
                Logger.Error("Se presento un error consultando al usuario" + NombreUsuario + ". " + Ex);

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
            try
            {
                oMensajeRespuesta = oConsumoAPISAP.ConsultarUserSAP(JsonConvert.DeserializeObject<PersonaModels>(UserDA).Identificacion);
                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                Logger.Error("Se presento un error consultando al usuario" + UserDA + ". " + Ex);

                oRespuestaSap.Exeption[0].ID = "-3";
                oRespuestaSap.Exeption[0].MESSAGE = "Ocurrió un error inesperado en la consulta de la información.Contacte al administrador del sistema.";

                oMensajeRespuesta.Codigo = oRespuestaSap.Exeption[0].ID = "-3";
                oMensajeRespuesta.Mensaje = oRespuestaSap.Exeption[0].MESSAGE;
                oMensajeRespuesta.Resultado = Json(JsonConvert.SerializeObject(oMensajeRespuesta, Formatting.Indented), JsonRequestBehavior.AllowGet);

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }
        }



    }
}
