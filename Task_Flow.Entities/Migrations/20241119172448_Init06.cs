using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task_Flow.Entities.Migrations
{
    /// <inheritdoc />
    public partial class Init06 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecentActivities_AspNetUsers_UserId",
                table: "RecentActivities");

            migrationBuilder.AddForeignKey(
                name: "FK_RecentActivities_AspNetUsers_UserId",
                table: "RecentActivities",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecentActivities_AspNetUsers_UserId",
                table: "RecentActivities");

            migrationBuilder.AddForeignKey(
                name: "FK_RecentActivities_AspNetUsers_UserId",
                table: "RecentActivities",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
