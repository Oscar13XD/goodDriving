using GoodDriving.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace GoodDriving.Controllers
{
    [Authorize(Roles = "ADMINISTRADOR")]
    public class AdministradorController : Controller
    {
        private readonly goodDrivingContext _context;

        public AdministradorController(goodDrivingContext context)
        {
            _context = context;
        }
        public IActionResult Index()
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

        struct strUsuario
        {
            public int Id { get; set; }
            public string Nombre1 { get; set; }
            public string Nombre2 { get; set; }
            public string Apellido1 { get; set; }
            public string Apellido2 { get; set; }
            public string Email { get; set; }
            public string Estado { get; set; }
        }
    }
}
