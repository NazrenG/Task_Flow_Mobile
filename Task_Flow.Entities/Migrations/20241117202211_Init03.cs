using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task_Flow.Entities.Migrations
{
    /// <inheritdoc />
    public partial class Init03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NotificationType",
                table: "RequestNotifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotificationType",
                table: "RequestNotifications");
        }
    }
}
