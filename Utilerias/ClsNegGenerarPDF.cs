using System.Configuration;
using System.Text;
using DinkToPdf;
using HtmlAgilityPack;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Html;
using MimeKit;
using OpenHtmlToPdf;
namespace ClbNegGestores
{
    public class ClsNegGenerarPDF
    {
        public static string ExportPDFPlantillaSolicitudInvestigacion(string Folio, string Path)
        {
            HtmlDocument doc = new HtmlDocument();
            // Crear el objeto para la conversión
        


            string base64 = "";
            try
            {
                string RutaCorreosNezterLoanding = $"";
                doc.Load(Path);
                
                doc.GetElementbyId("folio").InnerHtml =Folio;
                //doc.GetElementbyId("folio").InnerHtml = objPlantilla.FolioSolicitud;
                //doc.GetElementbyId("MunicipioTitulo").InnerHtml = objPlantilla.Municipio;
                //doc.GetElementbyId("EstadoTitulo").InnerHtml = objPlantilla.Estado;
                //doc.GetElementbyId("Estado").InnerHtml = objPlantilla.Estado;
                //doc.GetElementbyId("TipoInscripcion").InnerHtml = objPlantilla.TipoTransaccion;
                //doc.GetElementbyId("FechaInscripcion").InnerHtml = objPlantilla.FechaInscripcion.ToString("dd/MM/yyyy");
                //doc.GetElementbyId("FechaDocumento").InnerHtml = objPlantilla.FechaDocumento.ToString("dd/MM/yyyy");
                //doc.GetElementbyId("Foja").InnerHtml = objPlantilla.Fojas;
                //doc.GetElementbyId("TomoInscripcion").InnerHtml = objPlantilla.TomoInscripcion;
                //doc.GetElementbyId("Seccion").InnerHtml = objPlantilla.Seccion;
                //doc.GetElementbyId("NoInscripcion").InnerHtml = objPlantilla.NumeroInscripcion;
                //doc.GetElementbyId("Municipio").InnerHtml = objPlantilla.Municipio;

                //if (tipoPlantilla == 3)
                //{
                //    doc.GetElementbyId("FolioDocumento").InnerHtml = objPlantilla.FolioDocumento;
                //    doc.GetElementbyId("NoExpedienteDocumento").InnerHtml = objPlantilla.NoExpedienteDocumento;
                //    doc.GetElementbyId("AutoridadEmisoraDocumento").InnerHtml = objPlantilla.AutoridadEmisora;
                //    doc.GetElementbyId("DescripcionOficioDocumento").InnerHtml = objPlantilla.DescripcionOficio;
                //    doc.GetElementbyId("Solicitante").InnerHtml = objPlantilla.Solicitante;                    
                //}
                //else
                //{
                //    doc.GetElementbyId("Volumen").InnerHtml = objPlantilla.Volumen;
                //    doc.GetElementbyId("TomoDocumento").InnerHtml = objPlantilla.TomoDocumento;
                //    doc.GetElementbyId("CiudadNotario").InnerHtml = objPlantilla.MunicipioNotario;
                //    doc.GetElementbyId("Notario").InnerHtml = objPlantilla.Notario;
                //    doc.GetElementbyId("DescripcionActo").InnerHtml = objPlantilla.DescripcionActo;
                //    doc.GetElementbyId("DescripcionInmueble").InnerHtml = objPlantilla.DescripcionInmueble;
                //    doc.GetElementbyId("ClaveCatastral").InnerHtml = objPlantilla.ClaveCatastral;
                //}

                //doc.GetElementbyId("RutaCorreosNezterLoanding").SetAttributeValue("src", RutaCorreosNezterLoanding);



                //var memoryHtml = new MemoryStream();
                //doc.Save(memoryHtml);

                string htmlContent = doc.DocumentNode.OuterHtml;

                // byte[] pdfBytes = Pdf.From(Encoding.UTF8.GetString
                //(memoryHtml.ToArray()))  // El contenido HTML
                //.Content();               // Convierte a bytes PDF
                byte[] pdfBytes = System.Text.Encoding.UTF8.GetBytes(htmlContent);

                // Paso 2: Convertir los bytes PDF a Base64
                string base64String = Convert.ToBase64String(pdfBytes);
                base64 = base64String;


                //var pdf = Pdf.From(Encoding.UTF8.GetString(memoryHtml.ToArray())).Content();

                //MemoryStream MStream = new(pdf);
                //var documento = MStream;
                //byte[] bytes;
                //using (var memoryStream = new MemoryStream())
                //{
                //    documento.CopyTo(memoryStream);
                //    bytes = memoryStream.ToArray();
                //}
                //base64 = Convert.ToBase64String(bytes);
                //MStream.Dispose();
                //MStream.Close();
                //documento.Dispose();
                //documento.Close();
            }
            catch (Exception ex)
            {
               
                _ = ex.Message;
                
            }

            return base64;
        }

        public  string ExportPDFPlantillaSolicitudInvestigacionpdf(string Folio, string Path, string srcImage)
        {
            HtmlDocument doc2 = new HtmlDocument();

            // Crear el objeto para la conversión
            var converter = new BasicConverter(new PdfTools());
            string htmlString = "";
            byte[] pdf = null;

            string base64 = "";
            try
            {
                
                string RutaCorreosNezterLoanding = srcImage;
                doc2.Load(Path);

                //doc2.GetElementbyId("folio").InnerHtml = Folio;
                //doc.GetElementbyId("folio").InnerHtml = objPlantilla.FolioSolicitud;
                //doc.GetElementbyId("MunicipioTitulo").InnerHtml = objPlantilla.Municipio;
                //doc.GetElementbyId("EstadoTitulo").InnerHtml = objPlantilla.Estado;
                //doc.GetElementbyId("Estado").InnerHtml = objPlantilla.Estado;
                //doc.GetElementbyId("TipoInscripcion").InnerHtml = objPlantilla.TipoTransaccion;
                //doc.GetElementbyId("FechaInscripcion").InnerHtml = objPlantilla.FechaInscripcion.ToString("dd/MM/yyyy");
                //doc.GetElementbyId("FechaDocumento").InnerHtml = objPlantilla.FechaDocumento.ToString("dd/MM/yyyy");
                //doc.GetElementbyId("Foja").InnerHtml = objPlantilla.Fojas;
                //doc.GetElementbyId("TomoInscripcion").InnerHtml = objPlantilla.TomoInscripcion;
                //doc.GetElementbyId("Seccion").InnerHtml = objPlantilla.Seccion;
                //doc.GetElementbyId("NoInscripcion").InnerHtml = objPlantilla.NumeroInscripcion;
                //doc.GetElementbyId("Municipio").InnerHtml = objPlantilla.Municipio;

                //if (tipoPlantilla == 3)
                //{
                //    doc.GetElementbyId("FolioDocumento").InnerHtml = objPlantilla.FolioDocumento;
                //    doc.GetElementbyId("NoExpedienteDocumento").InnerHtml = objPlantilla.NoExpedienteDocumento;
                //    doc.GetElementbyId("AutoridadEmisoraDocumento").InnerHtml = objPlantilla.AutoridadEmisora;
                //    doc.GetElementbyId("DescripcionOficioDocumento").InnerHtml = objPlantilla.DescripcionOficio;
                //    doc.GetElementbyId("Solicitante").InnerHtml = objPlantilla.Solicitante;                    
                //}
                //else
                //{
                //    doc.GetElementbyId("Volumen").InnerHtml = objPlantilla.Volumen;
                //    doc.GetElementbyId("TomoDocumento").InnerHtml = objPlantilla.TomoDocumento;
                //    doc.GetElementbyId("CiudadNotario").InnerHtml = objPlantilla.MunicipioNotario;
                //    doc.GetElementbyId("Notario").InnerHtml = objPlantilla.Notario;
                //    doc.GetElementbyId("DescripcionActo").InnerHtml = objPlantilla.DescripcionActo;
                //    doc.GetElementbyId("DescripcionInmueble").InnerHtml = objPlantilla.DescripcionInmueble;
                //    doc.GetElementbyId("ClaveCatastral").InnerHtml = objPlantilla.ClaveCatastral;
                //}

                doc2.GetElementbyId("RutaCorreosNezterLoanding").SetAttributeValue("src", RutaCorreosNezterLoanding);
                htmlString = doc2.DocumentNode.OuterHtml;

            }
            catch (Exception ex)
            {

                _ = ex.Message;
            }

            return htmlString;
        }

     
    }
}