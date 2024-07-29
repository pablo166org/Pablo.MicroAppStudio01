﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pablo.MicroAppStudio01.MicroserviceName.Data;
using Volo.Abp.EntityFrameworkCore;

#nullable disable

namespace Pablo.MicroAppStudio01.MicroserviceName.Migrations
{
    [DbContext(typeof(MicroserviceNameDbContext))]
    partial class MicroserviceNameDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("_Abp_DatabaseProvider", EfCoreDatabaseProvider.SqlServer)
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Volo.Abp.EntityFrameworkCore.DistributedEvents.IncomingEventRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("CreationTime");

                    b.Property<byte[]>("EventData")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("EventName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("ExtraProperties")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ExtraProperties");

                    b.Property<string>("MessageId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("Processed")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ProcessedTime")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("MessageId");

                    b.HasIndex("Processed", "CreationTime");

                    b.ToTable("AbpEventInbox", (string)null);
                });

            modelBuilder.Entity("Volo.Abp.EntityFrameworkCore.DistributedEvents.OutgoingEventRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("CreationTime");

                    b.Property<byte[]>("EventData")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("EventName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("ExtraProperties")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ExtraProperties");

                    b.HasKey("Id");

                    b.HasIndex("CreationTime");

                    b.ToTable("AbpEventOutbox", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
