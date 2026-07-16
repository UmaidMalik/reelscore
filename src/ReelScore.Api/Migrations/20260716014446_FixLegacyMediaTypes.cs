using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReelScore.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixLegacyMediaTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DELETE FROM movies AS legacy
                USING movies AS valid
                WHERE legacy.tmdb_id = valid.tmdb_id
                AND (
                    legacy.media_type IS NULL
                    OR legacy.media_type = ''
                    OR legacy.media_type = '0'
                )
                AND valid.media_type = 'Movie';

                UPDATE movies
                SET media_type = 'Movie'
                WHERE media_type IS NULL
                OR media_type = ''
                OR media_type = '0';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
