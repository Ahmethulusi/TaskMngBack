using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskMngBack.Migrations
{
    /// <inheritdoc />
    public partial class AdminSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "PasswordHash", "Role" },
                values: new object[]
                {
                    1,
                    new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                    "admin@taskmanager.local",
                    "Sistem Yöneticisi",
                    "$2a$12$SaEu4o5t1yVB8Ko3jiSPd.vqf71KmA9Ctan2JHy2mVL4jQp/xHYwm",
                    0
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
