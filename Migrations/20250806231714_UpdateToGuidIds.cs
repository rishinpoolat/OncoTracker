using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OncoTrack.Migrations
{
    /// <inheritdoc />
    public partial class UpdateToGuidIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key constraints first
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Patients_PatientId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Medications_Patients_PatientId", 
                table: "Medications");

            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentUpdates_Patients_PatientId",
                table: "TreatmentUpdates");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Doctors_AssignedDoctorId",
                table: "Patients");

            // Drop primary key constraint
            migrationBuilder.DropPrimaryKey(
                name: "PK_Patients",
                table: "Patients");

            // Recreate Patients table with TEXT primary key
            migrationBuilder.CreateTable(
                name: "Patients_New",
                columns: table => new
                {
                    PatientId = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    CancerType = table.Column<string>(type: "TEXT", nullable: false),
                    Stage = table.Column<string>(type: "TEXT", nullable: false),
                    DiagnosisDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AssignedDoctorId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.PatientId);
                    table.ForeignKey(
                        name: "FK_Patients_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Patients_Doctors_AssignedDoctorId",
                        column: x => x.AssignedDoctorId,
                        principalTable: "Doctors",
                        principalColumn: "DoctorId",
                        onDelete: ReferentialAction.SetNull);
                });

            // Drop old table and rename new one
            migrationBuilder.DropTable(name: "Patients");
            
            migrationBuilder.RenameTable(
                name: "Patients_New",
                newName: "Patients");

            // Recreate indexes
            migrationBuilder.CreateIndex(
                name: "IX_Patients_AssignedDoctorId",
                table: "Patients",
                column: "AssignedDoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_UserId", 
                table: "Patients",
                column: "UserId");

            // Recreate foreign key constraints for dependent tables
            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Patients_PatientId",
                table: "Appointments",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Medications_Patients_PatientId",
                table: "Medications", 
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentUpdates_Patients_PatientId",
                table: "TreatmentUpdates",
                column: "PatientId", 
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // This rollback would be complex and may result in data loss
            // Consider backing up data before running this migration
            
            // Drop foreign key constraints first
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Patients_PatientId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Medications_Patients_PatientId",
                table: "Medications");

            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentUpdates_Patients_PatientId", 
                table: "TreatmentUpdates");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Doctors_AssignedDoctorId",
                table: "Patients");

            // Drop primary key constraint  
            migrationBuilder.DropPrimaryKey(
                name: "PK_Patients",
                table: "Patients");

            // Recreate Patients table with INTEGER AUTOINCREMENT primary key
            migrationBuilder.CreateTable(
                name: "Patients_Old", 
                columns: table => new
                {
                    PatientId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    CancerType = table.Column<string>(type: "TEXT", nullable: false),
                    Stage = table.Column<string>(type: "TEXT", nullable: false),
                    DiagnosisDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AssignedDoctorId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.PatientId);
                    table.ForeignKey(
                        name: "FK_Patients_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers", 
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Patients_Doctors_AssignedDoctorId",
                        column: x => x.AssignedDoctorId,
                        principalTable: "Doctors",
                        principalColumn: "DoctorId",
                        onDelete: ReferentialAction.SetNull);
                });

            // Drop current table and rename old one
            migrationBuilder.DropTable(name: "Patients");
            
            migrationBuilder.RenameTable(
                name: "Patients_Old",
                newName: "Patients");

            // Recreate indexes
            migrationBuilder.CreateIndex(
                name: "IX_Patients_AssignedDoctorId",
                table: "Patients", 
                column: "AssignedDoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_UserId",
                table: "Patients",
                column: "UserId");

            // Note: Foreign keys for dependent tables would need to be updated to INTEGER as well
            // This would require additional complex changes to maintain referential integrity
        }
    }
}
