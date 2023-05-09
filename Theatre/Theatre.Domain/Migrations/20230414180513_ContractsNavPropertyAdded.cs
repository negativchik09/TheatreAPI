using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Theatre.Domain.Migrations
{
    /// <inheritdoc />
    public partial class ContractsNavPropertyAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Shows_ShowId",
                table: "Contracts");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Shows_ShowId",
                table: "Contracts",
                column: "ShowId",
                principalTable: "Shows",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Shows_ShowId",
                table: "Contracts");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Shows_ShowId",
                table: "Contracts",
                column: "ShowId",
                principalTable: "Shows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
