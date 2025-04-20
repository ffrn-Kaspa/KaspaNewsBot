using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Telegram.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class Third : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tweeter",
                table: "tweeters");

            migrationBuilder.AddColumn<string>(
                name: "link",
                table: "tweeters",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "tweeters",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "username",
                table: "tweeters",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "link",
                table: "tweeters");

            migrationBuilder.DropColumn(
                name: "name",
                table: "tweeters");

            migrationBuilder.DropColumn(
                name: "username",
                table: "tweeters");

            migrationBuilder.AddColumn<string>(
                name: "tweeter",
                table: "tweeters",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
