using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSeatProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BasePrice",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "ExtraFee",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "IsBlocked",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "SeatType",
                table: "Seats");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BasePrice",
                table: "Seats",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ExtraFee",
                table: "Seats",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBlocked",
                table: "Seats",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SeatType",
                table: "Seats",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
