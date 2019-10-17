using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Vacaciones.Utilities
{
    public class MensajeRespuesta
    {
        public string Codigo { get; set; }
        public string Mensaje { get; set; }
        public JsonResult Resultado { get; set; }
    }
}