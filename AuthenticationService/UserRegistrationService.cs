using System.Text;
using System.Text.Json;
using APIG.DTO;
using APIG.AuthenticationServiceices.Interfaces;

namespace APIG.AuthenticationServiceices;

public class UserRegistrationService : IUserRegistrationService
{
    private readonly HttpClient _httpClient;

    public UserRegistrationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IResult> RegisterUserAsync(CreateUserRequest user)
    {
        var formData = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", "adduser-client"),
            new KeyValuePair<string, string>("client_secret", "8dm7NVYDcQw4jGHnx7n7souieX0Y6IOV"),
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
        });

        var tokenResponse = await _httpClient.PostAsync(
            "http://localhost:8080/realms/realm-adduser/protocol/openid-connect/token", formData);

        if (!tokenResponse.IsSuccessStatusCode)
        {
            var errorText = await tokenResponse.Content.ReadAsStringAsync();
            return Results.BadRequest($"Error al obtener el token: {errorText}");
        }

        var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
        var tokenData = JsonSerializer.Deserialize<JsonElement>(tokenJson);
        var accessToken = tokenData.GetProperty("access_token").GetString();

        var newUser = new
        {
            username = user.Username,
            email = user.Email,
            firstName = user.FirstName,
            lastName = user.LastName,
            enabled = true,
            credentials = new[]
            {
                new { type = "password", value = user.Password, temporary = false }
            }
        };

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(newUser), 
            Encoding.UTF8, 
            "application/json");


        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var userResponse = await _httpClient.PostAsync(
            "http://localhost:8080/admin/realms/realm-adduser/users", jsonContent);

        if (!userResponse.IsSuccessStatusCode)
        {
            var errorText = await userResponse.Content.ReadAsStringAsync();
            return Results.BadRequest($"Error al registrar usuario: {errorText}");
        }

        return Results.Ok("Usuario registrado exitosamente.");
    }
}