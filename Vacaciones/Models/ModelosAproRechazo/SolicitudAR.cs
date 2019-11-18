using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vacaciones.Models.ModelosAproRechazo
{
    public class SolicitudAR
    {
        public int id { get; set; }
        public int fk_slctd_encbzdo { get; set; }
        public string nmbre_cmplto { get; set; }
        public string nmroDcmnto { get; set; }
        public int nmro_ds { get; set; }
        public DateTime fcha_inco_vccns { get; set; }
        public DateTime fcha_fn_vcc { get; set; }
        public string crreo_slctnte { get; set; }
        public string idntfccn_slctnte { get; set; }
        public string crro_antdr { get; set; }
    }

    public class ResultadoCambioEstado
    {
        public int Codigo { get; set; }
        public string Respuesta { get; set; }
    }
}