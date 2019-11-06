using System;

namespace Vacaciones.Models.ModelosGuardarSolicitud
{
    public class SolicitudDetalle
    {
        public string nmbrs_slctnte { get; set; }

        public string apllds_slctnte { get; set; }

        public DateTime fcha_inco_vccns { get; set; }

        public DateTime fcha_fn_vcc { get; set; }

        public int nmro_ds { get; set; }

        public bool sbdo_hbl { get; set; }

        public DateTime? fcha_hra_aprvc { get; set; }

        public DateTime? fcha_hra_rgstro_nvdd { get; set; }

        public string crreo_slctnte { get; set; }

        public string crreo_jfe_slctnte { get; set; }

        public string codEmpldo { get; set; }

        public int idEstdoSlctd { get; set; }

        public string scdd { get; set; }
    }
}