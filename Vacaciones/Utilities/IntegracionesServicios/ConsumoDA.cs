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
        // URIS para consumo del servicio del DA
        readonly string URIDA = WebConfigurationManager.AppSettings["URIDA"].ToString();
        readonly string UserDA = WebConfigurationManager.AppSettings["VariableAPIDA"].ToString();
        // Variables par ala peticion del WebServices
        HttpWebRequest oHttpWebRequest;
        Encoding oEncoding;
        HttpWebResponse oHttpWebResponse;

        public MensajeRespuesta ConsultarUserDA(string NombreUsuario)
        {
            // Variable para almacenar respuestas de los servicios
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
            //Persona Respuesta
            PersonaModels oPersona = new PersonaModels();

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

                    switch (oPersona.Codigo)
                    {
                        //Usuario consultado correctamente
                        case 0:
                            break;

                        //Usuario vacio
                        case -1:
                            Logger.Error("No se envió un usuario para proceder con la consulta." +
                                "Mensaje del servicio: " + oPersona.Respuesta);

                            oPersona.Respuesta = "No se logró identificar un usuario en el directorio activo";
                            break;

                        //No se encontro usuario en el AD
                        case -2:

                            Logger.Error("No se encontró el usuario en el directorio activo. " +
                                "Nombre del usuario: " + NombreUsuario +
                                ". Mensaje del servicio: " + oPersona.Respuesta);

                            oPersona.Respuesta = "No se logró identificar un usuario en el directorio activo";

                            break;

                        //Error en el API del AD
                        case -3:

                            Logger.Error("Ocurrió un error consultando la información del directorio Activo " +
                                "Nombre del usuario: " + NombreUsuario +
                                ". Mensaje del servicio: " + oPersona.Respuesta);

                            oPersona.Respuesta = "Ocurrió un error en el api del directorio activo";
                            break;

                        //Se encontro el usuario pero no tiene informacion basica
                        case -4:

                            Logger.Error("El usuario enviado no tiene la información básica para proceder (Usuario genérico). " +
                                "Nombre del usuario: " + NombreUsuario +
                                ". Mensaje del servicio: " + oPersona.Respuesta);

                            oPersona.Respuesta = " El usuario identificado en el directorio activo no tiene la información necesaria para continuar";
                            break;
                    }

                    oMensajeRespuesta.Codigo = oPersona.Codigo.ToString();
                    oMensajeRespuesta.Mensaje = oPersona.Respuesta;
                    oMensajeRespuesta.Resultado = Json(oPersona, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    oMensajeRespuesta.Codigo = "-3";
                    oMensajeRespuesta.Mensaje = "Ocurrió un error en el api del directorio activo";
                    oMensajeRespuesta.Resultado = Json(oPersona, JsonRequestBehavior.AllowGet);

                    //Se deja registro en el Log del error
                    Logger.Error("Se presento un error en la disponibilidad del servicio del directorio activo consultando al usuario: " + NombreUsuario +
                        ". StatusCodeResponse: " + oHttpWebResponse.StatusCode.ToString() +
                        ". StatusDescriptionResponse: " + oHttpWebResponse.StatusDescription.ToString());

                }

                return oMensajeRespuesta;

            }
            catch (Exception Ex)
            {
                Logger.Error("Ocurrió un error consultando la información del directorio activo " +
                                "Nombre del usuario: " + NombreUsuario +
                                ". Exception " + Ex);

                oPersona = new PersonaModels
                {
                    Codigo = -3,
                    Respuesta = "Ocurrió un error en el api del directorio activo"
                };

                oMensajeRespuesta.Codigo = oPersona.Codigo.ToString();
                oMensajeRespuesta.Mensaje = oPersona.Respuesta;
                oMensajeRespuesta.Resultado = Json(JsonConvert.SerializeObject(oPersona, Formatting.Indented), JsonRequestBehavior.AllowGet);

                return oMensajeRespuesta;
            };

        }
    }
}