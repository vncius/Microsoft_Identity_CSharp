using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApp.Identity.Migrations
{
    public partial class ADD_MEMBER_TO_USER : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Member",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Member",
                table: "AspNetUsers");
        }
    }
}
