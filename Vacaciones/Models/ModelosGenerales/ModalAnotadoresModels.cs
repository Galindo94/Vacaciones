﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Vacaciones.Models.ModelosGenerales
{
    public class ModalAnotadoresModels
    {
        public string NroDias { get; set; }
        public string NombreEmpleado { get; set; }
        public string ApellidoEmpleado { get; set; }
        public double MinimoDias { get; set; }
        public DateTime InicioFecha { get; set; }
        public DateTime FinFecha { get; set; }
        public string SabadoHabil { get; set; }
        public string DiasFestivosSabadosDomingos { get; set; }
        public string CorreoSolicitante { get; set; }
        public string CorreoJefeSolicitante { get; set; }
        public string CodigoEmpleado { get; set; }
        public string Sociedad { get; set; }


    }
}