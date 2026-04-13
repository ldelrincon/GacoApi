namespace gaco_api.Models.DTOs.Responses.Calendario
{
    public class CalendarioResponse
    {
        public int IdCalendario { get; set; }
        public string? Descripcion { get; set; }
        public DateTime? FechaTarea { get; set; }
        public long? IdUsuarioTarea { get; set; }
        public string? UsuarioTarea { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public long IdUsuarioCreacion { get; set; }
        public bool? Terminado { get; set; }
        public DateTime? FechaTerminado { get; set; }
    }
}
