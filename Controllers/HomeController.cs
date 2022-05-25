using GoodDriving.Models;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Net.Mail;
using System.Net;
using Syncfusion.Pdf;

namespace GoodDriving.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Sobre()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /*public IActionResult PruebaPDF()
        {
            return View();
        }

        public IActionResult CreateDocument()
        {
            using (FileStream fileStreamPath = new FileStream(Path.GetFullPath(@"FormatoPrueba/formatoPrueba.docx"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                //Creates a new Word document.
                using (WordDocument document = new WordDocument(fileStreamPath, FormatType.Automatic))
                {
                    //BUSCAMOS PALABRA 
                    TextSelection NombreUsuario = document.Find("<<Fortalezas>>", false, true);
                    //OBTENEMOS TEXTO COMO ÚNICO RANGO DE TEXTO
                    WTextRange NombreUsuarioRange = NombreUsuario.GetAsOneRange();
                    NombreUsuarioRange.Text = "La fortaleza eres tu";
                    using (DocIORenderer renderer = new DocIORenderer())
                    {
                        //Sets Chart rendering Options.
                        //Converts Word document into PDF document.
                        using (PdfDocument pdfDocument = renderer.ConvertToPDF(document))
                        {
                            //Saves the PDF file to file system.    
                            using (FileStream outputStream = new FileStream(Path.GetFullPath(@"WordToPDF.pdf"), FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                            {
                                pdfDocument.Save(outputStream);
                            }
                        }
                    }
                    SendEmail();
                    


                }
            }
            return Content("si");
        }

        private bool SendEmail()
        {
            string Destinatario = "hernandezmahechaoscar0@gmail.com";
            string urlDomain = "http://localhost:5204/";
            string EmailOrigen = "gooddriving2022@gmail.com";
            string Password = "proyecto2022*";
            Attachment Archivo = new Attachment(Path.GetFullPath(@"WordToPDF.pdf"));
            //FileStream outputFileStream = new FileStream(Path.GetFullPath(@"FormatoPrueba/Result.docx"), FileMode.Create, FileAccess.ReadWrite);
            //NOMBRE MENSAJE
            string Nombre = "Good Driving";

            string Cuerpo = "" +
                 "<div>" +
                    "<a href=\"\">Click Aqui</a>" +
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
        }*/
        }
    }