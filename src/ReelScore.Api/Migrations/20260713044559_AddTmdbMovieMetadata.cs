using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReelScore.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTmdbMovieMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "backdrop_path",
                table: "movies",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "genres",
                table: "movies",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string>(
                name: "original_title",
                table: "movies",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "poster_path",
                table: "movies",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "release_date",
                table: "movies",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "runtime_minutes",
                table: "movies",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "tmdb_id",
                table: "movies",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_movies_tmdb_id",
                table: "movies",
                column: "tmdb_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_movies_tmdb_id",
                table: "movies");

            migrationBuilder.DropColumn(
                name: "backdrop_path",
                table: "movies");

            migrationBuilder.DropColumn(
                name: "genres",
                table: "movies");

            migrationBuilder.DropColumn(
                name: "original_title",
                table: "movies");

            migrationBuilder.DropColumn(
                name: "poster_path",
                table: "movies");

            migrationBuilder.DropColumn(
                name: "release_date",
                table: "movies");

            migrationBuilder.DropColumn(
                name: "runtime_minutes",
                table: "movies");

            migrationBuilder.DropColumn(
                name: "tmdb_id",
                table: "movies");
        }
    }
}
