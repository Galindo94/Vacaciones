using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Vacaciones.Models.ModelosGuardarSolicitud;
using Vacaciones.Models.ModelosMotorDeReglas;
using Vacaciones.Models.ModelosRespuestaSAP;
using Vacaciones.Utilities;
using Vacaciones.Utilities.IntegracionesServicios;
using Vacaciones.Utilities.UtilitiesGenerales;

namespace Vacaciones.Controllers
{
    public class EjecutivosYPlantaController : Controller
    {
        //Variable para almacenar los Log's
        private static readonly ILog Logger = LogManager.GetLogger(Environment.MachineName);

        // GET: EjecutivosYPlanta
        public ActionResult Index(string oDatosFormulario, string oDatosSAP)
        {
            try
            {
                RespuestaMotorModels oRespuestaMotor = new RespuestaMotorModels();
                RespuestaSAPModels oRespuestaSAPModels = new RespuestaSAPModels();
                DiasContingente oDiasContingente = new DiasContingente();

                oRespuestaMotor = JsonConvert.DeserializeObject<RespuestaMotorModels>(oDatosFormulario);
                ViewBag.oRespuestaMotor = JsonConvert.SerializeObject(oRespuestaMotor);

                oRespuestaSAPModels = JsonConvert.DeserializeObject<RespuestaSAPModels>(oDatosSAP);
                ViewBag.oRespuestaSAPModels = JsonConvert.SerializeObject(oRespuestaSAPModels);

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
                oMensajeRespuesta.Mensaje = "Ocurrió un error almacenando la solicitud de vacaciones. Contacte al administrador del sistema";
                oMensajeRespuesta.Resultado = Json(JsonConvert.SerializeObject(oMensajeRespuesta, Formatting.Indented), JsonRequestBehavior.AllowGet);

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }

        }


        public JsonResult CalcularFechaFin(int NumeroDias, string FechaInicio, string DiasFestivosSabadosDomingos)
        {
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
            try
            {
                //string DiasFestivosSabadosDomingos = FestivosColombia.DiasFestivoSabadosDomingosConcatenado(DateTime.Now.Year, SabadoHabil == "NO" ? true : false);
                DateTime FechaFin = Convert.ToDateTime(FechaInicio).AddDays(NumeroDias);

                //{
                //    string[] DatosFechaItem = item.Split('/');

                //    var FechaItem = new DateTime(Convert.ToInt32(DatosFechaItem[2]), Convert.ToInt32(DatosFechaItem[0]), Convert.ToInt32(DatosFechaItem[1])).ToShortDateString();


                //    if (Convert.ToDateTime(FechaItem) >= Convert.ToDateTime(FechaInicio) && Convert.ToDateTime(FechaItem) <= FechaFin)
                //        contador++;
                //}

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
    }

}
