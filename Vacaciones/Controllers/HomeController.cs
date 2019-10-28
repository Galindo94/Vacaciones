using System.Collections.Generic;
using System.Web.Mvc;
using Vacaciones.Models;
using Vacaciones.Models.ModelosConsumo;
using Vacaciones.Utilities;

namespace Vacaciones.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var user = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            ViewBag.eldato = user;

            return View();
        }

        public JsonResult Login(int Cedula)
        {
            MensajeRespuesta oMensajeRespuesta = new MensajeRespuesta();
            bool Encontro = false;

            PersonaModels oPersona = new PersonaModels()
            {
                Identificacion = Cedula
            };

            #region Escenario 1 planta ejecutiva


            if (Cedula == 71625018 && !Encontro)
            {
                oMensajeRespuesta.Codigo = "1$1";
                oPersona.Nombres = "LUIS GILBERTO";
                oPersona.Apellidos = "BETANCUR ZULUAGA";
                oPersona.NumeroDias = 31.25;
                Encontro = true;
            }

            if (Cedula == 75095036 && !Encontro)
            {
                oMensajeRespuesta.Codigo = "1$1";
                oPersona.Nombres = "OSCAR YESID";
                oPersona.Apellidos = "ROMERO ARENAS";
                oPersona.NumeroDias = 33.75;
                Encontro = true;
            }

            #endregion

            #region Empleados

            if (Cedula == 98711404 && !Encontro)
            {
                oMensajeRespuesta.Codigo = "1$2";
                oPersona.Nombres = "JOAN ESTEBAN";
                oPersona.Apellidos = "BOLIVAR RESTREPO";
                oPersona.NumeroDias = 11.25;
                Encontro = true;
            }

            if (Cedula == 1017183009 && !Encontro)
            {
                oMensajeRespuesta.Codigo = "1$2";
                oPersona.Nombres = "NATALIA ANDREA";
                oPersona.Apellidos = "GAVIRIA OVIEDO";
                oPersona.NumeroDias = 22.5;
                Encontro = true;
            }

            #endregion

            #region Anotador

            if (Cedula == 8356830 && !Encontro)
            {
                oMensajeRespuesta.Codigo = "1$3";
                oPersona.Nombres = "CRISTIAN ESTEBAN";
                oPersona.Apellidos = "PIEDRAHITA OCAMPO";
                oPersona.NumeroDias = 13.75;
                Encontro = true;
            }

            if (Cedula == 15374042 && !Encontro)
            {
                oMensajeRespuesta.Codigo = "1$3";
                oPersona.Nombres = "JEISON ALEJANDRO";
                oPersona.Apellidos = "RAMIREZ RAMIREZ";
                oPersona.NumeroDias = 8.75;
                Encontro = true;
            }

            #endregion

            if (!Encontro)
            {
                oMensajeRespuesta.Codigo = "-1$-1";
                oMensajeRespuesta.Mensaje = "Documento invalido, verifiquelo e intente de nuevo.";
                oMensajeRespuesta.Resultado = Json("", JsonRequestBehavior.AllowGet);
            }
            else
            {
                //oMensajeRespuesta.Codigo = "1";
                oMensajeRespuesta.Mensaje = "";
                oMensajeRespuesta.Resultado = Json(oPersona, JsonRequestBehavior.AllowGet);
            }

            return Json(oMensajeRespuesta, JsonRequestBehavior.AllowGet);
        }


    }
}
