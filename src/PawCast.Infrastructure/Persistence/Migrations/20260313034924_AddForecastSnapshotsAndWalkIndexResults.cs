using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawCast.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddForecastSnapshotsAndWalkIndexResults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "forecast_snapshots",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    longitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    forecast_time_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    fetched_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    source = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    raw_weather_json = table.Column<string>(type: "jsonb", nullable: false),
                    raw_air_quality_json = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_forecast_snapshots", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "walk_index_results",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    longitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    forecast_time_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    generated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    walk_index = table.Column<int>(type: "integer", nullable: false),
                    grade = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    reasons_json = table.Column<string>(type: "jsonb", nullable: false),
                    temperature_c = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    wind_kph = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    precipitation_probability = table.Column<int>(type: "integer", nullable: false),
                    uv_index = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    pm25 = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_walk_index_results", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_forecast_snapshots_latitude_longitude_forecast_time_utc",
                table: "forecast_snapshots",
                columns: new[] { "latitude", "longitude", "forecast_time_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_walk_index_results_latitude_longitude_forecast_time_utc",
                table: "walk_index_results",
                columns: new[] { "latitude", "longitude", "forecast_time_utc" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "forecast_snapshots");

            migrationBuilder.DropTable(
                name: "walk_index_results");
        }
    }
}
