using GoodDriving.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Security.Cryptography;

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

        public IActionResult Vehiculos()
        {
            return View();
        }

        public IActionResult AsignarHorarios()
        {
            return View();
        }

        public IActionResult SolicitudClase()
        {
            return View();
        }

        //FUNCIONES USUARIOS

        [HttpPost] //CREAR USUARIO
        public async Task<IActionResult> RegistrarUsuario(string nombres, string apellidos, string documento, int tipoDocumento,
            string Direccion, long telefono, string direccion, DateTime fechanacimiento, string ingresecorreoelectronico,
            string ingresecontraseña, string confirmacontraseña, int TipoUsuario)
        {
            if (ingresecontraseña == confirmacontraseña)
            {
                List<Usuario> Documento = await _context.Usuarios.Where(b => b.NoDocumento == documento).ToListAsync();
                List<Usuario> Email = await _context.Usuarios.Where(b => b.Email == ingresecorreoelectronico).ToListAsync();

                if (Documento.Count > 0 && Email.Count > 0)
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
                ingresecontraseña = codifica(ingresecontraseña);
                //CREAMOS LOS DATOS DEL USUARIO
                Usuario usuario = new Usuario();
                usuario.NoDocumento = documento;
                usuario.Email = ingresecorreoelectronico;
                usuario.Password = ingresecontraseña;
                usuario.Apellido1 = apellido1;
                usuario.Apellido2 = apellido2;
                usuario.Nombre1 = nombre1;
                usuario.Nombre2 = nombre2;
                usuario.Telefono = telefono;
                usuario.FechaNacimiento = Convert.ToDateTime(fechanacimiento.ToString("yyyy-MM-dd"));
                usuario.Direccion = direccion;
                usuario.IdTipo = TipoUsuario;
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
            return Content("contraseñas incorrectas");
        }

        [HttpGet] //LEER USUARIOS
        public async Task<IActionResult> TraerUsuarios()
        {
            List<Usuario> Usuarios = await _context.Usuarios.Include(e => e.IdTipoNavigation).Include(e => e.IdEstadoNavigation).ToListAsync();
            List<strUsuario> strUsuarios = new List<strUsuario>();

            foreach (Usuario Usuario in Usuarios)
            {
                TimeSpan Edad = DateTime.Now - Usuario.FechaNacimiento;
                Usuario.Nombre1 += " " + Usuario.Nombre2;
                Usuario.Apellido1 += " " + Usuario.Apellido2;
                strUsuario str = new strUsuario();
                str.Id = Usuario.Id;
                str.Nombre1 = Usuario.Nombre1;
                str.Apellido1 = Usuario.Apellido1;
                str.FechaNacimiento = Usuario.FechaNacimiento.ToString("yyyy-MM-dd");
                str.NoDocumento = Usuario.NoDocumento;
                str.Telefono = Usuario.Telefono;
                str.Direccion = Usuario.Direccion;
                str.Email = Usuario.Email;
                str.TipoUsuario = Usuario.IdTipoNavigation.Tipo;
                str.Estado = Usuario.IdEstadoNavigation.Estado;
                str.Edad = (int)Math.Floor(Edad.TotalDays/365.25);
                strUsuarios.Add(str);
            }
            return Json(new { usuarios = strUsuarios });
        }

        [HttpPost] //EDITAR UN USUARIO
        public async Task<IActionResult> EditarUsuario(int idUsuario, string nombres, string apellidos, string documento,
            int tipoDocumento, string direccion, long telefono, DateTime fechanacimiento, string ingresecorreoelectronico, int TipoUsuario)
        {
            Usuario usuario = await _context.Usuarios.Where(b => b.Id == idUsuario).FirstOrDefaultAsync();
            if (usuario != null)
            {
                if (usuario.Email != ingresecorreoelectronico)
                {
                    //VALIDAR QUE EL CORREO NO EXISTA 
                    Usuario UsuarioEmail = await _context.Usuarios.Where(b => b.Email == ingresecorreoelectronico).FirstOrDefaultAsync();
                    if (UsuarioEmail != null)
                    {
                        return Content("correo existe");
                    }

                }
                if (usuario.NoDocumento != documento)
                {
                    //VALIDAMOS DE QUE EL DOCUMENTO NO EXISTA
                    Usuario UsuarioDocumento = await _context.Usuarios.Where(b => b.NoDocumento == documento).FirstOrDefaultAsync();
                    if (UsuarioDocumento != null)
                    {
                        return Content("documento existe");
                    }
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
                usuario.NoDocumento = documento;
                usuario.Email = ingresecorreoelectronico;
                usuario.Apellido1 = apellido1;
                usuario.Apellido2 = apellido2;
                usuario.Nombre1 = nombre1;
                usuario.Nombre2 = nombre2;
                usuario.Telefono = telefono;
                usuario.FechaNacimiento = Convert.ToDateTime(fechanacimiento.ToString("yyyy-MM-dd"));
                usuario.Direccion = direccion;
                usuario.IdTipo = TipoUsuario;
                usuario.IdEstado = 1;
                usuario.IdTipoDocumento = tipoDocumento;
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                    return Content("actualizado");
                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }
            }
            return Content("no existe");
        }

        [HttpGet] //LEER UN SOLO USUARIO
        public async Task<IActionResult> TraerUsuarioU(int id)
        {
            List<Usuario> Usuarios = await _context.Usuarios.Include(e => e.IdTipoNavigation).Include(e => e.IdEstadoNavigation).Include(e => e.IdTipoDocumentoNavigation).Where(b => b.Id == id).ToListAsync();
            List<strUsuario> strUsuarios = new List<strUsuario>();

            foreach (Usuario Usuario in Usuarios)
            {
                Usuario.Nombre1 += " " + Usuario.Nombre2;
                Usuario.Apellido1 += " " + Usuario.Apellido2;
                strUsuario str = new strUsuario();
                str.Id = Usuario.Id;
                str.Nombre1 = Usuario.Nombre1;
                str.Apellido1 = Usuario.Apellido1;
                str.FechaNacimiento = Usuario.FechaNacimiento.ToString("yyyy-MM-dd");
                str.NoDocumento = Usuario.NoDocumento;
                str.Telefono = Usuario.Telefono;
                str.Direccion = Usuario.Direccion;
                str.Email = Usuario.Email;
                str.TipoUsuario = Usuario.IdTipoNavigation.Tipo;
                str.Estado = Usuario.IdEstadoNavigation.Estado;
                str.IdTipoDocumento = Usuario.IdTipoDocumentoNavigation.Id;
                str.IdTipoUsuario = Usuario.IdTipoNavigation.Id;
                strUsuarios.Add(str);
            }
            return Json(new { usuario = strUsuarios });
        }

        [HttpPost] //ELIMINAR USUARIO
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            Usuario usuario = await _context.Usuarios.Where(b => b.Id == id).FirstOrDefaultAsync();
            if (usuario != null)
            {
                try
                {
                    _context.Remove(usuario);
                    await _context.SaveChangesAsync();
                    return Content("eliminado");
                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }
            }
            return Content("no encontrado");
        }


        //FUNCIONES VEHICULOS

        [HttpPost] //CREAR VEHICULO
        public async Task<IActionResult> RegistrarVehiculo(int cantidad, int marca, int modelo)
        {
            List<Vehiculo> ModeloMarca = await _context.Vehiculos.Where(b => b.IdMarca == marca && b.IdModelo == modelo).ToListAsync();
            //List<Vehiculo> Marca= await _context.Vehiculos.ToListAsync();
            if (ModeloMarca.Count > 0)
            {
                return Content("modelo y  marca existentes");
            }


            Vehiculo vehiculo = new Vehiculo();
            vehiculo.Cantidad = cantidad;
            vehiculo.IdMarca = marca;
            vehiculo.IdModelo = modelo;
            try
            {
                _context.Add(vehiculo);
                _context.SaveChanges();
                return Content("registro realizado");
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }

        }

        [HttpGet] //LEER VEHICULOS
        public async Task<IActionResult> TraerVehiculos()
        {
            List<Vehiculo> Vehiculos = await _context.Vehiculos.Include(e => e.IdMarcaNavigation).Include(e => e.IdModeloNavigation).ToListAsync();
            List<strVehiculo> strVehiculos = new List<strVehiculo>();
            foreach (Vehiculo Vehiculo in Vehiculos)
            {
                strVehiculo str = new strVehiculo();
                str.Id = Vehiculo.Id;
                str.Cantidad = Vehiculo.Cantidad;
                str.IdMarca = Vehiculo.IdMarca;
                str.IdModelo = Vehiculo.IdModelo;
                str.DescripcionMarca = Vehiculo.IdMarcaNavigation.Descripcion;
                str.DescripcionModelo = Vehiculo.IdModeloNavigation.Descripcion;
                strVehiculos.Add(str);
            }
            return Json(new { vehiculos = strVehiculos });
        }

        [HttpGet] //LEER UN SOLO VEHICULO
        public async Task<IActionResult> TraerVehiculoU(int id)
        {
            List<Vehiculo> Vehiculos = await _context.Vehiculos.Include(e => e.IdMarcaNavigation).Include(e => e.IdModeloNavigation).Where(b => b.Id == id).ToListAsync();
            List<strVehiculo> strVehiculos = new List<strVehiculo>();
            foreach (Vehiculo Vehiculo in Vehiculos)
            {
                strVehiculo str = new strVehiculo();
                str.Id = Vehiculo.Id;
                str.Cantidad = Vehiculo.Cantidad;
                str.IdMarca = Vehiculo.IdMarca;
                str.IdModelo = Vehiculo.IdModelo;
                str.DescripcionMarca = Vehiculo.IdMarcaNavigation.Descripcion;
                str.DescripcionModelo = Vehiculo.IdModeloNavigation.Descripcion;
                strVehiculos.Add(str);
            }
            return Json(new { vehiculo = strVehiculos });
        }
        [HttpPost]  // EDITAR VEHICULO

        public async Task<IActionResult> EditarVehiculo( int idVehiculo, int cantidad, int marca, int modelo)
        {
            Vehiculo Vehiculo = await _context.Vehiculos.Where(v => v.Id == idVehiculo).FirstOrDefaultAsync();
            if (Vehiculo != null)
            {
                if(Vehiculo.IdMarca != marca && Vehiculo.IdModelo != modelo)
                {
                    List<Vehiculo> ModeloMarca = await _context.Vehiculos.Where(b => b.IdMarca == marca && b.IdModelo == modelo).ToListAsync();
                    //List<Vehiculo> Marca= await _context.Vehiculos.ToListAsync();
                    if (ModeloMarca.Count > 0)
                    {
                        return Content("modelo y marca existentes");
                    }
                }
                if(Vehiculo.IdMarca != marca)
                {
                    List<Vehiculo> ModeloMarca = await _context.Vehiculos.Where(b => b.IdMarca == marca && b.IdModelo == modelo).ToListAsync();
                    //List<Vehiculo> Marca= await _context.Vehiculos.ToListAsync();
                    if (ModeloMarca.Count > 0)
                    {
                        return Content("modelo y marca existentes");
                    }
                }
                if(Vehiculo.IdModelo != modelo)
                {
                    List<Vehiculo> ModeloMarca = await _context.Vehiculos.Where(b => b.IdMarca == marca && b.IdModelo == modelo).ToListAsync();
                    //List<Vehiculo> Marca= await _context.Vehiculos.ToListAsync();
                    if (ModeloMarca.Count > 0)
                    {
                        return Content("modelo y marca existentes");
                    }
                }
                Vehiculo.IdMarca = marca;
                Vehiculo.IdModelo = modelo;
                Vehiculo.Cantidad = cantidad;
                try
                {
                    _context.Update(Vehiculo);
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
        
        [HttpPost] //ELIMINAR VEHICULO
        public async Task<IActionResult> EliminarVehiculo(int id)
        {
            Vehiculo Vehiculo = await _context.Vehiculos.Where(v => v.Id == id).FirstOrDefaultAsync();
            if (Vehiculo != null)
            {
                try
                {
                    _context.Remove(Vehiculo);
                    await _context.SaveChangesAsync();
                    return Content("eliminado");
                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }
               }
            return Content("no encontrado");
        }
        //FUNCIONES ASIGNAR HORARIOS

        [HttpPost] //REGISTRAR HORARIO
        public async Task<IActionResult> RegistrarHorario(int idTutor, string dia, string hora)
        {
            List<HorarioTutor> DiaHora = await _context.HorarioTutors.Where(x => x.IdTutor == idTutor && x.Dia == dia && x.Hora == hora).ToListAsync();
            if (DiaHora.Count > 0)
            {
                return Content("horario existente");
            }
            HorarioTutor horarioTutor = new HorarioTutor();
            horarioTutor.IdTutor = idTutor;
            horarioTutor.Dia = dia;
            horarioTutor.Hora = hora;
            horarioTutor.Cupo = 20;

            try
            {
                _context.Add(horarioTutor);
                _context.SaveChanges();
                return Content("registro realizado");
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }

        [HttpGet] //LEER LOS HORARIOS DE TUTORES
        public async Task<IActionResult> TraerTutoresHorarios()
        {
            //List<HorarioTutor> HorarioTutors = await _context.HorarioTutors.Include(e => e.IdTutorNavigation).ToListAsync();
            var HorarioTutors = await _context.HorarioTutors.Include(e => e.IdTutorNavigation).Select(e => new { e.IdTutor, e.IdTutorNavigation.Nombre1, e.IdTutorNavigation.Apellido1, e.IdTutorNavigation.Apellido2, e.IdTutorNavigation.Nombre2 }).Distinct().ToListAsync();
            List<strHorarioTutor> strHorarioTutors = new List<strHorarioTutor>();
            foreach (var horarioTutor in HorarioTutors)
            {
                //horarioTutor.Nombre1 += " " + horarioTutor.Nombre2;
                //horarioTutor.Apellido1 += " " + horarioTutor.Apellido2;
                strHorarioTutor str = new strHorarioTutor();
                str.IdTutor = horarioTutor.IdTutor;
                str.Nombre1Tutor = horarioTutor.Nombre1;
                str.Nombre2Tutor = horarioTutor.Nombre2;
                str.Apellido1Tutor = horarioTutor.Apellido1;
                str.Apellido2Tutor = horarioTutor.Apellido2;
                strHorarioTutors.Add(str);
            }
            return Json(new { horarioTutors = strHorarioTutors });
        }

        [HttpGet] //LEER LOS HORARIOS TUTOR POR TUTOR
        public async Task<IActionResult> TraerHorariosTutores(int idTutor)
        {
            List<HorarioTutor> HorarioTutors = await _context.HorarioTutors.Where(x => x.IdTutor == idTutor).ToListAsync();
            List<strHorarioTutor> strHorarioTutors = new List<strHorarioTutor>();
            foreach (HorarioTutor horarioTutor in HorarioTutors)
            {
                //horarioTutor.Nombre1 += " " + horarioTutor.Nombre2;
                //horarioTutor.Apellido1 += " " + horarioTutor.Apellido2;
                strHorarioTutor str = new strHorarioTutor();
                str.Id = horarioTutor.Id;
                str.IdTutor = horarioTutor.IdTutor;
                str.Dia = horarioTutor.Dia;
                str.Hora = horarioTutor.Hora;
                str.Cupo = horarioTutor.Cupo;
                strHorarioTutors.Add(str);
            }
            return Json(new { horarioTutors = strHorarioTutors });
        }

        [HttpPost] //ELIMINAR HORARIO TUTOR
        public async Task<IActionResult> EliminarHorarioTutor(int id)
        {
            HorarioTutor Horario = await _context.HorarioTutors.Where(b => b.Id == id).FirstOrDefaultAsync();
            if (Horario != null)
            {
                try
                {
                    _context.Remove(Horario);
                    await _context.SaveChangesAsync();
                    return Content("eliminado");
                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }
            }
            return Content("no encontrado");
        }

        //FUNCIONES GENERALES
        [HttpGet] 
        public async Task<IActionResult> TraerUsuario(int id)
        {
            List<Usuario> usuarios = await _context.Usuarios.Include(e => e.IdTipoNavigation).Include(e => e.IdEstadoNavigation).Where(b => b.Id == id).ToListAsync();
            List<strUsuario> strUsuarios = new List<strUsuario>();
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

        [HttpGet] //LEER TABLA TIPO DOCUMENTO
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

        [HttpGet] //LEER TABLA TIPO USUARIO
        public async Task<IActionResult> TraerTipoDeUsuario()
        {
            List<TipoUsuario> tipoUsuarios = await _context.TipoUsuarios.ToListAsync();
            List<strTipoUsuario> strTipoUsuario = new List<strTipoUsuario>();
            foreach (TipoUsuario tipoUsuario in tipoUsuarios)
            {
                strTipoUsuario str = new strTipoUsuario();
                str.Id = tipoUsuario.Id;
                str.Tipo = tipoUsuario.Tipo;
                strTipoUsuario.Add(str);
            }
            return Json(new { tipoUsuarios = strTipoUsuario });
        }

        [HttpGet] //LEER TABLA MARCA VEHICULO
        public async Task<IActionResult> TraerMarca()
        {
            List<MarcaVehiculo> marcaVehiculos = await _context.MarcaVehiculos.ToListAsync();
            List<strMarca> strMarcas = new List<strMarca>();
            foreach (MarcaVehiculo marcaVehiculo in marcaVehiculos)
            {
                strMarca str = new strMarca();
                str.Id = marcaVehiculo.Id;
                str.Descripcion = marcaVehiculo.Descripcion;
                strMarcas.Add(str);
            }
            return Json(new { marcas = strMarcas });
        }
        
        [HttpGet] //LEER TABLA MODELO VEHICULO
        public async Task<IActionResult> TraerModelo()
        {
            List<ModeloVehiculo> modeloVehiculos = await _context.ModeloVehiculos.ToListAsync();
            List<strModelo> strModelos = new List<strModelo>();
            foreach (ModeloVehiculo modeloVehiculo in modeloVehiculos)
            {
                strModelo str = new strModelo();
                str.Id = modeloVehiculo.Id;
                str.Descripcion = modeloVehiculo.Descripcion;
                strModelos.Add(str);
            }
            return Json(new { modelos = strModelos });

        }

        [HttpGet] //LEER TABLA MODELO VEHICULO
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

        // FUNCIONES DE SOLICITAR CLASES 
        [HttpGet]
        public async Task<IActionResult> TraerSolicitudClases()
        {
       
            List<Clase> Clases= await _context.Clases.Include(e=> e.IdTipoNavigation).Include(e=> e.IdTutorNavigation).Include(e=> e.IdUsuarioNavigation).Include(e=> e.IdLicenciaNavigation).Include(e=> e.IdEstadoNavigation).ToListAsync();           
            List<strClase> strClases = new List<strClase>();
            foreach (Clase clase in Clases)
            {
                clase.IdTutorNavigation.Nombre1 += " " + clase.IdTutorNavigation.Nombre2;
                clase.IdTutorNavigation.Apellido1 += " " + clase.IdTutorNavigation.Apellido2;
                clase.IdUsuarioNavigation.Nombre1 += " " + clase.IdUsuarioNavigation.Nombre2;
                clase.IdUsuarioNavigation.Apellido1 += " " + clase.IdUsuarioNavigation.Apellido2;

                strClase str = new strClase();
                str.Id = clase.Id;
                str.IdTutor = clase.IdTutor;
                str.Nombre1Tutor = clase.IdTutorNavigation.Nombre1;
                str.Apellido1Tutor = clase.IdTutorNavigation.Apellido1;
                str.IdUsuario = clase.IdUsuario;
                str.IdEstado = clase.IdEstado;
                str.Nombre1Usuario = clase.IdUsuarioNavigation.Nombre1;
                str.Apellido1Usuario=clase.IdUsuarioNavigation.Apellido1;
                str.IdLicencia= clase.IdLicencia;
                str.CategoriaLicencia = clase.IdLicenciaNavigation.Categoria;
                str.IdTipo= clase.IdTipo;
                str.DescripcionEstado = clase.IdEstadoNavigation.Descripcion;
                str.FechaSolicitud = clase.FechaSolicitud.ToShortDateString();
                str.FechaFinalizacion = clase.FechaFinalizacion.ToString();
                strClases.Add(str);
            }

            return Json(new { clases = strClases });
        }

        // ESTRUCTURAS DE SOLICITUD DE CLASES 
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

        //ESTRUCTURAS DE LAS PETICIONES
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
            public int Edad { get; set; }
            public string NoDocumento { get; set; }
            public long Telefono { get; set; }
            public string Direccion { get; set; }
            public string TipoUsuario { get; set; }
            public int IdTipoDocumento { get; set; }
            public int IdTipoUsuario { get; set; }

        }
        struct strVehiculo
        {
            public int Id { get; set; }
            public int? IdMarca { get; set; }
            public string? DescripcionMarca { get; set; }
            public int? IdModelo { get; set; }
            public string? DescripcionModelo { get; set; }
            public int Cantidad { get; set; }
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
        struct strMarca
        {
            public int Id { get; set; }
            public string? Descripcion { get; set; }
        }
        struct strModelo
        {
            public int Id { get; set; }
            public string? Descripcion { get; set; }
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
