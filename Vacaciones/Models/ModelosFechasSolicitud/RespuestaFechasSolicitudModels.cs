using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vacaciones.Models.ModelosFechasSolicitud
{
    public class RespuestaFechasSolicitudModels
    {
        public int Codigo { get; set; }
        public string Mensaje { get; set; }
        public List<string> Fechas { get; set; }
    }
}