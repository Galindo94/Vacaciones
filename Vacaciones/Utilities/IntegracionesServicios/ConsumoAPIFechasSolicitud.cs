using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Vacaciones.Models.ModelosFechasSolicitud;

namespace Vacaciones.Utilities.IntegracionesServicios
{
    public class ConsumoAPIFechasSolicitud : Controller
    {
        // Variable para almacenar los Log's
        private static readonly ILog Logger = LogManager.GetLogger(Environment.MachineName);
        // URIS para consumo del servicio del DA
        readonly string URIFechasSolicitud = WebConfigurationManager.AppSettings["URIFechasSolicitud"].ToString();
        readonly string VariableURIFechaSolicitud = WebConfigurationManager.AppSettings["VariableURIFechaSolicitud"].ToString();
        // Variables par ala peticion del WebServices
        HttpWebRequest oHttpWebRequest;
        Encoding oEncoding;
        HttpWebResponse oHttpWebResponse;

        public MensajeRespuesta ConsultarFechasSolicitud(string Identificacion)
        {
            // Variable para almacenar respuestas de los servicios
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
            //Persona Respuesta
            RespuestaFechasSolicitudModels oListFechas = new RespuestaFechasSolicitudModels();

            try
            {
                string url = URIFechasSolicitud + VariableURIFechaSolicitud + Identificacion;
                oHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                oHttpWebRequest.ContentType = "application/json";
                oHttpWebRequest.Method = "GET";
                oEncoding = Encoding.GetEncoding("utf-8");
                oHttpWebResponse = (HttpWebResponse)oHttpWebRequest.GetResponse();

                if (oHttpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    StreamReader oStreamReader = new StreamReader(oHttpWebResponse.GetResponseStream());

                    oListFechas = JsonConvert.DeserializeObject<RespuestaFechasSolicitudModels>(oStreamReader.ReadToEnd());

                    switch (oListFechas.Codigo)
                    {
                        //Se ingresó una identificacón vacía
                        case -1:
                            Logger.Info("El Nro. de identificación enviado para consultar las fechas de solicitudes actuales estaba vacío");

                            oListFechas.Mensaje = "El Nro. de identificación enviado para consultar las fechas de solicitudes actuales no puede estar vacío. Contacte al administrador del sistema";
                            break;

                        //No existen solicitudes con la identificación
                        case -2:
                            Logger.Info("Se realizo el consumo del API de Fechas de vacaciones correctamente, pero no se encontró ninguna solicitud que cumpliera con los parámetros de búsqueda.  ");
                            oListFechas.Codigo = 1;
                            oListFechas.Mensaje = "No se encontró ninguna solicitud que cumpla con los parámetros de búsqueda";
                            break;

                        //Ocurrió un error intentando consultar la identificación 
                        case -3:

                            Logger.Error("Ocurrió un error interno en el API de FechasSolicitud.  Nro de documento:  " +
                                 Identificacion +
                                ". Mensaje del servicio: " + oListFechas.Mensaje);

                            oListFechas.Mensaje = "Ocurrió un error verificando la fecha de solicitudes existente para el empleado. Contacte al administrador del sistema";

                            break;

                        //Consulta satisfactoria
                        case 1:
                            Logger.Info("Se realizo el consumo del API de Fechas de vacaciones correctamente y se obtuvieron resultados en la busqueda para el Nro documento: " + Identificacion);
                            oListFechas.Mensaje = "Se realizo el consumo del API de Fechas de vacaciones correctamente";

                            break;

                    }

                    oMensajeRespuesta.Codigo = oListFechas.Codigo.ToString();
                    oMensajeRespuesta.Mensaje = oListFechas.Mensaje;
                    oMensajeRespuesta.Resultado = Json(oListFechas, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    oMensajeRespuesta.Codigo = "-3";
                    oMensajeRespuesta.Mensaje = "Ocurrió un error interno verificando las fechas de solicitudes del empleado. Contacte al administrador del sistema";
                    oMensajeRespuesta.Resultado = Json(oListFechas, JsonRequestBehavior.AllowGet);

                    //Se deja registro en el Log del error
                    Logger.Error("Se presento un error en la disponibilidad del servicio fechas de solicitud consultando el Nro documento: " + Identificacion +
                        ". StatusCodeResponse: " + oHttpWebResponse.StatusCode.ToString() +
                        ". StatusDescriptionResponse: " + oHttpWebResponse.StatusDescription.ToString());

                }

                return oMensajeRespuesta;

            }
            catch (Exception Ex)
            {
              
                //Se deja registro en el Log del error
                Logger.Error("Se presento un error en la disponibilidad del servicio fechas de solicitud consultando el Nro documento: " + Identificacion +
                    ". Exception: " + Ex);

                oListFechas = new RespuestaFechasSolicitudModels
                {
                    Codigo = -3,
                    Mensaje = "Ocurrió un error interno verificando las fechas de solicitudes del empleado. Contacte al administrador del sistema",
                    Fechas = new List<string>()
                };

                oMensajeRespuesta.Codigo = oListFechas.Codigo.ToString();
                oMensajeRespuesta.Mensaje = oListFechas.Mensaje;
                oMensajeRespuesta.Resultado = Json(JsonConvert.SerializeObject(oListFechas, Formatting.Indented), JsonRequestBehavior.AllowGet);

                return oMensajeRespuesta;
            };

        }
    }
}