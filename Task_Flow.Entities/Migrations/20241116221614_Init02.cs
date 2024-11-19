using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task_Flow.Entities.Migrations
{
    /// <inheritdoc />
    public partial class Init02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NewProjectProposals",
                table: "NotificationSettings",
                newName: "TaskDueDate");

            migrationBuilder.RenameColumn(
                name: "InternalTeamMessages",
                table: "NotificationSettings",
                newName: "ProjectCompletationDate");

            migrationBuilder.RenameColumn(
                name: "IncomingComments",
                table: "NotificationSettings",
                newName: "NewTaskWithInProject");

            migrationBuilder.RenameColumn(
                name: "DeadlineReminders",
                table: "NotificationSettings",
                newName: "InnovationNewProject");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TaskDueDate",
                table: "NotificationSettings",
                newName: "NewProjectProposals");

            migrationBuilder.RenameColumn(
                name: "ProjectCompletationDate",
                table: "NotificationSettings",
                newName: "InternalTeamMessages");

            migrationBuilder.RenameColumn(
                name: "NewTaskWithInProject",
                table: "NotificationSettings",
                newName: "IncomingComments");

            migrationBuilder.RenameColumn(
                name: "InnovationNewProject",
                table: "NotificationSettings",
                newName: "DeadlineReminders");
        }
    }
}
