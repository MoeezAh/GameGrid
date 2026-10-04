# Application Layer

tags: #backend #application #cqrs #mediator #dtos

## Overview

The Application layer (`GameCollection.Application`) contains all **business orchestration logic** — CQRS handlers, DTOs, validation, and AutoMapper profiles. It references only the Domain layer.

---

## CQRS with MediatR

Every API operation dispatches a **Command** (write) or **Query** (read) through `IMediator.Send()`.

### Feature Folders

```
Features/
├── Games/
│   ├── Commands/
│   │   ├── CreateGameCommand.cs + Handler
│   │   ├── UpdateGameCommand.cs + Handler
│   │   └── DeleteGameCommand.cs + Handler
│   └── Queries/
│       ├── GetGamesQuery.cs + Handler       ← paginated, filtered
│       └── GetGameByIdQuery.cs + Handler
├── Libraries/
│   ├── Commands/
│   │   ├── AddToLibraryCommand.cs + Handler
│   │   ├── UpdateLibraryEntryCommand.cs + Handler
│   │   └── RemoveFromLibraryCommand.cs + Handler
│   └── Queries/
│       ├── GetMyLibraryQuery.cs + Handler
│       └── GetLibraryEntryByIdQuery.cs + Handler
├── GameRequests/
│   ├── Commands/
│   │   ├── SubmitGameRequestCommand.cs + Handler
│   │   ├── ApproveGameRequestCommand.cs + Handler
│   │   └── RejectGameRequestCommand.cs + Handler
│   └── Queries/
│       ├── GetMyGameRequestsQuery.cs + Handler
│       └── GetAllGameRequestsQuery.cs + Handler
└── Dashboard/
    └── Queries/
        └── GetDashboardStatsQuery.cs + Handler
```

### Example Command

```csharp
public class CreateGameCommand : IRequest<int>
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public List<int> PlatformIds { get; set; }
    public List<int> GenreIds { get; set; }
    // ... more fields
}

public class CreateGameCommandHandler : IRequestHandler<CreateGameCommand, int>
{
    public async Task<int> Handle(CreateGameCommand request, CancellationToken ct)
    {
        var game = _mapper.Map<Game>(request);
        // attach navigation entities by ID
        await _repository.AddAsync(game);
        await _unitOfWork.SaveChangesAsync(ct);
        return game.Id;
    }
}
```

---

## FluentValidation

Each Command/Query that requires validation has a corresponding `Validator` class registered as a MediatR pipeline behavior.

```csharp
public class CreateGameCommandValidator : AbstractValidator<CreateGameCommand>
{
    public CreateGameCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200);
        
        RuleFor(x => x.PlatformIds)
            .NotEmpty().WithMessage("At least one platform is required.");
    }
}
```

Validation errors are caught by `ApiExceptionMiddleware` and returned as RFC 7807 `ProblemDetails` with a `400 Bad Request`.

---

## DTOs (Data Transfer Objects)

```
DTOs/
├── Auth/
│   ├── LoginRequest.cs
│   ├── RegisterRequest.cs
│   └── AuthResponse.cs          ← { token, username, email, roles[], permissions[], isSuperAdmin }
├── Game/
│   ├── GameDto.cs               ← summary card data
│   ├── GameDetailDto.cs         ← full detail view
│   └── CreateGameRequest.cs
├── Library/
│   ├── LibraryEntryDto.cs
│   └── AddToLibraryRequest.cs
├── GameRequest/
│   ├── GameRequestDto.cs
│   └── SubmitGameRequestRequest.cs
├── Dashboard/
│   └── DashboardStatsDto.cs     ← KPI counts + chart datasets
├── Metadata/
│   └── MetadataItemDto.cs       ← generic { id, name, description }
└── RBAC/
    ├── RoleDto.cs
    ├── PermissionDto.cs
    └── AssignRolesRequest.cs
```

---

## AutoMapper Profiles

Located in `Mappings/`, profiles map between domain entities and DTOs:

```csharp
public class GameMappingProfile : Profile
{
    public GameMappingProfile()
    {
        CreateMap<Game, GameDto>()
            .ForMember(d => d.Platforms, opt => opt.MapFrom(s => s.Platforms.Select(p => p.Name)));
        
        CreateMap<Game, GameDetailDto>();
        
        CreateMap<CreateGameCommand, Game>();
    }
}
```

---

## Common Interfaces (Application-Defined Contracts)

These interfaces are defined in Application and implemented in Infrastructure — following the Dependency Inversion Principle:

| Interface | Implemented By | Purpose |
|:---|:---|:---|
| `IIdentityService` | `IdentityService` | Register, Login, ChangePassword, get user profile |
| `IPermissionService` | `PermissionService` | HasPermissionAsync, IsSuperAdminAsync |
| `ICurrentUserService` | `CurrentUserService` | Get authenticated user's ID from HttpContext |
| `IFileStorageService` | `LocalFileStorageService` | Save uploaded files, return relative URL |

---

## DI Registration

```csharp
// GameCollection.Application/ConfigureServices.cs
public static IServiceCollection AddApplicationServices(this IServiceCollection services)
{
    services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    services.AddAutoMapper(Assembly.GetExecutingAssembly());
    
    // Register FluentValidation pipeline behavior
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    
    return services;
}
```

---

## Navigation

← [[Backend/Domain Layer]] | [[Backend/Infrastructure Layer]] →
