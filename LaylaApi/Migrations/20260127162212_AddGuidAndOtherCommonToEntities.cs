using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LaylaApi.Migrations
{
    /// <inheritdoc />
    public partial class AddGuidAndOtherCommonToEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Created",
                table: "RefreshTokens",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "SentAt",
                table: "Messages",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "UploadedAt",
                table: "MediaFiles",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true
                );

            migrationBuilder.Sql(@"
              UPDATE Users
               SET Guid = NEWID()
                   WHERE Guid IS NULL
                ");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "Reviews",
                type: "uniqueidentifier",
                nullable: true
                );

            migrationBuilder.Sql(@"
              UPDATE Reviews
               SET Guid = NEWID()
                   WHERE Guid IS NULL
                ");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Reviews",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Reviews",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Reports",
                type: "int",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "Reports",
                type: "uniqueidentifier",
                nullable: true
                );

            migrationBuilder.Sql(@"
              UPDATE Reports
               SET Guid = NEWID()
                   WHERE Guid IS NULL
                ");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Reports",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Reports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "RefreshTokens",
                type: "uniqueidentifier",
                nullable: true
                );

            migrationBuilder.Sql(@"
              UPDATE RefreshTokens
               SET Guid = NEWID()
                   WHERE Guid IS NULL
                ");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "RefreshTokens",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "RefreshTokens",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "Payments",
                type: "uniqueidentifier",
                nullable: true
                );

            migrationBuilder.Sql(@"
              UPDATE Payments
               SET Guid = NEWID()
                   WHERE Guid IS NULL
                ");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Payments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Payments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "Messages",
                type: "uniqueidentifier",
                nullable: true
               );

            migrationBuilder.Sql(@"
              UPDATE Messages
               SET Guid = NEWID()
                   WHERE Guid IS NULL
                ");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Messages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "MediaFiles",
                type: "uniqueidentifier",
                nullable: true
                );

            migrationBuilder.Sql(@"
              UPDATE MediaFiles
               SET Guid = NEWID()
                   WHERE Guid IS NULL
                ");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "MediaFiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "MediaFiles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "Conversations",
                type: "uniqueidentifier",
                nullable: true
                );

            migrationBuilder.Sql(@"
              UPDATE Conversations
               SET Guid = NEWID()
                   WHERE Guid IS NULL
                ");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Conversations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Conversations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "Contracts",
                type: "uniqueidentifier",
                nullable: true
                );

            migrationBuilder.Sql(@"
              UPDATE Contracts
               SET Guid = NEWID()
                   WHERE Guid IS NULL
                ");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Contracts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Contracts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RenterId",
                table: "Contracts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "Bookings",
                type: "uniqueidentifier",
                nullable: true
                );

            migrationBuilder.Sql(@"
              UPDATE Bookings
               SET Guid = NEWID()
                   WHERE Guid IS NULL
                ");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Bookings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Apartments",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "Apartments",
                type: "uniqueidentifier",
                nullable: true
                );

            migrationBuilder.Sql(@"
              UPDATE Apartments
               SET Guid = NEWID()
                   WHERE Guid IS NULL
                ");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Apartments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Guid",
                table: "Users",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_Guid",
                table: "Reviews",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reports_Guid",
                table: "Reports",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Guid",
                table: "RefreshTokens",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Guid",
                table: "Payments",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_Guid",
                table: "Messages",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_Guid",
                table: "MediaFiles",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_Guid",
                table: "Conversations",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_Guid",
                table: "Contracts",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_Guid",
                table: "Bookings",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_Guid",
                table: "Apartments",
                column: "Guid",
                unique: true);

            
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Guid",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_Guid",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reports_Guid",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_Guid",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_Payments_Guid",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Messages_Guid",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_MediaFiles_Guid",
                table: "MediaFiles");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_Guid",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_Guid",
                table: "Contracts");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_Guid",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Apartments_Guid",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "RenterId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Apartments");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "RefreshTokens",
                newName: "Created");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Messages",
                newName: "SentAt");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "MediaFiles",
                newName: "UploadedAt");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Reports",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Apartments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
