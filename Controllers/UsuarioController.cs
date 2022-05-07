using GoodDriving.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace GoodDriving.Controllers
{
    [Authorize(Roles = "USUARIO")]
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

        [HttpGet]
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

        [HttpPost]
        public async Task<IActionResult> RegistrarClase(int idUsuario, int idLicencia, int idTutor)
        {
            List<Clase> clases = await _context.Clases.Where(x => x.IdUsuario == idUsuario && x.IdTipo == 1).ToListAsync();
            if (clases.Count > 0)
            {
                return Content("clase solicitada");
            }
            Clase clase = new Clase();
            clase.IdTutor = idTutor;
            clase.IdUsuario = idUsuario;
            clase.IdEstado = 1;
            clase.IdTipo = 1;
            clase.FechaSolicitud = DateTime.Now.Date;
            clase.FechaFinalizacion = null;
            clase.IdLicencia = idLicencia;
            try
            {
                _context.Add(clase);
                _context.SaveChanges();
                return Content("registro realizado");
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }

        [HttpGet]
        public async Task<IActionResult> TraerClaseUsuario(int id)
        {
            List<Clase> clases = await _context.Clases.Include(x => x.IdTutorNavigation).Include(x => x.IdLicenciaNavigation).Include(x => x.IdEstadoNavigation).Where(x => x.IdUsuario == id && x.IdTipo == 1).ToListAsync();
            List<strClase> strClases = new List<strClase>();

            if (clases.Count > 0)
            {
                foreach (Clase clase in clases)
                {
                    clase.IdTutorNavigation.Nombre1 += " " + clase.IdTutorNavigation.Nombre2;
                    clase.IdTutorNavigation.Apellido1 += " " + clase.IdTutorNavigation.Apellido2;
                    strClase str = new strClase();
                    str.Id = clase.Id;
                    str.Nombre1Tutor = clase.IdTutorNavigation.Nombre1;
                    str.Apellido1Tutor = clase.IdTutorNavigation.Apellido1;
                    str.CategoriaLicencia = clase.IdLicenciaNavigation.Categoria;
                    str.FechaSolicitud = clase.FechaSolicitud.ToShortDateString();
                    str.FechaFinalizacion = clase.FechaFinalizacion.ToString();
                    str.IdEstado = clase.IdEstado;
                    str.DescripcionEstado = clase.IdEstadoNavigation.Descripcion;
                    strClases.Add(str);
                }
                return Json(new { clases = strClases });
            }
            return Content("no hay");
        }
        // ELIMINAR CLASE USUARIO
        [HttpPost]
        public async Task<IActionResult> EliminarClase(int id)
        {
            Clase clase = await _context.Clases.Where(c => c.Id == id).FirstOrDefaultAsync();
            if (clase != null)
            {
                try
                {
                    _context.Remove(clase);
                    await _context.SaveChangesAsync();
                    return Content("eliminado");
                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }

            }
            return Content("No hay");
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
        struct strClase
        {
            public int Id { get; set; }
            public int? IdTutor { get; set; }
            public int? IdUsuario { get; set; }
            public int? IdEstado { get; set; }
            public int? IdVehiculo { get; set; }
            public int? IdLicencia { get; set; }
            public int? IdTipo { get; set; }
            public string? FechaSolicitud { get; set; }
            public string? FechaFinalizacion { get; set; }
            public string? DescripcionEstado { get; set; }
            public string Apellido1Tutor { get; set; }
            public string Nombre1Tutor { get; set; }
            public string Apellido1Usuario { get; set; }
            public string Nombre1Usuario { get; set; }
            public int? IdMarca { get; set; }
            public int? IdModelo { get; set; }
            public string? DescripcionMarca { get; set; }
            public string? DescripcionModelo { get; set; }
            public string? CategoriaLicencia { get; set; }

        }

    }
}
