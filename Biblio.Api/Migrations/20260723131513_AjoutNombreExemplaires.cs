using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblio.Api.Migrations
{
    /// <inheritdoc />
    public partial class AjoutNombreExemplaires : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NombreExemplaires",
                table: "Livres",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NombreExemplaires",
                table: "Livres");
        }
    }
}
