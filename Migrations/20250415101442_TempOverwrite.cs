using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeFirstGenerator.Migrations
{
    /// <inheritdoc />
    public partial class TempOverwrite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Overwrite",
                table: "Templates",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            //设置默认值
            migrationBuilder.Sql("UPDATE `Templates` SET `Overwrite` = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Overwrite",
                table: "Templates");
        }
    }
}
