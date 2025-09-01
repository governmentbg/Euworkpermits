using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BlueCardPortal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EMail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "e_mails",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<string>(type: "text", nullable: true),
                    feed_back_name = table.Column<string>(type: "text", nullable: true),
                    message = table.Column<string>(type: "text", nullable: true),
                    feed_back_email = table.Column<string>(type: "text", nullable: true),
                    mail_template_name = table.Column<string>(type: "text", nullable: true),
                    context = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    failed_attempts = table.Column<int>(type: "integer", nullable: false),
                    failed_attempts_errors = table.Column<string>(type: "text", nullable: true),
                    create_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modify_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_e_mails", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "e_mails");
        }
    }
}
