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



namespace gaco_api.Utilerias
{
    public class NotificacionCorreo
    {

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


        public static void Send(ClsModCorreo Correo, string ContentRootPath)
        {
            // Crear un documento HTML que luego convertiremos a PDF
            var PathPlantilla = Path.Combine(ContentRootPath, "Files", "solicitudInvestigacion.html");
            var srcImage = Path.Combine(ContentRootPath, "Image", "Gaco.jpeg");
            ClsNegGenerarPDF objNegGenerarPDF = new ClsNegGenerarPDF();
            string strTexto = objNegGenerarPDF.ExportPDFPlantillaSolicitudInvestigacionpdf("Pruebas luis", PathPlantilla, srcImage);
            //var htmlContent = "<h1>Este es un correo con un archivo adjunto</h1><p>Este archivo es un PDF generado a partir de HTML.</p>";
            var htmlContent = strTexto;

            // Convertir HTML a PDF usando DinkToPdf
            var converter = new BasicConverter(new PdfTools());
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = { ColorMode = ColorMode.Color, Orientation = Orientation.Portrait },
                Objects = { new ObjectSettings { HtmlContent = htmlContent, WebSettings = { DefaultEncoding = "utf-8" } } }
            };
        

            // Guardar el PDF en un archivo
        
            string filePath = PathPlantilla;
            byte[] pdf = converter.Convert(doc);
            //File.WriteAllBytes(filePath, pdf);

            // Crear el mensaje de correo con MimeKit
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(System.Text.Encoding.UTF8, "", Correo.strFrom));
            message.To.Add(new MailboxAddress(System.Text.Encoding.UTF8, "", Correo.strTo));
            message.Subject = "Correo con archivo adjunto";

            // Crear el cuerpo del mensaje en formato HTML
            var bodyBuilder = new BodyBuilder { HtmlBody = htmlContent };

            // Adjuntar el archivo PDF
            //bodyBuilder.Attachments.Add(filePath);
            bodyBuilder.Attachments.Add(fileName: "Archivo_1.pdf",
                            data: pdf,
                            contentType: MimeKit.ContentType.Parse(MediaTypeNames.Application.Pdf));

            // Asignar el cuerpo del correo
            message.Body = bodyBuilder.ToMessageBody();

            // Enviar el correo utilizando MailKit (SMTP)
            using (var clientimp = new MailKit.Net.Smtp.SmtpClient())
            {
                clientimp.Timeout = 200000;

                clientimp.Connect(Correo.strHost, (int)Correo.intPuerto, SecureSocketOptions.StartTls);
                clientimp.Authenticate(Correo.strFrom, Correo.strPassword);
                clientimp.Send(message);
                clientimp.Disconnect(true);
            }
            // Eliminar el archivo PDF después de enviarlo (opcional)
            //File.Delete(filePath);
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
