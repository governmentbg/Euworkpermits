using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BlueCardPortal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Application : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "application_item_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, comment: "Идентификатор")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false, comment: "Наименование"),
                    name_en = table.Column<string>(type: "text", nullable: false, comment: "Наименование en")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_application_item_types", x => x.id);
                },
                comment: "Тип раздел от заявление");

            migrationBuilder.CreateTable(
                name: "applications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Идентификатор"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Идентификатор на потребител"),
                    status = table.Column<int>(type: "integer", nullable: false, comment: "Статус"),
                    error = table.Column<string>(type: "text", nullable: true, comment: "Грешка върната от core системата"),
                    date_wrt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата на последна промяна")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_applications", x => x.id);
                },
                comment: "Заявления");

            migrationBuilder.CreateTable(
                name: "application_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Идентификатор"),
                    application_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Идентификатор на заявление"),
                    item_type_id = table.Column<int>(type: "integer", nullable: false, comment: "Тип"),
                    data_content = table.Column<string>(type: "jsonb", nullable: true, comment: "Данни за раздел сериализирани в json"),
                    date_wrt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата на последна промяна")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_application_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_application_items_application_item_types_item_type_id",
                        column: x => x.item_type_id,
                        principalTable: "application_item_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_application_items_applications_application_id",
                        column: x => x.application_id,
                        principalTable: "applications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Данни раздел от заявление");

            migrationBuilder.CreateIndex(
                name: "ix_application_items_application_id",
                table: "application_items",
                column: "application_id");

            migrationBuilder.CreateIndex(
                name: "ix_application_items_item_type_id",
                table: "application_items",
                column: "item_type_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "application_items");

            migrationBuilder.DropTable(
                name: "application_item_types");

            migrationBuilder.DropTable(
                name: "applications");
        }
    }
}
