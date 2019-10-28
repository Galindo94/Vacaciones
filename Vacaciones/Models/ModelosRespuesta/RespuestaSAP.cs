using System.Collections.Generic;

namespace Vacaciones.Models.ModelosRespuesta
{
    public class RespuestaSAP
    {
        public List<DETAILS> DETAILS { get; set; }
        public List<EXCEPTION> EXCEPTION { get; set; }
    }
}