using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BowelMovementTracker.Migrations
{
    /// <inheritdoc />
    public partial class DefineRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserDiaryIdentifier",
                table: "User");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserIdentifier",
                table: "User",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "newsequentialid()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<bool>(
                name: "LogWasMilkConsumed",
                table: "Log",
                type: "bit",
                nullable: true,
                defaultValueSql: "0",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "LogWasCoffeeConsumed",
                table: "Log",
                type: "bit",
                nullable: true,
                defaultValueSql: "0",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "LogNotes",
                table: "Log",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LogLastUpdated",
                table: "Log",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "getutcdate()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LogDateTime",
                table: "Log",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "getutcdate()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<Guid>(
                name: "LogIdentifier",
                table: "Log",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "newsequentialid()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "DiaryIdentifier",
                table: "Diary",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "newsequentialid()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_User_UserIdentifier",
                table: "User",
                column: "UserIdentifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Log_LogDiaryIdentifier",
                table: "Log",
                column: "LogDiaryIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Log_LogIdentifier",
                table: "Log",
                column: "LogIdentifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Diary_DiaryIdentifier",
                table: "Diary",
                column: "DiaryIdentifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Diary_DiaryUserIdentifier",
                table: "Diary",
                column: "DiaryUserIdentifier",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Diary",
                table: "Diary",
                column: "DiaryUserIdentifier",
                principalTable: "User",
                principalColumn: "UserIdentifier",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Log_Diary",
                table: "Log",
                column: "LogDiaryIdentifier",
                principalTable: "Diary",
                principalColumn: "DiaryIdentifier",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Diary",
                table: "Diary");

            migrationBuilder.DropForeignKey(
                name: "FK_Log_Diary",
                table: "Log");

            migrationBuilder.DropIndex(
                name: "IX_User_UserIdentifier",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_Log_LogDiaryIdentifier",
                table: "Log");

            migrationBuilder.DropIndex(
                name: "IX_Log_LogIdentifier",
                table: "Log");

            migrationBuilder.DropIndex(
                name: "IX_Diary_DiaryIdentifier",
                table: "Diary");

            migrationBuilder.DropIndex(
                name: "IX_Diary_DiaryUserIdentifier",
                table: "Diary");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserIdentifier",
                table: "User",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "newsequentialid()");

            migrationBuilder.AddColumn<Guid>(
                name: "UserDiaryIdentifier",
                table: "User",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "LogWasMilkConsumed",
                table: "Log",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "0");

            migrationBuilder.AlterColumn<bool>(
                name: "LogWasCoffeeConsumed",
                table: "Log",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "0");

            migrationBuilder.AlterColumn<string>(
                name: "LogNotes",
                table: "Log",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LogLastUpdated",
                table: "Log",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "getutcdate()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LogDateTime",
                table: "Log",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValueSql: "getutcdate()");

            migrationBuilder.AlterColumn<Guid>(
                name: "LogIdentifier",
                table: "Log",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "newsequentialid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "DiaryIdentifier",
                table: "Diary",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "newsequentialid()");
        }
    }
}
