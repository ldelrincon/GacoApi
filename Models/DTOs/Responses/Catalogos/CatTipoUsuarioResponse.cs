namespace gaco_api.Models.DTOs.Responses.Catalogos
{
    public class CatTipoUsuarioResponse
    {
        public int Id { get; set; }

        public string TipoUsuario { get; set; } = null!;

        public string Descripcion { get; set; } = null!;

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public int IdCatEstatus { get; set; }

        public string? Estatus { get; set; }
    }
}
