using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Configuration;
using System.Web.Mvc;
using Vacaciones.Models.ModelosRespuestaSAP;

namespace Vacaciones.Utilities.IntegracionesServicios
{
    public class ConsumoAPISAP : Controller
    {
        //Variable para almacenar los Log's
        private static readonly ILog Logger = LogManager.GetLogger(Environment.MachineName);
        // Variable para almacenar respuestas de los servicios
        readonly string URISAP = WebConfigurationManager.AppSettings["URISAP"].ToString();
        readonly string VariableAPISAP = WebConfigurationManager.AppSettings["VariableAPISAP"].ToString();
        readonly string SociedadVacaciones = WebConfigurationManager.AppSettings["SociedadVacaciones"].ToString();
        HttpWebRequest oHttpWebRequest;
        Encoding oEncoding;
        HttpWebResponse oHttpWebResponse;
        int POS = 0;
        int Contador = 0;

        public MensajeRespuesta ConsultarUserSAP(int Identificacion)
        {
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
            RespuestaSAPModels oRespuestaSAP = new RespuestaSAPModels
            {
                Details = new List<DetailsModels>(),
                Exception = new List<ExceptionModels>()
            };
            RespuestaSAPModels oRespuestaSAPCliente = new RespuestaSAPModels
            {
                Details = new List<DetailsModels>(),
                Exception = new List<ExceptionModels>()
            };

            try
            {
                string url = URISAP + VariableAPISAP + Identificacion;
                oHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                oHttpWebRequest.ContentType = "application/json";
                oHttpWebRequest.Method = "GET";
                oEncoding = Encoding.GetEncoding("utf-8");
                oHttpWebResponse = (HttpWebResponse)oHttpWebRequest.GetResponse();

                if (oHttpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    StreamReader oStreamReader = new StreamReader(oHttpWebResponse.GetResponseStream());

                    oRespuestaSAP = JsonConvert.DeserializeObject<RespuestaSAPModels>(oStreamReader.ReadToEnd());

                    if (oRespuestaSAP.Exception != null)
                    {
                        if (oRespuestaSAP.Exception.Count > 1)
                        {
                            foreach (var oException in oRespuestaSAP.Exception)
                            {
                                if (oException.ID == "0")
                                {
                                    if (oRespuestaSAP.Details[Contador].Sociedad == SociedadVacaciones)
                                    {
                                        POS = Contador;
                                        break;
                                    }
                                }

                                Contador++;
                            }
                        }

                        oRespuestaSAPCliente.Details.Add(oRespuestaSAP.Details[POS]);
                        oRespuestaSAPCliente.Exception.Add(oRespuestaSAP.Exception[POS]);

                        oMensajeRespuesta.Codigo = oRespuestaSAP.Exception[POS].ID;
                        oMensajeRespuesta.Mensaje = oRespuestaSAP.Exception[POS].MESSAGE;
                        oMensajeRespuesta.Resultado = Json(oRespuestaSAPCliente, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        oMensajeRespuesta.Codigo = "-3";
                        oMensajeRespuesta.Mensaje = "Se presento un error en la disponibilidad del servicio de SAP. Contacte al administrador del sistema.";
                        oMensajeRespuesta.Resultado = Json("", JsonRequestBehavior.AllowGet);

                        //Se deja registro en el Log del error
                        Logger.Error("Se presento un error en la API que implementa el consumo de SAP. Error consultando el Nro. De Identificacion: " + Identificacion.ToString());
                    }
                }
                else
                {
                    oMensajeRespuesta.Codigo = "-3";
                    oMensajeRespuesta.Mensaje = "Se presento un error en la disponibilidad del servicio de SAP. Contacte al administrador del sistema.";
                    oMensajeRespuesta.Resultado = Json(oRespuestaSAPCliente, JsonRequestBehavior.AllowGet);

                    //Se deja registro en el Log del error
                    Logger.Error("Se presento un error en la API que implementa el consumo de SAP. Error consultando el Nro. De Identificacion: " + Identificacion.ToString() +
                                    ". StatusCodeResponse: " + oHttpWebResponse.StatusCode.ToString() +
                                    ". StatusDescription: " + oHttpWebResponse.StatusDescription.ToString());

                }

                return oMensajeRespuesta;
            }
            catch (Exception Ex)
            {
                Logger.Error("Se presento un error consultando el Nro. Documento: " + Identificacion + ". " + Ex);

                oRespuestaSAPCliente.Exception[0].ID = "-3";
                oRespuestaSAPCliente.Exception[0].MESSAGE = "Se presento un error en la disponibilidad del servicio de SAP. Contacte al administrador del sistema.";

                oMensajeRespuesta.Codigo = oRespuestaSAP.Exception[0].ID;
                oMensajeRespuesta.Mensaje = oRespuestaSAP.Exception[0].MESSAGE;
                oMensajeRespuesta.Resultado = Json(JsonConvert.SerializeObject(oRespuestaSAPCliente, Formatting.Indented), JsonRequestBehavior.AllowGet);

                return oMensajeRespuesta;
            }

        }
    }
}