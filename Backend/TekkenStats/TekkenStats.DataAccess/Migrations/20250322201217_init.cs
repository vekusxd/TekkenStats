using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TekkenStats.API.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Character",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", maxLength: 50, nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Abbreviation = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Character", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Player",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    WinCount = table.Column<int>(type: "integer", nullable: false),
                    LossCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Player", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Battle",
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
                    table.PrimaryKey("PK_Battle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Battle_Character_PlayerCharacter1Id",
                        column: x => x.PlayerCharacter1Id,
                        principalTable: "Character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Battle_Character_PlayerCharacter2Id",
                        column: x => x.PlayerCharacter2Id,
                        principalTable: "Character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Battle_Player_Player1Id",
                        column: x => x.Player1Id,
                        principalTable: "Player",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Battle_Player_Player2Id",
                        column: x => x.Player2Id,
                        principalTable: "Player",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterInfo",
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
                    table.PrimaryKey("PK_CharacterInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterInfo_Character_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterInfo_Player_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Player",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerName",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    PlayerId = table.Column<string>(type: "character varying(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerName_Player_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Player",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Battle_Player1Id",
                table: "Battle",
                column: "Player1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Battle_Player2Id",
                table: "Battle",
                column: "Player2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Battle_PlayerCharacter1Id",
                table: "Battle",
                column: "PlayerCharacter1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Battle_PlayerCharacter2Id",
                table: "Battle",
                column: "PlayerCharacter2Id");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterInfo_CharacterId",
                table: "CharacterInfo",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterInfo_PlayerId",
                table: "CharacterInfo",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerName_PlayerId",
                table: "PlayerName",
                column: "PlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Battle");

            migrationBuilder.DropTable(
                name: "CharacterInfo");

            migrationBuilder.DropTable(
                name: "PlayerName");

            migrationBuilder.DropTable(
                name: "Character");

            migrationBuilder.DropTable(
                name: "Player");
        }
    }
}
