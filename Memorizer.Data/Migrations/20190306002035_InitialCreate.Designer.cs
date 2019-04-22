﻿// <auto-generated />
using System;
using Memorizer.Data;
using Memorizer.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Memorizer.Data.Migrations
{
    [DbContext(typeof(MemoContext))]
    [Migration("20190306002035_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Memorizer.Data.Memos", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("A")
                        .IsRequired()
                        .HasMaxLength(4000)
                        .IsUnicode(false);

                    b.Property<int>("PostponeLevel")
                        .HasColumnName("postpone_level");

                    b.Property<string>("Q")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.Property<DateTime>("RepeatDate")
                        .HasColumnName("repeat_date")
                        .HasColumnType("datetime");

                    b.Property<int>("Scores")
                        .HasColumnName("scores");

                    b.HasKey("Id");

                    b.HasIndex("Q")
                        .IsUnique()
                        .HasName("Q_Unique");

                    b.ToTable("memos");
                });
#pragma warning restore 612, 618
        }
    }
}