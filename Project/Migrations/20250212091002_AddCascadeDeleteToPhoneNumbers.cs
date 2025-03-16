using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Migrations
{
    /// <inheritdoc />
    public partial class AddCascadeDeleteToPhoneNumbers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhoneNumber_PhysicalPerson_PhysicalPersonId",
                table: "PhoneNumber");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhoneNumber",
                table: "PhoneNumber");

            migrationBuilder.RenameTable(
                name: "PhoneNumber",
                newName: "PhoneNumbers");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "PhoneNumbers",
                newName: "Type");

            migrationBuilder.RenameIndex(
                name: "IX_PhoneNumber_PhysicalPersonId",
                table: "PhoneNumbers",
                newName: "IX_PhoneNumbers_PhysicalPersonId");

            migrationBuilder.AlterColumn<int>(
                name: "PhysicalPersonId",
                table: "PhoneNumbers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhoneNumbers",
                table: "PhoneNumbers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PhoneNumbers_PhysicalPerson_PhysicalPersonId",
                table: "PhoneNumbers",
                column: "PhysicalPersonId",
                principalTable: "PhysicalPerson",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhoneNumbers_PhysicalPerson_PhysicalPersonId",
                table: "PhoneNumbers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhoneNumbers",
                table: "PhoneNumbers");

            migrationBuilder.RenameTable(
                name: "PhoneNumbers",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "PhoneNumber",
                newName: "type");

            migrationBuilder.RenameIndex(
                name: "IX_PhoneNumbers_PhysicalPersonId",
                table: "PhoneNumber",
                newName: "IX_PhoneNumber_PhysicalPersonId");

            migrationBuilder.AlterColumn<int>(
                name: "PhysicalPersonId",
                table: "PhoneNumber",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhoneNumber",
                table: "PhoneNumber",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PhoneNumber_PhysicalPerson_PhysicalPersonId",
                table: "PhoneNumber",
                column: "PhysicalPersonId",
                principalTable: "PhysicalPerson",
                principalColumn: "Id");
        }
    }
}
