using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Memorizer.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "memos",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Q = table.Column<string>(unicode: false, maxLength: 255, nullable: false),
                    A = table.Column<string>(unicode: false, maxLength: 4000, nullable: false),
                    repeat_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    postpone_level = table.Column<int>(nullable: false),
                    scores = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_memos", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "Q_Unique",
                table: "memos",
                column: "Q",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "memos");
        }
    }
}
