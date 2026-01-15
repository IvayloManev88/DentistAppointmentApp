using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentistApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class PreparedDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterTable(
                name: "AspNetUsers",
                comment: "Dentist User Entity in the system");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                comment: "User's first name");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "If the user is deleted = false then he/she is an active user");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                comment: "User's last name");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.CreateTable(
                name: "ManipulationTypes",
                columns: table => new
                {
                    ManipulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Primary Key for the Manipulation type class"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Name of the manipulation"),
                    PriceRange = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, comment: "Defined expected price range. The type string is defined because the expected values are 50-100 and should be informative for the customer/patient"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, comment: "If the manipulation is deleted = false then it should not be selectable and visible")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManipulationTypes", x => x.ManipulationId);
                },
                comment: "Table defining the manipulation types");

            migrationBuilder.CreateTable(
                name: "Procedures",
                columns: table => new
                {
                    ProcedureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Primary Key for the Procedure type class"),
                    PatientId = table.Column<string>(type: "nvarchar(450)", nullable: false, comment: "Id of the Patient to which the procedure was done on"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Date and time of the procedure"),
                    ManipulationTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Id of the Manipulation performed"),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "Note left by the dentist after the procedure"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, comment: "If the procedure's IsDeleted is set to true it will not be displayed in the system")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Procedures", x => x.ProcedureId);
                    table.ForeignKey(
                        name: "FK_Procedures_AspNetUsers_PatientId",
                        column: x => x.PatientId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Procedures_ManipulationTypes_ManipulationTypeId",
                        column: x => x.ManipulationTypeId,
                        principalTable: "ManipulationTypes",
                        principalColumn: "ManipulationId",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Table defining the Procedures done.");

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    ReservationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Primary Key for the Reservations type class"),
                    PatientId = table.Column<string>(type: "nvarchar(450)", nullable: false, comment: "Id of the Patient to which the procedure was done on"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Date and time of the procedure"),
                    ManipulationTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Id of the Manipulation performed"),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "Note left by the patient for the dentist while making reservation"),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, comment: "If the procedure's IsDeleted is set to true it will not be displayed in the system")
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
                name: "IX_Procedures_ManipulationTypeId",
                table: "Procedures",
                column: "ManipulationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Procedures_PatientId",
                table: "Procedures",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ManipulationTypeId",
                table: "Reservations",
                column: "ManipulationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_PatientId",
                table: "Reservations",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Procedures");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "ManipulationTypes");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.AlterTable(
                name: "AspNetUsers",
                oldComment: "Dentist User Entity in the system");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
