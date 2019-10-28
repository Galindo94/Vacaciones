using Newtonsoft.Json;
using Vacaciones.Utilities;
using System.Web.Mvc;
using Vacaciones.Models;

namespace Vacaciones.Controllers
{
    public class VacacionesIntranetController : Controller
    {
        Persona oPersonaCodigo = new Persona();
        ConsumoDA oConsumoDA = new ConsumoDA();
        ConsumoAPISAP oConsumoAPISAP = new ConsumoAPISAP();

        MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();

        // GET: VacacionesIntranet
        public ActionResult Index()
        {
            ViewBag.UsuarioIntranet = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            return View();
        }

        // GET: VacacionesIntranet/Details/5
        public JsonResult ConsultarUserDA(string NombreUsuario)
        {
            oMensajeRespuesta = oConsumoDA.ConsultarUserDA(NombreUsuario);

            oPersonaCodigo = JsonConvert.DeserializeObject<Persona>(oMensajeRespuesta.Resultado.Data.ToString());

            if (oMensajeRespuesta.Codigo == "200")
            {
                oMensajeRespuesta.Codigo = oPersonaCodigo.Codigo.ToString();
                oMensajeRespuesta.Mensaje = oPersonaCodigo.Respuesta;

            }

            return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ConsultaUserSAP(string UserDA)
        {

            ////var json = JsonConvert.SerializeObject(UserDA.Data);
            //ConsumoDA oConsumoDAA = new ConsumoDA();
            //var br = JsonConvert.DeserializeObject<ConsumoDA>(UserDA);

            //oConsumoDAA = JsonConvert.DeserializeObject<ConsumoDA>(UserDA);

            //oMensajeRespuesta = oConsumoAPISAP.ConsultarUserSAP(oConsumoDAA.Identificacion);

            //oConsumoDA = JsonConvert.DeserializeObject<ConsumoDA>(oMensajeRespuesta.Resultado.Data.ToString());

            //if (oMensajeRespuesta.Codigo == "200")
            //{
            //    oMensajeRespuesta.Codigo = oConsumoDA.Codigo.ToString();
            //    oMensajeRespuesta.Mensaje = oConsumoDA.Respuesta;

            //    //if (oConsumoDA.Codigo == 0 && oConsumoDA.Identificacion > 0)
            //    //{
            //    //    oConsumoAPISAP.ConsultarUserSAP(oConsumoDA.Identificacion);
            //    //}
            //}

            //return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);

            return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
        }

    }
}
