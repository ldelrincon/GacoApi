namespace gaco_api.Models.DTOs.Requests.Gastos
{
    public class ActualizarGastoRequest
    {
        public long Id { get; set; }

        public string Concepto { get; set; } = null!;
        public int IdCatEstatus { get; set; }
        public List<DetGastoRequest>? DetGastos { get; set; } = null!;
    }
}
