using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentistApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingDentistToAppointments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    AppointmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Primary Key for the Appointments type class"),
                    PatientId = table.Column<string>(type: "nvarchar(450)", nullable: false, comment: "Id of the Patient to which the procedure was done on"),
                    DentistId = table.Column<string>(type: "nvarchar(450)", nullable: false, comment: "Id of the Dentist that will perform the  procedure"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Date and time of the procedure"),
                    PatientPhoneNumber = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false, comment: "Patient Phone number"),
                    ManipulationTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Id of the Manipulation performed"),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "Note left by the patient for the dentist while making Appointment"),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "If the procedure's IsDeleted is set to true it will not be displayed in the system")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.AppointmentId);
                    table.ForeignKey(
                        name: "FK_Appointments_AspNetUsers_DentistId",
                        column: x => x.DentistId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_AspNetUsers_PatientId",
                        column: x => x.PatientId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_ManipulationTypes_ManipulationTypeId",
                        column: x => x.ManipulationTypeId,
                        principalTable: "ManipulationTypes",
                        principalColumn: "ManipulationId",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Table defining the Appointments made");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DentistId",
                table: "Appointments",
                column: "DentistId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ManipulationTypeId",
                table: "Appointments",
                column: "ManipulationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientId",
                table: "Appointments",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    ReservationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Primary Key for the Reservations type class"),
                    ManipulationTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Id of the Manipulation performed"),
                    PatientId = table.Column<string>(type: "nvarchar(450)", nullable: false, comment: "Id of the Patient to which the procedure was done on"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Date and time of the procedure"),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "If the procedure's IsDeleted is set to true it will not be displayed in the system"),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "Note left by the patient for the dentist while making reservation"),
                    PatientPhoneNumber = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false, comment: "Patient Phone number")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.ReservationId);
                    table.ForeignKey(
                        name: "FK_Reservations_AspNetUsers_PatientId",
                        column: x => x.PatientId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reservations_ManipulationTypes_ManipulationTypeId",
                        column: x => x.ManipulationTypeId,
                        principalTable: "ManipulationTypes",
                        principalColumn: "ManipulationId",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Table defining the Reservations made");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ManipulationTypeId",
                table: "Reservations",
                column: "ManipulationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_PatientId",
                table: "Reservations",
                column: "PatientId");
        }
    }
}
