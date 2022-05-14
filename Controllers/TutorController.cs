using GoodDriving.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.Pdf;
using System.Net;
using System.Net.Mail;

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
                str.IdUsuario = clase.IdUsuarioNavigation.Id;
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
        public async Task<IActionResult> RegistrarCuestionario(int idTutor, int idUsuario, int pregunta1, int pregunta2, int pregunta3, int pregunta4, int pregunta5,
            int pregunta6, int pregunta7, int pregunta8, int pregunta9, int pregunta10)
        {
            Usuario Usuario = await _context.Usuarios.Where(x => x.Id == idUsuario).FirstOrDefaultAsync();
            Usuario Tutor = await _context.Usuarios.Where(x => x.Id == idTutor).FirstOrDefaultAsync();

            string Fortalezas = "";
            string Debilidades = "";
            string Recomendaciones = "";

            switch (pregunta1)
            {
                case 1:
                    Fortalezas += "";
                    Debilidades += "El usuario provocaría un accidente por lo que el otro vehículo también va a velocidad y puede que no pueda reaccionar a aquel movimiento. </br>";
                    Recomendaciones += "Abstenerse de hacer esto no es necesario bajar completamente la velocidad para dar un poco de espacio para que el otro vehículo pase. </br>";
                    break;
                case 2:
                    Fortalezas += "";
                    Debilidades += "El usuario provocaría un accidente por lo que el otro vehículo también va a velocidad y puede que no pueda reaccionar a aquel movimiento.";
                    Recomendaciones += "Abstenerse de hacer esto no es necesario bajar completamente la velocidad para dar un poco de espacio para que el otro vehículo pase.";
                    break;
                case 3:
                    Fortalezas += "El usuario no obstruiría el paso del vehículo";
                    Debilidades += "";
                    Recomendaciones += "El usuario debe de saber a que distancia es mejor bajar la velocidad para no provocar ningún accidente y/o choque y el otro vehículo podría pasar sin problema.";
                    break;
                default:
                    break;
            }

            switch (pregunta2)
            {
                case 1:
                    Fortalezas += "";
                    Debilidades += "";
                    Recomendaciones += "";
                    break;
                case 2:
                    Fortalezas += "";
                    Debilidades += "";
                    Recomendaciones += "";
                    break;
                case 3:
                    Fortalezas += "";
                    Debilidades += "";
                    Recomendaciones += "";
                    break;
                default:
                    break;
            }

            switch (pregunta3)
            {
                case 1:
                    Fortalezas += "";
                    Debilidades += "";
                    Recomendaciones += "";
                    break;
                case 2:
                    Fortalezas += "";
                    Debilidades += "";
                    Recomendaciones += "";
                    break;
                case 3:
                    Fortalezas += "";
                    Debilidades += "";
                    Recomendaciones += "";
                    break;
                default:
                    break;
            }

            switch (pregunta4)
            {
                case 1:
                    Fortalezas += "";
                    Debilidades += "";
                    Recomendaciones += "";
                    break;
                case 2:
                    Fortalezas += "";
                    Debilidades += "";
                    Recomendaciones += "";
                    break;
                case 3:
                    Fortalezas += "";
                    Debilidades += "";
                    Recomendaciones += "";
                    break;
                default:
                    break;
            }

            switch (pregunta5)
            {
                case 1:
                    Fortalezas += "";
                    Debilidades += "";
                    Recomendaciones += "";
                    break;
                case 2:
                    Fortalezas += "";
                    Debilidades += "";
                    Recomendaciones += "";
                    break;
                case 3:
                    Fortalezas += "";
                    Debilidades += "";
                    Recomendaciones += "";
                    break;
                default:
                    break;
            }

            switch (pregunta6)
            {
                case 1:
                    Fortalezas += "";
                    Debilidades += "";
                    Recomendaciones += "";
                    break;
                case 2:
                    Fortalezas += "";
                    Debilidades += "";
                    Recomendaciones += "";
                    break;
                case 3:
                    Fortalezas += "";
                    Debilidades += "";
                    Recomendaciones += "";
                    break;
                default:
                    break;
            }

            switch (pregunta7)
            {
                case 1:
                    Fortalezas += "";
                    Debilidades += "";
                    Recomendaciones += "";
                    break;
                case 2:
                    Fortalezas += "";
                    Debilidades += "";
                    Recomendaciones += "";
                    break;
                case 3:
                    Fortalezas += "";
                    Debilidades += "";
                    Recomendaciones += "";
                    break;
                default:
                    break;
            }

            switch (pregunta8)
            {
                case 1:
                    Fortalezas += "";
                    Debilidades += "";
                    Recomendaciones += "";
                    break;
                case 2:
                    Fortalezas += "";
                    Debilidades += "";
                    Recomendaciones += "";
                    break;
                case 3:
                    Fortalezas += "";
                    Debilidades += "";
                    Recomendaciones += "";
                    break;
                default:
                    break;
            }

            switch (pregunta9)
            {
                case 1:
                    Fortalezas += "";
                    Debilidades += "";
                    Recomendaciones += "";
                    break;
                case 2:
                    Fortalezas += "";
                    Debilidades += "";
                    Recomendaciones += "";
                    break;
                case 3:
                    Fortalezas += "";
                    Debilidades += "";
                    Recomendaciones += "";
                    break;
                default:
                    break;
            }

            switch (pregunta10)
            {
                case 1:
                    Fortalezas += "";
                    Debilidades += "";
                    Recomendaciones += "";
                    break;
                case 2:
                    Fortalezas += "";
                    Debilidades += "";
                    Recomendaciones += "";
                    break;
                case 3:
                    Fortalezas += "";
                    Debilidades += "";
                    Recomendaciones += "";
                    break;
                default:
                    break;
            }

            using (FileStream fileStreamPath = new FileStream(Path.GetFullPath(@"FormatoPrueba/formatoPrueba.docx"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (WordDocument document = new WordDocument(fileStreamPath, FormatType.Automatic))
                {
                    //BUSCAMOS PALABRA 
                    TextSelection Nombre1Tutor = document.Find("<<Nombre1Tutor>>", false, true);
                    //OBTENEMOS TEXTO COMO ÚNICO RANGO DE TEXTO
                    WTextRange Nombre1TutorRange = Nombre1Tutor.GetAsOneRange();
                    Nombre1TutorRange.Text = Tutor.Nombre1;

                    //BUSCAMOS PALABRA 
                    TextSelection Nombre2Tutor = document.Find("<<Nombre2Tutor>>", false, true);
                    //OBTENEMOS TEXTO COMO ÚNICO RANGO DE TEXTO
                    WTextRange Nombre2TutorRange = Nombre1Tutor.GetAsOneRange();
                    if (Tutor.Nombre2 != null)
                    {
                        Nombre2TutorRange.Text = Tutor.Nombre2;
                    }
                    else
                    {
                        Nombre2TutorRange.Text = "";
                    }

                    //BUSCAMOS PALABRA 
                    TextSelection Apellido1Tutor = document.Find("<<Apellido1Tutor>>", false, true);
                    //OBTENEMOS TEXTO COMO ÚNICO RANGO DE TEXTO
                    WTextRange Apellido1TutorRange = Apellido1Tutor.GetAsOneRange();
                    Apellido1TutorRange.Text = Tutor.Apellido1;

                    //BUSCAMOS PALABRA 
                    TextSelection Apellido2Tutor = document.Find("<<Apellido2Tutor>>", false, true);
                    //OBTENEMOS TEXTO COMO ÚNICO RANGO DE TEXTO
                    WTextRange Apellido2TutorRange = Apellido2Tutor.GetAsOneRange();
                    Apellido2TutorRange.Text = Tutor.Apellido2;

                    //BUSCAMOS PALABRA 
                    TextSelection Nombre1Usuario = document.Find("<<Nombre1Usuario>>", false, true);
                    //OBTENEMOS TEXTO COMO ÚNICO RANGO DE TEXTO
                    WTextRange Nombre1UsuarioRange = Nombre1Usuario.GetAsOneRange();
                    Nombre1UsuarioRange.Text = Usuario.Nombre1;

                    //BUSCAMOS PALABRA 
                    TextSelection Nombre2Usuario = document.Find("<<Nombre2Usuario>>", false, true);
                    //OBTENEMOS TEXTO COMO ÚNICO RANGO DE TEXTO
                    WTextRange Nombre2UsuarioRange = Nombre1Usuario.GetAsOneRange();
                    Nombre2UsuarioRange.Text = Usuario.Nombre2;

                    //BUSCAMOS PALABRA 
                    TextSelection Apellido1Usuario = document.Find("<<Apellido1Usuario>>", false, true);
                    //OBTENEMOS TEXTO COMO ÚNICO RANGO DE TEXTO
                    WTextRange Apellido1UsuarioRange = Apellido1Usuario.GetAsOneRange();
                    Apellido1UsuarioRange.Text = Usuario.Apellido1;

                    //BUSCAMOS PALABRA 
                    TextSelection Apellido2Usuario = document.Find("<<Apellido2Usuario>>", false, true);
                    //OBTENEMOS TEXTO COMO ÚNICO RANGO DE TEXTO
                    WTextRange Apellido2UsuarioRange = Apellido2Usuario.GetAsOneRange();
                    Apellido2UsuarioRange.Text = Usuario.Apellido2;

                    //BUSCAMOS PALABRA 
                    TextSelection fortalezas = document.Find("<<Fortalezas>>", false, true);
                    //OBTENEMOS TEXTO COMO ÚNICO RANGO DE TEXTO
                    WTextRange FortalezasRange = fortalezas.GetAsOneRange();
                    FortalezasRange.Text = Fortalezas;

                    //BUSCAMOS PALABRA 
                    TextSelection debilidades = document.Find("<<debilidades>>", false, true);
                    //OBTENEMOS TEXTO COMO ÚNICO RANGO DE TEXTO
                    WTextRange debilidadesRange = debilidades.GetAsOneRange();
                    debilidadesRange.Text = Debilidades;

                    //BUSCAMOS PALABRA 
                    TextSelection recomendaciones = document.Find("<<recomendaciones>>", false, true);
                    //OBTENEMOS TEXTO COMO ÚNICO RANGO DE TEXTO
                    WTextRange recomendacionesRange = recomendaciones.GetAsOneRange();
                    recomendacionesRange.Text = Recomendaciones;

                    Directory.CreateDirectory(Path.GetFullPath(@"FormatoPrueba/DocumentosPruebaPDF/" + Usuario.NoDocumento));
                    
                    using (DocIORenderer renderer = new DocIORenderer())
                    {
                        using (PdfDocument pdfDocument = renderer.ConvertToPDF(document))
                        {
                            using (FileStream outputStream = new FileStream(Path.GetFullPath(@"FormatoPrueba/DocumentosPruebaPDF/"+ Usuario.NoDocumento + "/PruebaGooddriving.pdf"), FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                            {
                                pdfDocument.Save(outputStream);
                            }
                        }
                    }
                }
            }
            bool correo = SendEmail(Usuario);
            if (correo)
            {
                return Content("Enviado");
            }
            return Content("no se envio");
        }

        private bool SendEmail(Usuario Usuario)
        {
            string Destinatario = "nathbernalc@gmail.com";
            //string urlDomain = "http://localhost:5204/";
            string EmailOrigen = "gooddriving2022@gmail.com";
            string Password = "proyecto2022*";
            Attachment Archivo = new Attachment(Path.GetFullPath(@"FormatoPrueba/DocumentosPruebaPDF/" + Usuario.NoDocumento + "/PruebaGooddriving.pdf"));
            //FileStream outputFileStream = new FileStream(Path.GetFullPath(@"FormatoPrueba/Result.docx"), FileMode.Create, FileAccess.ReadWrite);
            //NOMBRE MENSAJE
            string Nombre = "Good Driving";
            string Cuerpo = "";
            string Asunto = "Prueba GoodDriving";

            var mail = new MailMessage()
            {
                From = new MailAddress(EmailOrigen, Nombre),
                Subject = Asunto,
                Body = Cuerpo,
                BodyEncoding = System.Text.Encoding.UTF8,
                SubjectEncoding = System.Text.Encoding.Default,
                IsBodyHtml = true,

            };
            //AÑADIMOS DOCUMENTO ADJUNTO
            mail.Attachments.Add(Archivo);


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
                return true;
            }
            catch (Exception ex)
            {
                //ENVIAMOS POR CORREO MENSAJES DE ERROR
                Console.WriteLine(ex.ToString());
                return false;
            }
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
