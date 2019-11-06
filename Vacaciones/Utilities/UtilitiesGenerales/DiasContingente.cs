using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vacaciones.Models.ModelosMotorDeReglas;
using Vacaciones.Models.ModelosRespuestaSAP;

namespace Vacaciones.Utilities.UtilitiesGenerales
{
    public class DiasContingente
    {
        public double CalcularDiasContingente(List<ContigenteModels> Contigente, ReglaModels oRegla)
        {
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
            double oRespuesta = 0;
            try
            {
                if (Contigente != null && Contigente.Count > 0)
                {

                    string[] ValoresRango = oRegla.Crtro.Split('-');
                    foreach (var item in Contigente)
                    {
                        oRespuesta = oRespuesta + Double.Parse(item.NroDias);
                    }
                }
                return oRespuesta;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}