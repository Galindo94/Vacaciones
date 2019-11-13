using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vacaciones.Utilities;
using Vacaciones.Utilities.IntegracionesServicios;
namespace Vacaciones.Controllers
{
    public class AprobacionRechazoController : Controller
    {
        // GET: AprobacionRechazo
        public ActionResult Index(int csctvo_slctd, string crreo_jfe_slctnte)
        {
            ConsumoAPIAprobacion cons = new ConsumoAPIAprobacion();
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
            oMensajeRespuesta = cons.ConsultarAprobacionRechazo(csctvo_slctd,crreo_jfe_slctnte);
            
            ViewBag.Respuesta = Json(oMensajeRespuesta.Resultado.Data, JsonRequestBehavior.AllowGet).Data;
            return View();
        }
    }
}