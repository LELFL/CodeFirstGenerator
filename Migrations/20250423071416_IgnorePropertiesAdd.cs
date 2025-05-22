using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeFirstGenerator.Migrations
{
    /// <inheritdoc />
    public partial class IgnorePropertiesAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IgnoreProperties",
                table: "EntityInfos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Templates_Name",
                table: "Templates",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Templates_OutputPath",
                table: "Templates",
                column: "OutputPath",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Solutionss_Name",
                table: "Solutionss",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntityInfos_ClassName",
                table: "EntityInfos",
                column: "ClassName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Templates_Name",
                table: "Templates");

            migrationBuilder.DropIndex(
                name: "IX_Templates_OutputPath",
                table: "Templates");

            migrationBuilder.DropIndex(
                name: "IX_Solutionss_Name",
                table: "Solutionss");

            migrationBuilder.DropIndex(
                name: "IX_EntityInfos_ClassName",
                table: "EntityInfos");

            migrationBuilder.DropColumn(
                name: "IgnoreProperties",
                table: "EntityInfos");
        }
    }
}
