using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlueCardPortal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "update_messages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Идентификатор"),
                    source_type_id = table.Column<int>(type: "integer", nullable: false, comment: "Тип източник"),
                    source_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Идентификатор на източник"),
                    registration_data = table.Column<string>(type: "jsonb", nullable: true, comment: "Данни за заявление сериализирани в json"),
                    response_status = table.Column<string>(type: "text", nullable: true, comment: "Отговор"),
                    registration_time_stamp = table.Column<byte[]>(type: "bytea", nullable: true),
                    registration_data_signature = table.Column<string>(type: "text", nullable: true),
                    response_message = table.Column<string>(type: "text", nullable: true, comment: "Отговор"),
                    date_wrt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата на последна промяна")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_update_messages", x => x.id);
                },
                comment: "Подписани данни от саоотказ/жалба/промяна изпратени към коре системата");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "update_messages");
        }
    }
}
