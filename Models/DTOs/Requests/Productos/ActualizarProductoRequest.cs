namespace gaco_api.Models.DTOs.Requests.Productos
{
    public class ActualizarProductoRequest
    {
        public long Id { get; set; }
        public int IdCatGrupoProducto { get; set; }
        public string Producto { get; set; }
        public string Descripcion { get; set; }
        public string Codigo { get; set; }
    }
}
