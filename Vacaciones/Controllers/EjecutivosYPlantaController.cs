﻿using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Vacaciones.Models.ModelosFlow;
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
            ConsumoAPIFlow oConsumoApiFlow = new ConsumoAPIFlow();

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

                FlowModels oFlow = new FlowModels
                {
                    correoSolicitante = oLstSolicitudDetalle[0].crreo_slctnte,
                    nombreSolicitante = oLstSolicitudDetalle[0].nmbrs_slctnte + " " + oLstSolicitudDetalle[0].apllds_slctnte,
                    fecha_fin = oLstSolicitudDetalle[0].fcha_fn_vcc.ToShortDateString(),
                    fecha_inicio = oLstSolicitudDetalle[0].fcha_inco_vccns.ToShortDateString(),
                    CorreoJefe = oLstSolicitudDetalle[0].crreo_jfe_slctnte,
                    url = "<a href='www.google.com'>Haga clic aqui </a>",
                    opt = 1
                };

                oConsumoApiFlow.EnviarNotificacionFlow(oFlow);

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

        //public JsonResult EnviarNotificacionFlow(string oDataActual)
        //{
        //    List<SolicitudDetalle> oLstSolicitudDetalle = new List<SolicitudDetalle>();
        //    List<string> oLstCorreos = new List<string>();
        //    MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
        //    ConsumoAPIFlow oConsumoApiFlow = new ConsumoAPIFlow();
        //    FlowModels oFlow = new FlowModels();
        //    try
        //    {
        //        string URIAprobacionyRechazo = Request.Url.Scheme + //Https
        //                                       "://" + Request.Url.Authority + //WWW.
        //                                       Request.ApplicationPath.TrimEnd('/') + "/" + //Base del sitio
        //                                       URIAprobacion; // AprobacionYRechazo/Index

        //        oLstSolicitudDetalle = GenerarObjetoSolicitudDetalle(oDataActual);

        //        if (oLstSolicitudDetalle != null && oLstSolicitudDetalle.Count > 0)
        //        {
        //            foreach (SolicitudDetalle oSolicitudDetalle in oLstSolicitudDetalle)
        //            {
        //                if (oLstCorreos == null && oLstCorreos.Count == 0)
        //                    oLstCorreos.Add(oSolicitudDetalle.crreo_jfe_slctnte);
        //                else
        //                {
        //                    int Count = oLstCorreos.Count(element => element == oSolicitudDetalle.crreo_jfe_slctnte);
        //                    if (Count == 0)
        //                        oLstCorreos.Add(oSolicitudDetalle.crreo_jfe_slctnte);
        //                }
        //            }
        //        }




        //        foreach (var oCorreo in oLstCorreos)
        //        {
        //            string correo = "<Table>";
        //            correo += "<tr>" +
        //                            "<th> Código del empleado </th>" +
        //                            "<th> Nombres y apellidos </th>" +
        //                            "<th> Fecha de inicio </th>" +
        //                            "<th> Fecha de fin </th>" +
        //                            "<th> Nro. Días </th> " +
        //                       "</tr>";

        //            foreach (var oDetalle in oLstSolicitudDetalle)
        //            {
        //                correo += "<tr>" +
        //                                "<th>" + oDetalle.codEmpldo + "</th>" +
        //                                "<th>" + oDetalle.nmbre_cmplto + "</th>" +
        //                                "<th>" + oDetalle.fcha_inco_vccns + "</th>" +
        //                                "<th>" + oDetalle.fcha_fn_vcc + "</th>" +
        //                                "<th>" + oDetalle.nmro_ds + "</th>" +
        //                          "</tr>";


        //                oFlow.correoSolicitante = oDetalle.crreo_slctnte;
        //                oFlow.nombreSolicitante = oDetalle.nmbre_cmplto;
        //                oFlow.fecha_inicio = oDetalle.fcha_inco_vccns.ToShortDateString();
        //                oFlow.fecha_fin = oDetalle.fcha_fn_vcc.ToShortDateString();
        //                oFlow.opt = 4;


        //                //Aqui se debe enviar notificacion individual
        //                oMensajeRespuesta = oConsumoApiFlow.EnviarNotificacionFlow(oFlow);

        //                if (oMensajeRespuesta.Codigo != "1")
        //                {
        //                    Logger.Error("Ocurrió un error enviando las notificaciones por correo electrónico para el empleado con código SAP: " +
        //                        oDetalle.codEmpldo + "Nombre Completo: " + oDetalle.nmbre_cmplto);
        //                }
        //            }

        //            correo += "</Table>";

        //            oFlow = new FlowModels
        //            {
        //                CorreoJefe = oCorreo,
        //                lista = correo,
        //                url = URIAprobacionyRechazo,
        //                opt = 3
        //            };

        //            //Aqui se debe enviar notificacion individual
        //            oMensajeRespuesta = oConsumoApiFlow.EnviarNotificacionFlow(oFlow);

        //            if (oMensajeRespuesta.Codigo != "1")
        //                Logger.Error("Ocurrió un error enviando las notificaciones por correo electrónico para el jefe con correo: " + oCorreo);

        //        }



        //        oMensajeRespuesta.Codigo = "1";
        //        oMensajeRespuesta.Mensaje = "Se genero la lista de correos satisfactoriamente";
        //        oMensajeRespuesta.Resultado = Json(oLstCorreos, JsonRequestBehavior.AllowGet);

        //        return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception Ex)
        //    {
        //        Logger.Error("Ocurrió un error enviando las notificaciones por correo electrónico." +
        //                   ". Exception: " + Ex);

        //        oMensajeRespuesta.Codigo = "-1";
        //        oMensajeRespuesta.Mensaje = "Ocurrió un error enviando las notificaciones por correo electrónico. Contacte al administrador del sistema";
        //        return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
        //    }
        //}
    }

}
