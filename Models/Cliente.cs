using System;
using System.Collections.Generic;

namespace gaco_api.Models;

public partial class Cliente
{
    public long Id { get; set; }

    public string Telefono { get; set; } = null!;

    public string Rfc { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int IdCatEstatus { get; set; }

    public string Nombre { get; set; } = null!;

    public string Codigo { get; set; } = null!;

    public long IdCatMunicipio { get; set; }

    public string CodigoPostal { get; set; } = null!;

    public string RazonSocial { get; set; } = null!;

    public int IdRegimenFiscal { get; set; }

    public string Correo { get; set; } = null!;

    public virtual CatEstatus IdCatEstatusNavigation { get; set; } = null!;

    public virtual CatMunicipio IdCatMunicipioNavigation { get; set; } = null!;

    public virtual CatRegimenFiscale IdRegimenFiscalNavigation { get; set; } = null!;

    public virtual ICollection<ReporteServicio> ReporteServicios { get; set; } = new List<ReporteServicio>();
}
