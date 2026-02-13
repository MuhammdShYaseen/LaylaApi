using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LaylaApi.Migrations
{
    /// <inheritdoc />
    public partial class AddSpatialIndexToApartments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
        CREATE SPATIAL INDEX IX_Apartments_Location
        ON Apartments(Location_Location)
        USING GEOGRAPHY_GRID
        WITH (
            GRIDS =(LEVEL_1 = MEDIUM, LEVEL_2 = MEDIUM, LEVEL_3 = MEDIUM, LEVEL_4 = MEDIUM),
            CELLS_PER_OBJECT = 16
        );
    """);

            migrationBuilder.DropIndex(
                name: "IX_Apartments_PricePerDay_PricePerHour_Area_FloorNumber_NumberOfBedRooms_NumberOfBathrooms",
                table: "Apartments");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Orientation",
                table: "Apartments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_PricePerDay_PricePerHour_Area_FloorNumber_NumberOfBedRooms_NumberOfBathrooms_NumberOfLivingRooms",
                table: "Apartments",
                columns: new[] { "PricePerDay", "PricePerHour", "Area", "FloorNumber", "NumberOfBedRooms", "NumberOfBathrooms", "NumberOfLivingRooms" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
        DROP INDEX IX_Apartments_Location
        ON Apartments;
    """);

            migrationBuilder.DropIndex(
                name: "IX_Apartments_PricePerDay_PricePerHour_Area_FloorNumber_NumberOfBedRooms_NumberOfBathrooms_NumberOfLivingRooms",
                table: "Apartments");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Orientation",
                table: "Apartments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_PricePerDay_PricePerHour_Area_FloorNumber_NumberOfBedRooms_NumberOfBathrooms",
                table: "Apartments",
                columns: new[] { "PricePerDay", "PricePerHour", "Area", "FloorNumber", "NumberOfBedRooms", "NumberOfBathrooms" });
        }
    }
}
