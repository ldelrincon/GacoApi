namespace gaco_api.Models.DTOs.Requests.Calendario
{
    public class AddTarea
    {
        public string? Descripcion { get; set; } 
        public DateTime FechaTarea { get; set; }
        public long IdUsuarioTarea { get; set; }
        public long IdUsuarioCreacion { get; set; }
    }
}
