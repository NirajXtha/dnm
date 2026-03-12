using NeoERP.DocumentTemplate.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Interface
{
    public interface ISubLegerMappingSetup
    {
        List<SubLedgerMapDetail> GetSubLedgerMappingForGrid(string subCode, string masterCode);

        List<SubLedgerMappingModal> GetCharOfAccountTree();

        List<CustomerSubLedgerModal> GetCustomerSubLedger();

        List<EmployeeSubLedgerModal> GetEmployeeSubLedger();

        List<ChartOfItemModel> GetChatOfItemSubLedger();
        List<SupplierSubLederModal> GetSupplierSubLedger();
        string SaveSubLedgerMapping(ModalToSaveSubLedgerMapping modal);
        List<CostCenterSubLedgerModal> GetCostCenterSubLedger();
        string SaveCostCenterMapping(ModalToSaveSubLedgerMapping modal);
        List<CostCenterSubLedgerModal> GetCostCenterMappingForGrid(string subCode, string masterCode);  
    }
}
