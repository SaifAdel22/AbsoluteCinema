using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AbsoluteCinema.Migrations
{
    public partial class SeedInitialData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Categories
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Drama" },
                    { 2, "Action / Western" },
                    { 3, "War / History" }
                });

            // 2. Cinemas
            migrationBuilder.InsertData(
                table: "Cinemas",
                columns: new[] { "Id", "Name", "Img" },
                values: new object[,]
                {
                    { 1, "IMAX Cinema City", "imax.jpg" },
                    { 2, "Grand Cinema", "grand.jpg" }
                });

            // 3. Actors
            migrationBuilder.InsertData(
                table: "Actors",
                columns: new[] { "Id", "Name", "Img" },
                values: new object[,]
                {
                    { 1, "Christian Bale", "bale.jpg" },
                    { 2, "Hugh Jackman", "jackman.jpg" },
                    { 3, "Jamie Foxx", "foxx.jpg" },
                    { 4, "Leonardo DiCaprio", "dicaprio.jpg" },
                    { 5, "Christoph Waltz", "waltz.jpg" },
                    { 6, "Brad Pitt", "pitt.jpg" }
                });

            // 4. Movies
            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "Id", "Name", "Description", "Price", "Status", "DateTime", "MainImg", "CategoryId", "CinemaId" },
                values: new object[,]
                {
                    {
                        1,
                        "The Prestige",
                        "Two stage magicians in 1890s London engage in a battle to create the ultimate illusion.",
                        150.00m,
                        true,
                        new DateTime(2026, 10, 1, 18, 0, 0),
                        "prestige_main.jpg",
                        1, // Drama
                        1  // IMAX
                    },
                    {
                        2,
                        "Django Unchained",
                        "A freed slave sets out to rescue his wife from a brutal plantation owner with the help of a German bounty-hunter.",
                        120.00m,
                        true,
                        new DateTime(2026, 10, 2, 21, 0, 0),
                        "django_main.jpg",
                        2, // Action/Western
                        2  // Grand Cinema
                    },
                    {
                        3,
                        "Inglourious Basterds",
                        "In Nazi-occupied France, a group of Jewish U.S. soldiers plan to assassinate Nazi leaders.",
                        140.00m,
                        true,
                        new DateTime(2026, 10, 3, 19, 30, 0),
                        "basterds_main.jpg",
                        3, // War
                        1  // IMAX
                    }
                });

            // 5. MovieActors (Join Table with CharacterName)
            migrationBuilder.InsertData(
                table: "MovieActors",
                columns: new[] { "MovieId", "ActorId", "CharacterName" },
                values: new object[,]
                {
                    // The Prestige
                    { 1, 1, "Alfred Borden" },
                    { 1, 2, "Robert Angier" },

                    // Django Unchained
                    { 2, 3, "Django Freeman" },
                    { 2, 4, "Calvin Candie" },
                    { 2, 5, "Dr. King Schultz" },

                    // Inglourious Basterds
                    { 3, 6, "Lt. Aldo Raine" },
                    { 3, 5, "Col. Hans Landa" }
                });

            // 6. MovieSubImgs
            migrationBuilder.InsertData(
                table: "MovieSubImgs",
                columns: new[] { "Id", "Img", "MovieId" },
                values: new object[,]
                {
                    { 1, "prestige_sub1.jpg", 1 },
                    { 2, "prestige_sub2.jpg", 1 },
                    { 3, "django_sub1.jpg", 2 },
                    { 4, "basterds_sub1.jpg", 3 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM MovieSubImgs");
            migrationBuilder.Sql("DELETE FROM MovieActors");
            migrationBuilder.Sql("DELETE FROM Movies");
            migrationBuilder.Sql("DELETE FROM Actors");
            migrationBuilder.Sql("DELETE FROM Cinemas");
            migrationBuilder.Sql("DELETE FROM Categories");
        }
    }
}