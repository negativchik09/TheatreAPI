using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Theatre.Domain.Migrations
{
    /// <inheritdoc />
    public partial class ActorFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Actors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Actors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Telephone",
                table: "Actors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "Telephone",
                table: "Actors");
        }
    }
}
