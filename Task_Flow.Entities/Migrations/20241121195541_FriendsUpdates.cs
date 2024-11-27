using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task_Flow.Entities.Migrations
{
    /// <inheritdoc />
    public partial class FriendsUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasRequestPending",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsFriend",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "HasRequestPending",
                table: "Friends",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFriend",
                table: "Friends",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasRequestPending",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "IsFriend",
                table: "Friends");

            migrationBuilder.AddColumn<bool>(
                name: "HasRequestPending",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFriend",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
