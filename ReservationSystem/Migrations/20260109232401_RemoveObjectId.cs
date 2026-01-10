using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservationSystem.Migrations
{
    /// <inheritdoc />
    public partial class RemoveObjectId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ObjectId",
                table: "Reservations");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAENBmy05AIJVp/O9ybCqxl05GSWdaNmHXg/YRTyM9UbBzlMlwJLrBeAXRn/3sMUxmbw==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ObjectId",
                table: "Reservations",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEAYvuUpzleUKvJ8knPt190mBEsM7AAg5PPPc3RMXF93qYVcIObrMrB61kxIXs8qsrw==");
        }
    }
}
