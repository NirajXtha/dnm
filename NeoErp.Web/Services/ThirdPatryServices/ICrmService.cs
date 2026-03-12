using NeoErp.Models.ThirdPartyModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Services.ThirdPatryServices
{
    public interface ICrmService
    {
        List<ItemModel> GetItemList();
        List<CategoryModel> GetCategoryList();
        List<BranchModel> GetBranchList();
        List<UnitModel> GetUnitList();
        List<StockModel> GetStockList();
        string SaveSalesOrder(SalesOrderModel model);
        List<SalesInvoiceModel> FetchSODetails(string SalesNumber, string TransNo);

    }
}
