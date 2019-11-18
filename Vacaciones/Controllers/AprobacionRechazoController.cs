using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Vacaciones.Models.ModelosAproRechazo;
using Vacaciones.Models.ModelosFlow;
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
            oMensajeRespuesta = cons.ConsultarAprobacionRechazo(csctvo_slctd, crreo_jfe_slctnte);

            ViewBag.Respuesta = Json(oMensajeRespuesta.Resultado.Data, JsonRequestBehavior.AllowGet).Data;
            return View();
        }

        public JsonResult EnviarCambioEstado(int Id, int estado, int csctvo_slctd, string crreo_jfe_slctnte, DateTime fcha_inco_vccns, DateTime fcha_fn_vcc, string nmbre_cmplto, int fk_slctd_encbzdo, string crreo_slctnte, string crro_antdr)
        {
            ConsumoAPIAprobacion cons = new ConsumoAPIAprobacion();
            ResultadoCambioEstado oMensajeRespuesta = new ResultadoCambioEstado();
            oMensajeRespuesta = cons.CambiarEstadoSolicitud(Id, estado);

            MensajeRespuesta DataGrid = new MensajeRespuesta();
            DataGrid = cons.ConsultarAprobacionRechazo(csctvo_slctd, crreo_jfe_slctnte);
            if (oMensajeRespuesta.Codigo == 1 && estado == 3)
            {
                ConsumoAPIFlow consFlow = new ConsumoAPIFlow();
                FlowModels item = new FlowModels();
                item.cnsctvo_slctd = fk_slctd_encbzdo;
                item.CorreoJefe = crreo_jfe_slctnte;
                item.correoSolicitante = crreo_slctnte;
                item.correoAnotador = crro_antdr;
                item.fecha_inicio = fcha_inco_vccns.ToString();
                item.fecha_fin = fcha_fn_vcc.ToString();
                item.opt = 2;
                item.nombreSolicitante = nmbre_cmplto;
                MensajeRespuesta mensajeCorreo = new MensajeRespuesta();
                mensajeCorreo = consFlow.EnviarNotificacionFlow(item);
            }

            

            DataGrid.Codigo = oMensajeRespuesta.Codigo.ToString();
            DataGrid.Mensaje = oMensajeRespuesta.Respuesta.ToString();

            return Json(DataGrid, JsonRequestBehavior.AllowGet);
        }

    }
}