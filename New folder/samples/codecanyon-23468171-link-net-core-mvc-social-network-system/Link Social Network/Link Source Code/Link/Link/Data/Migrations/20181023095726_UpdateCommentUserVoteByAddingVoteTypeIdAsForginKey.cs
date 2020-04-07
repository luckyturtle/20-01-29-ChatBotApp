/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using Microsoft.EntityFrameworkCore.Migrations;

namespace Link.Data.Migrations
{
    public partial class UpdateCommentUserVoteByAddingVoteTypeIdAsForginKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentUserVote_VoteType_VoteTypeId",
                table: "CommentUserVote");

            migrationBuilder.AlterColumn<int>(
                name: "VoteTypeId",
                table: "CommentUserVote",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentUserVote_VoteType_VoteTypeId",
                table: "CommentUserVote",
                column: "VoteTypeId",
                principalTable: "VoteType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentUserVote_VoteType_VoteTypeId",
                table: "CommentUserVote");

            migrationBuilder.AlterColumn<int>(
                name: "VoteTypeId",
                table: "CommentUserVote",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_CommentUserVote_VoteType_VoteTypeId",
                table: "CommentUserVote",
                column: "VoteTypeId",
                principalTable: "VoteType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
