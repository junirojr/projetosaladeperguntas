using FastEndpoints;
using FluentValidation;

namespace Rocketq.Identity.Endpoints.Register;

public record RegisterRequest(string Username, string Email, string Password);

public class RegisterRequestValidator : Validator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(r => r.Username).NotEmpty().MaximumLength(50);
        RuleFor(r => r.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(r => r.Password).NotEmpty().MinimumLength(8);
    }
}

public record RegisterResponse(Guid Id, string Username, string Email, string Token);