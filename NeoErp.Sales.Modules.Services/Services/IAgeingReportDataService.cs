using NeoErp.Core.Domain;
using NeoErp.Sales.Modules.Services.Models.AgeingReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Services
{
    public interface IAgeingReportDataService
    {
        IEnumerable<AgeingDataModel> GetAgeingDebitAmount(string asOnDate, List<string> parameter);
        IEnumerable<AgeingDataModel> GetAgeingCreditAmount(string asOnDate, List<string> parameter);
        IEnumerable<AgeingDataModel> GetAgeingDebitAmount(string asOnDate, List<string> parameter, List<string> branchFilter, string billwise = null);
        IEnumerable<AgeingDataModel> GetAgeingCreditAmount(string asOnDate, List<string> parameter, List<string> branchFilter, string billwise = null);

        IEnumerable<AgeingDataModel> GetMobileAgeingDebitAmount(string asOnDate, List<string> parameter, List<string> branchFilter, string customerCode, string companyCode);
        IEnumerable<AgeingDataModel> GetMobileAgeingCreditAmount(string asOnDate, List<string> parameter, List<string> branchFilter,string customerCode, string companyCode);
        IEnumerable<AgeingGroupData> GetMobileGroupData(List<string> branchFilter,string customerCode, string companyCode);

        IEnumerable<AgeingGroupData> GetGroupData();
        IEnumerable<AgeingGroupData> GetGroupData(List<string> branchFilter);
        
        IEnumerable<AgeingDataModel> GetAgeingChartDebitAmount(string asOnDate, List<string> masterCustomerIds, List<string> branchFilter);
        IEnumerable<AgeingDataModel> GetAgeingChartCreditAmount(string asOnDate, List<string> masterCustomerIds, List<string> branchFilter);

        IEnumerable<AgeingDataModel> GetMobileAgeingChartDebitAmount(string asOnDate, List<string> masterCustomerIds, List<string> branchFilter);
        IEnumerable<AgeingDataModel> GetMobileAgeingChartCreditAmount(string asOnDate, List<string> masterCustomerIds, List<string> branchFilter);

        IEnumerable<AgeingDataModel> GetAgeingBillWiseChartDebitAmount(string asOnDate, List<string> masterCustomerIds, List<string> branchFilter);
        IEnumerable<AgeingDataModel> GetAgeingBillWiseChartCreditAmount(string asOnDate, List<string> masterCustomerIds, List<string> branchFilter);

        IEnumerable<AgeingDataModel> GetMobileAgeingDealerCreditAmount(string asOnDate, List<string> masterCustomerIds, List<string> branchFilter, string AccountCode = "0", string DealerGropCode = "0");
        IEnumerable<AgeingDataModel> GetMobileAgeingDealerDebitAmount(string asOnDate, List<string> masterCustomerIds, List<string> branchFilter, string AccountCode = "0", string DealerGropCode = "0");

        IEnumerable<testAgeing> testAgeingData(string asOnDate, List<string> masterCustomerIds, List<string> branchFilter);
        //IEnumerable<AgeingDataModel> GetAgeingDebitAmount(string asOnDate, List<string> parameter,User userinfo);
        //IEnumerable<AgeingDataModel> GetAgeingCreditAmount(string asOnDate, List<string> parameter,User userinfo);
        //IEnumerable<AgeingGroupData> GetGroupData(User userinfo);
    }
}
