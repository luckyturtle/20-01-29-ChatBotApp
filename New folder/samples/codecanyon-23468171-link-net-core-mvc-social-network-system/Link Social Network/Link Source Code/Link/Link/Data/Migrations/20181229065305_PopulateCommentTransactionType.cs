/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using Microsoft.EntityFrameworkCore.Migrations;

namespace Link.Data.Migrations
{
    public partial class PopulateCommentTransactionType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
           table: "CommentTransactionType",
           columns: new[] { "Id", "Name" },
           values: new object[] { 1, "Add" });

            migrationBuilder.InsertData(
           table: "CommentTransactionType",
           columns: new[] { "Id", "Name" },
           values: new object[] { 2, "Edit" });

            migrationBuilder.InsertData(
           table: "CommentTransactionType",
           columns: new[] { "Id", "Name" },
           values: new object[] { 3, "Delete" });

            migrationBuilder.InsertData(
           table: "CommentTransactionType",
           columns: new[] { "Id", "Name" },
           values: new object[] { 4, "Vote" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
