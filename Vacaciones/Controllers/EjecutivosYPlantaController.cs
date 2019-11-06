using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Vacaciones.Models.ModelosGenerales;
using Vacaciones.Models.ModelosMotorDeReglas;
using Vacaciones.Models.ModelosRespuestaSAP;
using Vacaciones.Utilities;
using Vacaciones.Utilities.IntegracionesServicios;

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

                oRespuestaMotor = JsonConvert.DeserializeObject<RespuestaMotorModels>(oDatosFormulario);
                oRespuestaSAPModels = JsonConvert.DeserializeObject<RespuestaSAPModels>(oDatosSAP);

                ViewBag.Documento = oRespuestaSAPModels.Details[0].NroDocumento;

                //Asignacion dle nombre del Empleado
                ViewBag.NombreEmpleado = oRespuestaSAPModels.Details[0].PrimerNombre + " " + oRespuestaSAPModels.Details[0].SegundoNombre + " " +
                                         oRespuestaSAPModels.Details[0].PrimerApellido + " " + oRespuestaSAPModels.Details[0].SegundoApellido;

                //Falta definir el numero de dias
                ViewBag.NumeroDias = 22;  // Pendiente por realizar ////////////////////////


                foreach (var oReglas in oRespuestaMotor.Reglas)
                {
                    switch (oReglas.Prmtro)
                    {
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

        public JsonResult ValidarCantidadDias(int NumeroDias, float NumDiasDisponibles)
        {
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();

            if (NumeroDias < 6)
            {
                oMensajeRespuesta = new MensajeRespuesta
                {
                    Codigo = "1",
                    Mensaje = "La cantidad de días debe ser superior a",
                    Resultado = Json("", JsonRequestBehavior.AllowGet)
                };
            }

            if (NumeroDias > NumDiasDisponibles)
            {
                oMensajeRespuesta = new MensajeRespuesta
                {
                    Codigo = "2",
                    Mensaje = "La cantidad de días debe ser menor o igual al número de días disponibles",
                    Resultado = Json("", JsonRequestBehavior.AllowGet)
                };
            }

            return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
        }


        public JsonResult AgregarEmpleado(string Cedula, string NumeroDias, string FechaInicio, string FechaFin)
        {
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();

            oMensajeRespuesta = new MensajeRespuesta
            {
                Codigo = "1",
                Mensaje = "Su solicitud ha sido enviada exitosamente.",
                Resultado = Json("", JsonRequestBehavior.AllowGet)
            };

            return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);

        }


        public JsonResult CalcularFechaFin(int NumeroDias, string FechaInicio, string SabadoHabil, string DiasFestivosSabadosDomingos)
        {
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
            try
            {
                //string DiasFestivosSabadosDomingos = FestivosColombia.DiasFestivoSabadosDomingosConcatenado(DateTime.Now.Year, SabadoHabil == "NO" ? true : false);
                DateTime FechaFin = Convert.ToDateTime(FechaInicio).AddDays(NumeroDias);
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
    }

}
