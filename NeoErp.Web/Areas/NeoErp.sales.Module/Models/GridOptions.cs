using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;

namespace NeoErp.sales.Module.Models
{
    public class GridOptions
    {

    }
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

    public class filterOption
    {
        public string formDate { get; set; } = null;
        public string toDate { get; set; } = null;
        public List<GridSort> sort { get; set; } = null;
        public GridFilters filter { get; set; } = null;
        public int take { get; set; } = 100;
        public int skip { get; set; } = 0;
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 100;
        public List<aggregatemodel> aggregate { get; set; }
        public filterOption()
        {

        }
       
    }

    public class aggregatemodel
    {
        public string field { get; set; }
        public string aggregate { get; set; }
    }

}