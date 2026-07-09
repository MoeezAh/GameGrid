using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameCollection.Domain.Entities;
using GameCollection.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameCollection.Infrastructure.Data;

public static class GameDbContextSeed
{
    public static async Task SeedAsync(GameDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // 1. Seed Roles
        if (!await roleManager.RoleExistsAsync("Administrator"))
        {
            await roleManager.CreateAsync(new IdentityRole("Administrator"));
        }
        if (!await roleManager.RoleExistsAsync("User"))
        {
            await roleManager.CreateAsync(new IdentityRole("User"));
        }

        // 2. Seed Users
        IdentityUser? adminUser = await userManager.FindByNameAsync("admin");
        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = "admin",
                Email = "admin@gamecollection.com",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Administrator");
                await userManager.AddToRoleAsync(adminUser, "User");
            }
        }

        IdentityUser? normalUser = await userManager.FindByNameAsync("user");
        if (normalUser == null)
        {
            normalUser = new IdentityUser
            {
                UserName = "user",
                Email = "user@gamecollection.com",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(normalUser, "User123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(normalUser, "User");
            }
        }

        // 3. Seed Master Data (Only if empty)
        if (!await context.Platforms.AnyAsync())
        {
            var platforms = new List<Platform>
            {
                new() { Name = "PC", Manufacturer = "Various", ReleaseDate = new DateTimeOffset(1981, 8, 12, 0, 0, 0, TimeSpan.Zero), Generation = 0, Notes = "Personal Computer" },
                new() { Name = "PlayStation 5", Manufacturer = "Sony", ReleaseDate = new DateTimeOffset(2020, 11, 12, 0, 0, 0, TimeSpan.Zero), Generation = 9, Notes = "Next-gen Sony console" },
                new() { Name = "Xbox Series X/S", Manufacturer = "Microsoft", ReleaseDate = new DateTimeOffset(2020, 11, 10, 0, 0, 0, TimeSpan.Zero), Generation = 9, Notes = "Next-gen Microsoft console" },
                new() { Name = "Nintendo Switch", Manufacturer = "Nintendo", ReleaseDate = new DateTimeOffset(2017, 3, 3, 0, 0, 0, TimeSpan.Zero), Generation = 8, Notes = "Hybrid handheld/home console" },
                new() { Name = "Steam Deck", Manufacturer = "Valve", ReleaseDate = new DateTimeOffset(2022, 2, 25, 0, 0, 0, TimeSpan.Zero), Generation = 8, Notes = "Handheld gaming PC" }
            };
            await context.Platforms.AddRangeAsync(platforms);
        }

        if (!await context.DigitalServices.AnyAsync())
        {
            var services = new List<DigitalService>
            {
                new() { Name = "Steam", Website = "https://store.steampowered.com", Notes = "Valve's digital distribution platform" },
                new() { Name = "Epic Games Store", Website = "https://store.epicgames.com", Notes = "Epic Games digital platform" },
                new() { Name = "GOG", Website = "https://www.gog.com", Notes = "DRM-free PC games service by CD Projekt" },
                new() { Name = "Xbox Game Pass", Website = "https://www.xbox.com/xbox-game-pass", Notes = "Microsoft game subscription service" },
                new() { Name = "PlayStation Plus", Website = "https://www.playstation.com/playstation-plus", Notes = "Sony game subscription service" }
            };
            await context.DigitalServices.AddRangeAsync(services);
        }

        if (!await context.Developers.AnyAsync())
        {
            var devs = new List<Developer>
            {
                new() { Name = "Valve", Website = "https://www.valvesoftware.com", Country = "USA", FoundedDate = new DateTimeOffset(1996, 8, 24, 0, 0, 0, TimeSpan.Zero), Description = "Creators of Steam, Half-Life, Portal, etc." },
                new() { Name = "CD Projekt Red", Website = "https://cdprojektred.com", Country = "Poland", FoundedDate = new DateTimeOffset(2002, 7, 1, 0, 0, 0, TimeSpan.Zero), Description = "Creators of The Witcher series and Cyberpunk 2077" },
                new() { Name = "FromSoftware", Website = "https://www.fromsoftware.jp", Country = "Japan", FoundedDate = new DateTimeOffset(1986, 11, 1, 0, 0, 0, TimeSpan.Zero), Description = "Creators of Dark Souls, Bloodborne, Elden Ring, etc." },
                new() { Name = "Nintendo", Website = "https://www.nintendo.com", Country = "Japan", FoundedDate = new DateTimeOffset(1889, 9, 23, 0, 0, 0, TimeSpan.Zero), Description = "Gaming pioneers, creators of Mario, Zelda, Metroid" }
            };
            await context.Developers.AddRangeAsync(devs);
        }

        if (!await context.Publishers.AnyAsync())
        {
            var pubs = new List<Publisher>
            {
                new() { Name = "Valve", Website = "https://www.valvesoftware.com", Country = "USA" },
                new() { Name = "CD Projekt", Website = "https://www.cdprojekt.com", Country = "Poland" },
                new() { Name = "Bandai Namco", Website = "https://www.bandainamcoent.com", Country = "Japan" },
                new() { Name = "Nintendo", Website = "https://www.nintendo.com", Country = "Japan" }
            };
            await context.Publishers.AddRangeAsync(pubs);
        }

        if (!await context.Genres.AnyAsync())
        {
            var genres = new List<Genre>
            {
                new() { Name = "Action", Description = "Fast-paced gameplay focusing on physical challenges" },
                new() { Name = "RPG", Description = "Role-Playing Games focusing on character progression and narrative" },
                new() { Name = "Adventure", Description = "Focusing on exploration, puzzle-solving, and story" },
                new() { Name = "FPS", Description = "First-Person Shooters" },
                new() { Name = "Strategy", Description = "Tactical planning and resource management" },
                new() { Name = "Simulation", Description = "Simulating real-world or fictional systems" }
            };
            await context.Genres.AddRangeAsync(genres);
        }

        if (!await context.Themes.AnyAsync())
        {
            var themes = new List<Theme>
            {
                new() { Name = "Sci-Fi", Description = "Futuristic science fiction settings" },
                new() { Name = "Fantasy", Description = "Magic, swords, and mythological settings" },
                new() { Name = "Cyberpunk", Description = "High tech, low life dystopian settings" },
                new() { Name = "Horror", Description = "Scary, spooky, or survival horror settings" }
            };
            await context.Themes.AddRangeAsync(themes);
        }

        if (!await context.Tags.AnyAsync())
        {
            var tags = new List<Tag>
            {
                new() { Name = "Singleplayer" },
                new() { Name = "Multiplayer" },
                new() { Name = "Co-op" },
                new() { Name = "Open World" },
                new() { Name = "Indie" }
            };
            await context.Tags.AddRangeAsync(tags);
        }

        if (!await context.Franchises.AnyAsync())
        {
            var franchises = new List<Franchise>
            {
                new() { Name = "The Witcher", Description = "Geralt of Rivia fantasy franchise" },
                new() { Name = "Dark Souls", Description = "Challenging action-RPG series" },
                new() { Name = "The Legend of Zelda", Description = "Nintendo's legendary adventure series" }
            };
            await context.Franchises.AddRangeAsync(franchises);
        }

        if (!await context.Series.AnyAsync())
        {
            var series = new List<Series>
            {
                new() { Name = "Main Entry", Description = "Core canon releases" },
                new() { Name = "Spin-off", Description = "Alternative releases" }
            };
            await context.Series.AddRangeAsync(series);
        }

        await context.SaveChangesAsync();

        // 4. Seed Games (Only if empty library)
        if (!await context.Games.AnyAsync())
        {
            // Retrieve seeded lookup references
            var pc = await context.Platforms.FirstAsync(p => p.Name == "PC");
            var ps5 = await context.Platforms.FirstAsync(p => p.Name == "PlayStation 5");
            var switchPlat = await context.Platforms.FirstAsync(p => p.Name == "Nintendo Switch");
            var deck = await context.Platforms.FirstAsync(p => p.Name == "Steam Deck");

            var steam = await context.DigitalServices.FirstAsync(s => s.Name == "Steam");
            var gog = await context.DigitalServices.FirstAsync(s => s.Name == "GOG");

            var cdpr = await context.Developers.FirstAsync(d => d.Name == "CD Projekt Red");
            var fromsoft = await context.Developers.FirstAsync(d => d.Name == "FromSoftware");
            var nintendo = await context.Developers.FirstAsync(d => d.Name == "Nintendo");

            var cdprPub = await context.Publishers.FirstAsync(p => p.Name == "CD Projekt");
            var bandai = await context.Publishers.FirstAsync(p => p.Name == "Bandai Namco");
            var nintendoPub = await context.Publishers.FirstAsync(p => p.Name == "Nintendo");

            var rpg = await context.Genres.FirstAsync(g => g.Name == "RPG");
            var action = await context.Genres.FirstAsync(g => g.Name == "Action");
            var adventure = await context.Genres.FirstAsync(g => g.Name == "Adventure");

            var fantasy = await context.Themes.FirstAsync(t => t.Name == "Fantasy");
            var cyberpunkTheme = await context.Themes.FirstAsync(t => t.Name == "Cyberpunk");
            var sciFi = await context.Themes.FirstAsync(t => t.Name == "Sci-Fi");

            var sp = await context.Tags.FirstAsync(t => t.Name == "Singleplayer");
            var ow = await context.Tags.FirstAsync(t => t.Name == "Open World");

            var witcherFranchise = await context.Franchises.FirstAsync(f => f.Name == "The Witcher");
            var zeldaFranchise = await context.Franchises.FirstAsync(f => f.Name == "The Legend of Zelda");

            var mainSeries = await context.Series.FirstAsync(s => s.Name == "Main Entry");

            var games = new List<Game>
            {
                new()
                {
                    Title = "The Witcher 3: Wild Hunt",
                    AlternateTitles = "Witcher 3, wild hunt",
                    OriginalTitle = "Wiedźmin 3: Dziki Gon",
                    Description = "The Witcher: Wild Hunt is a story-driven, next-generation open world role-playing game set in a visually stunning fantasy universe full of meaningful choices and impactful consequences.",
                    Notes = "Bought during GOG Summer Sale. Outstanding RPG, highly recommended.",
                    PersonalNotes = "Completed twice, including Hearts of Stone and Blood and Wine expansions.",
                    OwnGame = true,
                    Wishlist = false,
                    Backlog = false,
                    PhysicalCopy = false,
                    DigitalCopy = true,
                    CollectorsEdition = false,
                    SpecialEdition = true,
                    PurchaseDate = new DateTimeOffset(2018, 6, 20, 0, 0, 0, TimeSpan.Zero),
                    PurchasePrice = 14.99m,
                    Currency = "USD",
                    StorePurchasedFrom = "GOG",
                    PurchaseRegion = "Global",
                    Gifted = false,
                    StartedPlayingDate = new DateTimeOffset(2018, 6, 22, 0, 0, 0, TimeSpan.Zero),
                    CompletedDate = new DateTimeOffset(2018, 8, 15, 0, 0, 0, TimeSpan.Zero),
                    LastPlayedDate = new DateTimeOffset(2025, 12, 25, 0, 0, 0, TimeSpan.Zero),
                    HoursPlayed = 154.5,
                    CompletionStatus = CompletionStatus.Completed100,
                    PersonalRating = 10,
                    CommunityRating = 9.8,
                    CriticRating = 9.3,
                    ReleaseDate = new DateTimeOffset(2015, 5, 19, 0, 0, 0, TimeSpan.Zero),
                    EsrbRating = "M",
                    PegiRating = "18",
                    MetacriticScore = 93,
                    OpenCriticScore = 92,
                    MultiplayerSupport = false,
                    CoopSupport = false,
                    VrSupport = false,
                    CrossplaySupport = false,
                    CloudSaveSupport = true,
                    ControllerSupport = true,
                    SteamDeckCompatibility = "Verified",
                    AchievementCount = 78,
                    DlcCount = 16,
                    ExpansionCount = 2,
                    UserId = adminUser?.Id ?? "System",
                    FranchiseId = witcherFranchise.Id,
                    SeriesId = mainSeries.Id,
                    Platforms = new List<Platform> { pc, deck },
                    DigitalServices = new List<DigitalService> { gog },
                    Developers = new List<Developer> { cdpr },
                    Publishers = new List<Publisher> { cdprPub },
                    Genres = new List<Genre> { rpg },
                    Themes = new List<Theme> { fantasy },
                    Tags = new List<Tag> { sp, ow }
                },
                new()
                {
                    Title = "Elden Ring",
                    OriginalTitle = "エルデンリング",
                    Description = "Rise, Tarnished, and be guided by grace to brandish the power of the Elden Ring and become an Elden Lord in the Lands Between.",
                    Notes = "Pre-ordered on Steam. Masterpiece of open-world design.",
                    OwnGame = true,
                    Wishlist = false,
                    Backlog = false,
                    PhysicalCopy = false,
                    DigitalCopy = true,
                    CollectorsEdition = false,
                    SpecialEdition = false,
                    PurchaseDate = new DateTimeOffset(2022, 2, 24, 0, 0, 0, TimeSpan.Zero),
                    PurchasePrice = 59.99m,
                    Currency = "USD",
                    StorePurchasedFrom = "Steam",
                    PurchaseRegion = "Global",
                    Gifted = false,
                    StartedPlayingDate = new DateTimeOffset(2022, 2, 25, 0, 0, 0, TimeSpan.Zero),
                    LastPlayedDate = new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.Zero),
                    HoursPlayed = 85.0,
                    CompletionStatus = CompletionStatus.Playing,
                    PersonalRating = 9.5,
                    CommunityRating = 9.5,
                    CriticRating = 9.6,
                    ReleaseDate = new DateTimeOffset(2022, 2, 25, 0, 0, 0, TimeSpan.Zero),
                    EsrbRating = "M",
                    PegiRating = "16",
                    MetacriticScore = 96,
                    OpenCriticScore = 95,
                    MultiplayerSupport = true,
                    CoopSupport = true,
                    VrSupport = false,
                    CrossplaySupport = false,
                    CloudSaveSupport = true,
                    ControllerSupport = true,
                    SteamDeckCompatibility = "Verified",
                    AchievementCount = 42,
                    DlcCount = 1,
                    ExpansionCount = 0,
                    UserId = adminUser?.Id ?? "System",
                    SeriesId = mainSeries.Id,
                    Platforms = new List<Platform> { pc, ps5 },
                    DigitalServices = new List<DigitalService> { steam },
                    Developers = new List<Developer> { fromsoft },
                    Publishers = new List<Publisher> { bandai },
                    Genres = new List<Genre> { rpg, action },
                    Themes = new List<Theme> { fantasy },
                    Tags = new List<Tag> { sp, ow }
                },
                new()
                {
                    Title = "The Legend of Zelda: Breath of the Wild",
                    Description = "Forget everything you know about The Legend of Zelda games. Step into a world of discovery, exploration, and adventure in The Legend of Zelda: Breath of the Wild, a boundary-breaking new game in the acclaimed series.",
                    Notes = "Physical copy bought with Switch console.",
                    OwnGame = true,
                    Wishlist = false,
                    Backlog = false,
                    PhysicalCopy = true,
                    DigitalCopy = false,
                    CollectorsEdition = false,
                    SpecialEdition = false,
                    PurchaseDate = new DateTimeOffset(2017, 3, 3, 0, 0, 0, TimeSpan.Zero),
                    PurchasePrice = 59.99m,
                    Currency = "USD",
                    StorePurchasedFrom = "Amazon",
                    Gifted = false,
                    StartedPlayingDate = new DateTimeOffset(2017, 3, 4, 0, 0, 0, TimeSpan.Zero),
                    CompletedDate = new DateTimeOffset(2017, 4, 15, 0, 0, 0, TimeSpan.Zero),
                    LastPlayedDate = new DateTimeOffset(2020, 5, 20, 0, 0, 0, TimeSpan.Zero),
                    HoursPlayed = 120.0,
                    CompletionStatus = CompletionStatus.Completed,
                    PersonalRating = 9.5,
                    CommunityRating = 9.6,
                    CriticRating = 9.7,
                    ReleaseDate = new DateTimeOffset(2017, 3, 3, 0, 0, 0, TimeSpan.Zero),
                    EsrbRating = "E10+",
                    PegiRating = "12",
                    MetacriticScore = 97,
                    OpenCriticScore = 96,
                    MultiplayerSupport = false,
                    CoopSupport = false,
                    VrSupport = false,
                    CrossplaySupport = false,
                    CloudSaveSupport = true,
                    ControllerSupport = true,
                    SteamDeckCompatibility = "Unsupported",
                    UserId = adminUser?.Id ?? "System",
                    FranchiseId = zeldaFranchise.Id,
                    SeriesId = mainSeries.Id,
                    Platforms = new List<Platform> { switchPlat },
                    Developers = new List<Developer> { nintendo },
                    Publishers = new List<Publisher> { nintendoPub },
                    Genres = new List<Genre> { adventure, action },
                    Themes = new List<Theme> { fantasy },
                    Tags = new List<Tag> { sp, ow }
                },
                new()
                {
                    Title = "Cyberpunk 2077",
                    Description = "Cyberpunk 2077 is an open-world, action-adventure RPG set in the megalopolis of Night City, where you play as a cyberpunk mercenary wrapped up in a do-or-die fight for survival.",
                    Notes = "Steam digital download key.",
                    OwnGame = true,
                    Wishlist = false,
                    Backlog = true,
                    PhysicalCopy = false,
                    DigitalCopy = true,
                    CollectorsEdition = false,
                    SpecialEdition = false,
                    PurchaseDate = new DateTimeOffset(2020, 12, 10, 0, 0, 0, TimeSpan.Zero),
                    PurchasePrice = 29.99m,
                    Currency = "USD",
                    StorePurchasedFrom = "Steam",
                    Gifted = false,
                    StartedPlayingDate = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero),
                    LastPlayedDate = new DateTimeOffset(2021, 1, 5, 0, 0, 0, TimeSpan.Zero),
                    HoursPlayed = 4.5,
                    CompletionStatus = CompletionStatus.OnHold,
                    PersonalRating = 7.0,
                    CommunityRating = 8.5,
                    CriticRating = 8.6,
                    ReleaseDate = new DateTimeOffset(2020, 12, 10, 0, 0, 0, TimeSpan.Zero),
                    EsrbRating = "M",
                    PegiRating = "18",
                    MetacriticScore = 86,
                    OpenCriticScore = 82,
                    UserId = adminUser?.Id ?? "System",
                    Platforms = new List<Platform> { pc, deck },
                    DigitalServices = new List<DigitalService> { steam },
                    Developers = new List<Developer> { cdpr },
                    Publishers = new List<Publisher> { cdprPub },
                    Genres = new List<Genre> { rpg, action },
                    Themes = new List<Theme> { cyberpunkTheme, sciFi },
                    Tags = new List<Tag> { sp, ow }
                }
            };

            await context.Games.AddRangeAsync(games);

            // Let's copy the same games to normalUser to give them seed data too!
            foreach (var g in games)
            {
                var userGameCopy = new Game
                {
                    Title = g.Title,
                    AlternateTitles = g.AlternateTitles,
                    OriginalTitle = g.OriginalTitle,
                    Description = g.Description,
                    Notes = g.Notes,
                    PersonalNotes = g.PersonalNotes,
                    OwnGame = g.OwnGame,
                    Wishlist = g.Wishlist,
                    Backlog = g.Backlog,
                    PhysicalCopy = g.PhysicalCopy,
                    DigitalCopy = g.DigitalCopy,
                    CollectorsEdition = g.CollectorsEdition,
                    SpecialEdition = g.SpecialEdition,
                    PurchaseDate = g.PurchaseDate,
                    PurchasePrice = g.PurchasePrice,
                    Currency = g.Currency,
                    StorePurchasedFrom = g.StorePurchasedFrom,
                    Gifted = g.Gifted,
                    StartedPlayingDate = g.StartedPlayingDate,
                    CompletedDate = g.CompletedDate,
                    LastPlayedDate = g.LastPlayedDate,
                    HoursPlayed = g.HoursPlayed * 0.8, // Slightly different playtime
                    CompletionStatus = g.CompletionStatus,
                    PersonalRating = g.PersonalRating,
                    CommunityRating = g.CommunityRating,
                    CriticRating = g.CriticRating,
                    ReleaseDate = g.ReleaseDate,
                    EsrbRating = g.EsrbRating,
                    PegiRating = g.PegiRating,
                    MetacriticScore = g.MetacriticScore,
                    OpenCriticScore = g.OpenCriticScore,
                    UserId = normalUser?.Id ?? "UserSystem",
                    FranchiseId = g.FranchiseId,
                    SeriesId = g.SeriesId,
                    Platforms = g.Platforms.ToList(),
                    DigitalServices = g.DigitalServices.ToList(),
                    Developers = g.Developers.ToList(),
                    Publishers = g.Publishers.ToList(),
                    Genres = g.Genres.ToList(),
                    Themes = g.Themes.ToList(),
                    Tags = g.Tags.ToList()
                };
                await context.Games.AddAsync(userGameCopy);
            }

            await context.SaveChangesAsync();
        }
    }
}
