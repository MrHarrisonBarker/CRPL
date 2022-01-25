﻿// <auto-generated />
using System;
using CRPL.Data.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CRPL.Web.Migrations.Application
{
    [DbContext(typeof(ApplicationContext))]
    partial class ApplicationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("CRPL.Data.Account.RegisteredWork", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("RightId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid?>("UserAccountId")
                        .HasColumnType("char(36)");

                    b.Property<string>("WalletAddress")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("UserAccountId");

                    b.ToTable("RegisteredWorks");
                });

            modelBuilder.Entity("CRPL.Data.Account.UserAccount", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Email")
                        .HasColumnType("longtext");

                    b.Property<string>("FirstName")
                        .HasColumnType("longtext");

                    b.Property<string>("LastName")
                        .HasColumnType("longtext");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("longtext");

                    b.Property<string>("RegisteredJurisdiction")
                        .HasColumnType("longtext");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("UserAccounts");
                });

            modelBuilder.Entity("CRPL.Data.Account.RegisteredWork", b =>
                {
                    b.HasOne("CRPL.Data.Account.UserAccount", null)
                        .WithMany("RegisteredWorks")
                        .HasForeignKey("UserAccountId");
                });

            modelBuilder.Entity("CRPL.Data.Account.UserAccount", b =>
                {
                    b.OwnsOne("CRPL.Data.Account.UserAccount+DOB", "DateOfBirth", b1 =>
                        {
                            b1.Property<Guid>("UserAccountId")
                                .HasColumnType("char(36)");

                            b1.Property<int>("Day")
                                .HasColumnType("int");

                            b1.Property<int>("Month")
                                .HasColumnType("int");

                            b1.Property<int>("Year")
                                .HasColumnType("int");

                            b1.HasKey("UserAccountId");

                            b1.ToTable("UserAccounts");

                            b1.WithOwner()
                                .HasForeignKey("UserAccountId");
                        });

                    b.OwnsOne("CRPL.Data.Account.UserWallet", "Wallet", b1 =>
                        {
                            b1.Property<Guid>("UserAccountId")
                                .HasColumnType("char(36)");

                            b1.Property<byte[]>("Nonce")
                                .IsRequired()
                                .HasColumnType("longblob");

                            b1.Property<string>("PublicAddress")
                                .IsRequired()
                                .HasColumnType("longtext");

                            b1.HasKey("UserAccountId");

                            b1.ToTable("UserAccounts");

                            b1.WithOwner()
                                .HasForeignKey("UserAccountId");
                        });

                    b.Navigation("DateOfBirth");

                    b.Navigation("Wallet")
                        .IsRequired();
                });

            modelBuilder.Entity("CRPL.Data.Account.UserAccount", b =>
                {
                    b.Navigation("RegisteredWorks");
                });
#pragma warning restore 612, 618
        }
    }
}
