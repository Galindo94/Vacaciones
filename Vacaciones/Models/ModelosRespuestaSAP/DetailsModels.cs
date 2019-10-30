using System.Collections.Generic;

namespace Vacaciones.Models.ModelosRespuestaSAP
{
    public class DetailsModels
    {
        public string TipoDocumento { get; set; }
        public string NroDocumento { get; set; }
        public string NroPersonal { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string CodCargo { get; set; }
        public string DesCargo { get; set; }
        public string DivPersonal { get; set; }
        public string DesDivPersonal { get; set; }
        public string CorreoPersonal { get; set; }
        public string CorreoCorp { get; set; }
        public string Clasificacion { get; set; }
        public string FechaIngreso { get; set; }
        public string SabadoHabil { get; set; }
        public string IdGestor { get; set; }
        public string RepNovedad { get; set; }
        public string NroPersonalJefe { get; set; }
        public string PrimerApellidoJefe { get; set; }
        public string SegundoApellidoJefe { get; set; }
        public string PrimerNombreJefe { get; set; }
        public string SegundoNombreJefe { get; set; }
        public string CorreoPersonalJefe { get; set; }
        public string CorreoCorpJefe { get; set; }
        public string Sociedad { get; set; }
        public ContingentesModels Contingentes { get; set; }
        public VacacionesModels Vacaciones { get; set; }

    }
}