using Microsoft.EntityFrameworkCore.Migrations;

namespace CBTenroller.Migrations
{
    public partial class StudentDeptCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeptCode",
                table: "Students",
                type: "varchar(6)",
                maxLength: 6,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "Halls",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "EnforceBatch",
                table: "CourseHalls",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<short>(
                name: "LastNumber",
                table: "CourseHalls",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "StartNumber",
                table: "CourseHalls",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeptCode",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Token",
                table: "Halls");

            migrationBuilder.DropColumn(
                name: "EnforceBatch",
                table: "CourseHalls");

            migrationBuilder.DropColumn(
                name: "LastNumber",
                table: "CourseHalls");

            migrationBuilder.DropColumn(
                name: "StartNumber",
                table: "CourseHalls");
        }
    }
}
