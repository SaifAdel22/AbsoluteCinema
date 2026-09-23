using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AbsoluteCinema.Migrations
{
    /// <inheritdoc />
    public partial class order : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "Seats",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "ApplicationUserOTPs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdateById",
                table: "ApplicationUserOTPs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UpdateById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_UpdateById",
                        column: x => x.UpdateById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Seats_OrderId",
                table: "Seats",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserOTPs_CreatedById",
                table: "ApplicationUserOTPs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserOTPs_UpdateById",
                table: "ApplicationUserOTPs",
                column: "UpdateById");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ApplicationUserId",
                table: "Orders",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CreatedById",
                table: "Orders",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_MovieId",
                table: "Orders",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UpdateById",
                table: "Orders",
                column: "UpdateById");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserOTPs_AspNetUsers_CreatedById",
                table: "ApplicationUserOTPs",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserOTPs_AspNetUsers_UpdateById",
                table: "ApplicationUserOTPs",
                column: "UpdateById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_Orders_OrderId",
                table: "Seats",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserOTPs_AspNetUsers_CreatedById",
                table: "ApplicationUserOTPs");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserOTPs_AspNetUsers_UpdateById",
                table: "ApplicationUserOTPs");

            migrationBuilder.DropForeignKey(
                name: "FK_Seats_Orders_OrderId",
                table: "Seats");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Seats_OrderId",
                table: "Seats");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationUserOTPs_CreatedById",
                table: "ApplicationUserOTPs");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationUserOTPs_UpdateById",
                table: "ApplicationUserOTPs");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "ApplicationUserOTPs");

            migrationBuilder.DropColumn(
                name: "UpdateById",
                table: "ApplicationUserOTPs");
        }
    }
}
