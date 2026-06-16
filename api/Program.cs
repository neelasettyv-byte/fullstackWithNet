using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// --- Configuration ---
// In real apps, never hard-code this. Use environment variables / Azure Key Vault.
var jwtKey = builder.Configuration["JWT_KEY"] ?? "this-is-a-demo-secret-key-change-me-12345678";
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

// --- Services ---
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true
        };
    });
builder.Services.AddAuthorization();

// Allow the React frontend to call this API
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// --- A simple in-memory "user store" (no database needed for practice) ---
var users = new Dictionary<string, string>
{
    { "admin", "password123" },   // username : password
    { "quest", "cricket" }
};

// --- Public endpoint: health check ---
app.MapGet("/api/health", () => Results.Ok(new { status = "healthy", time = DateTime.UtcNow }));

// --- Public endpoint: login ---
app.MapPost("/api/login", (LoginRequest req) =>
{
    if (!users.TryGetValue(req.Username, out var pwd) || pwd != req.Password)
        return Results.Unauthorized();

    // Build a JWT token
    var claims = new[] { new Claim(ClaimTypes.Name, req.Username) };
    var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);
    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: creds);

    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
    return Results.Ok(new { token = tokenString, username = req.Username });
});

// --- Protected endpoint: only works with a valid token ---
app.MapGet("/api/secret", [Authorize] (ClaimsPrincipal user) =>
{
    var name = user.Identity?.Name ?? "unknown";
    return Results.Ok(new { message = $"Hello {name}, this is protected data only logged-in users can see!" });
});

app.Run();

// Request model
record LoginRequest(string Username, string Password);
