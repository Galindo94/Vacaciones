using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Web.Mvc;
using Vacaciones.Models.ModelosMotorDeReglas;
using Vacaciones.Models.ModelosRespuestaSAP;

namespace Vacaciones.Utilities.UtilitiesGenerales
{
    public class UtilitiesGenerales : Controller
    {

        private static readonly ILog Logger = LogManager.GetLogger(Environment.MachineName);

        public double CalcularDiasContingente(List<ContigenteModels> Contigente, ReglaModels oRegla)
        {
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
            double oRespuesta = 0;

            try
            {
                string[] ValoresRango = oRegla.Crtro.Split('-');

                if (Contigente != null && Contigente.Count > 0)
                {
                    if (!string.IsNullOrEmpty(Contigente[0].NroDias))
                    {
                        string oDiasContingenteActual = Contigente[0].NroDias.Replace('.', ',');
                        if (Double.Parse(oDiasContingenteActual) >= Double.Parse(ValoresRango[0]) && Double.Parse(oDiasContingenteActual) <= Double.Parse(ValoresRango[1]))
                            Contigente[0].NroDias = oRegla.Vlr_Slda;

                        foreach (var item in Contigente)
                        {
                            string oDiasContingente = item.NroDias.Replace('.', ',');
                            oRespuesta = oRespuesta + Double.Parse(oDiasContingente);
                        }
                    }
                }

                return oRespuesta;
            }
            catch (Exception Ex)
            {
                Logger.Error("Ocurrió un error calculando el Nro de dias para el contingente. Exception: " + Ex);
                return 0;
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

        public string ObtenerIp()
        {
            try
            {
                string Ip = string.Empty;

                Ip = System.Web.HttpContext.Current.Request.UserHostAddress;


                if (!string.IsNullOrEmpty(Ip))
                {
                    return Ip;
                }
                else
                {
                    Logger.Error("Ocurrió un error obteniendo la direccion IP. Fecha de la operacion: " + DateTime.Now);
                    return Ip;

                }
            }
            catch (Exception Ex)
            {
                Logger.Error("Ocurrió un error obteniendo la direccion IP. Fecha de la operacion: " + DateTime.Now
                    + " Exception: " + Ex);
                return "";
            }


        }

        public string ObtenerNombreMaquina()
        {
            string NombreMaquina = string.Empty;

            try
            {

                NombreMaquina = Dns.GetHostEntry(System.Web.HttpContext.Current.Request.UserHostAddress).HostName;
                
                if (!string.IsNullOrEmpty(NombreMaquina))
                {
                    return NombreMaquina;
                }
                else
                {
                    Logger.Error("Ocurrió un error obteniendo el nombre de la maquina. Fecha de la operacion: " + DateTime.Now);
                    return NombreMaquina;

                }

            }

            catch (Exception Ex)
            {
                Logger.Error("Ocurrió un error obteniendo el nombre de la maquina. Fecha de la operacion: " + DateTime.Now
                    + " Exception: " + Ex);
                return "";
            }


        }
    }
}
