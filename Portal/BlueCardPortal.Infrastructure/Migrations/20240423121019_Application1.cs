using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlueCardPortal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Application1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "type",
                table: "application_item_types",
                type: "text",
                nullable: false,
                defaultValue: "",
                comment: "Тип на модела");

            migrationBuilder.AddColumn<string>(
                name: "view_name",
                table: "application_item_types",
                type: "text",
                nullable: false,
                defaultValue: "",
                comment: "Име на view");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "type",
                table: "application_item_types");

            migrationBuilder.DropColumn(
                name: "view_name",
                table: "application_item_types");
        }
    }
}
