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
        PersonaModels oPersona = new PersonaModels();

        List<RespuestaSAPModels> oLstRespuestaSAPModels = new List<RespuestaSAPModels>();



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
                                                 string FechaInicio, string FechaFin, string DataActual, string oLstRespuestaSAP, string Dani)
        {
            try
            {
                RespuestaSAPModels oRespuestaSAPModels = new RespuestaSAPModels();
                List<SolicitudDetalle> oLstSolicitudDetalle = new List<SolicitudDetalle>();

                oLstSolicitudDetalle = JsonConvert.DeserializeObject<List<SolicitudDetalle>>(DataActual);





                if (!EsEdit)
                {

                    //Se valida si ya la cedula ha sido agregada
                    int Existe = oLstSolicitudDetalle
                        .Where(w => w.nmroDcmnto == NroIdentificacion).Count();

                    if (Existe == 0)
                    {
                        if (oLstRespuestaSAP != null && oLstRespuestaSAP.Count() > 0)
                        {
                            oLstRespuestaSAPModels = JsonConvert.DeserializeObject<List<RespuestaSAPModels>>(oLstRespuestaSAP);

                            if (oLstRespuestaSAPModels != null && oLstRespuestaSAPModels.Count > 0)
                            {
                                foreach (RespuestaSAPModels item in oLstRespuestaSAPModels)
                                {
                                    if (item.Details[0].NroDocumento == NroIdentificacion)
                                    {
                                        oRespuestaSAPModels = item;
                                        break;
                                    }
                                }
                            }
                        }


                        oLstSolicitudDetalle.Add(new SolicitudDetalle
                        {
                            nmroDcmnto = NroIdentificacion,
                            nmbrs_slctnte = NombresEmpleado,
                            apllds_slctnte = ApellidosEmpleado,
                            nmbre_cmplto = NombresEmpleado + " " + ApellidosEmpleado,
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
                            nmro_ds_dspnbls = int.Parse(NumeroDiasDisponibles)
                        });



                        oMensajeRespuesta = new MensajeRespuesta
                        {
                            Codigo = "1",
                            Mensaje = "Empleado agregado correctamente",
                            Resultado = Json(oLstSolicitudDetalle, JsonRequestBehavior.AllowGet)

                        };
                    }
                    else
                    {
                        oMensajeRespuesta = new MensajeRespuesta
                        {
                            Codigo = "2",
                            Mensaje = "El empleado ya ha sido agregado a la lista",
                            Resultado = Json("", JsonRequestBehavior.AllowGet)
                        };


                    }

                    return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);

                }
                else
                {

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

                    return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception Ex)
            {
                oMensajeRespuesta = new MensajeRespuesta
                {
                    Codigo = "-1",
                    Mensaje = "Ocurrió un error. Por favor contacte al administrador del sistema.",
                    Resultado = Json("", JsonRequestBehavior.AllowGet)

                };

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


        public JsonResult ConsultarEmpleadosAnotador(int Cedula)
        {
            try
            {
                bool Encontro = false;

                #region Escenario 1 planta ejecutiva


                if (Cedula == 98714393 && !Encontro)
                {
                    oMensajeRespuesta.Codigo = "1";
                    oPersona.Identificacion = Cedula;
                    oPersona.Nombres = "NELSON ENRIQUE";
                    oPersona.Apellidos = "USUGA MESA";
                    oPersona.NumeroDias = 19.42;
                    Encontro = true;
                }

                if (Cedula == 1045138486 && !Encontro)
                {
                    oMensajeRespuesta.Codigo = "1";
                    oPersona.Identificacion = Cedula;
                    oPersona.Nombres = "JOHN FREDIS";
                    oPersona.Apellidos = "CORDOBA VASQUEZ";
                    oPersona.NumeroDias = 19.67;
                    Encontro = true;
                }

                #endregion

                if (!Encontro)
                {
                    oMensajeRespuesta.Codigo = "2";
                    oMensajeRespuesta.Mensaje = "El documento ingresado no se encontró en el sistema. Verifíquelo e inténtelo de nuevo.";
                    oMensajeRespuesta.Resultado = Json("", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    oMensajeRespuesta.Mensaje = "";
                    oMensajeRespuesta.Resultado = Json(oPersona, JsonRequestBehavior.AllowGet);
                }

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                oMensajeRespuesta.Codigo = "-1";
                oMensajeRespuesta.Mensaje = "Ocurrió un error al consultar el documento. Contacte al administrador del sistema.";
                oMensajeRespuesta.Resultado = Json("", JsonRequestBehavior.AllowGet);

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult EditarEmpleado(int Cedula)
        {
            try
            {
                bool Encontro = false;

                #region Escenario 1 planta ejecutiva

                if (Cedula == 8356830 && !Encontro)
                {
                    oMensajeRespuesta.Codigo = "1";
                    oPersona.Nombres = "CRISTIAN ESTEBAN";
                    oPersona.Apellidos = "PIEDRAHITA OCAMPO";
                    oPersona.NumeroDias = 13.75;
                    Encontro = true;
                }

                if (Cedula == 98714393 && !Encontro)
                {
                    oMensajeRespuesta.Codigo = "1";
                    oPersona.Identificacion = Cedula;
                    oPersona.Nombres = "NELSON ENRIQUE";
                    oPersona.Apellidos = "USUGA MESA";
                    oPersona.NumeroDias = 19.42;
                    Encontro = true;
                }

                if (Cedula == 15374042 && !Encontro)
                {
                    oMensajeRespuesta.Codigo = "1";
                    oPersona.Nombres = "JEISON ALEJANDRO";
                    oPersona.Apellidos = "RAMIREZ RAMIREZ";
                    oPersona.NumeroDias = 8.75;
                    Encontro = true;
                }


                if (Cedula == 1045138486 && !Encontro)
                {
                    oMensajeRespuesta.Codigo = "1";
                    oPersona.Identificacion = Cedula;
                    oPersona.Nombres = "JOHN FREDIS";
                    oPersona.Apellidos = "CORDOBA VASQUEZ";
                    oPersona.NumeroDias = 19.67;
                    Encontro = true;
                }

                #endregion

                if (!Encontro)
                {
                    oMensajeRespuesta.Codigo = "2";
                    oMensajeRespuesta.Mensaje = "El documento ingresado no se encontró en el sistema. Verifíquelo e inténtelo de nuevo.";
                    oMensajeRespuesta.Resultado = Json("", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    oMensajeRespuesta.Mensaje = "";
                    oMensajeRespuesta.Resultado = Json(oPersona, JsonRequestBehavior.AllowGet);
                }

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                oMensajeRespuesta.Codigo = "-1";
                oMensajeRespuesta.Mensaje = "Ocurrió un error al consultar el documento. Contacte al administrador del sistema.";
                oMensajeRespuesta.Resultado = Json("", JsonRequestBehavior.AllowGet);

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }
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

    }
}
