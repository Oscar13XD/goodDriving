using GoodDriving.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoodDriving.Controllers
{
    [Authorize(Roles = "TUTOR")]
    public class TutorController : Controller
    {
        private readonly goodDrivingContext _context;

        public TutorController(goodDrivingContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult VerHorarioTeorico()
        {
            return View();
        }

        public IActionResult ClasesAgendadas()
        {
            return View();
        }

        public IActionResult ClasesPracticas()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> TraerHorarioTutorTeorico(int id)
        {
            List<HorarioTutor> HorarioTutors = await _context.HorarioTutors.Where(x => x.IdTutor == id).ToListAsync();
            if(HorarioTutors.Count > 0)
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

        [HttpGet]
        public async Task<IActionResult> TraerSolicitudClases(int id)
        {
            List<Clase> Clases = await _context.Clases.Include(e => e.IdTipoNavigation).Include(e => e.IdUsuarioNavigation).Include(e => e.IdLicenciaNavigation).Include(e => e.IdEstadoNavigation).Where(x=>x.IdTutor == id && (x.IdEstado != 1 && x.IdEstado != 4) && x.IdTipo == 1).ToListAsync();
            List<strClase> strClases = new List<strClase>();
            foreach (Clase clase in Clases)
            {

                strClase str = new strClase();
                str.Id = clase.Id;
                str.EmailUsuario = clase.IdUsuarioNavigation.Email;
                str.IdEstado = clase.IdEstado;
                str.IdLicencia = clase.IdLicencia;
                str.CategoriaLicencia = clase.IdLicenciaNavigation.Categoria;
                str.IdTipo = clase.IdTipo;
                str.DescripcionEstado = clase.IdEstadoNavigation.Descripcion;
                str.FechaSolicitud = clase.FechaSolicitud.ToShortDateString();
                str.FechaFinalizacion = clase.FechaFinalizacion.ToString();
                strClases.Add(str);
            }

            return Json(new { clases = strClases });
        }

        [HttpGet]
        public async Task<IActionResult> TraerEstadoClase()
        {
            List<EstadoClase> estadoClases = await _context.EstadoClases.ToListAsync();
            List<strEstadoClase> strEstadoClases = new List<strEstadoClase>();
            foreach (EstadoClase estadoClase in estadoClases)
            {
                strEstadoClase str = new strEstadoClase();
                str.Id = estadoClase.Id;
                str.Descripcion = estadoClase.Descripcion;
                strEstadoClases.Add(str);
            }
            return Json(new { estadosClase = strEstadoClases });

        }

        [HttpGet]
        public async Task<IActionResult> TraerClaseU(int id)
        {
            List<Clase> Clases = await _context.Clases.Include(e => e.IdTipoNavigation).Include(e => e.IdUsuarioNavigation).Include(e => e.IdLicenciaNavigation).Include(e => e.IdEstadoNavigation).Where(x => x.Id == id).ToListAsync();
            List<strClase> strClases = new List<strClase>();

            if (Clases.Count > 0)
            {
                foreach (Clase clase in Clases)
                {
                    string NombreUsuario = clase.IdUsuarioNavigation.Nombre1 + " " + clase.IdUsuarioNavigation.Nombre2;
                    string ApellidoUsuario = clase.IdUsuarioNavigation.Apellido1 + " " + clase.IdUsuarioNavigation.Apellido2;
                    TimeSpan Edad = DateTime.Now - clase.IdUsuarioNavigation.FechaNacimiento;

                    //string FechaFinalizacion = Convert.ToString(clase.FechaFinalizacion);
                    //DateTime fecha = DateTime.Parse(FechaFinalizacion);
                    //FechaFinalizacion = fecha.ToString("dd/MM/yyyy");

                    strClase str = new strClase();
                    str.Id = clase.Id;
                    str.IdTutor = clase.IdTutor;
                    str.IdUsuario = clase.IdUsuario;
                    str.IdEstado = clase.IdEstado;
                    str.IdLicencia = clase.IdLicencia;
                    str.CategoriaLicencia = clase.IdLicenciaNavigation.Categoria;
                    str.IdTipo = clase.IdTipo;
                    str.DescripcionEstado = clase.IdEstadoNavigation.Descripcion;
                    str.FechaSolicitud = clase.FechaSolicitud.ToShortDateString();
                    str.FechaFinalizacion = clase.FechaFinalizacion.ToString();

                    //DATOS DE USUARIO
                    str.Nombre1Usuario = NombreUsuario;
                    str.Apellido1Usuario = ApellidoUsuario;
                    str.EdadUsuario = (int)Math.Floor(Edad.TotalDays / 365.25);
                    str.DocumentoUsuario = clase.IdUsuarioNavigation.NoDocumento;
                    str.TelefonoUsuario = clase.IdUsuarioNavigation.Telefono;
                    str.DireccionUsuario = clase.IdUsuarioNavigation.Direccion;
                    str.EmailUsuario = clase.IdUsuarioNavigation.Email;

                    strClases.Add(str);
                }
                return Json(new { clase = strClases });
            }
            return Content("no hay");
        }

        [HttpPost]
        public async Task<IActionResult> EditarClase(int idClase, int idEstado)
        {
            Clase clase = await _context.Clases.Where(v => v.Id == idClase).FirstOrDefaultAsync();
            if (clase != null)
            {
                if (idEstado == 3)
                {
                    List<Clase> clases = await _context.Clases.Where(v => v.IdTutor == clase.IdTutor && v.IdUsuario == clase.IdUsuario && v.IdLicencia == clase.IdLicencia && v.IdTipo == 2).ToListAsync();
                    if (clases.Count == 0)
                    {
                        Clase clasePractica = new Clase();
                        clasePractica.IdTutor = clase.IdTutor;
                        clasePractica.IdUsuario = clase.IdUsuario;
                        clasePractica.IdEstado = 2;
                        clasePractica.IdLicencia = clase.IdLicencia;
                        clasePractica.IdTipo = 2;
                        clasePractica.FechaSolicitud = DateTime.Now.Date;
                        try
                        {
                            _context.Add(clasePractica);
                            _context.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            return Content(ex.Message);
                        }
                    }
                }
                else
                {
                    Clase clases = await _context.Clases.Where(v => v.IdTutor == clase.IdTutor && v.IdUsuario == clase.IdUsuario && v.IdLicencia == clase.IdLicencia && v.IdTipo == 2).FirstOrDefaultAsync();
                    if (clases != null)
                    {
                        try
                        {
                            _context.Remove(clases);
                            await _context.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            return Content(ex.Message);
                        }
                    }
                }
                if (idEstado == 3 || idEstado == 4)
                {
                    clase.FechaFinalizacion = DateTime.Now.Date;
                }
                else
                {
                    clase.FechaFinalizacion = null;
                }
                clase.IdEstado = idEstado;
                try
                {
                    _context.Update(clase);
                    await _context.SaveChangesAsync();
                    return Content("actualizado");
                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }
            }
            return Content("no hay");
        }

        [HttpGet]
        public async Task<IActionResult> TraerSolicitudClasesPracticas(int id)
        {
            List<Clase> Clases = await _context.Clases.Include(e => e.IdVehiculoNavigation).Include(e => e.IdVehiculoNavigation.IdMarcaNavigation).Include(e => e.IdVehiculoNavigation.IdModeloNavigation).Include(e => e.IdTipoNavigation).Include(e => e.IdUsuarioNavigation).Include(e => e.IdLicenciaNavigation).Include(e => e.IdEstadoNavigation).Where(x => x.IdTutor == id && x.IdEstado == 2 && x.IdTipo == 2).ToListAsync();
            List<strClase> strClases = new List<strClase>();
            foreach (Clase clase in Clases)
            {
                strClase str = new strClase();
                str.Id = clase.Id;
                str.EmailUsuario = clase.IdUsuarioNavigation.Email;
                str.IdEstado = clase.IdEstado;
                str.IdLicencia = clase.IdLicencia;
                str.CategoriaLicencia = clase.IdLicenciaNavigation.Categoria;
                str.IdTipo = clase.IdTipo;
                str.DescripcionEstado = clase.IdEstadoNavigation.Descripcion;
                str.FechaSolicitud = clase.FechaSolicitud.ToShortDateString();
                str.FechaFinalizacion = clase.FechaFinalizacion.ToString();

                //DATOS DEL VEHICULO
                if (clase.IdVehiculo != null)
                {
                    str.IdVehiculo = clase.IdVehiculo;
                    str.DescripcionMarca = clase.IdVehiculoNavigation.IdMarcaNavigation.Descripcion;
                    str.DescripcionModelo = clase.IdVehiculoNavigation.IdModeloNavigation.Descripcion;
                }
                else
                {
                    str.DescripcionMarca = "";
                    str.DescripcionModelo = "";
                }

                strClases.Add(str);
            }

            return Json(new { clases = strClases });
        }

        // REGISTRAR CUESTIONARIO
        [HttpPost]
        public async Task<IActionResult> RegistrarCuestionario(int pregunta1, int pregunta2, int pregunta3, int pregunta4, int pregunta5,
            int pregunta6, int pregunta7, int pregunta8, int pregunta9, int pregunta10)
        {
            int resultado= pregunta1 +pregunta2 +pregunta3 +pregunta4 +pregunta5 +pregunta6 +pregunta7 +pregunta8 +pregunta9 +pregunta10;
            return Content(resultado.ToString());
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
            public int EdadUsuario { get; set; }
            public string DocumentoUsuario { get; set; }
            public long TelefonoUsuario { get; set; }
            public string DireccionUsuario { get; set; }
            public string EmailUsuario { get; set; }
            public string DocumentoTutor { get; set; }
            public long TelefonoTutor { get; set; }
            public string DireccionTutor { get; set; }
            public string EmailTutor { get; set; }


        }

        struct strEstadoClase
        {
            public int Id { get; set; }
            public string? Descripcion { get; set; }
        }
    }
}
