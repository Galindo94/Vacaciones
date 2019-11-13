using log4net;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Configuration;
using System.Web.Mvc;
using Vacaciones.Models.ModelosGuardarSolicitud;

namespace Vacaciones.Utilities.IntegracionesServicios
{
    public class ConsumoAPIGuardarSolicitud : Controller
    {
        //Variable para almacenar los Log's
        private static readonly ILog Logger = LogManager.GetLogger(Environment.MachineName);
        // Variable para almacenar respuestas de los servicios        
        readonly string URIGuardarNovedad = WebConfigurationManager.AppSettings["URIGuardarNovedad"].ToString();
        HttpWebRequest oHttpWebRequest;
        Encoding oEncoding;
        HttpWebResponse oHttpWebResponse;

        public MensajeRespuesta AlmacenarSolicitud(Solicitudes oSolicitud)
        {
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
            RespuestaGuardarSolicitudModels oRespuestaGuardarSolicitudModels = new RespuestaGuardarSolicitudModels();

            try
            {
                string url = URIGuardarNovedad;
                oHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                oHttpWebRequest.ContentType = "application/json";
                oHttpWebRequest.Method = "POST";
                oEncoding = Encoding.GetEncoding("utf-8");

                using (var streamWriter = new StreamWriter(oHttpWebRequest.GetRequestStream()))
                {
                    string json = JsonConvert.SerializeObject(oSolicitud, Formatting.Indented);

                    streamWriter.Write(json);
                }

                oHttpWebResponse = (HttpWebResponse)oHttpWebRequest.GetResponse();

                if (oHttpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    StreamReader oStreamReader = new StreamReader(oHttpWebResponse.GetResponseStream());
                    oRespuestaGuardarSolicitudModels = JsonConvert.DeserializeObject<RespuestaGuardarSolicitudModels>(oStreamReader.ReadToEnd());

                    oMensajeRespuesta.Codigo = oRespuestaGuardarSolicitudModels.Codigo.ToString();
                    oMensajeRespuesta.Resultado = Json(oRespuestaGuardarSolicitudModels, JsonRequestBehavior.AllowGet);

                    switch (oRespuestaGuardarSolicitudModels.Codigo)
                    {
                        case 1:
                            oMensajeRespuesta.Mensaje = "Solicitud enviada satisfactoriamente";
                            break;

                        case -1:

                            Logger.Error("Ocurrió un error almacenando la solicitud de vacaciones. Nro Documento Encabezado: " +
                              oSolicitud.nmro_idntfccn +
                              ". Mensaje del servicio: " + oRespuestaGuardarSolicitudModels.Respuesta + ". ");


                            oMensajeRespuesta.Mensaje = "Ocurrió un error almacenando la solicitud de vacaciones. Contacte al administrador del sistema";


                            break;

                        case -2:

                            Logger.Error("Ocurrió un error almacenando la solicitud de vacaciones. Nro Documento Encabezado: " +
                             oSolicitud.nmro_idntfccn +
                             ". Mensaje del servicio: " + oRespuestaGuardarSolicitudModels.Respuesta + ". ");

                            oMensajeRespuesta.Mensaje = "Ocurrió un error almacenando la solicitud de vacaciones. Contacte al administrador del sistema";

                            break;

                        case -3:

                            Logger.Error("Ocurrió un error almacenando la solicitud de vacaciones. Nro Documento Encabezado: " +
                             oSolicitud.nmro_idntfccn +
                             ". Mensaje del servicio: " + oRespuestaGuardarSolicitudModels.Respuesta + ". ");

                            oMensajeRespuesta.Mensaje = "Ocurrió un error almacenando la solicitud de vacaciones. Contacte al administrador del sistema";

                            break;
                    }

                }
                else
                {

                    Logger.Error("Ocurrió un error almacenando la solicitud de vacaciones. Nro Documento Encabezado: " +
                             oSolicitud.nmro_idntfccn +
                             ". StatusCodeResponse: " + oHttpWebResponse.StatusCode.ToString() +
                             ". StatusDescription: " + oHttpWebResponse.StatusDescription.ToString());

                    oMensajeRespuesta.Codigo = "-3";
                    oMensajeRespuesta.Mensaje = "Se presento un error en la disponibilidad del motor de reglas. Contacte al administrador del sistema.";
                    oMensajeRespuesta.Resultado = Json(oRespuestaGuardarSolicitudModels, JsonRequestBehavior.AllowGet);


                }

                return oMensajeRespuesta;
            }
            catch (Exception Ex)
            {

                Logger.Error("Ocurrió un error almacenando la solicitud de vacaciones. Nro Documento Encabezado: " +
                            oSolicitud.nmro_idntfccn +
                            ". Exception: " + Ex);

                oRespuestaGuardarSolicitudModels.Codigo = -3;
                oRespuestaGuardarSolicitudModels.Respuesta = "Ocurrió un error almacenando la solicitud de vacaciones. Contacte al administrador del sistema";

                oMensajeRespuesta.Codigo = oRespuestaGuardarSolicitudModels.Codigo.ToString();
                oMensajeRespuesta.Mensaje = oRespuestaGuardarSolicitudModels.Respuesta;
                oMensajeRespuesta.Resultado = Json(JsonConvert.SerializeObject(oRespuestaGuardarSolicitudModels, Formatting.Indented), JsonRequestBehavior.AllowGet);

                return oMensajeRespuesta;
            }

        }


    }
}