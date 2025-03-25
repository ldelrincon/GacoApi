using System;
using System.Configuration;
using System.Globalization;
using System.Text;
using DinkToPdf;
using gaco_api.Models.DTOs.Responses.ReporteSolicitudes;
using HtmlAgilityPack;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Html;
using MimeKit;
using Newtonsoft.Json.Linq;

namespace ClbNegGestores
{
    public class ClsNegGenerarPDF
    {
     
        public  string ExportPDFPlantillaSolicitudInvestigacionpdf(string Folio, string Path, string srcImage, ReporteServicioResponse objReporteServicioResponse)
        {
            HtmlDocument doc2 = new HtmlDocument();

            // Crear el objeto para la conversión
            var converter = new BasicConverter(new PdfTools());
            string htmlString = "";
            byte[] pdf = null;
            string strTablaProductos = GenerarTablaHtml(objReporteServicioResponse);
            string strTablaEvidencias = GenerarTablaHtmlImagenes(objReporteServicioResponse);
            string base64 = "";
            try
            {
                //saca los montos para factura
                decimal? SubTotal = 0, Iva = 0, Total = 0, IvaAplicado = 0;
                string textoFechaInicio = objReporteServicioResponse.FechaInicio.HasValue
                  ? objReporteServicioResponse.FechaInicio.Value.ToString("dd/MM/yyyy") // Convertir con el formato deseado
                       : "Fecha no disponible";

                string textoFechaVisita = objReporteServicioResponse.ProximaVisita.HasValue
                  ? objReporteServicioResponse.ProximaVisita.Value.ToString("dd/MM/yyyy") // Convertir con el formato deseado
                       : "Fecha no disponible";
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
                doc2.GetElementbyId("NoServicio").InnerHtml ="S"+ objReporteServicioResponse.Id.ToString();
                doc2.GetElementbyId("FechaInicio").InnerHtml = textoFechaInicio;
                doc2.GetElementbyId("Cliente").InnerHtml = objReporteServicioResponse.Cliente;
                doc2.GetElementbyId("Telefono").InnerHtml = objReporteServicioResponse.Telefono;
                doc2.GetElementbyId("Correo").InnerHtml = objReporteServicioResponse.Correo;
                doc2.GetElementbyId("RazonSocial").InnerHtml = objReporteServicioResponse.RazonSocial;
                doc2.GetElementbyId("Direccion").InnerHtml = objReporteServicioResponse.Direccion+" C.P:"+objReporteServicioResponse.CodigoPostal;
                doc2.GetElementbyId("DatosEquipo").InnerHtml = objReporteServicioResponse.Titulo;
                doc2.GetElementbyId("Accesorios").InnerHtml = objReporteServicioResponse.Accesorios;
                doc2.GetElementbyId("Preventivo").InnerHtml = strPreventivo;
                doc2.GetElementbyId("Correctivo").InnerHtml = strCorrectivo;
                doc2.GetElementbyId("TrabajoRealizado").InnerHtml = objReporteServicioResponse.Descripcion.Replace("\n \t", "<br>"); 
                doc2.GetElementbyId("Observaciones").InnerHtml = objReporteServicioResponse.ObservacionesRecomendaciones;
                doc2.GetElementbyId("EncargadoArea").InnerHtml = objReporteServicioResponse.UsuarioEncargado;
                doc2.GetElementbyId("TecnicoEncargado").InnerHtml = objReporteServicioResponse.UsuarioTecnico ?? "sin asignar";
                doc2.GetElementbyId("FechaVisita").InnerHtml = textoFechaVisita;
                doc2.GetElementbyId("DescripcionVisita").InnerHtml = objReporteServicioResponse.DescripcionProximaVisita ?? "";
                doc2.GetElementbyId("RegimenFiscal").InnerHtml = objReporteServicioResponse.RegimenFiscal;
                doc2.GetElementbyId("Productos").InnerHtml = strTablaProductos;
                doc2.GetElementbyId("Imagenes").InnerHtml = strTablaEvidencias;

                doc2.GetElementbyId("SubTotal").InnerHtml = SubTotal?.ToString("C2")?? "$0.00";
                doc2.GetElementbyId("Iva").InnerHtml = Iva?.ToString("C2") ?? "$0.00";
                doc2.GetElementbyId("Total").InnerHtml = Total?.ToString("C2") ?? "$0.00";


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

        static string GenerarTablaHtmlImagenes(ReporteServicioResponse objReporteServicioResponse)
        {

            var sb = new StringBuilder();
            string strValidacion = "";
            string strReturn = "";

            sb.AppendLine("<table border=\"1\">");
            sb.AppendLine("  <thead>");
            sb.AppendLine("    <tr>");
            sb.AppendLine("      <th colspan=\"2\">Evidencias</th>"); // Cabecera que abarca dos columnas
            sb.AppendLine("    </tr>");
            sb.AppendLine("  </thead>");
            sb.AppendLine("  <tbody>");

            int imageCount = 0; // Contador para rastrear imágenes en cada fila
            foreach (var objEvidencias in objReporteServicioResponse.Evidencias)
            {
                if (objEvidencias.Extension.Contains("png") || objEvidencias.Extension.Contains("jpg") || objEvidencias.Extension.Contains("jpeg"))
                {
                    // Inicia una nueva fila si es la primera imagen o si ya hay dos imágenes en la fila anterior
                    if (imageCount % 2 == 0)
                    {
                        sb.AppendLine("    <tr>");
                    }

                    sb.AppendLine($"      <td><img src=\"data:image/jpeg;base64,{objEvidencias.Base64}\" alt=\"Base64 Image\" style=\"width: 250px; height: auto;\" /></td>");

                    imageCount++;

                    // Cierra la fila después de dos imágenes
                    if (imageCount % 2 == 0)
                    {
                        sb.AppendLine("    </tr>");
                    }
                    strValidacion = "Contiene";
                }
            }

            // Si el total de imágenes no es múltiplo de 2, cierra la última fila
            if (imageCount % 2 != 0)
            {
                sb.AppendLine("    </tr>");
            }

            sb.AppendLine("  </tbody>");
            sb.AppendLine("</table>");

            //return sb.ToString();
            if (strValidacion == "Contiene")
            {
                strReturn = sb.ToString();
            }
            else
            {
                strReturn= "";
            }
            return strReturn;
        }


    }
}