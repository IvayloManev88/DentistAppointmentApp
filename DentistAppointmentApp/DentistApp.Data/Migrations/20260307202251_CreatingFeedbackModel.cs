using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentistApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreatingFeedbackModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "Procedures",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true,
                comment: "Note left by the dentist after the procedure",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Note left by the dentist after the procedure");

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "Appointments",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true,
                comment: "Note left by the patient for the dentist while making Appointment",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Note left by the patient for the dentist while making Appointment");

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    FeedbackId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Primary Key for the Patient Feedback type class"),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Date and Time when the feedback was left on the site"),
                    FeedbackText = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false, comment: "Actual feedback message"),
                    Rating = table.Column<int>(type: "int", nullable: false, comment: "Rating will be used for getting the overall rating score"),
                    ProcedureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Id of the Procedure for which we are leaving feedback"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "If the feedback's IsDeleted is set to true it will not be displayed in the system")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.FeedbackId);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Procedures_ProcedureId",
                        column: x => x.ProcedureId,
                        principalTable: "Procedures",
                        principalColumn: "ProcedureId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_ProcedureId",
                table: "Feedbacks",
                column: "ProcedureId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "Procedures",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Note left by the dentist after the procedure",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450,
                oldNullable: true,
                oldComment: "Note left by the dentist after the procedure");

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Note left by the patient for the dentist while making Appointment",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450,
                oldNullable: true,
                oldComment: "Note left by the patient for the dentist while making Appointment");
        }
    }
}
