using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Vacaciones.Models.ModelosGenerales;
using Vacaciones.Utilities;

namespace Vacaciones.Controllers
{
    public class AnotadorController : Controller
    {
        List<EmpleadoModels> oLstEmpleados = new List<EmpleadoModels>();
        MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
        PersonaModels oPersona = new PersonaModels();

        // GET: Anotador
        public ActionResult Index(string Valores)
        {
            PersonaModels oPersona = new PersonaModels();
            oPersona = JsonConvert.DeserializeObject<PersonaModels>(Valores);

            ViewBag.NombreEmpleado = oPersona.Nombres + oPersona.Apellidos;
            ViewBag.NumeroDias = oPersona.NumeroDias;
            ViewBag.Documento = oPersona.Identificacion;

            // Se obtienen las fechas de los festivos, sabados y domingos (Si se envía true incluira los sábados, si se envía false no incluirá los sábados, según criterio)
            string DiasFestivosSabadosDomingos = FestivosColombia.DiasFestivoSabadosDomingosConcatenado(DateTime.Now.Year, true);
            ViewBag.DiasFestivosSabadosDomingos = DiasFestivosSabadosDomingos;

            // Fecha donde el usuario puede iniciar sus vacaciones (Se le agregan los días según condicion, 1 o 15 días)
            ViewBag.InicioFecha = DateTime.Now.AddDays(15);


            // Fecha maxima que se le mostrará al usuario para pedir sus vacaciones (Esta para 60 días se puede cambiar según criterio)
            ViewBag.FinFecha = DateTime.Now.AddDays(60);


            return View();
        }

        public JsonResult AgregarOEditarEmpleado(string Cedula, string NumeroDias, string FechaInicio, string FechaFin, string DataActual, bool EsEdit)
        {
            try
            {
                oLstEmpleados = JsonConvert.DeserializeObject<List<EmpleadoModels>>(DataActual);

                if (!EsEdit)
                {

                    EmpleadoModels oEmpleado = new EmpleadoModels
                    {
                        Cedula = Cedula,
                        NumeroDias = Convert.ToInt32(NumeroDias),
                        FechaInicio = FechaInicio,
                        FechaFin = FechaFin
                    };

                    //Se valida si ya la cedula ha sido agregada
                    int Existe = oLstEmpleados
                        .Where(w => w.Cedula == oEmpleado.Cedula).Count();
                    if (Existe == 0)
                    {
                        oLstEmpleados.Add(oEmpleado);

                        oMensajeRespuesta = new MensajeRespuesta
                        {
                            Codigo = "1",
                            Mensaje = "Empleado agregado correctamente.",
                            Resultado = Json(oLstEmpleados, JsonRequestBehavior.AllowGet)

                        };
                    }
                    else
                    {
                        oMensajeRespuesta = new MensajeRespuesta
                        {
                            Codigo = "2",
                            Mensaje = "El empleado ya ha sido agregado a la lista.",
                            Resultado = Json("", JsonRequestBehavior.AllowGet)
                        };


                    }

                    return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);

                }
                else
                {

                    if (oLstEmpleados != null && oLstEmpleados.Count > 0)
                    {
                        foreach (var item in oLstEmpleados)
                        {
                            if (item.Cedula == Cedula)
                            {
                                item.NumeroDias = Convert.ToInt32(NumeroDias);
                                item.FechaInicio = FechaInicio;
                                item.FechaFin = FechaFin;
                            }
                        }
                    }

                    oMensajeRespuesta = new MensajeRespuesta
                    {
                        Codigo = "1",
                        Mensaje = "Empleado actualizado correctamente.",
                        Resultado = Json(oLstEmpleados, JsonRequestBehavior.AllowGet)

                    };

                    return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception)
            {
                oMensajeRespuesta = new MensajeRespuesta
                {
                    Codigo = "-1",
                    Mensaje = "Ocurrió un error. Por favor contacte al administrador del sistema.",
                    Resultado = Json(oLstEmpleados, JsonRequestBehavior.AllowGet)

                };

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult ConsultarListaEmpleado()
        {
            if (oLstEmpleados == null)
            {
                oLstEmpleados = new List<EmpleadoModels>();
            }

            return Json(oLstEmpleados, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidarCantidadDias(int NumeroDias, float NumDiasDisponibles)
        {
            if (NumeroDias < 6)
            {
                oMensajeRespuesta = new MensajeRespuesta
                {
                    Codigo = "1",
                    Mensaje = "La cantidad de días debe ser superior a 6.",
                    Resultado = Json("", JsonRequestBehavior.AllowGet)
                };
            }

            if (NumeroDias > NumDiasDisponibles)
            {
                oMensajeRespuesta = new MensajeRespuesta
                {
                    Codigo = "1",
                    Mensaje = "La cantidad de días debe ser menor o igual al número de días disponibles.",
                    Resultado = Json("", JsonRequestBehavior.AllowGet)
                };
            }

            return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ConsultarEmpleadosAnotador(int Cedula)
        {
            try
            {
                bool Encontro = false;

                #region Escenario 1 planta ejecutiva


                if (Cedula == 98714393 && !Encontro)
                {
                    oMensajeRespuesta.Codigo = "1";
                    oPersona.Identificacion = Cedula;
                    oPersona.Nombres = "NELSON ENRIQUE";
                    oPersona.Apellidos = "USUGA MESA";
                    oPersona.NumeroDias = 19.42;
                    Encontro = true;
                }

                if (Cedula == 1045138486 && !Encontro)
                {
                    oMensajeRespuesta.Codigo = "1";
                    oPersona.Identificacion = Cedula;
                    oPersona.Nombres = "JOHN FREDIS";
                    oPersona.Apellidos = "CORDOBA VASQUEZ";
                    oPersona.NumeroDias = 19.67;
                    Encontro = true;
                }

                #endregion

                if (!Encontro)
                {
                    oMensajeRespuesta.Codigo = "2";
                    oMensajeRespuesta.Mensaje = "El documento ingresado no se encontró en el sistema. Verifíquelo e inténtelo de nuevo.";
                    oMensajeRespuesta.Resultado = Json("", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    oMensajeRespuesta.Mensaje = "";
                    oMensajeRespuesta.Resultado = Json(oPersona, JsonRequestBehavior.AllowGet);
                }

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                oMensajeRespuesta.Codigo = "-1";
                oMensajeRespuesta.Mensaje = "Ocurrió un error al consultar el documento. Contacte al administrador del sistema.";
                oMensajeRespuesta.Resultado = Json("", JsonRequestBehavior.AllowGet);

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult EditarEmpleado(int Cedula)
        {
            try
            {
                bool Encontro = false;

                #region Escenario 1 planta ejecutiva

                if (Cedula == 8356830 && !Encontro)
                {
                    oMensajeRespuesta.Codigo = "1";
                    oPersona.Nombres = "CRISTIAN ESTEBAN";
                    oPersona.Apellidos = "PIEDRAHITA OCAMPO";
                    oPersona.NumeroDias = 13.75;
                    Encontro = true;
                }

                if (Cedula == 98714393 && !Encontro)
                {
                    oMensajeRespuesta.Codigo = "1";
                    oPersona.Identificacion = Cedula;
                    oPersona.Nombres = "NELSON ENRIQUE";
                    oPersona.Apellidos = "USUGA MESA";
                    oPersona.NumeroDias = 19.42;
                    Encontro = true;
                }

                if (Cedula == 15374042 && !Encontro)
                {
                    oMensajeRespuesta.Codigo = "1";
                    oPersona.Nombres = "JEISON ALEJANDRO";
                    oPersona.Apellidos = "RAMIREZ RAMIREZ";
                    oPersona.NumeroDias = 8.75;
                    Encontro = true;
                }


                if (Cedula == 1045138486 && !Encontro)
                {
                    oMensajeRespuesta.Codigo = "1";
                    oPersona.Identificacion = Cedula;
                    oPersona.Nombres = "JOHN FREDIS";
                    oPersona.Apellidos = "CORDOBA VASQUEZ";
                    oPersona.NumeroDias = 19.67;
                    Encontro = true;
                }

                #endregion

                if (!Encontro)
                {
                    oMensajeRespuesta.Codigo = "2";
                    oMensajeRespuesta.Mensaje = "El documento ingresado no se encontró en el sistema. Verifíquelo e inténtelo de nuevo.";
                    oMensajeRespuesta.Resultado = Json("", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    oMensajeRespuesta.Mensaje = "";
                    oMensajeRespuesta.Resultado = Json(oPersona, JsonRequestBehavior.AllowGet);
                }

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                oMensajeRespuesta.Codigo = "-1";
                oMensajeRespuesta.Mensaje = "Ocurrió un error al consultar el documento. Contacte al administrador del sistema.";
                oMensajeRespuesta.Resultado = Json("", JsonRequestBehavior.AllowGet);

                return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
