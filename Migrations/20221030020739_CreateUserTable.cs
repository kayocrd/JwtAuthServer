using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JwtAuthServerApi.Migrations
{
    public partial class CreateUserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "varchar(55)", nullable: false),
                    Email = table.Column<string>(type: "varchar(30)", nullable: false),
                    Username = table.Column<string>(type: "varchar(15)", nullable: false),
                    PasswordHash = table.Column<string>(type: "varchar(100)", nullable: false),
                    IsLocked = table.Column<bool>(type: "bool", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Guid);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
