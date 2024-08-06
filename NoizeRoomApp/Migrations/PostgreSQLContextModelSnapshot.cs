﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NoizeRoomApp.Database;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NoizeRoomApp.Migrations
{
    [DbContext(typeof(PostgreSQLContext))]
    partial class PostgreSQLContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("NoizeRoomApp.Database.Models.BookingEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("BookerId")
                        .HasColumnType("uuid");

                    b.Property<string>("BookerName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("TimeFrom")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("TimeTo")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("BookerId");

                    b.ToTable("Bookings");
                });

            modelBuilder.Entity("NoizeRoomApp.Database.Models.NotifyEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Notifies");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "noNotify"
                        },
                        new
                        {
                            Id = 2,
                            Name = "emailNotify"
                        },
                        new
                        {
                            Id = 3,
                            Name = "vkNotify"
                        });
                });

            modelBuilder.Entity("NoizeRoomApp.Database.Models.RoleEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Admin"
                        },
                        new
                        {
                            Id = 2,
                            Name = "User"
                        });
                });

            modelBuilder.Entity("NoizeRoomApp.Database.Models.UserEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AccessToken")
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("NotifyTypeId")
                        .HasColumnType("integer");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("NotifyTypeId");

                    b.HasIndex("RoleId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("NoizeRoomApp.Database.Models.BookingEntity", b =>
                {
                    b.HasOne("NoizeRoomApp.Database.Models.UserEntity", "User")
                        .WithMany("Bookings")
                        .HasForeignKey("BookerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("NoizeRoomApp.Database.Models.UserEntity", b =>
                {
                    b.HasOne("NoizeRoomApp.Database.Models.NotifyEntity", "Notify")
                        .WithMany("Users")
                        .HasForeignKey("NotifyTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("NoizeRoomApp.Database.Models.RoleEntity", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Notify");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("NoizeRoomApp.Database.Models.NotifyEntity", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("NoizeRoomApp.Database.Models.RoleEntity", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("NoizeRoomApp.Database.Models.UserEntity", b =>
                {
                    b.Navigation("Bookings");
                });
#pragma warning restore 612, 618
        }
    }
}
