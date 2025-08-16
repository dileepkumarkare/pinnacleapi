using Pinnacle.Entities;

namespace Pinnacle.IServices
{
    public interface IAuthenticationService
    {
        Task<Ret> Login(LoginEntity entity);
    }
}
