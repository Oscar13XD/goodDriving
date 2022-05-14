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
        public async Task<IActionResult> RegistrarCuestionario(int idClase, int idTutor, int idUsuario, int pregunta1, int pregunta2, int pregunta3, int pregunta4, int pregunta5,
            int pregunta6, int pregunta7, int pregunta8, int pregunta9, int pregunta10)
        {
            Usuario Usuario = await _context.Usuarios.Where(x => x.Id == idUsuario).FirstOrDefaultAsync();
            Usuario Tutor = await _context.Usuarios.Where(x => x.Id == idTutor).FirstOrDefaultAsync();
            Clase Clase = await _context.Clases.Where(x => x.Id == idClase).FirstOrDefaultAsync();

            string Fortalezas = "";
            string Debilidades = "";
            string Recomendaciones = "";

            switch (pregunta1)
            {
                case 1:
                    Fortalezas += "";
                    Debilidades += " El usuario provocaría un accidente por lo que el otro vehículo también va a velocidad y puede que no pueda reaccionar a aquel movimiento.";
                    Recomendaciones += "Abstenerse de hacer esto no es necesario bajar completamente la velocidad para dar un poco de espacio para que el otro vehículo pase.";
                    break;
                case 2:
                    Fortalezas += "";
                    Debilidades += " El usuario provocaría un accidente por lo que el otro vehículo también va a velocidad y puede que no pueda reaccionar a aquel movimiento.";
                    Recomendaciones += "Abstenerse de hacer esto no es necesario bajar completamente la velocidad para dar un poco de espacio para que el otro vehículo pase.";
                    break;
                case 3:
                    Fortalezas += " El usuario no obstruiría el paso del vehículo.";
                    Debilidades += "";
                    Recomendaciones += " El usuario debe de saber a que distancia es mejor bajar la velocidad para no provocar ningún accidente y/o choque y el otro vehículo podría pasar sin problema.";
                    break;
                default:
                    break;
            }

            switch (pregunta2)
            {
                case 1:
                    Fortalezas += " El usuario tendrá visualidad del camino.";
                    Debilidades += " No es suficiente para el usuario evitar algún tipo de accidente.";
                    Recomendaciones += " El usuario debe de controlar tanta velocidad y mejorar la precaución por lo que la luz baja no le da vista amplia del camino.";
                    break;
                case 2:
                    Fortalezas += " Es adecuado porque el ambiente no es el mejor para manejar.";
                    Debilidades += " Aunque el usuario baje la luz no puede ser suficiente.";
                    Recomendaciones += " Aunque el usuario tiene precaución la velocidad también se debe de tener en cuenta.";
                    break;
                case 3:
                    Fortalezas += " El usuario puede ser capaz de reaccionar por si hay otro vehículo o un objeto en la vía así podría evitar un accidente.";
                    Debilidades += "";
                    Recomendaciones += " Cuando niebla también podría prender sus direccionales con el fin de avisar y ser visto por un vehículo que se encuentre detrás suyo.";
                    break;
                default:
                    break;
            }

            switch (pregunta3)
            {
                case 1:
                    Fortalezas += " El usuario debe de ir con tiempo para lo que pase en el camino u pase sin salir heridos.";
                    Debilidades += " El usuario provocaría un choque de tener un vehículo demasiado cerca.";
                    Recomendaciones += " El usuario debe tener en cuenta el camino para que sea de forma adecuada el pasar por estas situaciones.";
                    break;
                case 2:
                    Fortalezas += " El usuario podría evitar que algo mas lo tome por sorpresa.";
                    Debilidades += " El usuario por solo esquivar a un animal y una persona provocaría un accidente o choque con otro carro.";
                    Recomendaciones += " El usuario debe tener en cuenta siempre el alrededor y saber esquivar de forma adecuada para evitar choques tanto con más vehículos como con cunetas y postes.";
                    break;
                case 3:
                    Fortalezas += " El usuario dejare que pase otro carro y seguir el camino de él.";
                    Debilidades += "";
                    Recomendaciones += " Dejara que pase totalmente lo que hace obstrucción porque puede ser tocado con el vehículo cuando quiera avanzar.";
                    break;
                default:
                    break;
            }

            switch (pregunta4)
            {
                case 1:
                    Fortalezas += " El conductor es el único de que se pare totalmente el vehículo.";
                    Debilidades += " Que el usuario no frene en su totalidad y puede que el choque sea más duro.";
                    Recomendaciones += " Al momento de que el usuario frena tan abruptamente se debe de tener en cuenta que en la parte de atrás puede venir alguien mas y ser chocado.";
                    break;
                case 2:
                    Fortalezas += " Para una emergencia el usuario tiene que tomar como recurso más adecuado y es pisar el closth y el freno.";
                    Debilidades += " Debe de ser al tiempo que el usuario frene de no serlo el freno no serviría adecuadamente.";
                    Recomendaciones += " Que el usuario pise con precisión ambos.";
                    break;
                case 3:
                    Fortalezas += " Si el usuario puede tener el tiempo y espacio se puede hacer adecuadamente la frenada.";
                    Debilidades += "";
                    Recomendaciones += " El usuario tiene que tener en cuenta que puede que el uso constante de los frenos los recaliente y fallen.";
                    break;
                default:
                    break;
            }

            switch (pregunta5)
            {
                case 1:
                    Fortalezas += "";
                    Debilidades += " El usuario debe respetar las normas de tránsito si no provocara muchos accidentes por falta de responsabilidad.";
                    Recomendaciones += " Conocer y respetar las normas de transito.";
                    break;
                case 2:
                    Fortalezas += "";
                    Debilidades += " El usuario al hacer impulsivo solo provocaría accidentes o chocar su vehículo.";
                    Recomendaciones += " El usuario debe de tomarse el tiempo para cualquier acción para analizar todos los aspectos antes de tomar un camino.";
                    break;
                case 3:
                    Fortalezas += " Que el conductor sea precavido sin demasiados accidentes.";
                    Debilidades += "";
                    Recomendaciones += " El usuario debe de seguir las normas para tener precaución y seguir conduciendo de la mejor manera.";
                    break;
                default:
                    break;
            }

            switch (pregunta6)
            {
                case 1:
                    Fortalezas += "";
                    Debilidades += " No le daría tiempo al usuario para reaccionar si va con mucha velocidad.";
                    Recomendaciones += " El usuario no debería de subir la velocidad a tal punto que no lo deje reaccionar y tener precaución al manejar.";
                    break;
                case 2:
                    Fortalezas += "";
                    Debilidades += " El usuario al estar cansado no podría reaccionar adecuadamente.";
                    Recomendaciones += " Cuando un usuario va a manejar debe de descansar lo necesario o tratar de dejar que alguien mas conduzca.";
                    break;
                case 3:
                    Fortalezas += "";
                    Debilidades += " El usuario no estará predispuesto ni para manejar ni reaccionar y la velocidad no le dará el tiempo ni espacio.";
                    Recomendaciones += " El usuario debe de descansar para evitar un accidente y no salir perjudicado por no tomar las mejores medidas de precaución.";
                    break;
                default:
                    break;
            }

            switch (pregunta7)
            {
                case 1:
                    Fortalezas += "";
                    Debilidades += " Al momento de tomar una vía y tengas espacio muchas personas aceleran sin tomar en cuenta que pueda travesarse algo y ocasionar un accidente.";
                    Recomendaciones += " Abstenerse de subir demasiado la velocidad.";
                    break;
                case 2:
                    Fortalezas += "";
                    Debilidades += " El usuario no esta consiente de la velocidad que esta tomando al conducir un vehículo.";
                    Recomendaciones += " Cuando manejas no tomes ni consumas sustancias psicoactivas que te hagan perder tus sentidos.";
                    break;
                case 3:
                    Fortalezas += "";
                    Debilidades += " Al momento de un usuario maneje mucho es agotador y al estar ebrio no estaría predispuesto a reaccionar y al ir rápido seria provocar un accidente.";
                    Recomendaciones += " Si aquella persona va a manejar que no tome y si toma que alguien maneje es lo más ideal.";
                    break;
                default:
                    break;
            }

            switch (pregunta8)
            {
                case 1:
                    Fortalezas += " La mejor manera para que el usuario este bien es dormir para que se le pase el cansancio.";
                    Debilidades += " No sería una solución total para el usuario porque no dormirá lo suficiente para volver a manejar.";
                    Recomendaciones += " Que duerma demasiado o lo suficiente que descanse un día para estar realmente en sus 5 sentidos.";
                    break;
                case 2:
                    Fortalezas += "";
                    Debilidades += " Al usuario el café no es una sustancia que ayude frente al estado de ebriedad.";
                    Recomendaciones += " Que el usuario no maneje es la mejor manera.";
                    break;
                case 3:
                    Fortalezas += "";
                    Debilidades += "";
                    Recomendaciones += " Si aquella persona va a manejar que no tome y si toma que alguien maneje es lo más ideal.";
                    break;
                default:
                    break;
            }

            switch (pregunta9)
            {
                case 1:
                    Fortalezas += " Al usuario lo mejor que puede hacer es detenerse si se siente cansado.";
                    Debilidades += "";
                    Recomendaciones += " Parar al sentirse demasiado cansado y descansar.";
                    break;
                case 2:
                    Fortalezas += "";
                    Debilidades += " Al comer el usuario da más cansancio.";
                    Recomendaciones += " Descansar lo suficiente para poder continuar con el viaje.";
                    break;
                case 3:
                    Fortalezas += " Lo mejor que puede hacer el usuario es parar no provocar ningún accidente.";
                    Debilidades += "";
                    Recomendaciones += " Antes de tomar camino, sentirse bien y tomar, comer algo que les de energía, para que puedan reaccionar y afrontar alguna circunstancia.";
                    break;
                default:
                    break;
            }

            switch (pregunta10)
            {
                case 1:
                    Fortalezas += " Que el usuario les avisara a otros conductores de lo que está aconteciendo.";
                    Debilidades += " Al usuario hacer con las manos las señales no siempre se le presta mucha atención.";
                    Recomendaciones += " Buscar un lugar adecuado para hacer que el vehículo tenga que ir frenando, no está demás siempre estar pendiente de frenos y componentes que hagan que el vehículo falle.";
                    break;
                case 2:
                    Fortalezas += " Lo mejor que puede hacer el usuario es disminuir la velocidad ayuda en el momento en que no se tenga control total de lo freno.";
                    Debilidades += " Sería algo difícil y es disminuir la velocidad porque sin frenos resulta más difícil disminuir la velocidad.";
                    Recomendaciones += " Tener precaución con la velocidad y verificar los frenos con constancia.";
                    break;
                case 3:
                    Fortalezas += " Que el usuario no obstruya el paso de otros vehículos.";
                    Debilidades += " A veces las carreteras no son tan poco concurridas y tiende a ser complicado hacerse en la parte donde no haya vehículos.";
                    Recomendaciones += " Al momento de que pase esta situación lo mejor es ir avisando a aquellas personas que van detrás y adelante de ser posible, para no provocar un accidente mucho mayor.";
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
                try
                {
                    Clase.ReportePdf = "FormatoPrueba/DocumentosPruebaPDF/"+Usuario.NoDocumento+ "/PruebaGooddriving.pdf";
                    _context.Update(Clase);
                    await _context.SaveChangesAsync();
                    return Content("Exito");
                    
                }
                catch (Exception ex)
                {
                    return Content(ex.Message);

                }
            }
            return Content("no se envio");
        }

        private bool SendEmail(Usuario Usuario)
        {
            //string Destinatario = Usuario.Email;
            string Destinatario = "solanyimilena97@gmail.com";
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
