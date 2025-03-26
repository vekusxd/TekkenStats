using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TekkenStats.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ProcessedMessagesUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProcessedMessages_MessageId",
                table: "ProcessedMessages");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessedMessages_MessageId",
                table: "ProcessedMessages",
                column: "MessageId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProcessedMessages_MessageId",
                table: "ProcessedMessages");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessedMessages_MessageId",
                table: "ProcessedMessages",
                column: "MessageId");
        }
    }
}
