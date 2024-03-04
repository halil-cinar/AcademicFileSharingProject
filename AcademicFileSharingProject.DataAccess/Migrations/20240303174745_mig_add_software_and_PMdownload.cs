using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademicFileSharingProject.DataAccess.Migrations
{
    public partial class mig_add_software_and_PMdownload : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PostVideoId",
                table: "Post",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PostMediaDownload",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostMediaId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostMediaDownload", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostMediaDownload_PostMedia_PostMediaId",
                        column: x => x.PostMediaId,
                        principalTable: "PostMedia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostMediaDownload_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Software",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileId = table.Column<long>(type: "bigint", nullable: false),
                    DocumentId = table.Column<long>(type: "bigint", nullable: true),
                    LogoId = table.Column<long>(type: "bigint", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    IsAir = table.Column<bool>(type: "bit", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Software", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Software_Media_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Media",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Software_Media_FileId",
                        column: x => x.FileId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Software_Media_LogoId",
                        column: x => x.LogoId,
                        principalTable: "Media",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Software_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Post_PostVideoId",
                table: "Post",
                column: "PostVideoId");

            migrationBuilder.CreateIndex(
                name: "IX_PostMediaDownload_PostMediaId",
                table: "PostMediaDownload",
                column: "PostMediaId");

            migrationBuilder.CreateIndex(
                name: "IX_PostMediaDownload_UserId",
                table: "PostMediaDownload",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Software_DocumentId",
                table: "Software",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Software_FileId",
                table: "Software",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_Software_LogoId",
                table: "Software",
                column: "LogoId");

            migrationBuilder.CreateIndex(
                name: "IX_Software_UserId",
                table: "Software",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Media_PostVideoId",
                table: "Post",
                column: "PostVideoId",
                principalTable: "Media",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_Media_PostVideoId",
                table: "Post");

            migrationBuilder.DropTable(
                name: "PostMediaDownload");

            migrationBuilder.DropTable(
                name: "Software");

            migrationBuilder.DropIndex(
                name: "IX_Post_PostVideoId",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "PostVideoId",
                table: "Post");
        }
    }
}
