using Product.Application.Interfaces.Services;

namespace Product.Infrastructure.Services
{
    public class BaseService
    {
        internal readonly IAuthenticatedUserService _authenticatedUserService;
        public BaseService(IAuthenticatedUserService authenticatedUserService)
        {
            _authenticatedUserService = authenticatedUserService;
        }


        internal int GetUserId()
        {
            if (_authenticatedUserService.UserId == 0) throw new UnauthorizedAccessException($"Access Denied.");
            return _authenticatedUserService.UserId;
        }
    }
}
