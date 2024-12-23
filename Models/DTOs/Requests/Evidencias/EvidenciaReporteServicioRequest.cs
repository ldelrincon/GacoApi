namespace gaco_api.Models.DTOs.Requests.Evidencias
{
    public class EvidenciaReporteServicioRequest
    {
        public string Nombre { get; set; }
        public string Extension { get; set; }
        public decimal Tamanio { get; set; }
        public string Base64 { get; set; }
    }
}
