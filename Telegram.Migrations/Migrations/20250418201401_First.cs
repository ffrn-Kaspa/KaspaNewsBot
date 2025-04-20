using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Telegram.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class First : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "chats",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    chat_name = table.Column<string>(type: "text", nullable: false),
                    chat_identification_number = table.Column<long>(type: "bigint", nullable: false),
                    topic_id = table.Column<int>(type: "integer", nullable: true),
                    chat_type = table.Column<string>(type: "text", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_chats", x => x.id);
                    table.UniqueConstraint("ak_chats_chat_identification_number", x => x.chat_identification_number);
                });

            migrationBuilder.CreateTable(
                name: "regions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    region_code = table.Column<string>(type: "text", nullable: false),
                    region_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_regions", x => x.id);
                    table.UniqueConstraint("ak_regions_region_code", x => x.region_code);
                    table.UniqueConstraint("ak_regions_region_name", x => x.region_name);
                });

            migrationBuilder.CreateTable(
                name: "chat_language",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    chat_id = table.Column<long>(type: "bigint", nullable: false),
                    region_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_chat_language", x => x.id);
                    table.ForeignKey(
                        name: "fk_chat_language_allowed_languages_region_id",
                        column: x => x.region_id,
                        principalTable: "regions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_chat_language_chats_chat_id",
                        column: x => x.chat_id,
                        principalTable: "chats",
                        principalColumn: "chat_identification_number",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "chat_regions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    chat_id = table.Column<Guid>(type: "uuid", nullable: false),
                    region_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_chat_regions", x => x.id);
                    table.ForeignKey(
                        name: "fk_chat_regions_allowed_languages_region_id",
                        column: x => x.region_id,
                        principalTable: "regions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_chat_regions_chats_chat_id",
                        column: x => x.chat_id,
                        principalTable: "chats",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tweeters",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    region_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tweeter = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tweeters", x => x.id);
                    table.ForeignKey(
                        name: "fk_tweeters_allowed_languages_region_id",
                        column: x => x.region_id,
                        principalTable: "regions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tweets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tweeter_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tweet = table.Column<string>(type: "text", nullable: false),
                    tweet_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tweets", x => x.id);
                    table.ForeignKey(
                        name: "fk_tweets_tweeters_tweeter_id",
                        column: x => x.tweeter_id,
                        principalTable: "tweeters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_chat_language_chat_id",
                table: "chat_language",
                column: "chat_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_chat_language_region_id",
                table: "chat_language",
                column: "region_id");

            migrationBuilder.CreateIndex(
                name: "ix_chat_regions_chat_id",
                table: "chat_regions",
                column: "chat_id");

            migrationBuilder.CreateIndex(
                name: "ix_chat_regions_region_id",
                table: "chat_regions",
                column: "region_id");

            migrationBuilder.CreateIndex(
                name: "ix_tweeters_region_id",
                table: "tweeters",
                column: "region_id");

            migrationBuilder.CreateIndex(
                name: "ix_tweets_tweeter_id",
                table: "tweets",
                column: "tweeter_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chat_language");

            migrationBuilder.DropTable(
                name: "chat_regions");

            migrationBuilder.DropTable(
                name: "tweets");

            migrationBuilder.DropTable(
                name: "chats");

            migrationBuilder.DropTable(
                name: "tweeters");

            migrationBuilder.DropTable(
                name: "regions");
        }
    }
}
