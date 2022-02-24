using Microsoft.EntityFrameworkCore.Migrations;

namespace MyNews.Migrations
{
    public partial class NewMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Publication_AspNetUsers_UserId1",
                table: "Publication");

            migrationBuilder.DropForeignKey(
                name: "FK_PublicationItem_Publication_PublicationId",
                table: "PublicationItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PublicationItem",
                table: "PublicationItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Publication",
                table: "Publication");

            migrationBuilder.RenameTable(
                name: "PublicationItem",
                newName: "PublicationItems");

            migrationBuilder.RenameTable(
                name: "Publication",
                newName: "Publications");

            migrationBuilder.RenameIndex(
                name: "IX_PublicationItem_PublicationId",
                table: "PublicationItems",
                newName: "IX_PublicationItems_PublicationId");

            migrationBuilder.RenameIndex(
                name: "IX_Publication_UserId1",
                table: "Publications",
                newName: "IX_Publications_UserId1");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "PublicationItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Publications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PublicationItems",
                table: "PublicationItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Publications",
                table: "Publications",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PublicationItems_Publications_PublicationId",
                table: "PublicationItems",
                column: "PublicationId",
                principalTable: "Publications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Publications_AspNetUsers_UserId1",
                table: "Publications",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PublicationItems_Publications_PublicationId",
                table: "PublicationItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Publications_AspNetUsers_UserId1",
                table: "Publications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Publications",
                table: "Publications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PublicationItems",
                table: "PublicationItems");

            migrationBuilder.RenameTable(
                name: "Publications",
                newName: "Publication");

            migrationBuilder.RenameTable(
                name: "PublicationItems",
                newName: "PublicationItem");

            migrationBuilder.RenameIndex(
                name: "IX_Publications_UserId1",
                table: "Publication",
                newName: "IX_Publication_UserId1");

            migrationBuilder.RenameIndex(
                name: "IX_PublicationItems_PublicationId",
                table: "PublicationItem",
                newName: "IX_PublicationItem_PublicationId");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Publication",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "PublicationItem",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Publication",
                table: "Publication",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PublicationItem",
                table: "PublicationItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Publication_AspNetUsers_UserId1",
                table: "Publication",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PublicationItem_Publication_PublicationId",
                table: "PublicationItem",
                column: "PublicationId",
                principalTable: "Publication",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
