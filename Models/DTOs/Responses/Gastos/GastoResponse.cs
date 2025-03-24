namespace gaco_api.Models.DTOs.Responses.Gastos
{
    public class GastoResponse
    {
        public long Id { get; set; }
        public long IdUsuarioCreacion { get; set; }
        public string NombreUsuarioCreacion { get; set; } = null!;
        public string Concepto { get; set; } = null!;
        public decimal Monto { get; set; }  
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int IdCatEstatus { get; set; }
        public string NombreCatEstatus { get; set; } = null!;
    }
}
