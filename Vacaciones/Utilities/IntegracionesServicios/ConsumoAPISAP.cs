using log4net;
using Newtonsoft.Json;
using System;
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
        MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
        RespuestaSAPModels oRespuestaSAP = new RespuestaSAPModels();
        readonly string URISAP = WebConfigurationManager.AppSettings["URISAP"].ToString();
        readonly string VariableAPISAP = WebConfigurationManager.AppSettings["VariableAPISAP"].ToString();
        HttpWebRequest oHttpWebRequest;
        Encoding oEncoding;
        HttpWebResponse oHttpWebResponse;

        public MensajeRespuesta ConsultarUserSAP(int Identificacion)
        {
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

                    //Pendiente por eliminar
                    var daniel = oStreamReader.ReadToEnd();

                    oRespuestaSAP = JsonConvert.DeserializeObject<RespuestaSAPModels>(oStreamReader.ReadToEnd());
                    oMensajeRespuesta.Resultado = Json(oRespuestaSAP, JsonRequestBehavior.AllowGet);

                    if (oRespuestaSAP.Exeption[0].ID != "0")
                    {
                        switch (oRespuestaSAP.Exeption[0].ID)
                        {
                            //Error en nuestra API
                            case "-3":

                                oMensajeRespuesta.Codigo = "-3";
                                oMensajeRespuesta.Mensaje = oRespuestaSAP.Exeption[0].MESSAGE;

                                //Se deja registro en el Log del error
                                Logger.Error("Se presento un error en la API que implementa el consumo de SAP. Error consultando el Nro. De Identificacion: " + Identificacion.ToString() +
                                                "Mensaje del API" + oRespuestaSAP.Exeption[0].MESSAGE +
                                                ". StatusCodeResponse: " + oHttpWebResponse.StatusCode.ToString() +
                                                ". StatusDescription: " + oHttpWebResponse.StatusDescription.ToString());
                                break;

                            //Error consultando el Api De Marcos
                            case "-2":

                                oMensajeRespuesta.Codigo = "-2";
                                oMensajeRespuesta.Mensaje = oRespuestaSAP.Exeption[0].MESSAGE;

                                //Se deja registro en el Log del error
                                Logger.Error("Se presento un error en la API de SAP. Error consultando el Nro. De Identificacion: " + Identificacion.ToString() +
                                                 "Mensaje del API" + oRespuestaSAP.Exeption[0].MESSAGE +
                                                 ". StatusCodeResponse: " + oHttpWebResponse.StatusCode.ToString() +
                                                 ". StatusDescription: " + oHttpWebResponse.StatusDescription.ToString());
                                break;

                            //Identificacion vacia
                            case "-1":

                                oMensajeRespuesta.Codigo = "-1";
                                oMensajeRespuesta.Mensaje = oRespuestaSAP.Exeption[0].MESSAGE;

                                break;
                        }
                    }
                    else
                    {
                        oMensajeRespuesta.Codigo = oRespuestaSAP.Exeption[0].ID;
                        oMensajeRespuesta.Mensaje = oRespuestaSAP.Exeption[0].MESSAGE;
                    }
                }
                else
                {
                    oMensajeRespuesta.Codigo = "-3";
                    oMensajeRespuesta.Mensaje = "Se presento un error en la disponibilidad del servicio de SAP. Contacte al administrador del sistema.";
                    oMensajeRespuesta.Resultado = Json(oRespuestaSAP, JsonRequestBehavior.AllowGet);

                    //Se deja registro en el Log del error
                    Logger.Error("Se presento un error en la API que implementa el consumo de SAP. Error consultando el Nro. De Identificacion: " + Identificacion.ToString() +
                                    "Mensaje del API" + oRespuestaSAP.Exeption[0].MESSAGE +
                                    ". StatusCodeResponse: " + oHttpWebResponse.StatusCode.ToString() +
                                    ". StatusDescription: " + oHttpWebResponse.StatusDescription.ToString());

                }

                return oMensajeRespuesta;
            }
            catch (Exception Ex)
            {
                Logger.Error("Se presento un error consultando el Nro. Documento: " + Identificacion + ". " + Ex);

                oRespuestaSAP = new RespuestaSAPModels();

                oRespuestaSAP.Exeption[0].ID = "-3";
                oRespuestaSAP.Exeption[0].MESSAGE = "Se presento un error en la disponibilidad del servicio de SAP. Contacte al administrador del sistema.";

                oMensajeRespuesta.Codigo = oRespuestaSAP.Exeption[0].ID;
                oMensajeRespuesta.Mensaje = oRespuestaSAP.Exeption[0].MESSAGE;
                oMensajeRespuesta.Resultado = Json(JsonConvert.SerializeObject(oRespuestaSAP, Formatting.Indented), JsonRequestBehavior.AllowGet);

                return oMensajeRespuesta;
            }

        }
    }
}