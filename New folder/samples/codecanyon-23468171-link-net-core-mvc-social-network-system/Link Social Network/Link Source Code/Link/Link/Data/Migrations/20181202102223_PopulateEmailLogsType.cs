/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using Microsoft.EntityFrameworkCore.Migrations;

namespace Link.Data.Migrations
{
    public partial class PopulateEmailLogsType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
           table: "EmailType",
           columns: new[] { "Id", "Name" },
           values: new object[] { 1, "Registration" });

            migrationBuilder.InsertData(
         table: "EmailType",
         columns: new[] { "Id", "Name" },
         values: new object[] { 2, "ForgotPassword" });


            migrationBuilder.InsertData(
      table: "EmailType",
      columns: new[] { "Id", "Name" },
      values: new object[] { 3, "System" });

            migrationBuilder.InsertData(
      table: "EmailType",
      columns: new[] { "Id", "Name" },
      values: new object[] { 4, "GroupRequest" });

            migrationBuilder.InsertData(
      table: "EmailType",
      columns: new[] { "Id", "Name" },
      values: new object[] { 5, "AddNewComment" });

            migrationBuilder.InsertData(
      table: "EmailType",
      columns: new[] { "Id", "Name" },
      values: new object[] { 6, "AddReplyOnComment" });


            migrationBuilder.InsertData(
      table: "EmailType",
      columns: new[] { "Id", "Name" },
      values: new object[] { 7, "VoteComment" });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
