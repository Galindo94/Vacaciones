using log4net;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Configuration;
using System.Web.Mvc;
using Vacaciones.Models.ModelosConsumo;
using Vacaciones.Utilities;

namespace Vacaciones.Utilities.IntegracionesServicios
{
    public class ConsumoDA : Controller
    {
        // Variable para almacenar los Log's
        private static readonly ILog Logger = LogManager.GetLogger(Environment.MachineName);
        // Variable para almacenar respuestas de los servicios
        MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
        // Respuesta Body
        string oRespuestaBody = string.Empty;
        // URIS para consumo del servicio del DA
        readonly string URIDA = WebConfigurationManager.AppSettings["URIDA"].ToString();
        readonly string UserDA = WebConfigurationManager.AppSettings["VariableAPIDA"].ToString();
        // Variables par ala peticion del WebServices
        HttpWebRequest oHttpWebRequest;
        Encoding oEncoding;
        HttpWebResponse oHttpWebResponse;

        public MensajeRespuesta ConsultarUserDA(string NombreUsuario)
        {
            try
            {
                string url = URIDA + UserDA + NombreUsuario;
                oHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                oHttpWebRequest.ContentType = "application/json";
                oHttpWebRequest.Method = "GET";
                oEncoding = Encoding.GetEncoding("utf-8");
                oHttpWebResponse = (HttpWebResponse)oHttpWebRequest.GetResponse();

                if (oHttpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    StreamReader oStreamReader = new StreamReader(oHttpWebResponse.GetResponseStream());
                    oRespuestaBody = oStreamReader.ReadToEnd();

                    oMensajeRespuesta.Codigo = "200";
                    oMensajeRespuesta.Mensaje = "Consumo realizado correctamente";
                    oMensajeRespuesta.Resultado = Json(oRespuestaBody, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    oMensajeRespuesta.Codigo = oHttpWebResponse.StatusCode.ToString();
                    oMensajeRespuesta.Mensaje = "Se presento un error en la disponibilidad del servicio del DA. Contacte al administrador del sistema.";
                    oMensajeRespuesta.Resultado = Json(oRespuestaBody, JsonRequestBehavior.AllowGet);

                    //Se deja registro en el Log del error
                    Logger.Info("Se presento un error en la disponibilidad del servicio del DA." + "StatusCodeResponse: " + oHttpWebResponse.StatusCode.ToString() + "StatusDescriptionResponse: " + oHttpWebResponse.StatusDescription.ToString());

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
            };

        }
    }
}