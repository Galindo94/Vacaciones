namespace Vacaciones.Models.ModelosFlow
{
    public class FlowModels
    {
        public string correoSolicitante { get; set; }
        public string CorreoJefe { get; set; }
        public string nombreSolicitante { get; set; }
        public string fecha_inicio { get; set; }
        public string fecha_fin { get; set; }
        public int opt { get; set; }
        public string lista { get; set; }
        public string url { get; set; }

        //Propiedad adicional para almacenar el Log
        public int cnsctvo_slctd { get; set; }

    }
}