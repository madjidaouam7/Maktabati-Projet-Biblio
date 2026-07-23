using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblio.Api.Migrations
{
    /// <inheritdoc />
    public partial class AjoutEmprunts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Emprunts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LivreId = table.Column<int>(type: "INTEGER", nullable: false),
                    Utilisateur = table.Column<string>(type: "TEXT", nullable: false),
                    DateEmprunt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateRetourPrevue = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateRetourEffective = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Etat = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emprunts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Emprunts_Livres_LivreId",
                        column: x => x.LivreId,
                        principalTable: "Livres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Emprunts_LivreId",
                table: "Emprunts",
                column: "LivreId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Emprunts");
        }
    }
}
