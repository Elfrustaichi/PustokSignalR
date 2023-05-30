using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PustokBackTask.Migrations
{
    public partial class PhoneAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
 

            migrationBuilder.AddColumn<string>(
                name: "Adress",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

           
        }

        
    }
}
