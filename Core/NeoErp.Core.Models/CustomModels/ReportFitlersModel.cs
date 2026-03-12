using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Core.Models
{
    public class ReportFiltersModel
    {

        public string Id { get; set; }

        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public decimal ExchangeRate { get; set; } = 1.00m;
        public string QuantityFigureFilter { get; set; } = FigureFilter.Actual.ToString();

        public string QuantityRoundUpFilter { get; set; } = "0.00";

        public string AmountFigureFilter { get; set; } = FigureFilter.Actual.ToString();

        public string AmountRoundUpFilter { get; set; } = "0.00";

        public string AmountRangeFilter { get; set; } = string.Empty;

        public string QuantityRangeFilter { get; set; } = string.Empty;

        public string RateRangeFilter { get; set; } = string.Empty;
        //public string LcNumberFilter { get; set; } = string.Empty;

        public List<String> CustomerFilter { get; set; } = new List<string>();

        public List<string> SalesPersonFilter { get; set; } = new List<string>();
        public List<String> CurrencyFilter { get; set; } = new List<string>();

        public List<String> ProductFilter { get; set; } = new List<string>();

        public List<String> SupplierFilter { get; set; } = new List<string>();
        public List<String> EmployeeFilter { get; set; } = new List<string>();
        public List<String> AgentFilter { get; set; } = new List<string>();
        public List<String> DivisionFilter { get; set; } = new List<string>();
        public List<String> DocumentFilter { get; set; } = new List<string>();

        public List<String> PurchaseDocumentFilter { get; set; } = new List<string>();

        public List<String> CategoryFilter { get; set; } = new List<string>();

        public List<String> PartyTypeFilter { get; set; } = new List<string>();

        public List<String> AreaTypeFilter { get; set; } = new List<string>();

        public List<String> LocationFilter { get; set; } = new List<string>();

        public List<String> CompanyFilter { get; set; } = new List<string>();

        public List<String> DistAreaFilter { get; set; } = new List<string>();

        public List<String> BrandFilter { get; set; } = new List<string>();

        public List<String> BranchFilter { get; set; } = new List<string>();
        public List<string> ContractStatusTypeFilter { get; set; } = new List<string>();

        public List<FiscalYearFilter> FiscalYearFilter { get; set; } = new List<Models.FiscalYearFilter>();

        public List<String> LcStatusFilter { get; set; } = new List<string>();
        public List<String> LcNumberFilter { get; set; } = new List<string>();
        public List<String> ItemNameFilter { get; set; } = new List<string>();
        public List<String> InvoiceNumberFilter { get; set; } = new List<string>();

        public List<String> UserNameFilter { get; set; } = new List<string>();
        public List<String> ModuleNameFilter { get; set; } = new List<string>();
        public List<String> MenuNameFilter { get; set; } = new List<string>();

        public List<String> LcLocationFilter { get; set; } = new List<string>();

        public List<string> DistEmployeeFilter { get; set; } = new List<string>();

        public List<String> LedgerFilter { get; set; } = new List<string>();

        public List<string> PostedGenericFilter { get; set; }
        public List<string> ItemBrandFilter { get; set; } = new List<string>();
        public List<string> AccountFilter { get; set; } = new List<string>();
        public List<String> FormCodeFilter { get; set; } = new List<string>();

    }


    public class FiscalYearFilter
    {
        public string DBName { get; set; }
        public string FiscalYear { get; set; }
    }
    public class ExchangeRate
    {
        public string ExchangeRateValue { get; set; }
    }
    public class CurrencyModel
    {
        public string CURRENCY_CODE { get; set; }
        public string CURRENCY_EDESC { get; set; }
    }

    public class BrandFilterModel
    {
        public string ITEM_CODE { get; set; }
        public string BRAND_NAME { get; set; }
    }

}
