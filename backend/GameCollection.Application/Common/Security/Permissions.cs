using System.Collections.Generic;

namespace GameCollection.Application.Common.Security;

public static class Permissions
{
    public static class Games
    {
        public const string View = "Games.View";
        public const string Create = "Games.Create";
        public const string Update = "Games.Update";
        public const string Delete = "Games.Delete";
        public const string ManageMetadata = "Games.ManageMetadata";
    }

    public static class Libraries
    {
        public const string ViewOwn = "Libraries.ViewOwn";
        public const string AddGame = "Libraries.AddGame";
        public const string UpdateOwn = "Libraries.UpdateOwn";
        public const string RemoveGame = "Libraries.RemoveGame";
        public const string ViewAny = "Libraries.ViewAny";
        public const string UpdateAny = "Libraries.UpdateAny";
    }

    public static class GameRequests
    {
        public const string Create = "GameRequests.Create";
        public const string ViewOwn = "GameRequests.ViewOwn";
        public const string ViewAny = "GameRequests.ViewAny";
        public const string Approve = "GameRequests.Approve";
        public const string Reject = "GameRequests.Reject";
        public const string Edit = "GameRequests.Edit";
        public const string Delete = "GameRequests.Delete";
    }

    public static class Users
    {
        public const string View = "Users.View";
        public const string Create = "Users.Create";
        public const string Update = "Users.Update";
        public const string Delete = "Users.Delete";
        public const string AssignRoles = "Users.AssignRoles";
    }

    public static class Roles
    {
        public const string View = "Roles.View";
        public const string Create = "Roles.Create";
        public const string Update = "Roles.Update";
        public const string Delete = "Roles.Delete";
        public const string AssignPermissions = "Roles.AssignPermissions";
    }

    public static class Metadata
    {
        public const string View = "Metadata.View";
        public const string ManagePlatforms = "Metadata.ManagePlatforms";
        public const string ManageServices = "Metadata.ManageServices";
        public const string ManageDevelopers = "Metadata.ManageDevelopers";
        public const string ManagePublishers = "Metadata.ManagePublishers";
        public const string ManageTaxonomies = "Metadata.ManageTaxonomies";
    }

    public record PermissionDefinition(string Name, string DisplayName, string Description, string Category);

    public static readonly IReadOnlyList<PermissionDefinition> All = new List<PermissionDefinition>
    {
        // Games
        new(Games.View, "View Games", "View and search games in the central catalog", "Games"),
        new(Games.Create, "Add Game to Catalog", "Create new game entries in the central catalog", "Games"),
        new(Games.Update, "Edit Catalog Game", "Modify game details in the central catalog", "Games"),
        new(Games.Delete, "Delete Catalog Game", "Remove games from the central catalog", "Games"),
        new(Games.ManageMetadata, "Manage Game Metadata", "Assign developers, publishers, platforms and taxonomies to catalog games", "Games"),

        // Libraries
        new(Libraries.ViewOwn, "View Own Library", "View personal game library collection", "Libraries"),
        new(Libraries.AddGame, "Add Game to Own Library", "Add catalog games to personal library with owned platforms", "Libraries"),
        new(Libraries.UpdateOwn, "Update Own Library", "Update playtime, ratings, status and personal notes for library games", "Libraries"),
        new(Libraries.RemoveGame, "Remove from Own Library", "Remove games from personal library", "Libraries"),
        new(Libraries.ViewAny, "View Any User Library", "View other users' game libraries", "Libraries"),
        new(Libraries.UpdateAny, "Modify Any User Library", "Modify another user's library entries", "Libraries"),

        // Game Requests
        new(GameRequests.Create, "Submit Game Request", "Request new games to be added to the catalog", "GameRequests"),
        new(GameRequests.ViewOwn, "View Own Requests", "View status of submitted game requests", "GameRequests"),
        new(GameRequests.ViewAny, "View All Requests", "View all submitted game requests across the system", "GameRequests"),
        new(GameRequests.Approve, "Approve Game Request", "Review and approve game requests into catalog", "GameRequests"),
        new(GameRequests.Reject, "Reject Game Request", "Reject game requests with feedback notes", "GameRequests"),
        new(GameRequests.Edit, "Edit Game Request", "Edit pending game request details", "GameRequests"),
        new(GameRequests.Delete, "Delete Game Request", "Cancel or remove game requests", "GameRequests"),

        // Users
        new(Users.View, "View Users", "View registered user list and profiles", "Users"),
        new(Users.Create, "Add User", "Create new user accounts directly", "Users"),
        new(Users.Update, "Edit User", "Modify user profile information and status", "Users"),
        new(Users.Delete, "Delete User", "Remove or deactivate user accounts", "Users"),
        new(Users.AssignRoles, "Assign User Roles", "Assign and remove roles from users", "Users"),

        // Roles (Super Admin exclusive operations)
        new(Roles.View, "View Roles", "View system roles and their assigned permissions", "Roles"),
        new(Roles.Create, "Create Role", "Create new custom roles with configurable permissions", "Roles"),
        new(Roles.Update, "Edit Role", "Modify role names, descriptions and statuses", "Roles"),
        new(Roles.Delete, "Delete Role", "Delete custom roles from the system", "Roles"),
        new(Roles.AssignPermissions, "Assign Role Permissions", "Assign or revoke permissions for roles", "Roles"),

        // Metadata
        new(Metadata.View, "View Metadata", "Browse platforms, services, developers, and taxonomies", "Metadata"),
        new(Metadata.ManagePlatforms, "Manage Platforms", "Create, edit, and delete gaming platforms", "Metadata"),
        new(Metadata.ManageServices, "Manage Services", "Create, edit, and delete digital storefront services", "Metadata"),
        new(Metadata.ManageDevelopers, "Manage Developers", "Create, edit, and delete game development studios", "Metadata"),
        new(Metadata.ManagePublishers, "Manage Publishers", "Create, edit, and delete game publishers", "Metadata"),
        new(Metadata.ManageTaxonomies, "Manage Taxonomies", "Create, edit, and delete genres, tags, themes, series, and franchises", "Metadata")
    };
}
