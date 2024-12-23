namespace gaco_api.Models.DTOs.Responses.Evidencias
{
    public class EvidenciaResponse
    {
        public long Id { get; set; }

        public long IdSeguimento { get; set; }

        public string Nombre { get; set; } = null!;

        public string Extension { get; set; } = null!;

        public string Ruta { get; set; } = null!;

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public int IdCatEstatus { get; set; }
        public string? Base64 { get; set; }
    }
}
