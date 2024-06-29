using System;
using System.Collections.Generic;
using System.Linq;

public class Genre
{
    public string Name { get; set; }
    public string Description { get; set; }
}

public class Game
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; }
    public IEnumerable<Genre> Genres { get; set; }
}

public static class Data
{
    public static List<Game> GetGames()
    {
        var genres = new List<Genre>
        {
            new Genre { Name = "Action", Description = "Action games" },
            new Genre { Name = "Adventure", Description = "Adventure games" },
            new Genre { Name = "RPG", Description = "Role-playing games" },
            new Genre { Name = "Strategy", Description = "Strategy games" },
            new Genre { Name = "Sports", Description = "Sports games" },
        };

        return new List<Game>
        {
            new Game { Id = 1, Name = "The Witcher 3: Wild Hunt", Price = 39.99m, Category = "AAA", Genres = new List<Genre> { genres[0], genres[2] } },
            new Game { Id = 2, Name = "Hades", Price = 24.99m, Category = "Indie", Genres = new List<Genre> { genres[0], genres[2] } },
            new Game { Id = 3, Name = "Red Dead Redemption 2", Price = 59.99m, Category = "AAA", Genres = new List<Genre> { genres[0], genres[1] } },
            new Game { Id = 4, Name = "Stardew Valley", Price = 14.99m, Category = "Indie", Genres = new List<Genre> { genres[1], genres[3] } },
            new Game { Id = 5, Name = "Cyberpunk 2077", Price = 49.99m, Category = "AAA", Genres = new List<Genre> { genres[0], genres[2] } },
            new Game { Id = 6, Name = "Rocket League", Price = 19.99m, Category = "Indie", Genres = new List<Genre> { genres[4], genres[0] } },
            new Game { Id = 7, Name = "The Elder Scrolls V: Skyrim", Price = 29.99m, Category = "AAA", Genres = new List<Genre> { genres[2], genres[1] } },
            new Game { Id = 8, Name = "Among Us", Price = 4.99m, Category = "Indie", Genres = new List<Genre> { genres[0], genres[1] } },
            new Game { Id = 9, Name = "Civilization VI", Price = 39.99m, Category = "AAA", Genres = new List<Genre> { genres[3] } },
            new Game { Id = 10, Name = "FIFA 21", Price = 59.99m, Category = "AAA", Genres = new List<Genre> { genres[4] } },
        };
    }
}

public static class GameQueries
{
    public static Game GetGameById(List<Game> games, int id)
    {
        return games.FirstOrDefault(g => g.Id == id);
    }

    public static List<Game> GetGamesInPriceRange(List<Game> games, decimal minPrice, decimal maxPrice)
    {
        return games.Where(g => g.Price >= minPrice && g.Price <= maxPrice).ToList();
    }

    public static List<Genre> GetGenresByGame(Game game)
    {
        return game.Genres.ToList();
    }

    public static List<string> GetUniqueCategories(List<Game> games)
    {
        return games.Select(g => g.Category).Distinct().ToList();
    }

    public static List<Game> FilterGamesByCategoryAndGenres(List<Game> games, string category, List<string> genreNames)
    {
        return games.Where(g => g.Category == category && g.Genres.Any(genre => genreNames.Contains(genre.Name))).ToList();
    }

    public static List<Game> GetGamesWithPagination(List<Game> games, int pageNumber, int pageSize = 5)
    {
        return games.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
    }
}

class Program
{
    static void Main()
    {
        var games = Data.GetGames();
        bool running = true;

        while (running)
        {
            Console.WriteLine("Select an option:");
            Console.WriteLine("1. Get one Game by Id");
            Console.WriteLine("2. Get a list of Games in price range");
            Console.WriteLine("3. Get a list of Genres by a game");
            Console.WriteLine("4. Get unique Categories from game list");
            Console.WriteLine("5. Filter Games by Category and Genres");
            Console.WriteLine("6. Get Games with pagination");
            Console.WriteLine("7. Exit");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Enter game Id: ");
                    int id = int.Parse(Console.ReadLine());
                    var game = GameQueries.GetGameById(games, id);
                    Console.WriteLine(game != null ? $"Game by ID {id}: {game.Name}" : "Game not found.");
                    break;
                case "2":
                    Console.Write("Enter min price: ");
                    decimal minPrice = decimal.Parse(Console.ReadLine());
                    Console.Write("Enter max price: ");
                    decimal maxPrice = decimal.Parse(Console.ReadLine());
                    var gamesInPriceRange = GameQueries.GetGamesInPriceRange(games, minPrice, maxPrice);
                    Console.WriteLine($"Games in price range {minPrice} - {maxPrice}:");
                    gamesInPriceRange.ForEach(g => Console.WriteLine(g.Name));
                    break;
                case "3":
                    Console.Write("Enter game Id: ");
                    int gameId = int.Parse(Console.ReadLine());
                    var genres = GameQueries.GetGenresByGame(GameQueries.GetGameById(games, gameId));
                    Console.WriteLine($"Genres for game {gameId}:");
                    genres.ForEach(g => Console.WriteLine(g.Name));
                    break;
                case "4":
                    var uniqueCategories = GameQueries.GetUniqueCategories(games);
                    Console.WriteLine("Unique categories:");
                    uniqueCategories.ForEach(c => Console.WriteLine(c));
                    break;
                case "5":
                    Console.Write("Enter category: ");
                    string category = Console.ReadLine();
                    Console.Write("Enter genre names (comma separated): ");
                    var genreNames = Console.ReadLine().Split(',').Select(name => name.Trim()).ToList();
                    var filteredGames = GameQueries.FilterGamesByCategoryAndGenres(games, category, genreNames);
                    Console.WriteLine($"Filtered games (Category: {category}, Genres: {string.Join(", ", genreNames)}):");
                    filteredGames.ForEach(g => Console.WriteLine(g.Name));
                    break;
                case "6":
                    Console.Write("Enter page number: ");
                    int pageNumber = int.Parse(Console.ReadLine());
                    var paginatedGames = GameQueries.GetGamesWithPagination(games, pageNumber);
                    Console.WriteLine($"Paginated games (Page {pageNumber}):");
                    paginatedGames.ForEach(g => Console.WriteLine(g.Name));
                    break;
                case "7":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option, please try again.");
                    break;
            }

            Console.WriteLine();
        }
    }
}
