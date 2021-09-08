using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityToken.API.Migrations
{
    public partial class AddAuthenticatedIdentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_IdentityAuthWallet",
                table: "IdentityAuthWallet");

            migrationBuilder.RenameTable(
                name: "IdentityAuthWallet",
                newName: "IdentityAuthWallets");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdentityAuthWallets",
                table: "IdentityAuthWallets",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AuthenticatedIdentities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "text", nullable: false),
                    PolicyId = table.Column<string>(type: "text", nullable: true),
                    AssetName = table.Column<string>(type: "text", nullable: true),
                    Avatar = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthenticatedIdentities", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthenticatedIdentities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IdentityAuthWallets",
                table: "IdentityAuthWallets");

            migrationBuilder.RenameTable(
                name: "IdentityAuthWallets",
                newName: "IdentityAuthWallet");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdentityAuthWallet",
                table: "IdentityAuthWallet",
                column: "Id");
        }
    }
}
