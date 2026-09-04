using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsflowApi.Migrations
{
    /// <inheritdoc />
    public partial class ChangedPermissionNamePattern : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Name",
                keyValue: "STAFF_CREATE",
                column: "Name",
                value: "CREATE_STAFF"
                );

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Name",
                keyValue: "ROLE_MANAGE",
                column: "Name",
                value: "MANAGE_ROLE"
                );

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Name",
                keyValue: "USER_CREATE",
                column: "Name",
                value: "CREATE_USER"
                );

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Name",
                keyValue: "USER_UPDATE",
                column: "Name",
                value: "UPDATE_USER"
                );

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Name",
                keyValue: "USER_SUSPEND",
                column: "Name",
                value: "SUSPEND_USER"
                );

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Name",
                keyValue: "ARTICLE_CREATE",
                column: "Name",
                value: "CREATE_ARTICLE"
                );

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Name",
                keyValue: "ARTICLE_EDIT_OWN",
                column: "Name",
                value: "EDIT_OWN_ARTICLE"
                );

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Name",
                keyValue: "ARTICLE_EDIT_ANY",
                column: "Name",
                value: "EDIT_ANY_ARTICLE"
                );

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Name",
                keyValue: "ARTICLE_REVIEW",
                column: "Name",
                value: "REVIEW_ARTICLE"
                );

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Name",
                keyValue: "ARTICLE_APPROVE",
                column: "Name",
                value: "APPROVE_ARTICLE"
                );

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Name",
                keyValue: "ARTICLE_SUBMIT",
                column: "Name",
                value: "SUBMIT_ARTICLE"
                );

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Name",
                keyValue: "ARTICLE_REQUEST_CHANGES",
                column: "Name",
                value: "REQUEST_ARTICLE_CHANGES"
                );

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Name",
                keyValue: "ARTICLE_PUBLISH",
                column: "Name",
                value: "PUBLISH_ARTICLE"
                );

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Name",
                keyValue: "MEDIA_UPLOAD",
                column: "Name",
                value: "UPLOAD_MEDIA"
                );

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Name",
                keyValue: "ANALYTICS_VIEW",
                column: "Name",
                value: "VIEW_ANALYTICS"
                );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Name",
                keyValue: "CREATE_STAFF",
                column: "Name",
                value: "STAFF_CREATE"
                );

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Name",
                keyValue: "MANAGE_ROLE",
                column: "Name",
                value: "ROLE_MANAGE"
                );

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Name",
                keyValue: "CREATE_USER",
                column: "Name",
                value: "USER_CREATE"
                );

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Name",
                keyValue: "UPDATE_USER",
                column: "Name",
                value: "USER_UPDATE"
                );

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Name",
                keyValue: "SUSPEND_USER",
                column: "Name",
                value: "USER_SUSPEND"
                );

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Name",
                keyValue: "CREATE_ARTICLE",
                column: "Name",
                value: "ARTICLE_CREATE"
                );

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Name",
                keyValue: "EDIT_OWN_ARTICLE",
                column: "Name",
                value: "ARTICLE_EDIT_OWN"
                );

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Name",
                keyValue: "EDIT_ANY_ARTICLE",
                column: "Name",
                value: "ARTICLE_EDIT_ANY"
                );

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Name",
                keyValue: "REVIEW_ARTICLE",
                column: "Name",
                value: "ARTICLE_REVIEW"
                );

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Name",
                keyValue: "APPROVE_ARTICLE",
                column: "Name",
                value: "ARTICLE_APPROVE"
                );

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Name",
                keyValue: "SUBMIT_ARTICLE",
                column: "Name",
                value: "ARTICLE_SUBMIT"
                );

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Name",
                keyValue: "REQUEST_ARTICLE_CHANGES",
                column: "Name",
                value: "ARTICLE_REQUEST_CHANGES"
                );

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Name",
                keyValue: "PUBLISH_ARTICLE",
                column: "Name",
                value: "ARTICLE_PUBLISH"
                );

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Name",
                keyValue: "UPLOAD_MEDIA",
                column: "Name",
                value: "MEDIA_UPLOAD"
                );

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Name",
                keyValue: "VIEW_ANALYTICS",
                column: "Name",
                value: "ANALYTICS_VIEW"
                );
        }
    }
}

