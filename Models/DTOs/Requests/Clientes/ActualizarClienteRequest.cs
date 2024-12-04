namespace gaco_api.Models.DTOs.Requests.Clientes
{
    public class ActualizarClienteRequest
    {
        public long Id { get; set; }
        public string Telefono { get; set; }
        public string Rfc { get; set; }
        public string Direccion { get; set; }
        public int IdCatEstatus { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public int IdCatMunicipio { get; set; }
        public string CodigoPostal { get; set; }
        public string RazonSocial { get; set; }
        public int IdRegimenFiscal { get; set; }
        public string Correo { get; set; }
    }
}
