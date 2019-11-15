using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Configuration;
using System.Web.Mvc;
using Vacaciones.Models.ModelosRespuestaSAP;

namespace Vacaciones.Utilities.IntegracionesServicios
{
    public class ConsumoAPISAP : Controller
    {
        //Variable para almacenar los Log's
        private static readonly ILog Logger = LogManager.GetLogger(Environment.MachineName);
        // Variable para almacenar respuestas de los servicios
        readonly string URISAP = WebConfigurationManager.AppSettings["URISAP"].ToString();
        readonly string VariableAPISAP = WebConfigurationManager.AppSettings["VariableAPISAP"].ToString();
        readonly string SociedadVacaciones = WebConfigurationManager.AppSettings["SociedadVacaciones"].ToString();
        HttpWebRequest oHttpWebRequest;
        Encoding oEncoding;
        HttpWebResponse oHttpWebResponse;
        int POS = 0;
        int Contador = 0;

        public MensajeRespuesta ConsultarUserSAP(int Identificacion)
        {
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
            RespuestaSAPModels oRespuestaSAP = new RespuestaSAPModels
            {
                Details = new List<DetailsModels>(),
                Exception = new List<ExceptionModels>()
            };
            RespuestaSAPModels oRespuestaSAPCliente = new RespuestaSAPModels
            {
                Details = new List<DetailsModels>(),
                Exception = new List<ExceptionModels>()
            };

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

                    oRespuestaSAP = JsonConvert.DeserializeObject<RespuestaSAPModels>(oStreamReader.ReadToEnd());

                    if (oRespuestaSAP.Exception != null && oRespuestaSAP.Exception.Count > 0)
                    {
                        if (oRespuestaSAP.Exception.Count > 1)
                        {
                            foreach (var oException in oRespuestaSAP.Exception)
                            {
                                if (oException.ID == "0")
                                {
                                    if (oRespuestaSAP.Details[Contador].Sociedad == SociedadVacaciones)
                                    {
                                        POS = Contador;
                                        break;
                                    }
                                }

                                Contador++;
                            }
                        }

                        oRespuestaSAPCliente.Details.Add(oRespuestaSAP.Details[POS]);
                        oRespuestaSAPCliente.Exception.Add(oRespuestaSAP.Exception[POS]);

                        switch (oRespuestaSAPCliente.Exception[0].ID)
                        {
                            //Procesado correctamente
                            case "4":
                                if (oRespuestaSAPCliente.Details[0].Contingentes.Contigente != null && oRespuestaSAPCliente.Details[0].Contingentes.Contigente.Count > 0 && oRespuestaSAPCliente.Details[0].IdGestor == "NO")
                                {
                                    foreach (var item in oRespuestaSAPCliente.Details[0].Contingentes.Contigente)
                                    {
                                        if (string.IsNullOrEmpty(item.NroDias))
                                        {
                                            oRespuestaSAPCliente.Exception[0].ID = "5";
                                            oRespuestaSAPCliente.Exception[POS].MESSAGE = "Usted no cuenta con días disponibles para disfrute de vacaciones";
                                        }
                                    }
                                }
                                break;

                            //Error: Favor Enviar Nro.(s) de Identificación
                            case "3":

                                Logger.Error("El número de identificación enviado al servicio de SAP estaba vacío. " +
                                    ". Mensaje del servicio: " + oRespuestaSAPCliente.Exception[0].MESSAGE);

                                oRespuestaSAPCliente.Exception[POS].MESSAGE = "No se logro identificar un documento de identidad valido para realizar la consulta pertinente. Por favor contacte al administrador del sistema";

                                break;

                            //Error: En fecha del contingente, por favor comunicarse con el área de nómina
                            case "2":

                                Logger.Error("Error en fecha del contingente consultando el documento de identidad Nro. " +
                                    Identificacion +
                                   ". Mensaje del servicio: " + oRespuestaSAPCliente.Exception[0].MESSAGE);

                                oRespuestaSAPCliente.Exception[POS].MESSAGE = "Se presento un error consultando la fecha del contingente. Por favor contacte al administrador del sistema";

                                break;

                            //Error: No se encontraron datos con la(s) Identificación(es) enviada(s)
                            case "1":

                                Logger.Error("No se encontraron datos con el documento de identidad enviado. Nro. Documento" +
                                    Identificacion +
                                   ". Mensaje del servicio: " + oRespuestaSAPCliente.Exception[0].MESSAGE);

                                oRespuestaSAPCliente.Exception[POS].MESSAGE = "No se encontraron datos con el número del documento enviado";

                                break;

                            case "-1":

                                Logger.Error("El Nro de identificacion no puede ser vacio. Nro. Documento" +
                                    Identificacion +
                                   ". Mensaje del servicio: " + oRespuestaSAPCliente.Exception[0].MESSAGE);

                                oRespuestaSAPCliente.Exception[POS].MESSAGE = "El Nro. de documento no puede ser vacío";

                                break;

                            case "-2":

                                Logger.Error("Ocurrió un error consultando la información a la API de solicitud de vacaciones. Nro. Documento" +
                                    Identificacion +
                                   ". Mensaje del servicio: " + oRespuestaSAPCliente.Exception[0].MESSAGE);

                                oRespuestaSAPCliente.Exception[POS].MESSAGE = "Ocurrió un error consultando la información a la API de solicitud de vacaciones";

                                break;


                            case "-3":

                                Logger.Error("Ocurrió un error interno en el API de solicitud de vacaciones. Contacte al administrador del sistema . Nro. Documento" +
                                    Identificacion +
                                   ". Mensaje del servicio: " + oRespuestaSAPCliente.Exception[0].MESSAGE);

                                oRespuestaSAPCliente.Exception[POS].MESSAGE = "Ocurrió un error interno en el API de solicitud de vacaciones. Contacte al administrador del sistema";

                                break;

                            case "-4":

                                Logger.Error("El empleado ya cuenta con una solicitud de vacaciones pendiente de aprobación. Nro. Documento" +
                                    Identificacion +
                                   ". Mensaje del servicio: " + oRespuestaSAPCliente.Exception[0].MESSAGE);

                                oRespuestaSAPCliente.Exception[POS].MESSAGE = "El empleado ya cuenta con una solicitud de vacaciones pendiente de aprobación";

                                break;
                        }

                        oMensajeRespuesta.Codigo = oRespuestaSAPCliente.Exception[POS].ID;
                        oMensajeRespuesta.Mensaje = oRespuestaSAPCliente.Exception[POS].MESSAGE;
                        oMensajeRespuesta.Resultado = Json(oRespuestaSAPCliente, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        Logger.Error("Ocurrió un error des serializando la respuesta del API de SAP en un Objeto de tipo RespuestaSAPModels.  " +
                               "Nro. Documento: " + Identificacion);

                        oMensajeRespuesta.Codigo = "3";
                        oMensajeRespuesta.Mensaje = "Ocurrió un error en el API del servicio de SAP";
                        oMensajeRespuesta.Resultado = Json("", JsonRequestBehavior.AllowGet);



                    }
                }
                else
                {
                    //Se deja registro en el Log del error
                    Logger.Error("Se presento un error en la API que implementa el consumo de SAP. Error consultando el Nro. De Identificacion: " + Identificacion.ToString() +
                                    ". StatusCodeResponse: " + oHttpWebResponse.StatusCode.ToString() +
                                    ". StatusDescription: " + oHttpWebResponse.StatusDescription.ToString());


                    oMensajeRespuesta.Codigo = "3";
                    oMensajeRespuesta.Mensaje = "Ocurrió un error en el API del servicio de SAP";
                    oMensajeRespuesta.Resultado = Json(oRespuestaSAPCliente, JsonRequestBehavior.AllowGet);


                }

                return oMensajeRespuesta;
            }
            catch (Exception Ex)
            {
                Logger.Error("Ocurrió un error interno en el consumo del API de SAP con el " +
                               "Nro. Documento: " + Identificacion +
                               "Exception: " + Ex);

                oMensajeRespuesta.Codigo = "3";
                oMensajeRespuesta.Mensaje = "Ocurrió un error en el API del servicio de SAP";
                oMensajeRespuesta.Resultado = Json(oRespuestaSAPCliente, JsonRequestBehavior.AllowGet);

                return oMensajeRespuesta;
            }

        }
    }
}