using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Configuration;
using System.Web.Mvc;
using Vacaciones.Utilities;

namespace Vacaciones.Models
{
    public class ConsumoDA : Controller
    {
       

        MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();

        string URIDA = WebConfigurationManager.AppSettings["URIDA"].ToString();
        string UserDA = WebConfigurationManager.AppSettings["VariableAPIDA"].ToString();
        HttpWebRequest oHttpWebRequest;
        Encoding oEncoding;
        HttpWebResponse oHttpWebResponse;

        public MensajeRespuesta ConsultarUserDA(string NombreUsuario)
        {
            string oRespuesta = string.Empty;

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
               
                oMensajeRespuesta.Codigo = "-1";
                oMensajeRespuesta.Mensaje = "Ocurrió un error inesperado en la consulta de la información. Contacte al administrador del sistema.";
                oMensajeRespuesta.Resultado = Json(oRespuesta, JsonRequestBehavior.AllowGet);

                //To Do
                //Almacenar Ex en Log4Net

                return oMensajeRespuesta;
            }
        }
    }
}