using GoodDriving.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace GoodDriving.Controllers
{
    public class IniciarSesionController : Controller
    {

        private readonly SCHEDULE_CLASSContext _context;

        public IniciarSesionController(SCHEDULE_CLASSContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Registro()
        {
            return View();
        }

        public IActionResult RecuperarPassword()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Recuperar(string? token)
        {
            if (token == null)
            {
                return NotFound();
            }
            Usuario usuario =  _context.Usuarios.Where(b => b.TokenRecovery == token).FirstOrDefault();
            if (usuario != null)
            {
                return View();
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> IniciarSesion(string email, string password)
        {
            password = codifica(password);
            List<Usuario> usuarios = await _context.Usuarios.Include(e => e.IdTipoNavigation).Include(e => e.IdEstadoNavigation).Where(b => b.Email == email && b.Password == password).ToListAsync();
            if(usuarios.Count > 0)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuarios[0].Id.ToString()),
                    new Claim(ClaimTypes.Email, usuarios[0].Email),
                    new Claim(ClaimTypes.Role, usuarios[0].IdTipoNavigation.Tipo)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                switch (usuarios[0].IdTipoNavigation.Tipo)
                {
                    case "ADMINISTRADOR":
                        return Content("ADMINISTRADOR");
                    case "TUTOR":
                        return Content("TUTOR");
                    case "USUARIO":
                        return Content("USUARIO");
                }
                //return Json(new { Usuario = strUsuarios});
            }
            return Content("error datos");
        }

        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> TraerTiposDeDocumento()
        {
            List<TipoDocumento> tipoDocumentos = await _context.TipoDocumentos.ToListAsync();
            List<strTipoDocumento> strTipoDocumento = new List<strTipoDocumento>();
            foreach (TipoDocumento tipoDocumento in tipoDocumentos)
            {
                strTipoDocumento str = new strTipoDocumento();
                str.Id = tipoDocumento.Id;
                str.Tipo = tipoDocumento.Tipo;
                strTipoDocumento.Add(str);
            }
            return Json(new { tipoDocumentos = strTipoDocumento });
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarUsuario(string nombres, string apellidos, string documento, int tipoDocumento, 
            string direccion, long telefono, DateTime fechaNacimiento, string email, string password, string passwordConfirm)
        {
            if(password == passwordConfirm)
            {

                //VALIDAMOS QUE EL DOCUMENTO Y CORREO NO EXISTA
                List<Usuario> Documento = await _context.Usuarios.Where(b => b.NoDocumento == documento).ToListAsync();
                List<Usuario> Email = await _context.Usuarios.Where(b => b.Email == email).ToListAsync();

                if(Documento.Count > 0 && Email.Count > 0)
                {
                    return Content("correo y documento existentes");
                }

                if (Documento.Count > 0)
                {
                    return Content("documento existente");

                }
                if (Email.Count > 0)
                {
                    return Content("email existente");

                }

                //SEPARAMOS LOS NOMBRES Y APELLIDOS
                string[] Nombres = nombres.Split();
                string[] Apellidos = apellidos.Split();
                string nombre1 = Nombres[0];
                string nombre2 = null;
                try
                {
                    nombre2 = Nombres[1];
                }
                catch (Exception)
                {
                }
                string apellido1 = Apellidos[0];
                string apellido2 = null;
                try
                {
                    apellido2 = Apellidos[1];
                }
                catch (Exception)
                {
                }
                password = codifica(password);
                //CREAMOS LOS DATOS DEL USUARIO
                Usuario usuario = new Usuario();
                usuario.NoDocumento = documento;
                usuario.Email = email;
                usuario.Password = password;
                usuario.Apellido1 = apellido1;
                usuario.Apellido2 = apellido2;
                usuario.Nombre1 = nombre1;
                usuario.Nombre2 = nombre2;
                usuario.Telefono = telefono;
                usuario.FechaNacimiento = Convert.ToDateTime(fechaNacimiento.ToString("yyyy-MM-dd"));
                usuario.Direccion = direccion;
                usuario.IdTipo = 3;
                usuario.IdEstado = 1;
                usuario.IdTipoDocumento = tipoDocumento;
                try
                {
                    _context.Add(usuario);
                    _context.SaveChanges();
                    return Content("registro realizado");
                }
                catch (Exception ex)
                {
                    return Content(ex.ToString());
                }
            }
            else
            {
                return Content("contraseñas incorrectas");
            }
        }
        
        
        [HttpPost]
        public async Task<IActionResult> RecuperarPassword(string email)
        {
            string token = GetSha256(Guid.NewGuid().ToString());
            Usuario usuario = await _context.Usuarios.Where(b => b.Email == email).FirstOrDefaultAsync();
            if (usuario != null)
            {
                try
                {
                    usuario.TokenRecovery = token;
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch(Exception ex)
                {
                    return Content(ex.Message);
                }
                string enviado = SendEmail(email, token);
                if (enviado=="ok")
                {
                    return Content("correo enviado");
                }
                else
                {
                    return Content(enviado);
                }

            }
            return Content("no existe");
        }

        [HttpPost]
        public async Task<IActionResult> Recuperar(string password, string confirmPassword, string token)
        {
            if(password != confirmPassword)
            {
                return Content("no coinciden");

            }
            Usuario usuario = await _context.Usuarios.Where(b => b.TokenRecovery == token).FirstOrDefaultAsync();
            if (usuario != null)
            {
                password = codifica(password);
                try
                {
                    usuario.TokenRecovery = null;
                    usuario.Password = password;
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                    return Content("actualizado");
                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }
            }
            return Content("error");
        }

        struct strUsuario
        {
            public int Id { get; set; }
            public string Tipo { get; set; }
            public string Estado { get; set; }
        }
    
        struct strTipoDocumento
        {
            public int Id { get; set; }
            public string Tipo { get; set; }
        }


        #region ENCRIPTACION DEL TOKEN
        private string GetSha256(string str)
        {
            SHA256 sha256 = SHA256Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            Byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha256.ComputeHash(encoding.GetBytes(str));
            for(int i = 0; i < stream.Length; i++)
            {
                sb.AppendFormat("{0:x2}", stream[i]);
            }
            return sb.ToString();
        }


        #endregion

        public static string SendEmail(string Destinatario, string token)
        {
            string urlDomain = "http://www.goodDriving.somee.com/";
            string EmailOrigen = "gooddriving2022@gmail.com";
            string Password = "proyecto2022*";
            string url = urlDomain + "IniciarSesion/Recuperar?token=" + token;

            //NOMBRE MENSAJE
            string Nombre = "Good Driving";

            string Cuerpo = "" +
                 "<div>" +
                    "<a href=\""+ url + "\">Click Aqui</a>" +
                 "</div>";
            string Asunto = "Recuperacion de Contraseña";

            var mail = new MailMessage()
            {
                From = new MailAddress(EmailOrigen, Nombre),
                Subject = Asunto,
                Body = Cuerpo,
                BodyEncoding = System.Text.Encoding.UTF8,
                SubjectEncoding = System.Text.Encoding.Default,
                IsBodyHtml = true,

            };
            mail.To.Add(Destinatario.ToLower().Trim());
            var client = new SmtpClient()
            {
                EnableSsl = true,
                Port = 587,
                Host = "smtp.gmail.com",
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(EmailOrigen, Password)
            };

            //ENVÍAMOS CORREO
            try
            {
                client.Send(mail);
                client.Dispose();
                return "ok";
            }
            catch (Exception ex)
            {
                //ENVIAMOS POR CORREO MENSAJES DE ERROR
                return ex.ToString();
            }

            /*MailMessage oMailMessage = new MailMessage(EmailOrigen, EmailDestino, "Recuperación de contraseña",
                "<p>Correo para recuperación de contraseña</p><br>" +
                "<a href='" + url + "'>Click para recuperar</a>");

            oMailMessage.IsBodyHtml = true;

            SmtpClient oSmtpClient = new SmtpClient("smtp.gmail.com");
            oSmtpClient.EnableSsl = true;
            oSmtpClient.UseDefaultCredentials = false;
            oSmtpClient.Port = 587;
            oSmtpClient.Credentials = new System.Net.NetworkCredential(EmailOrigen, Contraseña);

            oSmtpClient.Send(oMailMessage);

            oSmtpClient.Dispose();*/
        }


        //SHA256
        public static string ToHexString(byte[] array)
        {
            StringBuilder hex = new StringBuilder(array.Length * 2);
            foreach (byte b in array)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }
        public static string codifica(string valor)
        {
            string hash;
            string llave = "6v+h*+jb!+91psuc%lj8ty(ql*fx-8(1remclj(ch5=fd-5-";
            ASCIIEncoding encoder = new ASCIIEncoding();
            Byte[] code = encoder.GetBytes(llave);
            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(llave)))
            {
                Byte[] hmBytes = hmac.ComputeHash(encoder.GetBytes(valor));
                hash = ToHexString(hmBytes);
            }
            return hash.ToUpper(); 
        }
    }
}