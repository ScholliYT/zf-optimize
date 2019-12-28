using Microsoft.EntityFrameworkCore.Migrations;

namespace webapp.Migrations
{
    public partial class CastingCellAmount_To_Double : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "CastingCellAmount",
                table: "Ovens",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CastingCellAmount",
                table: "Ovens",
                type: "int",
                nullable: false,
                oldClrType: typeof(double));
        }
    }
}
