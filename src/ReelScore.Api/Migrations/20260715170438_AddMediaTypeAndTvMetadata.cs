using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReelScore.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaTypeAndTvMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_movies_tmdb_id",
                table: "movies");

            migrationBuilder.AddColumn<string>(
                name: "media_type",
                table: "movies",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "number_of_episodes",
                table: "movies",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "number_of_seasons",
                table: "movies",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_movies_tmdb_id_media_type",
                table: "movies",
                columns: new[] { "tmdb_id", "media_type" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_movies_tmdb_id_media_type",
                table: "movies");

            migrationBuilder.DropColumn(
                name: "media_type",
                table: "movies");

            migrationBuilder.DropColumn(
                name: "number_of_episodes",
                table: "movies");

            migrationBuilder.DropColumn(
                name: "number_of_seasons",
                table: "movies");

            migrationBuilder.CreateIndex(
                name: "IX_movies_tmdb_id",
                table: "movies",
                column: "tmdb_id",
                unique: true);
        }
    }
}
