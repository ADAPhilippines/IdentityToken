using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityToken.API.Migrations
{
    public partial class ModelUpdate090820211 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ExpiresIn",
                table: "AuthenticatedIdentities",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiresIn",
                table: "AuthenticatedIdentities");
        }
    }
}
