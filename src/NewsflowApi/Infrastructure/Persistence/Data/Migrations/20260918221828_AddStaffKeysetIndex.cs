using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsflowApi.Migrations
{
    /// <inheritdoc />
    public partial class AddStaffKeysetIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Staffs_CreatedAt_Id",
                table: "Staffs",
                columns: ["CreatedAt", "Id"]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Staffs_CreatedAt_Id",
                table: "Staffs");
        }
    }
}
