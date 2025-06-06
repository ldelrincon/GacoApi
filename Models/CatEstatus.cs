﻿using System;
using System.Collections.Generic;

namespace gaco_api.Models;

public partial class CatEstatus
{
    public int Id { get; set; }

    public string Estatus { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<CatEntidadesFederativa> CatEntidadesFederativas { get; set; } = new List<CatEntidadesFederativa>();

    public virtual ICollection<CatGrupoProducto> CatGrupoProductos { get; set; } = new List<CatGrupoProducto>();

    public virtual ICollection<CatMunicipio> CatMunicipios { get; set; } = new List<CatMunicipio>();

    public virtual ICollection<CatRegimenFiscale> CatRegimenFiscales { get; set; } = new List<CatRegimenFiscale>();

    public virtual ICollection<CatTipoSolicitude> CatTipoSolicitudes { get; set; } = new List<CatTipoSolicitude>();

    public virtual ICollection<CatTipoUsuario> CatTipoUsuarios { get; set; } = new List<CatTipoUsuario>();

    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();

    public virtual ICollection<DetGasto> DetGastos { get; set; } = new List<DetGasto>();

    public virtual ICollection<Evidencia> Evidencia { get; set; } = new List<Evidencia>();

    public virtual ICollection<Gasto> Gastos { get; set; } = new List<Gasto>();

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();

    public virtual ICollection<RelPerfilEstatus> RelPerfilEstatuses { get; set; } = new List<RelPerfilEstatus>();

    public virtual ICollection<RelSeguimentoProducto> RelSeguimentoProductos { get; set; } = new List<RelSeguimentoProducto>();

    public virtual ICollection<ReporteServicio> ReporteServicios { get; set; } = new List<ReporteServicio>();

    public virtual ICollection<Seguimento> Seguimentos { get; set; } = new List<Seguimento>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
