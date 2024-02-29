using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademicFileSharingProject.DataAccess.Migrations
{
    public partial class mig2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blog_Media_MediaId",
                table: "Blog");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Media_ProfileImageId",
                table: "User");

            migrationBuilder.AlterColumn<long>(
                name: "ProfileImageId",
                table: "User",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "MediaId",
                table: "Blog",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_Blog_Media_MediaId",
                table: "Blog",
                column: "MediaId",
                principalTable: "Media",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Media_ProfileImageId",
                table: "User",
                column: "ProfileImageId",
                principalTable: "Media",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blog_Media_MediaId",
                table: "Blog");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Media_ProfileImageId",
                table: "User");

            migrationBuilder.AlterColumn<long>(
                name: "ProfileImageId",
                table: "User",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "MediaId",
                table: "Blog",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Blog_Media_MediaId",
                table: "Blog",
                column: "MediaId",
                principalTable: "Media",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Media_ProfileImageId",
                table: "User",
                column: "ProfileImageId",
                principalTable: "Media",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
