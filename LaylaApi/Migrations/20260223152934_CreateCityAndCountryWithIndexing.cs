using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LaylaApi.Migrations
{
    /// <inheritdoc />
    public partial class CreateCityAndCountryWithIndexing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reviews_ApartmentId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_ApartmentId_Status_StartDate_EndDate",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Apartments_IsAvailable_Type_Finishing",
                table: "Apartments");

            migrationBuilder.DropIndex(
                name: "IX_Apartments_PricePerDay_PricePerHour_Area_FloorNumber_NumberOfBedRooms_NumberOfBathrooms_NumberOfLivingRooms",
                table: "Apartments");

            migrationBuilder.AlterColumn<string>(
                name: "Orientation",
                table: "Apartments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "Apartments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Country",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "City",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_City", x => x.Id);
                    table.ForeignKey(
                        name: "FK_City_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ApartmentId_Rating",
                table: "Reviews",
                columns: new[] { "ApartmentId", "Rating" });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ApartmentId_StartDate_EndDate",
                table: "Bookings",
                columns: new[] { "ApartmentId", "StartDate", "EndDate" },
                filter: "[Status] IN (1, 2)");

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_CityId",
                table: "Apartments",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_IsAvailable_Type_Finishing_HasElevator_HasParking_HasPool_View",
                table: "Apartments",
                columns: new[] { "IsAvailable", "Type", "Finishing", "HasElevator", "HasParking", "HasPool", "View" });

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_IsAvailable_Type_Finishing_PricePerDay_Area",
                table: "Apartments",
                columns: new[] { "IsAvailable", "Type", "Finishing", "PricePerDay", "Area" })
                .Annotation("SqlServer:Include", new[] { "Id", "Title", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_NumberOfBedRooms_NumberOfBathrooms_NumberOfLivingRooms_FloorNumber",
                table: "Apartments",
                columns: new[] { "NumberOfBedRooms", "NumberOfBathrooms", "NumberOfLivingRooms", "FloorNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_Orientation",
                table: "Apartments",
                column: "Orientation");

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_PricePerDay_PricePerHour_Area",
                table: "Apartments",
                columns: new[] { "PricePerDay", "PricePerHour", "Area" });

            migrationBuilder.CreateIndex(
                name: "IX_City_CountryId",
                table: "City",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_City_Guid",
                table: "City",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Country_Guid",
                table: "Country",
                column: "Guid",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Apartments_City_CityId",
                table: "Apartments",
                column: "CityId",
                principalTable: "City",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Apartments_City_CityId",
                table: "Apartments");


            migrationBuilder.DropTable(
                name: "City");

            migrationBuilder.DropTable(
                name: "Country");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_ApartmentId_Rating",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_ApartmentId_StartDate_EndDate",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Apartments_CityId",
                table: "Apartments");

            

            migrationBuilder.DropIndex(
                name: "IX_Apartments_IsAvailable_Type_Finishing_HasElevator_HasParking_HasPool_View",
                table: "Apartments");

            migrationBuilder.DropIndex(
                name: "IX_Apartments_IsAvailable_Type_Finishing_PricePerDay_Area",
                table: "Apartments");

            migrationBuilder.DropIndex(
                name: "IX_Apartments_NumberOfBedRooms_NumberOfBathrooms_NumberOfLivingRooms_FloorNumber",
                table: "Apartments");

            migrationBuilder.DropIndex(
                name: "IX_Apartments_Orientation",
                table: "Apartments");

            migrationBuilder.DropIndex(
                name: "IX_Apartments_PricePerDay_PricePerHour_Area",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Apartments");

            

            migrationBuilder.AlterColumn<string>(
                name: "Orientation",
                table: "Apartments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ApartmentId",
                table: "Reviews",
                column: "ApartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ApartmentId_Status_StartDate_EndDate",
                table: "Bookings",
                columns: new[] { "ApartmentId", "Status", "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_IsAvailable_Type_Finishing",
                table: "Apartments",
                columns: new[] { "IsAvailable", "Type", "Finishing" });

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_PricePerDay_PricePerHour_Area_FloorNumber_NumberOfBedRooms_NumberOfBathrooms_NumberOfLivingRooms",
                table: "Apartments",
                columns: new[] { "PricePerDay", "PricePerHour", "Area", "FloorNumber", "NumberOfBedRooms", "NumberOfBathrooms", "NumberOfLivingRooms" });
        }
    }
}
