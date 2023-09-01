﻿// <auto-generated />
using System;
using LazyStockDiaryApi.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace LazyStockDiaryApi.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20230901211722_searchCache4")]
    partial class searchCache4
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("LazyStockDiaryApi.Models.SearchSymbol", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Currency")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Exchange")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ISIN")
                        .HasColumnType("longtext");

                    b.Property<double?>("PreviousClose")
                        .HasColumnType("double");

                    b.Property<DateTime?>("PreviousCloseDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("Code");

                    b.ToTable("SearchSymbol");
                });
#pragma warning restore 612, 618
        }
    }
}
