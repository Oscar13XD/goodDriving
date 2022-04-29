using GoodDriving.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace GoodDriving.Controllers
{
    //[Authorize(Roles = "ADMINISTRADOR")]
    public class AdministradorController : Controller
    {
        private readonly goodDrivingContext _context;

        public AdministradorController(goodDrivingContext context)
        {
            _context = context;
        }

        //SECCION DE VISTAS
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Usuarios()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> TraerUsuario(int id)
        {
            List<Usuario> usuarios = await _context.Usuarios.Include(e => e.IdTipoNavigation).Include(e => e.IdEstadoNavigation).Where(b => b.Id == id).ToListAsync();
            List< strUsuario> strUsuarios = new List<strUsuario>();
            if (usuarios.Count > 0)
            {
                foreach (var usuario in usuarios)
                {
                    strUsuario str = new strUsuario();
                    str.Id = usuario.Id;
                    str.Nombre1 = usuario.Nombre1;
                    str.Nombre2 = usuario.Nombre2;
                    str.Apellido1 = usuario.Apellido1;
                    str.Apellido2 = usuario.Apellido2;
                    str.Email = usuario.Email;
                    str.Estado = usuario.IdEstadoNavigation.Estado;
                    strUsuarios.Add(str);

                }

                return Json(new { Usuario = strUsuarios });
            }
            return Content("No existe");
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

        [HttpGet]
        public async Task<IActionResult> TraerTipoDeUsuario()
        {
            List<TipoUsuario>tipoUsuarios=await _context.TipoUsuarios.ToListAsync();
            List<strTipoUsuario> strTipoUsuario = new List<strTipoUsuario>();
            foreach (TipoUsuario tipoUsuario in tipoUsuarios)
            {
                strTipoUsuario str= new strTipoUsuario();
                str.Id = tipoUsuario.Id;
                str.Tipo= tipoUsuario.Tipo;
                strTipoUsuario.Add(str);
            }
            return Json(new { tipoUsuarios = strTipoUsuario });
        }

        [HttpGet]
        public async Task<IActionResult> TraerUsuarios()
        {
            List<Usuario> Usuarios = await _context.Usuarios.Include(e => e.IdTipoNavigation).Include(e => e.IdEstadoNavigation).ToListAsync();
            List<strUsuario> strUsuarios = new List<strUsuario>();

            foreach(Usuario Usuario in Usuarios)
            {
                Usuario.Nombre1 += " "+Usuario.Nombre2;
                Usuario.Apellido1 += " "+Usuario.Apellido2;
                strUsuario str = new strUsuario();
                str.Id = Usuario.Id;
                str.Nombre1 = Usuario.Nombre1;
                str.Apellido1 = Usuario.Apellido1;
                str.FechaNacimiento = Usuario.FechaNacimiento;
                str.NoDocumento = Usuario.NoDocumento;
                str.Telefono = Usuario.Telefono;
                str.Direccion = Usuario.Direccion;
                str.Email = Usuario.Email;
                str.TipoUsuario = Usuario.IdTipoNavigation.Tipo;
                str.Estado = Usuario.IdEstadoNavigation.Estado;
                strUsuarios.Add(str);
            }
            return Json(new { usuarios = strUsuarios });
        }


        struct strUsuario
        {
            public int Id { get; set; }
            public string Nombre1 { get; set; }
            public string Nombre2 { get; set; }
            public string Apellido1 { get; set; }
            public string Apellido2 { get; set; }
            public string Email { get; set; }
            public string Estado { get; set; }
            public DateTime FechaNacimiento { get; set; }
            public string NoDocumento { get; set; }
            public long Telefono { get; set; }
            public string Direccion { get; set; }
            public string TipoUsuario { get; set; }
 
        }

        struct strTipoDocumento
        {
            public int Id { get; set; }
            public string Tipo { get; set; }
        }
    
        struct strTipoUsuario
        {
            public int Id { get; set; }
            public string Tipo { get; set; }
        }

    }
}
