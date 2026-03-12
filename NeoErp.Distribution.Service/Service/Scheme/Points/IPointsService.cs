using NeoErp.Core.Models;
using NeoErp.Distribution.Service.Service.Scheme.models;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Service.Scheme.Points
{
    public interface IPointsService
    {
        Task<object> FetchPointsHistory(RequestModel request, NeoErpCoreEntity dbContext);
        Task<object> FetchAllPointsHistory(RequestModel request, NeoErpCoreEntity dbContext);
        Task<object> FetchRedeemHistory(RequestModel request, NeoErpCoreEntity dbContext);
        object ClaimPoints(RequestModel request, NeoErpCoreEntity dbContext);
        object RedeemPoints(RequestModel request, NeoErpCoreEntity dbContext);
        object LoadKhalti(LoadKhaltiRequestModel request, NeoErpCoreEntity dbContext);
    }
}
