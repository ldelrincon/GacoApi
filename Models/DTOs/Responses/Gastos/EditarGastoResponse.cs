namespace gaco_api.Models.DTOs.Responses.Gastos
{
    public class EditarGastoResponse
    {
        public long Id { get; set; }
        public long IdUsuarioCreacion { get; set; }
        public string Concepto { get; set; } = null!;
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int IdCatEstatus { get; set; }

        public List<DetGastoResponse> DetGastos { get; set; } = new List<DetGastoResponse>();
    }
}
