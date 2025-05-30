using APIG.DTO;

namespace APIG.AuthenticationServiceices.Interfaces;

public interface IUserRegistrationService
{
    Task<IResult> RegisterUserAsync(CreateUserRequest user);
}
