/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Link.Data.Migrations
{
    public partial class SeedAdminRoleAndUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var roleId = Guid.NewGuid().ToString();
            var adminId = Guid.NewGuid().ToString();

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { roleId, Guid.NewGuid().ToString(), "ADMIN", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] {
                    "Id",
                    "AccessFailedCount",
                    "ConcurrencyStamp",
                    "Email",
                    "EmailConfirmed",
                    "LockoutEnabled",
                    "LockoutEnd",
                    "NormalizedEmail",
                    "NormalizedUserName",
                    "PasswordHash",
                    "PhoneNumber",
                    "PhoneNumberConfirmed",
                    "SecurityStamp",
                    "TwoFactorEnabled",
                    "UserName" },
                values: new object[] {
                    adminId,
                    0,
                   Guid.NewGuid().ToString(),
                    "link@link.com",
                    true,
                    false,
                    null,
                    "link@link.com",
                    "link",
                    "AQAAAAEAACcQAAAAEGg7VgoEXgpq7AvZWnY0g5AddbOaUncs0TDAwOCWhse/ot9UPKsbRgosMU1xl/ypQw==",
                    null,
                    false,
                    "",
                    false,
                    "link" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { adminId, roleId });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
