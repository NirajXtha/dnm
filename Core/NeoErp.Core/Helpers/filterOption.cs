using NeoErp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Core.Helpers
{
    public class GridFilter
    {
        public string Operator { get; set; }
        public string Field { get; set; }
        public string Value { get; set; }
    }

    public class GridFilters
    {
        public List<GridFilter> Filters { get; set; }
        public string Logic { get; set; }
    }

    public class GridSort
    {
        public string Field { get; set; }
        public string Dir { get; set; }
    }
    public class TrialBalancefilterOption
    {
        public TrialBalancefilterOption()
        {
            this.ReportFilters = new ReportFiltersModel();
        }
        public string formDate { get; set; } = null;
        public string toDate { get; set; } = null;
        public List<GridSort> sort { get; set; } = null;
        public GridFilters filter { get; set; } = null;
        public int take { get; set; } = 100;
        public int skip { get; set; } = 0;
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 100;
        public int id { get; set; } = 0;
        public string parentId { get; set; } = "0";
        public string subledger { get; set;} = "0";
        public string datatype { get; set; } = "DG";
        public List<aggregatemodel> aggregate { get; set; }
        public ReportFiltersModel ReportFilters { get; set; }
    }

    public class filterOption
    {
        public filterOption()
        {
            this.ReportFilters = new ReportFiltersModel();
        }
        public string formDate { get; set; } = null;
        public string toDate { get; set; } = null;
        public List<GridSort> sort { get; set; } = null;
        public GridFilters filter { get; set; } = null;
        public int take { get; set; } = 100;
        public int skip { get; set; } = 0;
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 100;
        public List<aggregatemodel> aggregate { get; set; }
        public ReportFiltersModel ReportFilters { get; set; }
    }

    public class aggregatemodel
    {
        public string field { get; set; }
        public string aggregate { get; set; }
    }

    public class CustomerTree
    {
        public string customerName { get; set; }
        public string customerId { get; set; }
        public bool hasCustomers { get; set; }
        public int Level { get; set; }
        public string masterCustomerCode { get; set; }
        public string preCustomerCode { get; set; }
        public IEnumerable<CustomerTree> Items { get; set; }
    }



    public class ConsolidateTree
    {
        public string branch_edesc { get; set; }
        public string branch_Code { get; set; }
        public string GroupSkuFlag { get; set; }
        public bool hasBranch { get; set; }
        public string MasterItemCode { get; set; }
        public string pre_branch_code { get; set; }
        public string Abbr_Code { get; set; }
        public bool @checked { get; set; } = false;
        public IEnumerable<ConsolidateTree> Items { get; set; }
    }

    public class ProductTree
    {
        public string ItemName { get; set; }
        public string ItemCode { get; set; }

        public string GroupSkuFlag { get; set; }
        public bool hasProducts { get; set; }
        public int Level { get; set; }
        public string MasterItemCode { get; set; }
        public string PreItemCode { get; set; }
        public decimal? Rate { get; set; }
        public IEnumerable<ProductTree> Items { get; set; }
    }

    //** suplier tree view
    public class SupplierTree
    {
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
        public bool hasSuppliers { get; set; }
        public int Level { get; set; }
        public string MasterSupplierCode { get; set; }
        public string PreSupplierCode { get; set; }
        public IEnumerable<SupplierTree> Items { get; set; }
    }

    public class DocumentTree
    {
        public string VoucherName { get; set; }
        public string VoucherCode { get; set; }
        public bool hasChildren { get; set; }
        public int Level { get; set; }
        public string MasterFormCode { get; set; }
        public string PreFormCode { get; set; }
        public IEnumerable<ProductTree> Items { get; set; }
    }

    public class LocationTree
    {
        public string LocationName { get; set; }
        public string LocationCode { get; set; }
        public bool hasLocation { get; set; }
        public int Level { get; set; }
        public string MasterLocationCode { get; set; }
        public string PreLocationCode { get; set; }
        public IEnumerable<ProductTree> Items { get; set; }
    }

    public class BrandingActivityTree
    {
        public string BrandingActivityName { get; set; }
        public string BrandingActivityCode { get; set; }
        public string GroupSkuFlag { get; set; }
        public bool hasProducts { get; set; }
        public int Level { get; set; }
        public string MasterItemCode { get; set; }
        public string PreItemCode { get; set; }
        public IEnumerable<BrandingActivityTree> Items { get; set; }
    }
   
}