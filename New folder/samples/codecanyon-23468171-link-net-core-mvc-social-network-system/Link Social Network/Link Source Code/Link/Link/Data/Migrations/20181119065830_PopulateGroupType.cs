/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using Microsoft.EntityFrameworkCore.Migrations;

namespace Link.Data.Migrations
{
    public partial class PopulateGroupType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
          table: "GroupType",
          columns: new[] { "Id", "Name" },
          values: new object[] { 1, "Public" });

            migrationBuilder.InsertData(
           table: "GroupType",
           columns: new[] { "Id", "Name" },
           values: new object[] { 2, "Private" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
