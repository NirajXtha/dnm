using NeoErp.Core.Models;
using NeoErp.Distribution.Service.Service.Scheme.models;
using System.Web;

namespace NeoErp.Distribution.Service.Service.Scheme.authentication
{
    public interface IAuthenticationService
    {
        object Register(RegisterRequestModel model, HttpFileCollection files, NeoErpCoreEntity dbContext);

        object Login(LoginRequestModel model, NeoErpCoreEntity dbContext);

    }
}
