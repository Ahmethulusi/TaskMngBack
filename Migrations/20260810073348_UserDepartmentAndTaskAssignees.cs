using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskMngBack.Migrations
{
    /// <inheritdoc />
    public partial class UserDepartmentAndTaskAssignees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_AssignedToUserId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_AssignedToUserId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "AssignedToUserId",
                table: "Tasks");

            migrationBuilder.CreateTable(
                name: "TaskAssignees",
                columns: table => new
                {
                    AssignedTasksId = table.Column<int>(type: "integer", nullable: false),
                    AssignedUsersId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskAssignees", x => new { x.AssignedTasksId, x.AssignedUsersId });
                    table.ForeignKey(
                        name: "FK_TaskAssignees_Tasks_AssignedTasksId",
                        column: x => x.AssignedTasksId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskAssignees_Users_AssignedUsersId",
                        column: x => x.AssignedUsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDepartments",
                columns: table => new
                {
                    DepartmentsId = table.Column<int>(type: "integer", nullable: false),
                    UsersId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDepartments", x => new { x.DepartmentsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_UserDepartments_Departments_DepartmentsId",
                        column: x => x.DepartmentsId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserDepartments_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$12$t8fu4r4T2x5yE5xJbZjYEOqmJxiAz7iiGMTtBv4I3Fc0Az4fZwMsu");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAssignees_AssignedUsersId",
                table: "TaskAssignees",
                column: "AssignedUsersId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDepartments_UsersId",
                table: "UserDepartments",
                column: "UsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskAssignees");

            migrationBuilder.DropTable(
                name: "UserDepartments");

            migrationBuilder.AddColumn<int>(
                name: "AssignedToUserId",
                table: "Tasks",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$12$SaEu4o5t1yVB8Ko3jiSPd.vqf71KmA9Ctan2JHy2mVL4jQp/xHYwm");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AssignedToUserId",
                table: "Tasks",
                column: "AssignedToUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_AssignedToUserId",
                table: "Tasks",
                column: "AssignedToUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
