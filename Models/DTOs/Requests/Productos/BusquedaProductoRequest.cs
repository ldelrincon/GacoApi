namespace gaco_api.Models.DTOs.Requests.Productos
{
    public class BusquedaProductoRequest : BasePaginadorRequest
    {
        public string Descripcion { get; set; }

        public int IdCatGrupoProducto { get; set; }
         
    }
}
