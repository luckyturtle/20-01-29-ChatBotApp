/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using Microsoft.EntityFrameworkCore.Migrations;

namespace Link.Data.Migrations
{
    public partial class PopulateNotificationsType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.InsertData(
         table: "NotificationType",
         columns: new[] { "Id", "Name" },
         values: new object[] { 1, "NewChatMessage" });

            migrationBuilder.InsertData(
         table: "NotificationType",
         columns: new[] { "Id", "Name" },
         values: new object[] { 2, "System" });

            migrationBuilder.InsertData(
         table: "NotificationType",
         columns: new[] { "Id", "Name" },
         values: new object[] { 3, "GroupRequest" });

            migrationBuilder.InsertData(
         table: "NotificationType",
         columns: new[] { "Id", "Name" },
         values: new object[] { 4, "AddNewComment" });

            migrationBuilder.InsertData(
         table: "NotificationType",
         columns: new[] { "Id", "Name" },
         values: new object[] { 5, "AddReplyOnComment" });

            migrationBuilder.InsertData(
         table: "NotificationType",
         columns: new[] { "Id", "Name" },
         values: new object[] { 6, "VoteComment" });

            migrationBuilder.InsertData(
        table: "NotificationType",
        columns: new[] { "Id", "Name" },
        values: new object[] { 7, "FriendRequest" });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
