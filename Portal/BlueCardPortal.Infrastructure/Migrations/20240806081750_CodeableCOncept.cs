using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BlueCardPortal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CodeableCOncept : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "nomenclatures");

            migrationBuilder.CreateTable(
                name: "codeable_concepts",
                schema: "nomenclatures",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Systsem identifer of codeable concept")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, comment: "Nomenclaure value identifier"),
                    value = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, comment: "Nomenclature value in BG"),
                    value_en = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, comment: "Nomenclature value in EN"),
                    date_from = table.Column<DateTime>(type: "date", nullable: false, comment: "Start of the nomenclature validity"),
                    date_to = table.Column<DateTime>(type: "date", nullable: true, comment: "End of the nomenclature validity, never expires if NULL"),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "Nomenclaure identifier"),
                    parent_id = table.Column<long>(type: "bigint", nullable: true, comment: "FK for creating hierarchical nomenclatures"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false, comment: "Record created by"),
                    last_updated_by = table.Column<Guid>(type: "uuid", nullable: true, comment: "Record last updated by"),
                    last_updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "Record last updated on")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_codeable_concepts", x => x.id);
                    table.ForeignKey(
                        name: "fk_codeable_concepts_codeable_concepts_parent_id",
                        column: x => x.parent_id,
                        principalSchema: "nomenclatures",
                        principalTable: "codeable_concepts",
                        principalColumn: "id");
                },
                comment: "System nomenclatures");

            migrationBuilder.CreateTable(
                name: "nomenclature_types",
                schema: "nomenclatures",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, comment: "Primary key")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "Nomenclature type"),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false, comment: "Type name or description")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_nomenclature_types", x => x.id);
                },
                comment: "Types of nomenclatures available to the system");

            migrationBuilder.CreateTable(
                name: "additional_columns",
                schema: "nomenclatures",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "System identifier of additional column")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nomenclature_id = table.Column<long>(type: "bigint", nullable: false, comment: "FK Codeable concept identificator"),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "Column name"),
                    value_bg = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false, comment: "Column value in BG"),
                    value_en = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false, comment: "Column value in EN")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_additional_columns", x => x.id);
                    table.ForeignKey(
                        name: "fk_additional_columns_codeable_concepts_nomenclature_id",
                        column: x => x.nomenclature_id,
                        principalSchema: "nomenclatures",
                        principalTable: "codeable_concepts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Additiona values for codeable concepts");

            migrationBuilder.CreateIndex(
                name: "ix_additional_columns_nomenclature_id",
                schema: "nomenclatures",
                table: "additional_columns",
                column: "nomenclature_id");

            migrationBuilder.CreateIndex(
                name: "ix_codeable_concepts_parent_id",
                schema: "nomenclatures",
                table: "codeable_concepts",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_codeable_concepts_type_code_date_from_date_to",
                schema: "nomenclatures",
                table: "codeable_concepts",
                columns: new[] { "type", "code", "date_from", "date_to" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "additional_columns",
                schema: "nomenclatures");

            migrationBuilder.DropTable(
                name: "nomenclature_types",
                schema: "nomenclatures");

            migrationBuilder.DropTable(
                name: "codeable_concepts",
                schema: "nomenclatures");
        }
    }
}
