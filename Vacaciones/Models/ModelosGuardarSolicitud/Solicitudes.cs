using System;
using System.Collections.Generic;

namespace Vacaciones.Models.ModelosGuardarSolicitud
{
    public class Solicitudes
    {
        public DateTime? fcha_hra_slctd { get; set; }

        public string nmbrs_slctnte { get; set; }

        public string apllds_slctnte { get; set; }

        public string nmro_idntfccn { get; set; }

        public int cdgo_escenario { get; set; }

        public List<SolicitudDetalle> detalle { get; set; }
    }
}