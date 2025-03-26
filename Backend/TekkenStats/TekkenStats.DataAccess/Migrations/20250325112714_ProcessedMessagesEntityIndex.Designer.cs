﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TekkenStats.DataAccess;

#nullable disable

namespace TekkenStats.DataAccess.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250325112714_ProcessedMessagesEntityIndex")]
    partial class ProcessedMessagesEntityIndex
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TekkenStats.Core.Entities.Battle", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("GameVersion")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<DateTime>("PlayedDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Player1Id")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<int>("Player1RatingBefore")
                        .HasColumnType("integer");

                    b.Property<int>("Player1RatingChange")
                        .HasColumnType("integer");

                    b.Property<int>("Player1RoundsCount")
                        .HasColumnType("integer");

                    b.Property<string>("Player2Id")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<int>("Player2RatingBefore")
                        .HasColumnType("integer");

                    b.Property<int>("Player2RatingChange")
                        .HasColumnType("integer");

                    b.Property<int>("Player2RoundsCount")
                        .HasColumnType("integer");

                    b.Property<int>("PlayerCharacter1Id")
                        .HasColumnType("integer");

                    b.Property<int>("PlayerCharacter2Id")
                        .HasColumnType("integer");

                    b.Property<int>("Winner")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Player1Id");

                    b.HasIndex("Player2Id");

                    b.HasIndex("PlayerCharacter1Id");

                    b.HasIndex("PlayerCharacter2Id");

                    b.ToTable("Battles");
                });

            modelBuilder.Entity("TekkenStats.Core.Entities.Character", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<string>("Abbreviation")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.ToTable("Characters");
                });

            modelBuilder.Entity("TekkenStats.Core.Entities.CharacterInfo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("CharacterId")
                        .HasColumnType("integer");

                    b.Property<DateOnly>("LastPlayed")
                        .HasColumnType("date");

                    b.Property<int>("LossCount")
                        .HasColumnType("integer");

                    b.Property<string>("PlayerId")
                        .IsRequired()
                        .HasColumnType("character varying(50)");

                    b.Property<int>("Rating")
                        .HasColumnType("integer");

                    b.Property<int>("WinCount")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CharacterId");

                    b.HasIndex("PlayerId");

                    b.ToTable("CharacterInfos");
                });

            modelBuilder.Entity("TekkenStats.Core.Entities.Player", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<int>("LossCount")
                        .HasColumnType("integer");

                    b.Property<int>("WinCount")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("TekkenStats.Core.Entities.PlayerName", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("NormalizedName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PlayerId")
                        .IsRequired()
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName");

                    b.HasIndex("PlayerId");

                    b.ToTable("PlayerNames");
                });

            modelBuilder.Entity("TekkenStats.Core.Entities.ProcessedMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("MessageId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("MessageId");

                    b.ToTable("ProcessedMessages");
                });

            modelBuilder.Entity("TekkenStats.Core.Entities.Battle", b =>
                {
                    b.HasOne("TekkenStats.Core.Entities.Player", "Player1")
                        .WithMany("Player1Battles")
                        .HasForeignKey("Player1Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TekkenStats.Core.Entities.Player", "Player2")
                        .WithMany("Player2Battles")
                        .HasForeignKey("Player2Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TekkenStats.Core.Entities.Character", "PlayerCharacter1")
                        .WithMany("Character1Battles")
                        .HasForeignKey("PlayerCharacter1Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TekkenStats.Core.Entities.Character", "PlayerCharacter2")
                        .WithMany("Character2Battles")
                        .HasForeignKey("PlayerCharacter2Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player1");

                    b.Navigation("Player2");

                    b.Navigation("PlayerCharacter1");

                    b.Navigation("PlayerCharacter2");
                });

            modelBuilder.Entity("TekkenStats.Core.Entities.CharacterInfo", b =>
                {
                    b.HasOne("TekkenStats.Core.Entities.Character", "Character")
                        .WithMany("CharacterInfos")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TekkenStats.Core.Entities.Player", "Player")
                        .WithMany("CharacterInfos")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Character");

                    b.Navigation("Player");
                });

            modelBuilder.Entity("TekkenStats.Core.Entities.PlayerName", b =>
                {
                    b.HasOne("TekkenStats.Core.Entities.Player", "Player")
                        .WithMany("Names")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");
                });

            modelBuilder.Entity("TekkenStats.Core.Entities.Character", b =>
                {
                    b.Navigation("Character1Battles");

                    b.Navigation("Character2Battles");

                    b.Navigation("CharacterInfos");
                });

            modelBuilder.Entity("TekkenStats.Core.Entities.Player", b =>
                {
                    b.Navigation("CharacterInfos");

                    b.Navigation("Names");

                    b.Navigation("Player1Battles");

                    b.Navigation("Player2Battles");
                });
#pragma warning restore 612, 618
        }
    }
}
