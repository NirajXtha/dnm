using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;
using System.Reflection;
using System.Text;
using System.Web;

namespace NeoErp.Core.Helpers
{
    public static class KendoGridHelper
    {
        public static string BuildWhereClause<T>(int index, string logic, GridFilter filter, System.Collections.ArrayList parameters)
        {

            var entityType = (typeof(T));
            PropertyInfo property;
            if (filter.Field.Contains("."))
                property = GetNestedProp<T>(filter.Field.ToString());
            else
                property = entityType.GetProperty(filter.Field, BindingFlags.Instance | BindingFlags.Public |
                    BindingFlags.NonPublic);
            property = entityType.GetRuntimeProperty(filter.Field.ToString());
            var parameterIndex = parameters.Count;
            switch (filter.Operator.ToLower())
            {
                case "eq":
                case "neq":
                case "gte":
                case "gt":
                case "lte":
                case "lt":
                    if (property != null)
                    {
                        int i = 0;
                        var intValue = int.TryParse(filter.Value, out i);
                        decimal j = 0.0m;
                        var decimalValue = decimal.TryParse(filter.Value, out j);
                        DateTime k;
                        var dateValue = DateTime.TryParse(filter.Value, out k);
                        if (dateValue)
                        {
                            if (typeof(DateTime).IsAssignableFrom(property.PropertyType))
                            {
                                //linq operator is not supported 
                                parameters.Add(DateTime.Parse(filter.Value).Date);
                                //return string.Format("EntityFunctions.TruncateTime({0}){1}@{2}", filter.Field, ToLinqOperator(filter.Operator), index);
                                return string.Format("({0}){1}@{2}", filter.Field, ToLinqOperator(filter.Operator), index);
                            }
                            if (typeof(DateTime?).IsAssignableFrom(property.PropertyType))
                            {
                                parameters.Add(DateTime.Parse(filter.Value).Date);
                                return string.Format("({0}){1}@{2}", filter.Field, ToLinqOperator(filter.Operator), index);
                            }
                        }
                        else
                        {
                            return "";
                        }

                        if (intValue)
                        {
                            if (typeof(int).IsAssignableFrom(property.PropertyType))
                            {
                                parameters.Add(int.Parse(filter.Value));
                                return string.Format("{0}{1}@{2}", filter.Field, ToLinqOperator(filter.Operator), index);
                            }
                            if (typeof(int?).IsAssignableFrom(property.PropertyType))
                            {

                                parameters.Add(int.Parse(filter.Value));
                                return string.Format("{0}{1}@{2}", filter.Field, ToLinqOperator(filter.Operator), index);
                            }
                        }
                        else
                        {
                            return "";
                        }
                        if (decimalValue)
                        {
                            if (typeof(Decimal?).IsAssignableFrom(property.PropertyType))
                            {
                                parameters.Add(Decimal.Parse(filter.Value));
                                return string.Format("{0}{1}@{2}", filter.Field, ToLinqOperator(filter.Operator), index);
                            }
                            if (typeof(Decimal).IsAssignableFrom(property.PropertyType))
                            {
                                parameters.Add(Decimal.Parse(filter.Value));
                                return string.Format("{0}{1}@{2}", filter.Field, ToLinqOperator(filter.Operator), index);
                            }
                        }
                        else
                        {
                            return "";
                        }
                    }
                    else
                    {
                        parameters.Add(filter.Value);

                        return string.Format("{0}{1}@{2}", filter.Field, ToLinqOperator(filter.Operator), index);
                    }
                    return "";

                case "startswith": parameters.Add(filter.Value); return string.Format("{0}.StartsWith(" + "@{1})", filter.Field, index);
                case "endswith":
                    parameters.Add(filter.Value); return string.Format("{0}.EndsWith(" + "@{1})", filter.Field, index);
                case "contains": parameters.Add(filter.Value); return string.Format("{0}.Contains(" + "@{1})", filter.Field, index);
                default: throw new ArgumentException("This operator is not yet supported for this Grid", filter.Operator);
            }
        }
        public static string ToLinqOperator(string @operator)
        {
            switch (@operator.ToLower())
            {
                case "eq":
                    return " == ";
                case "neq":
                    return " != ";
                case "gte":
                    return " >= ";
                case "gt":
                    return " > ";
                case "lte": return " <= ";
                case "lt": return " < ";
                case "or": return " || ";
                case "and": return " && ";
                default: return null;
            }
        }
        public static PropertyInfo GetNestedProp<T>(String name)
        {
            PropertyInfo info = null;
            var type = (typeof(T));
            foreach (var prop in name.Split('.'))
            {
                info = type.GetProperty(prop);
                type = info.PropertyType;
            }
            return info;
        }
        public static void ProcessFilters<T>(filterOption model, ref IQueryable<T> queryable)
        {
            if (model.filter != null && (model.filter.Filters != null && model.filter.Filters.Count > 0))
            {
                string whereClause = null;
                var parameters = new System.Collections.ArrayList();
                var filters = model.filter.Filters;
                for (var i = 0; i < filters.Count; i++)
                {
                    if (i == 0)
                        whereClause += string.Format("{0}", BuildWhereClause<T>(i, model.filter.Logic, filters[i], parameters));
                    else
                        whereClause += string.Format(" {0} {1}", ToLinqOperator(model.filter.Logic), BuildWhereClause<T>(i, model.filter.Logic, filters[i], parameters));
                }
                // Where
                queryable.Where(whereClause, parameters.ToArray());

            }
        }

        public static Dictionary<string, AggregationModel> GetAggregation<T>(List<T> data, List<aggregatemodel> aggregateField = null)
        {
            var dictionary = new Dictionary<string, AggregationModel>();
            var type = typeof(T);
            var properties = type.GetProperties().Where(q => (q.PropertyType == typeof(decimal?)
            || q.PropertyType == typeof(int?) || q.PropertyType == typeof(decimal) || q.PropertyType == typeof(int) ||
            q.PropertyType == typeof(float?) || q.PropertyType == typeof(float) || q.PropertyType == typeof(double?) || q.PropertyType == typeof(double))).ToList();
            if (aggregateField != null)
            {
                var aggregateFieldNames = aggregateField.Select(q => q.field).Distinct().ToList();

                properties = properties.Where(q => aggregateFieldNames.Contains(q.Name)).ToList();
            }

            IQueryable<T> dataQueryable = data.AsQueryable<T>();
            if (dataQueryable.Count() > 0)
            {
                foreach (var property in properties)
                {
                    var aggregate = new AggregationModel() { };

                    if (aggregateField != null)
                    {
                        var aggregateFieldTypes = aggregateField.Where(q => q.field == property.Name).ToList();
                        foreach (var item in aggregateFieldTypes)
                        {
                            if (item.aggregate == "sum")
                                aggregate.sum = dataQueryable.GetAggregateResult("Sum", property.Name);
                            else if (item.aggregate == "min")
                                aggregate.min = dataQueryable.GetAggregateResult("Min", property.Name);
                            else if (item.aggregate == "max")
                                aggregate.max = dataQueryable.GetAggregateResult("Max", property.Name);
                        }

                    }
                    else
                    {
                        aggregate.sum = dataQueryable.GetAggregateResult("Sum", property.Name);
                        aggregate.min = dataQueryable.GetAggregateResult("Min", property.Name);
                        aggregate.max = dataQueryable.GetAggregateResult("Max", property.Name);
                    }


                    //aggregate.average = dataQueryable.GetAggregateResult("Average", property.Name);
                    dictionary.Add(property.Name, aggregate);
                }
            }
            return dictionary;
        }

        public static Dictionary<string, AggregationModel> GetAggregation<T>(List<T> data)
        {
            return GetAggregation(data, null);
            //var dictionary = new Dictionary<string, AggregationModel>();
            //var type = typeof(T);
            //var properties = type.GetProperties().Where(q => q.PropertyType == typeof(decimal?)
            //|| q.PropertyType == typeof(int?) || q.PropertyType == typeof(decimal) || q.PropertyType == typeof(int) ||
            //q.PropertyType == typeof(float?) || q.PropertyType == typeof(float) || q.PropertyType == typeof(double?) || q.PropertyType == typeof(double)).ToArray();
            //IQueryable<T> dataQueryable = data.AsQueryable<T>();
            //if (dataQueryable.Count() > 0)
            //{
            //    foreach (var property in properties)
            //    {
            //        var aggregate = new AggregationModel() { };
            //        aggregate.sum = dataQueryable.GetAggregateResult("Sum", property.Name);
            //        aggregate.min = dataQueryable.GetAggregateResult("Min", property.Name);
            //        aggregate.max = dataQueryable.GetAggregateResult("Max", property.Name);
            //        //aggregate.average = dataQueryable.GetAggregateResult("Average", property.Name);
            //        dictionary.Add(property.Name, aggregate);
            //    }
            //}
            //return dictionary;
        }
        public static DataTable FilterDataTable(this DataTable dataTable, int page, int pageSize, out int total, List<GridSort> gridSort = null, string logic = "", List<GridFilter> gridFilter = null)
        {
            var filterDataTable = dataTable.Clone();
            var dataRows = new List<DataRow>();

            var sortQuery = string.Empty;

            if (gridSort != null && gridSort.Count > 0)
            {
                var columnSort = gridSort.First();

                sortQuery = string.Format("{0} {1}", columnSort.Field, ToDataTableOperator(columnSort.Dir));
            }

            if (gridFilter != null)
            {
                var gridCount = gridFilter.Count();
                StringBuilder dataTableQuery = new StringBuilder();
                for (int i = 0; i < gridCount; i++)
                {
                    var item = gridFilter[i];
                    var column = dataTable.Columns[item.Field];

                    if (i == 0)
                        dataTableQuery.Append(BuildDataTableQuery(column.DataType, item));
                    else
                        dataTableQuery.Append(string.Format(" {0} {1}", ToDataTableOperator(logic), BuildDataTableQuery(column.DataType, item)));

                }
                dataTable.Select(dataTableQuery.ToString(), sortQuery).CopyToDataTable(filterDataTable, LoadOption.OverwriteChanges);
            }
            else
            {
                dataTable.Select(string.Empty, sortQuery).CopyToDataTable(filterDataTable, LoadOption.OverwriteChanges);
            }

            //foreach(var item in dataRows)
            //{
            //    filterDataTable.ImportRow(item);
            //}
            int skip = (page - 1) * pageSize;
            total = dataTable.Select().Count();
            dataTable.Clear();
            filterDataTable.AsEnumerable().Skip(skip).Take(pageSize).ToArray().CopyToDataTable(dataTable, LoadOption.OverwriteChanges);

            return dataTable;
        }
        public static string BuildDataTableQuery(Type t, GridFilter item)
        {
            switch (item.Operator.ToLower())
            {
                case "eq":
                case "neq":
                case "gte":
                case "gt":
                case "lte":
                case "lt":
                    if (t == typeof(DateTime))
                    {
                        return string.Format("{0} {1} #{2}#", item.Field, ToDataTableOperator(item.Operator), DateTime.Parse(item.Value).ToString("dd/MM/yyyy"));
                    }
                    else if (t == typeof(string))
                    {
                        return string.Format("{0} {1} '{2}'", item.Field, ToDataTableOperator(item.Operator), item.Value);
                    }
                    else
                    {
                        return string.Format("{0} {1} {2}", item.Field, ToDataTableOperator(item.Operator), item.Value);
                    }
                case "startswith":
                    return string.Format("{0} like '{1}%'", item.Field, item.Value);
                case "endswith":
                    return string.Format("{0} like '%{1}'", item.Field, item.Value);
                case "contains":
                    return string.Format("{0} like '%{1}%'", item.Field, item.Value);
                default: throw new ArgumentException("This operator is not yet supported for this Grid", item.Operator);

            }
        }
        public static string ToDataTableOperator(string @operator)
        {
            switch (@operator.ToLower())
            {
                case "eq":
                    return " = ";
                case "neq":
                    return " <> ";
                case "gte":
                    return " >= ";
                case "gt":
                    return " > ";
                case "asc":
                    return "ASC";
                case "desc":
                    return "desc";
                case "lte": return " <= ";
                case "lt": return " < ";
                case "or": return " or ";
                case "and": return " and ";
                default: return string.Empty;
            }
        }
    }
};