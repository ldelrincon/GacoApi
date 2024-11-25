namespace gaco_api.Models.DTOs.Requests.Clientes
{
    public class NuevoClienteRequest
    {
        public string Telefono { get; set; } = null!;

        public string Rfc { get; set; } = null!;

        public string Direccion { get; set; } = null!;

        public int IdCatEstatus { get; set; }

        public string Nombre { get; set; } = null!;

        public string Codigo { get; set; } = null!;

        public long IdCatMunicipio { get; set; }

        public string CodigoPostal { get; set; } = null!;

        public string RazonSocial { get; set; } = null!;

        public int IdRegimenFiscal { get; set; }

        public string Correo { get; set; } = null!;
    }
}
