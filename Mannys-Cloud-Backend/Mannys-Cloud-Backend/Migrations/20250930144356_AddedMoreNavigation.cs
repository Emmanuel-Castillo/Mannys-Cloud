using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mannys_Cloud_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddedMoreNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Folders_FolderId1",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_FolderId1",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "FolderId1",
                table: "Files");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FolderId1",
                table: "Files",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Files_FolderId1",
                table: "Files",
                column: "FolderId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Folders_FolderId1",
                table: "Files",
                column: "FolderId1",
                principalTable: "Folders",
                principalColumn: "FolderId");
        }
    }
}
