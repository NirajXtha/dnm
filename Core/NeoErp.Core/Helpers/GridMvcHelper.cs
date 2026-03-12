using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using GridMvc;
using GridMvc.Filtering;

namespace NeoErp.Core.Helpers
{
    public class GridMvcHelper
    {
        public static string OnRowSelect(string IDCol)
        {
            StringBuilder str = new StringBuilder();

            str.Append("  $(function() {");
            str.Append("pageGrids.ordersGrid.onRowSelect(function(e) {");
            str.Append("$(\"#id\").val(e.row." + IDCol + ");");
            str.Append(" });");
            str.Append("});");
        
            return str.ToString();
        }

        public class GridMvcFilter<T> where T : class
        {
            public static IEnumerable<T> ApplyFilter(IEnumerable<T> list)
            {
              

                
                var filter = new QueryStringFilterSettings(HttpContext.Current);
                Grid<T> grid = new Grid<T>(list);
                grid.AutoGenerateColumns();
                var process = new FilterGridItemsProcessor<T>(grid, filter); //Modified this class in GridMvc library from internal to public accessor.
                var data = process.Process(list.AsQueryable());
                return data;
            }
        }

    }

    /// <summary>
    ///     Object gets filter settings from query string
    /// </summary>
    public class QueryStringFilterSettings : IGridFilterSettings
    {
        public const string DefaultTypeQueryParameter = "grid-filter";
        private const string FilterDataDelimeter = "__";
        public const string DefaultFilterInitQueryParameter = "gridinit";
        public readonly HttpContext Context;
        private readonly DefaultFilterColumnCollection _filterValues = new DefaultFilterColumnCollection();

        #region Ctor's

        public QueryStringFilterSettings()
            : this(HttpContext.Current)
        {
        }

        public QueryStringFilterSettings(HttpContext context)
        {
            if (context == null)
                throw new ArgumentException("No http context here!");
            Context = context;

            string[] filters = Context.Request.QueryString.GetValues(DefaultTypeQueryParameter);

            //Overwrides filter value behaviour to address multiple filters in single , seperated key.
            if (filters!=null && filters.Length == 1 && filters[0].Contains(","))
            {
                var value = HttpContext.Current.Request.QueryString["grid-filter"];
                if (value != null)
                {
                    var valueArray = value.Split(',');
                    filters = valueArray;
                }
            }

            if (filters != null)
            {
                foreach (string filter in filters)
                {
                    ColumnFilterValue column = CreateColumnData(filter);
                    if (column != ColumnFilterValue.Null)
                        _filterValues.Add(column);
                }
            }
        }

        #endregion

        private ColumnFilterValue CreateColumnData(string queryParameterValue)
        {
            if (string.IsNullOrEmpty(queryParameterValue))
                return ColumnFilterValue.Null;

            string[] data = queryParameterValue.Split(new[] { FilterDataDelimeter }, StringSplitOptions.RemoveEmptyEntries);
            if (data.Length != 3)
                return ColumnFilterValue.Null;
            GridFilterType type;
            if (!Enum.TryParse(data[1], true, out type))
                type = GridFilterType.Equals;

            return new ColumnFilterValue { ColumnName = data[0], FilterType = type, FilterValue = data[2] };
        }

        #region IGridFilterSettings Members

        public IFilterColumnCollection FilteredColumns
        {
            get { return _filterValues; }
        }

        public bool IsInitState
        {
            get
            {
                if (FilteredColumns.Any()) return false;
                return Context.Request.QueryString[DefaultFilterInitQueryParameter] != null;
            }
        }

        #endregion
    }
}