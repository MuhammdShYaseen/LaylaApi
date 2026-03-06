using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LaylaApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMessageAndMediaFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Messages_ConversationId",
                table: "Messages");

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ReceiverId",
                table: "Messages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrimary",
                table: "MediaFiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ConversationId_ReceiverId_IsRead",
                table: "Messages",
                columns: new[] { "ConversationId", "ReceiverId", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReceiverId_IsRead",
                table: "Messages",
                columns: new[] { "ReceiverId", "IsRead" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Messages_ConversationId_ReceiverId_IsRead",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ReceiverId_IsRead",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ReceiverId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "IsPrimary",
                table: "MediaFiles");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ConversationId",
                table: "Messages",
                column: "ConversationId");
        }
    }
}
