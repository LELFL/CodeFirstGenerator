using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeFirstGenerator.Migrations
{
    /// <inheritdoc />
    public partial class ManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Templates_Solutionss_SolutionsId",
                table: "Templates");

            migrationBuilder.DropIndex(
                name: "IX_Templates_SolutionsId",
                table: "Templates");

            migrationBuilder.DropColumn(
                name: "SolutionsId",
                table: "Templates");

            migrationBuilder.CreateTable(
                name: "SolutionsTemplate",
                columns: table => new
                {
                    SolutionsId = table.Column<long>(type: "INTEGER", nullable: false),
                    TemplatesId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolutionsTemplate", x => new { x.SolutionsId, x.TemplatesId });
                    table.ForeignKey(
                        name: "FK_SolutionsTemplate_Solutionss_SolutionsId",
                        column: x => x.SolutionsId,
                        principalTable: "Solutionss",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SolutionsTemplate_Templates_TemplatesId",
                        column: x => x.TemplatesId,
                        principalTable: "Templates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolutionsTemplate_TemplatesId",
                table: "SolutionsTemplate",
                column: "TemplatesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolutionsTemplate");

            migrationBuilder.AddColumn<long>(
                name: "SolutionsId",
                table: "Templates",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Templates_SolutionsId",
                table: "Templates",
                column: "SolutionsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Templates_Solutionss_SolutionsId",
                table: "Templates",
                column: "SolutionsId",
                principalTable: "Solutionss",
                principalColumn: "Id");
        }
    }
}
