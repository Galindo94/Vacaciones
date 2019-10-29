using log4net;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Configuration;
using System.Web.Mvc;
using Vacaciones.Models.ModelosGenerales;

namespace Vacaciones.Utilities.IntegracionesServicios
{
    public class ConsumoDA : Controller
    {
        // Variable para almacenar los Log's
        private static readonly ILog Logger = LogManager.GetLogger(Environment.MachineName);
        // Variable para almacenar respuestas de los servicios
        MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
        //Persona Respuesta
        PersonaModels oPersona = new PersonaModels();
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

                    oPersona = JsonConvert.DeserializeObject<PersonaModels>(oStreamReader.ReadToEnd());

                    oMensajeRespuesta.Codigo = oPersona.Codigo.ToString();
                    oMensajeRespuesta.Mensaje = oPersona.Respuesta;
                    oMensajeRespuesta.Resultado = Json(oPersona, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    oMensajeRespuesta.Codigo = "-3";
                    oMensajeRespuesta.Mensaje = "Se presento un error en la disponibilidad del servicio del DA. Contacte al administrador del sistema.";
                    oMensajeRespuesta.Resultado = Json(oPersona, JsonRequestBehavior.AllowGet);

                    //Se deja registro en el Log del error
                    Logger.Error("Se presento un error en la disponibilidad del servicio del DA consultando al usuario: " + NombreUsuario +
                        ". StatusCodeResponse: " + oHttpWebResponse.StatusCode.ToString() +
                        ". StatusDescriptionResponse: " + oHttpWebResponse.StatusDescription.ToString());

                }

                return oMensajeRespuesta;

            }
            catch (Exception Ex)
            {
                Logger.Error("Se presento un error consultando al usuario" + NombreUsuario + ". " + Ex);

                oPersona = new PersonaModels();
                oPersona.Codigo = -3;
                oPersona.Respuesta = "Ocurrió un error inesperado en la consulta de la información. Contacte al administrador del sistema.";

                oMensajeRespuesta.Codigo = oPersona.Codigo.ToString();
                oMensajeRespuesta.Mensaje = oPersona.Respuesta;
                oMensajeRespuesta.Resultado = Json(JsonConvert.SerializeObject(oPersona, Formatting.Indented), JsonRequestBehavior.AllowGet);

                return oMensajeRespuesta;
            };

        }
    }
}