using FastEndpoints;
using FastEndpoints.Security;
using Rocketq.Identity;
using Rocketq.Rooms;
using Rocketq.Feed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpoints();

builder.Services
    .AddAuthenticationJwtBearer(s =>
    {
        s.SigningKey = builder.Configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("Jwt:Secret not configured.");
    })
    .AddAuthorization();

// Módulos
builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddRoomsModule(builder.Configuration);
builder.Services.AddFeedModule(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? throw new InvalidOperationException("Cors:AllowedOrigins not configured.");
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();

app.Run();

// Needed for WebApplicationFactory in tests
public partial class Program { }
