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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
        DROP INDEX IX_Apartments_Location
        ON Apartments;
    """);

        }
    }
}
