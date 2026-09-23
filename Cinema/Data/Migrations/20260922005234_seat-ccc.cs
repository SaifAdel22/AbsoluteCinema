using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AbsoluteCinema.Migrations
{
    /// <inheritdoc />
    public partial class seatccc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Seats_Movies_MovieId1",
                table: "Seats");

            migrationBuilder.DropIndex(
                name: "IX_Seats_MovieId1",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "MovieId1",
                table: "Seats");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MovieId1",
                table: "Seats",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Seats_MovieId1",
                table: "Seats",
                column: "MovieId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_Movies_MovieId1",
                table: "Seats",
                column: "MovieId1",
                principalTable: "Movies",
                principalColumn: "Id");
        }
    }
}
