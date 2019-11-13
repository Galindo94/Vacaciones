using log4net;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Configuration;
using System.Web.Mvc;
using Vacaciones.Models.ModelosFlow;

namespace Vacaciones.Utilities.IntegracionesServicios
{
    public class ConsumoAPIFlow : Controller
    {
        //Variable para almacenar los Log's
        private static readonly ILog Logger = LogManager.GetLogger(Environment.MachineName);
        // Variable para almacenar respuestas de los servicios        
        readonly string URIFlow = WebConfigurationManager.AppSettings["URIFlow"].ToString();
        HttpWebRequest oHttpWebRequest;
        Encoding oEncoding;
        HttpWebResponse oHttpWebResponse;


        public MensajeRespuesta EnviarNotificacionFlow(FlowModels oFlowModels)
        {
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
            RespuestaFlowModels oRespuestaFlow = new RespuestaFlowModels();

            try
            {
                
                string url = URIFlow + "?api-version=2016-06-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=gD1TRTuzW8O4kJzHKcaUkU7r35l4-F736v2s3F76QmI";
                oHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                oHttpWebRequest.ContentType = "application/json";
                oHttpWebRequest.Method = "POST";
                oEncoding = Encoding.GetEncoding("utf-8");

                using (var streamWriter = new StreamWriter(oHttpWebRequest.GetRequestStream()))
                {
                    string json = JsonConvert.SerializeObject(oFlowModels, Formatting.Indented);

                    streamWriter.Write(json);
                }

                oHttpWebResponse = (HttpWebResponse)oHttpWebRequest.GetResponse();

                if (oHttpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    StreamReader oStreamReader = new StreamReader(oHttpWebResponse.GetResponseStream());
                    oRespuestaFlow = JsonConvert.DeserializeObject<RespuestaFlowModels>(oStreamReader.ReadToEnd());

                    oMensajeRespuesta.Resultado = Json(oRespuestaFlow, JsonRequestBehavior.AllowGet);


                    oMensajeRespuesta.Codigo = "1";
                    oMensajeRespuesta.Mensaje = "Solicitud enviada satisfactoriamente";

                    Logger.Info("La notificación de correo electrónico ha sido enviada satisfactoriamente para la solicitud con Id Nro: " +
                      oFlowModels.cnsctvo_slctd +
                      ". Mensaje del servicio: " + oRespuestaFlow.status + ". ");

                }
                else
                {

                    oMensajeRespuesta.Codigo = "-3";
                    oMensajeRespuesta.Mensaje = "Ocurrió un error realizando el envío de la notificación. Contacte al administrador del sistema";

                    Logger.Error("Ocurrió un error realizando el envió de la notificación para la solicitud con el Id Nro: " +
                       oFlowModels.cnsctvo_slctd +
                      ". Mensaje del servicio: " + oRespuestaFlow.status + ". ");

                }

                return oMensajeRespuesta;
            }
            catch (Exception Ex)
            {

                oMensajeRespuesta.Codigo = "-3";
                oMensajeRespuesta.Mensaje = "Ocurrió un error realizando el envío de la notificación. Contacte al administrador del sistema";

                Logger.Error("Ocurrió un error realizando el envió de la notificación para la solicitud con el Id Nro: " +
                   oFlowModels.cnsctvo_slctd +
                  ". Exception: " + Ex + ". ");

                return oMensajeRespuesta;
            }

        }

    }
}