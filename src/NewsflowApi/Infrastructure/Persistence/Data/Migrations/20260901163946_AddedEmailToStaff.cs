using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsflowApi.Migrations
{
    /// <inheritdoc />
    public partial class AddedEmailToStaff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Staffs",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true
                );

            migrationBuilder.UpdateData(
                table: "Staffs",
                keyColumn: "Id",
                keyValue: new Guid("db595f26-04f4-4ceb-b25b-1079e6afd7a6"),
                column: "Email",
                value: "reporter@email.com"
                );

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Staffs",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false
                );

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_Email",
                table: "Staffs",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Staffs_Email",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Staffs");
        }
    }
}
