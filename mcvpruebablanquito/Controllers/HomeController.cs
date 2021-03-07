using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mcvpruebablanquito.Models;
using System.DirectoryServices.Protocols;
using System.Net;

namespace mcvpruebablanquito.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost]
        public IActionResult Logion(Usuarios user)
        {

            Result result =   autneticacion(user.Usuario, user.contraseña);

            if (result.Codigo == 1)
            {
                return Ok(result);

            }
            else
            {
                return Ok(result);
            }
           
            
        }
        public Result autneticacion(string Usuario ,string Contraseña)
        {
            Result Resultado = new Result();
            try
            {
                LdapConnection connection = new LdapConnection("192.168.1.3");
                NetworkCredential credential = new NetworkCredential(Usuario, Contraseña);
                connection.Credential = credential;
                connection.Bind();
                Resultado.Resultado = "logged in";
                Resultado.Codigo = 1;
                


            }
            catch (LdapException lexc)
            {
                String error = lexc.ServerErrorMessage;
                List<LADPCodesError> ErrosCodes = new List<LADPCodesError>()
                {
                    new LADPCodesError(){Code="52e",Descripcion="Usuario o Password Invalidos"},
                    new LADPCodesError(){Code="525",Descripcion="Usuario no encontrado"},
                    new LADPCodesError(){Code="530",Descripcion="no se permite iniciar sesión en este momento"},
                    new LADPCodesError(){Code="531",Descripcion="no se permite iniciar sesión en esta estación de trabajo"},
                    new LADPCodesError(){Code="532",Descripcion="la contraseña expiró"},
                    new LADPCodesError(){Code="533",Descripcion="Cuenta deshabilitada"},
                    new LADPCodesError(){Code="534",Descripcion="No se ha concedido al usuario el tipo de inicio de sesión solicitado en esta máquina"},
                    new LADPCodesError(){Code="701",Descripcion="Cuenta expirada"},
                    new LADPCodesError(){Code="773",Descripcion="el usuario debe restablecer la contraseña"},
                    new LADPCodesError(){Code="775",Descripcion="cuenta de usuario bloqueada"},


                };
                if (error != null)
                {
                    string pp = error.Substring(76, 4);
                    string ppp = pp.Trim();

                    Resultado.Resultado = ErrosCodes.Where(x => x.Code == ppp).Select(x => x.Descripcion).FirstOrDefault();
                    Resultado.Codigo = 0;
                }
                else
                {
                    Resultado.Resultado = lexc.Message ;
                    Resultado.Codigo = 0;
                }

            }
            catch (Exception exc)
            {
                Resultado.Resultado = exc.Message;
                Resultado.Codigo = 0;
            }
            return Resultado;
        }
    }
}
