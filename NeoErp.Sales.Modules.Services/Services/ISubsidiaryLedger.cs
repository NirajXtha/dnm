using NeoErp.Core.Domain;
using NeoErp.Core.Helpers;
using NeoErp.Core.Models;
using NeoErp.Sales.Modules.Services.Models;
using NeoErp.Sales.Modules.Services.Models.Ledger;
using NeoErp.Sales.Modules.Services.Models.Voucher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Services
{
    public interface ISubsidiaryLedger
    {
        List<TreeModels> CustomerListAllParentNodes(NeoErp.Core.Domain.User userinfo);
        List<TreeModels> GetCustomerListByCustCode(string level, string masterCustomerCode, NeoErp.Core.Domain.User userinfo);
        List<TreeModels> MisSubLedgerListAllParentNodes(NeoErp.Core.Domain.User userinfo);
        List<TreeModels> GetMiscSubLedgerBySubCode(string level, string masterCustomerCode, NeoErp.Core.Domain.User userinfo);
        List<TreeModels> EmployeeListAllParentNodes(NeoErp.Core.Domain.User userinfo);
        List<TreeModels> GetEmployeeListByEmpCode(string level, string masterCustomerCode, NeoErp.Core.Domain.User userinfo);
        List<TreeModels> SupplierListAllParentNodes(NeoErp.Core.Domain.User userinfo);
        List<TreeModels> GetSupplierListBySupCode(string level, string masterCustomerCode, NeoErp.Core.Domain.User userinfo);
        List<MultiSelectModels> GetSubsidiaryCustomers(NeoErp.Core.Domain.User userinfo);
        List<MultiSelectModels> GetSubsidiaryEmployees(NeoErp.Core.Domain.User userinfo);
        List<MultiSelectModels> GetSubsidiarySuppliers(NeoErp.Core.Domain.User userinfo);
        List<MultiSelectModels> GetSubsidiaryMSubLedger(NeoErp.Core.Domain.User userinfo);
        List<TreeModels> GetListByCode(string Code, NeoErp.Core.Domain.User userinfo,string listType);
        List<MultiSelectModels> GetListByHierarchy(string Code, User userinfo,string listType);
        List<MultiSelectModels> GetParentListByCode(string AccountCode,string listType,User userinfo);
        List<VoucherDetailModel> GetSubsidiaryVoucherDetails(ReportFiltersModel reportFilters, string formDate, string toDate, string AccountCode, NeoErp.Core.Domain.User userinfo, string linkSubCode, string BranchCode = null, string groupSkuFlag = "", string listType = "",string MasterCode="",string actionName="");
        List<MultiSelectModels> GetLedgerAC(string multiselectCodes);
    }
}
