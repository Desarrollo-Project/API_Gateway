using System.Text;
using System.Net.Http;
using APIG.DTO;
using APIG.AuthenticationServiceices.Interfaces;

namespace APIG.AuthenticationServiceices;

public class AuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;

    public AuthenticationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IResult> LoginAsync(LoginRequest request)
    {
        var formData = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", "adduser-client"),
            new KeyValuePair<string, string>("client_secret", "8dm7NVYDcQw4jGHnx7n7souieX0Y6IOV"),
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("username", request.Username),
            new KeyValuePair<string, string>("password", request.Password),
        });

        var response = await _httpClient.PostAsync(
            "http://localhost:8080/realms/realm-adduser/protocol/openid-connect/token", formData);

        var content = await response.Content.ReadAsStringAsync();
        return response.IsSuccessStatusCode ? Results.Ok(content) : Results.BadRequest(content);
    }
}