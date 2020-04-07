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
    public partial class AddNewFieldsToFollowrsModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FollowDate",
                table: "Followers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsSeen",
                table: "Followers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SeenDate",
                table: "Followers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FollowDate",
                table: "Followers");

            migrationBuilder.DropColumn(
                name: "IsSeen",
                table: "Followers");

            migrationBuilder.DropColumn(
                name: "SeenDate",
                table: "Followers");
        }
    }
}
