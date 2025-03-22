﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TekkenStats.API.Data;

#nullable disable

namespace TekkenStats.API.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250322201217_init")]
    partial class init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TekkenStats.API.Data.Models.Battle", b =>
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

                    b.ToTable("Battle");
                });

            modelBuilder.Entity("TekkenStats.API.Data.Models.Character", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Abbreviation")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Character");
                });

            modelBuilder.Entity("TekkenStats.API.Data.Models.CharacterInfo", b =>
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

                    b.ToTable("CharacterInfo");
                });

            modelBuilder.Entity("TekkenStats.API.Data.Models.Player", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<int>("LossCount")
                        .HasColumnType("integer");

                    b.Property<int>("WinCount")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Player");
                });

            modelBuilder.Entity("TekkenStats.API.Data.Models.PlayerName", b =>
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

                    b.Property<string>("PlayerId")
                        .IsRequired()
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.ToTable("PlayerName");
                });

            modelBuilder.Entity("TekkenStats.API.Data.Models.Battle", b =>
                {
                    b.HasOne("TekkenStats.API.Data.Models.Player", "Player1")
                        .WithMany("Player1Battles")
                        .HasForeignKey("Player1Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TekkenStats.API.Data.Models.Player", "Player2")
                        .WithMany("Player2Battles")
                        .HasForeignKey("Player2Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TekkenStats.API.Data.Models.Character", "PlayerCharacter1")
                        .WithMany("Character1Battles")
                        .HasForeignKey("PlayerCharacter1Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TekkenStats.API.Data.Models.Character", "PlayerCharacter2")
                        .WithMany("Character2Battles")
                        .HasForeignKey("PlayerCharacter2Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player1");

                    b.Navigation("Player2");

                    b.Navigation("PlayerCharacter1");

                    b.Navigation("PlayerCharacter2");
                });

            modelBuilder.Entity("TekkenStats.API.Data.Models.CharacterInfo", b =>
                {
                    b.HasOne("TekkenStats.API.Data.Models.Character", "Character")
                        .WithMany("CharacterInfos")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TekkenStats.API.Data.Models.Player", "Player")
                        .WithMany("CharacterInfos")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Character");

                    b.Navigation("Player");
                });

            modelBuilder.Entity("TekkenStats.API.Data.Models.PlayerName", b =>
                {
                    b.HasOne("TekkenStats.API.Data.Models.Player", "Player")
                        .WithMany("Names")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");
                });

            modelBuilder.Entity("TekkenStats.API.Data.Models.Character", b =>
                {
                    b.Navigation("Character1Battles");

                    b.Navigation("Character2Battles");

                    b.Navigation("CharacterInfos");
                });

            modelBuilder.Entity("TekkenStats.API.Data.Models.Player", b =>
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
