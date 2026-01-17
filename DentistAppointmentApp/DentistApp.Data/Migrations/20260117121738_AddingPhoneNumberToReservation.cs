using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentistApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingPhoneNumberToReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Reservations",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "If the procedure's IsDeleted is set to true it will not be displayed in the system",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "If the procedure's IsDeleted is set to true it will not be displayed in the system");

            migrationBuilder.AlterColumn<bool>(
                name: "IsConfirmed",
                table: "Reservations",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<string>(
                name: "PatientPhoneNumber",
                table: "Reservations",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "",
                comment: "Patient Phone number");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Procedures",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "If the procedure's IsDeleted is set to true it will not be displayed in the system",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "If the procedure's IsDeleted is set to true it will not be displayed in the system");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "ManipulationTypes",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "If the manipulation is deleted = false then it should not be selectable and visible",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "If the manipulation is deleted = false then it should not be selectable and visible");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "If the user is deleted = false then he/she is an active user",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "If the user is deleted = false then he/she is an active user");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PatientPhoneNumber",
                table: "Reservations");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Reservations",
                type: "bit",
                nullable: false,
                comment: "If the procedure's IsDeleted is set to true it will not be displayed in the system",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false,
                oldComment: "If the procedure's IsDeleted is set to true it will not be displayed in the system");

            migrationBuilder.AlterColumn<bool>(
                name: "IsConfirmed",
                table: "Reservations",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Procedures",
                type: "bit",
                nullable: false,
                comment: "If the procedure's IsDeleted is set to true it will not be displayed in the system",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false,
                oldComment: "If the procedure's IsDeleted is set to true it will not be displayed in the system");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "ManipulationTypes",
                type: "bit",
                nullable: false,
                comment: "If the manipulation is deleted = false then it should not be selectable and visible",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false,
                oldComment: "If the manipulation is deleted = false then it should not be selectable and visible");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                comment: "If the user is deleted = false then he/she is an active user",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false,
                oldComment: "If the user is deleted = false then he/she is an active user");
        }
    }
}
