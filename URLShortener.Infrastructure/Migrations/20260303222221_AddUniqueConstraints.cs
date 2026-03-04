using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace URLShortener.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ShortCode",
                table: "ShortenedURLs",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "LongURL",
                table: "ShortenedURLs",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShortenedURLs_LongURL",
                table: "ShortenedURLs",
                column: "LongURL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShortenedURLs_ShortCode",
                table: "ShortenedURLs",
                column: "ShortCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_ShortenedURLs_LongURL",
                table: "ShortenedURLs");

            migrationBuilder.DropIndex(
                name: "IX_ShortenedURLs_ShortCode",
                table: "ShortenedURLs");

            migrationBuilder.AlterColumn<string>(
                name: "ShortCode",
                table: "ShortenedURLs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LongURL",
                table: "ShortenedURLs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
