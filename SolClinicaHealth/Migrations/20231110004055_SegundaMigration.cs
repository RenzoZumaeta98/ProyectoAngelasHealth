using Microsoft.EntityFrameworkCore.Migrations;

namespace SolClinicaHealth.Migrations
{
    public partial class SegundaMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EspecialidadIdEspecialidad",
                table: "Cita",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cita_EspecialidadIdEspecialidad",
                table: "Cita",
                column: "EspecialidadIdEspecialidad");

            migrationBuilder.AddForeignKey(
                name: "FK_Cita_Especialidad_EspecialidadIdEspecialidad",
                table: "Cita",
                column: "EspecialidadIdEspecialidad",
                principalTable: "Especialidad",
                principalColumn: "IdEspecialidad",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cita_Especialidad_EspecialidadIdEspecialidad",
                table: "Cita");

            migrationBuilder.DropIndex(
                name: "IX_Cita_EspecialidadIdEspecialidad",
                table: "Cita");

            migrationBuilder.DropColumn(
                name: "EspecialidadIdEspecialidad",
                table: "Cita");
        }
    }
}
