using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlueCardPortal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationItemType_PreviewName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "preview_name",
                table: "application_item_types",
                type: "text",
                nullable: false,
                defaultValue: "",
                comment: "Наименование при преглед");

            migrationBuilder.AddColumn<string>(
                name: "preview_name_en",
                table: "application_item_types",
                type: "text",
                nullable: false,
                defaultValue: "",
                comment: "Наименование en при преглед");

            migrationBuilder.AddColumn<string>(
                name: "preview_name_view",
                table: "application_item_types",
                type: "text",
                nullable: false,
                defaultValue: "",
                comment: "Име на view за преглед");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "preview_name",
                table: "application_item_types");

            migrationBuilder.DropColumn(
                name: "preview_name_en",
                table: "application_item_types");

            migrationBuilder.DropColumn(
                name: "preview_name_view",
                table: "application_item_types");
        }
    }
}
