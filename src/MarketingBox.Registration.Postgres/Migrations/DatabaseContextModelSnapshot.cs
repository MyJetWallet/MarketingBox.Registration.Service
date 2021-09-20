﻿// <auto-generated />
using System;
using MarketingBox.Registration.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MarketingBox.Registration.Postgres.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("lead-service")
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.9")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("MarketingBox.Registration.Postgres.Entities.Lead.LeadEntity", b =>
                {
                    b.Property<long>("LeadId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<string>("Ip")
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .HasColumnType("text");

                    b.Property<long>("Sequence")
                        .HasColumnType("bigint");

                    b.Property<string>("TenantId")
                        .HasColumnType("text");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("LeadId");

                    b.HasIndex("TenantId", "LeadId");

                    b.ToTable("leads");
                });

            modelBuilder.Entity("MarketingBox.Registration.Postgres.Entities.Lead.LeadEntity", b =>
                {
                    b.OwnsOne("MarketingBox.Registration.Postgres.Entities.Lead.LeadAdditionalInfo", "AdditionalInfo", b1 =>
                        {
                            b1.Property<long>("LeadEntityLeadId")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("bigint")
                                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                            b1.Property<string>("MPC_1")
                                .HasColumnType("text");

                            b1.Property<string>("MPC_10")
                                .HasColumnType("text");

                            b1.Property<string>("MPC_2")
                                .HasColumnType("text");

                            b1.Property<string>("MPC_3")
                                .HasColumnType("text");

                            b1.Property<string>("MPC_4")
                                .HasColumnType("text");

                            b1.Property<string>("MPC_5")
                                .HasColumnType("text");

                            b1.Property<string>("MPC_6")
                                .HasColumnType("text");

                            b1.Property<string>("MPC_7")
                                .HasColumnType("text");

                            b1.Property<string>("MPC_8")
                                .HasColumnType("text");

                            b1.Property<string>("MPC_9")
                                .HasColumnType("text");

                            b1.Property<string>("So")
                                .HasColumnType("text");

                            b1.Property<string>("Sub")
                                .HasColumnType("text");

                            b1.HasKey("LeadEntityLeadId");

                            b1.ToTable("leads");

                            b1.WithOwner()
                                .HasForeignKey("LeadEntityLeadId");
                        });

                    b.OwnsOne("MarketingBox.Registration.Postgres.Entities.Lead.LeadGeneralInfo", "GeneralInfo", b1 =>
                        {
                            b1.Property<long>("LeadEntityLeadId")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("bigint")
                                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                            b1.Property<long>("AffiliateId")
                                .HasColumnType("bigint");

                            b1.Property<long>("BoxId")
                                .HasColumnType("bigint");

                            b1.Property<long>("CampaignId")
                                .HasColumnType("bigint");

                            b1.HasKey("LeadEntityLeadId");

                            b1.ToTable("leads");

                            b1.WithOwner()
                                .HasForeignKey("LeadEntityLeadId");
                        });

                    b.Navigation("AdditionalInfo");

                    b.Navigation("GeneralInfo");
                });
#pragma warning restore 612, 618
        }
    }
}