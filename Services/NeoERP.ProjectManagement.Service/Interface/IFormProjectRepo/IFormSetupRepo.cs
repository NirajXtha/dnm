using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoERP.ProjectManagement.Service.Models;

namespace NeoERP.ProjectManagement.Service.Interface
{
    public interface IFormSetupRepo
    {
        List<FormSetup> GetFormSetup();
        List<FormSetup> GetFormSetup(string filter);
        List<FormSetup> GetFormSetupByFormCode(string formCode);
        List<TemplateDraftListModel> GetAllMenuInventoryAssigneeDraftTemplateList();
        List<TemplateDraftListModel> GetAllMenuInventoryAssigneeSavedDraftTemplateList();
        List<TemplateDraftListModel> GetAllMenuFinanceVoucherAssigneeDraftTemplateList();
        List<TemplateDraftListModel> GetAllMenuFinanceVoucherAssigneeSavedDraftTemplateList();
        List<TemplateDraftListModel> GetAllMenuSalesAssigneeDraftTemplateList();
        List<TemplateDraftListModel> GetAllMenuSalesAssigneeSavedDraftTemplateList();
        List<TemplateDraftListModel> GetAllDraftTemplateListByFormCode(string formCode);
        List<DraftFormModel> GetAllDraftTemplateDatabyTempCode(string tempCode);
        List<FormControlModels> GetFormControls(string formcode);
        int GetBackDaysByFormCode(string formCode);


       // string SaveSalesChalanFromExcel(List<SalesChalanExcelData> chalanList);
        string SaveSalesChalanFromExcel(List<SalesChalanExcelData> chalanList,string formCode,string tableName);
    }
}
