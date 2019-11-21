using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Configuration;
using System.Web.Mvc;
using Vacaciones.Models.ModelosAproRechazo;
namespace Vacaciones.Utilities.IntegracionesServicios
{
    public class ConsumoAPIAprobacion : Controller
    {
        //Variable para almacenar los Log's
        private static readonly ILog Logger = LogManager.GetLogger(Environment.MachineName);
        // Variable para almacenar respuestas de los servicios        
        readonly string URIAproRecha = WebConfigurationManager.AppSettings["URIAprobacionRechazo"].ToString();
        readonly string cons = WebConfigurationManager.AppSettings["consecutivo"].ToString();
        readonly string correo = WebConfigurationManager.AppSettings["correo_jefe"].ToString();
        readonly string URICambioEstado = WebConfigurationManager.AppSettings["URICambiarEstado"].ToString();
        HttpWebRequest oHttpWebRequest;
        Encoding oEncoding;
        HttpWebResponse oHttpWebResponse;

        public MensajeRespuesta ConsultarAprobacionRechazo(int csctvo_slctd, string crreo_jfe_slctnte)
        {
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
            List<SolicitudAR> oAprobacion = new List<SolicitudAR>();
            try
            {
                string url = URIAproRecha + cons + csctvo_slctd + "&" + correo + crreo_jfe_slctnte;
                oHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                oHttpWebRequest.ContentType = "application/json";
                oHttpWebRequest.Method = "GET";
                oEncoding = Encoding.GetEncoding("utf-8");
                oHttpWebResponse = (HttpWebResponse)oHttpWebRequest.GetResponse();

                if (oHttpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    StreamReader oStreamReader = new StreamReader(oHttpWebResponse.GetResponseStream());
                    string oRespuesta = oStreamReader.ReadToEnd();
                    oAprobacion = JsonConvert.DeserializeObject<List<SolicitudAR>>(oRespuesta);

                    oMensajeRespuesta.Codigo = "1";
                    oMensajeRespuesta.Mensaje = "Consulta realizada correctamente";
                    oMensajeRespuesta.Resultado = Json(oAprobacion, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Logger.Error("Ocurrió un error consultando la información de la solicitud. Consecutivo: " +
                    csctvo_slctd + ". Coreeo jefe del solicitante: " + crreo_jfe_slctnte +
                               ". StatusCodeResponse: " + oHttpWebResponse.StatusCode.ToString() +
                               ". StatusDescription: " + oHttpWebResponse.StatusDescription.ToString());

                    oMensajeRespuesta.Codigo = "-3";
                    oMensajeRespuesta.Mensaje = "Se presento un error en la disponibilidad del motor de reglas. Contacte al administrador del sistema";
                    oMensajeRespuesta.Resultado = Json(oAprobacion, JsonRequestBehavior.AllowGet);
                }

                return oMensajeRespuesta;
            }
            catch (Exception Ex)
            {

                Logger.Error("Ocurrió un error consultando la información de la solicitud. Consecutivo: " +
                    csctvo_slctd + ". Correo jefe del solicitante: " + crreo_jfe_slctnte +
                    ". Exception: " + Ex);

                oMensajeRespuesta.Codigo = "-3";
                oMensajeRespuesta.Mensaje = "Ocurrió un error consultando la información del motor de reglas. Contacte al administrador del sistema";
                oMensajeRespuesta.Resultado = Json(JsonConvert.SerializeObject(oAprobacion, Formatting.Indented), JsonRequestBehavior.AllowGet);

                return oMensajeRespuesta;
            }
        }

        public ResultadoCambioEstado CambiarEstadoSolicitud(int Id, int estado)
        {
            ResultadoCambioEstado ResultadoAPI = new ResultadoCambioEstado();
            try
            {
                string url = URICambioEstado + "Id=" + Id + "&estado=" + estado;

                oHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                oHttpWebRequest.ContentType = "application/json";
                oHttpWebRequest.Method = "GET";
                oEncoding = Encoding.GetEncoding("utf-8");
                oHttpWebResponse = (HttpWebResponse)oHttpWebRequest.GetResponse();

                if (oHttpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    StreamReader oStreamReader = new StreamReader(oHttpWebResponse.GetResponseStream());
                    var Resultado = oStreamReader.ReadToEnd();
                    ResultadoAPI = JsonConvert.DeserializeObject<ResultadoCambioEstado>(Resultado);

                    //oMensajeRespuesta.Codigo = oAprobacion.Error.ID.ToString();
                    return ResultadoAPI;
                }
                else
                {
                    Logger.Error("Ocurrió un error tratando de consultar la API de cambio de estado (api/Solicitud/ActualizarEstadoSolicitud). Id: " +
                    Id + ". Id Estado: " + estado +
                               ". StatusCodeResponse: " + oHttpWebResponse.StatusCode.ToString() +
                               ". StatusDescription: " + oHttpWebResponse.StatusDescription.ToString());

                    ResultadoAPI.Codigo = -4;
                    ResultadoAPI.Respuesta = "Se presento un error tratando de consultar la API de cambio de estado. Contacte al administrador del sistema";
                    return ResultadoAPI;
                }
            }
            catch(Exception Ex)
            {
                Logger.Error("Ocurrió un error interno. Id : " + Id +
                    ". Id Estado: " + estado +
                    ". Exception: " + Ex);

                ResultadoAPI.Codigo = -5;
                ResultadoAPI.Respuesta = "Ocurrió un error interno. Contacte al administrador del sistema";

                return ResultadoAPI;
            }
        }
    }
}