/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using Microsoft.EntityFrameworkCore.Migrations;

namespace Link.Data.Migrations
{
    public partial class AddCommentGroupTypeFieldsToComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommentGroupTypeId",
                table: "Comment",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comment_CommentGroupTypeId",
                table: "Comment",
                column: "CommentGroupTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_CommentGroupType_CommentGroupTypeId",
                table: "Comment",
                column: "CommentGroupTypeId",
                principalTable: "CommentGroupType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_CommentGroupType_CommentGroupTypeId",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_CommentGroupTypeId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "CommentGroupTypeId",
                table: "Comment");
        }
    }
}
