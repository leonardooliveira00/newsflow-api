using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsflowApi.Migrations
{
    /// <inheritdoc />
    public partial class AddedPhoneAndBioForStaff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "Staffs",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                table: "Staffs",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true
                );

            migrationBuilder.UpdateData(
                table: "Staffs",
                keyColumn: "Id",
                keyValue: new Guid("db595f26-04f4-4ceb-b25b-1079e6afd7a6"),
                column: "ContactPhone",
                value: "85912345678"
                );

            migrationBuilder.AlterColumn<string>(
                name: "ContactPhone",
                table: "Staffs",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false
                );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bio",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                table: "Staffs");
        }
    }
}
