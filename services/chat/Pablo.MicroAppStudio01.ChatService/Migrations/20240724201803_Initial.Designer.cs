﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pablo.MicroAppStudio01.ChatService.Data;
using Volo.Abp.EntityFrameworkCore;

#nullable disable

namespace Pablo.MicroAppStudio01.ChatService.Migrations
{
    [DbContext(typeof(ChatServiceDbContext))]
    [Migration("20240724201803_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("_Abp_DatabaseProvider", EfCoreDatabaseProvider.SqlServer)
                .HasAnnotation("ProductVersion", "8.0.4")
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
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ExtraProperties");

                    b.Property<string>("MessageId")
                        .IsRequired()
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
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ExtraProperties");

                    b.HasKey("Id");

                    b.HasIndex("CreationTime");

                    b.ToTable("AbpEventOutbox", (string)null);
                });

            modelBuilder.Entity("Volo.Chat.Conversations.Conversation", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("LastMessage")
                        .HasMaxLength(4096)
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("LastMessage");

                    b.Property<DateTime>("LastMessageDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("LastMessageDate");

                    b.Property<byte>("LastMessageSide")
                        .HasColumnType("tinyint")
                        .HasColumnName("LastMessageSide");

                    b.Property<Guid>("TargetUserId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("TargetUserId");

                    b.Property<Guid?>("TenantId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("TenantId");

                    b.Property<int>("UnreadMessageCount")
                        .HasColumnType("int")
                        .HasColumnName("UnreadMessageCount");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("ChatConversations", (string)null);
                });

            modelBuilder.Entity("Volo.Chat.Messages.Message", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)")
                        .HasColumnName("ConcurrencyStamp");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("CreationTime");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("CreatorId");

                    b.Property<string>("ExtraProperties")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ExtraProperties");

                    b.Property<bool>("IsAllRead")
                        .HasColumnType("bit")
                        .HasColumnName("IsAllRead");

                    b.Property<DateTime?>("ReadTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("ReadTime");

                    b.Property<Guid?>("TenantId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("TenantId");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(4096)
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Text");

                    b.HasKey("Id");

                    b.ToTable("ChatMessages", (string)null);
                });

            modelBuilder.Entity("Volo.Chat.Messages.UserMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ChatMessageId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("ChatMessageId");

                    b.Property<bool>("IsRead")
                        .HasColumnType("bit")
                        .HasColumnName("IsRead");

                    b.Property<DateTime?>("ReadTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("ReadTime");

                    b.Property<byte>("Side")
                        .HasColumnType("tinyint")
                        .HasColumnName("Side");

                    b.Property<Guid?>("TargetUserId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("TargetUserId");

                    b.Property<Guid?>("TenantId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("TenantId");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ChatMessageId");

                    b.HasIndex("UserId");

                    b.HasIndex("UserId", "TargetUserId");

                    b.ToTable("ChatUserMessages", (string)null);
                });

            modelBuilder.Entity("Volo.Chat.Users.ChatUser", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)")
                        .HasColumnName("ConcurrencyStamp");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)")
                        .HasColumnName("Email");

                    b.Property<bool>("EmailConfirmed")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false)
                        .HasColumnName("EmailConfirmed");

                    b.Property<string>("ExtraProperties")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ExtraProperties");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit")
                        .HasColumnName("IsActive");

                    b.Property<string>("Name")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)")
                        .HasColumnName("Name");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(16)
                        .HasColumnType("nvarchar(16)")
                        .HasColumnName("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false)
                        .HasColumnName("PhoneNumberConfirmed");

                    b.Property<string>("Surname")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)")
                        .HasColumnName("Surname");

                    b.Property<Guid?>("TenantId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("TenantId");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)")
                        .HasColumnName("UserName");

                    b.HasKey("Id");

                    b.ToTable("ChatUsers", (string)null);
                });

            modelBuilder.Entity("Volo.Chat.Messages.UserMessage", b =>
                {
                    b.HasOne("Volo.Chat.Messages.Message", null)
                        .WithMany()
                        .HasForeignKey("ChatMessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
