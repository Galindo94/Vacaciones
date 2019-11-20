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
            RespuestaSAPModels oRespuestaSAPModels = new RespuestaSAPModels();

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
                oMensajeRespuestaDA = JsonConvert.DeserializeObject<MensajeRespuesta>(JsonConvert.SerializeObject(ConsultarUserDA(userName).Data));

                if (oMensajeRespuestaDA.Codigo == "0")
                {
                    MensajeRespuesta oMensajeRespuestaSAP = new MensajeRespuesta();
                    oMensajeRespuestaSAP = JsonConvert.DeserializeObject<MensajeRespuesta>(JsonConvert.SerializeObject(ConsultarUserSAP(oMensajeRespuestaDA.Resultado.Data.ToString()).Data));

                    if (oMensajeRespuestaSAP.Codigo == "4")
                    {
                        oRespuestaSAPModels = JsonConvert.DeserializeObject<RespuestaSAPModels>(oMensajeRespuestaSAP.Resultado.Data.ToString());

                        string oCorreoCorporativoSAP = oRespuestaSAPModels.Details[0].CorreoCorp;
                        string oCorreoPersonalSAP = oRespuestaSAPModels.Details[0].CorreoPersonal;




                        if (!string.IsNullOrEmpty(oCorreoCorporativoSAP) &&
                            (oCorreoCorporativoSAP == oCorreoDecodificado || oCorreoCorporativoSAP == oCorreoPersonalSAP))
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

        public JsonResult ConsultarUserDA(string NombreUsuario)
        {
            PersonaModels oPersona = new PersonaModels();
            ConsumoDA oConsumoDA = new ConsumoDA();
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();

            try
            {
                //Se guarda respuesta del API del DA
                oMensajeRespuesta = oConsumoDA.ConsultarUserDA(NombreUsuario);
                //Se retornan los valores
                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);

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

                oMensajeRespuesta.Codigo = "3";
                oMensajeRespuesta.Mensaje = "Ocurrió un error inesperado en la consulta de la información. Contacte al administrador del sistema";
                oMensajeRespuesta.Resultado = Json(JsonConvert.SerializeObject(oMensajeRespuesta, Formatting.Indented), JsonRequestBehavior.AllowGet);

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);

            }
        }

        public JsonResult ConsultarUserSAPDocumento(int NroDocumento, string oIdDecodificado, string oCorreoDecodificado)
        {

            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
            ConsumoAPISAP oConsumoAPISAP = new ConsumoAPISAP();
            RespuestaSAPModels oRespuestaSAPModels = new RespuestaSAPModels();
            ConsumoAPIAprobacion cons = new ConsumoAPIAprobacion();
            try
            {
                oMensajeRespuesta = oConsumoAPISAP.ConsultarUserSAP(NroDocumento);

                if (oMensajeRespuesta.Codigo == "4")
                {

                    string IdDecodificado = StringCipher.Decrypt(oIdDecodificado);
                    string CorreoDecodificado = StringCipher.Decrypt(oCorreoDecodificado);

                    oRespuestaSAPModels = JsonConvert.DeserializeObject<RespuestaSAPModels>(oMensajeRespuesta.Resultado.Data.ToString());

                    string oCorreoCorporativoSAP = oRespuestaSAPModels.Details[0].CorreoCorp;
                    string oCorreoPersonalSAP = oRespuestaSAPModels.Details[0].CorreoPersonal;


                    if (!string.IsNullOrEmpty(oCorreoCorporativoSAP) &&
                        (oCorreoCorporativoSAP == oCorreoDecodificado || oCorreoCorporativoSAP == oCorreoPersonalSAP))
                    {
                        oMensajeRespuesta = cons.ConsultarAprobacionRechazo(int.Parse(IdDecodificado), CorreoDecodificado);
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

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);

            }
            catch (Exception Ex)
            {
                Logger.Error("Ocurrió un error interno en el consumo del API de SAP con el " +
                                             "Nro. Documento: " + NroDocumento +
                                             "Exception: " + Ex);

                oMensajeRespuesta.Codigo = "-3";
                oMensajeRespuesta.Mensaje = "Ocurrió un error inesperado en la consulta de la información. Contacte al administrador del sistema";
                oMensajeRespuesta.Resultado = Json(JsonConvert.SerializeObject(oMensajeRespuesta, Formatting.Indented), JsonRequestBehavior.AllowGet);

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult EnviarCambioEstado(int Id, int estado, int csctvo_slctd, string crreo_jfe_slctnte, DateTime fcha_inco_vccns, DateTime fcha_fn_vcc, string nmbre_cmplto, int fk_slctd_encbzdo, string crreo_slctnte, string crro_antdr)
        {
            ConsumoAPIAprobacion cons = new ConsumoAPIAprobacion();
            ResultadoCambioEstado oMensajeRespuesta = new ResultadoCambioEstado();
            oMensajeRespuesta = cons.CambiarEstadoSolicitud(Id, estado);

            MensajeRespuesta DataGrid = new MensajeRespuesta();
            DataGrid = cons.ConsultarAprobacionRechazo(csctvo_slctd, crreo_jfe_slctnte);
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