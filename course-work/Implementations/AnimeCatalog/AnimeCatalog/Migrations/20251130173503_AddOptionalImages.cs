using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnimeCatalog.Migrations
{
    /// <inheritdoc />
    public partial class AddOptionalImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Writers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Animes",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Writers");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Animes");
        }
    }
}
