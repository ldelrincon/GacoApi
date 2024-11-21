using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gaco_api.Migrations
{
    /// <inheritdoc />
    public partial class ActualizacionModelo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CatEstatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Estatus = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false, defaultValue: "..."),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CatEstat__3214EC074A7EFB88", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CatGrupoProductos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Grupo = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false, defaultValue: "..."),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    IdCatEstatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CatGrupo__3214EC070E632ECA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatGrupoProductos_CatEstatuses_Id",
                        column: x => x.IdCatEstatus,
                        principalTable: "CatEstatuses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CatTipoSolicitudes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoSolicitud = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false, defaultValue: "..."),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    IdCatEstatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CatTipoS__3214EC077D1FFC14", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatTipoSolicitudes_CatEstatuses_Id",
                        column: x => x.IdCatEstatus,
                        principalTable: "CatEstatuses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CatTipoUsuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoUsuario = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false, defaultValue: "..."),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    IdCatEstatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CatTipoU__3214EC0773A44174", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatTipoUsuarios_CatEstatuses_Id",
                        column: x => x.IdCatEstatus,
                        principalTable: "CatEstatuses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Telefono = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: false),
                    RFC = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: false),
                    Direccion = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    IdCatEstatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Clientes__3214EC074AD95500", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clientes_CatEstatuses_Id",
                        column: x => x.IdCatEstatus,
                        principalTable: "CatEstatuses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCatGrupoProducto = table.Column<int>(type: "int", nullable: false),
                    Producto = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    IdCatEstatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Producto__3214EC0764646451", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Productos_CatEstatuses_Id",
                        column: x => x.IdCatEstatus,
                        principalTable: "CatEstatuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Productos_CatGrupoProductos_Id",
                        column: x => x.IdCatGrupoProducto,
                        principalTable: "CatGrupoProductos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCatTipoUsuario = table.Column<int>(type: "int", nullable: false),
                    Correo = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: false),
                    Contrasena = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: false),
                    CorreoConfirmado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Nombres = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: false),
                    Apellidos = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: false),
                    Telefono = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    IdCatEstatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Usuarios__3214EC07E8DEE2CB", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_CatEstatuses_Id",
                        column: x => x.IdCatEstatus,
                        principalTable: "CatEstatuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Usuarios_CatTipoUsuarios_Id",
                        column: x => x.IdCatTipoUsuario,
                        principalTable: "CatTipoUsuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ReporteServicios",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCatSolicitud = table.Column<int>(type: "int", nullable: false),
                    IdUsuarioCreacion = table.Column<long>(type: "bigint", nullable: false),
                    IdCliente = table.Column<long>(type: "bigint", nullable: false),
                    Titulo = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    IdCatEstatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ReporteS__3214EC07B6BCABCD", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReporteServicios_CatEstatuses_Id",
                        column: x => x.IdCatEstatus,
                        principalTable: "CatEstatuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReporteServicios_CatTipoSolicitudes_Id",
                        column: x => x.IdCatSolicitud,
                        principalTable: "CatTipoSolicitudes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReporteServicios_Clientes_Id",
                        column: x => x.IdCliente,
                        principalTable: "Clientes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReporteServicios_Usuarios_Id",
                        column: x => x.IdUsuarioCreacion,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Seguimentos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdReporteServicio = table.Column<long>(type: "bigint", nullable: false),
                    IdUsuario = table.Column<long>(type: "bigint", nullable: false),
                    Seguimento = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    IdCatEstatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Seguimen__3214EC07BB752A16", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Seguimentos_CatEstatuses_Id",
                        column: x => x.IdCatEstatus,
                        principalTable: "CatEstatuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Seguimentos_ReporteServicios_Id",
                        column: x => x.IdReporteServicio,
                        principalTable: "ReporteServicios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Seguimentos_Usuarios_Id",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Evidencias",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdSeguimento = table.Column<long>(type: "bigint", nullable: false),
                    Nombre = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: false),
                    Extension = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ruta = table.Column<string>(type: "varchar(800)", unicode: false, maxLength: 800, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    IdCatEstatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Evidenci__3214EC07887F9876", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evidencias_CatEstatuses_Id",
                        column: x => x.IdCatEstatus,
                        principalTable: "CatEstatuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Evidencias_Seguimentos_Id",
                        column: x => x.IdSeguimento,
                        principalTable: "Seguimentos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RelSeguimentoProductos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdSeguimento = table.Column<long>(type: "bigint", nullable: false),
                    IdProducto = table.Column<long>(type: "bigint", nullable: false),
                    IdUsuario = table.Column<long>(type: "bigint", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    IdCatEstatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__RelSegui__3214EC0779DF5CCC", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RelSeguimentoProductos_CatEstatuses_Id",
                        column: x => x.IdCatEstatus,
                        principalTable: "CatEstatuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RelSeguimentoProductos_Productos_Id",
                        column: x => x.IdProducto,
                        principalTable: "Productos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RelSeguimentoProductos_Seguimentos_Id",
                        column: x => x.IdSeguimento,
                        principalTable: "Seguimentos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RelSeguimentoProductos_Usuarios_Id",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CatGrupoProductos_IdCatEstatus",
                table: "CatGrupoProductos",
                column: "IdCatEstatus");

            migrationBuilder.CreateIndex(
                name: "IX_CatTipoSolicitudes_IdCatEstatus",
                table: "CatTipoSolicitudes",
                column: "IdCatEstatus");

            migrationBuilder.CreateIndex(
                name: "IX_CatTipoUsuarios_IdCatEstatus",
                table: "CatTipoUsuarios",
                column: "IdCatEstatus");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_IdCatEstatus",
                table: "Clientes",
                column: "IdCatEstatus");

            migrationBuilder.CreateIndex(
                name: "IX_Evidencias_IdCatEstatus",
                table: "Evidencias",
                column: "IdCatEstatus");

            migrationBuilder.CreateIndex(
                name: "IX_Evidencias_IdSeguimento",
                table: "Evidencias",
                column: "IdSeguimento");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_IdCatEstatus",
                table: "Productos",
                column: "IdCatEstatus");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_IdCatGrupoProducto",
                table: "Productos",
                column: "IdCatGrupoProducto");

            migrationBuilder.CreateIndex(
                name: "IX_RelSeguimentoProductos_IdCatEstatus",
                table: "RelSeguimentoProductos",
                column: "IdCatEstatus");

            migrationBuilder.CreateIndex(
                name: "IX_RelSeguimentoProductos_IdProducto",
                table: "RelSeguimentoProductos",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_RelSeguimentoProductos_IdSeguimento",
                table: "RelSeguimentoProductos",
                column: "IdSeguimento");

            migrationBuilder.CreateIndex(
                name: "IX_RelSeguimentoProductos_IdUsuario",
                table: "RelSeguimentoProductos",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_ReporteServicios_IdCatEstatus",
                table: "ReporteServicios",
                column: "IdCatEstatus");

            migrationBuilder.CreateIndex(
                name: "IX_ReporteServicios_IdCatSolicitud",
                table: "ReporteServicios",
                column: "IdCatSolicitud");

            migrationBuilder.CreateIndex(
                name: "IX_ReporteServicios_IdCliente",
                table: "ReporteServicios",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_ReporteServicios_IdUsuarioCreacion",
                table: "ReporteServicios",
                column: "IdUsuarioCreacion");

            migrationBuilder.CreateIndex(
                name: "IX_Seguimentos_IdCatEstatus",
                table: "Seguimentos",
                column: "IdCatEstatus");

            migrationBuilder.CreateIndex(
                name: "IX_Seguimentos_IdReporteServicio",
                table: "Seguimentos",
                column: "IdReporteServicio");

            migrationBuilder.CreateIndex(
                name: "IX_Seguimentos_IdUsuario",
                table: "Seguimentos",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdCatEstatus",
                table: "Usuarios",
                column: "IdCatEstatus");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdCatTipoUsuario",
                table: "Usuarios",
                column: "IdCatTipoUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Evidencias");

            migrationBuilder.DropTable(
                name: "RelSeguimentoProductos");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Seguimentos");

            migrationBuilder.DropTable(
                name: "CatGrupoProductos");

            migrationBuilder.DropTable(
                name: "ReporteServicios");

            migrationBuilder.DropTable(
                name: "CatTipoSolicitudes");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "CatTipoUsuarios");

            migrationBuilder.DropTable(
                name: "CatEstatuses");
        }
    }
}
