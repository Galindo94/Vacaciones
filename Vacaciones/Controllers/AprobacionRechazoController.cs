using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Vacaciones.Controllers
{
    public class AprobacionRechazoController : Controller
    {
        // GET: AprobacionRechazo
        public ActionResult Index(int consecutivo, string correo_jefe)
        {
            
            return View();
        }
    }
}