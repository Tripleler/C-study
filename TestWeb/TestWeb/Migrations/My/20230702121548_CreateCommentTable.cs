using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestWeb.Migrations.My
{
    /// <inheritdoc />
    public partial class CreateCommentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Board",
                columns: table => new
                {
                    BoardNo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BoardTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BoardContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BoardWritter = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BoardViewCount = table.Column<int>(type: "int", nullable: false),
                    Deleted = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Board", x => x.BoardNo);
                });

            migrationBuilder.CreateTable(
                name: "USER",
                columns: table => new
                {
                    USER_ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    USER_PWD = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EMAIL_ADRESS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NAME = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER", x => x.USER_ID);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    BoardNo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Board_BoardNo",
                        column: x => x.BoardNo,
                        principalTable: "Board",
                        principalColumn: "BoardNo");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_BoardNo",
                table: "Comments",
                column: "BoardNo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "USER");

            migrationBuilder.DropTable(
                name: "Board");
        }
    }
}
