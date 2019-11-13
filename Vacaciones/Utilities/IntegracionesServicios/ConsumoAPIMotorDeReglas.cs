using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Configuration;
using System.Web.Mvc;
using Vacaciones.Models.ModelosMotorDeReglas;

namespace Vacaciones.Utilities.IntegracionesServicios
{
    public class ConsumoAPIMotorDeReglas : Controller
    {
        //Variable para almacenar los Log's
        private static readonly ILog Logger = LogManager.GetLogger(Environment.MachineName);
        // Variable para almacenar respuestas de los servicios        
        readonly string URIMotorReglas = WebConfigurationManager.AppSettings["URIMotorReglas"].ToString();
        readonly string Variable1MotorReglas = WebConfigurationManager.AppSettings["Variable1MotorReglas"].ToString();
        readonly string Variable2MotorReglas = WebConfigurationManager.AppSettings["Variable2MotorReglas"].ToString();
        readonly string Variable3MotorReglas = WebConfigurationManager.AppSettings["Variable3MotorReglas"].ToString();
        HttpWebRequest oHttpWebRequest;
        Encoding oEncoding;
        HttpWebResponse oHttpWebResponse;

        public MensajeRespuesta ConsultarEscenarioYReglas(string clasificacion, string gestor, string DesCargo)
        {
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
            RespuestaMotorModels oRespuestaMotor = new RespuestaMotorModels
            {
                Escenario = new List<EscenarioModels>(),
                Reglas = new List<ReglaModels>(),
                Error = new ErrorModels()
            };

            try
            {
                string url = URIMotorReglas + Variable1MotorReglas + clasificacion + "&" + Variable2MotorReglas + gestor + "&" + Variable3MotorReglas + DesCargo;
                oHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                oHttpWebRequest.ContentType = "application/json";
                oHttpWebRequest.Method = "GET";
                oEncoding = Encoding.GetEncoding("utf-8");
                oHttpWebResponse = (HttpWebResponse)oHttpWebRequest.GetResponse();

                if (oHttpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    StreamReader oStreamReader = new StreamReader(oHttpWebResponse.GetResponseStream());
                    oRespuestaMotor = JsonConvert.DeserializeObject<RespuestaMotorModels>(oStreamReader.ReadToEnd());

                    oMensajeRespuesta.Codigo = oRespuestaMotor.Error.ID.ToString();
                    oMensajeRespuesta.Resultado = Json(oRespuestaMotor, JsonRequestBehavior.AllowGet);

                    switch (oRespuestaMotor.Error.ID)
                    {
                        case 1:
                            oMensajeRespuesta.Mensaje = "";
                            break;

                        case -1:

                            Logger.Error("Ocurrió un error consultando la información del motor de reglas. Clasificacion: " +
                              clasificacion + ". Gestor: " + gestor +
                              ". Mensaje del servicio: " + oRespuestaMotor.Error.MESSAGE + ". ");


                            oMensajeRespuesta.Mensaje = "No se encontraron datos dentro del motor de reglas con los parámetros enviados. Contacte al administrador del sistema";


                            break;

                        case -2:

                            Logger.Error("No fue posible validar el escenario con los parámetros enviados. Contacte al administrador del sistema. Clasificacion: " +
                                clasificacion + ". Gestor: " + gestor +
                                ". Mensaje del servicio: " + oRespuestaMotor.Error.MESSAGE + ". ");

                            oMensajeRespuesta.Mensaje = "No fue posible validar el escenario con los parámetros enviados. Contacte al administrador del sistema";

                            break;

                        case -3:

                            Logger.Error("Ocurrió un error consultando la información del motor de reglas. Clasificacion: " +
                                clasificacion + ". Gestor: " + gestor +
                                ". Mensaje del servicio: " + oRespuestaMotor.Error.MESSAGE + ". ");

                            oMensajeRespuesta.Mensaje = "Ocurrió un error consultando la información del motor de reglas. Contacte al administrador del sistema";

                            break;
                    }

                }
                else
                {
                    Logger.Error("Ocurrió un error consultando la información del motor de reglas. Clasificacion: " +
                               clasificacion + ". Gestor: " + gestor +
                               ". StatusCodeResponse: " + oHttpWebResponse.StatusCode.ToString() +
                               ". StatusDescription: " + oHttpWebResponse.StatusDescription.ToString());

                    oMensajeRespuesta.Codigo = "-3";
                    oMensajeRespuesta.Mensaje = "Se presento un error en la disponibilidad del motor de reglas. Contacte al administrador del sistema";
                    oMensajeRespuesta.Resultado = Json(oRespuestaMotor, JsonRequestBehavior.AllowGet);

                }

                return oMensajeRespuesta;
            }
            catch (Exception Ex)
            {

                Logger.Error("Ocurrió un error consultando la información del motor de reglas. Clasificacion: " +
                    clasificacion + ". Gestor: " + gestor +
                    ". Exception: " + Ex);

                oMensajeRespuesta.Codigo = "-3";
                oMensajeRespuesta.Mensaje = "Ocurrió un error consultando la información del motor de reglas. Contacte al administrador del sistema";
                oMensajeRespuesta.Resultado = Json(JsonConvert.SerializeObject(oRespuestaMotor, Formatting.Indented), JsonRequestBehavior.AllowGet);

                return oMensajeRespuesta;
            }

        }

    }
}