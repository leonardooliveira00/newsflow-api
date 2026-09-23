using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsflowApi.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusToStaff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Staffs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Staffs");
        }
    }
}
