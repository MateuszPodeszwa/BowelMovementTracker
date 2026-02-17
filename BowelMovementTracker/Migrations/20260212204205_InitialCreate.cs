using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BowelMovementTracker.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Diary",
                columns: table => new
                {
                    DiaryIdentifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiaryUserIdentifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diary", x => x.DiaryIdentifier);
                });

            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    LogIdentifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LogDiaryIdentifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LogDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LogNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogBowelMovementType = table.Column<int>(type: "int", nullable: false),
                    LogWasCoffeeConsumed = table.Column<bool>(type: "bit", nullable: false),
                    LogWasMilkConsumed = table.Column<bool>(type: "bit", nullable: false),
                    LogLastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.LogIdentifier);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserIdentifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserEmailAddress = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserDiaryIdentifier = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserIdentifier);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_UserEmailAddress",
                table: "User",
                column: "UserEmailAddress",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Diary");

            migrationBuilder.DropTable(
                name: "Log");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
