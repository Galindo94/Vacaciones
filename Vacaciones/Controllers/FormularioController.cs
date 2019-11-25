using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Helpers;
using System.Web.Mvc;
using Vacaciones.Models.ModelosFechasSolicitud;
using Vacaciones.Models.ModelosFlow;
using Vacaciones.Models.ModelosGuardarSolicitud;
using Vacaciones.Models.ModelosMotorDeReglas;
using Vacaciones.Models.ModelosRespuestaSAP;
using Vacaciones.Utilities;
using Vacaciones.Utilities.IntegracionesServicios;
using Vacaciones.Utilities.UtilitiesGenerales;

namespace Vacaciones.Controllers
{
    public class FormularioController : Controller
    {
        //Variable para almacenar los Log's
        private static readonly ILog Logger = LogManager.GetLogger(Environment.MachineName);
        readonly string URIAprobacion = WebConfigurationManager.AppSettings["URIAprobacion"].ToString();
        readonly string IdSolicitud = WebConfigurationManager.AppSettings["IdSolicitud"].ToString();
        readonly string CorreoJefe = WebConfigurationManager.AppSettings["CorreoJefe"].ToString();
        // GET: EjecutivosYPlanta
        public ActionResult Index(string oDatosFormulario, string oDatosSAP)
        {
            try
            {
                RespuestaMotorModels oRespuestaMotor = new RespuestaMotorModels();
                RespuestaSAPModels oRespuestaSAPModels = new RespuestaSAPModels();
                UtilitiesGenerales oDiasContingente = new UtilitiesGenerales();

                oRespuestaMotor = JsonConvert.DeserializeObject<RespuestaMotorModels>(oDatosFormulario);
                ViewBag.oRespuestaMotor = StringCipher.Encrypt(JsonConvert.SerializeObject(oRespuestaMotor));

                oRespuestaSAPModels = JsonConvert.DeserializeObject<RespuestaSAPModels>(oDatosSAP);
                ViewBag.oRespuestaSAPModels = StringCipher.Encrypt(JsonConvert.SerializeObject(oRespuestaSAPModels));

                ViewBag.NroIdentificacion = oRespuestaSAPModels.Details[0].NroDocumento;

                //Asignacion dle nombre del Empleado
                ViewBag.NombresEmpleado = oRespuestaSAPModels.Details[0].PrimerNombre + " " + oRespuestaSAPModels.Details[0].SegundoNombre + " ";
                ViewBag.ApellidosEmpleado = oRespuestaSAPModels.Details[0].PrimerApellido + " " + oRespuestaSAPModels.Details[0].SegundoApellido;


                foreach (var oReglas in oRespuestaMotor.Reglas)
                {
                    switch (oReglas.Prmtro)
                    {
                        case "NroDias":
                            ViewBag.NumeroDias = oDiasContingente.CalcularDiasContingente(oRespuestaSAPModels.Details[0].Contingentes.Contigente, oReglas).ToString().Replace('.', ',');  // Pendiente por realizar ////////////////////////
                            break;
                        case "NroMinDias":
                            ViewBag.MinimoDias = Convert.ToDouble(oReglas.Vlr_Slda);
                            break;

                        case "DiasMinCalendario":
                            ViewBag.InicioFecha = DateTime.Now.AddDays(Convert.ToDouble(oReglas.Vlr_Slda));
                            break;

                        case "DiasMaxCalendario":
                            ViewBag.FinFecha = DateTime.Now.AddDays(Convert.ToDouble(oReglas.Vlr_Slda));
                            break;

                        case "NroMinDiasCorreoCompensacion":
                            ViewBag.NroMinDiasCorreoCompensacion = int.Parse(oReglas.Vlr_Slda);
                            break;

                        case "CorreoCompensacion":
                            ViewBag.CorreoCompensacion = StringCipher.Encrypt(oReglas.Vlr_Slda);
                            break;

                    }
                }

                // Se obtienen las fechas de los festivos, sabados y domingos (Si se envía true incluira los sábados, si se envía false no incluirá los sábados, según criterio)
                string DiasFestivosSabadosDomingos = FestivosColombia.DiasFestivoSabadosDomingosConcatenado(DateTime.Now.Year, oRespuestaSAPModels.Details[0].SabadoHabil == "NO" ? true : false);
                ViewBag.DiasFestivosSabadosDomingos = DiasFestivosSabadosDomingos;


                return View();
            }
            catch (Exception Ex)
            {
                Logger.Error("Ocurrió un error construyendo el View de Ejecutivos y Planta." + "Exception: " + Ex);
                return null;
            }
        }

        public JsonResult ValidarCantidadDias(int NumeroDias, float NumDiasDisponibles, int MinimoDias)
        {
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();

            if (NumeroDias < MinimoDias)
            {
                oMensajeRespuesta = new MensajeRespuesta
                {
                    Codigo = "1",
                    Mensaje = "La cantidad de días debe ser mínimo " + MinimoDias,
                    Resultado = Json("", JsonRequestBehavior.AllowGet)
                };
            }

            if (NumeroDias > NumDiasDisponibles)
            {
                oMensajeRespuesta = new MensajeRespuesta
                {
                    Codigo = "2",
                    Mensaje = "La cantidad de días debe ser menor o igual al número de días disponibles (" + NumDiasDisponibles + ")",
                    Resultado = Json("", JsonRequestBehavior.AllowGet)
                };
            }

            return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GuardarSolicitud(string NroIdentificacion, string NombresEmpleado, string ApellidosEmpleado,
                                           string oRespuestaSAP, string oRespuestaMotor, string NumeroDias,
                                           string FechaInicio, string FechaFin, string NroMinDiasCorreoCompensacion,
                                           string CorreoCompensacion)
        {
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
            ConsumoAPIGuardarSolicitud oConsumoAPIGuardarSolicitud = new ConsumoAPIGuardarSolicitud();
            RespuestaSAPModels oRespuestaSAPModels = new RespuestaSAPModels();
            RespuestaMotorModels oRespuestaMotorModels = new RespuestaMotorModels();
            List<SolicitudDetalle> oLstSolicitudDetalle = new List<SolicitudDetalle>();
            Solicitudes oSolicitudes = new Solicitudes();
            ConsumoAPIFlow oConsumoApiFlow = new ConsumoAPIFlow();
            RespuestaGuardarSolicitudModels oRespuestaGuardarSolicitudModels = new RespuestaGuardarSolicitudModels();
            UtilitiesGenerales oUtilitiesGenerales = new UtilitiesGenerales();


            try
            {
                string NombreUser = System.Web.HttpContext.Current.User.Identity.Name;
                int backSlash = NombreUser.IndexOf("\\");
                string UserName = backSlash > 0 ? NombreUser.Substring(backSlash + 1) : NombreUser;

                oRespuestaSAPModels = JsonConvert.DeserializeObject<RespuestaSAPModels>(StringCipher.Decrypt(oRespuestaSAP));
                oRespuestaMotorModels = JsonConvert.DeserializeObject<RespuestaMotorModels>(StringCipher.Decrypt(oRespuestaMotor));

                oLstSolicitudDetalle.Add(new SolicitudDetalle
                {
                    nmbrs_slctnte = HttpUtility.HtmlDecode(NombresEmpleado),
                    apllds_slctnte = HttpUtility.HtmlDecode(ApellidosEmpleado),
                    fcha_inco_vccns = Convert.ToDateTime(FechaInicio),
                    fcha_fn_vcc = Convert.ToDateTime(FechaFin),
                    nmro_ds = int.Parse(NumeroDias),
                    sbdo_hbl = oRespuestaSAPModels.Details[0].SabadoHabil == "NO" ? false : true,
                    fcha_hra_aprvc = DateTime.Now,
                    fcha_hra_rgstro_nvdd = DateTime.Now,
                    crreo_slctnte = !string.IsNullOrEmpty(oRespuestaSAPModels.Details[0].CorreoCorp) ? oRespuestaSAPModels.Details[0].CorreoCorp : oRespuestaSAPModels.Details[0].CorreoPersonal,
                    crreo_jfe_slctnte = !string.IsNullOrEmpty(oRespuestaSAPModels.Details[0].CorreoCorpJefe) ? oRespuestaSAPModels.Details[0].CorreoCorpJefe : oRespuestaSAPModels.Details[0].CorreoPersonalJefe,
                    codEmpldo = oRespuestaSAPModels.Details[0].NroPersonal,
                    idEstdoSlctd = 1,
                    scdd = oRespuestaSAPModels.Details[0].Sociedad,
                    idntfccn_slctnte = NroIdentificacion,
                    NroMinDiasCorreoCompensacion = int.Parse(NroMinDiasCorreoCompensacion),
                    CorreoCompensacion = StringCipher.Decrypt(CorreoCompensacion)
                });

                oSolicitudes.fcha_hra_slctd = DateTime.Now;
                oSolicitudes.nmbrs_slctnte = HttpUtility.HtmlDecode(NombresEmpleado);
                oSolicitudes.apllds_slctnte = HttpUtility.HtmlDecode(ApellidosEmpleado);
                oSolicitudes.nmro_idntfccn = NroIdentificacion;
                oSolicitudes.cdgo_escenario = oRespuestaMotorModels.Escenario[0].Cdgo;
                oSolicitudes.detalle = oLstSolicitudDetalle;
                oSolicitudes.crro_antdr = "";
                oSolicitudes.ip = oUtilitiesGenerales.ObtenerIp();
                oSolicitudes.nmbre_usrio = UserName;
                oSolicitudes.nmbre_eqpo = oUtilitiesGenerales.ObtenerNombreMaquina();

                oMensajeRespuesta = oConsumoAPIGuardarSolicitud.AlmacenarSolicitud(oSolicitudes);

                if (oMensajeRespuesta.Codigo == "1")
                {

                    string oRespuestaGuardarSolicitud = JsonConvert.SerializeObject(oMensajeRespuesta.Resultado.Data, Formatting.Indented);

                    oRespuestaGuardarSolicitudModels = JsonConvert.DeserializeObject<RespuestaGuardarSolicitudModels>(oRespuestaGuardarSolicitud);



                    string URIAprobacionyRechazo = Request.Url.Scheme + //Https
                                                 "://" + Request.Url.Authority + //WWW.
                                                 Request.ApplicationPath.TrimEnd('/') + "/" + //Base del sitio
                                                 URIAprobacion + // AprobacionYRechazo/Index
                                                 IdSolicitud + HttpUtility.UrlEncode(StringCipher.Encrypt(oRespuestaGuardarSolicitudModels.Resultado.ToString())) + "&" +
                                                 CorreoJefe + HttpUtility.UrlEncode(StringCipher.Encrypt(oLstSolicitudDetalle[0].crreo_jfe_slctnte));


                    FlowModels oFlow = new FlowModels
                    {
                        correoSolicitante = oLstSolicitudDetalle[0].crreo_slctnte,
                        nombreSolicitante = HttpUtility.HtmlDecode(oLstSolicitudDetalle[0].nmbrs_slctnte) + " " + HttpUtility.HtmlDecode(oLstSolicitudDetalle[0].apllds_slctnte),
                        fecha_fin = oLstSolicitudDetalle[0].fcha_fn_vcc.ToShortDateString(),
                        fecha_inicio = oLstSolicitudDetalle[0].fcha_inco_vccns.ToShortDateString(),
                        CorreoJefe = oLstSolicitudDetalle[0].crreo_jfe_slctnte,
                        url = "<a href=" + URIAprobacionyRechazo + ">Haga clic aqui </a>",
                        opt = 1
                    };

                    oConsumoApiFlow.EnviarNotificacionFlow(oFlow);

                    if (oLstSolicitudDetalle[0].nmro_ds <= int.Parse(NroMinDiasCorreoCompensacion))
                    {
                        FlowModels oFlowMesaCompensacion = new FlowModels
                        {
                            CorreoCompensacion = StringCipher.Decrypt(CorreoCompensacion),
                            nombreSolicitante = HttpUtility.HtmlDecode(oLstSolicitudDetalle[0].nmbrs_slctnte) + " " + HttpUtility.HtmlDecode(oLstSolicitudDetalle[0].apllds_slctnte),
                            fecha_inicio = oLstSolicitudDetalle[0].fcha_inco_vccns.ToShortDateString(),
                            fecha_fin = oLstSolicitudDetalle[0].fcha_fn_vcc.ToShortDateString(),
                            opt = 5
                        };

                        oConsumoApiFlow.EnviarNotificacionFlow(oFlowMesaCompensacion);
                    }


                }


                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);

            }
            catch (Exception Ex)
            {

                Logger.Error("Ocurrió un error almacenando la solicitud de vacaciones. Nro Documento Encabezado: " +
                            oSolicitudes.nmro_idntfccn +
                            ". Exception: " + Ex);

                oMensajeRespuesta.Codigo = "-3";
                oMensajeRespuesta.Mensaje = "Ocurrió un error almacenando la solicitud de vacaciones. Contacte al administrador del sistema";
                oMensajeRespuesta.Resultado = Json(JsonConvert.SerializeObject(oMensajeRespuesta, Formatting.Indented), JsonRequestBehavior.AllowGet);

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult CalcularFechaFin(int NumeroDias, string FechaInicio, string DiasFestivosSabadosDomingos)
        {
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
            UtilitiesGenerales oUtilitiesGenerales = new UtilitiesGenerales();
            try
            {
                DateTime FechaFin = Convert.ToDateTime(FechaInicio).AddDays(NumeroDias - 1);


                FechaFin = oUtilitiesGenerales.CalcularFechaFinHabil(Convert.ToDateTime(FechaInicio), FechaFin, NumeroDias, DiasFestivosSabadosDomingos);

                oMensajeRespuesta.Codigo = "0";
                oMensajeRespuesta.Mensaje = "";
                oMensajeRespuesta.Resultado = Json(FechaFin.ToShortDateString());

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                Logger.Error("Ocurrió un error calculando la fecha de fin. Fecha de inicio: " +
                   FechaInicio + ". Número de días: " + NumeroDias +
                   ". Exception: " + Ex);

                oMensajeRespuesta.Codigo = "-1";
                oMensajeRespuesta.Mensaje = "Ocurrió un error inesperado. Consulte al administrador del sistema";
                oMensajeRespuesta.Resultado = Json(DateTime.Now.ToShortDateString(), JsonRequestBehavior.AllowGet);

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult ConsultaFechasSolicitudExistentes(string oIdentificacion, string FechaInicio, string FechaFin)
        {
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
            ConsumoAPIFechasSolicitud oConsumoAPIFechas = new ConsumoAPIFechasSolicitud();
            RespuestaFechasSolicitudModels oRespuesteFechaSolicitud = new RespuestaFechasSolicitudModels();
            List<DateTime> oRangoFechasSolicitud = new List<DateTime>();
            List<string> oListaFechasBD = new List<string>();
            bool encontrado = false;
            DateTime oFechaInicio = new DateTime();
            DateTime oFechaFin = new DateTime();
            try
            {
                oMensajeRespuesta = oConsumoAPIFechas.ConsultarFechasSolicitud(oIdentificacion);

                if (oMensajeRespuesta.Codigo == "1")
                {
                    oRespuesteFechaSolicitud = JsonConvert.DeserializeObject<RespuestaFechasSolicitudModels>(JsonConvert.SerializeObject(oMensajeRespuesta.Resultado.Data));

                    oListaFechasBD = oRespuesteFechaSolicitud.Fechas;

                    oFechaInicio = Convert.ToDateTime(FechaInicio);
                    oFechaFin = Convert.ToDateTime(FechaFin);

                    for (DateTime date = oFechaInicio; date <= oFechaFin; date = date.AddDays(1))
                        oRangoFechasSolicitud.Add(date);

                    if (oListaFechasBD != null && oListaFechasBD.Count > 0)
                    {
                        foreach (var oFecha in oRangoFechasSolicitud)
                        {

                            var ResultCount = from oItem in oListaFechasBD
                                              where Convert.ToDateTime(oItem) == oFecha
                                              select oItem.Count();

                            if (ResultCount.Count() > 0)
                            {
                                encontrado = true;
                                break;
                            }
                        }
                    }
                    if (encontrado)
                    {
                        oMensajeRespuesta.Codigo = "2";
                        oMensajeRespuesta.Mensaje = "Usted tiene una solicitud de vacaciones aprobada y pendiente de disfrutar en el período indicado";
                    }

                }

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {

                //Se deja registro en el Log del error
                Logger.Error("Ocurrió un error validando que el empelado con Nro documento: " + oIdentificacion +
                    " no tuviese una solicitud aprobada y pendiente de disfrute en el rango de fechas seleccionado.Fecha de inicio: " + FechaInicio +
                    " Fecha de fin: " + FechaFin +
                    ". Exception: " + Ex);

                oMensajeRespuesta.Codigo = "-3";
                oMensajeRespuesta.Mensaje = "Ocurrió un error validando que usted no tenga una solicitud de vacaciones pendiente de disfrute en el rango de fechas seleccionado. Contacte al administrador del sistema";
                oMensajeRespuesta.Resultado = Json("", JsonRequestBehavior.AllowGet);

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);

            }
        }

    }

}
