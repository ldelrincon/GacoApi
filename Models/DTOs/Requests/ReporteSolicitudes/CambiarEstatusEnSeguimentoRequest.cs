namespace gaco_api.Models.DTOs.Requests.ReporteSolicitudes
{
    public class CambiarEstatusEnSeguimentoRequest
    {
        public long Id { get; set; }
        public int IdEstatus { get; set; }
        public string? strBody {  get; set; }
        public DateTime? FechaInicio { get; set; }
    }
}
