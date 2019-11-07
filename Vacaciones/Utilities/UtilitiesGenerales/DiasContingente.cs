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
                string[] ValoresRango = oRegla.Crtro.Split('-');

                if (Contigente != null && Contigente.Count > 0)
                {
                    if (!string.IsNullOrEmpty(Contigente[0].NroDias))
                    {
                        string oDiasContingenteActual = Contigente[0].NroDias.Replace('.', ',');
                        if (Double.Parse(oDiasContingenteActual) >= Double.Parse(ValoresRango[0]) && Double.Parse(oDiasContingenteActual) <= Double.Parse(ValoresRango[1]))
                            Contigente[0].NroDias = oRegla.Vlr_Slda;

                        foreach (var item in Contigente)
                        {
                            string oDiasContingente = item.NroDias.Replace('.', ',');
                            oRespuesta = oRespuesta + Double.Parse(oDiasContingente);
                        }
                    }
                }

                return oRespuesta;
            }
            catch (Exception Ex)
            {
                return 0;
            }
        }
    }
}