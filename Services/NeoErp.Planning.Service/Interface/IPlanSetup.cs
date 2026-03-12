using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoErp.Core.Models;
using NeoErp.Planning.Service.Models;
using NeoErp.Planning.Service.Repository;

namespace NeoErp.Planning.Service.Interface
{
    public interface IPlanSetup
    {
        List<ItemModel> getItemByFilter(string filter);
        List<ItemModel> getItem();
        List<PlanModels> getPlanList(string filter, string startDate, string endDate);
        List<MasterSalesPlan> getAllMasterPlanNames(string filter,string startDate, string endDate);
        List<PlanSetupTitleModel> getTitleValues(string planCode);
        List<freqNameVlaue> getFrequency(string filter);
        List<ItemModel> getItemByCode(string planCode,string itemCode);
        List<MyColumnSettings> GetFrequencyTitle(string planCode);
        string SavePlan(List<savePlan> obj,string planCode,string time_frame_code);
        string SavePlan(List<savePlan> obj, SalesPlan sp);
        List<PlanModels> GetPlanDetailVale(string plancode);
        List<MyColumnSettings> GetFrequencyTitle(string startDate, string endDate,string timeFrameCode, string timeFrameName, string datetype="BS");
        SalesPlan GetPlanDetailValueByPlanCode(int plancode);
        SalesPlan GetSalesPlanDetailValueByPlanCode(int plancode);
        SalesPlan GetMasterPlanDetailValueByMasterPlanCode(int mp_code);
        List<PlanSalesRefrenceModel> getSalesItemDataForRefrences(SalesItemForReferenceModel model);
        List<PlanSalesRefrenceModel> getSalesItemDataForRefrence(string itemList, string startDate, string endDate, string customerCode, string divisionCode, string branchCode, string dateFormat, string frequency, string FiscalYear, string salesFlag);
        LcPreferenceSetup GetItemGoupEntryPreferenceSetup();
        List<PreferenceSetupModel> GetPreferenceSetups();

        PreferenceSetupModel GetPreferenceSetup(string pl_name);
        int SavePreferenceSetup(PreferenceSetupModel model);

        int SaveAllPreferenceSetup();
        string SavePlanFromExcell(List<ImportItems> dList);

        string SavePlanFromExcellAll(List<ImportItems> iList,List<ImportItems> dList);

        List<ProductSetupModel> GetProductRateByPlanCode(int plancode);

        string CreateTempSalesPlanReportTable(ReportFiltersModel model);
        string SaveEmployeeHandover(EmployeeHandoverModel model);
        plFiscalYearModel GetFiscalYear(string fiscalYearCode);
    }
}
