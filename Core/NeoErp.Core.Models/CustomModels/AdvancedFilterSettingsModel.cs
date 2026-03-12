using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Core.Models
{
    public class AdvancedFilterSettingsModel
    {
        public bool ShowCustomerFilter { get; set; }

        public bool ShowProductFilter { get; set; }

        public bool ShowSupplierFilter { get; set; }

        public bool ShowDocumentFilter { get; set; }

        public bool ShowPurchaseDocumentFilter { get; set; }

        public bool ShowCategoryFilter { get; set; }

        public bool ShowPartyTypeFilter { get; set; }

        public bool ShowLocationFilter { get; set; }

        public bool ShowLocationWiseFilter { get; set; }

        public bool ShowCompanyFilter { get; set; }

        public bool ShowBranchFilter { get; set; }      

        public bool ShowAccountFilter { get; set; }
        public bool ShowAccountsFilter { get; set; }

        public bool ShowEmployeeFilter { get; set; }

        public bool ShowDivisionFilter { get; set; }

        public bool ShowAgentFilter { get; set; }

        public bool ShowCurrencyFilter { get; set; }

        public bool ShowFiscalYearFilter { get; set; }

        public bool IsPopUp { get; set; } = true;

        public string ActionPageId { get; set; }

        public bool ShowContractStatusTye { get; set; }

        public bool ShowDistributionFilter { get; set; }

        public bool ShowResellerFilter { get; set; }

        public bool ShowSalesPersonFilter { get; set; }

        public bool ShowAreaFilter { get; set; }

        public bool ShowDistEmployeeFilter { get; set; }
        public bool ShownewPartyTypeFilter { get; set; }
        public bool ShowDistEmployeeListFilter { get; set; }
        public bool ShowBrandFilter { get; set; }

        public bool ShowDistAreaListFilter { get; set; }

        //For LC ShowItemNameFilter  
        public bool ShowLcStatusFilter { get; set; }
        public bool ShowLcNumberFilter { get; set; }
        public bool ShowInvoiceNumberFilter { get; set; }
        public bool ShowItemNameFilter { get; set; }
        public bool ShowExpiryDateFilter { get; set; }
        public bool ShowShipmentDateFilter { get; set; }
        public bool ShowLcLocationFilter { get; set; }
        public bool ShowMenuNameFilter { get; set; }
        public bool ShowModuleNameFilter { get; set; }
        public bool ShowUserNameFilter { get; set; }

        public bool ShowLedgerFilter { get; set; }

        public bool ShowPostedGenericFilter { get; set; }
        //

        //plan
        public bool ShowProductionProductFilter { get; set; }

        public bool ShowItemBrandFilter { get; set; }
        public bool ShowCustomerTreeFilter { get; set; }

    }
}
