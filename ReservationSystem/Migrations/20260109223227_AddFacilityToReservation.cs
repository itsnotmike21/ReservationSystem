using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservationSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddFacilityToReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FacilityId",
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

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_FacilityId",
                table: "Reservations",
                column: "FacilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Facilities_FacilityId",
                table: "Reservations",
                column: "FacilityId",
                principalTable: "Facilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Facilities_FacilityId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_FacilityId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "FacilityId",
                table: "Reservations");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEAuIGRFTh+O23M5FhHmyjF0HZvkPVJSpORpbdB5JZcDp9W7cD5+ysG3Ri669kmYSRA==");
        }
    }
}
