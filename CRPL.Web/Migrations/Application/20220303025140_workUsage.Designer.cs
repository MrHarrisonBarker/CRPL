﻿// <auto-generated />
using System;
using CRPL.Data.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CRPL.Web.Migrations.Application
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20220303025140_workUsage")]
    partial class workUsage
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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

                    b.Property<string>("Cid")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime(6)");

                    b.Property<byte[]>("Hash")
                        .HasColumnType("longblob");

                    b.Property<DateTime?>("LastPing")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("LastProxyUse")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("ProposalTransactionId")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("Registered")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("RegisteredTransactionId")
                        .HasColumnType("longtext");

                    b.Property<string>("RightId")
                        .HasColumnType("longtext");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<long>("TimesPinged")
                        .HasColumnType("bigint");

                    b.Property<long>("TimesProxyUsed")
                        .HasColumnType("bigint");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("WorkType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("RegisteredWorks");
                });

            modelBuilder.Entity("CRPL.Data.Account.UserAccount", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("AuthenticationToken")
                        .HasColumnType("longtext");

                    b.Property<string>("DialCode")
                        .HasColumnType("longtext");

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

            modelBuilder.Entity("CRPL.Data.Account.UserWork", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("WorkId")
                        .HasColumnType("char(36)");

                    b.HasKey("UserId", "WorkId");

                    b.HasIndex("WorkId");

                    b.ToTable("UserWorks");
                });

            modelBuilder.Entity("CRPL.Data.Applications.Application", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("ApplicationType")
                        .HasColumnType("int");

                    b.Property<Guid?>("AssociatedWorkId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("Modified")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("TransactionId")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("AssociatedWorkId");

                    b.ToTable("Applications");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Application");
                });

            modelBuilder.Entity("CRPL.Data.Applications.UserApplication", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("ApplicationId")
                        .HasColumnType("char(36)");

                    b.HasKey("UserId", "ApplicationId");

                    b.HasIndex("ApplicationId");

                    b.ToTable("UserApplications");
                });

            modelBuilder.Entity("CRPL.Data.Applications.DataModels.DeleteAccountApplication", b =>
                {
                    b.HasBaseType("CRPL.Data.Applications.Application");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("char(36)");

                    b.HasDiscriminator().HasValue("DeleteAccountApplication");
                });

            modelBuilder.Entity("CRPL.Data.Applications.DataModels.DisputeApplication", b =>
                {
                    b.HasBaseType("CRPL.Data.Applications.Application");

                    b.Property<string>("ContactAddress")
                        .HasColumnType("longtext");

                    b.Property<int>("DisputeType")
                        .HasColumnType("int");

                    b.Property<int?>("ExpectedRecourse")
                        .HasColumnType("int");

                    b.Property<Guid?>("ExpectedRecourseApplicationId")
                        .HasColumnType("char(36)");

                    b.Property<string>("ExpectedRecourseData")
                        .HasColumnType("longtext");

                    b.Property<int?>("Infractions")
                        .HasColumnType("int");

                    b.Property<string>("LinkToInfraction")
                        .HasColumnType("longtext");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("Spotted")
                        .HasColumnType("datetime(6)");

                    b.HasIndex("ExpectedRecourseApplicationId");

                    b.HasDiscriminator().HasValue("DisputeApplication");
                });

            modelBuilder.Entity("CRPL.Data.Applications.DataModels.WalletTransferApplication", b =>
                {
                    b.HasBaseType("CRPL.Data.Applications.Application");

                    b.Property<string>("WalletAddress")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasDiscriminator().HasValue("WalletTransferApplication");
                });

            modelBuilder.Entity("CRPL.Data.Applications.OwnershipRestructureApplication", b =>
                {
                    b.HasBaseType("CRPL.Data.Applications.Application");

                    b.Property<int>("BindStatus")
                        .HasColumnType("int");

                    b.Property<string>("CurrentStructure")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid?>("OriginId")
                        .HasColumnType("char(36)");

                    b.Property<string>("ProposedStructure")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("RestructureReason")
                        .HasColumnType("int");

                    b.HasIndex("OriginId");

                    b.HasDiscriminator().HasValue("OwnershipRestructureApplication");
                });

            modelBuilder.Entity("CRPL.Data.Applications.ViewModels.CopyrightRegistrationApplication", b =>
                {
                    b.HasBaseType("CRPL.Data.Applications.Application");

                    b.Property<string>("Legal")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("OwnershipStakes")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<byte[]>("WorkHash")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<int>("WorkType")
                        .HasColumnType("int");

                    b.Property<string>("WorkUri")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("YearsExpire")
                        .HasColumnType("int");

                    b.HasDiscriminator().HasValue("CopyrightRegistrationApplication");
                });

            modelBuilder.Entity("CRPL.Data.Account.RegisteredWork", b =>
                {
                    b.OwnsOne("CRPL.Data.Workds.VerificationResult", "VerificationResult", b1 =>
                        {
                            b1.Property<Guid>("RegisteredWorkId")
                                .HasColumnType("char(36)");

                            b1.Property<Guid?>("Collision")
                                .HasColumnType("char(36)");

                            b1.Property<bool>("IsAuthentic")
                                .HasColumnType("tinyint(1)");

                            b1.HasKey("RegisteredWorkId");

                            b1.ToTable("RegisteredWorks");

                            b1.WithOwner()
                                .HasForeignKey("RegisteredWorkId");
                        });

                    b.Navigation("VerificationResult");
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

                            b1.Property<string>("Nonce")
                                .HasColumnType("longtext");

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

            modelBuilder.Entity("CRPL.Data.Account.UserWork", b =>
                {
                    b.HasOne("CRPL.Data.Account.UserAccount", "UserAccount")
                        .WithMany("UserWorks")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CRPL.Data.Account.RegisteredWork", "RegisteredWork")
                        .WithMany("UserWorks")
                        .HasForeignKey("WorkId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RegisteredWork");

                    b.Navigation("UserAccount");
                });

            modelBuilder.Entity("CRPL.Data.Applications.Application", b =>
                {
                    b.HasOne("CRPL.Data.Account.RegisteredWork", "AssociatedWork")
                        .WithMany("AssociatedApplication")
                        .HasForeignKey("AssociatedWorkId");

                    b.Navigation("AssociatedWork");
                });

            modelBuilder.Entity("CRPL.Data.Applications.UserApplication", b =>
                {
                    b.HasOne("CRPL.Data.Applications.Application", "Application")
                        .WithMany("AssociatedUsers")
                        .HasForeignKey("ApplicationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CRPL.Data.Account.UserAccount", "UserAccount")
                        .WithMany("Applications")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Application");

                    b.Navigation("UserAccount");
                });

            modelBuilder.Entity("CRPL.Data.Applications.DataModels.DisputeApplication", b =>
                {
                    b.HasOne("CRPL.Data.Applications.Application", "ExpectedRecourseApplication")
                        .WithMany()
                        .HasForeignKey("ExpectedRecourseApplicationId");

                    b.OwnsOne("CRPL.Data.Applications.Core.ResolveResult", "ResolveResult", b1 =>
                        {
                            b1.Property<Guid>("DisputeApplicationId")
                                .HasColumnType("char(36)");

                            b1.Property<string>("Message")
                                .HasColumnType("longtext");

                            b1.Property<bool>("Rejected")
                                .HasColumnType("tinyint(1)");

                            b1.Property<int>("ResolvedStatus")
                                .HasColumnType("int");

                            b1.Property<string>("Transaction")
                                .HasColumnType("longtext");

                            b1.HasKey("DisputeApplicationId");

                            b1.ToTable("Applications");

                            b1.WithOwner()
                                .HasForeignKey("DisputeApplicationId");
                        });

                    b.Navigation("ExpectedRecourseApplication");

                    b.Navigation("ResolveResult")
                        .IsRequired();
                });

            modelBuilder.Entity("CRPL.Data.Applications.OwnershipRestructureApplication", b =>
                {
                    b.HasOne("CRPL.Data.Applications.Application", "Origin")
                        .WithMany()
                        .HasForeignKey("OriginId");

                    b.Navigation("Origin");
                });

            modelBuilder.Entity("CRPL.Data.Applications.ViewModels.CopyrightRegistrationApplication", b =>
                {
                    b.OwnsOne("CRPL.Contracts.Structs.Protections", "Protections", b1 =>
                        {
                            b1.Property<Guid>("CopyrightRegistrationApplicationId")
                                .HasColumnType("char(36)");

                            b1.Property<bool>("Authorship")
                                .HasColumnType("tinyint(1)");

                            b1.Property<bool>("CommercialAdaptation")
                                .HasColumnType("tinyint(1)");

                            b1.Property<bool>("CommercialDistribution")
                                .HasColumnType("tinyint(1)");

                            b1.Property<bool>("CommercialPerformance")
                                .HasColumnType("tinyint(1)");

                            b1.Property<bool>("CommercialReproduction")
                                .HasColumnType("tinyint(1)");

                            b1.Property<bool>("NonCommercialAdaptation")
                                .HasColumnType("tinyint(1)");

                            b1.Property<bool>("NonCommercialDistribution")
                                .HasColumnType("tinyint(1)");

                            b1.Property<bool>("NonCommercialPerformance")
                                .HasColumnType("tinyint(1)");

                            b1.Property<bool>("NonCommercialReproduction")
                                .HasColumnType("tinyint(1)");

                            b1.Property<bool>("ReviewOrCrit")
                                .HasColumnType("tinyint(1)");

                            b1.HasKey("CopyrightRegistrationApplicationId");

                            b1.ToTable("Applications");

                            b1.WithOwner()
                                .HasForeignKey("CopyrightRegistrationApplicationId");
                        });

                    b.Navigation("Protections")
                        .IsRequired();
                });

            modelBuilder.Entity("CRPL.Data.Account.RegisteredWork", b =>
                {
                    b.Navigation("AssociatedApplication");

                    b.Navigation("UserWorks");
                });

            modelBuilder.Entity("CRPL.Data.Account.UserAccount", b =>
                {
                    b.Navigation("Applications");

                    b.Navigation("UserWorks");
                });

            modelBuilder.Entity("CRPL.Data.Applications.Application", b =>
                {
                    b.Navigation("AssociatedUsers");
                });
#pragma warning restore 612, 618
        }
    }
}
