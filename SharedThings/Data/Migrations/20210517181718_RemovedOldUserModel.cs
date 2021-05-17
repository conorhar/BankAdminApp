using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SharedThings.Data.Migrations
{
    public partial class RemovedOldUserModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "User");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    LoginName = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "binary(64)", fixedLength: true, maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserID);
                });
        }
    }
}
