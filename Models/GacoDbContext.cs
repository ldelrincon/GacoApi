using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace gaco_api.Models;

public partial class GacoDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public GacoDbContext()
    {
    }

    public GacoDbContext(DbContextOptions<GacoDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public virtual DbSet<CatEstatus> CatEstatuses { get; set; }

    public virtual DbSet<CatGrupoProducto> CatGrupoProductos { get; set; }

    public virtual DbSet<CatTipoSolicitude> CatTipoSolicitudes { get; set; }

    public virtual DbSet<CatTipoUsuario> CatTipoUsuarios { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Evidencia> Evidencias { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<RelSeguimentoProducto> RelSeguimentoProductos { get; set; }

    public virtual DbSet<ReporteServicio> ReporteServicios { get; set; }

    public virtual DbSet<Seguimento> Seguimentos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CatEstatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CatEstat__3214EC074A7EFB88");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasDefaultValue("...");
            entity.Property(e => e.Estatus)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
        });

        modelBuilder.Entity<CatGrupoProducto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CatGrupo__3214EC070E632ECA");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasDefaultValue("...");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.Grupo)
                .HasMaxLength(300)
                .IsUnicode(false);

            entity.HasOne(d => d.IdCatEstatusNavigation).WithMany(p => p.CatGrupoProductos)
                .HasForeignKey(d => d.IdCatEstatus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CatGrupoProductos_CatEstatuses_Id");
        });

        modelBuilder.Entity<CatTipoSolicitude>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CatTipoS__3214EC077D1FFC14");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasDefaultValue("...");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.TipoSolicitud)
                .HasMaxLength(300)
                .IsUnicode(false);

            entity.HasOne(d => d.IdCatEstatusNavigation).WithMany(p => p.CatTipoSolicitudes)
                .HasForeignKey(d => d.IdCatEstatus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CatTipoSolicitudes_CatEstatuses_Id");
        });

        modelBuilder.Entity<CatTipoUsuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CatTipoU__3214EC0773A44174");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasDefaultValue("...");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.TipoUsuario)
                .HasMaxLength(300)
                .IsUnicode(false);

            entity.HasOne(d => d.IdCatEstatusNavigation).WithMany(p => p.CatTipoUsuarios)
                .HasForeignKey(d => d.IdCatEstatus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CatTipoUsuarios_CatEstatuses_Id");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Clientes__3214EC074AD95500");

            entity.Property(e => e.Direccion)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.Rfc)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("RFC");
            entity.Property(e => e.Telefono)
                .HasMaxLength(300)
                .IsUnicode(false);

            entity.HasOne(d => d.IdCatEstatusNavigation).WithMany(p => p.Clientes)
                .HasForeignKey(d => d.IdCatEstatus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Clientes_CatEstatuses_Id");
        });

        modelBuilder.Entity<Evidencia>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Evidenci__3214EC07887F9876");

            entity.Property(e => e.Extension)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.Nombre)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Ruta)
                .HasMaxLength(800)
                .IsUnicode(false);

            entity.HasOne(d => d.IdCatEstatusNavigation).WithMany(p => p.Evidencia)
                .HasForeignKey(d => d.IdCatEstatus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Evidencias_CatEstatuses_Id");

            entity.HasOne(d => d.IdSeguimentoNavigation).WithMany(p => p.Evidencia)
                .HasForeignKey(d => d.IdSeguimento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Evidencias_Seguimentos_Id");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Producto__3214EC0764646451");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.Producto1)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("Producto");

            entity.HasOne(d => d.IdCatEstatusNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdCatEstatus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Productos_CatEstatuses_Id");

            entity.HasOne(d => d.IdCatGrupoProductoNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdCatGrupoProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Productos_CatGrupoProductos_Id");
        });

        modelBuilder.Entity<RelSeguimentoProducto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RelSegui__3214EC0779DF5CCC");

            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");

            entity.HasOne(d => d.IdCatEstatusNavigation).WithMany(p => p.RelSeguimentoProductos)
                .HasForeignKey(d => d.IdCatEstatus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RelSeguimentoProductos_CatEstatuses_Id");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.RelSeguimentoProductos)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RelSeguimentoProductos_Productos_Id");

            entity.HasOne(d => d.IdSeguimentoNavigation).WithMany(p => p.RelSeguimentoProductos)
                .HasForeignKey(d => d.IdSeguimento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RelSeguimentoProductos_Seguimentos_Id");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.RelSeguimentoProductos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RelSeguimentoProductos_Usuarios_Id");
        });

        modelBuilder.Entity<ReporteServicio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ReporteS__3214EC07B6BCABCD");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.Titulo)
                .HasMaxLength(300)
                .IsUnicode(false);

            entity.HasOne(d => d.IdCatEstatusNavigation).WithMany(p => p.ReporteServicios)
                .HasForeignKey(d => d.IdCatEstatus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReporteServicios_CatEstatuses_Id");

            entity.HasOne(d => d.IdCatSolicitudNavigation).WithMany(p => p.ReporteServicios)
                .HasForeignKey(d => d.IdCatSolicitud)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReporteServicios_CatTipoSolicitudes_Id");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.ReporteServicios)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReporteServicios_Clientes_Id");

            entity.HasOne(d => d.IdUsuarioCreacionNavigation).WithMany(p => p.ReporteServicios)
                .HasForeignKey(d => d.IdUsuarioCreacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReporteServicios_Usuarios_Id");
        });

        modelBuilder.Entity<Seguimento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Seguimen__3214EC07BB752A16");

            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.Seguimento1)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Seguimento");

            entity.HasOne(d => d.IdCatEstatusNavigation).WithMany(p => p.Seguimentos)
                .HasForeignKey(d => d.IdCatEstatus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Seguimentos_CatEstatuses_Id");

            entity.HasOne(d => d.IdReporteServicioNavigation).WithMany(p => p.Seguimentos)
                .HasForeignKey(d => d.IdReporteServicio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Seguimentos_ReporteServicios_Id");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Seguimentos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Seguimentos_Usuarios_Id");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuarios__3214EC07E8DEE2CB");

            entity.Property(e => e.Apellidos)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Contrasena)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.CorreoConfirmado).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.Nombres)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(300)
                .IsUnicode(false);

            entity.HasOne(d => d.IdCatEstatusNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdCatEstatus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuarios_CatEstatuses_Id");

            entity.HasOne(d => d.IdCatTipoUsuarioNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdCatTipoUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuarios_CatTipoUsuarios_Id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
