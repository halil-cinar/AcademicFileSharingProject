using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademicFileSharingProject.DataAccess.Migrations
{
    public partial class mig_update_post : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_Media_PostImageId",
                table: "Post");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_Media_PostVideoId",
                table: "Post");

            migrationBuilder.DropIndex(
                name: "IX_Post_PostImageId",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "PostImageId",
                table: "Post");

            migrationBuilder.RenameColumn(
                name: "PostVideoId",
                table: "Post",
                newName: "PostMediaId");

            migrationBuilder.RenameIndex(
                name: "IX_Post_PostVideoId",
                table: "Post",
                newName: "IX_Post_PostMediaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Media_PostMediaId",
                table: "Post",
                column: "PostMediaId",
                principalTable: "Media",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_Media_PostMediaId",
                table: "Post");

            migrationBuilder.RenameColumn(
                name: "PostMediaId",
                table: "Post",
                newName: "PostVideoId");

            migrationBuilder.RenameIndex(
                name: "IX_Post_PostMediaId",
                table: "Post",
                newName: "IX_Post_PostVideoId");

            migrationBuilder.AddColumn<long>(
                name: "PostImageId",
                table: "Post",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Post_PostImageId",
                table: "Post",
                column: "PostImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Media_PostImageId",
                table: "Post",
                column: "PostImageId",
                principalTable: "Media",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Media_PostVideoId",
                table: "Post",
                column: "PostVideoId",
                principalTable: "Media",
                principalColumn: "Id");
        }
    }
}
