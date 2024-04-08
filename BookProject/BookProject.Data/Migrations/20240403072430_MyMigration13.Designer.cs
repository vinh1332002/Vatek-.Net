﻿// <auto-generated />
using System;
using BookProject.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BookProject.Data.Migrations
{
    [DbContext(typeof(BookProjectContext))]
    [Migration("20240403072430_MyMigration13")]
    partial class MyMigration13
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("BookProject.Data.Models.BookmarkPage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("BookmarkCheck")
                        .HasColumnType("int");

                    b.Property<int>("DocumentPageId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DocumentPageId")
                        .IsUnique();

                    b.ToTable("BookmarkPage", (string)null);
                });

            modelBuilder.Entity("BookProject.Data.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CategoryName")
                        .IsUnique();

                    b.ToTable("Category", (string)null);
                });

            modelBuilder.Entity("BookProject.Data.Models.DocumentInformation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("DocumentInformation", (string)null);
                });

            modelBuilder.Entity("BookProject.Data.Models.DocumentPage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("BookmarkPageId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DocumentInformationId")
                        .HasColumnType("int");

                    b.Property<byte[]>("FilePath")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<int>("Page")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("DocumentInformationId");

                    b.ToTable("DocumentPage", (string)null);
                });

            modelBuilder.Entity("BookProject.Data.Models.BookmarkPage", b =>
                {
                    b.HasOne("BookProject.Data.Models.DocumentPage", "DocumentPage")
                        .WithOne("Bookmark")
                        .HasForeignKey("BookProject.Data.Models.BookmarkPage", "DocumentPageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DocumentPage");
                });

            modelBuilder.Entity("BookProject.Data.Models.DocumentInformation", b =>
                {
                    b.HasOne("BookProject.Data.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("BookProject.Data.Models.DocumentPage", b =>
                {
                    b.HasOne("BookProject.Data.Models.DocumentInformation", null)
                        .WithMany("DocumentPages")
                        .HasForeignKey("DocumentInformationId");
                });

            modelBuilder.Entity("BookProject.Data.Models.DocumentInformation", b =>
                {
                    b.Navigation("DocumentPages");
                });

            modelBuilder.Entity("BookProject.Data.Models.DocumentPage", b =>
                {
                    b.Navigation("Bookmark");
                });
#pragma warning restore 612, 618
        }
    }
}