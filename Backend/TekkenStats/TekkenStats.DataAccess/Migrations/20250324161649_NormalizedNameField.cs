using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TekkenStats.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class NormalizedNameField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlayerNames_Name",
                table: "PlayerNames");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "PlayerNames",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerNames_NormalizedName",
                table: "PlayerNames",
                column: "NormalizedName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlayerNames_NormalizedName",
                table: "PlayerNames");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "PlayerNames");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerNames_Name",
                table: "PlayerNames",
                column: "Name");
        }
    }
}
