using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Timestamp_Backend.Configurations;
using Timestamp_Backend.Services.Firebase;
using Timestamp_Backend.Services.MongoDB;
using Timestamp_Backend.Services.Token;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection(nameof(MongoDBSettings)));
builder.Services.AddSingleton<MongoDBServices>(serviceProvider => 
{
    var settings = serviceProvider.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    return new MongoDBServices(settings.ConnectionString, settings.DatabaseName);
});

builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection(nameof(TokenSettings)));
FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile("firebase.json")
});

builder.Services.AddSingleton<IAuthenticationServices, AuthenticationServices>();

builder.Services.AddHttpClient<ITokenServices, TokenServices>((serviceProvider, httpClient) => 
{
    var settings = serviceProvider.GetRequiredService<IOptions<TokenSettings>>().Value;
    httpClient.BaseAddress = new Uri(settings.TokenUri);
});
builder.Services.AddHttpClient<IRefreshTokenServices, RefreshTokenServices>((serviceProvider, httpClient) =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<TokenSettings>>().Value;
    httpClient.BaseAddress = new Uri(settings.RefreshTokenUri);
});

builder.Services.AddAuthentication().AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwtOptions => ValidateTokenServices.ConfigureJwtOptions(jwtOptions, builder.Configuration));

builder.Services.AddControllers();
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
// app.UseHttpsRedirection();

app.Run();
