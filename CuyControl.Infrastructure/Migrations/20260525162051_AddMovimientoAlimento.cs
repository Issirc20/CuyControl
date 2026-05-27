using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CuyControl.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMovimientoAlimento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alimentaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JaulaId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    FechaAlimentacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CantidadAlimento = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TipoAlimento = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioCreacionId = table.Column<int>(type: "int", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioModificacionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alimentaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alimentaciones_Jaulas_JaulaId",
                        column: x => x.JaulaId,
                        principalTable: "Jaulas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alimentaciones_Usuarios_UsuarioCreacionId",
                        column: x => x.UsuarioCreacionId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Alimentaciones_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alimentaciones_Usuarios_UsuarioModificacionId",
                        column: x => x.UsuarioModificacionId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MovimientosAlimento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventarioAlimentoId = table.Column<int>(type: "int", nullable: false),
                    TipoMovimiento = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    FechaMovimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioCreacionId = table.Column<int>(type: "int", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioModificacionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosAlimento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimientosAlimento_InventariosAlimento_InventarioAlimentoId",
                        column: x => x.InventarioAlimentoId,
                        principalTable: "InventariosAlimento",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MovimientosAlimento_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alimentaciones_JaulaId",
                table: "Alimentaciones",
                column: "JaulaId");

            migrationBuilder.CreateIndex(
                name: "IX_Alimentaciones_UsuarioCreacionId",
                table: "Alimentaciones",
                column: "UsuarioCreacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Alimentaciones_UsuarioId",
                table: "Alimentaciones",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Alimentaciones_UsuarioModificacionId",
                table: "Alimentaciones",
                column: "UsuarioModificacionId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosAlimento_InventarioAlimentoId",
                table: "MovimientosAlimento",
                column: "InventarioAlimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosAlimento_UsuarioId",
                table: "MovimientosAlimento",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alimentaciones");

            migrationBuilder.DropTable(
                name: "MovimientosAlimento");
        }
    }
}
