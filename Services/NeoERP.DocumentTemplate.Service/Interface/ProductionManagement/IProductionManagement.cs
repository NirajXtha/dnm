using NeoERP.DocumentTemplate.Service.Models.ProductionManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Interface.ProductionManagement
{
    public interface IProductionManagement
    {
        List<ProductionPlan> GetAllProductionPlanningList(ProductionPlanningFilterParamsModel requestParms);
        Task<List<PMProductModelTreeModel>> GetProductTreeStructureListAsync();
        Task<List<ProductItemModel>> GetParticularProductListAsync(string item_code);
        ProductItemDetailModel GetProductItemDetailsAsync(string item_code);
        decimal GetNewPlanCode();
        List<RawMaterialItemsModel> GetRowPackingMaterialAfterVerianceInfoInsert(decimal? planNo = null);
        // Task<object> PrepareRowMaterialBasedOnCalcAndInsertAsync(string item_code, decimal requestedQty, string plan_no = "", List<VarianceInfoModel> varianceInfoModelList = null);
        int InsertOrderPlanProcess(OrderPlanProcess model);
        Task<object> GetOrderListOfSelectedItemCodeOrOrderNo(string item_code, string order_no = "");
        Task<object> GetProductionPipeDataList(string item_code, string planNoToExclude, string order_no = "");
        Task<object> GetResourceDataList();
        Task<ProductionPlan> GetPlanDetailsForEdit(string plan_no);
        int UpdateOrderPlanProcess(OrderPlanProcess planObj);
        Task<List<ProductItemModel>> GetParticularProductListUsingSearchTextAsync(string searchText);
        List<ProductionOrderDataModel> GetProductionOrderList(string searchText);
        Task<List<BatchTransactionResult>> GetBatchTransectionInfo(string item_code, string order_no, string plan_no);
        Task<List<OrderDetail>> GetOrderListForEditData(string planNo);
        object DeleteVarianceInfo(string planNo);
        object PrepareRowMaterialBasedOnCalcAndInsertAsync(List<ItemAndQty> itemWithQtyList, decimal requestedQty, string plan_no = "", List<VarianceInfoModel> varianceInfoModelList = null);
        List<PlanItemModel> GetProductionPlanningListWhileInputProcess(string searchText);
        object GetPlanSearchWithQueryWhileIssueItemForPPlan(string searchText, int page, int pageSize);
        List<PlanUsedInInputProcessModel> GetPlanDtlList();
        object GenerateRequisition(GenerateRequisitionRequest request);

        string InsertProductionPreference(ProductionPreference prefObj);
        List<FormSetupDto> GetFormMappingList();
        ProductionPreference GetProductionPreferences();
        List<FormSetupDto> GetIndentFormMappingList();
    }
}
