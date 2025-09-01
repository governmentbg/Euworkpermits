using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlueCardPortal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationItemType_StepNum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "step_num",
                table: "application_item_types",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                comment: "Номер на стъпка");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "step_num",
                table: "application_item_types");
        }
    }
}
