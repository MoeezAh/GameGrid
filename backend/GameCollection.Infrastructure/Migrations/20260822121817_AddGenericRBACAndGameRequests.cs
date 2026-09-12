using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameCollection.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGenericRBACAndGameRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Backlog",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "CollectorsEdition",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "CompletedDate",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "CompletionStatus",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "DigitalCopy",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Gifted",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "HoursPlayed",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "LastPlayedDate",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "OwnGame",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "PersonalNotes",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "PersonalRating",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "PhysicalCopy",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "PurchaseDate",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "PurchasePrice",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "PurchaseRegion",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "ReceiptReference",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "SpecialEdition",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "StartedPlayingDate",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "StorePurchasedFrom",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Wishlist",
                table: "Games");

            migrationBuilder.CreateTable(
                name: "ApplicationRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsSuperAdmin = table.Column<bool>(type: "bit", nullable: false),
                    IsSystemRole = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApproximateReleaseYear = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Platforms = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Links = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdditionalInformation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReviewNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReviewedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedGameId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameRequests_Games_CreatedGameId",
                        column: x => x.CreatedGameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserLibraryEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    OwnGame = table.Column<bool>(type: "bit", nullable: false),
                    Wishlist = table.Column<bool>(type: "bit", nullable: false),
                    Backlog = table.Column<bool>(type: "bit", nullable: false),
                    PhysicalCopy = table.Column<bool>(type: "bit", nullable: false),
                    DigitalCopy = table.Column<bool>(type: "bit", nullable: false),
                    CollectorsEdition = table.Column<bool>(type: "bit", nullable: false),
                    SpecialEdition = table.Column<bool>(type: "bit", nullable: false),
                    PurchaseDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    PurchasePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StorePurchasedFrom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchaseRegion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiptReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gifted = table.Column<bool>(type: "bit", nullable: false),
                    StartedPlayingDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CompletedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastPlayedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    HoursPlayed = table.Column<double>(type: "float", nullable: false),
                    CompletionStatus = table.Column<int>(type: "int", nullable: false),
                    PersonalRating = table.Column<double>(type: "float", nullable: true),
                    PersonalNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLibraryEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLibraryEntries_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUserRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationUserRoles_ApplicationRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "ApplicationRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationRolePermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationRolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationRolePermissions_ApplicationRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "ApplicationRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationRolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLibraryPlatforms",
                columns: table => new
                {
                    PlatformsId = table.Column<int>(type: "int", nullable: false),
                    UserLibraryEntryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLibraryPlatforms", x => new { x.PlatformsId, x.UserLibraryEntryId });
                    table.ForeignKey(
                        name: "FK_UserLibraryPlatforms_Platforms_PlatformsId",
                        column: x => x.PlatformsId,
                        principalTable: "Platforms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserLibraryPlatforms_UserLibraryEntries_UserLibraryEntryId",
                        column: x => x.UserLibraryEntryId,
                        principalTable: "UserLibraryEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLibraryServices",
                columns: table => new
                {
                    DigitalServicesId = table.Column<int>(type: "int", nullable: false),
                    UserLibraryEntryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLibraryServices", x => new { x.DigitalServicesId, x.UserLibraryEntryId });
                    table.ForeignKey(
                        name: "FK_UserLibraryServices_DigitalServices_DigitalServicesId",
                        column: x => x.DigitalServicesId,
                        principalTable: "DigitalServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserLibraryServices_UserLibraryEntries_UserLibraryEntryId",
                        column: x => x.UserLibraryEntryId,
                        principalTable: "UserLibraryEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationRolePermissions_PermissionId",
                table: "ApplicationRolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationRolePermissions_RoleId",
                table: "ApplicationRolePermissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserRoles_RoleId",
                table: "ApplicationUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserRoles_UserId_RoleId",
                table: "ApplicationUserRoles",
                columns: new[] { "UserId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameRequests_CreatedGameId",
                table: "GameRequests",
                column: "CreatedGameId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLibraryEntries_GameId",
                table: "UserLibraryEntries",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLibraryPlatforms_UserLibraryEntryId",
                table: "UserLibraryPlatforms",
                column: "UserLibraryEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLibraryServices_UserLibraryEntryId",
                table: "UserLibraryServices",
                column: "UserLibraryEntryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationRolePermissions");

            migrationBuilder.DropTable(
                name: "ApplicationUserRoles");

            migrationBuilder.DropTable(
                name: "GameRequests");

            migrationBuilder.DropTable(
                name: "UserLibraryPlatforms");

            migrationBuilder.DropTable(
                name: "UserLibraryServices");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "ApplicationRoles");

            migrationBuilder.DropTable(
                name: "UserLibraryEntries");

            migrationBuilder.AddColumn<bool>(
                name: "Backlog",
                table: "Games",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CollectorsEdition",
                table: "Games",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CompletedDate",
                table: "Games",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompletionStatus",
                table: "Games",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Games",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DigitalCopy",
                table: "Games",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Gifted",
                table: "Games",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "HoursPlayed",
                table: "Games",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastPlayedDate",
                table: "Games",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "OwnGame",
                table: "Games",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PersonalNotes",
                table: "Games",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PersonalRating",
                table: "Games",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhysicalCopy",
                table: "Games",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PurchaseDate",
                table: "Games",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PurchasePrice",
                table: "Games",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PurchaseRegion",
                table: "Games",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiptReference",
                table: "Games",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SpecialEdition",
                table: "Games",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "StartedPlayingDate",
                table: "Games",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StorePurchasedFrom",
                table: "Games",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Games",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Wishlist",
                table: "Games",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
