

using NeoERP.DocumentTemplate.Service.Models.ThirdPartyApi;

namespace NeoERP.DocumentTemplate.Service.Interface.ThirdPartyApi
{
    public interface IShymphonyService
    {


        //void InsertInvoiceRecord(Shymphony Record, string formCode);
        void InsertInvoiceRecord(Shymphony Record);
        void ifIsRealTimeFalse(Shymphony Record);
    }
}
