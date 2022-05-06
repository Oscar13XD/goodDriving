using GoodDriving.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace GoodDriving.Controllers
{
    //[Authorize(Roles = "USUARIO")]
    public class UsuarioController : Controller
    {
        private readonly goodDrivingContext _context;

        public UsuarioController(goodDrivingContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult SolicitarClase()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> TraerLicencias()
        {
            List<Licencium> Licencias = await _context.Licencia.ToListAsync();
            List<strLicencia> strLicencias = new List<strLicencia>();
            foreach (Licencium licencia in Licencias)
            {
                strLicencia str = new strLicencia();
                str.Id = licencia.Id;
                str.Categoria = licencia.Categoria;
                str.Descripcion = licencia.Descripcion;
                strLicencias.Add(str);
            }
            return Json(new { licencias = strLicencias });
        }

        [HttpGet]
        public async Task<IActionResult> TraerUsuariosTutores()
        {
            List<Usuario> Tutores = await _context.Usuarios.Where(e => e.IdTipo == 2).ToListAsync();
            List<strUsuario> strTutores = new List<strUsuario>();

            foreach (Usuario Tutor in Tutores)
            {
                Tutor.Nombre1 += " " + Tutor.Nombre2;
                Tutor.Apellido1 += " " + Tutor.Apellido2;
                strUsuario str = new strUsuario();
                str.Id = Tutor.Id;
                str.Nombre1 = Tutor.Nombre1;
                str.Apellido1 = Tutor.Apellido1;
                strTutores.Add(str);
            }
            return Json(new { tutores = strTutores });
        }

        [HttpGet] //LEER USUARIOS
        public async Task<IActionResult> TraerHorarioTutorTeorico(int id)
        {
            List<HorarioTutor> HorarioTutors = await _context.HorarioTutors.Where(x => x.IdTutor == id).ToListAsync();
            if (HorarioTutors.Count > 0)
            {
                List<strHorarioTutor> strHorarioTutors = new List<strHorarioTutor>();
                foreach (HorarioTutor horarioTutor in HorarioTutors)
                {
                    //horarioTutor.Nombre1 += " " + horarioTutor.Nombre2;
                    //horarioTutor.Apellido1 += " " + horarioTutor.Apellido2;
                    strHorarioTutor str = new strHorarioTutor();
                    str.Id = horarioTutor.Id;
                    str.Dia = horarioTutor.Dia;
                    str.Hora = horarioTutor.Hora;
                    str.Cupo = horarioTutor.Cupo;
                    strHorarioTutors.Add(str);
                }
                return Json(new { horarioTutors = strHorarioTutors });
            }
            return Content("no hay");
        }

        struct strLicencia
        {
            public int Id { get; set; }
            public string? Categoria { get; set; }
            public string? Descripcion { get; set; }
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
            public string FechaNacimiento { get; set; }
            public string NoDocumento { get; set; }
            public long Telefono { get; set; }
            public string Direccion { get; set; }
            public string TipoUsuario { get; set; }
            public int IdTipoDocumento { get; set; }
            public int IdTipoUsuario { get; set; }

        }
        struct strHorarioTutor
        {
            public int Id { get; set; }
            public int? IdTutor { get; set; }
            public string Nombre1Tutor { get; set; }
            public string Nombre2Tutor { get; set; }
            public string Apellido1Tutor { get; set; }
            public string Apellido2Tutor { get; set; }
            public string? Dia { get; set; }
            public string? Hora { get; set; }
            public int? Cupo { get; set; }
        }

    }
}
