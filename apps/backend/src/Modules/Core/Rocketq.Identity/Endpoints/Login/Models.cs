using FastEndpoints;
using FluentValidation;

namespace Rocketq.Identity.Endpoints.Login;

public record LoginRequest(string Email, string Password);

public class LoginRequestValidator : Validator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(r => r.Email).NotEmpty().EmailAddress();
        RuleFor(r => r.Password).NotEmpty();
    }
}

public record LoginResponse(Guid Id, string Username, string Email, string Token);
