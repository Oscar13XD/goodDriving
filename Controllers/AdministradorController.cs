using GoodDriving.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Security.Cryptography;

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

        [HttpGet]
        public async Task<IActionResult> TraerUsuarios()
        {
            List<Usuario> Usuarios = await _context.Usuarios.Include(e => e.IdTipoNavigation).Include(e => e.IdEstadoNavigation).ToListAsync();
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
                strUsuarios.Add(str);
            }
            return Json(new { usuarios = strUsuarios });
        }
        [HttpGet]
        public async Task<IActionResult> TraerVehiculos()
        {
            List<Vehiculo> Vehiculos = await _context.Vehiculos.Include(e => e.IdMarcaNavigation).Include(e => e.IdModeloNavigation).ToListAsync();
            List<strVehiculo> strVehiculos = new List<strVehiculo>();
            foreach (Vehiculo Vehiculo in Vehiculos)
            {
                strVehiculo str = new strVehiculo();
                str.Id = Vehiculo.Id;
                str.Cantidad=Vehiculo.Cantidad;
                str.IdMarca=Vehiculo.IdMarca;
                str.IdModelo=Vehiculo.IdModelo;
                str.DescripcionMarca = Vehiculo.IdMarcaNavigation.Descripcion;
                str.DescripcionModelo = Vehiculo.IdModeloNavigation.Descripcion;
                strVehiculos.Add(str);
            }
            return Json(new { vehiculos = strVehiculos });
        }
        [HttpGet]
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
        [HttpGet]
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
        [HttpPost]
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

        [HttpPost]
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

        [HttpPost]
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

        [HttpGet]
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
        [HttpGet]
        public async Task<IActionResult> TraerVehiculoU(int id)
        {
            List<Vehiculo> Vehiculos= await _context.Vehiculos.Include(e => e.IdMarcaNavigation).Include(e => e.IdModeloNavigation).Where(b => b.Id == id).ToListAsync();
            List<strVehiculo> strVehiculos = new  List<strVehiculo>();
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
    
        [HttpPost]
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

        public int sumar()
        {
            int a = 5;
            int b = 6;
            return a + b;
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
