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
                    var DAniel = oStreamReader.ReadToEnd();
                    oAprobacion = JsonConvert.DeserializeObject<List<SolicitudAR>>(DAniel);

                    //oMensajeRespuesta.Codigo = oAprobacion.Error.ID.ToString();
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
                    csctvo_slctd + ". Coreeo jefe del solicitante: " + crreo_jfe_slctnte +
                    ". Exception: " + Ex);

                oMensajeRespuesta.Codigo = "-3";
                oMensajeRespuesta.Mensaje = "Ocurrió un error consultando la información del motor de reglas. Contacte al administrador del sistema";
                oMensajeRespuesta.Resultado = Json(JsonConvert.SerializeObject(oAprobacion, Formatting.Indented), JsonRequestBehavior.AllowGet);

                return oMensajeRespuesta;
            }
        }
    }
}