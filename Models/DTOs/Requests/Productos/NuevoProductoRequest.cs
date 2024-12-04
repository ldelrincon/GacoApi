namespace gaco_api.Models.DTOs.Requests.Productos
{
    public class NuevoProductoRequest
    {
        public int IdCatGrupoProducto { get; set; }

        public string Producto { get; set; } = null!;

        public string Descripcion { get; set; } = null!;

        public string Codigo { get; set; } = null!;
    }
}
