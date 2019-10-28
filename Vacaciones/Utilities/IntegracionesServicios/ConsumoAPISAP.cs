using log4net;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Configuration;
using System.Web.Mvc;
using Vacaciones.Models.ModelosConsumo;
using Vacaciones.Models.ModelosRespuesta;
using Vacaciones.Utilities;

namespace Vacaciones.Utilities.IntegracionesServicios
{
    public class ConsumoAPISAP : Controller
    {
        //Variable para almacenar los Log's
        private static readonly ILog Logger = LogManager.GetLogger(Environment.MachineName);
        // Variable para almacenar respuestas de los servicios
        MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
        RespuestaSAP oRespuestaSAP = new RespuestaSAP();
        // Respuesta Body
        string oRespuestaBody = string.Empty;
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
                    oRespuestaBody = oStreamReader.ReadToEnd();

                    oRespuestaSAP = JsonConvert.DeserializeObject<RespuestaSAP>(oRespuestaBody);

                    //To Do 
                    // Validar si viene exepcion y realizar procesos pertinentes

                    oMensajeRespuesta.Codigo = "200";
                    oMensajeRespuesta.Mensaje = "Consumo realizado correctamente";
                    oMensajeRespuesta.Resultado = Json(oRespuestaBody, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    oMensajeRespuesta.Codigo = oHttpWebResponse.StatusCode.ToString();
                    oMensajeRespuesta.Mensaje = "Se presento un error en la disponibilidad del servicio de SAP. Contacte al administrador del sistema.";
                    oMensajeRespuesta.Resultado = Json(oRespuestaBody, JsonRequestBehavior.AllowGet);

                    //Se deja registro en el Log del error
                    Logger.Info("Se presento un error en la disponibilidad del servicio de SAP." + "StatusCodeResponse: " + oHttpWebResponse.StatusCode.ToString() + "StatusDescription: " + oHttpWebResponse.StatusDescription.ToString());

                }

                return oMensajeRespuesta;
            }
            catch (Exception Ex)
            {
                Logger.Error(Ex);

                PersonaModels oPersonaError = new PersonaModels()
                {
                    Codigo = -3,
                    Respuesta = "Ocurrió un error inesperado en la consulta de la información. Contacte al administrador del sistema."
                };

                oMensajeRespuesta.Codigo = "-1";
                oMensajeRespuesta.Mensaje = "Ocurrió un error inesperado en la consulta de la información. Contacte al administrador del sistema.";
                oMensajeRespuesta.Resultado = Json(JsonConvert.SerializeObject(oPersonaError, Formatting.Indented), JsonRequestBehavior.AllowGet);

                return oMensajeRespuesta;
            }

        }
    }
}