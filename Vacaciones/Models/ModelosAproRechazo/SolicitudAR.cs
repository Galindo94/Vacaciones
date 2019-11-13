using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vacaciones.Models.ModelosAproRechazo
{
    public class SolicitudAR
    {
        public string nmbre_cmplto { get; set; }
        public string nmroDcmnto { get; set; }
        public int nmro_ds { get; set; }
        public DateTime fcha_inco_vccns { get; set; }
        public DateTime fcha_fn_vcc { get; set; }
    }
}