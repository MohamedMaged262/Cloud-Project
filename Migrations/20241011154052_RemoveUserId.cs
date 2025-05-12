using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ZAPLACE.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4dd1709e-25a9-4048-b332-dd6c74be5156");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c14ff353-c8c2-45e1-ba5e-6df9acded52a");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "41f56ded-589e-4c6a-a97c-8b91ce64a510", "98e85b9a-2f24-4d45-80ce-f3f5d143404a" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "41f56ded-589e-4c6a-a97c-8b91ce64a510");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "98e85b9a-2f24-4d45-80ce-f3f5d143404a");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4919333e-ed23-489a-91c5-611f3cb4163a", null, "STUDENT", "STUDENT" },
                    { "6e21cef8-221c-4755-9474-3cf4dcd0740c", null, "TEACHER", "TEACHER" },
                    { "8ad146a1-1896-4a43-9d73-416cd9d6237a", null, "ADMIN", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AboutDescription", "AccessFailedCount", "ConcurrencyStamp", "CreatedOn", "DoB", "Email", "EmailConfirmed", "FullName", "Gender", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "ParentEmailAddress", "ParentPhoneNumber", "PasswordHash", "Permision", "PhoneNumber", "PhoneNumberConfirmed", "ProfilePicture", "RoleName", "SecurityStamp", "TwoFactorEnabled", "UpdatedBy", "UpdatedOn", "UserName", "UserStatus" },
                values: new object[] { "9c1a0fba-ea68-4568-a471-95fddd4e3bf1", null, 0, "e86eebea-2393-46bd-9c33-b37a0ebdd8b6", new DateTime(2024, 10, 11, 18, 40, 51, 602, DateTimeKind.Local).AddTicks(3336), new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@domain.com", true, "Admin User", true, false, null, "ADMIN@DOMAIN.COM", "ADMIN@DOMAIN.COM", null, null, "AQAAAAIAAYagAAAAEG+4pNaAVqAwIN7cqHFFkiEBENdx+xfMQMMestJ9bPFaPccCz3uWF8i9bcY3s118Ng==", 0, "1234567890", true, null, null, "fdc9fe39-595d-4094-ae5f-26aed469f63d", false, null, null, "admin@domain.com", true });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "8ad146a1-1896-4a43-9d73-416cd9d6237a", "9c1a0fba-ea68-4568-a471-95fddd4e3bf1" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4919333e-ed23-489a-91c5-611f3cb4163a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6e21cef8-221c-4755-9474-3cf4dcd0740c");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "8ad146a1-1896-4a43-9d73-416cd9d6237a", "9c1a0fba-ea68-4568-a471-95fddd4e3bf1" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8ad146a1-1896-4a43-9d73-416cd9d6237a");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9c1a0fba-ea68-4568-a471-95fddd4e3bf1");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "41f56ded-589e-4c6a-a97c-8b91ce64a510", null, "ADMIN", "ADMIN" },
                    { "4dd1709e-25a9-4048-b332-dd6c74be5156", null, "STUDENT", "STUDENT" },
                    { "c14ff353-c8c2-45e1-ba5e-6df9acded52a", null, "TEACHER", "TEACHER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AboutDescription", "AccessFailedCount", "ConcurrencyStamp", "CreatedOn", "DoB", "Email", "EmailConfirmed", "FullName", "Gender", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "ParentEmailAddress", "ParentPhoneNumber", "PasswordHash", "Permision", "PhoneNumber", "PhoneNumberConfirmed", "ProfilePicture", "RoleName", "SecurityStamp", "TwoFactorEnabled", "UpdatedBy", "UpdatedOn", "UserId", "UserName", "UserStatus" },
                values: new object[] { "98e85b9a-2f24-4d45-80ce-f3f5d143404a", null, 0, "2f7fa42a-866b-4a33-bb31-4f2591dd38b6", new DateTime(2024, 10, 11, 18, 21, 9, 530, DateTimeKind.Local).AddTicks(3363), new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@domain.com", true, "Admin User", true, false, null, "ADMIN@DOMAIN.COM", "ADMIN@DOMAIN.COM", null, null, "AQAAAAIAAYagAAAAEK6xbC3wmJelJA1470KF3QVkhp+NfTs2SKDIJWB5Tq8C/F84hP6rPAZgUlnXRF1xUQ==", 0, "1234567890", true, null, null, "16305ab4-a3c7-4b6f-bb3d-f3fe0dfed22e", false, null, null, new Guid("00000000-0000-0000-0000-000000000000"), "admin@domain.com", true });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "41f56ded-589e-4c6a-a97c-8b91ce64a510", "98e85b9a-2f24-4d45-80ce-f3f5d143404a" });
        }
    }
}
