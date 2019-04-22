using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Memorizer.Data.Migrations
{
    public partial class UpdateSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_memos",
                table: "memos");

            migrationBuilder.RenameTable(
                name: "memos",
                newName: "Memos");

            migrationBuilder.RenameColumn(
                name: "scores",
                table: "Memos",
                newName: "Scores");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Memos",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "repeat_date",
                table: "Memos",
                newName: "RepeatDate");

            migrationBuilder.RenameColumn(
                name: "postpone_level",
                table: "Memos",
                newName: "PostponeLevel");

            migrationBuilder.RenameColumn(
                name: "Q",
                table: "Memos",
                newName: "Question");

            migrationBuilder.RenameColumn(
                name: "A",
                table: "Memos",
                newName: "Answer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RepeatDate",
                table: "Memos",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Memos",
                table: "Memos",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Memos",
                table: "Memos");

            migrationBuilder.RenameTable(
                name: "Memos",
                newName: "memos");

            migrationBuilder.RenameColumn(
                name: "Scores",
                table: "memos",
                newName: "scores");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "memos",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "RepeatDate",
                table: "memos",
                newName: "repeat_date");

            migrationBuilder.RenameColumn(
                name: "PostponeLevel",
                table: "memos",
                newName: "postpone_level");

            migrationBuilder.RenameColumn(
                name: "Question",
                table: "memos",
                newName: "Q");

            migrationBuilder.RenameColumn(
                name: "Answer",
                table: "memos",
                newName: "A");

            migrationBuilder.AlterColumn<DateTime>(
                name: "repeat_date",
                table: "memos",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddPrimaryKey(
                name: "PK_memos",
                table: "memos",
                column: "id");
        }
    }
}
