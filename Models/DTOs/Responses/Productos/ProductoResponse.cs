namespace gaco_api.Models.DTOs.Responses.Productos
{
    public class ProductoResponse
    {
        public long Id { get; set; }

        public int IdCatGrupoProducto { get; set; }

        public string Producto { get; set; } = null!;

        public string Descripcion { get; set; } = null!;

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public int IdCatEstatus { get; set; }

        public string Codigo { get; set; } = null!;

        public string? Estatus { get; set; }

        public string? GrupoProducto { get; set; }
    }
}
