using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TekkenStats.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Abbreviation = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    WinCount = table.Column<int>(type: "integer", nullable: false),
                    LossCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Battles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    GameVersion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PlayedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Player1Id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Player2Id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PlayerCharacter1Id = table.Column<int>(type: "integer", nullable: false),
                    PlayerCharacter2Id = table.Column<int>(type: "integer", nullable: false),
                    Player1RoundsCount = table.Column<int>(type: "integer", nullable: false),
                    Player2RoundsCount = table.Column<int>(type: "integer", nullable: false),
                    Player1RatingBefore = table.Column<int>(type: "integer", nullable: false),
                    Player2RatingBefore = table.Column<int>(type: "integer", nullable: false),
                    Player1RatingChange = table.Column<int>(type: "integer", nullable: false),
                    Player2RatingChange = table.Column<int>(type: "integer", nullable: false),
                    Winner = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Battles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Battles_Characters_PlayerCharacter1Id",
                        column: x => x.PlayerCharacter1Id,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Battles_Characters_PlayerCharacter2Id",
                        column: x => x.PlayerCharacter2Id,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Battles_Players_Player1Id",
                        column: x => x.Player1Id,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Battles_Players_Player2Id",
                        column: x => x.Player2Id,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    PlayerId = table.Column<string>(type: "character varying(50)", nullable: false),
                    WinCount = table.Column<int>(type: "integer", nullable: false),
                    LossCount = table.Column<int>(type: "integer", nullable: false),
                    LastPlayed = table.Column<DateOnly>(type: "date", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterInfos_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterInfos_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerNames",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    PlayerId = table.Column<string>(type: "character varying(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerNames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerNames_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Battles_Player1Id",
                table: "Battles",
                column: "Player1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_Player2Id",
                table: "Battles",
                column: "Player2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_PlayerCharacter1Id",
                table: "Battles",
                column: "PlayerCharacter1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_PlayerCharacter2Id",
                table: "Battles",
                column: "PlayerCharacter2Id");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterInfos_CharacterId",
                table: "CharacterInfos",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterInfos_PlayerId",
                table: "CharacterInfos",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerNames_PlayerId",
                table: "PlayerNames",
                column: "PlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Battles");

            migrationBuilder.DropTable(
                name: "CharacterInfos");

            migrationBuilder.DropTable(
                name: "PlayerNames");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
