using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Telegram.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class Second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "topic_id",
                table: "chats");

            migrationBuilder.AddColumn<int>(
                name: "topic_id",
                table: "chat_regions",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "topic_id",
                table: "chat_regions");

            migrationBuilder.AddColumn<int>(
                name: "topic_id",
                table: "chats",
                type: "integer",
                nullable: true);
        }
    }
}
