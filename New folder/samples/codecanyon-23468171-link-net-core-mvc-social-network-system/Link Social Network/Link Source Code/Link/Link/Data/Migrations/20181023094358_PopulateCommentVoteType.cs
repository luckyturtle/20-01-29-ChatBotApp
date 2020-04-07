/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using Microsoft.EntityFrameworkCore.Migrations;

namespace Link.Data.Migrations
{
    public partial class PopulateCommentVoteType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
              table: "VoteType",
              columns: new[] { "Id", "Name" },
              values: new object[] { 1, "Like" });

            migrationBuilder.InsertData(
             table: "VoteType",
             columns: new[] { "Id", "Name" },
             values: new object[] { 2, "Dislike" });

            migrationBuilder.InsertData(
           table: "VoteType",
           columns: new[] { "Id", "Name" },
           values: new object[] { 3, "Love" });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
