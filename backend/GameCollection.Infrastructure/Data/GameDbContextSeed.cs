using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameCollection.Application.Common.Security;
using GameCollection.Domain.Entities;
using GameCollection.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameCollection.Infrastructure.Data;

public static class GameDbContextSeed
{
    public static async Task SeedAsync(GameDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // 1. Seed Permissions
        var existingPermissionNames = await context.Permissions.Select(p => p.Name).ToListAsync();
        var missingPermissions = Permissions.All
            .Where(p => !existingPermissionNames.Contains(p.Name))
            .Select(p => new Permission
            {
                Name = p.Name,
                DisplayName = p.DisplayName,
                Description = p.Description,
                Category = p.Category
            })
            .ToList();

        if (missingPermissions.Any())
        {
            await context.Permissions.AddRangeAsync(missingPermissions);
            await context.SaveChangesAsync();
        }

        var allPermissions = await context.Permissions.ToListAsync();

        // 2. Seed Standard Roles
        var superAdminRole = await context.ApplicationRoles.FirstOrDefaultAsync(r => r.Name == "Super Admin" && !r.IsDeleted);
        if (superAdminRole == null)
        {
            superAdminRole = new ApplicationRole
            {
                Name = "Super Admin",
                Description = "Unrestricted system administrator with full access to all resources and role management.",
                IsActive = true,
                IsSuperAdmin = true,
                IsSystemRole = true,
                CreatedDate = DateTimeOffset.UtcNow,
                CreatedBy = "System"
            };
            await context.ApplicationRoles.AddAsync(superAdminRole);
            await context.SaveChangesAsync();
        }

        var adminRole = await context.ApplicationRoles.Include(r => r.RolePermissions).FirstOrDefaultAsync(r => r.Name == "Admin" && !r.IsDeleted);
        if (adminRole == null)
        {
            adminRole = new ApplicationRole
            {
                Name = "Admin",
                Description = "Administrator with access to catalog, users, metadata and request review.",
                IsActive = true,
                IsSuperAdmin = false,
                IsSystemRole = false,
                CreatedDate = DateTimeOffset.UtcNow,
                CreatedBy = "System"
            };

            // Grant all permissions except Roles.AssignPermissions and Roles.Delete
            var adminPerms = allPermissions
                .Where(p => p.Category != "Roles" || p.Name == Permissions.Roles.View)
                .Select(p => new ApplicationRolePermission { PermissionId = p.Id });

            foreach (var p in adminPerms) adminRole.RolePermissions.Add(p);

            await context.ApplicationRoles.AddAsync(adminRole);
            await context.SaveChangesAsync();
        }

        var curatorRole = await context.ApplicationRoles.Include(r => r.RolePermissions).FirstOrDefaultAsync(r => r.Name == "Game Curator" && !r.IsDeleted);
        if (curatorRole == null)
        {
            curatorRole = new ApplicationRole
            {
                Name = "Game Curator",
                Description = "Manages central catalog games, reviews game requests, and organizes metadata.",
                IsActive = true,
                IsSuperAdmin = false,
                IsSystemRole = false,
                CreatedDate = DateTimeOffset.UtcNow,
                CreatedBy = "System"
            };

            var curatorPermNames = new HashSet<string>
            {
                Permissions.Games.View, Permissions.Games.Create, Permissions.Games.Update, Permissions.Games.ManageMetadata,
                Permissions.GameRequests.ViewAny, Permissions.GameRequests.Approve, Permissions.GameRequests.Reject, Permissions.GameRequests.Edit,
                Permissions.Metadata.View, Permissions.Metadata.ManagePlatforms, Permissions.Metadata.ManageServices,
                Permissions.Metadata.ManageDevelopers, Permissions.Metadata.ManagePublishers, Permissions.Metadata.ManageTaxonomies,
                Permissions.Libraries.ViewOwn, Permissions.Libraries.AddGame, Permissions.Libraries.UpdateOwn, Permissions.Libraries.RemoveGame
            };

            var curatorPerms = allPermissions
                .Where(p => curatorPermNames.Contains(p.Name))
                .Select(p => new ApplicationRolePermission { PermissionId = p.Id });

            foreach (var p in curatorPerms) curatorRole.RolePermissions.Add(p);

            await context.ApplicationRoles.AddAsync(curatorRole);
            await context.SaveChangesAsync();
        }

        var userRole = await context.ApplicationRoles.Include(r => r.RolePermissions).FirstOrDefaultAsync(r => r.Name == "User" && !r.IsDeleted);
        if (userRole == null)
        {
            userRole = new ApplicationRole
            {
                Name = "User",
                Description = "Standard gamer with personal library management and game request submission.",
                IsActive = true,
                IsSuperAdmin = false,
                IsSystemRole = false,
                CreatedDate = DateTimeOffset.UtcNow,
                CreatedBy = "System"
            };

            var userPermNames = new HashSet<string>
            {
                Permissions.Games.View,
                Permissions.Libraries.ViewOwn, Permissions.Libraries.AddGame, Permissions.Libraries.UpdateOwn, Permissions.Libraries.RemoveGame,
                Permissions.GameRequests.Create, Permissions.GameRequests.ViewOwn,
                Permissions.Metadata.View
            };

            var userPerms = allPermissions
                .Where(p => userPermNames.Contains(p.Name))
                .Select(p => new ApplicationRolePermission { PermissionId = p.Id });

            foreach (var p in userPerms) userRole.RolePermissions.Add(p);

            await context.ApplicationRoles.AddAsync(userRole);
            await context.SaveChangesAsync();
        }

        // 3. Seed Users & Assign Application Roles
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
                if (!await roleManager.RoleExistsAsync("Administrator"))
                    await roleManager.CreateAsync(new IdentityRole("Administrator"));
                await userManager.AddToRoleAsync(adminUser, "Administrator");
            }
        }

        // Assign Super Admin role to admin user
        if (adminUser != null && !await context.ApplicationUserRoles.AnyAsync(ur => ur.UserId == adminUser.Id && ur.RoleId == superAdminRole.Id))
        {
            await context.ApplicationUserRoles.AddAsync(new ApplicationUserRole
            {
                UserId = adminUser.Id,
                RoleId = superAdminRole.Id
            });
            await context.SaveChangesAsync();
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
                if (!await roleManager.RoleExistsAsync("User"))
                    await roleManager.CreateAsync(new IdentityRole("User"));
                await userManager.AddToRoleAsync(normalUser, "User");
            }
        }

        // Assign User role to normal user
        if (normalUser != null && !await context.ApplicationUserRoles.AnyAsync(ur => ur.UserId == normalUser.Id && ur.RoleId == userRole.Id))
        {
            await context.ApplicationUserRoles.AddAsync(new ApplicationUserRole
            {
                UserId = normalUser.Id,
                RoleId = userRole.Id
            });
            await context.SaveChangesAsync();
        }

        // 4. Seed Metadata Lookups
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
            await context.SaveChangesAsync();
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
            await context.SaveChangesAsync();
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
            await context.SaveChangesAsync();
        }

        if (!await context.Publishers.AnyAsync())
        {
            var pubs = new List<Publisher>
            {
                new() { Name = "Bandai Namco Entertainment", Website = "https://en.bandainamcoent.eu", Country = "Japan", FoundedDate = new DateTimeOffset(2006, 3, 31, 0, 0, 0, TimeSpan.Zero) },
                new() { Name = "CD Projekt", Website = "https://www.cdprojekt.com", Country = "Poland", FoundedDate = new DateTimeOffset(1994, 5, 1, 0, 0, 0, TimeSpan.Zero) },
                new() { Name = "Nintendo", Website = "https://www.nintendo.com", Country = "Japan", FoundedDate = new DateTimeOffset(1889, 9, 23, 0, 0, 0, TimeSpan.Zero) },
                new() { Name = "Electronic Arts", Website = "https://www.ea.com", Country = "USA", FoundedDate = new DateTimeOffset(1982, 5, 27, 0, 0, 0, TimeSpan.Zero) }
            };
            await context.Publishers.AddRangeAsync(pubs);
            await context.SaveChangesAsync();
        }

        if (!await context.Genres.AnyAsync())
        {
            var genres = new List<Genre>
            {
                new() { Name = "Action", Description = "Fast-paced games focusing on physical challenges" },
                new() { Name = "RPG", Description = "Role-playing games featuring character progression and storytelling" },
                new() { Name = "Adventure", Description = "Exploration and puzzle-solving journeys" },
                new() { Name = "Strategy", Description = "Tactical planning and resource management" },
                new() { Name = "Shooter", Description = "Gunplay and projectile challenges" }
            };
            await context.Genres.AddRangeAsync(genres);
            await context.SaveChangesAsync();
        }

        if (!await context.Tags.AnyAsync())
        {
            var tags = new List<Tag>
            {
                new() { Name = "Singleplayer" },
                new() { Name = "Open World" },
                new() { Name = "Souls-like" },
                new() { Name = "Cyberpunk" },
                new() { Name = "Atmospheric" },
                new() { Name = "Masterpiece" }
            };
            await context.Tags.AddRangeAsync(tags);
            await context.SaveChangesAsync();
        }

        if (!await context.Themes.AnyAsync())
        {
            var themes = new List<Theme>
            {
                new() { Name = "Dark Fantasy" },
                new() { Name = "Sci-Fi" },
                new() { Name = "Post-Apocalyptic" },
                new() { Name = "Cyberpunk" },
                new() { Name = "High Fantasy" }
            };
            await context.Themes.AddRangeAsync(themes);
            await context.SaveChangesAsync();
        }

        if (!await context.Franchises.AnyAsync())
        {
            var franchises = new List<Franchise>
            {
                new() { Name = "The Legend of Zelda", Description = "Iconic Nintendo action-adventure franchise" },
                new() { Name = "The Witcher", Description = "Dark fantasy series based on Andrzej Sapkowski novels" },
                new() { Name = "Dark Souls / Soulsborne", Description = "Challenging action RPGs by FromSoftware" }
            };
            await context.Franchises.AddRangeAsync(franchises);
            await context.SaveChangesAsync();
        }

        if (!await context.Series.AnyAsync())
        {
            var seriesList = new List<Series>
            {
                new() { Name = "Main Series" },
                new() { Name = "Spin-offs" }
            };
            await context.Series.AddRangeAsync(seriesList);
            await context.SaveChangesAsync();
        }

        // 5. Seed Catalog Games & User Library Entries
        if (!await context.Games.AnyAsync())
        {
            var pc = await context.Platforms.FirstAsync(p => p.Name == "PC");
            var ps5 = await context.Platforms.FirstAsync(p => p.Name == "PlayStation 5");
            var switchPlat = await context.Platforms.FirstAsync(p => p.Name == "Nintendo Switch");
            var deck = await context.Platforms.FirstAsync(p => p.Name == "Steam Deck");

            var steam = await context.DigitalServices.FirstAsync(s => s.Name == "Steam");
            var fromsoft = await context.Developers.FirstAsync(d => d.Name == "FromSoftware");
            var cdpr = await context.Developers.FirstAsync(d => d.Name == "CD Projekt Red");
            var nintendo = await context.Developers.FirstAsync(d => d.Name == "Nintendo");

            var bandai = await context.Publishers.FirstAsync(p => p.Name == "Bandai Namco Entertainment");
            var cdprPub = await context.Publishers.FirstAsync(p => p.Name == "CD Projekt");
            var nintendoPub = await context.Publishers.FirstAsync(p => p.Name == "Nintendo");

            var rpg = await context.Genres.FirstAsync(g => g.Name == "RPG");
            var action = await context.Genres.FirstAsync(g => g.Name == "Action");
            var adventure = await context.Genres.FirstAsync(g => g.Name == "Adventure");

            var fantasy = await context.Themes.FirstAsync(t => t.Name == "Dark Fantasy");
            var cyberpunkTheme = await context.Themes.FirstAsync(t => t.Name == "Cyberpunk");
            var sciFi = await context.Themes.FirstAsync(t => t.Name == "Sci-Fi");

            var sp = await context.Tags.FirstAsync(t => t.Name == "Singleplayer");
            var ow = await context.Tags.FirstAsync(t => t.Name == "Open World");
            var souls = await context.Tags.FirstAsync(t => t.Name == "Souls-like");

            var soulsFranchise = await context.Franchises.FirstAsync(f => f.Name.Contains("Dark Souls"));
            var zeldaFranchise = await context.Franchises.FirstAsync(f => f.Name.Contains("Zelda"));
            var mainSeries = await context.Series.FirstAsync(s => s.Name == "Main Series");

            var eldenRing = new Game
            {
                Title = "Elden Ring",
                Description = "THE NEW FANTASY ACTION RPG. Rise, Tarnished, and be guided by grace to brandish the power of the Elden Ring and become an Elden Lord in the Lands Between.",
                ReleaseDate = new DateTimeOffset(2022, 2, 25, 0, 0, 0, TimeSpan.Zero),
                CommunityRating = 9.5,
                CriticRating = 9.6,
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
                FranchiseId = soulsFranchise.Id,
                SeriesId = mainSeries.Id,
                Platforms = new List<Platform> { pc, ps5 },
                DigitalServices = new List<DigitalService> { steam },
                Developers = new List<Developer> { fromsoft },
                Publishers = new List<Publisher> { bandai },
                Genres = new List<Genre> { rpg, action },
                Themes = new List<Theme> { fantasy },
                Tags = new List<Tag> { sp, ow, souls }
            };

            var zelda = new Game
            {
                Title = "The Legend of Zelda: Breath of the Wild",
                Description = "Forget everything you know about The Legend of Zelda games. Step into a world of discovery, exploration, and adventure in The Legend of Zelda: Breath of the Wild.",
                ReleaseDate = new DateTimeOffset(2017, 3, 3, 0, 0, 0, TimeSpan.Zero),
                CommunityRating = 9.6,
                CriticRating = 9.7,
                EsrbRating = "E10+",
                PegiRating = "12",
                MetacriticScore = 97,
                OpenCriticScore = 96,
                MultiplayerSupport = false,
                ControllerSupport = true,
                SteamDeckCompatibility = "Unsupported",
                FranchiseId = zeldaFranchise.Id,
                SeriesId = mainSeries.Id,
                Platforms = new List<Platform> { switchPlat },
                Developers = new List<Developer> { nintendo },
                Publishers = new List<Publisher> { nintendoPub },
                Genres = new List<Genre> { adventure, action },
                Themes = new List<Theme> { fantasy },
                Tags = new List<Tag> { sp, ow }
            };

            var cyberpunk = new Game
            {
                Title = "Cyberpunk 2077",
                Description = "Cyberpunk 2077 is an open-world, action-adventure RPG set in the megalopolis of Night City, where you play as a cyberpunk mercenary wrapped up in a do-or-die fight for survival.",
                ReleaseDate = new DateTimeOffset(2020, 12, 10, 0, 0, 0, TimeSpan.Zero),
                CommunityRating = 8.5,
                CriticRating = 8.6,
                EsrbRating = "M",
                PegiRating = "18",
                MetacriticScore = 86,
                OpenCriticScore = 82,
                Platforms = new List<Platform> { pc, ps5, deck },
                DigitalServices = new List<DigitalService> { steam },
                Developers = new List<Developer> { cdpr },
                Publishers = new List<Publisher> { cdprPub },
                Genres = new List<Genre> { rpg, action },
                Themes = new List<Theme> { cyberpunkTheme, sciFi },
                Tags = new List<Tag> { sp, ow }
            };

            await context.Games.AddRangeAsync(new[] { eldenRing, zelda, cyberpunk });
            await context.SaveChangesAsync();

            // Seed User Library Entries for admin and user
            if (adminUser != null)
            {
                var adminLibrary = new List<UserLibraryEntry>
                {
                    new()
                    {
                        UserId = adminUser.Id,
                        GameId = eldenRing.Id,
                        OwnGame = true,
                        PurchaseDate = new DateTimeOffset(2022, 2, 25, 0, 0, 0, TimeSpan.Zero),
                        PurchasePrice = 59.99m,
                        Currency = "USD",
                        StorePurchasedFrom = "Steam",
                        HoursPlayed = 94.5,
                        CompletionStatus = CompletionStatus.Completed,
                        PersonalRating = 10.0,
                        PersonalNotes = "Masterpiece from FromSoft.",
                        Platforms = new List<Platform> { pc, ps5 },
                        DigitalServices = new List<DigitalService> { steam }
                    },
                    new()
                    {
                        UserId = adminUser.Id,
                        GameId = zelda.Id,
                        OwnGame = true,
                        PhysicalCopy = true,
                        PurchaseDate = new DateTimeOffset(2017, 3, 3, 0, 0, 0, TimeSpan.Zero),
                        PurchasePrice = 59.99m,
                        Currency = "USD",
                        StorePurchasedFrom = "Amazon",
                        HoursPlayed = 120.0,
                        CompletionStatus = CompletionStatus.Completed,
                        PersonalRating = 9.5,
                        Platforms = new List<Platform> { switchPlat }
                    },
                    new()
                    {
                        UserId = adminUser.Id,
                        GameId = cyberpunk.Id,
                        OwnGame = true,
                        DigitalCopy = true,
                        Backlog = true,
                        PurchaseDate = new DateTimeOffset(2020, 12, 10, 0, 0, 0, TimeSpan.Zero),
                        PurchasePrice = 29.99m,
                        Currency = "USD",
                        StorePurchasedFrom = "Steam",
                        HoursPlayed = 4.5,
                        CompletionStatus = CompletionStatus.OnHold,
                        PersonalRating = 7.0,
                        Platforms = new List<Platform> { pc },
                        DigitalServices = new List<DigitalService> { steam }
                    }
                };

                await context.UserLibraryEntries.AddRangeAsync(adminLibrary);
            }

            if (normalUser != null)
            {
                var userLibrary = new List<UserLibraryEntry>
                {
                    new()
                    {
                        UserId = normalUser.Id,
                        GameId = eldenRing.Id,
                        OwnGame = true,
                        HoursPlayed = 45.0,
                        CompletionStatus = CompletionStatus.Playing,
                        PersonalRating = 9.0,
                        Platforms = new List<Platform> { pc },
                        DigitalServices = new List<DigitalService> { steam }
                    },
                    new()
                    {
                        UserId = normalUser.Id,
                        GameId = cyberpunk.Id,
                        OwnGame = true,
                        HoursPlayed = 12.0,
                        CompletionStatus = CompletionStatus.Playing,
                        PersonalRating = 8.5,
                        Platforms = new List<Platform> { ps5 }
                    }
                };

                await context.UserLibraryEntries.AddRangeAsync(userLibrary);
            }

            await context.SaveChangesAsync();
        }
    }
}
