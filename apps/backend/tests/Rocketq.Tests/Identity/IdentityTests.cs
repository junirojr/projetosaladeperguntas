using System.Net;
using System.Net.Http.Headers;
using FastEndpoints.Testing;
using Rocketq.Identity.Endpoints.GetProfile;
using Rocketq.Identity.Endpoints.Login;
using Rocketq.Identity.Endpoints.Register;
using Xunit;
using RegisterEndpoint = Rocketq.Identity.Endpoints.Register.Endpoint;
using LoginEndpoint = Rocketq.Identity.Endpoints.Login.Endpoint;
using GetProfileEndpoint = Rocketq.Identity.Endpoints.GetProfile.Endpoint;

namespace Rocketq.Tests.Identity;

[Collection(nameof(AppCollection))]
public class IdentityTests(AppFactory app) : TestBase<AppFactory>(app)
{
    [Fact]
    public async Task Register_WithValidData_Returns201WithToken()
    {
        var (rsp, body) = await App.Client.POSTAsync<
            RegisterEndpoint, RegisterRequest, RegisterResponse>(
            new RegisterRequest("alice", $"alice_{Guid.NewGuid():N}@test.com", "senha1234"));

        Assert.Equal(HttpStatusCode.Created, rsp.StatusCode);
        Assert.NotEmpty(body!.Token);
        Assert.Equal("alice", body.Username);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_Returns400()
    {
        string email = $"bob_{Guid.NewGuid():N}@test.com";

        await App.Client.POSTAsync<RegisterEndpoint, RegisterRequest, RegisterResponse>(
            new RegisterRequest("bob", email, "senha1234"));

        var (rsp, _) = await App.Client.POSTAsync<RegisterEndpoint, RegisterRequest, RegisterResponse>(
            new RegisterRequest("bob2", email, "outrasenha"));

        Assert.Equal(HttpStatusCode.BadRequest, rsp.StatusCode);
    }

    [Fact]
    public async Task Login_WithCorrectCredentials_ReturnsToken()
    {
        string email = $"carol_{Guid.NewGuid():N}@test.com";
        await App.Client.POSTAsync<RegisterEndpoint, RegisterRequest, RegisterResponse>(
            new RegisterRequest("carol", email, "senha1234"));

        var (rsp, body) = await App.Client.POSTAsync<
            LoginEndpoint, LoginRequest, LoginResponse>(
            new LoginRequest(email, "senha1234"));

        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
        Assert.NotEmpty(body!.Token);
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns401()
    {
        string email = $"dave_{Guid.NewGuid():N}@test.com";
        await App.Client.POSTAsync<RegisterEndpoint, RegisterRequest, RegisterResponse>(
            new RegisterRequest("dave", email, "senha1234"));

        var (rsp, _) = await App.Client.POSTAsync<
            LoginEndpoint, LoginRequest, LoginResponse>(
            new LoginRequest(email, "senhaerrada"));

        Assert.Equal(HttpStatusCode.Unauthorized, rsp.StatusCode);
    }

    [Fact]
    public async Task GetProfile_WithoutToken_Returns401()
    {
        var anonClient = App.CreateClient();
        var rsp = await anonClient.GetAsync("/users/me");

        Assert.Equal(HttpStatusCode.Unauthorized, rsp.StatusCode);
    }

    [Fact]
    public async Task GetProfile_WithValidToken_ReturnsUserData()
    {
        string email = $"eve_{Guid.NewGuid():N}@test.com";
        var (_, registerBody) = await App.Client.POSTAsync<
            RegisterEndpoint, RegisterRequest, RegisterResponse>(
            new RegisterRequest("eve", email, "senha1234"));

        var authClient = App.CreateClient(c =>
            c.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", registerBody!.Token));

        var (rsp, body) = await authClient.GETAsync<GetProfileEndpoint, GetProfileResponse>();

        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
        Assert.Equal("eve", body!.Username);
        Assert.Equal(email, body.Email);
    }
}