using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotNetSandbox.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsVerified = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "BalanceLogs",
                columns: table => new
                {
                    BalanceId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    BalanceBefore = table.Column<decimal>(type: "TEXT", nullable: true),
                    Balance = table.Column<decimal>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Operator = table.Column<string>(type: "TEXT", nullable: false),
                    TransactionId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BalanceLogs", x => x.BalanceId);
                    table.ForeignKey(
                        name: "FK_BalanceLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransferLogs",
                columns: table => new
                {
                    TransferId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FromUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    ToUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    FromBalanceLogId = table.Column<int>(type: "INTEGER", nullable: true),
                    ToBalanceLogId = table.Column<int>(type: "INTEGER", nullable: true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferLogs", x => x.TransferId);
                    table.ForeignKey(
                        name: "FK_TransferLogs_BalanceLogs_FromBalanceLogId",
                        column: x => x.FromBalanceLogId,
                        principalTable: "BalanceLogs",
                        principalColumn: "BalanceId");
                    table.ForeignKey(
                        name: "FK_TransferLogs_BalanceLogs_ToBalanceLogId",
                        column: x => x.ToBalanceLogId,
                        principalTable: "BalanceLogs",
                        principalColumn: "BalanceId");
                    table.ForeignKey(
                        name: "FK_TransferLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BalanceLogs_UserId",
                table: "BalanceLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferLogs_FromBalanceLogId",
                table: "TransferLogs",
                column: "FromBalanceLogId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferLogs_ToBalanceLogId",
                table: "TransferLogs",
                column: "ToBalanceLogId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferLogs_UserId",
                table: "TransferLogs",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransferLogs");

            migrationBuilder.DropTable(
                name: "BalanceLogs");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
