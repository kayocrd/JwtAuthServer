using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JwtAuthServer.Migrations
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
                    PhoneNumber = table.Column<string>(type: "varchar(15)", nullable: false),
                    Email = table.Column<string>(type: "varchar(30)", nullable: false),
                    Identifier = table.Column<string>(type: "varchar(15)", nullable: false),
                    PasswordHash = table.Column<string>(type: "varchar(150)", nullable: false),
                    RefreshToken = table.Column<string>(type: "varchar(100)", nullable: false),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(1, 1, 1, 3, 0, 0, 0, DateTimeKind.Utc)),
                    ValidationCode = table.Column<string>(type: "varchar(6)", nullable: false),
                    IsVerified = table.Column<bool>(type: "bool", nullable: false),
                    IsLocked = table.Column<bool>(type: "bool", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Guid);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Identifier",
                table: "Users",
                column: "Identifier",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
