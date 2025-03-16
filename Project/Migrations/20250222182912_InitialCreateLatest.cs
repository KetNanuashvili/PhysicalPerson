using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateLatest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Relations_PhysicalPerson_ToId",
                table: "Relations");

            migrationBuilder.AddForeignKey(
                name: "FK_Relations_PhysicalPerson_ToId",
                table: "Relations",
                column: "ToId",
                principalTable: "PhysicalPerson",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Relations_PhysicalPerson_ToId",
                table: "Relations");

            migrationBuilder.AddForeignKey(
                name: "FK_Relations_PhysicalPerson_ToId",
                table: "Relations",
                column: "ToId",
                principalTable: "PhysicalPerson",
                principalColumn: "Id");
        }
    }
}
