﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Zambon.OrderManagement.Core;

#nullable disable

namespace Zambon.OrderManagement.Core.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20230823002606_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.21")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Zambon.OrderManagement.Core.BusinessEntities.General.Customers", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"), 1L, 1);

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("0");

                    b.Property<string>("Name")
                        .HasColumnType("VARCHAR(500)");

                    b.HasKey("ID");

                    b.ToTable("Customers", "General");
                });

            modelBuilder.Entity("Zambon.OrderManagement.Core.BusinessEntities.Security.RefreshTokens", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"), 1L, 1);

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("DATETIME");

                    b.Property<DateTime>("Expiration")
                        .HasColumnType("DATETIME");

                    b.Property<DateTime?>("RevokedOn")
                        .HasColumnType("DATETIME");

                    b.Property<string>("Token")
                        .HasColumnType("VARCHAR(50)");

                    b.Property<long>("UserID")
                        .HasColumnType("bigint");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.ToTable("RefreshTokens", "Security");
                });

            modelBuilder.Entity("Zambon.OrderManagement.Core.BusinessEntities.Security.Users", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"), 1L, 1);

                    b.Property<string>("Email")
                        .HasColumnType("VARCHAR(200)");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("0");

                    b.Property<string>("Name")
                        .HasColumnType("VARCHAR(500)");

                    b.Property<string>("Password")
                        .HasColumnType("VARCHAR(100)");

                    b.Property<string>("Username")
                        .HasColumnType("VARCHAR(100)");

                    b.HasKey("ID");

                    b.ToTable("Users", "Security");
                });

            modelBuilder.Entity("Zambon.OrderManagement.Core.BusinessEntities.Stock.Orders", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"), 1L, 1);

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("DATETIME");

                    b.Property<long>("CustomerID")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("0");

                    b.HasKey("ID");

                    b.HasIndex("CustomerID");

                    b.ToTable("Orders", "Stock");
                });

            modelBuilder.Entity("Zambon.OrderManagement.Core.BusinessEntities.Stock.OrdersProducts", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"), 1L, 1);

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("0");

                    b.Property<long>("OrderID")
                        .HasColumnType("bigint");

                    b.Property<long>("ProductID")
                        .HasColumnType("bigint");

                    b.Property<int>("Qty")
                        .HasColumnType("int");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("ID");

                    b.HasIndex("OrderID");

                    b.HasIndex("ProductID");

                    b.ToTable("OrdersProducts", "Stock");
                });

            modelBuilder.Entity("Zambon.OrderManagement.Core.BusinessEntities.Stock.Products", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"), 1L, 1);

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("0");

                    b.Property<string>("Name")
                        .HasColumnType("VARCHAR(500)");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("ID");

                    b.ToTable("Products", "Stock");
                });

            modelBuilder.Entity("Zambon.OrderManagement.Core.BusinessEntities.Security.RefreshTokens", b =>
                {
                    b.HasOne("Zambon.OrderManagement.Core.BusinessEntities.Security.Users", "User")
                        .WithMany("RefreshTokens")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Zambon.OrderManagement.Core.BusinessEntities.Stock.Orders", b =>
                {
                    b.HasOne("Zambon.OrderManagement.Core.BusinessEntities.General.Customers", "Customer")
                        .WithMany("Orders")
                        .HasForeignKey("CustomerID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("Zambon.OrderManagement.Core.BusinessEntities.Stock.OrdersProducts", b =>
                {
                    b.HasOne("Zambon.OrderManagement.Core.BusinessEntities.Stock.Orders", "Order")
                        .WithMany("Products")
                        .HasForeignKey("OrderID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Zambon.OrderManagement.Core.BusinessEntities.Stock.Products", "Product")
                        .WithMany()
                        .HasForeignKey("ProductID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Zambon.OrderManagement.Core.BusinessEntities.General.Customers", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("Zambon.OrderManagement.Core.BusinessEntities.Security.Users", b =>
                {
                    b.Navigation("RefreshTokens");
                });

            modelBuilder.Entity("Zambon.OrderManagement.Core.BusinessEntities.Stock.Orders", b =>
                {
                    b.Navigation("Products");
                });
#pragma warning restore 612, 618
        }
    }
}
