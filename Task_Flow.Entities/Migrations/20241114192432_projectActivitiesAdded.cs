using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task_Flow.Entities.Migrations
{
    /// <inheritdoc />
    public partial class projectActivitiesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectActivities_AspNetUsers_CustomUserId",
                table: "ProjectActivities");

            migrationBuilder.DropIndex(
                name: "IX_ProjectActivities_CustomUserId",
                table: "ProjectActivities");

            migrationBuilder.DropColumn(
                name: "CustomUserId",
                table: "ProjectActivities");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ProjectActivities",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectActivities_UserId",
                table: "ProjectActivities",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectActivities_AspNetUsers_UserId",
                table: "ProjectActivities",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectActivities_AspNetUsers_UserId",
                table: "ProjectActivities");

            migrationBuilder.DropIndex(
                name: "IX_ProjectActivities_UserId",
                table: "ProjectActivities");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ProjectActivities",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "CustomUserId",
                table: "ProjectActivities",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectActivities_CustomUserId",
                table: "ProjectActivities",
                column: "CustomUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectActivities_AspNetUsers_CustomUserId",
                table: "ProjectActivities",
                column: "CustomUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
