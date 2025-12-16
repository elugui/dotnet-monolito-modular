using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonolitoModular.Slices.Cadastrados.Estruturas.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "estruturas");

            migrationBuilder.CreateTable(
                name: "Estruturas",
                schema: "estruturas",
                columns: table => new
                {
                    Codigo = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EstruturaTipoCodigo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CodigoExterno = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    InicioVigencia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TerminoVigencia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Versao = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estruturas", x => x.Codigo);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Estruturas",
                schema: "estruturas");
        }
    }
}
