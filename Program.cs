using APIG.AuthenticationServiceices;
using APIG.AuthenticationServiceices.Interfaces;
using APIG.DTO;

var builder = WebApplication.CreateBuilder(args);

// Autenticación JWT
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "http://localhost:8080/realms/realm-adduser";
        options.RequireHttpsMetadata = false;
        options.Audience = "adduser-client";
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Servicios de autenticación y registro desacoplados
builder.Services.AddHttpClient<IAuthenticationService, AuthenticationService>();
builder.Services.AddHttpClient<IUserRegistrationService, UserRegistrationService>();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/hola", () => Results.Ok("Hola bebe"));

app.MapPost("/auth/login", async (LoginRequest request, IAuthenticationService authService) =>
    await authService.LoginAsync(request)
).AllowAnonymous();

app.MapPost("/register-user", async (CreateUserRequest user, IUserRegistrationService userService) =>
    await userService.RegisterUserAsync(user)
).AllowAnonymous();

//app.MapReverseProxy().RequireAuthorization();

app.MapReverseProxy();

app.Run();

