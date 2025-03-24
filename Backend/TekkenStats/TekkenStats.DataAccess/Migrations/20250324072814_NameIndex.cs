using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TekkenStats.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class NameIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PlayerNames_Name",
                table: "PlayerNames",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlayerNames_Name",
                table: "PlayerNames");
        }
    }
}
