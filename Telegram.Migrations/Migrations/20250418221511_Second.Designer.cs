﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Telegram.Db;

#nullable disable

namespace Telegram.Migrations.Migrations
{
    [DbContext(typeof(TelegramDbContext))]
    [Migration("20250418221511_Second")]
    partial class Second
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Telegram.Db.Model.ChatDbo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<long>("ChatIdentificationNumber")
                        .HasColumnType("bigint")
                        .HasColumnName("chat_identification_number");

                    b.Property<string>("ChatName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("chat_name");

                    b.Property<string>("ChatType")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("chat_type");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.HasKey("Id")
                        .HasName("pk_chats");

                    b.HasAlternateKey("ChatIdentificationNumber")
                        .HasName("ak_chats_chat_identification_number");

                    b.ToTable("chats", (string)null);
                });

            modelBuilder.Entity("Telegram.Db.Model.ChatLanguageDbo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint")
                        .HasColumnName("chat_id");

                    b.Property<Guid>("RegionId")
                        .HasColumnType("uuid")
                        .HasColumnName("region_id");

                    b.HasKey("Id")
                        .HasName("pk_chat_language");

                    b.HasIndex("ChatId")
                        .IsUnique()
                        .HasDatabaseName("ix_chat_language_chat_id");

                    b.HasIndex("RegionId")
                        .HasDatabaseName("ix_chat_language_region_id");

                    b.ToTable("chat_language", (string)null);
                });

            modelBuilder.Entity("Telegram.Db.Model.ChatRegionDbo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("ChatId")
                        .HasColumnType("uuid")
                        .HasColumnName("chat_id");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<Guid>("RegionId")
                        .HasColumnType("uuid")
                        .HasColumnName("region_id");

                    b.Property<int?>("TopicId")
                        .HasColumnType("integer")
                        .HasColumnName("topic_id");

                    b.HasKey("Id")
                        .HasName("pk_chat_regions");

                    b.HasIndex("ChatId")
                        .HasDatabaseName("ix_chat_regions_chat_id");

                    b.HasIndex("RegionId")
                        .HasDatabaseName("ix_chat_regions_region_id");

                    b.ToTable("chat_regions", (string)null);
                });

            modelBuilder.Entity("Telegram.Db.Model.RegionDbo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("RegionCode")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("region_code");

                    b.Property<string>("RegionName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("region_name");

                    b.HasKey("Id")
                        .HasName("pk_regions");

                    b.HasAlternateKey("RegionCode")
                        .HasName("ak_regions_region_code");

                    b.HasAlternateKey("RegionName")
                        .HasName("ak_regions_region_name");

                    b.ToTable("regions", (string)null);
                });

            modelBuilder.Entity("Telegram.Db.Model.TweetDbo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Tweet")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("tweet");

                    b.Property<DateTime>("TweetDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("tweet_date")
                        .HasDefaultValueSql("NOW()");

                    b.Property<Guid>("TweeterId")
                        .HasColumnType("uuid")
                        .HasColumnName("tweeter_id");

                    b.HasKey("Id")
                        .HasName("pk_tweets");

                    b.HasIndex("TweeterId")
                        .HasDatabaseName("ix_tweets_tweeter_id");

                    b.ToTable("tweets", (string)null);
                });

            modelBuilder.Entity("Telegram.Db.Model.TweeterDbo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("RegionId")
                        .HasColumnType("uuid")
                        .HasColumnName("region_id");

                    b.Property<string>("Tweeter")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("tweeter");

                    b.HasKey("Id")
                        .HasName("pk_tweeters");

                    b.HasIndex("RegionId")
                        .HasDatabaseName("ix_tweeters_region_id");

                    b.ToTable("tweeters", (string)null);
                });

            modelBuilder.Entity("Telegram.Db.Model.ChatLanguageDbo", b =>
                {
                    b.HasOne("Telegram.Db.Model.ChatDbo", "Chat")
                        .WithOne("Language")
                        .HasForeignKey("Telegram.Db.Model.ChatLanguageDbo", "ChatId")
                        .HasPrincipalKey("Telegram.Db.Model.ChatDbo", "ChatIdentificationNumber")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_chat_language_chats_chat_id");

                    b.HasOne("Telegram.Db.Model.RegionDbo", "Region")
                        .WithMany("ChatLanguages")
                        .HasForeignKey("RegionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_chat_language_allowed_languages_region_id");

                    b.Navigation("Chat");

                    b.Navigation("Region");
                });

            modelBuilder.Entity("Telegram.Db.Model.ChatRegionDbo", b =>
                {
                    b.HasOne("Telegram.Db.Model.ChatDbo", "Chat")
                        .WithMany("Regions")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_chat_regions_chats_chat_id");

                    b.HasOne("Telegram.Db.Model.RegionDbo", "Region")
                        .WithMany("Chats")
                        .HasForeignKey("RegionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_chat_regions_allowed_languages_region_id");

                    b.Navigation("Chat");

                    b.Navigation("Region");
                });

            modelBuilder.Entity("Telegram.Db.Model.TweetDbo", b =>
                {
                    b.HasOne("Telegram.Db.Model.TweeterDbo", "Tweeter")
                        .WithMany("Tweets")
                        .HasForeignKey("TweeterId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_tweets_tweeters_tweeter_id");

                    b.Navigation("Tweeter");
                });

            modelBuilder.Entity("Telegram.Db.Model.TweeterDbo", b =>
                {
                    b.HasOne("Telegram.Db.Model.RegionDbo", "Region")
                        .WithMany("Tweeters")
                        .HasForeignKey("RegionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_tweeters_allowed_languages_region_id");

                    b.Navigation("Region");
                });

            modelBuilder.Entity("Telegram.Db.Model.ChatDbo", b =>
                {
                    b.Navigation("Language")
                        .IsRequired();

                    b.Navigation("Regions");
                });

            modelBuilder.Entity("Telegram.Db.Model.RegionDbo", b =>
                {
                    b.Navigation("ChatLanguages");

                    b.Navigation("Chats");

                    b.Navigation("Tweeters");
                });

            modelBuilder.Entity("Telegram.Db.Model.TweeterDbo", b =>
                {
                    b.Navigation("Tweets");
                });
#pragma warning restore 612, 618
        }
    }
}
