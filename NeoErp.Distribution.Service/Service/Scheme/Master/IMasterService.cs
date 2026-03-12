using NeoErp.Core.Models;
using NeoErp.Distribution.Service.Model.Mobile;
using NeoErp.Distribution.Service.Service.Scheme.models;

namespace NeoErp.Distribution.Service.Service.Scheme.Master
{
    public interface IMasterService
    {

        PreferenceModel FetchPreferences(string comp_code, NeoErpCoreEntity dbContext);
        object FetchItems(RequestModel model, NeoErpCoreEntity dbContext);

        object FetchEntity(RequestModel model, NeoErpCoreEntity dbContext);
        object FetchSchemeUsers(RequestModel model, NeoErpCoreEntity dbContext);
        object FetchSchemes(NeoErpCoreEntity dbContext);
        object CreateOffer(RequestModel request, NeoErpCoreEntity dbContext);
        object LinkKhaltiNumber(LinkKhaltiRequestModel request, NeoErpCoreEntity dbContext);

        object FetchArea(RequestModel model, NeoErpCoreEntity dbContext);
        object GenerateQr(RequestModel model, NeoErpCoreEntity dbContext);
        object UpdateQr(RequestModel model, NeoErpCoreEntity dbContext);
        object GetUnprintedQr(RequestModel model, NeoErpCoreEntity dbContext);
        object UpdateQrPrinted(RequestModel model, NeoErpCoreEntity dbContext);


    }
}
