using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlueCardPortal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "application_updates",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Идентификатор"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Идентификатор на потребител"),
                    status = table.Column<int>(type: "integer", nullable: false, comment: "Статус"),
                    error = table.Column<string>(type: "text", nullable: true, comment: "Грешка върната от core системата"),
                    date_wrt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата на последна промяна"),
                    apply_number_from = table.Column<string>(type: "text", nullable: true, comment: "Номер заявление от core системата"),
                    data_content = table.Column<string>(type: "jsonb", nullable: true, comment: "Данни за  жалба сериализирани в json")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_application_updates", x => x.id);
                },
                comment: "Жалби");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "application_updates");
        }
    }
}
