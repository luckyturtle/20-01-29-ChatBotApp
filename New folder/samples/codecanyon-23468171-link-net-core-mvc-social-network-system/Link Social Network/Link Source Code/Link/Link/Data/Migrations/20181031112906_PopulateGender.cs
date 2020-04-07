/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using Microsoft.EntityFrameworkCore.Migrations;

namespace Link.Data.Migrations
{
    public partial class PopulateGender : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
              table: "Gender",
              columns: new[] { "Id", "Name", "Lang" },
              values: new object[] { 1, "Male", "en" });

            migrationBuilder.InsertData(
             table: "Gender",
             columns: new[] { "Id", "Name", "Lang" },
             values: new object[] { 2, "Female", "en" });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
