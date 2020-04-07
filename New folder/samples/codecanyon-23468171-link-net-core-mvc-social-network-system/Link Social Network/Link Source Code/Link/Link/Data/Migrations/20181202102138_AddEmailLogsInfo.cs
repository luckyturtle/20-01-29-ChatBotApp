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
    public partial class AddEmailLogsInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailLogsInfo",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    EmailTypeId = table.Column<int>(nullable: false),
                    From = table.Column<string>(nullable: true),
                    IsSent = table.Column<bool>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    To = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailLogsInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailLogsInfo_EmailType_EmailTypeId",
                        column: x => x.EmailTypeId,
                        principalTable: "EmailType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailLogsInfo_EmailTypeId",
                table: "EmailLogsInfo",
                column: "EmailTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailLogsInfo");

            migrationBuilder.DropTable(
                name: "EmailType");
        }
    }
}
