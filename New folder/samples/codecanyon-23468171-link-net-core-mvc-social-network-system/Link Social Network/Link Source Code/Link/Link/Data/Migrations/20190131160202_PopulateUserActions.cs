/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using Microsoft.EntityFrameworkCore.Migrations;

namespace Link.Data.Migrations
{
    public partial class PopulateUserActions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
            table: "UserAction",
            columns: new[] { "Id", "Name" },
            values: new object[] { 0, "LogOut" });

            migrationBuilder.InsertData(
            table: "UserAction",
            columns: new[] { "Id", "Name" },
            values: new object[] { 1, "LogIn" });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
