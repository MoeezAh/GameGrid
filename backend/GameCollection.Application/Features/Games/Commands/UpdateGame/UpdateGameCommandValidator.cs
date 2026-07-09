using FluentValidation;

namespace GameCollection.Application.Features.Games.Commands.UpdateGame;

public class UpdateGameCommandValidator : AbstractValidator<UpdateGameCommand>
{
    public UpdateGameCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Game ID is required.");

        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(v => v.HoursPlayed)
            .GreaterThanOrEqualTo(0).WithMessage("Hours played must be a non-negative number.");

        RuleFor(v => v.PersonalRating)
            .InclusiveBetween(0, 10).When(v => v.PersonalRating.HasValue)
            .WithMessage("Personal rating must be between 0 and 10.");

        RuleFor(v => v.MetacriticScore)
            .InclusiveBetween(0, 100).When(v => v.MetacriticScore.HasValue)
            .WithMessage("Metacritic score must be between 0 and 100.");

        RuleFor(v => v.OpenCriticScore)
            .InclusiveBetween(0, 100).When(v => v.OpenCriticScore.HasValue)
            .WithMessage("OpenCritic score must be between 0 and 100.");
    }
}
