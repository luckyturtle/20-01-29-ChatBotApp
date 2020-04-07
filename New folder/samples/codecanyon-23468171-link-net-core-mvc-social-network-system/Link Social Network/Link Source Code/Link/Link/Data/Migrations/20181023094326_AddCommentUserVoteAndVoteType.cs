/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Link.Data.Migrations
{
    public partial class AddCommentUserVoteAndVoteType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Comment",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "VoteType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoteType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommentUserVote",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CommentId = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    VoteTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentUserVote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentUserVote_VoteType_VoteTypeId",
                        column: x => x.VoteTypeId,
                        principalTable: "VoteType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentUserVote_VoteTypeId",
                table: "CommentUserVote",
                column: "VoteTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentUserVote");

            migrationBuilder.DropTable(
                name: "VoteType");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Comment",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
