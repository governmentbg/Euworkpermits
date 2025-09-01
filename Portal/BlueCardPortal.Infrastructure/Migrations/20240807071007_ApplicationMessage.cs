using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlueCardPortal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "application_messages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Идентификатор"),
                    application_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Идентификатор на заявление"),
                    data_content = table.Column<string>(type: "jsonb", nullable: true, comment: "Данни за заявление сериализирани в json"),
                    response_status = table.Column<string>(type: "text", nullable: true, comment: "Отговор"),
                    registration_time_stamp = table.Column<string>(type: "text", nullable: true),
                    registration_data_signature = table.Column<string>(type: "text", nullable: true),
                    response_message = table.Column<string>(type: "text", nullable: true, comment: "Отговор"),
                    date_wrt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата на последна промяна")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_application_messages", x => x.id);
                    table.ForeignKey(
                        name: "fk_application_messages_applications_application_id",
                        column: x => x.application_id,
                        principalTable: "applications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Подписани данни от заявление изпратени към коре системата");

            migrationBuilder.CreateIndex(
                name: "ix_application_messages_application_id",
                table: "application_messages",
                column: "application_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "application_messages");
        }
    }
}
