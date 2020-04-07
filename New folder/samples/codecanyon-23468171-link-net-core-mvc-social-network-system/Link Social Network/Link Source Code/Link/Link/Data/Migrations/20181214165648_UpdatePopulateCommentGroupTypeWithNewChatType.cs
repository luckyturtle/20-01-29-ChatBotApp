/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using Microsoft.EntityFrameworkCore.Migrations;

namespace Link.Data.Migrations
{
    public partial class UpdatePopulateCommentGroupTypeWithNewChatType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
             table: "CommentGroupType",
             columns: new[] { "Id", "Name" },
             values: new object[] { 4, "GroupChat" });

            migrationBuilder.InsertData(
            table: "CommentGroupType",
            columns: new[] { "Id", "Name" },
            values: new object[] { 5, "ProfileChat" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
