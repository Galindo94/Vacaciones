using log4net;
using Newtonsoft.Json;
using System;
using System.Web.Mvc;
using Vacaciones.Models;
using Vacaciones.Models.ModelosConsumo;
using Vacaciones.Utilities;
using Vacaciones.Utilities.IntegracionesServicios;

namespace Vacaciones.Controllers
{
    public class VacacionesIntranetController : Controller
    {
        // Variable para almacenar los Log's
        private static readonly ILog Logger = LogManager.GetLogger(Environment.MachineName);
        PersonaModels oPersonaCodigo = new PersonaModels();
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
                oMensajeRespuesta = oConsumoDA.ConsultarUserDA(NombreUsuario);

                //Se serializa la respuesta que arrojo el Servicio en formato de Persona
                oPersonaCodigo = JsonConvert.DeserializeObject<PersonaModels>(oMensajeRespuesta.Resultado.Data.ToString());

                //Al mensaje que se va a retornar se le asigna el codigo y el mensaje de la persona
                oMensajeRespuesta.Codigo = oPersonaCodigo.Codigo.ToString();
                oMensajeRespuesta.Mensaje = oPersonaCodigo.Respuesta;

                //Se retornan los valores
                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);

            }
            catch (Exception Ex)
            {
                Logger.Error(Ex);

                PersonaModels oPersonaError = new PersonaModels()
                {
                    Codigo = -3,
                    Respuesta = "Ocurrió un error inesperado en la consulta de la información. Contacte al administrador del sistema."
                };

                oMensajeRespuesta.Codigo = "-3";
                oMensajeRespuesta.Mensaje = "Ocurrió un error inesperado en la consulta de la información. Contacte al administrador del sistema.";
                oMensajeRespuesta.Resultado = Json(JsonConvert.SerializeObject(oPersonaError, Formatting.Indented), JsonRequestBehavior.AllowGet);

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult ConsultarUserSAP(string UserDA)
        {
            try
            {
                oMensajeRespuesta = oConsumoAPISAP.ConsultarUserSAP(JsonConvert.DeserializeObject<PersonaModels>(UserDA).Identificacion);

                oPersonaCodigo = JsonConvert.DeserializeObject<PersonaModels>(oMensajeRespuesta.Resultado.Data.ToString());

                oMensajeRespuesta.Codigo = oPersonaCodigo.Codigo.ToString();
                oMensajeRespuesta.Mensaje = oPersonaCodigo.Respuesta;

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {

                Logger.Error(Ex);

                PersonaModels oPersonaError = new PersonaModels()
                {
                    Codigo = -3,
                    Respuesta = "Ocurrió un error inesperado en la consulta de la información. Contacte al administrador del sistema."
                };

                oMensajeRespuesta.Codigo = "-3";
                oMensajeRespuesta.Mensaje = "Ocurrió un error inesperado en la consulta de la información. Contacte al administrador del sistema.";
                oMensajeRespuesta.Resultado = Json(JsonConvert.SerializeObject(oPersonaError, Formatting.Indented), JsonRequestBehavior.AllowGet);

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }
        }



    }
}
