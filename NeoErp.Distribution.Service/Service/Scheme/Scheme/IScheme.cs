using NeoErp.Core.Models;
using NeoErp.Distribution.Service.Service.Scheme.models;

namespace NeoErp.Distribution.Service.Service.Scheme.Scheme
{
    public interface IScheme
    {

        object ClaimOffer(RequestModel request, NeoErpCoreEntity dbContext);
        object CreateOffer(RequestModel request, NeoErpCoreEntity dbContext);
        //object FetchOffer(RequestModel request, NeoErpCoreEntity dbContext);

    }
}
