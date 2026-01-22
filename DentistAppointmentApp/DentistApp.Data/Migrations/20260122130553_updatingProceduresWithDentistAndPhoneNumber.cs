using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentistApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatingProceduresWithDentistAndPhoneNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DentistId",
                table: "Procedures",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                comment: "Id of the Dentist that will perform the  procedure");

            migrationBuilder.AddColumn<string>(
                name: "PatientPhoneNumber",
                table: "Procedures",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "",
                comment: "Patient Phone number");

            migrationBuilder.CreateIndex(
                name: "IX_Procedures_DentistId",
                table: "Procedures",
                column: "DentistId");

            migrationBuilder.AddForeignKey(
                name: "FK_Procedures_AspNetUsers_DentistId",
                table: "Procedures",
                column: "DentistId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Procedures_AspNetUsers_DentistId",
                table: "Procedures");

            migrationBuilder.DropIndex(
                name: "IX_Procedures_DentistId",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "DentistId",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "PatientPhoneNumber",
                table: "Procedures");
        }
    }
}
