using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademicFileSharingProject.DataAccess.Migrations
{
    public partial class mig3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_Media_PostImageId",
                table: "Post");

            migrationBuilder.AlterColumn<long>(
                name: "PostImageId",
                table: "Post",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Media_PostImageId",
                table: "Post",
                column: "PostImageId",
                principalTable: "Media",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_Media_PostImageId",
                table: "Post");

            migrationBuilder.AlterColumn<long>(
                name: "PostImageId",
                table: "Post",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Media_PostImageId",
                table: "Post",
                column: "PostImageId",
                principalTable: "Media",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
