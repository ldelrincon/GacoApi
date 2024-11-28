namespace gaco_api.Models.DTOs.Responses.Catalogos
{
    public class CatMunicipioResponse
    {
        public long Id { get; set; }

        public string? CatalogKey { get; set; }

        public string? Municipio { get; set; }

        public string? EfeKey { get; set; }

        public string? Estatus { get; set; }

        public int IdCatEstatus { get; set; }
    }
}
