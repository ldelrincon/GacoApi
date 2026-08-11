using System.Net.Mail;
//using MimeKit;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using MimeKit;
using MailKit.Security;
using System.Configuration;
using gaco_api.Models;
using ClbNegGestores;
using static gaco_api.Models.ClsModCorreo;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Org.BouncyCastle.Utilities;
using Microsoft.AspNetCore.Html;
using System.Web;
using System.IO;
using System.Net.Mime;
using DinkToPdf;
using HtmlAgilityPack;
using gaco_api.Models.DTOs.Responses.ReporteSolicitudes;
using gaco_api.Models.DTOs.Requests.Evidencias;
using gaco_api.Customs;


namespace gaco_api.Utilerias
{
    public class NotificacionCorreo
    {
        private readonly Utilidades _utilidades;

        public NotificacionCorreo(Utilidades Utilidades)
        {
            _utilidades = Utilidades;
        }
        public static void Send2(ClsModCorreo Correo, string ContentRootPath)
        {


            MimeMessage mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(System.Text.Encoding.UTF8, "", Correo.strFrom));
            mimeMessage.To.Add(new MailboxAddress(System.Text.Encoding.UTF8, "", Correo.strTo));

            if (Correo.strCC != "")
            {
                mimeMessage.Cc.Add(new MailboxAddress(System.Text.Encoding.UTF8, "", Correo.strCC));
            }

            mimeMessage.Subject = Correo.strSubject;
            // mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = Correo.strBody };
            // Crear el cuerpo del mensaje
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = Correo.strBody // Asignar el cuerpo HTML
            };

            //convertir hmtl a pdf
            var PathPlantilla = Path.Combine(ContentRootPath, "Files", "solicitudInvestigacion.html");

            //string base64String = ClsNegGenerarPDF.ExportPDFPlantillaSolicitudInvestigacion("pruebas luis", PathPlantilla);
            //byte[] pdfBytes = ClsNegGenerarPDF.ExportPDFPlantillaSolicitudInvestigacionpdf("pruebas luis", PathPlantilla);
            byte[] pdfBytes = null;
            ClsModAttachment ModAttachment = new();
            try
            {
                if (pdfBytes != null)
                {
                    //byte[] pdfBytes = Convert.FromBase64String(base64String);
                    ModAttachment.FileName = "Solicitud Facturacion";
                    ModAttachment.ContentType = "application/pdf";
                    ModAttachment.FileContent = pdfBytes;
               
                    //bodyBuilder.Attachments.Add("Receipt.pdf", pdfBytes);

                    bodyBuilder.Attachments.Add(fileName: "Archivo_1.pdf",
                            data: pdfBytes,
                            contentType: MimeKit.ContentType.Parse(MediaTypeNames.Application.Pdf));


                }
            
                mimeMessage.Body = bodyBuilder.ToMessageBody();


                using (var clientimp = new MailKit.Net.Smtp.SmtpClient())
                {
                    
                    clientimp.Timeout = 200000;
                   
                    clientimp.Connect(Correo.strHost, (int)Correo.intPuerto, SecureSocketOptions.StartTls);
                    clientimp.Authenticate(Correo.strFrom, Correo.strPassword);
                    clientimp.Send(mimeMessage);
                    clientimp.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
            }
        }


        public void Send(ClsModCorreo Correo, string ContentRootPath, ReporteServicioResponse objReporteServicioResponse)
        {
            SendAsync(Correo, ContentRootPath, objReporteServicioResponse).GetAwaiter().GetResult();
        }

        public async Task SendAsync(ClsModCorreo Correo, string ContentRootPath, ReporteServicioResponse objReporteServicioResponse)
        {
            try
            {
                var PathPlantilla = Path.Combine(ContentRootPath, "Files", "solicitudInvestigacion.html");
                var srcImage = Path.Combine(ContentRootPath, "Image", "Gaco.jpeg");
                var objNegGenerarPDF = new ClsNegGenerarPDF();
                var strTexto = objNegGenerarPDF.ExportPDFPlantillaSolicitudInvestigacionpdf("Pruebas luis", PathPlantilla, srcImage, objReporteServicioResponse);
                var htmlContent = strTexto;

                var converter = new BasicConverter(new PdfTools());
                var doc = new HtmlToPdfDocument()
                {
                    GlobalSettings = { ColorMode = ColorMode.Color, Orientation = Orientation.Portrait },
                    Objects = { new ObjectSettings { HtmlContent = htmlContent, WebSettings = { DefaultEncoding = "utf-8" } } }
                };

                var pdf = await Task.Run(() => converter.Convert(doc));

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(System.Text.Encoding.UTF8, "", Correo.strFrom));
                message.To.Add(new MailboxAddress(System.Text.Encoding.UTF8, "", Correo.strTo));
                message.Subject = "Seguimiento para facturación de cliente: " + objReporteServicioResponse.Cliente + "";

                if (!string.IsNullOrEmpty(Correo.strCC))
                {
                    foreach (var cc in Correo.strCC.Split(',', ';'))
                    {
                        var ccTrimmed = cc.Trim();
                        if (!string.IsNullOrEmpty(ccTrimmed))
                            message.Cc.Add(new MailboxAddress(System.Text.Encoding.UTF8, "", ccTrimmed));
                    }
                }

                var bodyBuilder = new BodyBuilder { TextBody = Correo.strBody };

                foreach (var objEvidencias in objReporteServicioResponse.Evidencias.Where(x => x.Extension.Contains("pdf")).ToList())
                {
                    var Ruta = _utilidades.GetPhysicalPath(objEvidencias.Ruta);
                    if (File.Exists(Ruta) && !string.IsNullOrEmpty(Ruta))
                    {
                        var Base64pdf = _utilidades.GetFileBytes(Ruta);
                        bodyBuilder.Attachments.Add(fileName: objEvidencias.Nombre,
                            data: Base64pdf,
                            contentType: MimeKit.ContentType.Parse(MediaTypeNames.Application.Pdf));
                    }
                }

                bodyBuilder.Attachments.Add(fileName: "Seguimiento.pdf",
                    data: pdf,
                    contentType: MimeKit.ContentType.Parse(MediaTypeNames.Application.Pdf));

                message.Body = bodyBuilder.ToMessageBody();

                using (var clientimp = new MailKit.Net.Smtp.SmtpClient())
                {
                    clientimp.Timeout = 200000;
                    await clientimp.ConnectAsync(Correo.strHost, (int)Correo.intPuerto, SecureSocketOptions.StartTls);
                    await clientimp.AuthenticateAsync(Correo.strFrom, Correo.strPassword);
                    await clientimp.SendAsync(message);
                    await clientimp.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public void SendProyecto(ClsModCorreo Correo, string ContentRootPath, ReporteServicioResponse objReporteServicioResponse)
        {
            SendProyectoAsync(Correo, ContentRootPath, objReporteServicioResponse).GetAwaiter().GetResult();
        }

        public async Task SendProyectoAsync(ClsModCorreo Correo, string ContentRootPath, ReporteServicioResponse objReporteServicioResponse)
        {
            try
            {
                var PathPlantilla = Path.Combine(ContentRootPath, "Files", "solicitudInvestigacionProyecto.html");
                var srcImage = Path.Combine(ContentRootPath, "Image", "Gaco.jpeg");
                var objNegGenerarPDF = new ClsNegGenerarPDF();
                var strTexto = objNegGenerarPDF.ExportPDFPlantillaSolicitudInvestigacionProyectopdf("Pruebas luis", PathPlantilla, srcImage, objReporteServicioResponse);
                var htmlContent = strTexto;

                var converter = new BasicConverter(new PdfTools());
                var doc = new HtmlToPdfDocument()
                {
                    GlobalSettings = { ColorMode = ColorMode.Color, Orientation = Orientation.Portrait },
                    Objects = { new ObjectSettings { HtmlContent = htmlContent, WebSettings = { DefaultEncoding = "utf-8" } } }
                };

                var pdf = await Task.Run(() => converter.Convert(doc));

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(System.Text.Encoding.UTF8, "", Correo.strFrom));
                message.To.Add(new MailboxAddress(System.Text.Encoding.UTF8, "", Correo.strTo));
                message.Subject = "Seguimiento para facturación de cliente: " + objReporteServicioResponse.Cliente + "";

                if (!string.IsNullOrEmpty(Correo.strCC))
                {
                    foreach (var cc in Correo.strCC.Split(',', ';'))
                    {
                        var ccTrimmed = cc.Trim();
                        if (!string.IsNullOrEmpty(ccTrimmed))
                            message.Cc.Add(new MailboxAddress(System.Text.Encoding.UTF8, "", ccTrimmed));
                    }
                }

                var bodyBuilder = new BodyBuilder { TextBody = Correo.strBody };

                foreach (var objEvidencias in objReporteServicioResponse.Evidencias.Where(x => x.Extension.Contains("pdf")).ToList())
                {
                    var Ruta = _utilidades.GetPhysicalPath(objEvidencias.Ruta);
                    if (File.Exists(Ruta) && !string.IsNullOrEmpty(Ruta))
                    {
                        var Base64pdf = _utilidades.GetFileBytes(Ruta);
                        bodyBuilder.Attachments.Add(fileName: objEvidencias.Nombre,
                            data: Base64pdf,
                            contentType: MimeKit.ContentType.Parse(MediaTypeNames.Application.Pdf));
                    }
                }

                bodyBuilder.Attachments.Add(fileName: "Seguimiento.pdf",
                    data: pdf,
                    contentType: MimeKit.ContentType.Parse(MediaTypeNames.Application.Pdf));

                message.Body = bodyBuilder.ToMessageBody();

                using (var clientimp = new MailKit.Net.Smtp.SmtpClient())
                {
                    clientimp.Timeout = 200000;
                    await clientimp.ConnectAsync(Correo.strHost, (int)Correo.intPuerto, SecureSocketOptions.StartTls);
                    await clientimp.AuthenticateAsync(Correo.strFrom, Correo.strPassword);
                    await clientimp.SendAsync(message);
                    await clientimp.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }






        //public async Task<Contentemail> sendMasive(CorreoModel correo)
        //public void sendMasive(ClsModCorreo correo)
        //{
        //    //string errorMessage = "";
        //    // string StrFrom = correo.strFrom;
        //    // string StrBody = correo.strBody; 
        //    // HttpClient client = new HttpClient();
        //    // client.DefaultRequestHeaders.Accept.Clear();
        //    // client.DefaultRequestHeaders.Add("Accept", "application/json");
        //    // client.DefaultRequestHeaders.Add("MailPace-Server-Token", correo.strPassword);
        //    // Contentemail contentemail = new Contentemail();
        //    // contentemail.from = StrFrom;
        //    // contentemail.to = correo.strTo;
        //    // contentemail.subject = correo.strSubject;
        //    // contentemail.htmlbody = StrBody;  
        //    // HttpResponseMessage response = await client.PostAsJsonAsync("https://app.mailpace.com/api/v1/send", contentemail);
        //    // string statusCode = response.ToString().Substring(0, 15);
        //    // statusCode = statusCode.Substring(12);
        //    // switch (statusCode)
        //    // {
        //    //     case "200":
        //    //         errorMessage = "Envio correcto de correos";
        //    //         break;
        //    //     case "400":
        //    //         errorMessage = "error : Invalid API Token | Email from address not parseable | to : [undefined field] | to : [is invalid] | to: [contains a blocked address | to : [number of email addresses exceeds maximum volume] | attachments.name : [Extension file type blocked, see Docs for full list of allowed file types | ";
        //    //         break;
        //    //     case "401":
        //    //         errorMessage = "error : Missing API Token";
        //    //         break;
        //    //     case "403":
        //    //         errorMessage = "error : Domain DKIM DNS not verified, please complete DKIM Verification | The organization that owns this domain does not have an active plan | This organization is unable to send emails. Please contact support | Verified domain does not match domain in From address of email";
        //    //         break;
        //    //     case "406":
        //    //         errorMessage = "error : Invalid request format or content type";
        //    //         break;
        //    //     case "429":
        //    //         errorMessage = "error : You are sending emails too quickly";
        //    //         break;
        //    //     case "500":
        //    //         errorMessage = "error : No content";
        //    //         break;
        //    //     default:
        //    //         errorMessage = "error : Undefined";
        //    //         break;
        //    // } 
        //    // contentemail.errorMessage = errorMessage;
        //    // contentemail.IsSuccessStatusCode = response.IsSuccessStatusCode;

        //    //return contentemail;
        //    MimeMessage mimeMessage = new MimeMessage();
        //    mimeMessage.From.Add(new MailboxAddress(System.Text.Encoding.UTF8, "", correo.strFrom));
        //    mimeMessage.To.Add(new MailboxAddress(System.Text.Encoding.UTF8, "", correo.strTo));
        //    if (correo.ListstrTo != null && correo.ListstrTo.Count > 0)
        //    {
        //        correo.ListstrTo.ForEach(x => mimeMessage.To.Add(new MailboxAddress(System.Text.Encoding.UTF8, "", x)));
        //    }
        //    if (correo.strCC != "")
        //    {
        //        mimeMessage.Cc.Add(new MailboxAddress(System.Text.Encoding.UTF8, "", correo.strCC));
        //    }

        //    mimeMessage.Subject = correo.strSubject;
        //    mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = correo.strBody };

        //    using (var clientimp = new MailKit.Net.Smtp.SmtpClient())
        //    {
        //        clientimp.Timeout = 200000;

        //        clientimp.Connect(Correo.strHost, (int)Correo.intPuerto, SecureSocketOptions.StartTls);
        //        clientimp.Authenticate(Correo.strFrom, Correo.strPassword);
        //        clientimp.Send(mimeMessage);
        //        clientimp.Disconnect(true);
        //    }
        //}
    }
}
