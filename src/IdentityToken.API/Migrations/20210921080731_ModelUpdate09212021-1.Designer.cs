﻿// <auto-generated />
using System;
using IdentityToken.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace IdentityToken.API.Migrations
{
    [DbContext(typeof(IdentityDbContext))]
    [Migration("20210921080731_ModelUpdate09212021-1")]
    partial class ModelUpdate092120211
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "6.0.0-preview.7.21378.4")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("IdentityToken.Common.Models.AuthenticatedIdentity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("AssetName")
                        .HasColumnType("text");

                    b.Property<string>("Avatar")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long>("ExpiresIn")
                        .HasColumnType("bigint");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PolicyId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("AuthenticatedIdentities", (string)null);
                });

            modelBuilder.Entity("IdentityToken.Common.Models.ChatMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("SenderId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Sent")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("SenderId");

                    b.ToTable("ChatMessages", (string)null);
                });

            modelBuilder.Entity("IdentityToken.Common.Models.ChatUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ConnectedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("ConnectionId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("IdentityId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsOnline")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastActivity")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("IdentityId");

                    b.ToTable("ChatUsers", (string)null);
                });

            modelBuilder.Entity("IdentityToken.Common.Models.IdentityAuthWallet", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("DateUpdated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Mnemonic")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("IdentityAuthWallets", (string)null);
                });

            modelBuilder.Entity("IdentityToken.Common.Models.Profile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("PaymentAddress")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("StakeAddress")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Profiles", (string)null);
                });

            modelBuilder.Entity("IdentityToken.Common.Models.ChatMessage", b =>
                {
                    b.HasOne("IdentityToken.Common.Models.AuthenticatedIdentity", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderId");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("IdentityToken.Common.Models.ChatUser", b =>
                {
                    b.HasOne("IdentityToken.Common.Models.AuthenticatedIdentity", "Identity")
                        .WithMany()
                        .HasForeignKey("IdentityId");

                    b.Navigation("Identity");
                });
#pragma warning restore 612, 618
        }
    }
}
