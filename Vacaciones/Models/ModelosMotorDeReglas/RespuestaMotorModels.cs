using System.Collections.Generic;

namespace Vacaciones.Models.ModelosMotorDeReglas
{
    public class RespuestaMotorModels
    {
        public List<EscenarioModels> Escenario { get; set; }
        public List<ReglaModels> Reglas { get; set; }
        public ErrorModels Error { get; set; }
    }
}