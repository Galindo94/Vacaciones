using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vacaciones.Models.ModelosConsumo
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