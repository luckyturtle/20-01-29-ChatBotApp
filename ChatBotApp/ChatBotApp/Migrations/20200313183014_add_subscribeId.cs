using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ChatBotApp.Migrations
{
    public partial class add_subscribeId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QnAMapping");

            migrationBuilder.AddColumn<string>(
                name: "SubscribeId",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubscribeId",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "QnAMapping",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnswerAth = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnswerTxt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Freq = table.Column<int>(type: "int", nullable: false),
                    QueryAth = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QueryTxt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    createdBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QnAMapping", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QnAMapping_Id",
                table: "QnAMapping",
                column: "Id");
        }
    }
}
