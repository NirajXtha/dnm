using NeoErp.Core.Domain;
using NeoErp.Core.Models;
using System.Collections.Generic;
using NeoErp.Core.Models.CustomModels;
using NeoERP.QuotationManagement.Service.Models;
using System.Net.Http;

namespace NeoERP.QuotationManagement.Service.Interface
{
    public interface IQuotationRepo
    {
        List<Products> GetAllProducts();
        List<QuotationLogModel> GetQuotationLogs();
        List<Company> GetCompany();
        string getUserType();
        List<Quotation_setup> GetQuotationId(string tender);
        bool InsertQuotationData(Quotation_setup data);
        bool CloneQuotation(Quotation_setup model);
        List<Quotation_setup> GetTenderId(string tenderNo);
        List<Quotation_setup> ListAllTenders();
        List<Quotation_setup> ListAllPendingTenders();
        List<Quotation_Details> ListQuotationDetails();
        bool deleteQuotationId(ApprovalRequest request);
        List<Quotation_setup> GetQuotationById(string tenderNo);
        bool updateItemsById(string tenderNo, string id, string q_no);
        List<CurrencyModel> getCurrency();
        List<Quotation_Details> QuotationDetailsById(string quotationNo, string tenderNo);
        List<Quotation_Details> QuotationDetailsId(string quotationNo, string tenderNo);

        List<SummaryReport> TendersItemWise(ReportFiltersModel model);
        List<Quotation> ItemDetailsTenderNo(string tenderNo);
        bool acceptQuotation(string quotationNo, string status, string type, string items, string itemId, string Remarks);
        bool rejectQuotation(string quotationNo, string status, string Remarks);
        bool InsertTenderData(Tender data);
        List<Tender> getTenderDetails();
        bool deleteTenderId(string id);
        List<Tender> getTenderById(string id);
        List<QuotationCount> GetQuotationCount();
        List<QuotationNotification> GetAllNotification();
        bool QuotationApproval(ApprovalRequest request);
        bool ApprovalProceeding();
        List<UserAcess> UserAccess(double amount);
        List<Tender> getTemplateOptions();
        List<Tender> getSelectQuotationOptions();
        List<Quotation_setup> getTemplateData(Template template);
        List<Quotation> getVoucherList(string code, string row);
        List<Employee> getUserValue();
        bool setUserValue(Employee employee);
        bool setUserAccess(UserAcess access);
        bool AddReference(List<Reference> reference, string form_code, string voucher_no);
        List<PartyDetails> partyDetails(string id);
        List<PartyDetailsItems> partyDetailsItems(string id);
        List<TermsAndConditions> termsAndConditions(string id);
        bool UserTypeToSetValue();
    }
}
