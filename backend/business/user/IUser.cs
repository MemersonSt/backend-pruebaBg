using backend.dto;

namespace backend.business.user
{
    public interface IUser
    {
        Task<string> Authenticate(AuthDTO auth);
        Task<UserDTO> GetUser(string token);
    }
}
