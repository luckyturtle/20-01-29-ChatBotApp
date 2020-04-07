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
    public partial class AddCommentTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommentTransactionType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentTransactionType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommentTransaction",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CommentId = table.Column<string>(nullable: true),
                    CommentRoot = table.Column<string>(nullable: true),
                    CommentTransactionTypeId = table.Column<int>(nullable: false),
                    Data = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentTransaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentTransaction_CommentTransactionType_CommentTransactionTypeId",
                        column: x => x.CommentTransactionTypeId,
                        principalTable: "CommentTransactionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentTransaction_CommentTransactionTypeId",
                table: "CommentTransaction",
                column: "CommentTransactionTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentTransaction");

            migrationBuilder.DropTable(
                name: "CommentTransactionType");
        }
    }
}
