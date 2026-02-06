using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace LaylaApi.Migrations
{
    /// <inheritdoc />
    public partial class updateIndexingAndGeoLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bookings_ApartmentId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Apartments");

            migrationBuilder.RenameColumn(
                name: "Longitude",
                table: "Apartments",
                newName: "Area");

            migrationBuilder.AlterColumn<int>(
                name: "Width",
                table: "MediaFiles",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PublicId",
                table: "MediaFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Height",
                table: "MediaFiles",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Format",
                table: "MediaFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Duration",
                table: "MediaFiles",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Finishing",
                table: "Apartments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FloorNumber",
                table: "Apartments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "HasElevator",
                table: "Apartments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasParking",
                table: "Apartments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasPool",
                table: "Apartments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Point>(
                name: "Location_Location",
                table: "Apartments",
                type: "geography",
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfBalconies",
                table: "Apartments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfBathrooms",
                table: "Apartments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfBedRooms",
                table: "Apartments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfLivingRooms",
                table: "Apartments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfReceptionRooms",
                table: "Apartments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Orientation",
                table: "Apartments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Apartments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "View",
                table: "Apartments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ApartmentId_Status_StartDate_EndDate",
                table: "Bookings",
                columns: new[] { "ApartmentId", "Status", "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_IsAvailable_Type_Finishing",
                table: "Apartments",
                columns: new[] { "IsAvailable", "Type", "Finishing" });

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_PricePerDay_PricePerHour_Area_FloorNumber_NumberOfBedRooms_NumberOfBathrooms",
                table: "Apartments",
                columns: new[] { "PricePerDay", "PricePerHour", "Area", "FloorNumber", "NumberOfBedRooms", "NumberOfBathrooms" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bookings_ApartmentId_Status_StartDate_EndDate",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Apartments_IsAvailable_Type_Finishing",
                table: "Apartments");

            migrationBuilder.DropIndex(
                name: "IX_Apartments_PricePerDay_PricePerHour_Area_FloorNumber_NumberOfBedRooms_NumberOfBathrooms",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "Finishing",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "FloorNumber",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "HasElevator",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "HasParking",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "HasPool",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "Location_Location",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "NumberOfBalconies",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "NumberOfBathrooms",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "NumberOfBedRooms",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "NumberOfLivingRooms",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "NumberOfReceptionRooms",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "Orientation",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "View",
                table: "Apartments");

            migrationBuilder.RenameColumn(
                name: "Area",
                table: "Apartments",
                newName: "Longitude");

            migrationBuilder.AlterColumn<int>(
                name: "Width",
                table: "MediaFiles",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "PublicId",
                table: "MediaFiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Height",
                table: "MediaFiles",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Format",
                table: "MediaFiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<double>(
                name: "Duration",
                table: "MediaFiles",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Apartments",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ApartmentId",
                table: "Bookings",
                column: "ApartmentId");
        }
    }
}
