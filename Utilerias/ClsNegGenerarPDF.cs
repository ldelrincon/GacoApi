using System.Configuration;
using System.Globalization;
using System.Text;
using DinkToPdf;
using gaco_api.Models.DTOs.Responses.ReporteSolicitudes;
using HtmlAgilityPack;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Html;
using MimeKit;

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

        public  string ExportPDFPlantillaSolicitudInvestigacionpdf(string Folio, string Path, string srcImage, ReporteServicioResponse objReporteServicioResponse)
        {
            HtmlDocument doc2 = new HtmlDocument();

            // Crear el objeto para la conversión
            var converter = new BasicConverter(new PdfTools());
            string htmlString = "";
            byte[] pdf = null;
            string strTablaProductos = GenerarTablaHtml(objReporteServicioResponse);
            string base64 = "";
            try
            {
                //saca los montos para factura
                decimal? SubTotal = 0, Iva = 0, Total = 0, IvaAplicado = 0;
                foreach (var objProductos in objReporteServicioResponse.Productos)
                {
                    SubTotal += (objProductos.Cantidad * objProductos.MontoVenta);
                }
                Iva = SubTotal * .16m;
                Total = SubTotal + Iva;


                string RutaCorreosNezterLoanding = srcImage;
                doc2.Load(Path);
                string strPreventivo="No", strCorrectivo="No";
                if (objReporteServicioResponse.ServicioPreventivo == true)
                {
                    strPreventivo = "Si";
                }
                if (objReporteServicioResponse.ServicioCorrectivo == true)
                {
                    strCorrectivo = "Si";
                }
                doc2.GetElementbyId("NoServicio").InnerHtml = objReporteServicioResponse.Id.ToString();
                doc2.GetElementbyId("FechaInicio").InnerHtml = objReporteServicioResponse.FechaInicio.ToString();
                doc2.GetElementbyId("Cliente").InnerHtml = objReporteServicioResponse.Cliente;
                doc2.GetElementbyId("Telefono").InnerHtml = objReporteServicioResponse.Telefono;
                doc2.GetElementbyId("Correo").InnerHtml = objReporteServicioResponse.Correo;
                doc2.GetElementbyId("RazonSocial").InnerHtml = objReporteServicioResponse.RazonSocial;
                doc2.GetElementbyId("Direccion").InnerHtml = objReporteServicioResponse.Direccion+" C.P:"+objReporteServicioResponse.CodigoPostal;
                doc2.GetElementbyId("DatosEquipo").InnerHtml = objReporteServicioResponse.Titulo;
                doc2.GetElementbyId("Accesorios").InnerHtml = objReporteServicioResponse.Accesorios;
                doc2.GetElementbyId("Preventivo").InnerHtml = strPreventivo;
                doc2.GetElementbyId("Correctivo").InnerHtml = strCorrectivo;
                doc2.GetElementbyId("TrabajoRealizado").InnerHtml = objReporteServicioResponse.Descripcion;
                doc2.GetElementbyId("Observaciones").InnerHtml = objReporteServicioResponse.ObservacionesRecomendaciones;
                doc2.GetElementbyId("EncargadoArea").InnerHtml = objReporteServicioResponse.UsuarioEncargado;
                doc2.GetElementbyId("TecnicoEncargado").InnerHtml = objReporteServicioResponse.UsuarioTecnico ?? "sin asignar";
                doc2.GetElementbyId("FechaVisita").InnerHtml = objReporteServicioResponse.ProximaVisita.ToString();
                doc2.GetElementbyId("DescripcionVisita").InnerHtml = objReporteServicioResponse.DescripcionProximaVisita;
                doc2.GetElementbyId("RegimenFiscal").InnerHtml = objReporteServicioResponse.RegimenFiscal;
                doc2.GetElementbyId("Productos").InnerHtml = strTablaProductos;

                doc2.GetElementbyId("SubTotal").InnerHtml = SubTotal?.ToString("C2");
                doc2.GetElementbyId("Iva").InnerHtml = Iva?.ToString("C2");
                doc2.GetElementbyId("Total").InnerHtml = Total?.ToString("C2");


                doc2.GetElementbyId("RutaCorreosNezterLoanding").SetAttributeValue("src", RutaCorreosNezterLoanding);
                htmlString = doc2.DocumentNode.OuterHtml;

            }
            catch (Exception ex)
            {

                _ = ex.Message;
            }

            return htmlString;
        }

        static string GenerarTablaHtml(ReporteServicioResponse objReporteServicioResponse)
        {
            var sb = new StringBuilder();
           

            sb.AppendLine("<table border=\"1\">");
            sb.AppendLine("  <thead>");
            sb.AppendLine("    <tr>");
            //sb.AppendLine("      <th>ID</th>");
            sb.AppendLine("      <th>Producto</th>");
            sb.AppendLine("      <th>Cantidad</th>");
            sb.AppendLine("      <th>Precio</th>");
            sb.AppendLine("    </tr>");
            sb.AppendLine("  </thead>");
            sb.AppendLine("  <tbody>");

            foreach (var objProductos in objReporteServicioResponse.Productos)
            {
                sb.AppendLine("    <tr>");
                sb.AppendLine($"      <td>{objProductos.Producto}</td>");
                sb.AppendLine($"      <td>{objProductos.Cantidad}</td>");
                sb.AppendLine($"      <td>{objProductos.MontoVenta?.ToString("C2")}</td>");
                //sb.AppendLine($"      <td>{persona.Edad}</td>");
                sb.AppendLine("    </tr>");
            }
            

            sb.AppendLine("  </tbody>");
            sb.AppendLine("</table>");


            return sb.ToString();
        }


    }
}