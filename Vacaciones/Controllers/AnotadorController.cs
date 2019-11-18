using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Vacaciones.Models.ModelosFlow;
using Vacaciones.Models.ModelosGenerales;
using Vacaciones.Models.ModelosGuardarSolicitud;
using Vacaciones.Models.ModelosMotorDeReglas;
using Vacaciones.Models.ModelosRespuestaSAP;
using Vacaciones.Utilities;
using Vacaciones.Utilities.IntegracionesServicios;
using Vacaciones.Utilities.UtilitiesGenerales;

namespace Vacaciones.Controllers
{
    public class AnotadorController : Controller
    {

        private static readonly ILog Logger = LogManager.GetLogger(Environment.MachineName);
        readonly string URIAprobacion = WebConfigurationManager.AppSettings["URIAprobacion"].ToString();
        readonly string IdSolicitud = WebConfigurationManager.AppSettings["IdSolicitud"].ToString();
        readonly string CorreoJefe = WebConfigurationManager.AppSettings["CorreoJefe"].ToString();
        MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
        RespuestaSAPModels oRespuestaSAPModels = new RespuestaSAPModels();

        // GET: Anotador
        public ActionResult Index(string oDatosFormulario, string oDatosSAP)
        {
            try
            {

                RespuestaMotorModels oRespuestaMotor = new RespuestaMotorModels();
                RespuestaSAPModels oRespuestaSAPModels = new RespuestaSAPModels();
                List<RespuestaSAPModels> oLstRespuestaSAPModels = new List<RespuestaSAPModels>();
                DiasContingente oDiasContingente = new DiasContingente();

                oRespuestaMotor = JsonConvert.DeserializeObject<RespuestaMotorModels>(oDatosFormulario);
                ViewBag.oRespuestaMotor = JsonConvert.SerializeObject(oRespuestaMotor);

                oRespuestaSAPModels = JsonConvert.DeserializeObject<RespuestaSAPModels>(oDatosSAP);
                ViewBag.oRespuestaSAPModels = JsonConvert.SerializeObject(oRespuestaSAPModels);

                oLstRespuestaSAPModels.Add(oRespuestaSAPModels);
                ViewBag.oLstRespuestaSAPModels = JsonConvert.SerializeObject(oLstRespuestaSAPModels);

                ViewBag.NroIdentificacion = oRespuestaSAPModels.Details[0].NroDocumento;

                //Asignacion dle nombre del Empleado
                ViewBag.NombresEmpleado = oRespuestaSAPModels.Details[0].PrimerNombre + " " + oRespuestaSAPModels.Details[0].SegundoNombre + " ";
                ViewBag.ApellidosEmpleado = oRespuestaSAPModels.Details[0].PrimerApellido + " " + oRespuestaSAPModels.Details[0].SegundoApellido;


                foreach (var oReglas in oRespuestaMotor.Reglas)
                {
                    switch (oReglas.Prmtro)
                    {
                        case "NroDias":
                            ViewBag.NumeroDias = oDiasContingente.CalcularDiasContingente(oRespuestaSAPModels.Details[0].Contingentes.Contigente, oReglas).ToString().Replace('.', ',');
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

                    }
                }

                ViewBag.SabadoHabil = oRespuestaSAPModels.Details[0].SabadoHabil;

                // Se obtienen las fechas de los festivos, sabados y domingos (Si se envía true incluira los sábados, si se envía false no incluirá los sábados, según criterio)
                string DiasFestivosSabadosDomingos = FestivosColombia.DiasFestivoSabadosDomingosConcatenado(DateTime.Now.Year, oRespuestaSAPModels.Details[0].SabadoHabil == "NO" ? true : false);
                ViewBag.DiasFestivosSabadosDomingos = DiasFestivosSabadosDomingos;


                return View();
            }
            catch (Exception Ex)
            {
                Logger.Error("Ocurrió un error construyendo el View de Anotador." + "Exception: " + Ex);
                return null;
            }
        }



        public JsonResult AgregarOEditarEmpleado(string NroIdentificacion, string NombresEmpleado, string ApellidosEmpleado,
                                                 string NumeroDias, string NumeroDiasDisponibles, bool EsEdit,
                                                 bool EsModal, string FechaInicio, string FechaFin,
                                                 string DataActual, string oRespuestaSAP, string SabadoHabil,
                                                 string CorreoSolicitante, string CorreoJefeSolicitante, string CodigoEmpleado,
                                                 string Sociedad, string MinimoDias, string InicioFecha,
                                                 string FinFecha, string DiasFestivosSabadosDomingos, string oRespuestaMotor)
        {
            RespuestaSAPModels oRespuestaSAPModels = new RespuestaSAPModels();
            RespuestaMotorModels oRespuestaMotorModels = new RespuestaMotorModels();
            List<SolicitudDetalle> oLstSolicitudDetalle = new List<SolicitudDetalle>();

            try
            {
                oLstSolicitudDetalle = JsonConvert.DeserializeObject<List<SolicitudDetalle>>(DataActual);

                if (!EsEdit)
                {

                    //Se valida si ya la cedula ha sido agregada
                    int Existe = oLstSolicitudDetalle
                        .Where(w => w.nmroDcmnto == NroIdentificacion).Count();

                    if (Existe == 0)
                    {
                        if (!EsModal)
                        {
                            oRespuestaSAPModels = JsonConvert.DeserializeObject<RespuestaSAPModels>(oRespuestaSAP);

                            oRespuestaMotorModels = JsonConvert.DeserializeObject<RespuestaMotorModels>(oRespuestaMotor);
                            double oMinimoDias = 0;
                            DateTime oInicioFecha = new DateTime();
                            DateTime oFinFecha = new DateTime();

                            foreach (var oReglas in oRespuestaMotorModels.Reglas)
                            {
                                switch (oReglas.Prmtro)
                                {
                                    case "NroMinDias":
                                        oMinimoDias = Convert.ToDouble(oReglas.Vlr_Slda);
                                        break;

                                    case "DiasMinCalendario":
                                        oInicioFecha = DateTime.Now.AddDays(Convert.ToDouble(oReglas.Vlr_Slda));
                                        break;

                                    case "DiasMaxCalendario":
                                        oFinFecha = DateTime.Now.AddDays(Convert.ToDouble(oReglas.Vlr_Slda));
                                        break;

                                }
                            }

                            //Aqui se agregan los items desde la pantalla principal
                            if (oRespuestaSAPModels != null && oRespuestaSAPModels.Details.Count > 0)
                            {
                                oLstSolicitudDetalle.Add(new SolicitudDetalle
                                {
                                    nmroDcmnto = NroIdentificacion,
                                    nmbrs_slctnte = HttpUtility.HtmlDecode(NombresEmpleado),
                                    apllds_slctnte = HttpUtility.HtmlDecode(ApellidosEmpleado),
                                    nmbre_cmplto = HttpUtility.HtmlDecode(NombresEmpleado) + " " + HttpUtility.HtmlDecode(ApellidosEmpleado),
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
                                    nmro_ds_dspnbls = double.Parse(NumeroDiasDisponibles),
                                    MinimoDias = oMinimoDias,
                                    InicioFecha = oInicioFecha,
                                    FinFecha = oFinFecha,
                                    DiasFestivosSabadosDomingos = DiasFestivosSabadosDomingos
                                });

                                oMensajeRespuesta = new MensajeRespuesta
                                {
                                    Codigo = "1",
                                    Mensaje = "Empleado agregado correctamente a la lista",
                                    Resultado = Json(oLstSolicitudDetalle, JsonRequestBehavior.AllowGet)

                                };

                            }
                            else
                            {
                                oMensajeRespuesta = new MensajeRespuesta
                                {
                                    Codigo = "2",
                                    Mensaje = "No fue posible adicionar el empleado a la lista. Contacte al administrador del sistema",
                                    Resultado = Json(oLstSolicitudDetalle, JsonRequestBehavior.AllowGet)
                                };

                                Logger.Error("No fue posible deserializar el Objeto de la respuesta de SAP " +
                                            "Nro. Documento: " + NroIdentificacion +
                                            "Era Modal Anotadores: " + EsModal);
                            }
                        }
                        else
                        {
                            //Aqui se agrega desde la pantalla modal
                            oLstSolicitudDetalle.Add(new SolicitudDetalle
                            {
                                nmroDcmnto = NroIdentificacion,
                                nmbrs_slctnte = HttpUtility.HtmlDecode(NombresEmpleado),
                                apllds_slctnte = HttpUtility.HtmlDecode(ApellidosEmpleado),
                                nmbre_cmplto = HttpUtility.HtmlDecode(NombresEmpleado) + " " + HttpUtility.HtmlDecode(ApellidosEmpleado),
                                fcha_inco_vccns = Convert.ToDateTime(FechaInicio),
                                fcha_fn_vcc = Convert.ToDateTime(FechaFin),
                                nmro_ds = int.Parse(NumeroDias),
                                sbdo_hbl = SabadoHabil == "NO" ? false : true,
                                fcha_hra_aprvc = DateTime.Now,
                                fcha_hra_rgstro_nvdd = DateTime.Now,
                                crreo_slctnte = CorreoSolicitante,
                                crreo_jfe_slctnte = CorreoJefeSolicitante,
                                codEmpldo = CodigoEmpleado,
                                idEstdoSlctd = 1,
                                scdd = Sociedad,
                                nmro_ds_dspnbls = double.Parse(NumeroDiasDisponibles),
                                MinimoDias = double.Parse(MinimoDias),
                                InicioFecha = Convert.ToDateTime(InicioFecha),
                                FinFecha = Convert.ToDateTime(FinFecha),
                                DiasFestivosSabadosDomingos = DiasFestivosSabadosDomingos
                            });

                            oMensajeRespuesta = new MensajeRespuesta
                            {
                                Codigo = "1",
                                Mensaje = "Empleado agregado correctamente a la lista",
                                Resultado = Json(oLstSolicitudDetalle, JsonRequestBehavior.AllowGet)

                            };
                        }
                    }
                    else
                    {
                        oMensajeRespuesta = new MensajeRespuesta
                        {
                            Codigo = "3",
                            Mensaje = "El empleado ya se encuentra agregado en la lista. Verifique la información e inténtelo de nuevo",
                            Resultado = Json(oLstSolicitudDetalle, JsonRequestBehavior.AllowGet)
                        };

                    }

                }
                else
                {
                    //Aqui se hacen las ediciones
                    SolicitudDetalle oSolicitudDetalle = new SolicitudDetalle();

                    if (oLstSolicitudDetalle != null && oLstSolicitudDetalle.Count > 0)
                    {
                        foreach (var item in oLstSolicitudDetalle)
                        {
                            if (item.nmroDcmnto == NroIdentificacion)
                            {
                                item.nmro_ds = int.Parse(NumeroDias);
                                item.fcha_inco_vccns = Convert.ToDateTime(FechaInicio);
                                item.fcha_fn_vcc = Convert.ToDateTime(FechaFin);
                                break;
                            }
                        }
                    }

                    oMensajeRespuesta = new MensajeRespuesta
                    {
                        Codigo = "1",
                        Mensaje = "Empleado actualizado correctamente",
                        Resultado = Json(oLstSolicitudDetalle, JsonRequestBehavior.AllowGet)

                    };


                }

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);

            }
            catch (Exception Ex)
            {
                oMensajeRespuesta = new MensajeRespuesta
                {
                    Codigo = "-1",
                    Mensaje = "Ocurrió un error. Por favor contacte al administrador del sistema",
                    Resultado = Json(oLstSolicitudDetalle, JsonRequestBehavior.AllowGet)

                };

                Logger.Error("Ocurrió un error interno agregando o editando el empleado en la pantalla de anotador. " +
                                                        "Nro. Documento: " + NroIdentificacion +
                                                        "Exception: " + Ex);

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }

        }


        public JsonResult ValidarCantidadDias(int NumeroDias, float NumDiasDisponibles, int MinimoDias)
        {
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();

            if (NumeroDias > 0)
            {
                if (NumeroDias < MinimoDias)
                {
                    oMensajeRespuesta = new MensajeRespuesta
                    {
                        Codigo = "1",
                        Mensaje = "La cantidad de días debe ser minimo  " + MinimoDias,
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
            }
            else
            {
                oMensajeRespuesta = new MensajeRespuesta
                {
                    Codigo = "3",
                    Mensaje = "La cantidad de días debe ser mayor a 0",
                    Resultado = Json("", JsonRequestBehavior.AllowGet)
                };
            }

            return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
        }



        public JsonResult CalcularFechaFin(int NumeroDias, string FechaInicio, string DiasFestivosSabadosDomingos)
        {
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
            try
            {
                DateTime FechaFin = Convert.ToDateTime(FechaInicio).AddDays(NumeroDias);

                FechaFin = CalcularFechaFinHabil(Convert.ToDateTime(FechaInicio), FechaFin, NumeroDias, DiasFestivosSabadosDomingos);

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

        public DateTime CalcularFechaFinHabil(DateTime FechaInicio, DateTime FechaFin, int NumeroDias, string DiasFestivosSabadosDomingos)
        {
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();

            try
            {
                TimeSpan tSpan = new TimeSpan();
                int contador = 0;

                string[] Fechas;
                Fechas = DiasFestivosSabadosDomingos.Split(',');

                foreach (var item in Fechas)
                {
                    string[] DatosFechaItem = item.Split('/');

                    var FechaItem = new DateTime(Convert.ToInt32(DatosFechaItem[2]), Convert.ToInt32(DatosFechaItem[0]), Convert.ToInt32(DatosFechaItem[1])).ToShortDateString();

                    if (Convert.ToDateTime(FechaItem) == FechaFin)
                    {
                        FechaFin = FechaFin.AddDays(1);
                        tSpan = FechaFin - FechaInicio;
                    }

                    if (Convert.ToDateTime(FechaItem) >= Convert.ToDateTime(FechaInicio) && Convert.ToDateTime(FechaItem) <= FechaFin)
                        contador++;

                }

                TimeSpan oCalculo = FechaFin - FechaInicio;
                int Resultado = oCalculo.Days - contador;

                if (Resultado < NumeroDias - 1)
                {
                    FechaFin = FechaFin.AddDays(1);
                    FechaFin = CalcularFechaFinHabil(FechaInicio, FechaFin, NumeroDias, DiasFestivosSabadosDomingos);

                }

                return FechaFin;

            }
            catch (Exception Ex)
            {
                Logger.Error("Ocurrió un error calculando la fecha de fin. Fecha de inicio: " +
                  FechaInicio + ". Número de días: " + NumeroDias +
                  ". Exception: " + Ex);

                oMensajeRespuesta.Codigo = "-1";
                oMensajeRespuesta.Mensaje = "Ocurrió un error inesperado. Consulte al administrador del sistema";
                oMensajeRespuesta.Resultado = Json("", JsonRequestBehavior.AllowGet);

                return DateTime.Now;
            }

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
                oMensajeRespuesta.Mensaje = "Ocurrió un error inesperado en la consulta de la información. Contacte al administrador del sistema";
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
                oMensajeRespuesta = oConsumoAPIMotorDeReglas.ConsultarEscenarioYReglas(oRespuestaSap.Details[0].Clasificacion, oRespuestaSap.Details[0].IdGestor, oRespuestaSap.Details[0].DesCargo);
                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {

                Logger.Error("Ocurrió un error interno en el consumo del API del motor de reglas. " +
                            "Exception: " + Ex);

                oMensajeRespuesta.Codigo = "-3";
                oMensajeRespuesta.Mensaje = "Ocurrió un error inesperado en la consulta de la información. Contacte al administrador del sistema";
                oMensajeRespuesta.Resultado = Json(JsonConvert.SerializeObject(oMensajeRespuesta, Formatting.Indented), JsonRequestBehavior.AllowGet);

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult ArmarObjetoPantallaModal(string RespuestaMotor, string RespuestaSAP)
        {
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
            RespuestaSAPModels oRespuestaSap = new RespuestaSAPModels();
            RespuestaMotorModels oRespuestaMotor = new RespuestaMotorModels
            {
                Escenario = new List<EscenarioModels>(),
                Reglas = new List<ReglaModels>(),
                Error = new ErrorModels()
            };

            DiasContingente oDiasContingente = new DiasContingente();
            ModalAnotadoresModels oModalAnotadoresModels = new ModalAnotadoresModels();

            try
            {
                oRespuestaMotor = JsonConvert.DeserializeObject<RespuestaMotorModels>(RespuestaMotor);
                oRespuestaSap = JsonConvert.DeserializeObject<RespuestaSAPModels>(RespuestaSAP);


                oModalAnotadoresModels.NombreEmpleado = HttpUtility.HtmlDecode(oRespuestaSap.Details[0].PrimerNombre) + " " +
                                                         HttpUtility.HtmlDecode(oRespuestaSap.Details[0].SegundoNombre) + " ";

                oModalAnotadoresModels.ApellidoEmpleado = HttpUtility.HtmlDecode(oRespuestaSap.Details[0].PrimerApellido) + " " +
                                                         HttpUtility.HtmlDecode(oRespuestaSap.Details[0].SegundoApellido);



                foreach (var oReglas in oRespuestaMotor.Reglas)
                {
                    switch (oReglas.Prmtro)
                    {
                        case "NroDias":
                            oModalAnotadoresModels.NroDias = oDiasContingente.CalcularDiasContingente(oRespuestaSap.Details[0].Contingentes.Contigente, oReglas).ToString().Replace('.', ',');  // Pendiente por realizar ////////////////////////
                            break;
                        case "NroMinDias":
                            oModalAnotadoresModels.MinimoDias = Convert.ToDouble(oReglas.Vlr_Slda);
                            break;

                        case "DiasMinCalendario":
                            oModalAnotadoresModels.InicioFecha = DateTime.Now.AddDays(Convert.ToDouble(oReglas.Vlr_Slda));
                            break;

                        case "DiasMaxCalendario":
                            oModalAnotadoresModels.FinFecha = DateTime.Now.AddDays(Convert.ToDouble(oReglas.Vlr_Slda));
                            break;

                    }
                }

                oModalAnotadoresModels.SabadoHabil = oRespuestaSap.Details[0].SabadoHabil;

                // Se obtienen las fechas de los festivos, sabados y domingos (Si se envía true incluira los sábados, si se envía false no incluirá los sábados, según criterio)
                string DiasFestivosSabadosDomingos = FestivosColombia.DiasFestivoSabadosDomingosConcatenado(DateTime.Now.Year, oModalAnotadoresModels.SabadoHabil == "NO" ? true : false);
                oModalAnotadoresModels.DiasFestivosSabadosDomingos = DiasFestivosSabadosDomingos;

                oModalAnotadoresModels.CorreoSolicitante = !string.IsNullOrEmpty(oRespuestaSap.Details[0].CorreoCorp) ? oRespuestaSap.Details[0].CorreoCorp : oRespuestaSap.Details[0].CorreoPersonal;
                oModalAnotadoresModels.CorreoJefeSolicitante = !string.IsNullOrEmpty(oRespuestaSap.Details[0].CorreoCorpJefe) ? oRespuestaSap.Details[0].CorreoCorpJefe : oRespuestaSap.Details[0].CorreoPersonalJefe;
                oModalAnotadoresModels.CodigoEmpleado = oRespuestaSap.Details[0].NroPersonal;
                oModalAnotadoresModels.Sociedad = oRespuestaSap.Details[0].Sociedad;

                oMensajeRespuesta.Codigo = "1";
                oMensajeRespuesta.Resultado = Json(oModalAnotadoresModels, JsonRequestBehavior.AllowGet);

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {

                Logger.Error("Ocurrió un error interno en el consumo del API del motor de reglas. " +
                            "Exception: " + Ex);

                oMensajeRespuesta.Codigo = "-3";
                oMensajeRespuesta.Mensaje = "Ocurrió un error inesperado en la consulta de la información. Contacte al administrador del sistema";
                oMensajeRespuesta.Resultado = Json(JsonConvert.SerializeObject(oMensajeRespuesta, Formatting.Indented), JsonRequestBehavior.AllowGet);

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }
        }

        public List<SolicitudDetalle> GenerarObjetoSolicitudDetalle(string oDataActual)
        {
            List<SolicitudDetalle> oLstSolicitudDetalle = new List<SolicitudDetalle>();
            List<SolicitudDetalle> oLstSolicitudesGrid = new List<SolicitudDetalle>();

            try
            {
                if (!string.IsNullOrEmpty(oDataActual))
                {
                    oLstSolicitudesGrid = JsonConvert.DeserializeObject<List<SolicitudDetalle>>(oDataActual);

                    if (oLstSolicitudesGrid != null && oLstSolicitudesGrid.Count > 0)
                    {
                        foreach (var item in oLstSolicitudesGrid)
                        {
                            oLstSolicitudDetalle.Add(new SolicitudDetalle
                            {
                                nmbrs_slctnte = HttpUtility.HtmlDecode(item.nmbrs_slctnte),
                                apllds_slctnte = HttpUtility.HtmlDecode(item.apllds_slctnte),
                                fcha_inco_vccns = item.fcha_inco_vccns,
                                fcha_fn_vcc = item.fcha_fn_vcc,
                                nmro_ds = item.nmro_ds,
                                sbdo_hbl = item.sbdo_hbl,
                                fcha_hra_aprvc = DateTime.Now,
                                fcha_hra_rgstro_nvdd = DateTime.Now,
                                crreo_slctnte = item.crreo_slctnte,
                                crreo_jfe_slctnte = item.crreo_jfe_slctnte,
                                codEmpldo = item.codEmpldo,
                                idEstdoSlctd = 1,
                                scdd = item.scdd,
                                idntfccn_slctnte = item.nmroDcmnto
                            });
                        }

                    }
                }

                return oLstSolicitudDetalle;
            }
            catch (Exception Ex)
            {
                Logger.Error("Ocurrió un error generando el Objeto solicitud detalle en la clase: AnotadorController. Método GenerarObjetoSolicitudDetalle" +
                           ". Exception: " + Ex);

                return oLstSolicitudDetalle;
            }
        }

        public JsonResult GuardarSolicitud(string NroIdentificacionAnotador, string NombresEmpleadoAnotador, string ApellidosEmpleadoAnotador,
                                           string oRespuestaMotor, string oDataActual)
        {
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
            ConsumoAPIGuardarSolicitud oConsumoAPIGuardarSolicitud = new ConsumoAPIGuardarSolicitud();
            RespuestaMotorModels oRespuestaMotorModels = new RespuestaMotorModels();
            Solicitudes oSolicitudes = new Solicitudes();
            string oCorreoAnotador = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(oRespuestaMotor))
                {
                    oRespuestaMotorModels = JsonConvert.DeserializeObject<RespuestaMotorModels>(oRespuestaMotor);

                    oCorreoAnotador = !string.IsNullOrEmpty(oRespuestaSAPModels.Details[0].CorreoCorp) ? oRespuestaSAPModels.Details[0].CorreoCorp : oRespuestaSAPModels.Details[0].CorreoPersonal;

                    oSolicitudes.fcha_hra_slctd = DateTime.Now;
                    oSolicitudes.nmbrs_slctnte = HttpUtility.HtmlDecode(NombresEmpleadoAnotador);
                    oSolicitudes.apllds_slctnte = HttpUtility.HtmlDecode(ApellidosEmpleadoAnotador);
                    oSolicitudes.nmro_idntfccn = NroIdentificacionAnotador;
                    oSolicitudes.cdgo_escenario = oRespuestaMotorModels.Escenario[0].Cdgo;
                    oSolicitudes.detalle = GenerarObjetoSolicitudDetalle(oDataActual);
                    oSolicitudes.crro_antdr = oCorreoAnotador;
                    if (oSolicitudes.detalle != null && oSolicitudes.detalle.Count > 0)
                    {
                        oMensajeRespuesta = oConsumoAPIGuardarSolicitud.AlmacenarSolicitud(oSolicitudes);
                    }
                    else
                    {
                        Logger.Error("Ocurrió un error almacenando la solicitud de vacaciones. Nro Documento Encabezado: " +
                             oSolicitudes.nmro_idntfccn +
                             ". Especificacion: Ocurrió un error generando el Objeto solicitud detalle en la clase AnotadorController método GenerarObjetoSolicitudDetalle" + ". ");

                        oMensajeRespuesta.Codigo = "-1";
                        oMensajeRespuesta.Mensaje = "Ocurrió un error almacenando la solicitud de vacaciones. Contacte al administrador del sistema";
                    }


                }
                else
                {
                    oMensajeRespuesta.Codigo = "-1";
                    oMensajeRespuesta.Mensaje = "Ocurrió un error almacenando la solicitud de vacaciones. Contacte al administrador del sistema";

                    Logger.Error("Ocurrió un error almacenando la solicitud de vacaciones. Nro Documento Encabezado: " +
                             NroIdentificacionAnotador + ". ");
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

        public JsonResult EnviarNotificacionFlow(string oDataActual, string oIdSolicitud, string oRespuestaSAP)
        {
            List<SolicitudDetalle> oLstSolicitudDetalle = new List<SolicitudDetalle>();
            List<string> oLstCorreos = new List<string>();
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
            RespuestaSAPModels oRespuestaSAPModels = new RespuestaSAPModels();
            ConsumoAPIFlow oConsumoApiFlow = new ConsumoAPIFlow();
            FlowModels oFlow = new FlowModels();
            string oTableAnotador = string.Empty;
            string oTableJefes = string.Empty;
            string oCorreoAnotador = string.Empty;
            try
            {
                string URIAprobacionyRechazo = Request.Url.Scheme + //Https
                                               "://" + Request.Url.Authority + //WWW.
                                               Request.ApplicationPath.TrimEnd('/') + "/" + //Base del sitio
                                               URIAprobacion + // AprobacionYRechazo/Index
                                               IdSolicitud + int.Parse(oIdSolicitud) + "&" +
                                               CorreoJefe;

                oRespuestaSAPModels = JsonConvert.DeserializeObject<RespuestaSAPModels>(oRespuestaSAP);


                oCorreoAnotador = !string.IsNullOrEmpty(oRespuestaSAPModels.Details[0].CorreoCorp) ? oRespuestaSAPModels.Details[0].CorreoCorp : oRespuestaSAPModels.Details[0].CorreoPersonal;

                //Se declara inicio de la tabla para el correo de los anotadores
                oTableAnotador = "<Table cellpadding=0 cellspacing=0 border=1>";


                oLstSolicitudDetalle = GenerarObjetoSolicitudDetalle(oDataActual);

                if (oLstSolicitudDetalle != null && oLstSolicitudDetalle.Count > 0)
                {
                    //Se crean los encabezados para la tabla del anotador
                    oTableAnotador += "<tr>" +
                                "<th> Nro. de identificación </th>" +
                                "<th> Nombres y apellidos </th>" +
                                "<th> Inicio de vacaciones </th>" +
                                "<th> Fin de vacaciones </th>" +
                                "<th> Nro. de días a disfrutar </th> " +
                           "</tr>";

                    foreach (SolicitudDetalle oSolicitudDetalle in oLstSolicitudDetalle)
                    {
                        if (oLstCorreos == null && oLstCorreos.Count == 0)
                            oLstCorreos.Add(oSolicitudDetalle.crreo_jfe_slctnte);
                        else
                        {
                            int Count = oLstCorreos.Count(element => element == oSolicitudDetalle.crreo_jfe_slctnte);
                            if (Count == 0)
                                oLstCorreos.Add(oSolicitudDetalle.crreo_jfe_slctnte);
                        }

                        //Se adiciona cada uno de los empleados a la tabla del anotador
                        oTableAnotador += "<tr>" +
                                        "<td>" + oSolicitudDetalle.idntfccn_slctnte + "</td>" +
                                        "<td>" + oSolicitudDetalle.nmbrs_slctnte + oSolicitudDetalle.apllds_slctnte + "</td>" +
                                        "<td>" + oSolicitudDetalle.fcha_inco_vccns.ToShortDateString() + "</td>" +
                                        "<td>" + oSolicitudDetalle.fcha_fn_vcc.ToShortDateString() + "</td>" +
                                        "<td>" + oSolicitudDetalle.nmro_ds + "</td>" +
                                        "</tr>";
                    }
                }

                //Se cierra la tabla del anotador
                oTableAnotador += "</Table>";

                oFlow = new FlowModels
                {
                    correoAnotador = oCorreoAnotador,
                    lista = oTableAnotador,
                    opt = 6
                };

                oTableAnotador = string.Empty;

                oMensajeRespuesta = new MensajeRespuesta();
                //Aqui se debe enviar notificacion individual
                oMensajeRespuesta = oConsumoApiFlow.EnviarNotificacionFlow(oFlow);

                if (oMensajeRespuesta.Codigo != "1")
                {
                    Logger.Error("Ocurrió un error enviando las notificaciones por correo electrónico para el anotador con correo: " + oCorreoAnotador +
                        ". Id de la solicitud: " + IdSolicitud);

                }


                foreach (var oCorreo in oLstCorreos)
                {
                    oTableJefes = "<Table cellpadding=0 cellspacing=0 border=1>";
                    oTableJefes += "<tr>" +
                                    "<th> Nro. de identificación </th>" +
                                    "<th> Nombres y apellidos </th>" +
                                    "<th> Inicio de vacaciones </th>" +
                                    "<th> Fin de vacaciones </th>" +
                                    "<th> Nro. de días a disfrutar </th> " +
                               "</tr>";

                    foreach (var oDetalle in oLstSolicitudDetalle)
                    {
                        if (oDetalle.crreo_jfe_slctnte == oCorreo)
                        {
                            oTableJefes += "<tr>" +
                                        "<td>" + oDetalle.idntfccn_slctnte + "</td>" +
                                        "<td>" + oDetalle.nmbrs_slctnte + oDetalle.apllds_slctnte + "</td>" +
                                        "<td>" + oDetalle.fcha_inco_vccns.ToShortDateString() + "</td>" +
                                        "<td>" + oDetalle.fcha_fn_vcc.ToShortDateString() + "</td>" +
                                        "<td>" + oDetalle.nmro_ds + "</td>" +
                                        "</tr>";


                            oFlow.correoSolicitante = oDetalle.crreo_slctnte;
                            oFlow.nombreSolicitante = HttpUtility.HtmlDecode(oDetalle.nmbrs_slctnte) + " " + HttpUtility.HtmlDecode(oDetalle.apllds_slctnte);
                            oFlow.fecha_inicio = oDetalle.fcha_inco_vccns.ToShortDateString();
                            oFlow.fecha_fin = oDetalle.fcha_fn_vcc.ToShortDateString();
                            oFlow.opt = 4;

                            oMensajeRespuesta = new MensajeRespuesta();
                            //Aqui se debe enviar notificacion individual
                            oMensajeRespuesta = oConsumoApiFlow.EnviarNotificacionFlow(oFlow);

                            if (oMensajeRespuesta.Codigo != "1")
                            {
                                Logger.Error("Ocurrió un error enviando las notificaciones por correo electrónico para el empleado con código SAP: " +
                                    oDetalle.codEmpldo + ". Nombre Completo: " + oDetalle.nmbrs_slctnte + oDetalle.apllds_slctnte +
                                    ". Id solcicitud: " + IdSolicitud);
                                oMensajeRespuesta = new MensajeRespuesta();
                            }

                        }
                    }

                    oTableJefes += "</Table>";

                    oFlow = new FlowModels
                    {
                        CorreoJefe = oCorreo,
                        lista = oTableJefes,
                        url = "<a href=" + URIAprobacionyRechazo + oCorreo + ">Haga clic aqui </a>",
                        opt = 3
                    };

                    oTableJefes = string.Empty;

                    oMensajeRespuesta = new MensajeRespuesta();
                    //Aqui se debe enviar notificacion individual
                    oMensajeRespuesta = oConsumoApiFlow.EnviarNotificacionFlow(oFlow);

                    if (oMensajeRespuesta.Codigo != "1")
                    {
                        Logger.Error("Ocurrió un error enviando las notificaciones por correo electrónico para el jefe con correo: " + oCorreo +
                            ". Id de la solicitud: " + IdSolicitud);

                    }
                }

                oMensajeRespuesta = new MensajeRespuesta();
                oMensajeRespuesta.Codigo = "1";
                oMensajeRespuesta.Mensaje = "Se genero la lista de correos satisfactoriamente";
                oMensajeRespuesta.Resultado = Json(oLstCorreos, JsonRequestBehavior.AllowGet);

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                Logger.Error("Ocurrió un error enviando las notificaciones por correo electrónico." +
                           ". Exception: " + Ex);

                oMensajeRespuesta.Codigo = "-1";
                oMensajeRespuesta.Mensaje = "Ocurrió un error enviando las notificaciones por correo electrónico. Contacte al administrador del sistema";
                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
