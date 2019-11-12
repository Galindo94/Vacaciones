using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
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
                            ViewBag.NumeroDias = 30; // oDiasContingente.CalcularDiasContingente(oRespuestaSAPModels.Details[0].Contingentes.Contigente, oReglas).ToString().Replace('.', ',');  // Pendiente por realizar ////////////////////////
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
                                    nmbrs_slctnte = NombresEmpleado,
                                    apllds_slctnte = ApellidosEmpleado,
                                    nmbre_cmplto = NombresEmpleado + " " + ApellidosEmpleado,
                                    fcha_inco_vccns = Convert.ToDateTime(FechaInicio),
                                    fcha_fn_vcc = Convert.ToDateTime(FechaFin),
                                    nmro_ds = double.Parse(NumeroDias),
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
                                    Mensaje = "Empleado agregado correctamente a la lista.",
                                    Resultado = Json(oLstSolicitudDetalle, JsonRequestBehavior.AllowGet)

                                };

                            }
                            else
                            {
                                oMensajeRespuesta = new MensajeRespuesta
                                {
                                    Codigo = "2",
                                    Mensaje = "No fue posible adicionar el empleado a la lista. Contacte al administrador del sistema.",
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
                                nmbrs_slctnte = NombresEmpleado,
                                apllds_slctnte = ApellidosEmpleado,
                                nmbre_cmplto = NombresEmpleado + " " + ApellidosEmpleado,
                                fcha_inco_vccns = Convert.ToDateTime(FechaInicio),
                                fcha_fn_vcc = Convert.ToDateTime(FechaFin),
                                nmro_ds = double.Parse(NumeroDias),
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
                                Mensaje = "Empleado agregado correctamente a la lista.",
                                Resultado = Json(oLstSolicitudDetalle, JsonRequestBehavior.AllowGet)

                            };
                        }
                    }
                    else
                    {
                        oMensajeRespuesta = new MensajeRespuesta
                        {
                            Codigo = "3",
                            Mensaje = "El empleado ya se encuentra agregado en la lista. Verifique la información e inténtelo de nuevo.",
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
                                item.nmro_ds = double.Parse(NumeroDias);
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
                    Mensaje = "Ocurrió un error. Por favor contacte al administrador del sistema.",
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

            if (NumeroDias < MinimoDias)
            {
                oMensajeRespuesta = new MensajeRespuesta
                {
                    Codigo = "1",
                    Mensaje = "La cantidad de días debe ser superior a " + MinimoDias,
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



        public JsonResult CalcularFechaFin(int NumeroDias, string FechaInicio, string SabadoHabil, string DiasFestivosSabadosDomingos)
        {
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
            try
            {
                //string DiasFestivosSabadosDomingos = FestivosColombia.DiasFestivoSabadosDomingosConcatenado(DateTime.Now.Year, SabadoHabil == "NO" ? true : false);
                DateTime FechaFin = Convert.ToDateTime(FechaInicio).AddDays(NumeroDias - 1);
                int contador = 0;
                string[] Fechas;
                Fechas = DiasFestivosSabadosDomingos.Split(',');
                foreach (var item in Fechas)
                {
                    string[] DatosFechaItem = item.Split('/');

                    var FechaItem = new DateTime(Convert.ToInt32(DatosFechaItem[2]), Convert.ToInt32(DatosFechaItem[0]), Convert.ToInt32(DatosFechaItem[1])).ToShortDateString();


                    if (Convert.ToDateTime(FechaItem) >= Convert.ToDateTime(FechaInicio) && Convert.ToDateTime(FechaItem) <= FechaFin)
                        contador++;
                }

                FechaFin = CalcularFechaFinHabil(Fechas, Convert.ToDateTime(FechaInicio), FechaFin, NumeroDias, NumeroDias - contador);

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
                oMensajeRespuesta.Mensaje = "Ocurrió un error inesperado. Consulte al administrador del sistema.";
                oMensajeRespuesta.Resultado = Json(DateTime.Now.ToShortDateString(), JsonRequestBehavior.AllowGet);

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }
        }

        public DateTime CalcularFechaFinHabil(string[] Fechas, DateTime FechaInicio, DateTime FechaFin, int NumeroDias, int NumeroDiasHabiles)
        {
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();

            try
            {
                TimeSpan tSpan = new TimeSpan();
                int contador = 0;

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

                if (Resultado < NumeroDias)
                {
                    FechaFin = FechaFin.AddDays(1);
                    FechaFin = CalcularFechaFinHabil(Fechas, FechaInicio, FechaFin, NumeroDias, Resultado);

                }

                return FechaFin;

            }
            catch (Exception Ex)
            {
                Logger.Error("Ocurrió un error calculando la fecha de fin. Fecha de inicio: " +
                  FechaInicio + ". Número de días: " + NumeroDias +
                  ". Exception: " + Ex);

                oMensajeRespuesta.Codigo = "-1";
                oMensajeRespuesta.Mensaje = "Ocurrió un error inesperado. Consulte al administrador del sistema.";
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


                oModalAnotadoresModels.NombreEmpleado = oRespuestaSap.Details[0].PrimerNombre + " " +
                                                        oRespuestaSap.Details[0].SegundoNombre + " ";

                oModalAnotadoresModels.ApellidoEmpleado = oRespuestaSap.Details[0].PrimerApellido + " " +
                                                        oRespuestaSap.Details[0].SegundoApellido;



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
                oMensajeRespuesta.Mensaje = "Ocurrió un error inesperado en la consulta de la información. Contacte al administrador del sistema.";
                oMensajeRespuesta.Resultado = Json(JsonConvert.SerializeObject(oMensajeRespuesta, Formatting.Indented), JsonRequestBehavior.AllowGet);

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GuardarSolicitud(string NroIdentificacion, string NombresEmpleado, string ApellidosEmpleado,
                                         string oRespuestaSAP, string oRespuestaMotor, string NumeroDias, string SabadoHabil, string FechaInicio, string FechaFin)
        {
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
            ConsumoAPIGuardarSolicitud oConsumoAPIGuardarSolicitud = new ConsumoAPIGuardarSolicitud();
            RespuestaSAPModels oRespuestaSAPModels = new RespuestaSAPModels();
            RespuestaMotorModels oRespuestaMotorModels = new RespuestaMotorModels();
            List<SolicitudDetalle> oLstSolicitudDetalle = new List<SolicitudDetalle>();
            Solicitudes oSolicitudes = new Solicitudes();

            try
            {

                oRespuestaSAPModels = JsonConvert.DeserializeObject<RespuestaSAPModels>(oRespuestaSAP);
                oRespuestaMotorModels = JsonConvert.DeserializeObject<RespuestaMotorModels>(oRespuestaMotor);

                oLstSolicitudDetalle.Add(new SolicitudDetalle
                {
                    nmbrs_slctnte = NombresEmpleado,
                    apllds_slctnte = ApellidosEmpleado,
                    fcha_inco_vccns = Convert.ToDateTime(FechaInicio),
                    fcha_fn_vcc = Convert.ToDateTime(FechaFin),
                    nmro_ds = double.Parse(NumeroDias),
                    sbdo_hbl = oRespuestaSAPModels.Details[0].SabadoHabil == "NO" ? false : true,
                    fcha_hra_aprvc = DateTime.Now,
                    fcha_hra_rgstro_nvdd = DateTime.Now,
                    crreo_slctnte = !string.IsNullOrEmpty(oRespuestaSAPModels.Details[0].CorreoCorp) ? oRespuestaSAPModels.Details[0].CorreoCorp : oRespuestaSAPModels.Details[0].CorreoPersonal,
                    crreo_jfe_slctnte = !string.IsNullOrEmpty(oRespuestaSAPModels.Details[0].CorreoCorpJefe) ? oRespuestaSAPModels.Details[0].CorreoCorpJefe : oRespuestaSAPModels.Details[0].CorreoPersonalJefe,
                    codEmpldo = oRespuestaSAPModels.Details[0].NroPersonal,
                    idEstdoSlctd = 1,
                    scdd = oRespuestaSAPModels.Details[0].Sociedad
                });


                oSolicitudes.fcha_hra_slctd = DateTime.Now;
                oSolicitudes.nmbrs_slctnte = NombresEmpleado;
                oSolicitudes.apllds_slctnte = ApellidosEmpleado;
                oSolicitudes.nmro_idntfccn = NroIdentificacion;
                oSolicitudes.cdgo_escenario = oRespuestaMotorModels.Escenario[0].Cdgo;
                oSolicitudes.detalle = oLstSolicitudDetalle;

                oMensajeRespuesta = oConsumoAPIGuardarSolicitud.AlmacenarSolicitud(oSolicitudes);

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);

            }
            catch (Exception Ex)
            {

                Logger.Error("Ocurrió un error almacenando la solicitud de vacaciones. Nro Documento Encabezado: " +
                            oSolicitudes.nmro_idntfccn +
                            ". Exception: " + Ex);

                oMensajeRespuesta.Codigo = "-3";
                oMensajeRespuesta.Mensaje = "Ocurrió un error almacenando la solicitud de vacaciones. Contacte al administrador del sistema.";
                oMensajeRespuesta.Resultado = Json(JsonConvert.SerializeObject(oMensajeRespuesta, Formatting.Indented), JsonRequestBehavior.AllowGet);

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }

        }


    }
}
