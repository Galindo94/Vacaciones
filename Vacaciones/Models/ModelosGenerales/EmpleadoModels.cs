using System.ComponentModel.DataAnnotations;

namespace Vacaciones.Models.ModelosGenerales
{
    public class EmpleadoModels
    {
        [Required]
        public string Cedula { get; set; }
        [Required]
        public int NumeroDias { get; set; }
        [Required]
        public string FechaInicio { get; set; }
        [Required]
        public string FechaFin { get; set; }
    }
}