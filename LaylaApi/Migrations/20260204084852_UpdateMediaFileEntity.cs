using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LaylaApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMediaFileEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Bytes",
                table: "MediaFiles",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<double>(
                name: "Duration",
                table: "MediaFiles",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Format",
                table: "MediaFiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "MediaFiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MediaStorageProvider",
                table: "MediaFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "MediaFiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "MediaFiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "MediaFiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Width",
                table: "MediaFiles",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bytes",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "Format",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "MediaStorageProvider",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "MediaFiles");
        }
    }
}
