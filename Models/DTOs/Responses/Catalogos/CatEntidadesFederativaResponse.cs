namespace gaco_api.Models.DTOs.Responses.Catalogos
{
    public class CatEntidadesFederativaResponse
    {
        public long Id { get; set; }

        public string? CatalogKey { get; set; }

        public string? EntidadFederativa { get; set; }

        public string? Abreviatura { get; set; }

        public int IdCatEstatus { get; set; }
    }
}
