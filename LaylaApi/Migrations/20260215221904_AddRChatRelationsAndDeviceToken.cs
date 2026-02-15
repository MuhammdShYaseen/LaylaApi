using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LaylaApi.Migrations
{
    /// <inheritdoc />
    public partial class AddRChatRelationsAndDeviceToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RegisteredAt",
                table: "DeviceTokens",
                newName: "LastSeenAt");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "DeviceTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "DeviceTokens",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DeviceId",
                table: "DeviceTokens",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "DeviceTokens",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "DeviceTokens",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Platform",
                table: "DeviceTokens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "DeviceTokens",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeviceTokens_Guid",
                table: "DeviceTokens",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeviceTokens_Token",
                table: "DeviceTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeviceTokens_UserId_DeviceId",
                table: "DeviceTokens",
                columns: new[] { "UserId", "DeviceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_UserId",
                table: "Conversations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Users_UserId",
                table: "Conversations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeviceTokens_Users_UserId",
                table: "DeviceTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Users_UserId",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_DeviceTokens_Users_UserId",
                table: "DeviceTokens");

            migrationBuilder.DropIndex(
                name: "IX_DeviceTokens_Guid",
                table: "DeviceTokens");

            migrationBuilder.DropIndex(
                name: "IX_DeviceTokens_Token",
                table: "DeviceTokens");

            migrationBuilder.DropIndex(
                name: "IX_DeviceTokens_UserId_DeviceId",
                table: "DeviceTokens");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_UserId",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "DeviceTokens");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "DeviceTokens");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "DeviceTokens");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "DeviceTokens");

            migrationBuilder.DropColumn(
                name: "Platform",
                table: "DeviceTokens");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "DeviceTokens");

            migrationBuilder.RenameColumn(
                name: "LastSeenAt",
                table: "DeviceTokens",
                newName: "RegisteredAt");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "DeviceTokens",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
