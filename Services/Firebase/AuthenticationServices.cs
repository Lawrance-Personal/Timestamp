using System;
using FirebaseAdmin.Auth;
using Timestamp_Backend.Services.Token;

namespace Timestamp_Backend.Services.Firebase;

public interface IAuthenticationServices
{
    public Task<string?> Register(string email, string password);
    public Task<AuthToken?> Login(string email, string password);
    public Task<AuthToken?> RefreshToken(string refreshToken);
    public void UpdateEmail(string identityId, string email);
    public void UpdatePassword(string identityId, string password);
    public void Unregister(string identityId);
}

public class AuthenticationServices(ITokenServices tokenService, IRefreshTokenServices refreshTokenService) : IAuthenticationServices
{
    private readonly ITokenServices _tokenService = tokenService;
    private readonly IRefreshTokenServices _refreshTokenService = refreshTokenService;
    public async Task<string?> Register(string email, string password)
    {
        UserRecord? user = null;
        try
        {
            user = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);
        }
        catch (Exception) { };
        if(user != null) return null;
        var newUser = await FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs
        {
            Email = email,
            Password = password
        });
        return newUser.Uid;
    }

    public async Task<AuthToken?> Login(string email, string password)
    {
        return await _tokenService.GenerateToken(email, password);
    }

    public async Task<AuthToken?> RefreshToken(string refreshToken)
    {
        return await _refreshTokenService.RefreshToken(refreshToken);
    }

    public async void UpdateEmail(string identityId, string email)
    {
        await FirebaseAuth.DefaultInstance.UpdateUserAsync(new UserRecordArgs
        {
            Uid = identityId,
            Email = email
        });
    }
    
    public async void UpdatePassword(string identityId, string password)
    {
        await FirebaseAuth.DefaultInstance.UpdateUserAsync(new UserRecordArgs
        {
            Uid = identityId,
            Password = password
        });
    }

    public async void Unregister(string identityId)
    {
        await FirebaseAuth.DefaultInstance.DeleteUserAsync(identityId);
    }
}
