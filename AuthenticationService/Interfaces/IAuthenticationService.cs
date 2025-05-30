using APIG.DTO;

namespace APIG.AuthenticationServiceices.Interfaces;

public interface IAuthenticationService
{
    Task<IResult> LoginAsync(LoginRequest request);
}
