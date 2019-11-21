using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Vacaciones.Models.ModelosAproRechazo;
using Vacaciones.Models.ModelosFlow;
using Vacaciones.Models.ModelosGenerales;
using Vacaciones.Models.ModelosRespuestaSAP;
using Vacaciones.Utilities;
using Vacaciones.Utilities.IntegracionesServicios;
using Vacaciones.Utilities.UtilitiesGenerales;

namespace Vacaciones.Controllers
{
    public class AprobacionRechazoController : Controller
    {
        private static readonly ILog Logger = LogManager.GetLogger(Environment.MachineName);

        // GET: AprobacionRechazo
        public ActionResult Index(string csctvo_slctd, string crreo_jfe_slctnte)
        {
            ConsumoAPIAprobacion cons = new ConsumoAPIAprobacion();
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
            PersonaModels oPersona = new PersonaModels();

            try
            {
                string NombreUser = System.Web.HttpContext.Current.User.Identity.Name;
                int backSlash = NombreUser.IndexOf("\\");
                string userName = backSlash > 0 ? NombreUser.Substring(backSlash + 1) : NombreUser;

                string oIdDecodificado = StringCipher.Decrypt(csctvo_slctd);
                string oCorreoDecodificado = StringCipher.Decrypt(crreo_jfe_slctnte);

                ViewBag.IdCodificado = csctvo_slctd;
                ViewBag.CorreoCodificado = crreo_jfe_slctnte;

                MensajeRespuesta oMensajeRespuestaDA = new MensajeRespuesta();
                oMensajeRespuestaDA = ConsultarUserDA(userName);

                if (oMensajeRespuestaDA.Codigo == "0")
                {
                    string oPersonaModel = JsonConvert.SerializeObject(oMensajeRespuestaDA.Resultado.Data);
                    oPersona = JsonConvert.DeserializeObject<PersonaModels>(oPersonaModel);
                    
                        if (oPersona.Correo.ToUpper() == oCorreoDecodificado.ToUpper())
                        {
                            oMensajeRespuesta = cons.ConsultarAprobacionRechazo(int.Parse(oIdDecodificado), oCorreoDecodificado);
                            ViewBag.Respuesta = Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet).Data;
                        }
                        else
                        {
                            oMensajeRespuesta.Codigo = "-3";
                            oMensajeRespuesta.Mensaje = "";
                            oMensajeRespuesta.Resultado = new JsonResult();

                            ViewBag.Respuesta = Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet).Data;

                        }

                   
                }
                else
                {
                    oMensajeRespuesta.Codigo = "-3";
                    oMensajeRespuesta.Mensaje = "";
                    oMensajeRespuesta.Resultado = new JsonResult();

                    ViewBag.Respuesta = Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet).Data;

                }


                return View();
            }
            catch (Exception Ex)
            {

                throw;
            }
        }

        public MensajeRespuesta ConsultarUserDA(string NombreUsuario)
        {
            PersonaModels oPersona = new PersonaModels();
            ConsumoDA oConsumoDA = new ConsumoDA();
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();

            try
            {
                //Se guarda respuesta del API del DA
                oMensajeRespuesta = oConsumoDA.ConsultarUserDA(NombreUsuario);
                //Se retornan los valores
                return oMensajeRespuesta;

            }
            catch (Exception Ex)
            {
                Logger.Error("Ocurrió un error consultando la información del Directorio Activo" +
                               "Nombre del usuario: " + NombreUsuario +
                               ". Exception " + Ex);

                oPersona.Codigo = -3;
                oPersona.Respuesta = "Ocurrió un error en el API del directorio activo";


                oMensajeRespuesta.Codigo = oPersona.Codigo.ToString();
                oMensajeRespuesta.Mensaje = oPersona.Respuesta;
                oMensajeRespuesta.Resultado = Json(oPersona, JsonRequestBehavior.AllowGet);

                return oMensajeRespuesta;
            }
        }

       

        public JsonResult EnviarCambioEstado(int Id, int estado, string csctvo_slctd, string crreo_jfe_slctnte, DateTime fcha_inco_vccns, DateTime fcha_fn_vcc, string nmbre_cmplto, int fk_slctd_encbzdo, string crreo_slctnte, string crro_antdr)
        {
            ConsumoAPIAprobacion cons = new ConsumoAPIAprobacion();
            ResultadoCambioEstado oMensajeRespuesta = new ResultadoCambioEstado();
            oMensajeRespuesta = cons.CambiarEstadoSolicitud(Id, estado);

            string oIdDecodificado = StringCipher.Decrypt(csctvo_slctd);
            string oCorreoDecodificado = StringCipher.Decrypt(crreo_jfe_slctnte);

            MensajeRespuesta DataGrid = new MensajeRespuesta();
            DataGrid = cons.ConsultarAprobacionRechazo(int.Parse(oIdDecodificado), oCorreoDecodificado);
            if (oMensajeRespuesta.Codigo == 1 && estado == 3)
            {
                ConsumoAPIFlow consFlow = new ConsumoAPIFlow();
                FlowModels item = new FlowModels();
                item.cnsctvo_slctd = fk_slctd_encbzdo;
                item.CorreoJefe = crreo_jfe_slctnte;
                item.correoSolicitante = crreo_slctnte;
                item.correoAnotador = crro_antdr;
                item.fecha_inicio = fcha_inco_vccns.ToString();
                item.fecha_fin = fcha_fn_vcc.ToString();
                item.opt = 2;
                item.nombreSolicitante = nmbre_cmplto;
                MensajeRespuesta mensajeCorreo = new MensajeRespuesta();
                mensajeCorreo = consFlow.EnviarNotificacionFlow(item);
            }



            DataGrid.Codigo = oMensajeRespuesta.Codigo.ToString();
            DataGrid.Mensaje = oMensajeRespuesta.Respuesta.ToString();

            return Json(DataGrid, JsonRequestBehavior.AllowGet);
        }

    }
}