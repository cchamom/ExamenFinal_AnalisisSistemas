using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ExamenFinal_Analisis.Migrations
{
    /// <inheritdoc />
    public partial class InicialExamen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departamentos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Estados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Orden = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Paquetes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CodigoRastreo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Remitente = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Destinatario = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    DepartamentoDestinoId = table.Column<int>(type: "INTEGER", nullable: false),
                    EstadoActualId = table.Column<int>(type: "INTEGER", nullable: false),
                    IntentosEntrega = table.Column<int>(type: "INTEGER", nullable: false),
                    CostoEnvio = table.Column<decimal>(type: "TEXT", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paquetes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Paquetes_Departamentos_DepartamentoDestinoId",
                        column: x => x.DepartamentoDestinoId,
                        principalTable: "Departamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Paquetes_Estados_EstadoActualId",
                        column: x => x.EstadoActualId,
                        principalTable: "Estados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistorialEstados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PaqueteId = table.Column<int>(type: "INTEGER", nullable: false),
                    EstadoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Ubicacion = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Observaciones = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialEstados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialEstados_Estados_EstadoId",
                        column: x => x.EstadoId,
                        principalTable: "Estados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HistorialEstados_Paquetes_PaqueteId",
                        column: x => x.PaqueteId,
                        principalTable: "Paquetes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Departamentos",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Guatemala" },
                    { 2, "Alta Verapaz" },
                    { 3, "Baja Verapaz" },
                    { 4, "Chimaltenango" },
                    { 5, "Chiquimula" },
                    { 6, "El Progreso" },
                    { 7, "Escuintla" },
                    { 8, "Huehuetenango" },
                    { 9, "Izabal" },
                    { 10, "Jalapa" },
                    { 11, "Jutiapa" },
                    { 12, "Petén" },
                    { 13, "Quetzaltenango" },
                    { 14, "Quiché" },
                    { 15, "Retalhuleu" },
                    { 16, "Sacatepéquez" },
                    { 17, "San Marcos" },
                    { 18, "Santa Rosa" }
                });

            migrationBuilder.InsertData(
                table: "Estados",
                columns: new[] { "Id", "Nombre", "Orden" },
                values: new object[,]
                {
                    { 1, "Registrado", 1 },
                    { 2, "EnReparto", 2 },
                    { 3, "Entregado", 3 },
                    { 4, "En Devolucion", 4 },
                    { 5, "Devuelto", 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_HistorialEstados_EstadoId",
                table: "HistorialEstados",
                column: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialEstados_PaqueteId",
                table: "HistorialEstados",
                column: "PaqueteId");

            migrationBuilder.CreateIndex(
                name: "IX_Paquetes_DepartamentoDestinoId",
                table: "Paquetes",
                column: "DepartamentoDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_Paquetes_EstadoActualId",
                table: "Paquetes",
                column: "EstadoActualId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistorialEstados");

            migrationBuilder.DropTable(
                name: "Paquetes");

            migrationBuilder.DropTable(
                name: "Departamentos");

            migrationBuilder.DropTable(
                name: "Estados");
        }
    }
}
