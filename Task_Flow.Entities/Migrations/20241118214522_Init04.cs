using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task_Flow.Entities.Migrations
{
    /// <inheritdoc />
    public partial class Init04 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProjectName",
                table: "RequestNotifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectName",
                table: "RequestNotifications");
        }
    }
}
