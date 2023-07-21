﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Project.Infrastructure;

#nullable disable

namespace Project.Infrastructure.Migrations
{
    [DbContext(typeof(ProjectDbContext))]
    [Migration("20230721064832_InitSomethingContext")]
    partial class InitSomethingContext
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Project.Domain.SomethingContext.Models.SomethingAggregate", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.ToTable("SomethingAggregates");
                });

            modelBuilder.Entity("Project.Domain.SomethingContext.Models.SomethingValueObject", b =>
                {
                    b.Property<long>("_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<bool>("Boolean")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Number")
                        .HasColumnType("int");

                    b.Property<Guid?>("SomethingAggregateId")
                        .HasColumnType("char(36)");

                    b.Property<string>("String")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("_id");

                    b.HasIndex("SomethingAggregateId");

                    b.ToTable("SomethingValueObjects");
                });

            modelBuilder.Entity("Project.Domain.SomethingContext.Models.SomethingAggregate", b =>
                {
                    b.OwnsOne("Project.Domain.SomethingContext.Models.SomethingEntity", "Entity", b1 =>
                        {
                            b1.Property<Guid>("SomethingAggregateId")
                                .HasColumnType("char(36)");

                            b1.Property<Guid>("Id")
                                .HasColumnType("char(36)")
                                .HasColumnName("EntityId");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("longtext")
                                .HasColumnName("EntityName");

                            b1.HasKey("SomethingAggregateId");

                            b1.ToTable("SomethingAggregates");

                            b1.WithOwner()
                                .HasForeignKey("SomethingAggregateId");
                        });

                    b.Navigation("Entity")
                        .IsRequired();
                });

            modelBuilder.Entity("Project.Domain.SomethingContext.Models.SomethingValueObject", b =>
                {
                    b.HasOne("Project.Domain.SomethingContext.Models.SomethingAggregate", null)
                        .WithMany("ValueObjects")
                        .HasForeignKey("SomethingAggregateId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Project.Domain.SomethingContext.Models.SomethingAggregate", b =>
                {
                    b.Navigation("ValueObjects");
                });
#pragma warning restore 612, 618
        }
    }
}
