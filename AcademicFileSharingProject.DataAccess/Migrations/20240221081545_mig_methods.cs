using AcademicFileSharingProject.Entities;
using AcademicFileSharingProject.Entities.Enums;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics;

#nullable disable

namespace AcademicFileSharingProject.DataAccess.Migrations
{
    public partial class mig_methods : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (var item in Enum.GetValues<EMethod>())
            {
                Console.WriteLine(string.Join(",", item));
                migrationBuilder.InsertData(
                table: "RoleMethod",
                columns: new[] { "CreatedTime", "IsDeleted", "Role", "Method" },
                values: new object[,]
                {
                      {
                          DateTime.Now,
                          false,
                          (int)Entities.Enums.ERoles.Admin,
                          (int)item
                      }
                });
            }



        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
