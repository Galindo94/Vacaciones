using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Configuration;
using System.Web.Mvc;
using Vacaciones.Utilities;

namespace Vacaciones.Models
{
    public class ConsumoAPISAP : Controller
    {
        MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
        string URISAP = WebConfigurationManager.AppSettings["URISAP"].ToString();
        string VariableAPISAP = WebConfigurationManager.AppSettings["VariableAPISAP"].ToString();
        HttpWebRequest oHttpWebRequest;
        Encoding oEncoding;
        HttpWebResponse oHttpWebResponse;


        public MensajeRespuesta ConsultarUserSAP(int Documento)
        {
            string oRespuesta = string.Empty;

            try
            {
                string url = URISAP + VariableAPISAP + Documento;
                oHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                oHttpWebRequest.ContentType = "application/json";
                oHttpWebRequest.Method = "GET";
                oEncoding = Encoding.GetEncoding("utf-8");
                oHttpWebResponse = (HttpWebResponse)oHttpWebRequest.GetResponse();


                oHttpWebResponse = (HttpWebResponse)oHttpWebRequest.GetResponse();

                if (oHttpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    StreamReader oStreamReader = new StreamReader(oHttpWebResponse.GetResponseStream());
                    oRespuesta = oStreamReader.ReadToEnd();

                    oMensajeRespuesta.Codigo = "200";
                    oMensajeRespuesta.Mensaje = "Consumo realizado correctamente";
                    oMensajeRespuesta.Resultado = Json(oRespuesta, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    oMensajeRespuesta.Codigo = oHttpWebResponse.StatusCode.ToString();
                    oMensajeRespuesta.Mensaje = "Se presento un error en la disponibilidad del servicio del DA. Contacte al administrador del sistema.";
                    oMensajeRespuesta.Resultado = Json(oRespuesta, JsonRequestBehavior.AllowGet);

                    //To Do
                    //Almacenar Ex en Log4Net

                }

                return oMensajeRespuesta;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}