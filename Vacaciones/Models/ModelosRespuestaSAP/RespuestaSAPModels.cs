using System.Collections.Generic;

namespace Vacaciones.Models.ModelosRespuestaSAP
{
    public class RespuestaSAPModels
    {
        public List<DetailsModels> Details { get; set; }
        public List<ExceptionModels> Exception { get; set; }
    }
}