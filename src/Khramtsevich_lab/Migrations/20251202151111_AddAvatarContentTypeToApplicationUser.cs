using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Khramtsevich_lab.Migrations
{
    /// <inheritdoc />
    public partial class AddAvatarContentTypeToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarContentType",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarContentType",
                table: "AspNetUsers");
        }
    }
}
