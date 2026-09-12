using FluentValidation;

namespace GameCollection.Application.Features.Games.Commands.CreateGame;

public class CreateGameCommandValidator : AbstractValidator<CreateGameCommand>
{
    public CreateGameCommandValidator()
    {
        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(v => v.CommunityRating)
            .InclusiveBetween(0, 10).When(v => v.CommunityRating.HasValue)
            .WithMessage("Community rating must be between 0 and 10.");

        RuleFor(v => v.CriticRating)
            .InclusiveBetween(0, 10).When(v => v.CriticRating.HasValue)
            .WithMessage("Critic rating must be between 0 and 10.");

        RuleFor(v => v.MetacriticScore)
            .InclusiveBetween(0, 100).When(v => v.MetacriticScore.HasValue)
            .WithMessage("Metacritic score must be between 0 and 100.");

        RuleFor(v => v.OpenCriticScore)
            .InclusiveBetween(0, 100).When(v => v.OpenCriticScore.HasValue)
            .WithMessage("OpenCritic score must be between 0 and 100.");
    }
}
