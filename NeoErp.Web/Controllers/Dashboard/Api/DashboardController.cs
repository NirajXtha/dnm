using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Data.OracleClient;
//using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using OfficeOpenXml;
using System.Text.RegularExpressions;
namespace NeoErp.Controllers.Dashboard.Api
{
    [RoutePrefix("api/Dashboard")]
    public class DashboardController : ApiController
    {
        private string _connection;
        private OracleCommand _command;
        public DashboardController(string dbUser, string dbPass)
        {
            _connection = ConfigurationManager.ConnectionStrings["NeoErpCoreEntity"].ToString();
        }
        #region Stock Record Module Developed By: Prem Prakash Dhakal Date: 09-19-2024
        [HttpGet()]
        [Route("allsubledger/{companyCode}")]
        public async Task<IHttpActionResult> AllSubLedger(string companyCode)
        {
            var result = new Dictionary<string, object>();

            //Fetching Categories
            var categoryQuery = "SELECT category_code, CATEGORY_EDESC FROM IP_CATEGORY_CODE ORDER BY CATEGORY_EDESC ASC";
            result["cat"] = ExecuteQueryToDict(categoryQuery);

            //Fetching Item Groups
            var itemGroupQuery = $"SELECT ITEM_EDESC, master_ITEM_CODE FROM IP_ITEM_MASTER_SETUP WHERE GROUP_SKU_FLAG = 'G' AND company_code = '{companyCode}' ORDER BY ITEM_EDESC";
            result["grp"] = ExecuteQueryToDict(itemGroupQuery);

            //Fetching Brands
            var brandQuery = $"SELECT DISTINCT brand_name FROM ip_item_spec_setup WHERE brand_name IS NOT NULL AND brand_name NOT IN (' ') AND company_code = '{companyCode}' ORDER BY brand_name";
            result["brand"] = ExecuteQueryToList(brandQuery);

            //Fetching Customer Groups
            var customerGroupQuery = $"SELECT CUSTOMER_EDESC, master_CUSTOMER_CODE FROM SA_CUSTOMER_SETUP WHERE GROUP_SKU_FLAG = 'G' AND company_code = '{companyCode}' ORDER BY CUSTOMER_EDESC";
            result["cus_grp"] = ExecuteQueryToDict(customerGroupQuery);

            //Fetching Supplier Groups
            var supplierGroupQuery = $"SELECT supplier_edesc, master_supplier_code FROM ip_supplier_setup WHERE GROUP_SKU_FLAG = 'G' AND company_code = '{companyCode}' ORDER BY supplier_EDESC";
            result["sup_grp"] = ExecuteQueryToDict(supplierGroupQuery);

            //Fetching Units
            var unitQuery = @"SELECT DISTINCT index_mu_code FROM 
                              (SELECT item_code, index_mu_code, 1 ratio FROM ip_item_master_setup 
                               WHERE index_mu_code IS NOT NULL AND index_mu_code != 'None' AND GROUP_SKU_FLAG = 'I' AND deleted_flag = 'N'
                               UNION ALL
                               SELECT item_code, mu_code, CASE WHEN conversion_factor = 0 THEN 0 ELSE NVL(conversion_factor, 0) / NVL(fraction, 0) END ratio 
                               FROM IP_ITEM_UNIT_SETUP WHERE serial_no = '1' AND deleted_flag = 'N')";
            result["unit"] = ExecuteQueryToList(unitQuery);
            return Ok(result);
        }

        [HttpGet()]
        public async Task<IHttpActionResult> ItemWiseInOutStockReport(string rep_type, string company_code, string item_string_filter, string brand_str, string cat_str, string group_str)
        {
            var query = string.Empty;
            var results = new Dictionary<string, object>();

            if (rep_type == "purGrp")
            {
                query = $@"
                    SELECT ITEM_EDESC, INDEX_MU_CODE, GROUP_SKU_FLAG, MASTER_ITEM_CODE, PRE_ITEM_CODE,
                        ISNULL(SALES_QTY, 0) AS SALES_QTY,
                        ISNULL(SALES_VALUE, 0) AS SALES_VALUE,
                        ISNULL(SALES_RET_QTY, 0) AS SALES_RET_QTY,
                        ISNULL(SALES_RET_VALUE, 0) AS SALES_RET_VALUE,
                        ISNULL(NET_SALES_QTY, 0) AS NET_SALES_QTY,
                        ISNULL(NET_SALES_VALUE, 0) AS NET_SALES_VALUE
                    FROM
                    (
                        SELECT * FROM
                        (
                            SELECT ITEM_EDESC, ITEM_CODE, INDEX_MU_CODE, GROUP_SKU_FLAG, MASTER_ITEM_CODE, PRE_ITEM_CODE 
                            FROM IP_ITEM_MASTER_SETUP 
                            WHERE COMPANY_CODE = '01' AND DELETED_FLAG = 'N'
                        ) A
                        LEFT OUTER JOIN
                        (
                            SELECT ITEM_CODE AS F_CODE, 
                                SALES_ROLL_QTY, 
                                SALES_QTY, SALES_VALUE, SALES_RET_ROLL_QTY,
                                SALES_RET_QTY, SALES_RET_VALUE, SALES_ROLL_QTY - SALES_RET_ROLL_QTY AS NET_ROLL_QTY,
                                SALES_QTY - SALES_RET_QTY AS NET_SALES_QTY, SALES_VALUE - SALES_RET_VALUE AS NET_SALES_VALUE
                            FROM
                            (
                                SELECT ITEM_CODE, 
                                    SUM(ISNULL(ROLL_QTY, 0)) AS SALES_ROLL_QTY, 
                                    SUM(ISNULL(QUANTITY, 0)) AS SALES_QTY,  
                                    SUM(ISNULL(QUANTITY * CALC_UNIT_PRICE * EXCHANGE_RATE, 0)) AS SALES_VALUE, 
                                    0 AS SALES_RET_ROLL_QTY, 
                                    0 AS SALES_RET_QTY, 
                                    0 AS SALES_RET_VALUE 
                                FROM IP_PURCHASE_INVOICE 
                                WHERE COMPANY_CODE IN ({company_code}) AND DELETED_FLAG = 'N' {item_string_filter} {brand_str} {cat_str} {group_str}
                                GROUP BY COMPANY_CODE, BRANCH_CODE, ITEM_CODE 
                                UNION ALL 
                                SELECT COMPANY_CODE, BRANCH_CODE, ITEM_CODE, 0 AS SALES_ROLL_QTY, 0 AS SALES_QTY, 0 AS SALES_VALUE, 
                                    SUM(ISNULL(ROLL_QTY, 0)) AS SALES_RET_ROLL_QTY, 
                                    SUM(ISNULL(QUANTITY, 0)) AS SALES_RET_QTY,  
                                    SUM(ISNULL(QUANTITY * CALC_UNIT_PRICE * EXCHANGE_RATE, 0)) AS SALES_RET_VALUE  
                                FROM IP_PURCHASE_RETURN 
                                WHERE COMPANY_CODE IN ({company_code}) AND DELETED_FLAG = 'N' {item_string_filter} {brand_str} {cat_str} {group_str} 
                                GROUP BY COMPANY_CODE, BRANCH_CODE, ITEM_CODE
                            ) A
                            GROUP BY A.ITEM_CODE
                        ) B ON A.ITEM_CODE = B.F_CODE
                    )
                    ORDER BY 4, 3 ASC
                ";

                using (var connection = new OracleConnection(_connection))
                {
                    var command = new OracleCommand(query, connection);
                    await connection.OpenAsync();

                    var reader = await command.ExecuteReaderAsync();
                    var allCodes = new List<dynamic>();

                    while (await reader.ReadAsync())
                    {
                        allCodes.Add(new
                        {
                            ItemDesc = reader["ITEM_EDESC"],
                            IndexMuCode = reader["INDEX_MU_CODE"],
                            GroupSkuFlag = reader["GROUP_SKU_FLAG"],
                            MasterItemCode = reader["MASTER_ITEM_CODE"],
                            PreItemCode = reader["PRE_ITEM_CODE"],
                            SalesQty = reader["SALES_QTY"],
                            SalesValue = reader["SALES_VALUE"],
                            SalesRetQty = reader["SALES_RET_QTY"],
                            SalesRetValue = reader["SALES_RET_VALUE"],
                            NetSalesQty = reader["NET_SALES_QTY"],
                            NetSalesValue = reader["NET_SALES_VALUE"]
                        });
                    }

                    var vals = new List<Dictionary<string, object>>();
                    var arrTotal2 = new List<float>();
                    var arrTotal3 = new List<float>();
                    var arrTotal4 = new List<float>();
                    var arrTotal5 = new List<float>();
                    var arrTotal6 = new List<float>();
                    var arrTotal7 = new List<float>();

                    foreach (var i in allCodes)
                    {
                        var total2 = 0f;
                        var total3 = 0f;
                        var total4 = 0f;
                        var total5 = 0f;
                        var total6 = 0f;
                        var total7 = 0f;
                        var addOn = false;

                        foreach (var z in allCodes)
                        {
                            if (z.GroupSkuFlag == "I")
                            {
                                var asd = z.MasterItemCode.Length >= i.MasterItemCode.Length
                                    ? z.MasterItemCode.Substring(0, i.MasterItemCode.Length)
                                    : string.Empty;

                                if (i.MasterItemCode == asd)
                                {
                                    addOn = true;
                                    total2 += (float)z.SalesQty;
                                    total3 += (float)z.SalesValue;
                                    total4 += (float)z.SalesRetQty;
                                    total5 += (float)z.SalesRetValue;
                                    total6 += (float)z.NetSalesQty;
                                    total7 += (float)z.NetSalesValue;
                                }
                                else if (addOn)
                                {
                                    break;
                                }
                            }
                        }

                        if (total2 != 0 || total3 != 0 || total4 != 0 || total5 != 0 || total6 != 0 || total7 != 0)
                        {
                            vals.Add(new Dictionary<string, object>
                            {
                                {"name", i.ItemDesc},
                                {"unit", i.IndexMuCode},
                                {"group_flag", i.GroupSkuFlag},
                                //{"level", i.MasterItemCode.Count(c => c == '.')},//prem comment
                                {"master", i.MasterItemCode},
                                {"pre", i.PreItemCode},
                                {"p_qty", total2},
                                {"p_value", total3},
                                {"p_ret_qty", total4},
                                {"p_ret_value", total5},
                                {"p_net_qty", total6},
                                {"p_net_value", total7}
                            });
                        }

                        if (i.GroupSkuFlag == "I")
                        {
                            arrTotal2.Add((float)i.SalesQty);
                            arrTotal3.Add((float)i.SalesValue);
                            arrTotal4.Add((float)i.SalesRetQty);
                            arrTotal5.Add((float)i.SalesRetValue);
                            arrTotal6.Add((float)i.NetSalesQty);
                            arrTotal7.Add((float)i.NetSalesValue);
                        }
                    }

                    results["product_wise_report"] = vals;
                    results["p_qty"] = arrTotal2.Sum();
                    results["p_value"] = arrTotal3.Sum();
                    results["p_ret_qty"] = arrTotal4.Sum();
                    results["p_ret_value"] = arrTotal5.Sum();
                    results["p_net_qty"] = arrTotal6.Sum();
                    results["p_net_value"] = arrTotal7.Sum();
                }
            }
            else if (rep_type == "purSupp")
            {
                query = $@"
                    SELECT supplier_edesc, sales_qty, sales_value, sales_ret_qty, sales_ret_value, net_sales_qty, net_sales_value 
                    FROM
                    (
                        SELECT supplier_code, 
                            SALES_ROLL_QTY, 
                            SALES_QTY, SALES_VALUE, SALES_RET_ROLL_QTY,
                            SALES_RET_QTY, SALES_RET_VALUE, SALES_ROLL_QTY - SALES_RET_ROLL_QTY AS NET_ROLL_QTY,
                            SALES_QTY - SALES_RET_QTY AS NET_SALES_QTY, SALES_VALUE - SALES_RET_VALUE AS NET_SALES_VALUE
                        FROM
                        (
                            SELECT A.supplier_code, 
                                SUM(A.SALES_ROLL_QTY) AS SALES_ROLL_QTY, 
                                SUM(A.SALES_QTY) AS SALES_QTY,  
                                SUM(A.SALES_VALUE) AS SALES_VALUE, 
                                SUM(A.SALES_RET_ROLL_QTY) AS SALES_RET_ROLL_QTY, 
                                SUM(A.SALES_RET_QTY) AS SALES_RET_QTY,
                                SUM(A.SALES_RET_VALUE) AS SALES_RET_VALUE
                            FROM
                            (
                                SELECT COMPANY_CODE, BRANCH_CODE, SUPPLIER_CODE, 
                                    SUM(ISNULL(ROLL_QTY, 0)) AS SALES_ROLL_QTY, 
                                    SUM(ISNULL(QUANTITY, 0)) AS SALES_QTY,  
                                    SUM(ISNULL(QUANTITY * CALC_UNIT_PRICE * EXCHANGE_RATE, 0)) AS SALES_VALUE, 
                                    0 AS SALES_RET_ROLL_QTY, 
                                    0 AS SALES_RET_QTY, 
                                    0 AS SALES_RET_VALUE 
                                FROM IP_PURCHASE_INVOICE 
                                WHERE COMPANY_CODE IN ({company_code}) AND DELETED_FLAG = 'N' {item_string_filter} {brand_str} {cat_str} {group_str}
                                GROUP BY COMPANY_CODE, BRANCH_CODE, SUPPLIER_CODE 
                                UNION ALL 
                                SELECT COMPANY_CODE, BRANCH_CODE, SUPPLIER_CODE, 
                                    0 AS SALES_ROLL_QTY, 
                                    0 AS SALES_QTY, 
                                    0 AS SALES_VALUE, 
                                    SUM(ISNULL(ROLL_QTY, 0)) AS SALES_RET_ROLL_QTY, 
                                    SUM(ISNULL(QUANTITY, 0)) AS SALES_RET_QTY,  
                                    SUM(ISNULL(QUANTITY * CALC_UNIT_PRICE * EXCHANGE_RATE, 0)) AS SALES_RET_VALUE  
                                FROM IP_PURCHASE_RETURN 
                                WHERE COMPANY_CODE IN ({company_code}) AND DELETED_FLAG = 'N' {item_string_filter} {brand_str} {cat_str} {group_str}
                                GROUP BY COMPANY_CODE, BRANCH_CODE, SUPPLIER_CODE
                            ) A
                            GROUP BY A.supplier_code
                        ) A
                    ) B
                    LEFT OUTER JOIN IP_SUPPLIER_SETUP C ON B.supplier_code = C.supplier_code
                    WHERE C.deleted_flag = 'N'
                    ORDER BY 1
                ";

                using (var connection = new OracleConnection(_connection))
                {
                    var command = new OracleCommand(query, connection);
                    await connection.OpenAsync();

                    var reader = await command.ExecuteReaderAsync();
                    var vals = new List<Dictionary<string, object>>();

                    while (await reader.ReadAsync())
                    {
                        vals.Add(new Dictionary<string, object>
                        {
                            {"supplier", reader["supplier_edesc"]},
                            {"sales_qty", reader["sales_qty"]},
                            {"sales_value", reader["sales_value"]},
                            {"sales_ret_qty", reader["sales_ret_qty"]},
                            {"sales_ret_value", reader["sales_ret_value"]},
                            {"net_sales_qty", reader["net_sales_qty"]},
                            {"net_sales_value", reader["net_sales_value"]}
                        });
                    }
                    results["supplier_wise_report"] = vals;
                }
            }

            return Ok(results);
        }
        [HttpGet()]
        public async Task<IHttpActionResult> GetBrandWiseItem(string companyCode, string brand)
        {
            var items = await GetBrandWiseItemFromDatabase(companyCode, brand);
            return Ok(items);
        }

        private async Task<List<string>> GetBrandWiseItemFromDatabase(string companyCode, string brand)
        {
            var itemList = new List<string>();
            // SQL Query to get item codes
            string query = @"
                SELECT item_code 
                FROM ip_item_spec_setup 
                WHERE brand_name = @brand AND company_code = @companyCode";

            using (var connection = new OracleConnection(_connection))
            {
                var command = new OracleCommand(query, connection);
                command.Parameters.AddWithValue("@brand", brand);
                command.Parameters.AddWithValue("@companyCode", companyCode);

                await connection.OpenAsync();
                var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    // Add item_code to the list
                    itemList.Add(reader["item_code"].ToString());
                }
            }
            return itemList;
        }
        [HttpGet]
        public async Task<IHttpActionResult> CatWiseGrp(string companyCode, string cat)
        {
            var result = new Dictionary<string, object>();

            using (var connection = new OracleConnection(_connection))
            {
                await connection.OpenAsync();

                var query = @"
                SELECT ITEM_EDESC, master_ITEM_CODE 
                FROM IP_ITEM_MASTER_SETUP 
                WHERE GROUP_SKU_FLAG = 'G' 
                  AND company_code = :companyCode 
                  AND category_code = :categoryCode 
                ORDER BY ITEM_EDESC";

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter("companyCode", companyCode));
                    command.Parameters.Add(new OracleParameter("categoryCode", cat));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var data = new Dictionary<string, string>();
                        while (await reader.ReadAsync())
                        {
                            data[reader.GetString(0)] = reader.GetString(1);
                        }
                        result["grp"] = data;
                    }
                }
            }
            return Ok(result);
        }


        [HttpGet()]
        public async Task<IHttpActionResult> GetBranchFilters(string companyCode, [FromBody] List<string> branchCode, string userNo)
        {
            var filteredBranchCodes = await GetBranchFilterFromDatabase(companyCode, branchCode, userNo);
            return Ok(filteredBranchCodes);
        }

        private async Task<List<string>> GetBranchFilterFromDatabase(string companyCode, List<string> branchCode, string userNo)
        {
            List<string> branchCodes = new List<string>();

            if (branchCode == null || branchCode.Count == 0)
            {
                // If branchCode is empty, fetch from the database
                string query = $@"
                    SELECT branch_code 
                    FROM SC_BRANCH_CONTROL 
                    WHERE user_no = @userNo AND company_code IN ({companyCode})";

                using (var connection = new OracleConnection(_connection))
                {
                    var command = new OracleCommand(query, connection);
                    command.Parameters.AddWithValue("@userNo", userNo);

                    await connection.OpenAsync();
                    var reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        branchCodes.Add(reader["branch_code"].ToString());
                    }
                }
            }
            else if (branchCode.Count == 1)
            {
                // If branchCode has one element, append "0" and convert to tuple-like structure
                branchCode.Add("0");
                branchCodes.AddRange(branchCode);
            }
            else
            {
                // If branchCode has more than one element, return it as a tuple-like structure
                branchCodes.AddRange(branchCode);
            }

            return branchCodes;
        }

        [HttpGet()]
        public async Task<IHttpActionResult> BranchFilters(string companyCode, [FromBody] List<string> branchCode, string userNo)
        {
            var filteredBranchCode = await FilterBranchCode(companyCode, branchCode, userNo);
            return Ok(filteredBranchCode);
        }

        private async Task<List<string>> FilterBranchCode(string companyCode, List<string> branchCode, string userNo)
        {
            List<string> branchCodes = new List<string>();

            if (branchCode == null || branchCode.Count == 0)
            {
                // If branchCode is empty, fetch from the database using a SQL query
                string query = $@"
                    SELECT branch_code 
                    FROM SC_BRANCH_CONTROL 
                    WHERE user_no = @userNo AND company_code IN ({companyCode})";

                using (var connection = new OracleConnection(_connection))
                {
                    var command = new OracleCommand(query, connection);
                    command.Parameters.AddWithValue("@userNo", userNo);

                    await connection.OpenAsync();
                    var reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        branchCodes.Add(reader["branch_code"].ToString());
                    }
                }
            }
            else if (branchCode.Count == 1)
            {
                // If there's only one branch code, append "0" and handle it as a tuple-like list
                branchCode.Add("0");
                branchCodes.AddRange(branchCode);
            }
            else
            {
                // If multiple branch codes are provided, handle them directly
                branchCodes.AddRange(branchCode);
            }

            return branchCodes;
        }
        public List<string> ForBranchFilterNamesAsync(List<string> sendedBranchCode, string userNo, string companyCode)
        {
            string query;
            List<string> branches = new List<string>();

            if (sendedBranchCode.Count == 0)
            {
                // If no branch code is sent, use a subquery with user number and company code
                query = $@"
                SELECT branch_edesc FROM FA_BRANCH_SETUP 
                WHERE branch_code IN (
                    SELECT branch_code FROM SC_BRANCH_CONTROL 
                    WHERE user_no = '{userNo}' AND company_code = '{companyCode}'
                )";
            }
            else if (sendedBranchCode.Count == 1)
            {
                // Add "0" to the list and create a query for single branch
                sendedBranchCode.Add("0");
                string branchCodeList = $"('{string.Join("', '", sendedBranchCode)}')";

                query = $@"
                SELECT branch_edesc FROM FA_BRANCH_SETUP 
                WHERE branch_code IN {branchCodeList} 
                AND company_code = '{companyCode}'";
            }
            else
            {
                // Handle multiple branch codes
                string branchCodeList = $"('{string.Join("', '", sendedBranchCode)}')";

                query = $@"
                SELECT branch_edesc FROM FA_BRANCH_SETUP 
                WHERE branch_code IN {branchCodeList} 
                AND company_code = '{companyCode}'";
            }
            using (var connection = new OracleConnection(_connection))
            {
                // Execute the query
                using (OracleCommand cmd = new OracleCommand(query, connection))
                {
                    connection.Open();
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            branches.Add(reader.GetString(0));
                        }
                    }
                    connection.Close();
                }
            }

            return branches;
        }

        [HttpGet()]
        public async Task<IHttpActionResult> ItemWiseStockReport(
           string companyCode,
           [FromBody] List<string> branchCode,
           [FromBody] List<string> locationCode,
           [FromBody] List<string> itemCode,
           string userNo,
           string brand,
           string cat,
           string group)
        {
            var stockData = await GetItemWiseStockReport(companyCode, branchCode, locationCode, itemCode, userNo, brand, cat, group);
            return Ok(stockData);
        }

        private async Task<List<Dictionary<string, object>>> GetItemWiseStockReport(
            string companyCode,
            List<string> branchCode,
            List<string> locationCode,
            List<string> itemCode,
            string userNo,
            string brand,
            string cat,
            string group)
        {
            List<Dictionary<string, object>> stockReport = new List<Dictionary<string, object>>();
            string brandStr = string.Empty;
            string catStr = string.Empty;
            string groupStr = string.Empty;
            string itemStringFilter = string.Empty;
            string locationStringFilter = string.Empty;

            // Brand filter
            if (!string.IsNullOrEmpty(brand))
            {
                brandStr = $@"
                    AND item_code IN (SELECT item_code FROM IP_ITEM_SPEC_SETUP WHERE brand_name = '{brand}')
                ";
            }

            // Group filter
            if (!string.IsNullOrEmpty(group))
            {
                groupStr = $@"
                    AND item_code IN (SELECT item_code FROM ip_item_master_setup WHERE master_item_code LIKE '%{group}%')
                ";
            }

            // Item Code Filter
            if (itemCode.Count == 0)
            {
                itemStringFilter = string.Empty;
            }
            else if (itemCode.Count == 1)
            {
                itemCode.Add("0");
                itemStringFilter = $" AND item_code IN ({string.Join(",", itemCode.Select(i => $"'{i}'"))}) ";
            }
            else
            {
                itemStringFilter = $" AND item_code IN ({string.Join(",", itemCode.Select(i => $"'{i}'"))}) ";
            }

            // Category filter
            if (!string.IsNullOrEmpty(cat))
            {
                if (itemCode.Count == 0)
                {
                    itemStringFilter = $@"
                        AND item_code IN (SELECT item_code FROM IP_ITEM_MASTER_SETUP WHERE category_code = '{cat}' AND company_code = '{companyCode}')
                    ";
                }
                else
                {
                    itemStringFilter = $@"
                        AND item_code IN (SELECT item_code FROM IP_ITEM_MASTER_SETUP WHERE category_code = '{cat}' AND item_code IN ({string.Join(",", itemCode.Select(i => $"'{i}'"))}) AND company_code = '{companyCode}')
                    ";
                }
            }

            // Location Code Filter
            if (locationCode.Count == 0)
            {
                locationStringFilter = string.Empty;
            }
            else if (locationCode.Count == 1)
            {
                locationCode.Add("0");
                locationStringFilter = $" AND location_code IN ({string.Join(",", locationCode.Select(l => $"'{l}'"))}) ";
            }
            else
            {
                locationStringFilter = $" AND location_code IN ({string.Join(",", locationCode.Select(l => $"'{l}'"))}) ";
            }

            string query = $@"
                SELECT item_edesc, total_stock, location, branch 
                FROM TEST_STOCK_VIEW 
                WHERE company_code = '{companyCode}'
                AND branch_code IN ({await GetBranchFilters(companyCode, branchCode, userNo)})
                {itemStringFilter} {locationStringFilter} {brandStr} {catStr} {groupStr}
            ";

            using (var connection = new OracleConnection(_connection))
            {
                await connection.OpenAsync();
                using (var command = new OracleCommand(query, connection))
                {
                    var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var stockRecord = new Dictionary<string, object>
                        {
                            {"name", reader["item_edesc"]},
                            {"cls_stock", reader["total_stock"]},
                            {"address", reader["location"]},
                            {"branch", reader["branch"]}
                        };
                        stockReport.Add(stockRecord);
                    }
                }
            }

            return stockReport;
        }
        [HttpGet()]
        public async Task<IHttpActionResult> GetAllLocations(string companyCode)
        {
            var locations = await GetLocations(companyCode);
            return Ok(locations);
        }

        private async Task<Dictionary<string, string>> GetLocations(string companyCode)
        {
            Dictionary<string, string> locationData = new Dictionary<string, string>();

            string query = $@"
                SELECT location_code, LOCATION_EDESC 
                FROM IP_LOCATION_SETUP 
                WHERE company_code = :companyCode 
                AND GROUP_SKU_FLAG = 'I' 
                AND deleted_flag = 'N' 
                ORDER BY LOCATION_EDESC
            ";

            using (var connection = new OracleConnection(_connection))
            {
                await connection.OpenAsync();

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter("companyCode", companyCode));

                    var reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        string locationCode = reader["location_code"].ToString();
                        string locationDesc = reader["LOCATION_EDESC"].ToString();
                        locationData.Add(locationDesc, locationCode);
                    }
                }
            }

            return locationData;
        }

        //[HttpPost()]
        //public IHttpActionResult RemoveWilds([FromBody] string input)
        //{
        //    string sanitizedString = RemoveWildCharacters(input);
        //    return Ok(sanitizedString);
        //}
        //private string RemoveWildCharacters(string input)
        //{
        //    if (string.IsNullOrEmpty(input)) return input;

        //    input = input.Replace("\\", "");
        //    input = input.Replace("\"", "");
        //    input = input.Replace("\t", "");  // Handles tab removal
        //    return input;
        //}
        private List<string> FnClosing(decimal[] drs, decimal[] crs)
        {
            List<decimal> closings = new List<decimal>();
            decimal closing = 0;

            for (int i = 0; i < drs.Length; i++)
            {
                if (i == 0)
                {
                    closing = drs[i] - crs[i];
                }
                else
                {
                    closing = closing - crs[i] + drs[i];
                }
                closings.Add(closing);
            }

            // Format the closings to two decimal places and store as string
            List<string> finalCol = new List<string>();
            foreach (var close in closings)
            {
                finalCol.Add(close.ToString("N2"));  // Format to 2 decimal places
            }

            return finalCol;
        }
        private void ExecuteNonQuery(string sql)
        {
            using (var connection = new OracleConnection(_connection))
            {
                connection.Open();
                using (var command = new OracleCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        private Dictionary<string, string> ExecuteQueryToDict(string query)
        {
            var result = new Dictionary<string, string>();
            OracleConnection con = new OracleConnection(_connection);
            con.Open();
            using (var cmd = new OracleCommand(query, con))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result[reader.GetString(1)] = reader.GetString(0);
                    }
                }
            }
            con.Close();
            return result;
        }
        private List<string> ExecuteQueryToList(string query)
        {
            var result = new List<string>();
            OracleConnection con = new OracleConnection(_connection);
            con.Open();
            using (var cmd = new OracleCommand(query, con))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(reader.GetString(0));
                    }
                }
            }
            con.Close();
            return result;
        }
        #endregion

        #region Initquery Developed By: Prem Prakash Dhakal Date: 09-19-2024
        public void CreateVDateRange()
        {
            string createViewSql = @"
            CREATE OR REPLACE FORCE VIEW V_DATE_RANGE
            (
                STARTDATE,
                ENDDATE,
                RANGENAME,
                SORTORDER
            )
            AS
            SELECT DISTINCT STARTDATE,
                            ENDDATE,
                            RANGENAME,
                            SORTORDER
                FROM (SELECT CS.AD_DATE STARTDATE,
                        (CS.AD_DATE + CS.DAYS_NO - 1) ENDDATE,
                        FN_BS_MONTH(SUBSTR(CS.BS_MONTH, -2, 2)) RANGENAME,
                        1 AS SORTORDER
                    FROM PREFERENCE_SETUP FY
                    JOIN CALENDAR_SETUP CS
                        ON CS.AD_DATE BETWEEN FY.FY_START_DATE AND FY.FY_END_DATE
                    UNION ALL
                    SELECT FY_START_DATE,
                            FY_END_DATE,
                            'This Year' DATE_NAME,
                            2 AS SORTORDER
                    FROM PREFERENCE_SETUP
                    UNION ALL
                    SELECT AD_DATE FY_START_DATE,
                            (AD_DATE + DAYS_NO - 1) FY_END_DATE,
                            'This Month' DATE_NAME,
                            3 AS SORTORDER
                    FROM CALENDAR_SETUP
                    WHERE SYSDATE BETWEEN AD_DATE AND (AD_DATE + DAYS_NO)
                    UNION ALL
                    SELECT DISTINCT
                            FIRST_VALUE(CS.AD_DATE)
                            OVER (ORDER BY CS.AD_DATE DESC, (CS.AD_DATE + CS.DAYS_NO - 1) DESC) FY_START_DATE,
                            FIRST_VALUE(CS.AD_DATE + CS.DAYS_NO - 1)
                            OVER (ORDER BY (CS.AD_DATE + CS.DAYS_NO - 1) DESC, CS.AD_DATE ASC) FY_END_DATE,
                            'Last Month' DATE_NAME,
                            4 AS SORTORDER
                    FROM PREFERENCE_SETUP FY
                    JOIN CALENDAR_SETUP CS
                        ON CS.AD_DATE BETWEEN FY.FY_START_DATE AND FY.FY_END_DATE
                        AND CS.AD_DATE < SYSDATE - DAYS_NO
                    UNION ALL
                    SELECT TRUNC((NEXT_DAY(SYSDATE, 'SUN') - 7)) FY_START_DATE,
                            TRUNC(SYSDATE) FY_END_DATE,
                            'This Week' DATE_NAME,
                            5 AS SORTORDER
                    FROM DUAL
                    UNION ALL
                    SELECT TRUNC((NEXT_DAY(SYSDATE, 'SUN') - 14)) FY_START_DATE,
                            TRUNC((NEXT_DAY(SYSDATE, 'SAT') - 7)) FY_END_DATE,
                            'Last Week' DATE_NAME,
                            6 AS SORTORDER
                    FROM DUAL
                    UNION ALL
                    SELECT DISTINCT
                            FIRST_VALUE(CS.AD_DATE)
                            OVER (ORDER BY CS.AD_DATE ASC, (CS.AD_DATE + CS.DAYS_NO - 1) DESC) FY_START_DATE,
                            FIRST_VALUE(CS.AD_DATE + CS.DAYS_NO - 1)
                            OVER (ORDER BY (CS.AD_DATE + CS.DAYS_NO - 1) DESC, CS.AD_DATE ASC) FY_END_DATE,
                            'First Quarter' DATE_NAME,
                            7 AS SORTORDER
                    FROM PREFERENCE_SETUP FY
                    JOIN CALENDAR_SETUP CS
                        ON CS.AD_DATE BETWEEN FY.FY_START_DATE AND FY.FY_END_DATE
                        AND SUBSTR(CS.BS_MONTH, -2, 2) >= '04'
                        AND SUBSTR(CS.BS_MONTH, -2, 2) < '07'
                    UNION ALL
                    SELECT DISTINCT
                            FIRST_VALUE(CS.AD_DATE)
                            OVER (ORDER BY CS.AD_DATE ASC, (CS.AD_DATE + CS.DAYS_NO - 1) DESC) FY_START_DATE,
                            FIRST_VALUE(CS.AD_DATE + CS.DAYS_NO - 1)
                            OVER (ORDER BY (CS.AD_DATE + CS.DAYS_NO - 1) DESC, CS.AD_DATE ASC) FY_END_DATE,
                            'Second Quarter' DATE_NAME,
                            8 AS SORTORDER
                    FROM PREFERENCE_SETUP FY
                    JOIN CALENDAR_SETUP CS
                        ON CS.AD_DATE BETWEEN FY.FY_START_DATE AND FY.FY_END_DATE
                        AND SUBSTR(CS.BS_MONTH, -2, 2) >= '07'
                        AND SUBSTR(CS.BS_MONTH, -2, 2) < '10'
                    UNION ALL
                    SELECT DISTINCT
                            FIRST_VALUE(CS.AD_DATE)
                            OVER (ORDER BY CS.AD_DATE ASC, (CS.AD_DATE + CS.DAYS_NO - 1) DESC) FY_START_DATE,
                            FIRST_VALUE(CS.AD_DATE + CS.DAYS_NO - 1)
                            OVER (ORDER BY (CS.AD_DATE + CS.DAYS_NO - 1) DESC, CS.AD_DATE ASC) FY_END_DATE,
                            'Third Quarter' DATE_NAME,
                            9 AS SORTORDER
                    FROM PREFERENCE_SETUP FY
                    JOIN CALENDAR_SETUP CS
                        ON CS.AD_DATE BETWEEN FY.FY_START_DATE AND FY.FY_END_DATE
                        AND SUBSTR(CS.BS_MONTH, -2, 2) >= '10'
                        AND SUBSTR(CS.BS_MONTH, -2, 2) <= '12'
                    UNION ALL
                    SELECT DISTINCT
                            FIRST_VALUE(CS.AD_DATE)
                            OVER (ORDER BY CS.AD_DATE ASC, (CS.AD_DATE + CS.DAYS_NO - 1) DESC) FY_START_DATE,
                            FIRST_VALUE(CS.AD_DATE + CS.DAYS_NO - 1)
                            OVER (ORDER BY (CS.AD_DATE + CS.DAYS_NO - 1) DESC, CS.AD_DATE ASC) FY_END_DATE,
                            'Forth Quarter' DATE_NAME,
                            10 AS SORTORDER
                    FROM PREFERENCE_SETUP FY
                    JOIN CALENDAR_SETUP CS
                        ON CS.AD_DATE BETWEEN FY.FY_START_DATE AND FY.FY_END_DATE
                        AND SUBSTR(CS.BS_MONTH, -2, 2) >= '01'
                        AND SUBSTR(CS.BS_MONTH, -2, 2) <= '03'
                    UNION ALL
                    SELECT FY_START_DATE,
                            FY_END_DATE,
                            'Custom' DATE_NAME,
                            -100 AS SORTORDER
                    FROM PREFERENCE_SETUP
                    )";
            ExecuteNonQuery(createViewSql);
        }

        public void CreateVDateRangeEng()
        {
            string createMaterializedViewSql = @"
            CREATE MATERIALIZED VIEW V_DATE_RANGE_ENG
            (
                STARTDATE,
                ENDDATE,
                RANGENAME,
                SORTORDER
            )
            AS
            SELECT fromdate startdate,
                    todate enddate,
                    month_name rangename,
                    1 SORTORDER
            FROM (
                SELECT *
                FROM (
                    SELECT month_name, MIN(dates) fromdate
                    FROM (
                        SELECT TRIM(month_name) || '-' || SUBSTR(dates, 8, 4) month_name,
                               dates
                        FROM (
                            SELECT TO_CHAR(
                                       (SELECT startdate
                                        FROM v_date_range
                                        WHERE rangename = 'This Year') + LEVEL - 1,
                                       'MONTH') AS month_name,
                                   (SELECT startdate
                                    FROM v_date_range
                                    WHERE rangename = 'This Year') + LEVEL - 1 dates
                            FROM DUAL
                            CONNECT BY LEVEL <=
                                ((SELECT enddate
                                  FROM v_date_range
                                  WHERE rangename = 'This Year') - 
                                 (SELECT startdate
                                  FROM v_date_range
                                  WHERE rangename = 'This Year')) + 1
                        )
                    )
                    GROUP BY month_name
                    ORDER BY fromdate
                ) a
                LEFT OUTER JOIN (
                    SELECT month_name mon, MAX(dates) todate
                    FROM (
                        SELECT TRIM(month_name) || '-' || SUBSTR(dates, 8, 4) month_name,
                               dates
                        FROM (
                            SELECT TO_CHAR(
                                       (SELECT startdate
                                        FROM v_date_range
                                        WHERE rangename = 'This Year') + LEVEL - 1,
                                       'MONTH') AS month_name,
                                   (SELECT startdate
                                    FROM v_date_range
                                    WHERE rangename = 'This Year') + LEVEL - 1 dates
                            FROM DUAL
                            CONNECT BY LEVEL <=
                                ((SELECT enddate
                                  FROM v_date_range
                                  WHERE rangename = 'This Year') - 
                                 (SELECT startdate
                                  FROM v_date_range
                                  WHERE rangename = 'This Year')) + 1
                        )
                    )
                    GROUP BY month_name
                    ORDER BY todate
                ) b
                ON a.month_name = b.mon
            )
        ";
            ExecuteNonQuery(createMaterializedViewSql);
        }
        #endregion

        #region Fiscal Module Developed By: Prem Prakash Dhakal Date: 09-19-2024
        [HttpGet()]
        public Dictionary<string, string> AllDb()
        {
            var allDbPass = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("db_data.txt"));
            OracleConnection con = new OracleConnection(_connection);
            con.Open();
            OracleCommand _command;
            var allMultiFy = new Dictionary<string, string>();
            foreach (var kvp in allDbPass)
            {
                var dbPass = kvp.Value;
                _command = con.CreateCommand();
                _command.CommandText = $"SELECT fn_decrypt_password('{dbPass}') FROM dual";
                var result = _command.ExecuteScalar();
                allMultiFy[kvp.Key] = result?.ToString();
            }

            return allMultiFy;
        }
        [HttpGet()]
        public Dictionary<string, Dictionary<string, double>> ProductWiseSales()
        {
            var query = @"
            SELECT ITEM_CODE, 
                   SUM(SALES_QTY) - SUM(SALES_RET_QTY) AS QTY, 
                   SUM(SALES_VALUE) - SUM(SALES_RET_VALUE) AS VAL 
            FROM (
                SELECT a.item_code,
                       SUM(NVL(A.ROLL_QTY,0)) AS SALES_ROLL_QTY, 
                       SUM(NVL(A.QUANTITY,0)) AS SALES_QTY, 
                       SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE,0)) AS SALES_VALUE,
                       0 AS SALES_RET_ROLL_QTY, 
                       0 AS SALES_RET_QTY, 
                       0 AS SALES_RET_VALUE  
                FROM SA_SALES_INVOICE A 
                WHERE A.DELETED_FLAG = 'N' 
                GROUP BY a.item_code
                UNION ALL
                SELECT a.item_code,
                       0 AS SALES_ROLL_QTY, 
                       0 AS SALES_QTY, 
                       0 AS SALES_VALUE,
                       SUM(NVL(A.ROLL_QTY,0)) AS SALES_RET_ROLL_QTY, 
                       SUM(NVL(A.QUANTITY,0)) AS SALES_RET_QTY,
                       SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE,0)) AS SALES_RET_VALUE  
                FROM SA_SALES_RETURN A 
                WHERE A.DELETED_FLAG = 'N' 
                GROUP BY a.item_code 
            ) 
            GROUP BY ITEM_CODE
        ";
            OracleConnection con = new OracleConnection(_connection);
            con.Open();

            _command.CommandText = query;
            var results = new Dictionary<string, Dictionary<string, double>>();

            using (var reader = _command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var itemCode = reader.GetString(0);
                    var qty = reader.GetDouble(1);
                    var value = reader.GetDouble(2);

                    results[itemCode] = new Dictionary<string, double>
                {
                    { "qty", qty },
                    { "value", value }
                };
                }
            }

            return results;
        }
        [HttpGet()]
        public Dictionary<string, Dictionary<string, double>> CustomerWiseSales()
        {
            var query = @"
            SELECT customer_code, 
                   SUM(SALES_QTY) - SUM(SALES_RET_QTY) AS QTY, 
                   SUM(SALES_VALUE) - SUM(SALES_RET_VALUE) AS VAL 
            FROM (
                SELECT a.customer_code,
                       SUM(NVL(A.ROLL_QTY, 0)) AS SALES_ROLL_QTY, 
                       SUM(NVL(A.QUANTITY, 0)) AS SALES_QTY, 
                       SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) AS SALES_VALUE,
                       0 AS SALES_RET_ROLL_QTY, 
                       0 AS SALES_RET_QTY, 
                       0 AS SALES_RET_VALUE  
                FROM SA_SALES_INVOICE A 
                WHERE A.DELETED_FLAG = 'N' 
                GROUP BY a.customer_code
                UNION ALL
                SELECT a.customer_code,
                       0 AS SALES_ROLL_QTY, 
                       0 AS SALES_QTY, 
                       0 AS SALES_VALUE,
                       SUM(NVL(A.ROLL_QTY, 0)) AS SALES_RET_ROLL_QTY, 
                       SUM(NVL(A.QUANTITY, 0)) AS SALES_RET_QTY,
                       SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) AS SALES_RET_VALUE  
                FROM SA_SALES_RETURN A 
                WHERE A.DELETED_FLAG = 'N' 
                GROUP BY a.customer_code
            ) 
            GROUP BY customer_code";
            OracleConnection con = new OracleConnection(_connection);
            con.Open();
            _command.CommandText = query;
            var results = new Dictionary<string, Dictionary<string, double>>();

            using (var reader = _command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var customerCode = reader.GetString(0);
                    var qty = reader.GetDouble(1);
                    var value = reader.GetDouble(2);

                    results[customerCode] = new Dictionary<string, double>
                {
                    { "qty", qty },
                    { "value", value }
                };
                }
            }

            return results;
        }

        [HttpGet()]
        public Dictionary<string, Dictionary<string, double>> EmployeeWiseSales()
        {
            var query = @"
            SELECT EMPLOYEE_CODE, 
                   SUM(SALES_QTY) - SUM(SALES_RET_QTY) AS QTY, 
                   SUM(SALES_VALUE) - SUM(SALES_RET_VALUE) AS VAL 
            FROM (
                SELECT a.EMPLOYEE_CODE,
                       SUM(NVL(A.ROLL_QTY, 0)) AS SALES_ROLL_QTY, 
                       SUM(NVL(A.QUANTITY, 0)) AS SALES_QTY, 
                       SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) AS SALES_VALUE,
                       0 AS SALES_RET_ROLL_QTY, 
                       0 AS SALES_RET_QTY, 
                       0 AS SALES_RET_VALUE  
                FROM SA_SALES_INVOICE A 
                WHERE A.DELETED_FLAG = 'N' 
                GROUP BY a.EMPLOYEE_CODE
                UNION ALL
                SELECT a.EMPLOYEE_CODE,
                       0 AS SALES_ROLL_QTY, 
                       0 AS SALES_QTY, 
                       0 AS SALES_VALUE,
                       SUM(NVL(A.ROLL_QTY, 0)) AS SALES_RET_ROLL_QTY, 
                       SUM(NVL(A.QUANTITY, 0)) AS SALES_RET_QTY,
                       SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) AS SALES_RET_VALUE  
                FROM SA_SALES_RETURN A 
                WHERE A.DELETED_FLAG = 'N' 
                GROUP BY a.EMPLOYEE_CODE
            ) 
            GROUP BY EMPLOYEE_CODE";
            OracleConnection con = new OracleConnection(_connection);
            con.Open();
            _command.CommandText = query;
            var results = new Dictionary<string, Dictionary<string, double>>();

            using (var reader = _command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var employeeCode = reader.GetString(0);
                    var qty = reader.GetDouble(1);
                    var value = reader.GetDouble(2);

                    results[employeeCode] = new Dictionary<string, double>
                {
                    { "qty", qty },
                    { "value", value }
                };
                }
            }

            return results;
        }
        [HttpGet()]
        public Dictionary<string, Dictionary<string, double>> CategoryWiseSales()
        {
            var query = @"
            SELECT category_code, 
                   SUM(qty), 
                   SUM(val) 
            FROM (
                SELECT * 
                FROM (
                    SELECT ITEM_CODE, 
                           SUM(SALES_QTY) - SUM(SALES_RET_QTY) AS QTY, 
                           SUM(SALES_VALUE) - SUM(SALES_RET_VALUE) AS VAL 
                    FROM (
                        SELECT a.item_code,
                               SUM(NVL(A.ROLL_QTY, 0)) AS SALES_ROLL_QTY, 
                               SUM(NVL(A.QUANTITY, 0)) AS SALES_QTY, 
                               SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) AS SALES_VALUE,
                               0 AS SALES_RET_ROLL_QTY, 
                               0 AS SALES_RET_QTY, 
                               0 AS SALES_RET_VALUE 
                        FROM SA_SALES_INVOICE A 
                        WHERE A.DELETED_FLAG = 'N' 
                        GROUP BY a.item_code
                        UNION ALL
                        SELECT a.item_code,
                               0 AS SALES_ROLL_QTY, 
                               0 AS SALES_QTY, 
                               0 AS SALES_VALUE,
                               SUM(NVL(A.ROLL_QTY, 0)) AS SALES_RET_ROLL_QTY, 
                               SUM(NVL(A.QUANTITY, 0)) AS SALES_RET_QTY,
                               SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) AS SALES_RET_VALUE 
                        FROM SA_SALES_RETURN A 
                        WHERE A.DELETED_FLAG = 'N' 
                        GROUP BY a.item_code
                    ) 
                    GROUP BY ITEM_CODE
                ) a 
                LEFT OUTER JOIN (
                    SELECT * 
                    FROM (
                        SELECT item_code, 
                               category_code 
                        FROM ip_item_master_setup 
                        WHERE company_code = '01' 
                        AND category_code IS NOT NULL
                    ) a 
                    LEFT OUTER JOIN (
                        SELECT category_code AS code, 
                               category_edesc 
                        FROM ip_category_code 
                        WHERE company_code = '01'
                    ) b 
                    ON a.category_code = b.code
                ) b 
                ON a.item_code = b.item_code
            ) 
            GROUP BY category_code";
            OracleConnection con = new OracleConnection(_connection);
            con.Open();
            _command.CommandText = query;
            var results = new Dictionary<string, Dictionary<string, double>>();

            using (var reader = _command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var categoryCode = reader.IsDBNull(0) ? null : reader.GetString(0);
                    var qty = reader.IsDBNull(1) ? 0 : reader.GetDouble(1);
                    var value = reader.IsDBNull(2) ? 0 : reader.GetDouble(2);

                    results[categoryCode ?? "Unknown"] = new Dictionary<string, double>
                {
                    { "qty", qty },
                    { "value", value }
                };
                }
            }
            return results;
        }
        [HttpGet()]
        public Dictionary<string, Dictionary<string, double>> MonthlySales()
        {
            var query = @"
            SELECT startdate, 
                   rangename, 
                   SUM(NVL(sales_qty, 0)) - SUM(NVL(sales_ret_qty, 0)) AS qty, 
                   SUM(NVL(sales_value, 0)) - SUM(NVL(sales_ret_value, 0)) AS value 
            FROM (
                SELECT * 
                FROM (
                    SELECT * 
                    FROM v_date_range 
                    WHERE sortorder = 1 
                    ORDER BY startdate
                ) a 
                LEFT OUTER JOIN (
                    SELECT a.sales_date,
                           SUM(NVL(A.ROLL_QTY, 0)) AS SALES_ROLL_QTY, 
                           SUM(NVL(A.QUANTITY, 0)) AS SALES_QTY, 
                           SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) AS SALES_VALUE,
                           0 AS SALES_RET_ROLL_QTY, 
                           0 AS SALES_RET_QTY, 
                           0 AS SALES_RET_VALUE 
                    FROM SA_SALES_INVOICE A 
                    WHERE A.DELETED_FLAG = 'N' 
                    GROUP BY a.sales_date
                    UNION ALL
                    SELECT a.return_date,
                           0 AS SALES_ROLL_QTY, 
                           0 AS SALES_QTY, 
                           0 AS SALES_VALUE,
                           SUM(NVL(A.ROLL_QTY, 0)) AS SALES_RET_ROLL_QTY, 
                           SUM(NVL(A.QUANTITY, 0)) AS SALES_RET_QTY,
                           SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) AS SALES_RET_VALUE 
                    FROM SA_SALES_RETURN A 
                    WHERE A.DELETED_FLAG = 'N' 
                    GROUP BY a.return_date
                ) b 
                ON b.sales_date BETWEEN a.startdate AND a.enddate
            ) 
            GROUP BY startdate, rangename 
            ORDER BY 1";
            OracleConnection con = new OracleConnection(_connection);
            con.Open();
            _command.CommandText = query;
            var results = new Dictionary<string, Dictionary<string, double>>();

            using (var reader = _command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var rangeName = reader.IsDBNull(1) ? null : reader.GetString(1);
                    var qty = reader.IsDBNull(2) ? 0 : reader.GetDouble(2);
                    var value = reader.IsDBNull(3) ? 0 : reader.GetDouble(3);

                    results[rangeName ?? "Unknown"] = new Dictionary<string, double>
                {
                    { "qty", qty },
                    { "value", value }
                };
                }
            }
            return results;
        }
        [HttpGet()]
        public Dictionary<string, Dictionary<string, string>> Products(Dictionary<string, List<string>> data)
        {
            int x = 0;
            string loopQuery = "";
            string firstDb = "";

            foreach (var db in data["comparison"])
            {
                x++;
                if (x == 1)
                {
                    firstDb = db;
                }
                else
                {
                    loopQuery += $@" UNION ALL
                                SELECT item_code, item_edesc FROM {db}.ip_item_master_setup 
                                WHERE company_code = '01' AND deleted_flag = 'N'";
                }
            }

            string query = $@"
                SELECT DISTINCT item_code, item_edesc FROM (
                    SELECT item_code, item_edesc FROM {firstDb}.ip_item_master_setup 
                    WHERE company_code = '01' AND deleted_flag = 'N'
                    {loopQuery}
                )";
            OracleConnection con = new OracleConnection(_connection);
            con.Open();
            _command.CommandText = query;
            var result = new Dictionary<string, Dictionary<string, string>>();

            using (var reader = _command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result[reader.GetString(0)] = new Dictionary<string, string>
                {
                    { "name", reader.GetString(1) }
                };
                }
            }

            return result;
        }
        [HttpGet()]
        public Dictionary<string, Dictionary<string, string>> Customers(Dictionary<string, List<string>> data)
        {
            int x = 0;
            string loopQuery = "";
            string firstDb = "";

            foreach (var db in data["comparison"])
            {
                x++;
                if (x == 1)
                {
                    firstDb = db;
                }
                else
                {
                    loopQuery += $@" UNION ALL
                                SELECT customer_code, customer_edesc FROM {db}.sa_customer_setup 
                                WHERE company_code = '01' AND deleted_flag = 'N'";
                }
            }
            string query = $@"
                SELECT DISTINCT customer_code, customer_edesc FROM (
                    SELECT customer_code, customer_edesc FROM {firstDb}.sa_customer_setup 
                    WHERE company_code = '01' AND deleted_flag = 'N'
                    {loopQuery}
                )";
            OracleConnection con = new OracleConnection(_connection);
            con.Open();
            _command.CommandText = query;
            var result = new Dictionary<string, Dictionary<string, string>>();

            using (var reader = _command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result[reader.GetString(0)] = new Dictionary<string, string>
                {
                    { "name", reader.GetString(1) }
                };
                }
            }
            return result;
        }
        [HttpGet()]
        public Dictionary<string, Dictionary<string, string>> Employees(Dictionary<string, List<string>> data)
        {
            int x = 0;
            string loopQuery = "";
            string firstDb = "";

            foreach (var db in data["comparison"])
            {
                x++;
                if (x == 1)
                {
                    firstDb = db;
                }
                else
                {
                    loopQuery += $@" UNION ALL
                                SELECT EMPLOYEE_CODE, EMPLOYEE_edesc FROM {db}.HR_EMPLOYEE_SETUP 
                                WHERE company_code = '01' AND deleted_flag = 'N'";
                }
            }
            string query = $@"
                SELECT DISTINCT EMPLOYEE_CODE, EMPLOYEE_edesc FROM (
                    SELECT EMPLOYEE_CODE, EMPLOYEE_edesc FROM {firstDb}.HR_EMPLOYEE_SETUP 
                    WHERE company_code = '01' AND deleted_flag = 'N'
                    {loopQuery})";
            OracleConnection con = new OracleConnection(_connection);
            con.Open();
            _command.CommandText = query;
            var result = new Dictionary<string, Dictionary<string, string>>();

            using (var reader = _command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result[reader.GetString(0)] = new Dictionary<string, string>
                {
                    { "name", reader.GetString(1) }
                };
                }
            }

            return result;
        }
        [HttpGet()]
        public Dictionary<string, Dictionary<string, string>> Categories(Dictionary<string, List<string>> data)
        {
            int x = 0;
            string loopQuery = "";
            string firstDb = "";

            foreach (var db in data["comparison"])
            {
                x++;
                if (x == 1)
                {
                    firstDb = db;
                }
                else
                {
                    loopQuery += $@" UNION ALL
                                SELECT category_code, category_edesc FROM {db}.ip_category_code 
                                WHERE company_code = '01' AND deleted_flag = 'N'";
                }
            }

            string query = $@"
                SELECT DISTINCT category_code, category_edesc FROM (
                    SELECT category_code, category_edesc FROM {firstDb}.ip_category_code 
                    WHERE company_code = '01' AND deleted_flag = 'N'
                    {loopQuery}
                )";
            OracleConnection con = new OracleConnection(_connection);
            con.Open();
            _command.CommandText = query;
            var result = new Dictionary<string, Dictionary<string, string>>();

            using (var reader = _command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result[reader.GetString(0)] = new Dictionary<string, string>
                {
                    { "name", reader.GetString(1) }
                };
                }
            }

            return result;
        }
        [HttpGet()]
        public Dictionary<string, Dictionary<string, string>> Month()
        {
            string query = @"
                SELECT rangename FROM v_date_range 
                WHERE sortorder = 1 
                ORDER BY startdate";
            OracleConnection con = new OracleConnection(_connection);
            con.Open();
            _command.CommandText = query;
            var result = new Dictionary<string, Dictionary<string, string>>();

            using (var reader = _command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result[reader.GetString(0)] = new Dictionary<string, string>
                {
                    { "name", reader.GetString(0) }
                };
                }
            }

            return result;
        }
        //[HttpGet("all-sub-ledger/{companyCode}")]
        [HttpGet()]
        public async Task<IHttpActionResult> GetAllSubLedger(string companyCode)
        {
            var sendRes = new Dictionary<string, object>();

            // Fetch category data
            var categoryData = await ExecuteQueryAsync("SELECT category_code, CATEGORY_EDESC FROM IP_CATEGORY_CODE ORDER BY CATEGORY_EDESC ASC");
            var catDict = new Dictionary<string, string>();
            foreach (DataRow row in categoryData.Rows)
            {
                catDict[row["CATEGORY_EDESC"].ToString()] = row["category_code"].ToString();
            }
            sendRes["cat"] = catDict;

            // Fetch group SKU items
            var itemData = await ExecuteQueryAsync($"SELECT ITEM_EDESC, master_ITEM_CODE FROM IP_ITEM_MASTER_SETUP WHERE GROUP_SKU_FLAG='G' AND company_code='{companyCode}' ORDER BY ITEM_EDESC");
            var itemDict = new Dictionary<string, string>();
            foreach (DataRow row in itemData.Rows)
            {
                itemDict[row["ITEM_EDESC"].ToString()] = row["master_ITEM_CODE"].ToString();
            }
            sendRes["grp"] = itemDict;

            // Fetch group SKU customers
            var customerData = await ExecuteQueryAsync($"SELECT CUSTOMER_EDESC, master_CUSTOMER_CODE FROM SA_CUSTOMER_SETUP WHERE GROUP_SKU_FLAG='G' AND company_code='{companyCode}' ORDER BY CUSTOMER_EDESC");
            var customerDict = new Dictionary<string, string>();
            foreach (DataRow row in customerData.Rows)
            {
                customerDict[row["CUSTOMER_EDESC"].ToString()] = row["master_CUSTOMER_CODE"].ToString();
            }
            sendRes["cus_grp"] = customerDict;

            // Fetch brand data
            var brandData = await ExecuteQueryAsync($"SELECT DISTINCT brand_name FROM ip_item_spec_setup WHERE brand_name IS NOT NULL AND brand_name NOT IN (' ') AND company_code='{companyCode}' ORDER BY brand_name");
            var brands = new List<string>();
            foreach (DataRow row in brandData.Rows)
            {
                brands.Add(row["brand_name"].ToString());
            }
            sendRes["brand"] = brands;

            // Fetch unit data
            var unitQuery = @"
                SELECT DISTINCT index_mu_code FROM (
                    SELECT item_code, index_mu_code, 1 AS ratio FROM ip_item_master_setup 
                    WHERE index_mu_code IS NOT NULL 
                    AND INDEX_MU_CODE != 'None' 
                    AND GROUP_SKU_FLAG='I' 
                    AND deleted_flag='N'
                    UNION ALL
                    SELECT item_code, mu_code, 
                        CASE WHEN conversion_factor = 0 THEN 0 ELSE ISNULL(conversion_factor, 0) / ISNULL(fraction, 0) END AS ratio 
                    FROM IP_ITEM_UNIT_SETUP
                    WHERE serial_no='1' AND deleted_flag='N'
                    UNION ALL
                    SELECT item_code, mu_code, 
                        CASE WHEN conversion_factor = 0 THEN 0 ELSE ISNULL(conversion_factor, 0) / ISNULL(fraction, 0) END AS ratio 
                    FROM IP_ITEM_UNIT_SETUP
                    WHERE serial_no='1' AND deleted_flag='N'
                )";
            var unitData = await ExecuteQueryAsync(unitQuery);
            var units = new List<string>();
            foreach (DataRow row in unitData.Rows)
            {
                units.Add(row["index_mu_code"].ToString());
            }
            sendRes["unit"] = units;

            return Ok(sendRes);
        }
        #endregion

        #region Data Module Developed By: Prem Prakash Dhakal Date: 09-19-2024
        [HttpGet()]
        public async Task<IHttpActionResult> GetOverview([FromBody] OverviewRequestModel model)
        {
            try
            {
                var response = new List<Dictionary<string, object>>();
                var fromDate = model.FromDate;
                var toDate = model.ToDate;
                var companyCode = model.CompanyCode;

                // Fetch the default date range if fromDate is null or empty
                if (string.IsNullOrEmpty(fromDate))
                {
                    var query = @"SELECT 
                            TO_CHAR(startdate, 'DD-Mon-YYYY') AS startdate,
                            TO_CHAR(enddate, 'DD-Mon-YYYY') AS enddate
                          FROM v_date_range
                          WHERE sortorder = 2";

                    var resultTable = await ExecuteQueryAsync(query);
                    if (resultTable.Rows.Count > 0)
                    {
                        fromDate = resultTable.Rows[0]["startdate"].ToString();
                        toDate = resultTable.Rows[0]["enddate"].ToString();
                    }
                }

                // Ensure date range is provided
                if (string.IsNullOrEmpty(fromDate) || string.IsNullOrEmpty(toDate))
                {
                    return BadRequest("Invalid date range.");
                }

                // Query 1: Sales and Sales Return Data
                string salesQuery = $@"
            SELECT NVL(SUM(sales_qty) - SUM(sales_ret_qty), 0) qty,
                   SUM(sales_value) - SUM(sales_ret_value) amt
            FROM (
                SELECT 
                    SUM(NVL(A.QUANTITY, 0)) SALES_QTY, 
                    SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) SALES_VALUE,
                    0 SALES_RET_QTY, 
                    0 SALES_RET_VALUE  
                FROM SA_SALES_INVOICE A 
                WHERE A.DELETED_FLAG = 'N' 
                AND A.SALES_DATE BETWEEN :fromDate AND :toDate
                AND A.COMPANY_CODE = :companyCode                                                
                UNION ALL
                SELECT 
                    0 SALES_QTY, 
                    0 SALES_VALUE,
                    SUM(NVL(A.QUANTITY, 0)) SALES_RET_QTY,
                    SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) SALES_RET_VALUE
                FROM SA_SALES_RETURN A
                WHERE A.DELETED_FLAG = 'N'
                AND A.RETURN_DATE BETWEEN :fromDate AND :toDate
                AND A.COMPANY_CODE = :companyCode
            )";

                // Use parameterized query to prevent SQL injection
                var salesParams = new Dictionary<string, object>
                {
                    { ":fromDate", fromDate },
                    { ":toDate", toDate },
                    { ":companyCode", companyCode }
                };

                var salesResultTable = await ExecuteQueryAsync(salesQuery, salesParams);

                foreach (DataRow row in salesResultTable.Rows)
                {
                    var vals = new Dictionary<string, object>
                    {
                        ["qty"] = Convert.ToDouble(row["qty"]),
                        ["amt"] = Convert.ToDouble(row["amt"]),
                        ["topic"] = "Sales"
                    };
                    response.Add(vals);
                }

                // Query 2: Category and Inventory Data
                var categoryQuery = @"
                    SELECT ISNULL(category_edesc, 'N/a') AS category_edesc, qty, amt 
                    FROM 
                    (
                        SELECT category_code, 
                               SUM(ISNULL(closing_qty, 0)) AS qty, 
                               SUM(ISNULL(closing_amt, 0)) AS amt 
                        FROM inventory_summary
                        WHERE closing_date BETWEEN :fromDate AND :toDate
                        AND company_code = :companyCode
                        GROUP BY category_code
                    )";

                // Use parameterized query for the second query
                var categoryParams = new Dictionary<string, object>
                {
                    { ":fromDate", fromDate },
                    { ":toDate", toDate },
                    { ":companyCode", companyCode }
                };

                var categoryResultTable = await ExecuteQueryAsync(categoryQuery, categoryParams);
                foreach (DataRow row in categoryResultTable.Rows)
                {
                    var categoryDesc = row["category_edesc"].ToString();
                    var qty = Convert.ToDecimal(row["qty"]);
                    var amt = Convert.ToDecimal(row["amt"]);

                    response.Add(new Dictionary<string, object>
                    {
                        ["category"] = categoryDesc,
                        ["qty"] = qty,
                        ["amt"] = amt,
                        ["topic"] = "Inventory"
                    });
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        private string DataTableToJson(DataTable dataTable)
        {
            return JsonConvert.SerializeObject(dataTable, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                Formatting = Formatting.Indented
            });
        }
        [HttpGet()]
        public async Task<IHttpActionResult> GetFinancialDataAsync(string fromDate, string toDate, string companyCode)
        {
            var response = new List<Dictionary<string, object>>();

            try
            {
                using (var connection = new OracleConnection(_connection))
                {
                    await connection.OpenAsync();

                    // Collection Query
                    var collectionQuery = @"
                        SELECT NVL(SUM(NVL(CR_AMOUNT,0) * NVL(EXCHANGE_RATE,1)),0) CR_AMOUNT
                        FROM V$VIRTUAL_SUB_LEDGER b
                        WHERE (COMPANY_CODE, Voucher_NO) IN
                        (
                            SELECT COMPANY_CODE, A.voucher_no
                            FROM V$VIRTUAL_GENERAL_LEDGER A
                            WHERE A.ACC_CODE IN 
                            (
                                SELECT ACC_CODE 
                                FROM FA_CHART_OF_ACCOUNTS_SETUP 
                                WHERE ACC_NATURE IN ('AB','AC','LC') 
                                AND COMPANY_CODE = A.COMPANY_CODE
                            )
                            AND A.TRANSACTION_TYPE = 'DR'                                        
                            AND A.COMPANY_CODE= :companyCode
                            AND A.DELETED_FLAG = 'N'
                            AND A.Voucher_NO != '0'
                            AND voucher_date BETWEEN :fromDate AND :toDate
                        )
                        AND SUBSTR(sub_code,1,1)='C'
                        AND TRANSACTION_TYPE = 'CR'
                        ";

                    using (var command = new OracleCommand(collectionQuery, connection))
                    {
                        command.Parameters.Add(new OracleParameter(":companyCode", companyCode));
                        command.Parameters.Add(new OracleParameter(":fromDate", fromDate));
                        command.Parameters.Add(new OracleParameter(":toDate", toDate));

                        var crAmount = await command.ExecuteScalarAsync();
                        var vals = new Dictionary<string, object>
                    {
                        { "amt", Convert.ToDouble(crAmount) },
                        { "topic", "Collection" }
                    };
                        response.Add(vals);
                    }

                    // Purchase Query
                    var purchaseQuery = @"
                        SELECT NVL(SUM(NVL(QUANTITY,0)), 0) QTY
                        FROM IP_Purchase_MRR 
                        WHERE DELETED_FLAG='N' 
                        AND COMPANY_CODE = :companyCode
                        AND mrr_date BETWEEN :fromDate AND :toDate
                        ";

                    using (var command = new OracleCommand(purchaseQuery, connection))
                    {
                        command.Parameters.Add(new OracleParameter(":companyCode", companyCode));
                        command.Parameters.Add(new OracleParameter(":fromDate", fromDate));
                        command.Parameters.Add(new OracleParameter(":toDate", toDate));

                        var qty = await command.ExecuteScalarAsync();
                        if (qty != null)
                        {
                            var vals = new Dictionary<string, object>
                        {
                            { "qty", Convert.ToDouble(qty) },
                            { "topic", "Purchase" }
                        };
                            response.Add(vals);
                        }
                    }

                    // Detailed Query
                    var detailedQuery = @"
                        SELECT SALES_ROLL_QTY, 
                               SALES_QTY, SALES_VALUE, SALES_RET_ROLL_QTY,
                               SALES_RET_QTY, SALES_RET_VALUE, SALES_ROLL_QTY - SALES_RET_ROLL_QTY NET_ROLL_QTY,
                               NVL(SALES_QTY - SALES_RET_QTY,0) NET_SALES_QTY, NVL(SALES_VALUE - SALES_RET_VALUE,0) NET_SALES_VALUE
                        FROM ( 
                            SELECT SUM(A.SALES_ROLL_QTY) SALES_ROLL_QTY, SUM(A.SALES_QTY) SALES_QTY,  
                                   SUM(A.SALES_VALUE) SALES_VALUE, SUM(SALES_RET_ROLL_QTY) SALES_RET_ROLL_QTY, 
                                   SUM(A.SALES_RET_QTY) SALES_RET_QTY, SUM(A.SALES_RET_VALUE) SALES_RET_VALUE  
                            FROM (
                                SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.ITEM_CODE, 
                                       SUM(NVL(A.ROLL_QTY,0)) SALES_ROLL_QTY, 
                                       SUM(NVL(A.QUANTITY,0)) SALES_QTY, 
                                       SUM(NVL(A.QUANTITY*A.CALC_UNIT_PRICE*EXCHANGE_RATE,0)) SALES_VALUE, 
                                       0 SALES_RET_ROLL_QTY, 0 SALES_RET_QTY, 
                                       0 SALES_RET_VALUE  
                                FROM IP_PURCHASE_INVOICE A 
                                WHERE company_code = :companyCode 
                                AND deleted_flag = 'N'  
                                AND TRUNC(invoice_DATE) BETWEEN :fromDate AND :toDate
                                GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.ITEM_CODE 
                                UNION ALL 
                                SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.ITEM_CODE, 
                                       0 SALES_ROLL_QTY, 0 SALES_QTY, 0 SALES_VALUE, 
                                       SUM(NVL(A.ROLL_QTY,0)) SALES_RET_ROLL_QTY, 
                                       SUM(NVL(A.QUANTITY,0)) SALES_RET_QTY,  
                                       SUM(NVL(A.QUANTITY*A.CALC_UNIT_PRICE*EXCHANGE_RATE,0)) SALES_RET_VALUE  
                                FROM IP_PURCHASE_RETURN A 
                                WHERE company_code = :companyCode 
                                AND deleted_flag = 'N'  
                                AND TRUNC(A.RETURN_DATE) BETWEEN :fromDate AND :toDate
                                GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.ITEM_CODE 
                            ) A
                        )
                        ";

                    using (var command = new OracleCommand(detailedQuery, connection))
                    {
                        command.Parameters.Add(new OracleParameter(":companyCode", companyCode));
                        command.Parameters.Add(new OracleParameter(":fromDate", fromDate));
                        command.Parameters.Add(new OracleParameter(":toDate", toDate));

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var qty = Convert.ToDouble(reader["NET_ROLL_QTY"]);
                                var amt = Convert.ToDouble(reader["NET_SALES_VALUE"]);
                                var vals = new Dictionary<string, object>
                            {
                                { "qty", qty },
                                { "amt", amt },
                                { "topic", "Sales" }
                            };
                                response.Add(vals);
                            }
                        }
                    }

                    // Receivable Query
                    var receivableQuery = @"
                        SELECT NVL(SUM(a),0) 
                        FROM (
                            SELECT SUM(dr_amount) - SUM(cr_amount) a 
                            FROM V$VIRTUAL_GENERAL_LEDGER
                            WHERE deleted_flag = 'N'
                            AND company_code = :companyCode
                            AND acc_code IN (
                                SELECT DISTINCT acc_code 
                                FROM FA_CHART_OF_ACCOUNTS_SETUP
                                WHERE DELETED_FLAG = 'N' 
                                AND acc_nature IN ('AE') 
                                AND COMPANY_CODE = :companyCode
                                AND acc_type_flag = 'T'
                            )
                            AND (form_code = 0 OR voucher_date < :fromDate)
                            UNION ALL
                            SELECT SUM(dr_amount) - SUM(cr_amount) 
                            FROM V$VIRTUAL_GENERAL_LEDGER
                            WHERE deleted_flag = 'N'
                            AND company_code = :companyCode
                            AND acc_code IN (
                                SELECT DISTINCT acc_code 
                                FROM FA_CHART_OF_ACCOUNTS_SETUP
                                WHERE DELETED_FLAG = 'N' 
                                AND acc_nature IN ('AE') 
                                AND COMPANY_CODE = :companyCode
                                AND acc_type_flag = 'T'
                            )
                            AND form_code <> 0
                            AND voucher_date BETWEEN :fromDate AND :toDate
                        )";

                    using (var command = new OracleCommand(receivableQuery, connection))
                    {
                        command.Parameters.Add(new OracleParameter(":companyCode", companyCode));
                        command.Parameters.Add(new OracleParameter(":fromDate", fromDate));
                        command.Parameters.Add(new OracleParameter(":toDate", toDate));

                        var receivableAmount = await command.ExecuteScalarAsync();
                        response.Add(new Dictionary<string, object>
                    {
                        { "topic", "Receivable" },
                        { "amount", receivableAmount }
                    });
                    }

                    // Payable Query
                    var payableQuery = @"
                    SELECT NVL(SUM(a),0)
                    FROM (
                        SELECT SUM(dr_amount) - SUM(cr_amount) a
                        FROM V$VIRTUAL_GENERAL_LEDGER
                        WHERE deleted_flag = 'N'
                        AND company_code = :companyCode
                        AND acc_code IN (
                            SELECT DISTINCT acc_code 
                            FROM FA_CHART_OF_ACCOUNTS_SETUP
                            WHERE DELETED_FLAG = 'N' 
                            AND acc_nature IN ('LD')
                            AND COMPANY_CODE = :companyCode
                            AND acc_type_flag = 'T'
                        )
                        AND form_code = 0
                        AND voucher_date <= :fromDate
                        UNION ALL
                        SELECT SUM(dr_amount) - SUM(cr_amount)
                        FROM V$VIRTUAL_GENERAL_LEDGER
                        WHERE deleted_flag = 'N'
                        AND company_code = :companyCode
                        AND acc_code IN (
                            SELECT DISTINCT acc_code 
                            FROM FA_CHART_OF_ACCOUNTS_SETUP
                            WHERE DELETED_FLAG = 'N' 
                            AND acc_nature IN ('LD')
                            AND COMPANY_CODE = :companyCode
                            AND acc_type_flag = 'T'
                        )
                        AND form_code <> 0
                        AND voucher_date BETWEEN :fromDate AND :toDate
                    )";

                    using (var command = new OracleCommand(payableQuery, connection))
                    {
                        command.Parameters.Add(new OracleParameter(":companyCode", companyCode));
                        command.Parameters.Add(new OracleParameter(":fromDate", fromDate));
                        command.Parameters.Add(new OracleParameter(":toDate", toDate));

                        var payableAmount = await command.ExecuteScalarAsync();
                        response.Add(new Dictionary<string, object>
                    {
                        { "topic", "Payable" },
                        { "amount", payableAmount }
                    });
                    }

                    // VAT Query
                    var vatQuery = @"
                        SELECT NVL(SUM(VAT_AMOUNT), 0) 
                        FROM VAT_TABLE
                        WHERE company_code = :companyCode
                        AND voucher_date BETWEEN :fromDate AND :toDate
                        ";

                    using (var command = new OracleCommand(vatQuery, connection))
                    {
                        command.Parameters.Add(new OracleParameter(":companyCode", companyCode));
                        command.Parameters.Add(new OracleParameter(":fromDate", fromDate));
                        command.Parameters.Add(new OracleParameter(":toDate", toDate));

                        var vatAmount = await command.ExecuteScalarAsync();
                        response.Add(new Dictionary<string, object>
                    {
                        { "topic", "VAT" },
                        { "amount", vatAmount }
                    });
                    }

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> OverviewDirectExpenses(
            [FromBody] DateTime fromDate,
            [FromBody] DateTime toDate,
            [FromBody] string companyCode)
        {
            var summary = new DirectExpenseSummary
            {
                Topic = "Direct Expenses",
                Details = new List<DirectExpenseDetail>()
            };

            try
            {
                using (var connection = new OracleConnection(_connection))
                {
                    await connection.OpenAsync();
                    var query = @"
                    SELECT acc_code, NVL(SUM(a), 0) AS aa 
                    FROM (
                        SELECT acc_code, SUM(dr_amount) - SUM(cr_amount) AS a
                        FROM V$VIRTUAL_GENERAL_LEDGER
                        WHERE deleted_flag = 'N'
                          AND company_code = :CompanyCode
                          AND acc_code IN (
                              SELECT DISTINCT acc_code
                              FROM FA_CHART_OF_ACCOUNTS_SETUP
                              WHERE DELETED_FLAG = 'N'
                                AND acc_nature IN ('EB')
                                AND company_code = :CompanyCode
                                AND acc_type_flag = 'T')
                          AND (form_code = 0 OR voucher_date < :FromDate)
                        GROUP BY acc_code
                        UNION ALL
                        SELECT acc_code, SUM(dr_amount) - SUM(cr_amount)
                        FROM V$VIRTUAL_GENERAL_LEDGER
                        WHERE deleted_flag = 'N'
                          AND company_code = :CompanyCode
                          AND acc_code IN (
                              SELECT DISTINCT acc_code
                              FROM FA_CHART_OF_ACCOUNTS_SETUP
                              WHERE DELETED_FLAG = 'N'
                                AND acc_nature IN ('EB')
                                AND company_code = :CompanyCode
                                AND acc_type_flag = 'T')
                          AND form_code <> 0
                          AND voucher_date BETWEEN :FromDate AND :ToDate
                        GROUP BY acc_code
                    ) GROUP BY acc_code
                    HAVING NVL(SUM(a), 0) != 0";

                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":CompanyCode", companyCode);
                        command.Parameters.AddWithValue(":FromDate", fromDate);
                        command.Parameters.AddWithValue(":ToDate", toDate);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            decimal directTotal = 0;

                            while (await reader.ReadAsync())
                            {
                                var detail = new DirectExpenseDetail
                                {
                                    Topic = reader["acc_code"].ToString(),
                                    Amount = reader.GetDecimal(reader.GetOrdinal("aa"))
                                };

                                summary.Details.Add(detail);
                                directTotal += detail.Amount;
                            }

                            summary.Amount = directTotal;
                        }
                    }
                }

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("An error occurred while fetching data.", ex));
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> OverviewIndirectExpenses(
                [FromBody] DateTime fromDate,
                [FromBody] DateTime toDate,
                [FromBody] string companyCode)
             {
                var summary = new IndirectExpenseSummary
                {
                    Topic = "Indirect Expenses",
                    Details = new List<IndirectExpenseDetail>()
                };
            try
            {
                using (var connection = new OracleConnection(_connection))
                {
                    await connection.OpenAsync();

                    var query = @"
                    SELECT b.acc_edesc, NVL(a.aa, 0) AS aa 
                    FROM (
                        SELECT acc_code, SUM(dr_amount) - SUM(cr_amount) AS aa
                        FROM V$VIRTUAL_GENERAL_LEDGER
                        WHERE deleted_flag = 'N'
                          AND company_code = :CompanyCode
                          AND acc_code IN (
                              SELECT DISTINCT acc_code
                              FROM FA_CHART_OF_ACCOUNTS_SETUP
                              WHERE DELETED_FLAG = 'N'
                                AND acc_nature IN ('EC')
                                AND company_code = :CompanyCode
                                AND acc_type_flag = 'T')
                          AND (form_code = 0 OR voucher_date < :FromDate)
                        GROUP BY acc_code
                        UNION ALL
                        SELECT acc_code, SUM(dr_amount) - SUM(cr_amount)
                        FROM V$VIRTUAL_GENERAL_LEDGER
                        WHERE deleted_flag = 'N'
                          AND company_code = :CompanyCode
                          AND acc_code IN (
                              SELECT DISTINCT acc_code
                              FROM FA_CHART_OF_ACCOUNTS_SETUP
                              WHERE DELETED_FLAG = 'N'
                                AND acc_nature IN ('EC')
                                AND company_code = :CompanyCode
                                AND acc_type_flag = 'T')
                          AND form_code <> 0
                          AND voucher_date BETWEEN :FromDate AND :ToDate
                        GROUP BY acc_code
                    ) a
                    LEFT JOIN (
                        SELECT acc_code, acc_edesc
                        FROM FA_CHART_OF_ACCOUNTS_SETUP
                        WHERE company_code = :CompanyCode
                    ) b ON a.acc_code = b.acc_code
                    WHERE NVL(a.aa, 0) != 0";

                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":CompanyCode", companyCode);
                        command.Parameters.AddWithValue(":FromDate", fromDate);
                        command.Parameters.AddWithValue(":ToDate", toDate);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            decimal indirectTotal = 0;

                            while (await reader.ReadAsync())
                            {
                                var detail = new IndirectExpenseDetail
                                {
                                    Topic = reader["acc_edesc"].ToString(),
                                    Amount = reader.GetDecimal(reader.GetOrdinal("aa"))
                                };

                                summary.Details.Add(detail);
                                indirectTotal += detail.Amount;
                            }

                            summary.Amount = indirectTotal;
                        }
                    }
                }
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("An error occurred while fetching data.", ex));
            }
        }
        [HttpGet]
        [Route("overviewLoanWithGrp")]
        public async Task<IHttpActionResult> OverviewLoanWithGrp(
                    [FromBody] DateTime fromDate,
                    [FromBody] string companyCode)
        {
            var result = new LoanOverview
            {
                Rep = new List<LoanGroupDetail>(),
                Name = "loan"
            };

            var query = @"
                SELECT i_code, acc_edesc,
                       master_acc_code,
                       pre_acc_code,
                       CASE WHEN acc_type_flag = 'N' THEN 'G' ELSE 'I' END AS group_flag,
                       acc_code, NVL(aa, 0) AS aa
                FROM (
                    SELECT acc_code AS i_code, acc_edesc, master_acc_code, pre_acc_code, acc_type_flag
                    FROM FA_CHART_OF_ACCOUNTS_SETUP
                    WHERE company_code = :CompanyCode AND deleted_flag = 'N'
                    GROUP BY acc_code, acc_edesc, master_acc_code, pre_acc_code, acc_type_flag
                ) a
                LEFT OUTER JOIN (
                    SELECT acc_code, NVL(SUM(a), 0) AS aa
                    FROM (
                        SELECT acc_code, SUM(cr_amount) - SUM(dr_amount) AS a
                        FROM V$VIRTUAL_GENERAL_LEDGER
                        WHERE deleted_flag = 'N'
                          AND company_code = :CompanyCode
                          AND acc_code IN (
                              SELECT DISTINCT acc_code
                              FROM FA_CHART_OF_ACCOUNTS_SETUP
                              WHERE DELETED_FLAG = 'N'
                                AND acc_nature IN ('LC')
                                AND company_code = :CompanyCode
                                AND acc_type_flag = 'T'
                          )
                          AND form_code <> 0
                          AND voucher_date BETWEEN :FromDate AND :ToDate
                        GROUP BY acc_code
                    ) b
                    GROUP BY acc_code
                ) b ON a.i_code = b.acc_code
                ORDER BY master_acc_code";

            try
            {
                using (var connection = new OracleConnection(_connection))
                {
                    await connection.OpenAsync();
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":CompanyCode", companyCode);
                        command.Parameters.AddWithValue(":FromDate", fromDate);
                        command.Parameters.AddWithValue(":ToDate", DateTime.Now); // Update this to a valid end date

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var allCodes = new List<object[]>();
                            while (await reader.ReadAsync())
                            {
                                allCodes.Add(new object[]
                                {
                                    reader["i_code"],
                                    reader["acc_edesc"],
                                    reader["master_acc_code"],
                                    reader["pre_acc_code"],
                                    reader["group_flag"],
                                    reader["acc_code"],
                                    reader["aa"]
                                });
                            }
                            var arrTotal1 = new List<decimal>();
                            foreach (var i in allCodes)
                            {
                                var total1 = 0m;
                                var groupFlag = (string)i[4];
                                if (groupFlag == "G")
                                {
                                    var addOn = false;
                                    foreach (var z in allCodes)
                                    {
                                        if ((string)z[4] == "I")
                                        {
                                            var asd = (string)z[2];
                                            asd = asd.Length >= ((string)i[2]).Length ? asd.Substring(0, ((string)i[2]).Length) : string.Empty;
                                            if (((string)i[2]).Contains(asd))
                                            {
                                                addOn = true;
                                                total1 += (decimal)z[6];
                                            }
                                            else if (addOn)
                                            {
                                                break;
                                            }
                                        }
                                    }

                                    if (total1 > 0)
                                    {
                                        result.Rep.Add(new LoanGroupDetail
                                        {
                                            Item = (string)i[1],
                                            GroupFlag = groupFlag,
                                            Total = total1,
                                            Level = ((string)i[2]).Count(ch => ch == '.'),
                                            Master = (string)i[2],
                                            Pre = (string)i[3],
                                            Code = (string)i[0]
                                        });
                                    }
                                }

                                if (groupFlag == "I" && (decimal)i[6] > 0)
                                {
                                    arrTotal1.Add((decimal)i[6]);
                                    result.Rep.Add(new LoanGroupDetail
                                    {
                                        Item = (string)i[1],
                                        GroupFlag = groupFlag,
                                        Total = (decimal)i[6],
                                        Level = ((string)i[2]).Count(ch => ch == '.'),
                                        Master = (string)i[2],
                                        Pre = (string)i[3],
                                        Code = (string)i[0]
                                    });
                                }
                            }
                            result.Total = arrTotal1.Sum();
                        }
                    }
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("An error occurred while fetching loan data.", ex));
            }
        }
        [HttpGet]
        [Route("overviewCurrentLiabilitiesWithGrp")]
        public async Task<IHttpActionResult> OverviewCurrentLiabilitiesWithGrp(
          DateTime fromDate,
          DateTime toDate,
          string companyCode)
        {
            var result = new LiabilityOverview
            {
                Rep = new List<LiabilityGroupDetail>(),
                Name = "Current Liabilities"
            };

            var query = @"
            SELECT i_code, acc_edesc,
                   master_acc_code,
                   pre_acc_code,
                   CASE WHEN acc_type_flag = 'N' THEN 'G' ELSE 'I' END AS group_flag,
                   acc_code, NVL(aa, 0) AS aa
            FROM (
                SELECT acc_code AS i_code, acc_edesc, master_acc_code, pre_acc_code, acc_type_flag
                FROM FA_CHART_OF_ACCOUNTS_SETUP
                WHERE company_code = :CompanyCode AND deleted_flag = 'N'
                GROUP BY acc_code, acc_edesc, master_acc_code, pre_acc_code, acc_type_flag
            ) a
            LEFT OUTER JOIN (
                SELECT acc_code, NVL(SUM(a), 0) AS aa
                FROM (
                    SELECT acc_code, SUM(cr_amount) - SUM(dr_amount) AS a
                    FROM V$VIRTUAL_GENERAL_LEDGER
                    WHERE deleted_flag = 'N'
                      AND company_code = :CompanyCode
                      AND acc_code IN (
                          SELECT DISTINCT acc_code
                          FROM FA_CHART_OF_ACCOUNTS_SETUP
                          WHERE DELETED_FLAG = 'N'
                            AND acc_nature IN ('LB')
                            AND company_code = :CompanyCode
                            AND acc_type_flag = 'T'
                      )
                      AND form_code <> 0
                      AND voucher_date BETWEEN :FromDate AND :ToDate
                    GROUP BY acc_code
                ) b
                GROUP BY acc_code
            ) b ON a.i_code = b.acc_code
            ORDER BY master_acc_code";

            try
            {
                using (var connection = new OracleConnection(_connection))
                {
                    await connection.OpenAsync();

                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":CompanyCode", companyCode);
                        command.Parameters.AddWithValue(":FromDate", fromDate);
                        command.Parameters.AddWithValue(":ToDate", toDate);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var allCodes = new List<object[]>();
                            while (await reader.ReadAsync())
                            {
                                allCodes.Add(new object[]
                                {
                                    reader["i_code"],
                                    reader["acc_edesc"],
                                    reader["master_acc_code"],
                                    reader["pre_acc_code"],
                                    reader["group_flag"],
                                    reader["acc_code"],
                                    reader["aa"]
                                });
                            }

                            var arrTotal1 = new List<decimal>();
                            foreach (var i in allCodes)
                            {
                                var total1 = 0m;
                                var groupFlag = (string)i[4];

                                if (groupFlag == "G")
                                {
                                    var addOn = false;
                                    foreach (var z in allCodes)
                                    {
                                        if ((string)z[4] == "I")
                                        {
                                            var asd = (string)z[2];
                                            asd = asd.Length >= ((string)i[2]).Length ? asd.Substring(0, ((string)i[2]).Length) : string.Empty;
                                            if (((string)i[2]).Contains(asd))
                                            {
                                                addOn = true;
                                                total1 += (decimal)z[6];
                                            }
                                            else if (addOn)
                                            {
                                                break;
                                            }
                                        }
                                    }

                                    if (total1 > 0)
                                    {
                                        result.Rep.Add(new LiabilityGroupDetail
                                        {
                                            Item = (string)i[1],
                                            GroupFlag = groupFlag,
                                            Total = total1,
                                            Level = ((string)i[2]).Count(ch => ch == '.'),
                                            Master = (string)i[2],
                                            Pre = (string)i[3],
                                            Code = (string)i[0]
                                        });
                                    }
                                }

                                if (groupFlag == "I" && (decimal)i[6] > 0)
                                {
                                    arrTotal1.Add((decimal)i[6]);
                                    result.Rep.Add(new LiabilityGroupDetail
                                    {
                                        Item = (string)i[1],
                                        GroupFlag = groupFlag,
                                        Total = (decimal)i[6],
                                        Level = ((string)i[2]).Count(ch => ch == '.'),
                                        Master = (string)i[2],
                                        Pre = (string)i[3],
                                        Code = (string)i[0]
                                    });
                                }
                            }
                            result.Total = arrTotal1.Sum();
                        }
                    }
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("An error occurred while fetching liabilities data.", ex));
            }
        }

        [HttpGet()]
        [Route("OverviewCurrentAssetsWithGrp")]
        public async Task<IHttpActionResult> OverviewCurrentAssetsWithGrp(
          [FromBody] DateTime fromDate,
          [FromBody] DateTime toDate,
          [FromBody] string companyCode)
        {
            var result = new AssetOverview
            {
                Rep = new List<AssetGroupDetail>(),
                Name = "Current Assets"
            };

            var query = @"
            SELECT i_code, acc_edesc,
                   master_acc_code,
                   pre_acc_code,
                   CASE WHEN acc_type_flag = 'N' THEN 'G' ELSE 'I' END AS group_flag,
                   acc_code, ISNULL(aa, 0) AS aa
            FROM (
                SELECT acc_code AS i_code, acc_edesc, master_acc_code, pre_acc_code, acc_type_flag
                FROM FA_CHART_OF_ACCOUNTS_SETUP
                WHERE company_code IN (@CompanyCode) AND deleted_flag = 'N'
                GROUP BY acc_code, acc_edesc, master_acc_code, pre_acc_code, acc_type_flag
            ) a
            LEFT OUTER JOIN (
                SELECT acc_code, ISNULL(SUM(a), 0) AS aa
                FROM (
                    SELECT acc_code, SUM(dr_amount) - SUM(cr_amount) AS a
                    FROM V$VIRTUAL_GENERAL_LEDGER
                    WHERE deleted_flag = 'N'
                      AND company_code IN (@CompanyCode)
                      AND acc_code IN (
                          SELECT DISTINCT acc_code
                          FROM FA_CHART_OF_ACCOUNTS_SETUP
                          WHERE DELETED_FLAG = 'N'
                            AND acc_nature IN ('AD')
                            AND company_code IN (@CompanyCode)
                            AND acc_type_flag = 'T'
                      )
                      AND form_code <> 0
                      AND voucher_date BETWEEN @FromDate AND @ToDate
                    GROUP BY acc_code
                ) b
                GROUP BY acc_code
            ) b ON a.i_code = b.acc_code
            ORDER BY master_acc_code";

            using (var connection = new OracleConnection(_connection))
            {
                await connection.OpenAsync();

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CompanyCode", companyCode);
                    command.Parameters.AddWithValue("@FromDate", fromDate);
                    command.Parameters.AddWithValue("@ToDate", toDate);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var allCodes = new List<object[]>();
                        while (await reader.ReadAsync())
                        {
                            allCodes.Add(new object[]
                            {
                            reader["i_code"],
                            reader["acc_edesc"],
                            reader["master_acc_code"],
                            reader["pre_acc_code"],
                            reader["group_flag"],
                            reader["acc_code"],
                            reader["aa"]
                            });
                        }

                        var arrTotal1 = new List<decimal>();
                        foreach (var i in allCodes)
                        {
                            var total1 = 0m;
                            var groupFlag = (string)i[4];

                            if (groupFlag == "G")
                            {
                                var addOn = false;
                                foreach (var z in allCodes)
                                {
                                    if ((string)z[4] == "I")
                                    {
                                        var asd = (string)z[2];
                                        asd = asd.Length >= ((string)i[2]).Length ? asd.Substring(0, ((string)i[2]).Length) : string.Empty;
                                        if (((string)i[2]).Contains(asd))
                                        {
                                            addOn = true;
                                            total1 += (decimal)z[6];
                                        }
                                        else if (addOn)
                                        {
                                            break;
                                        }
                                    }
                                }

                                if (total1 > 0)
                                {
                                    result.Rep.Add(new AssetGroupDetail
                                    {
                                        Item = (string)i[1],
                                        GroupFlag = groupFlag,
                                        Total = total1,
                                        Level = ((string)i[2]).Count(ch => ch == '.'),
                                        Master = (string)i[2],
                                        Pre = (string)i[3],
                                        Code = (string)i[0]
                                    });
                                }
                            }

                            if (groupFlag == "I" && (decimal)i[6] > 0)
                            {
                                arrTotal1.Add((decimal)i[6]);
                                result.Rep.Add(new AssetGroupDetail
                                {
                                    Item = (string)i[1],
                                    GroupFlag = groupFlag,
                                    Total = (decimal)i[6],
                                    Level = ((string)i[2]).Count(ch => ch == '.'),
                                    Master = (string)i[2],
                                    Pre = (string)i[3],
                                    Code = (string)i[0]
                                });
                            }
                        }

                        result.Total = arrTotal1.Sum();
                    }
                }
            }

            return Ok(result);
        }

        [HttpGet()]
        [Route("overviewDirectExpensesWithGrp")]
        public async Task<IHttpActionResult> OverviewDirectExpensesWithGrp(
         [FromBody] DateTime fromDate,
         [FromBody] DateTime toDate,
         [FromBody] string companyCode)
        {
            var result = new ExpenseOverview
            {
                Rep = new List<ExpenseGroupDetail>(),
                Name = "Direct Expenses"
            };

            var query = @"
            SELECT i_code, acc_edesc,
                   master_acc_code,
                   pre_acc_code,
                   CASE WHEN acc_type_flag = 'N' THEN 'G' ELSE 'I' END AS group_flag,
                   acc_code, ISNULL(aa, 0) AS aa
            FROM (
                SELECT acc_code AS i_code, acc_edesc, master_acc_code, pre_acc_code, acc_type_flag
                FROM FA_CHART_OF_ACCOUNTS_SETUP
                WHERE company_code IN (@CompanyCode) AND deleted_flag = 'N'
                GROUP BY acc_code, acc_edesc, master_acc_code, pre_acc_code, acc_type_flag
            ) a
            LEFT OUTER JOIN (
                SELECT acc_code, ISNULL(SUM(a), 0) AS aa
                FROM (
                    SELECT acc_code, SUM(dr_amount) - SUM(cr_amount) AS a
                    FROM V$VIRTUAL_GENERAL_LEDGER
                    WHERE deleted_flag = 'N'
                      AND company_code IN (@CompanyCode)
                      AND acc_code IN (
                          SELECT DISTINCT acc_code
                          FROM FA_CHART_OF_ACCOUNTS_SETUP
                          WHERE DELETED_FLAG = 'N'
                            AND acc_nature IN ('EB')
                            AND company_code IN (@CompanyCode)
                            AND acc_type_flag = 'T'
                      )
                      AND form_code <> 0
                      AND voucher_date BETWEEN @FromDate AND @ToDate
                    GROUP BY acc_code
                ) b
                GROUP BY acc_code
            ) b ON a.i_code = b.acc_code
            ORDER BY master_acc_code";

            using (var connection = new OracleConnection(_connection))
            {
                await connection.OpenAsync();

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CompanyCode", companyCode);
                    command.Parameters.AddWithValue("@FromDate", fromDate);
                    command.Parameters.AddWithValue("@ToDate", toDate);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var allCodes = new List<object[]>();
                        while (await reader.ReadAsync())
                        {
                            allCodes.Add(new object[]
                            {
                            reader["i_code"],
                            reader["acc_edesc"],
                            reader["master_acc_code"],
                            reader["pre_acc_code"],
                            reader["group_flag"],
                            reader["acc_code"],
                            reader["aa"]
                            });
                        }

                        var arrTotal1 = new List<decimal>();
                        foreach (var i in allCodes)
                        {
                            var total1 = 0m;
                            var groupFlag = (string)i[4];

                            if (groupFlag == "G")
                            {
                                var addOn = false;
                                foreach (var z in allCodes)
                                {
                                    if ((string)z[4] == "I")
                                    {
                                        var asd = (string)z[2];
                                        asd = asd.Length >= ((string)i[2]).Length ? asd.Substring(0, ((string)i[2]).Length) : string.Empty;
                                        if (((string)i[2]).Contains(asd))
                                        {
                                            addOn = true;
                                            total1 += (decimal)z[6];
                                        }
                                        else if (addOn)
                                        {
                                            break;
                                        }
                                    }
                                }

                                if (total1 > 0)
                                {
                                    result.Rep.Add(new ExpenseGroupDetail
                                    {
                                        Item = (string)i[1],
                                        GroupFlag = groupFlag,
                                        Total = total1,
                                        Level = ((string)i[2]).Count(ch => ch == '.'),
                                        Master = (string)i[2],
                                        Pre = (string)i[3],
                                        Code = (string)i[0]
                                    });
                                }
                            }

                            if (groupFlag == "I" && (decimal)i[6] > 0)
                            {
                                arrTotal1.Add((decimal)i[6]);
                                result.Rep.Add(new ExpenseGroupDetail
                                {
                                    Item = (string)i[1],
                                    GroupFlag = groupFlag,
                                    Total = (decimal)i[6],
                                    Level = ((string)i[2]).Count(ch => ch == '.'),
                                    Master = (string)i[2],
                                    Pre = (string)i[3],
                                    Code = (string)i[0]
                                });
                            }
                        }
                        result.Total = arrTotal1.Sum();
                    }
                }
            }
            return Ok(result);
        }

        [HttpGet()]
        [Route("overviewInDirectExpensesWithGrp")]
        public async Task<IHttpActionResult> OverviewInDirectExpensesWithGrp(
        [FromBody] DateTime fromDate,
        [FromBody] DateTime toDate,
        [FromBody] string companyCode)
        {
            var result = new ExpenseOverview
            {
                Rep = new List<ExpenseGroupDetail>(),
                Name = "InDirect Expenses"
            };

            var query = @"
            SELECT i_code, acc_edesc,
                   master_acc_code,
                   pre_acc_code,
                   CASE WHEN acc_type_flag = 'N' THEN 'G' ELSE 'I' END AS group_flag,
                   acc_code, ISNULL(aa, 0) AS aa
            FROM (
                SELECT acc_code AS i_code, acc_edesc, master_acc_code, pre_acc_code, acc_type_flag
                FROM FA_CHART_OF_ACCOUNTS_SETUP
                WHERE company_code IN (@CompanyCode) AND deleted_flag = 'N'
                GROUP BY acc_code, acc_edesc, master_acc_code, pre_acc_code, acc_type_flag
            ) a
            LEFT OUTER JOIN (
                SELECT acc_code, ISNULL(SUM(a), 0) AS aa
                FROM (
                    SELECT acc_code, SUM(dr_amount) - SUM(cr_amount) AS a
                    FROM V$VIRTUAL_GENERAL_LEDGER
                    WHERE deleted_flag = 'N'
                      AND company_code IN (@CompanyCode)
                      AND acc_code IN (
                          SELECT DISTINCT acc_code
                          FROM FA_CHART_OF_ACCOUNTS_SETUP
                          WHERE DELETED_FLAG = 'N'
                            AND acc_nature IN ('EC')
                            AND company_code IN (@CompanyCode)
                            AND acc_type_flag = 'T'
                      )
                      AND form_code <> 0
                      AND voucher_date BETWEEN @FromDate AND @ToDate
                    GROUP BY acc_code
                ) b
                GROUP BY acc_code
            ) b ON a.i_code = b.acc_code
            ORDER BY master_acc_code";

            using (var connection = new OracleConnection(_connection))
            {
                await connection.OpenAsync();

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CompanyCode", companyCode);
                    command.Parameters.AddWithValue("@FromDate", fromDate);
                    command.Parameters.AddWithValue("@ToDate", toDate);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var allCodes = new List<object[]>();
                        while (await reader.ReadAsync())
                        {
                            allCodes.Add(new object[]
                            {
                            reader["i_code"],
                            reader["acc_edesc"],
                            reader["master_acc_code"],
                            reader["pre_acc_code"],
                            reader["group_flag"],
                            reader["acc_code"],
                            reader["aa"]
                            });
                        }

                        var arrTotal1 = new List<decimal>();
                        foreach (var i in allCodes)
                        {
                            var total1 = 0m;
                            var groupFlag = (string)i[4];

                            if (groupFlag == "G")
                            {
                                var addOn = false;
                                foreach (var z in allCodes)
                                {
                                    if ((string)z[4] == "I")
                                    {
                                        var asd = (string)z[2];
                                        asd = asd.Length >= ((string)i[2]).Length ? asd.Substring(0, ((string)i[2]).Length) : string.Empty;
                                        if (((string)i[2]).Contains(asd))
                                        {
                                            addOn = true;
                                            total1 += (decimal)z[6];
                                        }
                                        else if (addOn)
                                        {
                                            break;
                                        }
                                    }
                                }

                                if (total1 > 0)
                                {
                                    result.Rep.Add(new ExpenseGroupDetail
                                    {
                                        Item = (string)i[1],
                                        GroupFlag = groupFlag,
                                        Total = total1,
                                        Level = ((string)i[2]).Count(ch => ch == '.'),
                                        Master = (string)i[2],
                                        Pre = (string)i[3],
                                        Code = (string)i[0]
                                    });
                                }
                            }

                            if (groupFlag == "I" && (decimal)i[6] > 0)
                            {
                                arrTotal1.Add((decimal)i[6]);
                                result.Rep.Add(new ExpenseGroupDetail
                                {
                                    Item = (string)i[1],
                                    GroupFlag = groupFlag,
                                    Total = (decimal)i[6],
                                    Level = ((string)i[2]).Count(ch => ch == '.'),
                                    Master = (string)i[2],
                                    Pre = (string)i[3],
                                    Code = (string)i[0]
                                });
                            }
                        }

                        result.Total = arrTotal1.Sum();
                    }
                }
            }
            return Ok(result);
        }
        //[HttpGet()]
        //public async Task<IHttpActionResult> OverviewLoan(
        // [FromBody] DateTime fromDate,
        // [FromBody] DateTime toDate,
        // [FromBody] string companyCode)
        //{
        //    var result = new LoanOverview
        //    {
        //        Detail = new List<LoanDetail>(),
        //        Topic = "Loan"
        //    };

        //    var query = @"
        //    SELECT * FROM 
        //    (
        //        SELECT acc_code, ISNULL(SUM(a), 0) AS aa
        //        FROM (
        //            SELECT acc_code, SUM(cr_amount) - SUM(dr_amount) AS a
        //            FROM V$VIRTUAL_GENERAL_LEDGER
        //            WHERE deleted_flag = 'N'
        //              AND company_code = @CompanyCode
        //              AND acc_code IN (
        //                  SELECT DISTINCT acc_code
        //                  FROM FA_CHART_OF_ACCOUNTS_SETUP
        //                  WHERE DELETED_FLAG = 'N'
        //                    AND acc_nature IN ('LC')
        //                    AND company_code = @CompanyCode
        //                    AND acc_type_flag = 'T'
        //              )
        //              AND (form_code = 0 OR voucher_date < @FromDate)
        //            GROUP BY acc_code

        //            UNION ALL

        //            SELECT acc_code, SUM(cr_amount) - SUM(dr_amount)
        //            FROM V$VIRTUAL_GENERAL_LEDGER
        //            WHERE deleted_flag = 'N'
        //              AND company_code = @CompanyCode
        //              AND acc_code IN (
        //                  SELECT DISTINCT acc_code
        //                  FROM FA_CHART_OF_ACCOUNTS_SETUP
        //                  WHERE DELETED_FLAG = 'N'
        //                    AND acc_nature IN ('LC')
        //                    AND company_code = @CompanyCode
        //                    AND acc_type_flag = 'T'
        //              )
        //              AND form_code <> 0
        //              AND voucher_date BETWEEN @FromDate AND @ToDate
        //            GROUP BY acc_code
        //        ) AS a
        //        GROUP BY acc_code
        //    ) AS a
        //    LEFT OUTER JOIN 
        //    (
        //        SELECT acc_edesc, acc_code AS i_code
        //        FROM FA_CHART_OF_ACCOUNTS_SETUP
        //        WHERE company_code = @CompanyCode
        //    ) AS b ON a.acc_code = b.i_code
        //    WHERE aa != 0";

        //    using (var connection = new OracleConnection(_connection))
        //    {
        //        await connection.OpenAsync();

        //        using (var command = new OracleCommand(query, connection))
        //        {
        //            command.Parameters.AddWithValue("@CompanyCode", companyCode);
        //            command.Parameters.AddWithValue("@FromDate", fromDate);
        //            command.Parameters.AddWithValue("@ToDate", toDate);

        //            using (var reader = await command.ExecuteReaderAsync())
        //            {
        //                var detailList = new List<LoanDetail>();
        //                decimal direct = 0;

        //                while (await reader.ReadAsync())
        //                {
        //                    var amount = reader.GetDecimal(1);
        //                    var topic = reader.GetString(2);
        //                    detailList.Add(new LoanDetail
        //                    {
        //                        Topic = topic,
        //                        Amount = amount
        //                    });
        //                    direct += amount;
        //                }

        //                result.Amt = direct;
        //                result.Detail = detailList;
        //            }
        //        }
        //    }

        //    return Ok(result);
        //}


        //    [HttpGet]
        //    public async Task<IHttpActionResult> OverviewLoan(
        //[FromBody] DateTime fromDate,
        //[FromBody] DateTime toDate,
        //[FromBody] string companyCode)
        //    {
        //        var result = new LoanOverview
        //        {
        //            Detail = new List<LoanDetail>(),
        //            Topic = "Loan"
        //        };

        //        var query = @"
        //SELECT * FROM 
        //(
        //    SELECT acc_code, NVL(SUM(a), 0) AS aa
        //    FROM (
        //        SELECT acc_code, SUM(cr_amount) - SUM(dr_amount) AS a
        //        FROM V$VIRTUAL_GENERAL_LEDGER
        //        WHERE deleted_flag = 'N'
        //          AND company_code = :CompanyCode
        //          AND acc_code IN (
        //              SELECT DISTINCT acc_code
        //              FROM FA_CHART_OF_ACCOUNTS_SETUP
        //              WHERE DELETED_FLAG = 'N'
        //                AND acc_nature IN ('LC')
        //                AND company_code = :CompanyCode
        //                AND acc_type_flag = 'T'
        //          )
        //          AND (form_code = 0 OR voucher_date < :FromDate)
        //        GROUP BY acc_code

        //        UNION ALL

        //        SELECT acc_code, SUM(cr_amount) - SUM(dr_amount) AS a
        //        FROM V$VIRTUAL_GENERAL_LEDGER
        //        WHERE deleted_flag = 'N'
        //          AND company_code = :CompanyCode
        //          AND acc_code IN (
        //              SELECT DISTINCT acc_code
        //              FROM FA_CHART_OF_ACCOUNTS_SETUP
        //              WHERE DELETED_FLAG = 'N'
        //                AND acc_nature IN ('LC')
        //                AND company_code = :CompanyCode
        //                AND acc_type_flag = 'T'
        //          )
        //          AND form_code <> 0
        //          AND voucher_date BETWEEN :FromDate AND :ToDate
        //        GROUP BY acc_code
        //    ) AS a
        //    GROUP BY acc_code
        //) AS a
        //LEFT OUTER JOIN 
        //(
        //    SELECT acc_edesc, acc_code AS i_code
        //    FROM FA_CHART_OF_ACCOUNTS_SETUP
        //    WHERE company_code = :CompanyCode
        //) AS b ON a.acc_code = b.i_code
        //WHERE aa != 0";

        //        using (var connection = new OracleConnection(_connection))
        //        {
        //            await connection.OpenAsync();

        //            using (var command = new OracleCommand(query, connection))
        //            {
        //                command.Parameters.Add(new OracleParameter(":CompanyCode", companyCode));
        //                command.Parameters.Add(new OracleParameter(":FromDate", fromDate));
        //                command.Parameters.Add(new OracleParameter(":ToDate", toDate));

        //                using (var reader = await command.ExecuteReaderAsync())
        //                {
        //                    var detailList = new List<LoanDetail>();
        //                    decimal direct = 0;

        //                    while (await reader.ReadAsync())
        //                    {
        //                        var amount = reader.IsDBNull(1) ? 0 : reader.GetDecimal(1);
        //                        var topic = reader.GetString(2); // Adjust the index based on your query

        //                        detailList.Add(new LoanDetail
        //                        {
        //                            Topic = topic,
        //                            Amount = amount
        //                        });
        //                        direct += amount;
        //                    }

        //                    result.Amt = direct;
        //                    result.Detail = detailList;
        //                }
        //            }
        //        }

        //        return Ok(result);
        //    }

        [HttpGet()]
        public async Task<IHttpActionResult> OverviewCurrentAssets(
            [FromBody] DateTime fromDate,
            [FromBody] DateTime toDate,
            [FromBody] string companyCode)
        {
            var result = new CurrentAssetsOverview
            {
                Detail = new List<AssetDetail>(),
                Topic = "Current Assets"
            };

            var query = @"
            SELECT * FROM 
            (
                SELECT acc_code, ISNULL(SUM(a), 0) AS aa
                FROM (
                    SELECT acc_code, SUM(dr_amount) - SUM(cr_amount) AS a
                    FROM V$VIRTUAL_GENERAL_LEDGER
                    WHERE deleted_flag = 'N'
                      AND company_code = @CompanyCode
                      AND acc_code IN (
                          SELECT DISTINCT acc_code
                          FROM FA_CHART_OF_ACCOUNTS_SETUP
                          WHERE DELETED_FLAG = 'N'
                            AND acc_nature IN ('AD')
                            AND company_code = @CompanyCode
                            AND acc_type_flag = 'T'
                      )
                      AND (form_code = 0 OR voucher_date < @FromDate)
                    GROUP BY acc_code

                    UNION ALL

                    SELECT acc_code, SUM(dr_amount) - SUM(cr_amount)
                    FROM V$VIRTUAL_GENERAL_LEDGER
                    WHERE deleted_flag = 'N'
                      AND company_code = @CompanyCode
                      AND acc_code IN (
                          SELECT DISTINCT acc_code
                          FROM FA_CHART_OF_ACCOUNTS_SETUP
                          WHERE DELETED_FLAG = 'N'
                            AND acc_nature IN ('AD')
                            AND company_code = @CompanyCode
                            AND acc_type_flag = 'T'
                      )
                      AND form_code <> 0
                      AND voucher_date BETWEEN @FromDate AND @ToDate
                    GROUP BY acc_code
                ) AS a
                GROUP BY acc_code
            ) AS a
            LEFT OUTER JOIN 
            (
                SELECT acc_edesc, acc_code AS i_code
                FROM FA_CHART_OF_ACCOUNTS_SETUP
                WHERE company_code = @CompanyCode
            ) AS b ON a.acc_code = b.i_code
            WHERE aa != 0";

            using (var connection = new OracleConnection(_connection))
            {
                await connection.OpenAsync();

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CompanyCode", companyCode);
                    command.Parameters.AddWithValue("@FromDate", fromDate);
                    command.Parameters.AddWithValue("@ToDate", toDate);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var detailList = new List<AssetDetail>();
                        decimal totalAmount = 0;

                        while (await reader.ReadAsync())
                        {
                            var amount = reader.GetDecimal(1);
                            var topic = reader.GetString(2);
                            detailList.Add(new AssetDetail
                            {
                                Topic = topic,
                                Amount = amount
                            });
                            totalAmount += amount;
                        }

                        result.Amt = totalAmount;
                        result.Detail = detailList;
                    }
                }
            }

            return Ok(result);
        }


        [HttpGet()]
        public async Task<IHttpActionResult> Login([FromBody] string userName)
        {
            var results = new List<UserAccessInfo>();

            using (var connection = new OracleConnection(_connection))
            {
                await connection.OpenAsync();

                // Ensure columns exist
                await EnsureColumnsExistAsync(connection);

                // Query to get user information
                var query = @"
                SELECT fn_decrypt_password(password) AS password, company_code, USER_NO, mobile_users, mobile_admin_users
                FROM SC_APPLICATION_USERS
                WHERE login_code = @UserName
                  AND GROUP_SKU_FLAG = 'I'
                UNION ALL
                SELECT password, company_code, USER_NO, mobile_users, mobile_admin_users
                FROM SC_APPLICATION_USERS
                WHERE login_code = @UserName
                  AND GROUP_SKU_FLAG = 'I'
                  AND fn_decrypt_password(password) IS NULL
                ORDER BY company_code";

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", userName);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            results.Add(new UserAccessInfo
                            {
                                Password = reader.IsDBNull(0) ? null : reader.GetString(0),
                                CompanyCode = reader.GetString(1),
                                UserNo = reader.GetInt32(2),
                                MobileUsers = reader.IsDBNull(3) ? null : reader.GetString(3),
                                MobileAdminUsers = reader.IsDBNull(4) ? null : reader.GetString(4)
                            });
                        }
                    }
                }
            }

            return Ok(results);
        }

        private async Task EnsureColumnsExistAsync(OracleConnection connection)
        {
            var tableAlterQueries = new List<string>
        {
            "IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'SC_APPLICATION_USERS' AND COLUMN_NAME = 'mobile_users') BEGIN ALTER TABLE SC_APPLICATION_USERS ADD mobile_users CHAR(1) END",
            "IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'SC_APPLICATION_USERS' AND COLUMN_NAME = 'mobile_admin_users') BEGIN ALTER TABLE SC_APPLICATION_USERS ADD mobile_admin_users CHAR(1) END",
            "IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'USER_MOBILE_ACCESS') BEGIN CREATE TABLE USER_MOBILE_ACCESS (USER_NO INT, REPORTS VARCHAR(50), ACCES CHAR(1), VIU CHAR(1), CREATED_DATE DATE DEFAULT GETDATE(), MODULE VARCHAR(50), COMPANY_CODE VARCHAR(2), PRIMARY KEY (USER_NO, REPORTS)) END"
        };

            foreach (var query in tableAlterQueries)
            {
                using (var command = new OracleCommand(query, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        [HttpPost()]
        public async Task<IHttpActionResult> InsertToken([FromBody] string companyCode, [FromBody] int userNo, [FromBody] string token)
        {
            string result;

            using (var connection = new OracleConnection(_connection))
            {
                await connection.OpenAsync();

                // Check if 'mob_token' column exists, add if not
                await EnsureMobTokenColumnExistsAsync(connection);

                // Retrieve current token
                string currentToken = await GetCurrentTokenAsync(connection, companyCode, userNo);

                if (currentToken == "NO_TOKEN")
                {
                    // Update token if no token exists
                    await UpdateTokenAsync(connection, companyCode, userNo, token);
                    result = "Token updated";
                }
                else if (currentToken != token)
                {
                    result = "Token already";
                }
                else
                {
                    result = "Token is the same";
                }

                // Add branch access for admin user
                await AddBranchAccessAsync(connection, userNo);

            }

            return Ok(result);
        }
        private async Task EnsureMobTokenColumnExistsAsync(OracleConnection connection)
        {
            var query = @"
            IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'sc_application_users' AND COLUMN_NAME = 'mob_token')
            BEGIN
                ALTER TABLE sc_application_users ADD mob_token VARCHAR(400)
            END";

            using (var command = new OracleCommand(query, connection))
            {
                await command.ExecuteNonQueryAsync();
            }
        }
        private async Task<string> GetCurrentTokenAsync(OracleConnection connection, string companyCode, int userNo)
        {
            var query = @"
            SELECT ISNULL(mob_token, 'NO_TOKEN') 
            FROM sc_application_users 
            WHERE user_no = @UserNo AND company_code = @CompanyCode";

            using (var command = new OracleCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserNo", userNo);
                command.Parameters.AddWithValue("@CompanyCode", companyCode);

                var result = await command.ExecuteScalarAsync();
                return result?.ToString();
            }
        }
        private async Task UpdateTokenAsync(OracleConnection connection, string companyCode, int userNo, string token)
        {
            var query = @"
            UPDATE sc_application_users 
            SET mob_token = @Token 
            WHERE user_no = @UserNo AND company_code = @CompanyCode";

            using (var command = new OracleCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Token", token);
                command.Parameters.AddWithValue("@UserNo", userNo);
                command.Parameters.AddWithValue("@CompanyCode", companyCode);

                await command.ExecuteNonQueryAsync();
            }
        }
        private async Task AddBranchAccessAsync(OracleConnection connection, int userNo)
        {
            var query = @"
            SELECT USER_NO 
            FROM sc_application_users 
            WHERE user_type = 'ADMIN' AND user_no = @UserNo 
            GROUP BY USER_TYPE, USER_NO";

            using (var command = new OracleCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserNo", userNo);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        int adminUserNo = reader.GetInt32(0);

                        var branchQuery = @"
                        SELECT BRANCH_CODE, COMPANY_CODE 
                        FROM FA_BRANCH_SETUP 
                        WHERE GROUP_SKU_FLAG = 'I'
                          AND branch_code NOT IN 
                              (SELECT branch_code FROM SC_BRANCH_CONTROL WHERE USER_NO = @UserNo)";

                        using (var branchCommand = new OracleCommand(branchQuery, connection))
                        {
                            branchCommand.Parameters.AddWithValue("@UserNo", adminUserNo);

                            using (var branchReader = await branchCommand.ExecuteReaderAsync())
                            {
                                while (await branchReader.ReadAsync())
                                {
                                    string branchCode = branchReader.GetString(0);
                                    string companyCode = branchReader.GetString(1);

                                    var insertQuery = @"
                                    INSERT INTO SC_BRANCH_CONTROL
                                    (USER_NO, BRANCH_CODE, ACCESS_FLAG, MORE_FLAG, COMPANY_CODE, CREATED_BY, CREATED_DATE, DELETED_FLAG)
                                    VALUES
                                    (@UserNo, @BranchCode, 'Y', '0', @CompanyCode, 'ADMIN', GETDATE(), 'N')";

                                    using (var insertCommand = new OracleCommand(insertQuery, connection))
                                    {
                                        insertCommand.Parameters.AddWithValue("@UserNo", adminUserNo);
                                        insertCommand.Parameters.AddWithValue("@BranchCode", branchCode);
                                        insertCommand.Parameters.AddWithValue("@CompanyCode", companyCode);

                                        await insertCommand.ExecuteNonQueryAsync();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        [HttpGet()]
        public async Task<IHttpActionResult> UserAccessedCompanies([FromBody] int userNo)
        {
            var companies = new Dictionary<string, CompanyResponse>();
            string changedCompany = string.Empty;

            using (var connection = new OracleConnection(_connection))
            {
                await connection.OpenAsync();

                var query = @"
                SELECT a.branch_code, a.branch_edesc AS branch_name, a.branch_address, a.telephone_no, a.company_code,
                       b.company_edesc AS company_name, b.address AS company_address, b.tpin_vat_no
                        FROM (SELECT branch_code, branch_edesc, address AS branch_address, telephone_no, company_code
                      FROM FA_BRANCH_SETUP
                      WHERE group_sku_flag = 'I' AND deleted_flag = 'N'
                      AND branch_code IN (SELECT branch_code FROM SC_BRANCH_CONTROL WHERE user_no = @UserNo AND access_flag = 'Y')
                      ORDER BY branch_code) a
                        LEFT JOIN (SELECT company_code, company_edesc, address AS company_address, tpin_vat_no
                           FROM company_setup) b
                        ON a.company_code = b.company_code";

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserNo", userNo);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var branchCode = reader.GetString(0);
                            var branchName = reader.GetString(1);
                            var branchAddress = reader.GetString(2);
                            var companyCode = reader.GetString(4);
                            var companyName = reader.GetString(5);
                            var companyAddress = reader.GetString(6);
                            var tpinVatNo = reader.GetString(7);

                            if (!companies.ContainsKey(companyCode))
                            {
                                companies[companyCode] = new CompanyResponse
                                {
                                    CompanyCode = companyCode,
                                    CompanyName = companyName,
                                    CompanyAddress = companyAddress,
                                    TpinVatNo = tpinVatNo,
                                    Branches = new List<BranchResponse>()
                                };
                            }

                            companies[companyCode].Branches.Add(new BranchResponse
                            {
                                BranchCode = branchCode,
                                BranchName = branchName,
                                BranchAddress = branchAddress
                            });
                        }
                    }
                }
            }

            return Ok(companies.Values);
        }
        [HttpGet]
        [Route("AllSubLedgers")]
        public async Task<IHttpActionResult> AllSubLedgers([FromBody] string companyCode)
        {
            var subLedgerData = new List<SubLedgerResponse>();

            var query = @"
                SELECT * FROM 
                (
                    SELECT link_sub_code, customer_edesc AS sub_name 
                    FROM sa_customer_setup 
                    WHERE GROUP_SKU_FLAG = 'I' AND company_code = :CompanyCode AND deleted_flag = 'N'
                    UNION ALL
                    SELECT link_sub_code, supplier_edesc AS sub_name 
                    FROM IP_SUPPLIER_SETUP 
                    WHERE GROUP_SKU_FLAG = 'I' AND company_code = :CompanyCode AND deleted_flag = 'N'
                    UNION ALL
                    SELECT link_sub_code, EMPLOYEE_EDESC AS sub_name 
                    FROM HR_EMPLOYEE_SETUP 
                    WHERE GROUP_SKU_FLAG = 'I' AND company_code = :CompanyCode AND deleted_flag = 'N'
                    UNION ALL
                    SELECT party_type_code AS link_sub_code, party_type_EDESC AS sub_name 
                    FROM IP_PARTY_TYPE_CODE 
                    WHERE company_code = :CompanyCode AND deleted_flag = 'N'
                ) ORDER BY sub_name";

            try
            {
                using (var connection = new OracleConnection(_connection))
                {
                    await connection.OpenAsync();
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(":CompanyCode", companyCode); // Changed @ to :

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                subLedgerData.Add(new SubLedgerResponse
                                {
                                    LinkSubCode = reader.GetString(0),
                                    SubName = reader.GetString(1)
                                });
                            }
                        }
                    }
                }

                return Ok(subLedgerData);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }
        [HttpGet()]
        public async Task<IHttpActionResult> AllDealer([FromBody] string companyCode)
        {
            var dealerData = new List<DealerResponse>();

            var query = @"
                SELECT * FROM 
                (
                    SELECT DISTINCT party_type_code 
                    FROM sa_sales_invoice 
                    WHERE company_code = @CompanyCode AND deleted_flag = 'N'
                ) a
                LEFT OUTER JOIN
                (
                    SELECT party_type_code AS p_code, party_type_edesc AS name 
                    FROM IP_PARTY_TYPE_CODE 
                    WHERE company_code = @CompanyCode
                ) b 
                ON a.party_type_code = b.p_code";

            using (var connection = new OracleConnection(_connection))
            {
                await connection.OpenAsync();
                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CompanyCode", companyCode);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            dealerData.Add(new DealerResponse
                            {
                                Code = reader.GetString(0),
                                Name = reader.IsDBNull(2) ? null : reader.GetString(2)
                            });
                        }
                    }
                }
            }

            return Ok(dealerData);
        }
        [HttpGet()]
        public async Task<IHttpActionResult> SubLedgerOnlyTransaction([FromBody] string companyCode)
        {
            var subLedgerData = new List<SubLedgerTransactionResponse>();

            var query = @"
            SELECT DISTINCT sub_edesc AS sub_name, sub_code AS link_sub_code
            FROM V$VIRTUAL_SUB_LEDGEr 
            WHERE company_code = @CompanyCode 
            ORDER BY sub_edesc ASC";

            using (var connection = new OracleConnection(_connection))
            {
                await connection.OpenAsync();
                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CompanyCode", companyCode);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            subLedgerData.Add(new SubLedgerTransactionResponse
                            {
                                LinkSubCode = reader.GetString(1),
                                SubName = reader.GetString(0)
                            });
                        }
                    }
                }
            }
            return Ok(subLedgerData);
        }

        [HttpGet()]
        public async Task<IHttpActionResult> Dealers([FromBody] string companyCode)
        {
            var dealerData = new List<DealerResponse>();

            var query = @"
                SELECT party_type_edesc AS sub_name, party_type_code AS link_sub_code
                FROM IP_PARTY_TYPE_CODE 
                WHERE deleted_flag = 'N' AND company_code = @CompanyCode 
                ORDER BY party_type_edesc ASC";

            using (var connection = new OracleConnection(_connection))
            {
                await connection.OpenAsync();
                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CompanyCode", companyCode);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            dealerData.Add(new DealerResponse
                            {
                                //LinkSubCode = reader.GetString(1),
                                //SubName = reader.GetString(0)
                                Code = reader.GetString(0),
                                Name = reader.IsDBNull(2) ? null : reader.GetString(2)
                            });
                        }
                    }
                }
            }

            return Ok(dealerData);
        }

        // Method to get customer details
        private Dictionary<string, object> GetCustomerDetail(string customerCode, string companyCode)
        {
            var customerData = new Dictionary<string, object>();

            // SQL query to get customer details (Oracle syntax)
            string customerQuery = $@"
                SELECT a.CUSTOMER_EDESC, a.REGD_OFFICE_EADDRESS, a.TEL_MOBILE_NO1, a.TPIN_VAT_NO, a.CREDIT_DAYS, 
                NVL(a.CREDIT_LIMIT, 0), 
                (SELECT SUM(bg_amount) FROM FA_BANK_GUARANTEE WHERE cs_code = a.CUSTOMER_CODE AND company_code = :companyCode),
                (SELECT SUM(pdc_amount) FROM FA_PDC_RECEIPTS WHERE customer_code = a.CUSTOMER_CODE AND company_code = :companyCode)
                FROM SA_CUSTOMER_SETUP a
                WHERE a.CUSTOMER_CODE = :customerCode AND company_code = :companyCode";

            using (OracleConnection conn = new OracleConnection(_connection))
            {
                OracleCommand cmd = new OracleCommand(customerQuery, conn);
                cmd.Parameters.Add(new OracleParameter(":customerCode", customerCode));
                cmd.Parameters.Add(new OracleParameter(":companyCode", companyCode));

                conn.Open();

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        customerData["name"] = reader.GetString(0);
                        customerData["address"] = reader.GetString(1);
                        customerData["ph_no"] = reader.GetString(2);
                        customerData["pan"] = reader.GetString(3);
                        customerData["credit_days"] = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
                        customerData["credit_limit"] = reader.IsDBNull(5) ? 0.0m : Convert.ToDecimal(reader.GetValue(5));
                        customerData["bg_amt"] = reader.IsDBNull(6) ? 0.0m : Convert.ToDecimal(reader.GetValue(6));
                        customerData["pdc_amt"] = reader.IsDBNull(7) ? 0.0m : Convert.ToDecimal(reader.GetValue(7));
                    }
                    else
                    {
                        return null; // Return null if no data found
                    }
                }
            }

            // SQL query to get sales and collection data
            string salesQuery = $@"
                SELECT rangename, sales_amount, col_amt, sortorder FROM 
                (
                    SELECT a.rangename, a.startdate, a.enddate, a.sortorder, SUM(NVL(b.amount, 0)) AS sales_amount 
                    FROM 
                    (
                        SELECT startdate, enddate, rangename, sortorder FROM V_DATE_RANGE WHERE sortorder IN ('5', '3', '2')
                    ) a
                    LEFT JOIN  
                    (
                        SELECT sales_date, (sales_value - sales_ret_value) AS amount 
                        FROM 
                        (
                            SELECT sales_date, SUM(NVL(QUANTITY * NET_GROSS_RATE, 0)) AS sales_value, 
                                   SUM(NVL(RET_QUANTITY * NET_GROSS_RATE, 0)) AS sales_ret_value
                            FROM SA_SALES_INVOICE
                            WHERE customer_code = :customerCode AND company_code = :companyCode
                            GROUP BY sales_date
                        )
                    ) b ON b.sales_date BETWEEN a.startdate AND a.enddate
                    GROUP BY a.rangename, a.startdate, a.enddate, a.sortorder
                ) a
                LEFT JOIN 
                (
                    SELECT a.rangename, SUM(NVL(b.amt, 0)) AS col_amt 
                    FROM 
                    (
                        SELECT startdate, enddate, rangename, sortorder FROM V_DATE_RANGE WHERE sortorder IN ('5', '3', '2')
                    ) a
                    LEFT JOIN  
                    (
                        SELECT voucher_date, SUM(NVL(CR_AMOUNT, 0) * NVL(EXCHANGE_RATE, 1)) AS amt 
                        FROM V$VIRTUAL_SUB_LEDGER 
                        WHERE customer_code = :customerCode AND company_code = :companyCode
                        GROUP BY voucher_date
                    ) b ON b.voucher_date BETWEEN a.startdate AND a.enddate
                    GROUP BY a.rangename
                ) b ON a.rangename = b.rangename
                ORDER BY sortorder DESC";

            using (OracleConnection conn = new OracleConnection(_connection))
            {
                OracleCommand cmd = new OracleCommand(salesQuery, conn);
                cmd.Parameters.Add(new OracleParameter(":customerCode", customerCode));
                cmd.Parameters.Add(new OracleParameter(":companyCode", companyCode));

                conn.Open();

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    var salesCollection = new Dictionary<string, float>();
                    int x = 0;

                    while (reader.Read())
                    {
                        x++;
                        if (x == 1)
                        {
                            salesCollection["w_col"] = reader.IsDBNull(2) ? 0 : (float)reader.GetDecimal(2);
                            salesCollection["w_sales"] = reader.IsDBNull(1) ? 0 : (float)reader.GetDecimal(1);
                        }
                        else if (x == 2)
                        {
                            salesCollection["m_col"] = reader.IsDBNull(2) ? 0 : (float)reader.GetDecimal(2);
                            salesCollection["m_sales"] = reader.IsDBNull(1) ? 0 : (float)reader.GetDecimal(1);
                        }
                        else if (x == 3)
                        {
                            salesCollection["y_col"] = reader.IsDBNull(2) ? 0 : (float)reader.GetDecimal(2);
                            salesCollection["y_sales"] = reader.IsDBNull(1) ? 0 : (float)reader.GetDecimal(1);
                        }
                    }

                    customerData["s_c"] = salesCollection;
                }
            }

            return customerData;
        }
        [HttpGet]
        public IHttpActionResult SuppliersDetail(string customerCode, string companyCode)
        {
            var vals = new Dictionary<string, object>();
            string query = @"
            SELECT 
                supplier_edesc, 
                REGD_OFFICE_EADDRESS, 
                TEL_MOBILE_NO1, 
                TPIN_VAT_NO, 
                CREDIT_DAYS, 
                CREDIT_LIMIT
            FROM IP_SUPPLIER_SETUP
            WHERE SUPPLIER_CODE = :CustomerCode AND COMPANY_CODE = :CompanyCode";

            using (OracleConnection con = new OracleConnection(_connection))
            {
                using (OracleCommand cmd = new OracleCommand(query, con))
                {
                    cmd.Parameters.Add(new OracleParameter(":CustomerCode", customerCode));
                    cmd.Parameters.Add(new OracleParameter(":CompanyCode", companyCode));

                    con.Open();

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                vals["name"] = reader["supplier_edesc"].ToString();
                                vals["address"] = reader["REGD_OFFICE_EADDRESS"].ToString();
                                vals["ph_no"] = reader["TEL_MOBILE_NO1"].ToString();
                                vals["pan"] = reader["TPIN_VAT_NO"].ToString();
                                vals["credit_days"] = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
                                vals["credit_limit"] = reader.IsDBNull(5) ? 0.0 : reader.GetDouble(5);
                            }
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                }
            }
            return Ok(vals);
        }

        //[HttpGet]
        //public IHttpActionResult ItemHistory(string companyCode, [FromBody] List<string> itemCodes)
        //{
        //    // Ensure itemCodes is not null or empty
        //    if (itemCodes == null || itemCodes.Count == 0)
        //    {
        //        return BadRequest("Item codes cannot be null or empty.");
        //    }

        //    // Safely create a comma-separated list of item codes for the SQL IN clause
        //    string itemCodeList = string.Join(",", itemCodes.Select(code => $"'{code}'"));

        //    // SQL query
        //    string query = $@"
        //SELECT A.supplier_code, A.INVOICE_NO, A.ITEM_CODE, A.UNIT_PRICE, B.ITEM_EDESC, C.SUPPLIER_EDESC
        //FROM (
        //    SELECT supplier_code, INVOICE_NO, ITEM_CODE, UNIT_PRICE, 
        //           ROW_NUMBER() OVER (PARTITION BY supplier_code ORDER BY INVOICE_NO DESC) rn_
        //    FROM IP_PURCHASE_INVOICE
        //    WHERE COMPANY_CODE = :CompanyCode AND ITEM_CODE IN ({itemCodeList})
        //) A
        //LEFT JOIN IP_ITEM_MASTER_SETUP B ON A.ITEM_CODE = B.ITEM_CODE
        //LEFT JOIN IP_SUPPLIER_SETUP C ON A.supplier_code = C.SUPPLIER_CODE
        //WHERE A.rn_ = 1
        //ORDER BY A.ITEM_CODE";

        //    var result = new List<Dictionary<string, object>>();

        //    using (var con = new OracleConnection(_connection))
        //    {
        //        using (var cmd = new OracleCommand(query, con))
        //        {
        //            cmd.Parameters.Add(new OracleParameter(":CompanyCode", companyCode));

        //            con.Open();

        //            using (var reader = cmd.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    var vals = new Dictionary<string, object>
        //                    {
        //                        ["invoice_no"] = reader["INVOICE_NO"].ToString(),
        //                        ["it_name"] = reader["ITEM_EDESC"]?.ToString() ?? string.Empty,
        //                        ["it_code"] = reader["ITEM_CODE"].ToString(),
        //                        ["rate"] = reader["UNIT_PRICE"] != DBNull.Value ? Convert.ToDouble(reader["UNIT_PRICE"]) : 0.0,
        //                        ["suppliers_name"] = reader["SUPPLIER_EDESC"]?.ToString() ?? string.Empty,
        //                        ["suppliers_code"] = reader["supplier_code"].ToString()
        //                    };

        //                    result.Add(vals);
        //                }
        //            }
        //        }
        //    }

        //    return Ok(result);
        //}



        //[HttpGet]
        //public IHttpActionResult SubLedgerReport(string companyCode, string branchCode, string subCode, string fromDate, string toDate, string userNo)
        //{
        //    var slData = new List<Dictionary<string, object>>();
        //    var opening = new Dictionary<string, object>();
        //    List<double> drsArray = new List<double>();
        //    List<double> crsArray = new List<double>();

        //    // First query to fetch transaction details
        //    string query1 = @"
        //        SELECT voucher_date, voucher_no, dr_amount * exchange_rate AS dr_amount, cr_amount * exchange_rate AS cr_amount,
        //               particulars, created_by, currency_code, exchange_rate, BS_DATE(voucher_date) AS bs_date
        //        FROM V$VIRTUAL_SUB_LEDGER
        //        WHERE sub_code = :SubCode
        //          AND company_code = :CompanyCode
        //          AND branch_code = :BranchCode
        //          AND voucher_date BETWEEN :FromDate AND :ToDate
        //          AND deleted_flag = 'N'
        //          AND form_code != 0
        //        ORDER BY voucher_date, voucher_no";

        //    using (OracleConnection con = new OracleConnection(_connection))
        //    {
        //        using (OracleCommand cmd = new OracleCommand(query1, con))
        //        {
        //            cmd.Parameters.AddWithValue(":SubCode", subCode);
        //            cmd.Parameters.AddWithValue(":CompanyCode", companyCode);
        //            cmd.Parameters.AddWithValue(":BranchCode", branchCode);
        //            cmd.Parameters.AddWithValue(":FromDate", DateTime.Parse(fromDate)); 
        //            cmd.Parameters.AddWithValue(":ToDate", DateTime.Parse(toDate)); 

        //            con.Open();

        //            using (OracleDataReader reader = cmd.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    double debitAmt = reader["dr_amount"] != DBNull.Value ? Convert.ToDouble(reader["dr_amount"]) : 0;
        //                    double creditAmt = reader["cr_amount"] != DBNull.Value ? Convert.ToDouble(reader["cr_amount"]) : 0;

        //                    drsArray.Add(debitAmt);
        //                    crsArray.Add(creditAmt);

        //                    var vals = new Dictionary<string, object>
        //                    {
        //                        ["voucher_date"] = Convert.ToDateTime(reader["voucher_date"]).ToString("dd-MMM-yyyy"),
        //                        ["voucher_no"] = reader["voucher_no"].ToString(),
        //                        ["dr_amount"] = debitAmt,
        //                        ["cr_amount"] = creditAmt,
        //                        ["particulars"] = reader["particulars"].ToString(),
        //                        ["created_by"] = reader["created_by"].ToString(),
        //                        ["currency_code"] = reader["currency_code"].ToString(),
        //                        ["exchange_rate"] = reader["exchange_rate"] != DBNull.Value ? Convert.ToDouble(reader["exchange_rate"]) : 0,
        //                        ["miti"] = reader["bs_date"].ToString()
        //                    };

        //                    slData.Add(vals);
        //                }
        //            }
        //        }
        //    }

        //    // Second query for opening balance
        //    string query2 = @"
        //                SELECT to_date(:FromDate) AS voucher_date, ' Opening Balance' AS voucher_no,
        //                       CASE WHEN SUM(dr_amount) - SUM(cr_amount) > 0 THEN SUM(dr_amount) - SUM(cr_amount) END AS dr_amount,
        //                       CASE WHEN SUM(cr_amount) - SUM(dr_amount) > 0 THEN SUM(cr_amount) - SUM(dr_amount) END AS cr_amount
        //                FROM V$VIRTUAL_SUB_LEDGER
        //                WHERE sub_code = :SubCode
        //                  AND company_code = :CompanyCode
        //                  AND branch_code = :BranchCode
        //                  AND deleted_flag = 'N'
        //                  AND (form_code = '0' OR voucher_date < :FromDate)
        //                GROUP BY sub_code";

        //    using (OracleConnection con = new OracleConnection(_connection))
        //    {
        //        using (OracleCommand cmd = new OracleCommand(query2, con))
        //        {
        //            cmd.Parameters.AddWithValue(":SubCode", subCode);
        //            cmd.Parameters.AddWithValue(":CompanyCode", companyCode);
        //            cmd.Parameters.AddWithValue(":BranchCode", branchCode);
        //            cmd.Parameters.AddWithValue(":FromDate", DateTime.Parse(fromDate)); // Ensure date parsing

        //            con.Open();

        //            using (OracleDataReader reader = cmd.ExecuteReader())
        //            {
        //                if (reader.Read())
        //                {
        //                    double openingDr = reader["dr_amount"] != DBNull.Value ? Convert.ToDouble(reader["dr_amount"]) : 0;
        //                    double openingCr = reader["cr_amount"] != DBNull.Value ? Convert.ToDouble(reader["cr_amount"]) : 0;

        //                    opening["DR"] = openingDr;
        //                    opening["CR"] = openingCr;
        //                }
        //            }
        //        }
        //    }

        //    double debitTotal = drsArray.Sum();
        //    double creditTotal = crsArray.Sum();
        //    double totalClosing = debitTotal - creditTotal;

        //    var transaction = new
        //    {
        //        transactions = slData,
        //        debit_total = debitTotal,
        //        credit_total = creditTotal,
        //        opening = opening,
        //        closing_balance = totalClosing,
        //        ageing_report = "ageing_report_placeholder" 
        //    };

        //    return Ok(transaction);
        //}

        //private double SumArray(List<double> array)
        //{
        //    double total = 0;
        //    foreach (var item in array)
        //    {
        //        total += item;
        //    }
        //    return total;
        //}


        //[HttpPost()]
        //public IHttpActionResult AllCustomerAgeing([FromBody] AgeingRequest request)
        //{
        //    try
        //    {
        //        string companyCode = request.CompanyCode[0];

        //        string cusCode = string.IsNullOrEmpty(request.CusCode) ? "" : $"AND SUB_CODE = '{request.CusCode}'";
        //        string grpCode = string.IsNullOrEmpty(request.GrpCode) ? "" :
        //            $"AND SUB_CODE IN (SELECT 'C' + CUSTOMER_CODE FROM SA_CUSTOMER_SETUP WHERE MASTER_CUSTOMER_CODE LIKE '{request.GrpCode}%' AND COMPANY_CODE = '{companyCode}')";

        //        string query = $@"
        //            SELECT * FROM 
        //            (
        //                SELECT * FROM 
        //                (
        //                    SELECT TO_NUMBER(SUBSTR(SUB_CODE, 2, 99)) AS SubCode, 
        //                        VOUCHER_DATE, VOUCHER_NO, 
        //                        CASE WHEN qw = 1 THEN NET_AMOUNT ELSE DR_AMOUNT END AS PENDING_BAL, 
        //                        TRUNC(SYSDATE) - TRUNC(VOUCHER_DATE) AS Age, 
        //                        CLOSING_BALANCE 
        //                    FROM 
        //                    (
        //                        SELECT SUB_CODE, VOUCHER_NO, VOUCHER_DATE, DR_AMOUNT, CR_AMOUNT, qw, 
        //                            (DR_AMOUNT - NVL(CR_AMOUNT, 0)) AS NET_AMOUNT, 
        //                            SUM(DR_AMOUNT - NVL(CR_AMOUNT, 0)) OVER (PARTITION BY TO_NUMBER(SUBSTR(SUB_CODE, 2, 99)) ORDER BY VOUCHER_DATE, VOUCHER_NO) AS CLOSING_BALANCE 
        //                        FROM 
        //                        (
        //                            SELECT SUB_CODE, VOUCHER_NO, VOUCHER_DATE, DR_AMOUNT, qw, 
        //                                CASE WHEN qw = 1 THEN CR_AMOUNT ELSE 0 END AS CR_AMOUNT 
        //                            FROM 
        //                            (
        //                                SELECT SUB_CODE, VOUCHER_NO, VOUCHER_DATE, DR_AMOUNT, CR_AMOUNT, 
        //                                    ROW_NUMBER() OVER (PARTITION BY TO_NUMBER(SUBSTR(SUB_CODE, 2, 9999)) ORDER BY TO_NUMBER(SUBSTR(SUB_CODE, 2, 9999)), VOUCHER_NO) AS qw 
        //                                FROM 
        //                                (
        //                                    SELECT SUB_CODE, VOUCHER_NO, VOUCHER_DATE, DR_AMOUNT, 
        //                                        SUM(CR_AMOUNT) OVER (PARTITION BY SUB_CODE) AS CR_AMOUNT 
        //                                    FROM V$VIRTUAL_SUB_LEDGER 
        //                                    WHERE COMPANY_CODE = '{companyCode}' 
        //                                        AND SUBSTR(SUB_CODE, 1, 1) = 'C' 
        //                                        {cusCode} {grpCode}
        //                                        AND VOUCHER_NO NOT IN 
        //                                        (
        //                                            SELECT DISTINCT NVL(BOUNCE_VC_NO, 'A000000') 
        //                                            FROM FA_PDC_RECEIPTS 
        //                                            WHERE (ENCASH_DATE IS NOT NULL OR BOUNCE_DATE IS NOT NULL) 
        //                                                AND BOUNCE_FLAG = 'Y'
        //                                        )
        //                                        AND FORM_CODE NOT IN 
        //                                        (
        //                                            SELECT DISTINCT FORM_CODE 
        //                                            FROM FORM_DETAIL_SETUP 
        //                                            WHERE TABLE_NAME = 'FA_PAY_ORDER' 
        //                                                AND COMPANY_CODE = '{companyCode}'
        //                                        )
        //                                    ORDER BY TO_NUMBER(SUBSTR(SUB_CODE, 2, 99)), VOUCHER_DATE, VOUCHER_NO
        //                                ) 
        //                                WHERE DR_AMOUNT != 0
        //                            )
        //                        )
        //                    )
        //                    WHERE CLOSING_BALANCE > 0
        //                )
        //                UNION ALL 
        //                SELECT TO_NUMBER(SUBSTR(SUB_CODE, 2, 99)) AS SubCode, 
        //                    VOUCHER_DATE, VOUCHER_NO, 
        //                    (PENDING_BAL * -1) AS PENDING_BAL, 
        //                    AGE, CLOSING_BALANCE 
        //                FROM 
        //                (
        //                    SELECT SUB_CODE, VOUCHER_DATE, VOUCHER_NO, 
        //                        CASE WHEN qw = 1 THEN NET_AMOUNT ELSE DR_AMOUNT END AS PENDING_BAL, 
        //                        TRUNC(SYSDATE) - TRUNC(VOUCHER_DATE) AS Age, 
        //                        CLOSING_BALANCE 
        //                    FROM 
        //                    (
        //                        SELECT SUB_CODE, VOUCHER_NO, VOUCHER_DATE, DR_AMOUNT, CR_AMOUNT, qw, 
        //                            (DR_AMOUNT - NVL(CR_AMOUNT, 0)) AS NET_AMOUNT, 
        //                            SUM(DR_AMOUNT - NVL(CR_AMOUNT, 0)) OVER (PARTITION BY TO_NUMBER(SUBSTR(SUB_CODE, 2, 99)) ORDER BY VOUCHER_DATE, VOUCHER_NO) AS CLOSING_BALANCE 
        //                        FROM 
        //                        (
        //                            SELECT SUB_CODE, VOUCHER_NO, VOUCHER_DATE, DR_AMOUNT, qw, 
        //                                CASE WHEN qw = 1 THEN CR_AMOUNT ELSE 0 END AS CR_AMOUNT 
        //                            FROM 
        //                            (
        //                                SELECT SUB_CODE, VOUCHER_NO, VOUCHER_DATE, DR_AMOUNT, CR_AMOUNT, 
        //                                    ROW_NUMBER() OVER (PARTITION BY TO_NUMBER(SUBSTR(SUB_CODE, 2, 9999)) ORDER BY TO_NUMBER(SUBSTR(SUB_CODE, 2, 9999)), VOUCHER_NO) AS qw 
        //                                FROM 
        //                                (
        //                                    SELECT SUB_CODE, VOUCHER_NO, VOUCHER_DATE, CR_AMOUNT AS DR_AMOUNT, 
        //                                        SUM(DR_AMOUNT) OVER (PARTITION BY SUB_CODE) AS CR_AMOUNT 
        //                                    FROM V$VIRTUAL_SUB_LEDGER 
        //                                    WHERE COMPANY_CODE = '{companyCode}' 
        //                                        AND SUBSTR(SUB_CODE, 1, 1) = 'C' 
        //                                        {cusCode} {grpCode}
        //                                        AND VOUCHER_NO NOT IN 
        //                                        (
        //                                            SELECT DISTINCT NVL(BOUNCE_VC_NO, 'A000000') 
        //                                            FROM FA_PDC_RECEIPTS 
        //                                            WHERE (ENCASH_DATE IS NOT NULL OR BOUNCE_DATE IS NOT NULL) 
        //                                                AND BOUNCE_FLAG = 'Y'
        //                                        )
        //                                        AND FORM_CODE NOT IN 
        //                                        (
        //                                            SELECT DISTINCT FORM_CODE 
        //                                            FROM FORM_DETAIL_SETUP 
        //                                            WHERE TABLE_NAME = 'FA_PAY_ORDER' 
        //                                                AND COMPANY_CODE = '{companyCode}'
        //                                        )
        //                                    ORDER BY TO_NUMBER(SUBSTR(SUB_CODE, 2, 99)), VOUCHER_DATE, VOUCHER_NO
        //                                ) 
        //                                WHERE DR_AMOUNT != 0
        //                            )
        //                        )
        //                    )
        //                    WHERE CLOSING_BALANCE > 0
        //                )
        //            ) 
        //            ORDER BY 1
        //        ";

        //        List<Dictionary<string, object>> results = ExecuteQuery(query);
        //        //return Ok(results);
        //        try
        //        {
        //            int interval = requestData.Interval;

        //            // Initialize collections
        //            List<CodeData> allCodes = requestData.AllCodes;
        //            List<BalanceDetails> first030 = new List<BalanceDetails>();
        //            List<BalanceDetails> third3160 = new List<BalanceDetails>();
        //            List<BalanceDetails> fourth6190 = new List<BalanceDetails>();
        //            List<BalanceDetails> sixth91120 = new List<BalanceDetails>();
        //            List<BalanceDetails> seventh120 = new List<BalanceDetails>();

        //            Dictionary<string, CustomerSummary> allData = new Dictionary<string, CustomerSummary>();
        //            List<float> allBal = new List<float>();
        //            string subCode = "";
        //            int x = 0;
        //            float bal = 0;
        //            int days = 0;
        //            string vNo = "";

        //            foreach (var code in allCodes)
        //            {
        //                if (x == 0)
        //                {
        //                    subCode = code.SubCode;
        //                    allBal = new List<float>();
        //                }
        //                else
        //                {
        //                    allBal.Add(bal);
        //                    days = code.Days;
        //                    if (days <= interval)
        //                        first030.Add(new BalanceDetails { Amt = bal, VNo = vNo, Days = days });
        //                    else if (days <= interval * 2)
        //                        third3160.Add(new BalanceDetails { Amt = bal, VNo = vNo, Days = days });
        //                    else if (days <= interval * 3)
        //                        fourth6190.Add(new BalanceDetails { Amt = bal, VNo = vNo, Days = days });
        //                    else if (days <= interval * 4)
        //                        sixth91120.Add(new BalanceDetails { Amt = bal, VNo = vNo, Days = days });
        //                    else
        //                        seventh120.Add(new BalanceDetails { Amt = bal, VNo = vNo, Days = days });
        //                }

        //                if (subCode != code.SubCode)
        //                {
        //                    allData[subCode] = new CustomerSummary
        //                    {
        //                        SubCode = subCode,
        //                        Total = allBal.Sum(),
        //                        First030 = first030.Sum(e => e.Amt),
        //                        First030Details = first030,
        //                        Third3160 = third3160.Sum(e => e.Amt),
        //                        Third3160Details = third3160,
        //                        Fourth6190 = fourth6190.Sum(e => e.Amt),
        //                        Fourth6190Details = fourth6190,
        //                        Sixth91120 = sixth91120.Sum(e => e.Amt),
        //                        Sixth91120Details = sixth91120,
        //                        Seventh120 = seventh120.Sum(e => e.Amt),
        //                        Seventh120Details = seventh120
        //                    };

        //                    // Reset for next group
        //                    allBal.Clear();
        //                    first030.Clear();
        //                    third3160.Clear();
        //                    fourth6190.Clear();
        //                    sixth91120.Clear();
        //                    seventh120.Clear();
        //                }

        //                subCode = code.SubCode;
        //                bal = code.Balance;
        //                days = code.Days;
        //                vNo = code.VNo;
        //                x++;
        //            }

        //            allBal.Add(bal);
        //            if (days <= interval)
        //                first030.Add(new BalanceDetails { Amt = bal, VNo = vNo, Days = days });
        //            else if (days <= interval * 2)
        //                third3160.Add(new BalanceDetails { Amt = bal, VNo = vNo, Days = days });
        //            else if (days <= interval * 3)
        //                fourth6190.Add(new BalanceDetails { Amt = bal, VNo = vNo, Days = days });
        //            else if (days <= interval * 4)
        //                sixth91120.Add(new BalanceDetails { Amt = bal, VNo = vNo, Days = days });
        //            else
        //                seventh120.Add(new BalanceDetails { Amt = bal, VNo = vNo, Days = days });

        //            allData[subCode] = new CustomerSummary
        //            {
        //                SubCode = subCode,
        //                Total = allBal.Sum(),
        //                First030 = first030.Sum(e => e.Amt),
        //                First030Details = first030,
        //                Third3160 = third3160.Sum(e => e.Amt),
        //                Third3160Details = third3160,
        //                Fourth6190 = fourth6190.Sum(e => e.Amt),
        //                Fourth6190Details = fourth6190,
        //                Sixth91120 = sixth91120.Sum(e => e.Amt),
        //                Sixth91120Details = sixth91120,
        //                Seventh120 = seventh120.Sum(e => e.Amt),
        //                Seventh120Details = seventh120
        //            };

        //            List<CustomerDetail> allCus = new List<CustomerDetail>();
        //            foreach (var customerCode in allCodes)
        //            {
        //                var totalData = allData.ContainsKey(customerCode.SubCode) ? allData[customerCode.SubCode] : new CustomerSummary();
        //                allCus.Add(new CustomerDetail
        //                {
        //                    CustomerCode = customerCode.SubCode,
        //                    Total = totalData.Total,
        //                    First030 = totalData.First030,
        //                    First030Details = totalData.First030Details,
        //                    Third3160 = totalData.Third3160,
        //                    Third3160Details = totalData.Third3160Details,
        //                    Fourth6190 = totalData.Fourth6190,
        //                    Fourth6190Details = totalData.Fourth6190Details,
        //                    Sixth91120 = totalData.Sixth91120,
        //                    Sixth91120Details = totalData.Sixth91120Details,
        //                    Seventh120 = totalData.Seventh120,
        //                    Seventh120Details = totalData.Seventh120Details
        //                });
        //            }

        //            return Ok(new { Detail = allCus, Total = allCus.Sum(c => c.Total) });


        //        }
        //        catch (Exception ex)
        //        {
        //            //return StatusCode(500, ex.Message);
        //        }
        //    }
        //}


        //[HttpPost()]
        //public IHttpActionResult AllCustomerAgeing([FromBody] AgeingRequest request)
        //{
        //    try
        //    {
        //        string companyCode = request.CompanyCode[0];

        //        // Handling optional parameters for cusCode and grpCode
        //        string cusCode = string.IsNullOrEmpty(request.CusCode) ? "" : $"AND SUB_CODE = '{request.CusCode}'";
        //        string grpCode = string.IsNullOrEmpty(request.GrpCode) ? "" :
        //            $"AND SUB_CODE IN (SELECT 'C' + CUSTOMER_CODE FROM SA_CUSTOMER_SETUP WHERE MASTER_CUSTOMER_CODE LIKE '{request.GrpCode}%' AND COMPANY_CODE = '{companyCode}')";

        //        // Query for customer ageing
        //        string query = $@"
        //    SELECT * FROM (
        //        SELECT * FROM (
        //            SELECT TO_NUMBER(SUBSTR(SUB_CODE, 2, 99)) AS SubCode, 
        //                VOUCHER_DATE, VOUCHER_NO, 
        //                CASE WHEN qw = 1 THEN NET_AMOUNT ELSE DR_AMOUNT END AS PENDING_BAL, 
        //                TRUNC(SYSDATE) - TRUNC(VOUCHER_DATE) AS Age, 
        //                CLOSING_BALANCE 
        //            FROM (
        //                SELECT SUB_CODE, VOUCHER_NO, VOUCHER_DATE, DR_AMOUNT, CR_AMOUNT, qw, 
        //                    (DR_AMOUNT - NVL(CR_AMOUNT, 0)) AS NET_AMOUNT, 
        //                    SUM(DR_AMOUNT - NVL(CR_AMOUNT, 0)) OVER (PARTITION BY TO_NUMBER(SUBSTR(SUB_CODE, 2, 99)) ORDER BY VOUCHER_DATE, VOUCHER_NO) AS CLOSING_BALANCE 
        //                FROM (
        //                    SELECT SUB_CODE, VOUCHER_NO, VOUCHER_DATE, DR_AMOUNT, qw, 
        //                        CASE WHEN qw = 1 THEN CR_AMOUNT ELSE 0 END AS CR_AMOUNT 
        //                    FROM (
        //                        SELECT SUB_CODE, VOUCHER_NO, VOUCHER_DATE, DR_AMOUNT, CR_AMOUNT, 
        //                            ROW_NUMBER() OVER (PARTITION BY TO_NUMBER(SUBSTR(SUB_CODE, 2, 9999)) ORDER BY TO_NUMBER(SUBSTR(SUB_CODE, 2, 9999)), VOUCHER_NO) AS qw 
        //                        FROM (
        //                            SELECT SUB_CODE, VOUCHER_NO, VOUCHER_DATE, DR_AMOUNT, 
        //                                SUM(CR_AMOUNT) OVER (PARTITION BY SUB_CODE) AS CR_AMOUNT 
        //                            FROM V$VIRTUAL_SUB_LEDGER 
        //                            WHERE COMPANY_CODE = '{companyCode}' 
        //                                AND SUBSTR(SUB_CODE, 1, 1) = 'C' 
        //                                {cusCode} {grpCode}
        //                                AND VOUCHER_NO NOT IN (
        //                                    SELECT DISTINCT NVL(BOUNCE_VC_NO, 'A000000') 
        //                                    FROM FA_PDC_RECEIPTS 
        //                                    WHERE (ENCASH_DATE IS NOT NULL OR BOUNCE_DATE IS NOT NULL) 
        //                                        AND BOUNCE_FLAG = 'Y'
        //                                )
        //                                AND FORM_CODE NOT IN (
        //                                    SELECT DISTINCT FORM_CODE 
        //                                    FROM FORM_DETAIL_SETUP 
        //                                    WHERE TABLE_NAME = 'FA_PAY_ORDER' 
        //                                        AND COMPANY_CODE = '{companyCode}'
        //                                )
        //                            ORDER BY TO_NUMBER(SUBSTR(SUB_CODE, 2, 99)), VOUCHER_DATE, VOUCHER_NO
        //                        ) 
        //                        WHERE DR_AMOUNT != 0
        //                    )
        //                )
        //            )
        //            WHERE CLOSING_BALANCE > 0
        //        )
        //        UNION ALL 
        //        SELECT TO_NUMBER(SUBSTR(SUB_CODE, 2, 99)) AS SubCode, 
        //            VOUCHER_DATE, VOUCHER_NO, 
        //            (PENDING_BAL * -1) AS PENDING_BAL, 
        //            AGE, CLOSING_BALANCE 
        //        FROM (
        //            SELECT SUB_CODE, VOUCHER_DATE, VOUCHER_NO, 
        //                CASE WHEN qw = 1 THEN NET_AMOUNT ELSE DR_AMOUNT END AS PENDING_BAL, 
        //                TRUNC(SYSDATE) - TRUNC(VOUCHER_DATE) AS Age, 
        //                CLOSING_BALANCE 
        //            FROM (
        //                SELECT SUB_CODE, VOUCHER_NO, VOUCHER_DATE, DR_AMOUNT, CR_AMOUNT, qw, 
        //                    (DR_AMOUNT - NVL(CR_AMOUNT, 0)) AS NET_AMOUNT, 
        //                    SUM(DR_AMOUNT - NVL(CR_AMOUNT, 0)) OVER (PARTITION BY TO_NUMBER(SUBSTR(SUB_CODE, 2, 99)) ORDER BY VOUCHER_DATE, VOUCHER_NO) AS CLOSING_BALANCE 
        //                FROM (
        //                    SELECT SUB_CODE, VOUCHER_NO, VOUCHER_DATE, DR_AMOUNT, qw, 
        //                        CASE WHEN qw = 1 THEN CR_AMOUNT ELSE 0 END AS CR_AMOUNT 
        //                    FROM (
        //                        SELECT SUB_CODE, VOUCHER_NO, VOUCHER_DATE, DR_AMOUNT, CR_AMOUNT, 
        //                            ROW_NUMBER() OVER (PARTITION BY TO_NUMBER(SUBSTR(SUB_CODE, 2, 9999)) ORDER BY TO_NUMBER(SUBSTR(SUB_CODE, 2, 9999)), VOUCHER_NO) AS qw 
        //                        FROM (
        //                            SELECT SUB_CODE, VOUCHER_NO, VOUCHER_DATE, CR_AMOUNT AS DR_AMOUNT, 
        //                                SUM(DR_AMOUNT) OVER (PARTITION BY SUB_CODE) AS CR_AMOUNT 
        //                            FROM V$VIRTUAL_SUB_LEDGER 
        //                            WHERE COMPANY_CODE = '{companyCode}' 
        //                                AND SUBSTR(SUB_CODE, 1, 1) = 'C' 
        //                                {cusCode} {grpCode}
        //                                AND VOUCHER_NO NOT IN (
        //                                    SELECT DISTINCT NVL(BOUNCE_VC_NO, 'A000000') 
        //                                    FROM FA_PDC_RECEIPTS 
        //                                    WHERE (ENCASH_DATE IS NOT NULL OR BOUNCE_DATE IS NOT NULL) 
        //                                        AND BOUNCE_FLAG = 'Y'
        //                                )
        //                                AND FORM_CODE NOT IN (
        //                                    SELECT DISTINCT FORM_CODE 
        //                                    FROM FORM_DETAIL_SETUP 
        //                                    WHERE TABLE_NAME = 'FA_PAY_ORDER' 
        //                                        AND COMPANY_CODE = '{companyCode}'
        //                                )
        //                            ORDER BY TO_NUMBER(SUBSTR(SUB_CODE, 2, 99)), VOUCHER_DATE, VOUCHER_NO
        //                        ) 
        //                        WHERE DR_AMOUNT != 0
        //                    )
        //                )
        //            )
        //            WHERE CLOSING_BALANCE > 0
        //        )
        //    ) 
        //    ORDER BY 1";

        //        // Execute query and process results
        //        List<Dictionary<string, object>> results = ExecuteQuery(query);

        //        try
        //        {
        //            int interval = request.Interval;

        //            // Initialize collections for ageing categories
        //            List<CodeData> allCodes = request.AllCodes;
        //            List<BalanceDetails> first030 = new List<BalanceDetails>();
        //            List<BalanceDetails> third3160 = new List<BalanceDetails>();
        //            List<BalanceDetails> fourth6190 = new List<BalanceDetails>();
        //            List<BalanceDetails> sixth91120 = new List<BalanceDetails>();
        //            List<BalanceDetails> seventh120 = new List<BalanceDetails>();

        //            Dictionary<string, CustomerSummary> allData = new Dictionary<string, CustomerSummary>();
        //            List<float> allBal = new List<float>();
        //            string subCode = "";
        //            float bal = 0;
        //            int days = 0;
        //            string vNo = "";

        //            foreach (var code in allCodes)
        //            {
        //                if (subCode != code.SubCode)
        //                {
        //                    if (!string.IsNullOrEmpty(subCode))
        //                    {
        //                        allData[subCode] = new CustomerSummary
        //                        {
        //                            SubCode = subCode,
        //                            Total = allBal.Sum(),
        //                            First030 = first030.Sum(e => e.Amt),
        //                            First030Details = new List<BalanceDetails>(first030),
        //                            Third3160 = third3160.Sum(e => e.Amt),
        //                            Third3160Details = new List<BalanceDetails>(third3160),
        //                            Fourth6190 = fourth6190.Sum(e => e.Amt),
        //                            Fourth6190Details = new List<BalanceDetails>(fourth6190),
        //                            Sixth91120 = sixth91120.Sum(e => e.Amt),
        //                            Sixth91120Details = new List<BalanceDetails>(sixth91120),
        //                            Seventh120 = seventh120.Sum(e => e.Amt),
        //                            Seventh120Details = new List<BalanceDetails>(seventh120)
        //                        };
        //                    }

        //                    // Reset for next customer
        //                    allBal.Clear();
        //                    first030.Clear();
        //                    third3160.Clear();
        //                    fourth6190.Clear();
        //                    sixth91120.Clear();
        //                    seventh120.Clear();
        //                    subCode = code.SubCode;
        //                }

        //                bal = code.Balance;
        //                days = code.Days;
        //                vNo = code.VNo;
        //                allBal.Add(bal);

        //                // Categorize by interval
        //                if (days <= interval)
        //                    first030.Add(new BalanceDetails { Amt = bal, VNo = vNo, Days = days });
        //                else if (days <= interval * 2)
        //                    third3160.Add(new BalanceDetails { Amt = bal, VNo = vNo, Days = days });
        //                else if (days <= interval * 3)
        //                    fourth6190.Add(new BalanceDetails { Amt = bal, VNo = vNo, Days = days });
        //                else if (days <= interval * 4)
        //                    sixth91120.Add(new BalanceDetails { Amt = bal, VNo = vNo, Days = days });
        //                else
        //                    seventh120.Add(new BalanceDetails { Amt = bal, VNo = vNo, Days = days });
        //            }

        //            // Process last customer
        //            if (!string.IsNullOrEmpty(subCode))
        //            {
        //                allData[subCode] = new CustomerSummary
        //                {
        //                    SubCode = subCode,
        //                    Total = allBal.Sum(),
        //                    First030 = first030.Sum(e => e.Amt),
        //                    First030Details = new List<BalanceDetails>(first030),
        //                    Third3160 = third3160.Sum(e => e.Amt),
        //                    Third3160Details = new List<BalanceDetails>(third3160),
        //                    Fourth6190 = fourth6190.Sum(e => e.Amt),
        //                    Fourth6190Details = new List<BalanceDetails>(fourth6190),
        //                    Sixth91120 = sixth91120.Sum(e => e.Amt),
        //                    Sixth91120Details = new List<BalanceDetails>(sixth91120),
        //                    Seventh120 = seventh120.Sum(e => e.Amt),
        //                    Seventh120Details = new List<BalanceDetails>(seventh120)
        //                };
        //            }

        //            return Ok(allData);
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"An error occurred while fetching customer ageing data: {ex.Message}");
        //    }
        //}

        [HttpPost()]
        public IHttpActionResult AllCustomerAgeing([FromBody] AgeingRequest request)
        {
            try
            {
                string companyCode = request.CompanyCode[0];

                // Handling optional parameters for cusCode and grpCode
                string cusCode = string.IsNullOrEmpty(request.CusCode) ? "" : $"AND SUB_CODE = '{request.CusCode}'";
                string grpCode = string.IsNullOrEmpty(request.GrpCode) ? "" :
                    $"AND SUB_CODE IN (SELECT 'C' + CUSTOMER_CODE FROM SA_CUSTOMER_SETUP WHERE MASTER_CUSTOMER_CODE LIKE '{request.GrpCode}%' AND COMPANY_CODE = '{companyCode}')";

                // Query for customer ageing
                string query = $@"
            SELECT * FROM (
                SELECT * FROM (
                    SELECT TO_NUMBER(SUBSTR(SUB_CODE, 2, 99)) AS SubCode, 
                        VOUCHER_DATE, VOUCHER_NO, 
                        CASE WHEN qw = 1 THEN NET_AMOUNT ELSE DR_AMOUNT END AS PENDING_BAL, 
                        TRUNC(SYSDATE) - TRUNC(VOUCHER_DATE) AS Age, 
                        CLOSING_BALANCE 
                    FROM (
                        SELECT SUB_CODE, VOUCHER_NO, VOUCHER_DATE, DR_AMOUNT, CR_AMOUNT, qw, 
                            (DR_AMOUNT - NVL(CR_AMOUNT, 0)) AS NET_AMOUNT, 
                            SUM(DR_AMOUNT - NVL(CR_AMOUNT, 0)) OVER (PARTITION BY TO_NUMBER(SUBSTR(SUB_CODE, 2, 99)) ORDER BY VOUCHER_DATE, VOUCHER_NO) AS CLOSING_BALANCE 
                        FROM (
                            SELECT SUB_CODE, VOUCHER_NO, VOUCHER_DATE, DR_AMOUNT, qw, 
                                CASE WHEN qw = 1 THEN CR_AMOUNT ELSE 0 END AS CR_AMOUNT 
                            FROM (
                                SELECT SUB_CODE, VOUCHER_NO, VOUCHER_DATE, DR_AMOUNT, CR_AMOUNT, 
                                    ROW_NUMBER() OVER (PARTITION BY TO_NUMBER(SUBSTR(SUB_CODE, 2, 9999)) ORDER BY TO_NUMBER(SUBSTR(SUB_CODE, 2, 9999)), VOUCHER_NO) AS qw 
                                FROM (
                                    SELECT SUB_CODE, VOUCHER_NO, VOUCHER_DATE, DR_AMOUNT, 
                                        SUM(CR_AMOUNT) OVER (PARTITION BY SUB_CODE) AS CR_AMOUNT 
                                    FROM V$VIRTUAL_SUB_LEDGER 
                                    WHERE COMPANY_CODE = '{companyCode}' 
                                        AND SUBSTR(SUB_CODE, 1, 1) = 'C' 
                                        {cusCode} {grpCode}
                                        AND VOUCHER_NO NOT IN (
                                            SELECT DISTINCT NVL(BOUNCE_VC_NO, 'A000000') 
                                            FROM FA_PDC_RECEIPTS 
                                            WHERE (ENCASH_DATE IS NOT NULL OR BOUNCE_DATE IS NOT NULL) 
                                                AND BOUNCE_FLAG = 'Y'
                                        )
                                        AND FORM_CODE NOT IN (
                                            SELECT DISTINCT FORM_CODE 
                                            FROM FORM_DETAIL_SETUP 
                                            WHERE TABLE_NAME = 'FA_PAY_ORDER' 
                                                AND COMPANY_CODE = '{companyCode}'
                                        )
                                    ORDER BY TO_NUMBER(SUBSTR(SUB_CODE, 2, 99)), VOUCHER_DATE, VOUCHER_NO
                                ) 
                                WHERE DR_AMOUNT != 0
                            )
                        )
                    )
                    WHERE CLOSING_BALANCE > 0
                )
                UNION ALL 
                SELECT TO_NUMBER(SUBSTR(SUB_CODE, 2, 99)) AS SubCode, 
                    VOUCHER_DATE, VOUCHER_NO, 
                    (PENDING_BAL * -1) AS PENDING_BAL, 
                    AGE, CLOSING_BALANCE 
                FROM (
                    SELECT SUB_CODE, VOUCHER_DATE, VOUCHER_NO, 
                        CASE WHEN qw = 1 THEN NET_AMOUNT ELSE DR_AMOUNT END AS PENDING_BAL, 
                        TRUNC(SYSDATE) - TRUNC(VOUCHER_DATE) AS Age, 
                        CLOSING_BALANCE 
                    FROM (
                        SELECT SUB_CODE, VOUCHER_NO, VOUCHER_DATE, DR_AMOUNT, CR_AMOUNT, qw, 
                            (DR_AMOUNT - NVL(CR_AMOUNT, 0)) AS NET_AMOUNT, 
                            SUM(DR_AMOUNT - NVL(CR_AMOUNT, 0)) OVER (PARTITION BY TO_NUMBER(SUBSTR(SUB_CODE, 2, 99)) ORDER BY VOUCHER_DATE, VOUCHER_NO) AS CLOSING_BALANCE 
                        FROM (
                            SELECT SUB_CODE, VOUCHER_NO, VOUCHER_DATE, DR_AMOUNT, qw, 
                                CASE WHEN qw = 1 THEN CR_AMOUNT ELSE 0 END AS CR_AMOUNT 
                            FROM (
                                SELECT SUB_CODE, VOUCHER_NO, VOUCHER_DATE, DR_AMOUNT, CR_AMOUNT, 
                                    ROW_NUMBER() OVER (PARTITION BY TO_NUMBER(SUBSTR(SUB_CODE, 2, 9999)) ORDER BY TO_NUMBER(SUBSTR(SUB_CODE, 2, 9999)), VOUCHER_NO) AS qw 
                                FROM (
                                    SELECT SUB_CODE, VOUCHER_NO, VOUCHER_DATE, CR_AMOUNT AS DR_AMOUNT, 
                                        SUM(DR_AMOUNT) OVER (PARTITION BY SUB_CODE) AS CR_AMOUNT 
                                    FROM V$VIRTUAL_SUB_LEDGER 
                                    WHERE COMPANY_CODE = '{companyCode}' 
                                        AND SUBSTR(SUB_CODE, 1, 1) = 'C' 
                                        {cusCode} {grpCode}
                                        AND VOUCHER_NO NOT IN (
                                            SELECT DISTINCT NVL(BOUNCE_VC_NO, 'A000000') 
                                            FROM FA_PDC_RECEIPTS 
                                            WHERE (ENCASH_DATE IS NOT NULL OR BOUNCE_DATE IS NOT NULL) 
                                                AND BOUNCE_FLAG = 'Y'
                                        )
                                        AND FORM_CODE NOT IN (
                                            SELECT DISTINCT FORM_CODE 
                                            FROM FORM_DETAIL_SETUP 
                                            WHERE TABLE_NAME = 'FA_PAY_ORDER' 
                                                AND COMPANY_CODE = '{companyCode}'
                                        )
                                    ORDER BY TO_NUMBER(SUBSTR(SUB_CODE, 2, 99)), VOUCHER_DATE, VOUCHER_NO
                                ) 
                                WHERE DR_AMOUNT != 0
                            )
                        )
                    )
                    WHERE CLOSING_BALANCE > 0
                )
            ) 
            ORDER BY 1";

                // Execute query and process results
                List<Dictionary<string, object>> results = ExecuteQuery(query);

                try
                {
                    int interval = Convert.ToInt32(request.Interval);
                    List<CodeData> allCodes = request.AllCodes;
                    List<BalanceDetails> first030 = new List<BalanceDetails>();
                    List<BalanceDetails> third3160 = new List<BalanceDetails>();
                    List<BalanceDetails> fourth6190 = new List<BalanceDetails>();
                    List<BalanceDetails> sixth91120 = new List<BalanceDetails>();
                    List<BalanceDetails> seventh120 = new List<BalanceDetails>();

                    Dictionary<string, CustomerSummary> allData = new Dictionary<string, CustomerSummary>();
                    List<float> allBal = new List<float>();
                    string subCode = "";
                    float bal = 0;
                    int days = 0;
                    string vNo = "";

                    foreach (var code in allCodes)
                    {
                        if (subCode != code.SubCode)
                        {
                            if (!string.IsNullOrEmpty(subCode))
                            {
                                allData[subCode] = new CustomerSummary
                                {
                                    SubCode = subCode,
                                    Total = allBal.Sum(),
                                    First030 = first030.Sum(e => e.Amt),
                                    First030Details = new List<BalanceDetails>(first030),
                                    Third3160 = third3160.Sum(e => e.Amt),
                                    Third3160Details = new List<BalanceDetails>(third3160),
                                    Fourth6190 = fourth6190.Sum(e => e.Amt),
                                    Fourth6190Details = new List<BalanceDetails>(fourth6190),
                                    Sixth91120 = sixth91120.Sum(e => e.Amt),
                                    Sixth91120Details = new List<BalanceDetails>(sixth91120),
                                    Seventh120 = seventh120.Sum(e => e.Amt),
                                    Seventh120Details = new List<BalanceDetails>(seventh120)
                                };
                            }

                            // Reset for next customer
                            allBal.Clear();
                            first030.Clear();
                            third3160.Clear();
                            fourth6190.Clear();
                            sixth91120.Clear();
                            seventh120.Clear();
                            subCode = code.SubCode;
                        }

                        bal = code.Balance;
                        days = code.Days;
                        vNo = code.VNo;
                        allBal.Add(bal);

                        // Categorize by interval
                        if (days <= interval)
                            first030.Add(new BalanceDetails { Amt = bal, VNo = vNo, Days = days });
                        else if (days <= interval * 2)
                            third3160.Add(new BalanceDetails { Amt = bal, VNo = vNo, Days = days });
                        else if (days <= interval * 3)
                            fourth6190.Add(new BalanceDetails { Amt = bal, VNo = vNo, Days = days });
                        else if (days <= interval * 4)
                            sixth91120.Add(new BalanceDetails { Amt = bal, VNo = vNo, Days = days });
                        else
                            seventh120.Add(new BalanceDetails { Amt = bal, VNo = vNo, Days = days });
                    }

                    // Process last customer
                    if (!string.IsNullOrEmpty(subCode))
                    {
                        allData[subCode] = new CustomerSummary
                        {
                            SubCode = subCode,
                            Total = allBal.Sum(),
                            First030 = first030.Sum(e => e.Amt),
                            First030Details = new List<BalanceDetails>(first030),
                            Third3160 = third3160.Sum(e => e.Amt),
                            Third3160Details = new List<BalanceDetails>(third3160),
                            Fourth6190 = fourth6190.Sum(e => e.Amt),
                            Fourth6190Details = new List<BalanceDetails>(fourth6190),
                            Sixth91120 = sixth91120.Sum(e => e.Amt),
                            Sixth91120Details = new List<BalanceDetails>(sixth91120),
                            Seventh120 = seventh120.Sum(e => e.Amt),
                            Seventh120Details = new List<BalanceDetails>(seventh120)
                        };
                    }

                    return Ok(allData);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while fetching customer ageing data: {ex.Message}");
            }
        }

        //[HttpGet()]
        //public async Task<IHttpActionResult> GetSuppliersAging(
        //[FromBody] string companyCode,
        //[FromBody] int interval,
        //[FromBody] string grpCode,
        //[FromBody] string cusCode)
        //{
        //    // SQL query construction
        //    var query = $@"
        //    SELECT * FROM VirtualSubLedgers
        //    WHERE CompanyCode = @p0
        //    AND VoucherNo != '0'
        //    AND FormCode NOT IN (SELECT FormCode FROM FormDetailSetup WHERE TableName = 'FA_PAY_ORDER' AND CompanyCode = @p0)
        //    {(string.IsNullOrEmpty(cusCode) ? "" : $"AND SubCode = '{cusCode}'")}
        //    {(string.IsNullOrEmpty(grpCode) ? "" : $"AND SubCode IN (SELECT 'C' || SupplierCode FROM SupplierSetups WHERE MasterSupplierCode LIKE '{grpCode}%' AND CompanyCode = '{companyCode}')")}
        //";

        //    var virtualSubLedgers = await _context.VirtualSubLedgers
        //        .FromSqlRaw(query, companyCode)
        //        .ToListAsync();

        //    // Process the data to calculate aging report
        //    var allData = ProcessSupplierAgingData(virtualSubLedgers, interval);

        //    var suppliers = await _context.SupplierSetups
        //        .Where(s => s.CompanyCode == companyCode && !s.DeletedFlag)
        //        .OrderBy(s => s.MasterSupplierCode)
        //        .ToListAsync();

        //    var report = new List<SupplierAgingReport>();
        //    foreach (var supplier in suppliers)
        //    {
        //        var subCode = int.Parse(supplier.SupplierCode.Substring(1)); // Assuming subCode is numeric part of SupplierCode
        //        var data = allData.ContainsKey(subCode) ? allData[subCode] : new SupplierAgingReport();

        //        report.Add(new SupplierAgingReport
        //        {
        //            CustomerCode = supplier.SupplierCode,
        //            CustomerEdesc = supplier.SupplierEdesc,
        //            GroupSkuFlag = supplier.GroupSkuFlag,
        //            MasterCustomerCode = supplier.MasterSupplierCode,
        //            PreCustomerCode = supplier.PreSupplierCode,
        //            Total = data.Total,
        //            Total_030 = data.Total_030,
        //            Total_030_Detail = data.Total_030_Detail,
        //            Total_3160 = data.Total_3160,
        //            Total_3160_Detail = data.Total_3160_Detail,
        //            Total_6190 = data.Total_6190,
        //            Total_6190_Detail = data.Total_6190_Detail,
        //            Total_91120 = data.Total_91120,
        //            Total_91120_Detail = data.Total_91120_Detail,
        //            Total_120Plus = data.Total_120Plus,
        //            Total_120Plus_Detail = data.Total_120Plus_Detail
        //        });
        //    }

        //    return Ok(report);
        //}



        //[HttpPost]
        //public async Task<IHttpActionResult> GetSuppliersAging(
        //[FromBody] string companyCode,
        //[FromBody] int interval,
        //[FromBody] string grpCode,
        //[FromBody] string cusCode)
        //    {
        //        // SQL query construction with parameterized query
        //        var query = $@"
        //    SELECT * FROM VirtualSubLedgers
        //    WHERE CompanyCode = @p0
        //    AND VoucherNo != '0'
        //    AND FormCode NOT IN (SELECT FormCode FROM FormDetailSetup WHERE TableName = 'FA_PAY_ORDER' AND CompanyCode = @p0)
        //    {(!string.IsNullOrEmpty(cusCode) ? "AND SubCode = @p1" : "")}
        //    {(!string.IsNullOrEmpty(grpCode) ? "AND SubCode IN (SELECT 'C' || SupplierCode FROM SupplierSetups WHERE MasterSupplierCode LIKE @p2 AND CompanyCode = @p0)" : "")}
        //";

        //        // Use parameterized queries to avoid SQL injection
        //        var virtualSubLedgers = await _context.VirtualSubLedgers
        //            .FromSqlRaw(query, companyCode, cusCode, grpCode + "%")
        //            .ToListAsync();

        //        // Process the data to calculate aging report
        //        var allData = ProcessSupplierAgingData(virtualSubLedgers, interval);

        //        var suppliers = await _context.SupplierSetups
        //            .Where(s => s.CompanyCode == companyCode && !s.DeletedFlag)
        //            .OrderBy(s => s.MasterSupplierCode)
        //            .ToListAsync();

        //        var report = new List<SupplierAgingReport>();
        //        foreach (var supplier in suppliers)
        //        {
        //            var subCode = int.Parse(supplier.SupplierCode.Substring(1)); 
        //            var data = allData.ContainsKey(subCode) ? allData[subCode] : new SupplierAgingReport();

        //            report.Add(new SupplierAgingReport
        //            {
        //                CustomerCode = supplier.SupplierCode,
        //                CustomerEdesc = supplier.SupplierEdesc,
        //                GroupSkuFlag = supplier.GroupSkuFlag,
        //                MasterCustomerCode = supplier.MasterSupplierCode,
        //                PreCustomerCode = supplier.PreSupplierCode,
        //                Total = data.Total,
        //                Total_030 = data.Total_030,
        //                Total_030_Detail = data.Total_030_Detail,
        //                Total_3160 = data.Total_3160,
        //                Total_3160_Detail = data.Total_3160_Detail,
        //                Total_6190 = data.Total_6190,
        //                Total_6190_Detail = data.Total_6190_Detail,
        //                Total_91120 = data.Total_91120,
        //                Total_91120_Detail = data.Total_91120_Detail,
        //                Total_120Plus = data.Total_120Plus,
        //                Total_120Plus_Detail = data.Total_120Plus_Detail
        //            });
        //        }

        //        return Ok(report);
        //    }

        //[HttpGet()]
        //public async Task<IHttpActionResult> GetSuppliersAging(
        //    [FromBody] string companyCode,
        //    [FromBody] int interval,
        //    [FromBody] string grpCode,
        //    [FromBody] string cusCode)
        //{
        //    // SQL query construction
        //    string query = $@"
        //        SELECT * FROM VirtualSubLedgers
        //        WHERE CompanyCode = :p0
        //        AND VoucherNo != '0'
        //        AND FormCode NOT IN (SELECT FormCode FROM FormDetailSetup WHERE TableName = 'FA_PAY_ORDER' AND CompanyCode = :p0)
        //        {(string.IsNullOrEmpty(cusCode) ? "" : $"AND SubCode = :p1")}
        //        {(string.IsNullOrEmpty(grpCode) ? "" : $"AND SubCode IN (SELECT 'C' || SupplierCode FROM SupplierSetups WHERE MasterSupplierCode LIKE :p2 AND CompanyCode = :p0)")}
        //    ";

        //    // Create a DataTable to hold the result
        //    DataTable virtualSubLedgersTable = new DataTable();

        //    // Oracle ADO.NET setup
        //    using (OracleConnection conn = new OracleConnection(_connection))
        //    {
        //        conn.Open();

        //        using (OracleCommand cmd = new OracleCommand(query, conn))
        //        {
        //            // Add parameters to prevent SQL injection
        //            cmd.Parameters.Add(new OracleParameter("p0", companyCode));

        //            if (!string.IsNullOrEmpty(cusCode))
        //            {
        //                cmd.Parameters.Add(new OracleParameter("p1", cusCode));
        //            }

        //            if (!string.IsNullOrEmpty(grpCode))
        //            {
        //                cmd.Parameters.Add(new OracleParameter("p2", grpCode + "%"));
        //            }

        //            // Use OracleDataAdapter to fill the DataTable
        //            using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
        //            {
        //                adapter.Fill(virtualSubLedgersTable);
        //            }
        //        }
        //    }

        //    // Process the data to calculate the aging report
        //    var allData = ProcessSupplierAgingData(virtualSubLedgersTable, interval);

        //    // Fetch suppliers from another query (replace Entity Framework with ADO.NET)
        //    var suppliers = await GetSuppliers(companyCode);

        //    // Prepare the report
        //    var report = new List<SupplierAgingReport>();
        //    foreach (var supplier in suppliers)
        //    {
        //        var subCode = int.Parse(supplier.SupplierCode.Substring(1)); // Assuming subCode is numeric part of SupplierCode
        //        var data = allData.ContainsKey(subCode) ? allData[subCode] : new SupplierAgingReport();

        //        report.Add(new SupplierAgingReport
        //        {
        //            CustomerCode = supplier.SupplierCode,
        //            CustomerEdesc = supplier.SupplierEdesc,
        //            GroupSkuFlag = supplier.GroupSkuFlag,
        //            MasterCustomerCode = supplier.MasterSupplierCode,
        //            PreCustomerCode = supplier.PreSupplierCode,
        //            Total = data.Total,
        //            Total_030 = data.Total_030,
        //            Total_030_Detail = data.Total_030_Detail,
        //            Total_3160 = data.Total_3160,
        //            Total_3160_Detail = data.Total_3160_Detail,
        //            Total_6190 = data.Total_6190,
        //            Total_6190_Detail = data.Total_6190_Detail,
        //            Total_91120 = data.Total_91120,
        //            Total_91120_Detail = data.Total_91120_Detail,
        //            Total_120Plus = data.Total_120Plus,
        //            Total_120Plus_Detail = data.Total_120Plus_Detail
        //        });
        //    }
        //    return Ok(report);
        //}

        [HttpGet]
        public async Task<IHttpActionResult> GetSuppliersAging(
        [FromUri] string companyCode,   // Changed to [FromUri] for GET request parameters
        [FromUri] int interval,
        [FromUri] string grpCode,
        [FromUri] string cusCode)
        {
            // SQL query construction with parameter placeholders
            string query = $@"
                SELECT * FROM VirtualSubLedgers
                WHERE CompanyCode = :p0
                AND VoucherNo != '0'
                AND FormCode NOT IN (SELECT FormCode FROM FormDetailSetup WHERE TableName = 'FA_PAY_ORDER' AND CompanyCode = :p0)
                {(string.IsNullOrEmpty(cusCode) ? "" : "AND SubCode = :p1")}
                {(string.IsNullOrEmpty(grpCode) ? "" : "AND SubCode IN (SELECT 'C' || SupplierCode FROM SupplierSetups WHERE MasterSupplierCode LIKE :p2 AND CompanyCode = :p0)")}
            ";

            // Create a DataTable to hold the result
            DataTable virtualSubLedgersTable = new DataTable();

            // Oracle ADO.NET setup
            try
            {
                using (OracleConnection conn = new OracleConnection(_connection)) // _connection should be your Oracle connection string
                {
                    await conn.OpenAsync(); // Asynchronous opening of the connection

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        // Add parameters to prevent SQL injection
                        cmd.Parameters.Add(new OracleParameter("p0", companyCode));

                        if (!string.IsNullOrEmpty(cusCode))
                        {
                            cmd.Parameters.Add(new OracleParameter("p1", cusCode));
                        }

                        if (!string.IsNullOrEmpty(grpCode))
                        {
                            cmd.Parameters.Add(new OracleParameter("p2", grpCode + "%"));
                        }

                        // Use OracleDataAdapter to fill the DataTable
                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            adapter.Fill(virtualSubLedgersTable); // Fill data into DataTable
                        }
                    }
                }

                var ledgers = virtualSubLedgersTable.AsEnumerable().Select(row => new VirtualSubLedger
                {
                    CompanyCode = row.Field<string>("CompanyCode"),
                    VoucherNo = row.Field<string>("VoucherNo"),
                    SubCode = row.Field<string>("SubCode"),
                }).ToList();

                // Process the data to calculate the aging report
                var allData = ProcessSupplierAgingData(ledgers, interval);

                // Fetch suppliers using a separate method (this will replace the EF part)
                var suppliers = await GetSuppliers(companyCode);

                // Prepare the report
                var report = new List<SupplierAgingReport>();
                foreach (var supplier in suppliers)
                {
                    // Assuming subCode is numeric part of SupplierCode (adjust if needed)
                    var subCode = int.Parse(supplier.SupplierCode.Substring(1));
                    var data = allData.ContainsKey(subCode) ? allData[subCode] : new SupplierAgingReport();

                    report.Add(new SupplierAgingReport
                    {
                        CustomerCode = supplier.SupplierCode,
                        CustomerEdesc = supplier.SupplierEdesc,
                        GroupSkuFlag = supplier.GroupSkuFlag,
                        MasterCustomerCode = supplier.MasterSupplierCode,
                        PreCustomerCode = supplier.PreSupplierCode,
                        Total = data.Total,
                        Total_030 = data.Total_030,
                        Total_030_Detail = data.Total_030_Detail,
                        Total_3160 = data.Total_3160,
                        Total_3160_Detail = data.Total_3160_Detail,
                        Total_6190 = data.Total_6190,
                        Total_6190_Detail = data.Total_6190_Detail,
                        Total_91120 = data.Total_91120,
                        Total_91120_Detail = data.Total_91120_Detail,
                        Total_120Plus = data.Total_120Plus,
                        Total_120Plus_Detail = data.Total_120Plus_Detail
                    });
                }

                return Ok(report); // Return the list of aging reports
            }
            catch (Exception ex)
            {
                // Log the exception and return a meaningful message
                return InternalServerError(ex);
            }
        }
        private async Task<List<SupplierSetup>> GetSuppliers(string companyCode)
        {
            List<SupplierSetup> suppliers = new List<SupplierSetup>();

            // Oracle query to get suppliers
            string supplierQuery = @"
                    SELECT * FROM SupplierSetups
                    WHERE CompanyCode = :p0
                    AND DeletedFlag = 0
                    ORDER BY MasterSupplierCode
                ";

            using (OracleConnection conn = new OracleConnection(_connection))
            {
                conn.Open();

                using (OracleCommand cmd = new OracleCommand(supplierQuery, conn))
                {
                    cmd.Parameters.Add(new OracleParameter("p0", companyCode));

                    using (OracleDataReader reader = (OracleDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            suppliers.Add(new SupplierSetup
                            {
                                SupplierCode = reader["SupplierCode"].ToString(),
                                SupplierEdesc = reader["SupplierEdesc"].ToString(),
                                //GroupSkuFlag = reader["GroupSkuFlag"] != DBNull.Value ? Convert.ToBoolean(reader["GroupSkuFlag"]) : (bool?)null,
                                MasterSupplierCode = reader["MasterSupplierCode"].ToString(),
                                PreSupplierCode = reader["PreSupplierCode"].ToString()
                            });
                        }
                    }
                }
            }
            return suppliers;
        }
        private Dictionary<int, SupplierAgingReport> ProcessSupplierAgingData(List<VirtualSubLedger> ledgers, int interval)
        {
            var allData = new Dictionary<int, SupplierAgingReport>();

            foreach (var ledger in ledgers)
            {
                int subCode = int.Parse(ledger.SubCode.Substring(1)); // Assuming numeric subcode is part of SubCode
                var agingReport = allData.ContainsKey(subCode) ? allData[subCode] : new SupplierAgingReport();

                var daysPassed = (DateTime.Now - ledger.VoucherDate).Days;
                var netAmount = ledger.DrAmount - ledger.CrAmount;

                if (daysPassed <= interval)
                {
                    agingReport.Total_030 += netAmount;
                    agingReport.Total_030_Detail.Add(new { ledger.VoucherNo, Amount = netAmount });
                }
                else if (daysPassed <= 2 * interval)
                {
                    agingReport.Total_3160 += netAmount;
                    agingReport.Total_3160_Detail.Add(new { ledger.VoucherNo, Amount = netAmount });
                }
                else if (daysPassed <= 3 * interval)
                {
                    agingReport.Total_6190 += netAmount;
                    agingReport.Total_6190_Detail.Add(new { ledger.VoucherNo, Amount = netAmount });
                }
                else if (daysPassed <= 4 * interval)
                {
                    agingReport.Total_91120 += netAmount;
                    agingReport.Total_91120_Detail.Add(new { ledger.VoucherNo, Amount = netAmount });
                }
                else
                {
                    agingReport.Total_120Plus += netAmount;
                    agingReport.Total_120Plus_Detail.Add(new { ledger.VoucherNo, Amount = netAmount });
                }

                agingReport.Total += netAmount;
                allData[subCode] = agingReport;
            }

            return allData;
        }

        //[HttpGet()]
        //public IHttpActionResult GetAgeingReport(string companyCode, string branchCode, string toDate, string subCode, string userNo)
        //{
        //    try
        //    {
        //        using (OracleConnection conn = new OracleConnection(_connection))
        //        {
        //            conn.Open();

        //            // First Query: Check if the balance is greater than zero to decide if it's a debit balance
        //            string checkBalanceQuery = $@"
        //                SELECT ISNULL(SUM(dr_amount) - SUM(cr_amount), 0) 
        //                FROM V$VIRTUAL_SUB_LEDGER 
        //                WHERE SUB_CODE = '{subCode}' 
        //                AND COMPANY_CODE IN ({companyCode})";

        //            OracleCommand command = new OracleCommand(checkBalanceQuery, conn);
        //            object result = command.ExecuteScalar();
        //            bool forDebitBalance = Convert.ToDecimal(result) > 0;

        //            // Second Query: Construct the ageing report query based on the balance type
        //            string ageingQuery;
        //            if (forDebitBalance)
        //            {
        //                ageingQuery = $@"
        //                SELECT 
        //                    SUM(CASE WHEN DATEDIFF(day, VOUCHER_DATE, '{toDate}') BETWEEN 0 AND 30 THEN REAL_BALANCE ELSE 0 END) AS A30,
        //                    SUM(CASE WHEN DATEDIFF(day, VOUCHER_DATE, '{toDate}') BETWEEN 31 AND 60 THEN REAL_BALANCE ELSE 0 END) AS A60,
        //                    SUM(CASE WHEN DATEDIFF(day, VOUCHER_DATE, '{toDate}') BETWEEN 61 AND 90 THEN REAL_BALANCE ELSE 0 END) AS A90,
        //                    SUM(CASE WHEN DATEDIFF(day, VOUCHER_DATE, '{toDate}') BETWEEN 91 AND 120 THEN REAL_BALANCE ELSE 0 END) AS A120,
        //                    SUM(CASE WHEN DATEDIFF(day, VOUCHER_DATE, '{toDate}') > 120 THEN REAL_BALANCE ELSE 0 END) AS A121
        //                FROM (
        //                    SELECT 
        //                        VOUCHER_NO, VOUCHER_DATE, DR_AMOUNT, BALANCE_AMOUNT, CR_AMOUNT, 
        //                        CASE WHEN ROW_NUMBER() OVER (ORDER BY VOUCHER_DATE) = 1 THEN ABS(BALANCE_AMOUNT) ELSE DR_AMOUNT END AS REAL_BALANCE
        //                    FROM (
        //                        SELECT VOUCHER_NO, VOUCHER_DATE, DR_AMOUNT, CR_AMOUNT, 
        //                            SUM(CR_AMOUNT - DR_AMOUNT) OVER (ORDER BY VOUCHER_DATE ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS BALANCE_AMOUNT
        //                        FROM V$VIRTUAL_SUB_LEDGER
        //                        WHERE COMPANY_CODE IN ({companyCode}) 
        //                        AND SUB_CODE = '{subCode}' 
        //                        AND DELETED_FLAG = 'N'
        //                    ) AS InnerQuery
        //                ) AS FinalQuery
        //                WHERE BALANCE_AMOUNT < 0";
        //            }
        //            else
        //            {
        //                ageingQuery = $@"
        //                SELECT 
        //                    SUM(CASE WHEN DATEDIFF(day, VOUCHER_DATE, '{toDate}') BETWEEN 0 AND 30 THEN REAL_BALANCE ELSE 0 END) AS A30,
        //                    SUM(CASE WHEN DATEDIFF(day, VOUCHER_DATE, '{toDate}') BETWEEN 31 AND 60 THEN REAL_BALANCE ELSE 0 END) AS A60,
        //                    SUM(CASE WHEN DATEDIFF(day, VOUCHER_DATE, '{toDate}') BETWEEN 61 AND 90 THEN REAL_BALANCE ELSE 0 END) AS A90,
        //                    SUM(CASE WHEN DATEDIFF(day, VOUCHER_DATE, '{toDate}') BETWEEN 91 AND 120 THEN REAL_BALANCE ELSE 0 END) AS A120,
        //                    SUM(CASE WHEN DATEDIFF(day, VOUCHER_DATE, '{toDate}') > 120 THEN REAL_BALANCE ELSE 0 END) AS A121
        //                FROM (
        //                    SELECT 
        //                        VOUCHER_NO, VOUCHER_DATE, CR_AMOUNT, BALANCE_AMOUNT, DR_AMOUNT, 
        //                        CASE WHEN ROW_NUMBER() OVER (ORDER BY VOUCHER_DATE) = 1 THEN ABS(BALANCE_AMOUNT) ELSE CR_AMOUNT END AS REAL_BALANCE
        //                    FROM (
        //                        SELECT VOUCHER_NO, VOUCHER_DATE, CR_AMOUNT, DR_AMOUNT, 
        //                            SUM(DR_AMOUNT - CR_AMOUNT) OVER (ORDER BY VOUCHER_DATE ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS BALANCE_AMOUNT
        //                        FROM V$VIRTUAL_SUB_LEDGER
        //                        WHERE COMPANY_CODE IN ({companyCode}) 
        //                        AND SUB_CODE = '{subCode}' 
        //                        AND DELETED_FLAG = 'N'
        //                    ) AS InnerQuery
        //                ) AS FinalQuery
        //                WHERE BALANCE_AMOUNT < 0";
        //            }

        //            // Execute the ageing report query
        //            command.CommandText = ageingQuery;
        //            OracleDataAdapter dataAdapter = new OracleDataAdapter(command);
        //            DataTable dataTable = new DataTable();
        //            dataAdapter.Fill(dataTable);

        //            // Prepare the response dictionary
        //            var ageingReport = new Dictionary<string, decimal>
        //            {
        //                ["0-30"] = Convert.ToDecimal(dataTable.Rows[0]["A30"] ?? 0),
        //                ["31-60"] = Convert.ToDecimal(dataTable.Rows[0]["A60"] ?? 0),
        //                ["61-90"] = Convert.ToDecimal(dataTable.Rows[0]["A90"] ?? 0),
        //                ["91-120"] = Convert.ToDecimal(dataTable.Rows[0]["A120"] ?? 0),
        //                ["120++"] = Convert.ToDecimal(dataTable.Rows[0]["A121"] ?? 0),
        //                ["total"] = Convert.ToDecimal(dataTable.Rows[0]["A30"] ?? 0) +
        //                            Convert.ToDecimal(dataTable.Rows[0]["A60"] ?? 0) +
        //                            Convert.ToDecimal(dataTable.Rows[0]["A90"] ?? 0) +
        //                            Convert.ToDecimal(dataTable.Rows[0]["A120"] ?? 0) +
        //                            Convert.ToDecimal(dataTable.Rows[0]["A121"] ?? 0)
        //            };

        //            return Ok(ageingReport);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}

        [HttpGet]
        public IHttpActionResult GetAgeingReport(string companyCode, string branchCode, string toDate, string subCode, string userNo)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(_connection))
                {
                    conn.Open();

                    // First Query: Check if the balance is greater than zero to decide if it's a debit balance
                    string checkBalanceQuery = @"
                        SELECT NVL(SUM(dr_amount) - SUM(cr_amount), 0) 
                        FROM V$VIRTUAL_SUB_LEDGER 
                        WHERE SUB_CODE = :SubCode 
                        AND COMPANY_CODE IN (:CompanyCode)";

                    using (OracleCommand command = new OracleCommand(checkBalanceQuery, conn))
                    {
                        command.Parameters.Add(new OracleParameter(":SubCode", subCode));
                        command.Parameters.Add(new OracleParameter(":CompanyCode", companyCode));

                        object result = command.ExecuteScalar();
                        bool forDebitBalance = Convert.ToDecimal(result) > 0;

                        // Second Query: Construct the ageing report query based on the balance type
                        string ageingQuery = forDebitBalance ? GetDebitBalanceAgeingQuery() : GetCreditBalanceAgeingQuery();

                        // Execute the ageing report query
                        command.CommandText = ageingQuery;
                        command.Parameters.Clear(); // Clear parameters for new query
                        command.Parameters.Add(new OracleParameter(":SubCode", subCode));
                        command.Parameters.Add(new OracleParameter(":CompanyCode", companyCode));
                        command.Parameters.Add(new OracleParameter(":ToDate", toDate));

                        OracleDataAdapter dataAdapter = new OracleDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);

                        // Prepare the response dictionary
                        var ageingReport = new Dictionary<string, decimal>
                        {
                            ["0-30"] = Convert.ToDecimal(dataTable.Rows[0]["A30"] ?? 0),
                            ["31-60"] = Convert.ToDecimal(dataTable.Rows[0]["A60"] ?? 0),
                            ["61-90"] = Convert.ToDecimal(dataTable.Rows[0]["A90"] ?? 0),
                            ["91-120"] = Convert.ToDecimal(dataTable.Rows[0]["A120"] ?? 0),
                            ["120++"] = Convert.ToDecimal(dataTable.Rows[0]["A121"] ?? 0),
                            ["total"] = Convert.ToDecimal(dataTable.Rows[0]["A30"] ?? 0) +
                                        Convert.ToDecimal(dataTable.Rows[0]["A60"] ?? 0) +
                                        Convert.ToDecimal(dataTable.Rows[0]["A90"] ?? 0) +
                                        Convert.ToDecimal(dataTable.Rows[0]["A120"] ?? 0) +
                                        Convert.ToDecimal(dataTable.Rows[0]["A121"] ?? 0)
                        };

                        return Ok(ageingReport);
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex); // Return 500 Internal Server Error with exception details
            }
        }
        private string GetDebitBalanceAgeingQuery()
        {
            return @"
            SELECT 
                SUM(CASE WHEN DATEDIFF(day, VOUCHER_DATE, :ToDate) BETWEEN 0 AND 30 THEN REAL_BALANCE ELSE 0 END) AS A30,
                SUM(CASE WHEN DATEDIFF(day, VOUCHER_DATE, :ToDate) BETWEEN 31 AND 60 THEN REAL_BALANCE ELSE 0 END) AS A60,
                SUM(CASE WHEN DATEDIFF(day, VOUCHER_DATE, :ToDate) BETWEEN 61 AND 90 THEN REAL_BALANCE ELSE 0 END) AS A90,
                SUM(CASE WHEN DATEDIFF(day, VOUCHER_DATE, :ToDate) BETWEEN 91 AND 120 THEN REAL_BALANCE ELSE 0 END) AS A120,
                SUM(CASE WHEN DATEDIFF(day, VOUCHER_DATE, :ToDate) > 120 THEN REAL_BALANCE ELSE 0 END) AS A121
            FROM (
                SELECT 
                VOUCHER_NO, VOUCHER_DATE, DR_AMOUNT, BALANCE_AMOUNT, CR_AMOUNT, 
                CASE WHEN ROW_NUMBER() OVER (ORDER BY VOUCHER_DATE) = 1 THEN ABS(BALANCE_AMOUNT) ELSE DR_AMOUNT END AS REAL_BALANCE
            FROM (
                SELECT VOUCHER_NO, VOUCHER_DATE, DR_AMOUNT, CR_AMOUNT, 
                    SUM(CR_AMOUNT - DR_AMOUNT) OVER (ORDER BY VOUCHER_DATE ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS BALANCE_AMOUNT
                FROM V$VIRTUAL_SUB_LEDGER
                WHERE COMPANY_CODE IN (:CompanyCode) 
                AND SUB_CODE = :SubCode 
                AND DELETED_FLAG = 'N'
            ) AS InnerQuery
        ) AS FinalQuery
        WHERE BALANCE_AMOUNT < 0";
        }
        private string GetCreditBalanceAgeingQuery()
        {
            return @"
        SELECT 
            SUM(CASE WHEN DATEDIFF(day, VOUCHER_DATE, :ToDate) BETWEEN 0 AND 30 THEN REAL_BALANCE ELSE 0 END) AS A30,
            SUM(CASE WHEN DATEDIFF(day, VOUCHER_DATE, :ToDate) BETWEEN 31 AND 60 THEN REAL_BALANCE ELSE 0 END) AS A60,
            SUM(CASE WHEN DATEDIFF(day, VOUCHER_DATE, :ToDate) BETWEEN 61 AND 90 THEN REAL_BALANCE ELSE 0 END) AS A90,
            SUM(CASE WHEN DATEDIFF(day, VOUCHER_DATE, :ToDate) BETWEEN 91 AND 120 THEN REAL_BALANCE ELSE 0 END) AS A120,
            SUM(CASE WHEN DATEDIFF(day, VOUCHER_DATE, :ToDate) > 120 THEN REAL_BALANCE ELSE 0 END) AS A121
        FROM (
            SELECT 
                VOUCHER_NO, VOUCHER_DATE, CR_AMOUNT, BALANCE_AMOUNT, DR_AMOUNT, 
                CASE WHEN ROW_NUMBER() OVER (ORDER BY VOUCHER_DATE) = 1 THEN ABS(BALANCE_AMOUNT) ELSE CR_AMOUNT END AS REAL_BALANCE
            FROM (
                SELECT VOUCHER_NO, VOUCHER_DATE, CR_AMOUNT, DR_AMOUNT, 
                    SUM(DR_AMOUNT - CR_AMOUNT) OVER (ORDER BY VOUCHER_DATE ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS BALANCE_AMOUNT
                FROM V$VIRTUAL_SUB_LEDGER
                WHERE COMPANY_CODE IN (:CompanyCode) 
                AND SUB_CODE = :SubCode 
                AND DELETED_FLAG = 'N'
            ) AS InnerQuery
        ) AS FinalQuery
        WHERE BALANCE_AMOUNT < 0";
        }
        // Helper method for branch filter (modify logic as needed)
        private string GetBranchFilter(string companyCode, string branchCode, string userNo)
        {
            if (string.IsNullOrEmpty(branchCode))
            {
                return $"(SELECT branch_code FROM SC_BRANCH_CONTROL WHERE user_no = '{userNo}' AND company_code IN ({companyCode}))";
            }
            else if (branchCode.Split(',').Length == 1)
            {
                return $"('{branchCode}', '0')";
            }
            else
            {
                return $"({branchCode})";
            }
        }
        [HttpGet]
        public IHttpActionResult GetBranchFilterNames([FromBody] List<string> sendedBranchCode, string userNo, string companyCode)
        {
            try
            {
                string query;

                using (OracleConnection conn = new OracleConnection(_connection))
                {
                    conn.Open();

                    if (sendedBranchCode == null || sendedBranchCode.Count == 0)
                    {
                        // Case when branch code list is empty
                        query = $@"
                    SELECT branch_edesc 
                    FROM FA_BRANCH_SETUP 
                    WHERE branch_code IN 
                        (SELECT branch_code FROM SC_BRANCH_CONTROL WHERE user_no = :userNo AND company_code IN (:companyCode))";
                    }
                    else if (sendedBranchCode.Count == 1)
                    {
                        // Case when branch code has one element, adding '0' as fallback
                        sendedBranchCode.Add("0");
                        string branchCodes = $"('{string.Join("','", sendedBranchCode)}')";
                        query = $@"
                    SELECT branch_edesc 
                    FROM FA_BRANCH_SETUP 
                    WHERE branch_code IN {branchCodes} 
                      AND company_code IN (:companyCode)";
                    }
                    else
                    {
                        // Case when branch code has multiple elements
                        string branchCodes = $"('{string.Join("','", sendedBranchCode)}')";
                        query = $@"
                    SELECT branch_edesc 
                    FROM FA_BRANCH_SETUP 
                    WHERE branch_code IN {branchCodes} 
                      AND company_code IN (:companyCode)";
                    }

                    OracleCommand command = new OracleCommand(query, conn);
                    command.Parameters.Add(new OracleParameter("userNo", userNo));
                    command.Parameters.Add(new OracleParameter("companyCode", companyCode));

                    OracleDataReader reader = command.ExecuteReader();

                    var branches = new List<string>();
                    while (reader.Read())
                    {
                        branches.Add(reader["branch_edesc"].ToString());
                    }

                    return Ok(branches);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult GetTransactionDate()
        {
            try
            {
                string query = @"
            SELECT fy_start_date, 
                   CASE WHEN SYSDATE > fy_end_date THEN fy_end_date 
                   ELSE SYSDATE END AS fy_end_date 
            FROM PREFERENCE_SETUP 
            WHERE ROWNUM = 1";

                using (OracleConnection conn = new OracleConnection(_connection))
                {
                    conn.Open();
                    OracleCommand command = new OracleCommand(query, conn);
                    OracleDataReader reader = command.ExecuteReader();
                    var vals = new Dictionary<string, string>();
                    if (reader.Read())
                    {
                        vals["fy_start_date"] = Convert.ToDateTime(reader["fy_start_date"]).ToString("dd-MMM-yyyy");
                        vals["fy_till_date"] = Convert.ToDateTime(reader["fy_end_date"]).ToString("dd-MMM-yyyy");
                    }
                    return Ok(vals);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpGet]
        public IHttpActionResult GetAllItems(string companyCode, string grp = "", string cat = "")
        {
            try
            {
                string query = @"
                SELECT item_code, item_edesc 
                FROM IP_ITEM_MASTER_SETUP 
                WHERE GROUP_SKU_FLAG = 'I' 
                  AND DELETED_FLAG = 'N' 
                  AND company_code = :companyCode";
                if (!string.IsNullOrEmpty(grp))
                {
                    query += " AND master_item_code LIKE '%' || :grp || '%'";
                }

                if (!string.IsNullOrEmpty(cat))
                {
                    query += " AND CATEGORY_CODE = :cat";
                }
                query += " ORDER BY item_edesc";
                using (OracleConnection conn = new OracleConnection(_connection))
                {
                    conn.Open();
                    OracleCommand command = new OracleCommand(query, conn);
                    command.Parameters.Add(new OracleParameter("companyCode", companyCode));

                    if (!string.IsNullOrEmpty(grp))
                    {
                        command.Parameters.Add(new OracleParameter("grp", grp));
                    }

                    if (!string.IsNullOrEmpty(cat))
                    {
                        command.Parameters.Add(new OracleParameter("cat", cat));
                    }
                    OracleDataReader reader = command.ExecuteReader();
                    var items = new Dictionary<string, string>();

                    while (reader.Read())
                    {
                        items[reader["item_code"].ToString()] = reader["item_edesc"].ToString();
                    }
                    return Ok(items);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpGet]
        [Route("GetVoucherDetails")]
        public IHttpActionResult GetVoucherDetails(string companyCode, string voucherNo)
        {
            using (OracleConnection conn = new OracleConnection(_connection)) // Use OracleConnection for Oracle DB
            {
                try
                {
                    conn.Open();

                    // Step 1: Query for getting voucher details
                    string query1 = @"
                        SELECT a.form_code, b.form_type, a.voucher_no, a.voucher_date, a.created_by,
                            (SELECT table_name FROM FORM_DETAIL_SETUP WHERE form_code = a.form_code AND company_code = :CompanyCode GROUP BY table_name) AS tab_name,
                            a.authorised_by, NVL(a.authorised_date, '') AS authorised_date, a.posted_by, NVL(a.posted_date, '') AS posted_date,
                            a.CHECKED_BY, NVL(a.CHECKED_date, '') AS checked_date
                        FROM master_transaction a
                        JOIN form_setup b ON a.form_code = b.form_code AND a.company_code = b.company_code
                        WHERE a.voucher_no = :VoucherNo AND a.company_code = :CompanyCode";

                    OracleCommand cmd = new OracleCommand(query1, conn);
                    cmd.Parameters.Add(new OracleParameter("CompanyCode", companyCode));
                    cmd.Parameters.Add(new OracleParameter("VoucherNo", voucherNo));

                    OracleDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();

                        // Step 2: Retrieve the voucher details from the first query
                        string formType = reader["form_type"].ToString();
                        string voucherNumber = reader["voucher_no"].ToString();
                        DateTime voucherDate = Convert.ToDateTime(reader["voucher_date"]);
                        string createdBy = reader["created_by"].ToString();
                        string tableName = reader["tab_name"].ToString();
                        string authorisedBy = reader["authorised_by"].ToString();
                        string authorisedDate = reader["authorised_date"].ToString();
                        string postedBy = reader["posted_by"].ToString();
                        string postedDate = reader["posted_date"].ToString();
                        string checkedBy = reader["checked_by"].ToString();
                        string checkedDate = reader["checked_date"].ToString();

                        // Step 3: Dictionary for dynamic table/column mapping
                        var distinctColumns = new Dictionary<string, Dictionary<string, string>>
                {
                    { "SA_QUOTATION_DETAILS", new Dictionary<string, string> { { "V_NO", "QUOTATION_NO" }, { "V_DATE", "QUOTATION_DATE" }, { "PARTY_NAME", "customer_edesc" }, { "PARTY_TABLE_NAME", "sa_customer_setup" }, { "PARTY_CODE", "customer_code" }, { "SUB_CODE", "C" } } },
                    { "SA_SALES_ORDER", new Dictionary<string, string> { { "V_NO", "ORDER_NO" }, { "V_DATE", "ORDER_DATE" }, { "PARTY_NAME", "customer_edesc" }, { "PARTY_TABLE_NAME", "sa_customer_setup" }, { "PARTY_CODE", "customer_code" }, { "SUB_CODE", "C" } } },
                    { "SA_SALES_INVOICE", new Dictionary<string, string> { { "V_NO", "SALES_NO" }, { "V_DATE", "SALES_DATE" }, { "PARTY_NAME", "customer_edesc" }, { "PARTY_TABLE_NAME", "sa_customer_setup" }, { "PARTY_CODE", "customer_code" }, { "SUB_CODE", "C" } } },
                    // Add other table mappings as necessary
                };

                        // Step 4: Check if the table exists in the dictionary
                        if (distinctColumns.ContainsKey(tableName))
                        {
                            var tableMapping = distinctColumns[tableName];

                            // Step 5: Prepare the second query based on dynamic table and columns
                            string subQuery = $@"
                        SELECT b.{tableMapping["PARTY_NAME"]}, a.{tableMapping["V_NO"]}, TO_CHAR(a.{tableMapping["V_DATE"]}, 'DD/MM/YYYY') AS {tableMapping["V_DATE"]},
                               a.serial_no, a.item_code, c.item_edesc, a.mu_code, a.QUANTITY, a.UNIT_PRICE, 
                               NVL(a.total_price * a.exchange_rate, 1) AS total_price, '{tableMapping["SUB_CODE"]}' || a.{tableMapping["PARTY_CODE"]} AS party_code, a.remarks
                        FROM {tableName} a
                        JOIN {tableMapping["PARTY_TABLE_NAME"]} b ON a.company_code = b.company_code AND a.{tableMapping["PARTY_CODE"]} = b.{tableMapping["PARTY_CODE"]}
                        JOIN ip_item_master_setup c ON a.company_code = c.company_code AND a.item_code = c.item_code
                        WHERE a.company_code = :CompanyCode AND a.{tableMapping["V_NO"]} = :VoucherNo
                        ORDER BY a.serial_no";

                            OracleCommand subCmd = new OracleCommand(subQuery, conn);
                            subCmd.Parameters.Add(new OracleParameter("CompanyCode", companyCode));
                            subCmd.Parameters.Add(new OracleParameter("VoucherNo", voucherNo));

                            OracleDataReader subReader = subCmd.ExecuteReader();
                            var itemList = new List<object>();

                            // Step 6: Fetch item details from the sub-query
                            while (subReader.Read())
                            {
                                itemList.Add(new
                                {
                                    SerialNo = subReader["serial_no"],
                                    Item = subReader["item_edesc"],
                                    Unit = subReader["mu_code"],
                                    Quantity = Convert.ToDouble(subReader["QUANTITY"]),
                                    Rate = Convert.ToDouble(subReader["UNIT_PRICE"]),
                                    Amount = Convert.ToDouble(subReader["total_price"]),
                                    Remarks = subReader["remarks"],
                                    PartyCode = subReader["party_code"]
                                });
                            }

                            // Step 7: Return the voucher details and the item list
                            return Ok(new
                            {
                                Detail = itemList,
                                VoucherNo = voucherNumber,
                                VoucherDate = voucherDate.ToString("yyyy-MM-dd"),
                                CreatedBy = createdBy,
                                AuthorisedBy = authorisedBy,
                                AuthorisedDate = authorisedDate,
                                PostedBy = postedBy,
                                PostedDate = postedDate,
                                CheckedBy = checkedBy,
                                CheckedDate = checkedDate
                            });
                        }

                        // Step 8: Return if the form type is invalid
                        return Ok(new { Message = "Invalid Form Type." });
                    }

                    // Step 9: Handle case where no voucher is found
                    return NotFound();
                }
                catch (Exception ex)
                {
                    // Step 10: Handle any errors
                    return InternalServerError(ex);
                }
            }
        }

        [HttpGet]
        [Route("sales_today_ytd_mtd")]
        public async Task<IHttpActionResult> GetSalesTodayYTDMTD(string companyCode, DateTime fromDate, DateTime toDate, string userNo)
        {
            try
            {
                using (var connection = new OracleConnection(_connection))
                {
                    // Open SQL connection
                    await connection.OpenAsync();

                    // Dictionary to store the results of all reports (sales, collection, production, purchase, payment)
                    var result = new Dictionary<string, object>();

                    // Report types to iterate over
                    string[] allReps = { "sales", "collection", "Production", "Purchase", "Payment" };

                    foreach (var reportType in allReps)
                    {
                        string query = string.Empty;

                        // Query for Sales Report
                        if (reportType == "sales")
                        {
                            query = $@"
                        SELECT startdate, rangename, inde, 
                               SUM(sales_qty - sales_ret_qty) AS TotalQty, 
                               SUM(sales_value - sales_ret_value) AS TotalValue,
                               CASE 
                                   WHEN RANGENAME = 'Today' THEN SUM(sales_value - sales_ret_value)
                                   ELSE (SUM(sales_value - sales_ret_value) / 
                                         CASE WHEN DATEDIFF(DAY, startdate, GETDATE()) = 0 THEN 1 
                                         ELSE DATEDIFF(DAY, startdate, GETDATE()) END)
                               END AS AvgPerDay
                        FROM (
                            SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.sales_date,
                                   SUM(ISNULL(A.QUANTITY, 0)) AS sales_qty, 
                                   SUM(ISNULL(A.QUANTITY * A.NET_GROSS_RATE, 0)) AS sales_value,
                                   0 AS sales_ret_qty, 0 AS sales_ret_value
                            FROM SA_SALES_INVOICE A
                            WHERE A.DELETED_FLAG = 'N'
                              AND A.COMPANY_CODE = @CompanyCode
                            GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.sales_date

                            UNION ALL

                            SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.return_date, 
                                   0 AS sales_qty, 0 AS sales_value,
                                   SUM(ISNULL(A.QUANTITY, 0)) AS sales_ret_qty, 
                                   SUM(ISNULL(A.QUANTITY * A.NET_GROSS_RATE, 0)) AS sales_ret_value
                            FROM SA_SALES_RETURN A
                            WHERE A.DELETED_FLAG = 'N'
                              AND A.COMPANY_CODE = @CompanyCode
                            GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.return_date
                        ) AS SalesData
                        LEFT JOIN (
                            SELECT startdate, enddate, rangename, 
                                   ROW_NUMBER() OVER(ORDER BY rangename) AS inde
                            FROM V_DATE_RANGE
                            WHERE sortorder IN (2, 3)
                        ) AS DateRange
                        ON SalesData.sales_date BETWEEN DateRange.startdate AND DateRange.enddate
                        GROUP BY startdate, rangename, inde
                        ORDER BY startdate";
                        }

                        // Query for Collection Report
                        else if (reportType == "collection")
                        {
                            query = $@"
                        SELECT startdate, rangename, inde, SUM(AMT) AS TotalAmt,
                               CASE 
                                   WHEN RANGENAME = 'Today' THEN SUM(AMT)
                                   ELSE (SUM(AMT) / 
                                         CASE WHEN DATEDIFF(DAY, startdate, GETDATE()) = 0 THEN 1 
                                         ELSE DATEDIFF(DAY, startdate, GETDATE()) END)
                               END AS AvgPerDay
                        FROM (
                            SELECT voucher_date, SUM(ISNULL(CR_AMOUNT, 0)) AS AMT
                            FROM V$VIRTUAL_SUB_LEDGER
                            WHERE DELETED_FLAG = 'N'
                              AND COMPANY_CODE = @CompanyCode
                              AND CR_AMOUNT > 0
                            GROUP BY voucher_date
                        ) AS CollectionData
                        LEFT JOIN (
                            SELECT startdate, enddate, rangename, 
                                   ROW_NUMBER() OVER(ORDER BY rangename) AS inde
                            FROM V_DATE_RANGE
                            WHERE sortorder IN (2, 3)
                        ) AS DateRange
                        ON CollectionData.voucher_date BETWEEN DateRange.startdate AND DateRange.enddate
                        GROUP BY startdate, rangename, inde
                        ORDER BY startdate";
                        }
                        // Create a DataTable to hold the result set
                        DataTable queryResult = new DataTable();
                        // OracleCommand to execute the query
                        using (OracleCommand command = new OracleCommand(query, connection))
                        {
                            // Adding parameters to OracleCommand
                            command.Parameters.Add(new OracleParameter("CompanyCode", companyCode));

                            // OracleDataAdapter to fetch the data
                            OracleDataAdapter dataAdapter = new OracleDataAdapter(command);

                            // Fill the DataTable with the result from the query
                            dataAdapter.Fill(queryResult);

                            // Add the DataTable result to the dictionary
                            result.Add(reportType, queryResult);
                        }
                        // Add the result to the dictionary
                        result.Add(reportType, queryResult);
                    }

                    // Return the result dictionary as JSON
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                // Log the error and return an error response
                return InternalServerError(new Exception("Internal server error: " + ex.Message));
            }
        }

        [HttpGet]
        [Route("build-query")]
        public IHttpActionResult BuildQuery([FromBody] string reportType, [FromBody] string companyCode)
        {
            string query = string.Empty;

            switch (reportType.ToLower())
            {
                case "sales":
                    query = $@"
                            SELECT startdate, rangename, inde, 
                            SUM(sales_qty - sales_ret_qty) as SalesQty, 
                            SUM(sales_value - sales_ret_value) as SalesValue, 
                            CASE WHEN RANGENAME = 'Today' THEN 
                            SUM(sales_value - sales_ret_value)
                            ELSE
                            (SUM(sales_value - sales_ret_value) / 
                            (CASE WHEN TRUNC(SYSDATE) - startdate = 0 THEN 1 ELSE TRUNC(SYSDATE) - startdate END)) 
                            END as AvgPerDay
                            FROM (
                                SELECT * FROM (
                                    SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.sales_date, 
                                    SUM(NVL(A.ROLL_QTY, 0)) SALES_ROLL_QTY, 
                                    SUM(NVL(A.QUANTITY, 0)) SALES_QTY, 
                                    SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) SALES_VALUE, 
                                    0 SALES_RET_ROLL_QTY, 0 SALES_RET_QTY, 0 SALES_RET_VALUE
                                    FROM SA_SALES_INVOICE A 
                                    WHERE A.DELETED_FLAG = 'N' 
                                    AND A.COMPANY_CODE IN ('{companyCode}') 
                                    GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.sales_date
                                    UNION ALL 
                                    SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.return_date, 
                                    0 SALES_ROLL_QTY, 0 SALES_QTY, 0 SALES_VALUE, 
                                    SUM(NVL(A.ROLL_QTY, 0)) SALES_RET_ROLL_QTY, 
                                    SUM(NVL(A.QUANTITY, 0)) SALES_RET_QTY, 
                                    SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) SALES_RET_VALUE
                                    FROM SA_SALES_RETURN A 
                                    WHERE A.DELETED_FLAG = 'N' 
                                    AND A.COMPANY_CODE IN ('{companyCode}')
                                    GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.return_date
                                ) a
                                LEFT OUTER JOIN (
                                    SELECT startdate, enddate, rangename, TO_NUMBER('11') AS inde 
                                    FROM V_DATE_RANGE 
                                    WHERE sortorder IN ('2')
                                    UNION ALL 
                                    SELECT AD_DATE(SUBSTR(BS_DATE(TRUNC(SYSDATE)), 1, 8) || '01'), 
                                    TRUNC(SYSDATE), 'This Month', 10 FROM DUAL
                                    UNION ALL 
                                    SELECT TRUNC(SYSDATE), TRUNC(SYSDATE), 'Today', TO_NUMBER('9') FROM V_DATE_RANGE 
                                    WHERE sortorder IN ('3')
                                    UNION ALL 
                                    SELECT TRUNC(SYSDATE - 1), TRUNC(SYSDATE - 1), 'Yesterday', TO_NUMBER('8') FROM DUAL
                                ) b ON a.sales_date BETWEEN b.startdate AND b.enddate
                            )
                            GROUP BY startdate, rangename, inde 
                            ORDER BY startdate";
                    break;

                case "collection":
                    query = $@"
                            SELECT startdate, rangename, inde, SUM(AMT) as Amt, 
                            CASE WHEN RANGENAME = 'Today' THEN SUM(AMT)
                            ELSE (SUM(AMT) / (CASE WHEN TRUNC(SYSDATE) - startdate = 0 THEN 1 ELSE TRUNC(SYSDATE) - startdate END)) 
                            END as AvgPerDay
                            FROM (
                                SELECT * FROM (
                                    SELECT voucher_date, NVL(SUM(NVL(CR_AMOUNT, 0) * NVL(EXCHANGE_RATE, 1)), 0) AS AMT 
                                    FROM V$VIRTUAL_SUB_LEDGER 
                                    WHERE DELETED_FLAG = 'N' 
                                    AND COMPANY_CODE = '{companyCode}' 
                                    AND CR_AMOUNT > 0
                                    GROUP BY voucher_date
                                ) a
                                LEFT OUTER JOIN (
                                    SELECT startdate, enddate, rangename, TO_NUMBER('11') AS inde 
                                    FROM V_DATE_RANGE 
                                    WHERE sortorder IN ('2')
                                    UNION ALL 
                                    SELECT AD_DATE(SUBSTR(BS_DATE(TRUNC(SYSDATE)), 1, 8) || '01'), 
                                    TRUNC(SYSDATE), 'This Month', 10 FROM DUAL
                                ) b ON a.voucher_date BETWEEN b.startdate AND b.enddate
                            )
                            GROUP BY startdate, rangename, inde 
                            ORDER BY startdate";
                    break;

                default:
                    return BadRequest("Invalid report type");
            }
            return ExecuteOracleQuery(query);
        }

        [HttpPost]
        [Route("Approve")]
        public async Task<IHttpActionResult> Approve([FromBody] ApproveData data, string userNo, string companyCode)
        {
            using (var connection = new OracleConnection(_connection))
            {
                await connection.OpenAsync();
                try
                {
                    // Construct query to fetch form and transaction details
                    string voucherNo = data.VoucherNo;
                    string query = @"
                    SELECT a.table_name,
                           (SELECT login_code FROM SC_APPLICATION_USERS  
                            WHERE user_no = :UserNo 
                              AND company_code = :CompanyCode 
                              AND group_sku_flag = 'I') AS login_name,
                           SYSDATE AS sysdate, 
                           a.form_code, 
                           b.form_action_flag
                    FROM form_detail_setup a
                    JOIN form_setup b ON a.form_code = b.form_code
                    WHERE a.form_code IN 
                          (SELECT form_code FROM master_transaction WHERE voucher_no = :VoucherNo) 
                    AND ROWNUM = 1";

                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.Add(new OracleParameter("UserNo", userNo));
                        command.Parameters.Add(new OracleParameter("CompanyCode", companyCode));
                        command.Parameters.Add(new OracleParameter("VoucherNo", voucherNo));

                        var adapter = new OracleDataAdapter(command);
                        var resultTable = new DataTable();
                        adapter.Fill(resultTable);

                        if (resultTable.Rows.Count == 0)
                        {
                            return Ok(new { approved = false });
                        }

                        var result = resultTable.Rows[0];
                        var stepArray = new List<string> { "CHECK", "AUTHORIZE/VERIFY", "POST" };
                        var containsStep = new List<string>();
                        string formActionFlag = result["form_action_flag"].ToString();

                        // Check if the provided step exists in the step array and form_action_flag
                        if (stepArray.Contains(data.Step))
                        {
                            for (int i = 0; i < formActionFlag.Length; i++)
                            {
                                if (formActionFlag[i] == '1')
                                {
                                    containsStep.Add(stepArray[i]);
                                }
                            }

                            if (containsStep.Contains(data.Step))
                            {
                                // Create voucher date from sysdate
                                string voucherDate = Convert.ToDateTime(result["sysdate"]).ToString("MM/dd/yyyy HH:mm:ss");
                                string voucherDateSql = $"TO_DATE('{voucherDate}', 'MM/DD/YYYY HH24:MI:SS')";

                                // Update query for master_transaction
                                string updateQuery = $@"
                                UPDATE master_transaction 
                                SET {data.Step}_by = :LoginName, 
                                    {data.Step}_date = {voucherDateSql}
                                WHERE voucher_no = :VoucherNo 
                                  AND company_code = :CompanyCode";

                                using (var updateCommand = new OracleCommand(updateQuery, connection))
                                {
                                    updateCommand.Parameters.Add(new OracleParameter("LoginName", result["login_name"].ToString()));
                                    updateCommand.Parameters.Add(new OracleParameter("VoucherNo", voucherNo));
                                    updateCommand.Parameters.Add(new OracleParameter("CompanyCode", companyCode));

                                    await updateCommand.ExecuteNonQueryAsync();
                                }

                                return Ok(new { approved = true });
                            }
                            else
                            {
                                return Ok(new { approved = false });
                            }
                        }
                        else
                        {
                            return Ok(new { approved = false });
                        }
                    }
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
        }
        [HttpPost]
        [Route("FetchToken/{userNo}")]
        public async Task<IHttpActionResult> FetchToken(string userNo)
        {
            using (var connection = new OracleConnection(_connection))
            {
                await connection.OpenAsync();
                var query = @"SELECT MOB_TOKEN FROM SC_APPLICATION_USERS WHERE mob_token IS NOT NULL";
                try
                {
                    var tokens = new List<string>();

                    using (var command = new OracleCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                if (!reader.IsDBNull(0))
                                {
                                    tokens.Add(reader.GetString(0));
                                }
                            }
                        }
                    }
                    return Ok(tokens);
                }
                catch (OracleException ex)
                {
                    return InternalServerError(ex);
                }
            }
        }
        [HttpPost]
        [Route("CompanyName/{companyCode}")]
        public async Task<IHttpActionResult> GetCompanyName(string companyCode)
        {
            using (var connection = new OracleConnection(_connection))
            {
                var query = @"SELECT company_edesc FROM company_setup WHERE company_code = :CompanyCode";

                try
                {
                    await connection.OpenAsync(); // Ensure the connection is opened

                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.Add(new OracleParameter("CompanyCode", companyCode));

                        var companyName = await command.ExecuteScalarAsync();
                        if (companyName == null)
                        {
                            return NotFound();
                        }

                        return Ok(companyName.ToString());  // Return the company name as a JSON string
                    }
                }
                catch (OracleException ex)
                {
                    return InternalServerError(ex); // Return 500 with error message
                }
            }
        }

        [HttpGet]
        [Route("ApprovalInfo/{companyCode}/{userNo}/{step}/{vNo}")]
        public async Task<IHttpActionResult> GetApprovalInfo(string companyCode, string userNo, string step, string vNo)
        {
            using (var connection = new OracleConnection(_connection))
            {
                await connection.OpenAsync(); // Ensure the connection is opened

                string v_str = string.IsNullOrEmpty(vNo) ? "" : $"voucher_no = :VoucherNo AND ";

                var query = $@"
                SELECT * FROM your_table 
                WHERE {v_str} company_code = :CompanyCode";

                try
                {
                    using (var command = new OracleCommand(query, connection))
                    {
                        if (!string.IsNullOrEmpty(vNo))
                        {
                            command.Parameters.Add(new OracleParameter("VoucherNo", vNo));
                        }
                        command.Parameters.Add(new OracleParameter("CompanyCode", companyCode));

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var approvalInfo = new List<dynamic>();
                            while (await reader.ReadAsync())
                            {
                                // Create an anonymous object or dictionary for each row
                                var row = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                }
                                approvalInfo.Add(row);
                            }
                            return Ok(approvalInfo);
                        }
                    }
                }
                catch (OracleException ex)
                {
                    return InternalServerError(ex);
                }
            }
        }
        [HttpGet]
        [Route("ApprovalInfo/{companyCode}/{userNo}/{step}/{vNo}")]
        public async Task<IHttpActionResult> GetApprovalInfos(string companyCode, string userNo, string step, string vNo)
        {
            string vStr = string.IsNullOrEmpty(vNo) ? "" : $"voucher_no = :VoucherNo AND ";
            string formAction = "";
            string strpStr = "";

            // Determine the filter based on the step
            if (step == "CHECK")
            {
                strpStr = "CHECKED_BY IS NULL AND AUTHORISED_BY IS NULL AND POSTED_BY IS NULL";
                formAction = "1";
            }
            else if (step == "AUTHORIZE/VERIFY")
            {
                strpStr = "CHECKED_BY IS NOT NULL AND AUTHORISED_BY IS NULL AND POSTED_BY IS NULL";
                formAction = "2";
            }
            else if (step == "POST")
            {
                strpStr = "CHECKED_BY IS NOT NULL AND AUTHORISED_BY IS NOT NULL AND POSTED_BY IS NULL";
                formAction = "3";
            }

            string query = $@"
            SELECT form_code, TRUNC(voucher_date) AS voucher_date, voucher_no, voucher_amount, created_by, 
                   TRUNC(SYSDATE) - voucher_date AS created_days, cc, form_edesc, 
                   module_code, code, module_edesc, form_type,
                   CASE 
                       WHEN pre_form_code = '01.01' THEN 'PAYMENT'
                       WHEN pre_form_code = '01.02' THEN 'RECEIPT'
                       WHEN pre_form_code = '05.02' THEN 'PO'
                       WHEN pre_form_code = '10.01' THEN 'SO'
                       WHEN pre_form_code = '14.03' THEN 'REQUISITION'
                       ELSE pre_form_code 
                   END AS TY,
                   FORM_ACTION_FLAG
            FROM (
                SELECT * FROM (
                    SELECT FORM_CODE, VOUCHER_DATE, VOUCHER_NO, VOUCHER_AMOUNT, CREATED_BY, 
                           TRUNC(SYSDATE) - VOUCHER_DATE AS CREATED_DAYS 
                    FROM MASTER_TRANSACTION
                    WHERE {strpStr} 
                      AND COMPANY_CODE = :CompanyCode 
                      AND {vStr}
                      FORM_CODE IN (SELECT FORM_CODE FROM FORM_SETUP WHERE 
                                      (PRE_FORM_CODE LIKE '01.01%' OR 
                                       PRE_FORM_CODE LIKE '01.02%' OR 
                                       PRE_FORM_CODE LIKE '05.02%' OR 
                                       PRE_FORM_CODE LIKE '14.03%' OR 
                                       PRE_FORM_CODE LIKE '10.01%') 
                                      AND COMPANY_CODE = :CompanyCode) 
                      AND DELETED_FLAG = 'N' 
                    ORDER BY VOUCHER_DATE
                ) A
                LEFT OUTER JOIN (
                    SELECT FORM_CODE AS cc, FORM_EDESC, MODULE_CODE, FORM_TYPE, 
                           pre_form_code, FORM_ACTION_FLAG  
                    FROM FORM_SETUP 
                    WHERE COMPANY_CODE = :CompanyCode
                ) B ON A.FORM_CODE = B.cc
                LEFT OUTER JOIN (
                    SELECT MODULE_CODE AS CODE, MODULE_EDESC 
                    FROM MODULE_SETUP
                ) C ON B.MODULE_CODE = C.CODE
            ) 
            WHERE {formAction} = '1' 
            ORDER BY MODULE_EDESC, FORM_EDESC, VOUCHER_NO, VOUCHER_DATE";

            // Logging the query
            Console.WriteLine(query);

            using (var connection = new OracleConnection(_connection))
            {
                await connection.OpenAsync(); // Ensure the connection is opened

                try
                {
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.Add(new OracleParameter("CompanyCode", companyCode));
                        if (!string.IsNullOrEmpty(vNo))
                        {
                            command.Parameters.Add(new OracleParameter("VoucherNo", vNo));
                        }

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var vals = new Dictionary<string, Dictionary<string, List<dynamic>>>();
                            string changedModule = null;
                            string changedForm = null;

                            while (await reader.ReadAsync())
                            {
                                string currentModule = reader["module_code"].ToString();
                                string currentForm = reader["form_edesc"].ToString();

                                if (changedModule != currentModule)
                                {
                                    vals[currentModule] = new Dictionary<string, List<dynamic>>();
                                    changedModule = currentModule;
                                }

                                if (changedForm != currentForm)
                                {
                                    vals[currentModule][currentForm] = new List<dynamic>();
                                    changedForm = currentForm;
                                }

                                vals[currentModule][currentForm].Add(new
                                {
                                    v_no = reader["voucher_no"],
                                    v_date = ((DateTime)reader["voucher_date"]).ToString("MM/dd/yyyy"),
                                    v_amt = reader["voucher_amount"],
                                    c_by = reader["created_by"],
                                    days = reader["created_days"],
                                    form_type = reader["form_type"],
                                    type = reader["TY"]
                                });
                            }

                            return Ok(vals);
                        }
                    }
                }
                catch (OracleException ex)
                {
                    return InternalServerError(ex); // Return 500 with error message
                }
            }
        }

        [HttpGet]
        [Route("BankCashClosing/{companyCode}/{fromDate}/{toDate}")]
        public async Task<IHttpActionResult> GetBankCashClosing(string companyCode, string fromDate, string toDate)
        {
            if (string.IsNullOrEmpty(fromDate))
            {
                fromDate = "(SELECT STARTDATE FROM V_DATE_RANGE WHERE SORTORDER='2')";
                toDate = "(SELECT ENDDATE FROM V_DATE_RANGE WHERE SORTORDER='2')";
            }
            else
            {
                fromDate = $"'{fromDate}'";
                toDate = $"'{toDate}'";
            }

            if (string.IsNullOrEmpty(companyCode))
            {
                companyCode = "(SELECT company_code FROM COMPANY_SETUP)";
            }
            else if (companyCode.Length == 1)
            {
                companyCode += ", '0000000088888888'";
            }

            string query = $@"
            SELECT DISTINCT a.company_code, 
                CASE 
                    WHEN a.acc_nature = 'AB' THEN 'CASH' 
                    WHEN a.acc_nature = 'AC' THEN 'BANK' 
                    ELSE '' 
                END AS TYPE,
                a.acc_edesc,
                a.acc_code, 
                NVL(SUM((SELECT NVL(SUM(FN_CONVERT_CURRENCY(ROUND(NVL(C.DR_AMOUNT, 0), 2) * NVL(C.EXCHANGE_RATE, 1), 'NRS', C.VOUCHER_DATE)), 0) - 
                            NVL(SUM(FN_CONVERT_CURRENCY(ROUND(NVL(C.CR_AMOUNT, 0), 2) * NVL(C.EXCHANGE_RATE, 1), 'NRS', C.VOUCHER_DATE)), 0) 
                        FROM V$VIRTUAL_GENERAL_LEDGER C 
                        WHERE C.COMPANY_CODE = A.COMPANY_CODE AND C.ACC_CODE = A.ACC_CODE AND C.DELETED_FLAG = 'N' 
                        AND C.COMPANY_CODE IN {companyCode} AND C.FORM_CODE='0')), 0) AS OPEN_AMOUNT,
                SUM((SELECT NVL(SUM(FN_CONVERT_CURRENCY(ROUND(NVL(C.DR_AMOUNT, 0), 2) * NVL(C.EXCHANGE_RATE, 1), 'NRS', C.VOUCHER_DATE)), 0) 
                        FROM V$VIRTUAL_GENERAL_LEDGER C 
                        WHERE C.COMPANY_CODE = A.COMPANY_CODE AND C.ACC_CODE = A.ACC_CODE AND C.DELETED_FLAG = 'N' 
                        AND C.COMPANY_CODE IN {companyCode} AND C.FORM_CODE <> '0')) AS DR_AMOUNT,
                SUM((SELECT NVL(SUM(FN_CONVERT_CURRENCY(ROUND(NVL(C.CR_AMOUNT, 0), 2) * NVL(C.EXCHANGE_RATE, 1), 'NRS', C.VOUCHER_DATE)), 0) 
                        FROM V$VIRTUAL_GENERAL_LEDGER C 
                        WHERE C.COMPANY_CODE = A.COMPANY_CODE AND C.ACC_CODE = A.ACC_CODE AND C.DELETED_FLAG = 'N' 
                        AND C.VOUCHER_DATE BETWEEN {fromDate} AND {toDate} 
                        AND C.FORM_CODE <> '0' AND C.COMPANY_CODE IN {companyCode})) AS CR_AMOUNT 
            FROM FA_CHART_OF_ACCOUNTS_SETUP A 
            WHERE A.DELETED_FLAG = 'N' 
                AND A.acc_nature IN ('AB', 'AC') 
                AND A.COMPANY_CODE IN {companyCode} 
            GROUP BY a.company_code, a.acc_nature, A.ACC_EDESC, A.ACC_CODE 
            ORDER BY company_code, TYPE";

            using (var connection = new OracleConnection(_connection))
            {
                await connection.OpenAsync();

                using (var command = new OracleCommand(query, connection))
                {
                    try
                    {
                        using (var adapter = new OracleDataAdapter(command))
                        {
                            var dataTable = new DataTable();
                            await Task.Run(() => adapter.Fill(dataTable));

                            var vals = new Dictionary<string, Dictionary<string, List<dynamic>>>();
                            string changeType = "";
                            string changeCom = "";

                            foreach (DataRow row in dataTable.Rows)
                            {
                                float opening = float.Parse(row["OPEN_AMOUNT"].ToString());
                                float dr = float.Parse(row["DR_AMOUNT"].ToString());
                                float cr = float.Parse(row["CR_AMOUNT"].ToString());
                                float closing = (float)Math.Round(opening + dr - cr, 3);

                                if (changeCom != row["company_code"].ToString())
                                {
                                    changeCom = row["company_code"].ToString();
                                    vals[changeCom] = new Dictionary<string, List<dynamic>>();
                                }

                                if (changeType != row["TYPE"].ToString())
                                {
                                    changeType = row["TYPE"].ToString();
                                }

                                if (closing != 0)
                                {
                                    if (!vals[changeCom].ContainsKey(changeType))
                                    {
                                        vals[changeCom][changeType] = new List<dynamic>();
                                    }

                                    vals[changeCom][changeType].Add(new
                                    {
                                        name = row["ACC_EDESC"].ToString(),
                                        closing = closing
                                    });
                                }
                            }

                            return Ok(vals);
                        }
                    }
                    catch (OracleException ex)
                    {
                        return InternalServerError(ex); // Return 500 with error message
                    }
                }
            }
        }
        [HttpGet]
        [Route("MonthDates")]
        public async Task<IHttpActionResult> GetMonthDates()
        {
            string query = @"
            SELECT * FROM V_DATE_RANGE WHERE sortorder NOT IN (-100)
            UNION ALL
            SELECT TRUNC(SYSDATE - 1), TRUNC(SYSDATE - 1), 'Yesterday', 0 FROM dual
            UNION ALL
            SELECT TRUNC(SYSDATE), TRUNC(SYSDATE), 'Today', -1 FROM dual
            ORDER BY 4, 1 ASC";

            using (var connection = new OracleConnection(_connection))
            {
                await connection.OpenAsync();

                using (var command = new OracleCommand(query, connection))
                {
                    try
                    {
                        using (var adapter = new OracleDataAdapter(command))
                        {
                            var dataTable = new DataTable();
                            await Task.Run(() => adapter.Fill(dataTable));

                            var allDateRange = new List<Dictionary<string, string>>();

                            foreach (DataRow row in dataTable.Rows)
                            {
                                var vals = new Dictionary<string, string>
                            {
                                { "startdate", ((DateTime)row["COLUMN1"]).ToString("dd-MMM-yyyy") },
                                { "enddate", ((DateTime)row["COLUMN2"]).ToString("dd-MMM-yyyy") },
                                { "ragename", row["COLUMN3"].ToString() }
                            };
                                allDateRange.Add(vals);
                            }

                            return Ok(allDateRange);
                        }
                    }
                    catch (OracleException ex)
                    {
                        return InternalServerError(ex); // Return 500 with error message
                    }
                }
            }
        }
        [HttpGet]
        [Route("MonthDatesEng")]
        public async Task<IHttpActionResult> GetMonthDatesEng()
        {
            string query = @"
            SELECT fromdate, todate, month_name, TO_NUMBER('1') sortorder FROM (
                SELECT * FROM (
                    SELECT month_name, MIN(dates) fromdate FROM (
                        SELECT TRIM(month_name) || '-' || SUBSTR(dates, 8, 4) month_name, dates FROM (
                            SELECT TO_CHAR((SELECT startdate FROM v_date_range WHERE rangename='This Year') + LEVEL - 1, 'MONTH') AS month_name,
                                    (SELECT startdate FROM v_date_range WHERE rangename='This Year') + LEVEL - 1 dates
                            FROM dual
                            CONNECT BY LEVEL <= ((SELECT enddate FROM v_date_range WHERE rangename='This Year') - (SELECT startdate FROM v_date_range WHERE rangename='This Year')) + 1
                        )
                    ) GROUP BY month_name ORDER BY fromdate
                ) a LEFT OUTER JOIN (
                    SELECT month_name mon, MAX(dates) todate FROM (
                        SELECT TRIM(month_name) || '-' || SUBSTR(dates, 8, 4) month_name, dates FROM (
                            SELECT TO_CHAR((SELECT startdate FROM v_date_range WHERE rangename='This Year') + LEVEL - 1, 'MONTH') AS month_name,
                                    (SELECT startdate FROM v_date_range WHERE rangename='This Year') + LEVEL - 1 dates
                            FROM dual
                            CONNECT BY LEVEL <= ((SELECT enddate FROM v_date_range WHERE rangename='This Year') - (SELECT startdate FROM v_date_range WHERE rangename='This Year')) + 1
                        )
                    ) GROUP BY month_name ORDER BY todate
                ) b ON a.month_name = b.mon
            )
            UNION ALL
            SELECT * FROM V_DATE_RANGE WHERE SORTORDER NOT IN ('1', '3', '4', '-100')
            UNION ALL
            SELECT TRUNC(ADD_MONTHS(SYSDATE, -1), 'MM'), TRUNC(LAST_DAY(ADD_MONTHS(SYSDATE, -1))), 'Last Month' rangename, TO_NUMBER('4') sortorder FROM DUAL
            UNION ALL
            SELECT TRUNC(SYSDATE, 'MM'), TRUNC(LAST_DAY(SYSDATE)), 'This Month' rangename, TO_NUMBER('3') sortorder FROM DUAL
            UNION ALL
            SELECT TRUNC(SYSDATE), TRUNC(SYSDATE), 'Today' Rangename, TO_NUMBER('999') sortorder FROM dual
            UNION ALL
            SELECT TRUNC(SYSDATE - 1), TRUNC(SYSDATE - 1), 'Yesterday' Rangename, TO_NUMBER('1000') sortorder FROM dual";

            using (var connection = new OracleConnection(_connection))
            {
                await connection.OpenAsync();

                using (var command = new OracleCommand(query, connection))
                {
                    try
                    {
                        using (var adapter = new OracleDataAdapter(command))
                        {
                            var dataTable = new DataTable();
                            await Task.Run(() => adapter.Fill(dataTable));

                            var allDateRange = new List<Dictionary<string, string>>();

                            foreach (DataRow row in dataTable.Rows)
                            {
                                var vals = new Dictionary<string, string>
                            {
                                { "startdate", ((DateTime)row["fromdate"]).ToString("dd-MMM-yyyy") },
                                { "enddate", ((DateTime)row["todate"]).ToString("dd-MMM-yyyy") },
                                { "ragename", row["month_name"].ToString() }
                            };
                                allDateRange.Add(vals);
                            }

                            return Ok(allDateRange);
                        }
                    }
                    catch (OracleException ex)
                    {
                        return InternalServerError(ex);
                    }
                }
            }
        }
        [HttpGet]
        [Route("Refer")]
        public async Task<IHttpActionResult> Refer(string xt, string company_code)
        {
            string text = xt;
            xt = xt.ToLower();

            var customerNamesPattern = new[] { @"\bof (\w+\s\w+)\b", @"\bof\s(\w+)\b" };
            var reportNames = new[] { @"([A-Za-z]+)" };
            string customerName = null;
            string reportName = null;

            foreach (var pattern in customerNamesPattern)
            {
                try
                {
                    var customerNamePattern = new Regex(pattern, RegexOptions.IgnoreCase);
                    var customerNameMatch = customerNamePattern.Match(text);
                    customerName = customerNameMatch.Success ? customerNameMatch.Groups[1].Value : null;

                    if (!string.IsNullOrEmpty(customerName))
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    customerName = string.Empty;
                }
            }

            foreach (var pattern in reportNames)
            {
                try
                {
                    var reportNamePattern = new Regex(pattern);
                    var reportNameMatch = reportNamePattern.Match(text);
                    reportName = reportNameMatch.Success ? reportNameMatch.Groups[1].Value : null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    reportName = string.Empty;
                }
            }

            var sampleCustomers = new List<Dictionary<string, string>>();
            string slTextToCompare = string.Empty;

            if (!string.IsNullOrEmpty(customerName))
            {
                customerName = customerName.Replace(" ", "");
                slTextToCompare = customerName;

                using (var connection = new OracleConnection(_connection))
                {
                    await connection.OpenAsync();

                    string query = string.Empty;

                    if (reportName == "balance")
                    {
                        query = @"
                        SELECT DISTINCT sub_edesc, sub_code 
                        FROM V$VIRTUAL_SUB_LEDGER 
                        WHERE company_code = :CompanyCode 
                        AND LOWER(REPLACE(sub_edesc, ' ', '')) LIKE '%' || :CustomerName || '%'";
                    }
                    else if (reportName == "stock")
                    {
                        query = @"
                        SELECT DISTINCT item_edesc, item_code 
                        FROM V$VIRTUAL_STOCK_WIP_LEDGER1 
                        WHERE company_code = :CompanyCode 
                        AND LOWER(REPLACE(item_edesc, ' ', '')) LIKE '%' || :CustomerName || '%'";
                    }

                    if (!string.IsNullOrEmpty(query))
                    {
                        using (var command = new OracleCommand(query, connection))
                        {
                            command.Parameters.Add(new OracleParameter("CompanyCode", company_code));
                            command.Parameters.Add(new OracleParameter("CustomerName", customerName));

                            var allCodes = await command.ExecuteReaderAsync();
                            while (await allCodes.ReadAsync())
                            {
                                var customerDict = new Dictionary<string, string>
                            {
                                { "name", reportName == "balance" ? allCodes["sub_edesc"].ToString() : allCodes["item_edesc"].ToString() },
                                { "code", reportName == "balance" ? allCodes["sub_code"].ToString() : allCodes["item_code"].ToString() }
                            };
                                sampleCustomers.Add(customerDict);
                            }
                        }
                    }
                }
            }

            var result = new
            {
                data = sampleCustomers,
                report = reportName,
                sl_name = slTextToCompare
            };

            return Ok(result);
        }

        [HttpPost]
        [Route("RemoveWilds")]
        public IHttpActionResult RemoveWild([FromBody] string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest("Query cannot be null or empty.");
            }

            var cleanedQuery = RemoveWildCharacters(query); // Updated method name
            return Ok(cleanedQuery);
        }

        private string RemoveWildCharacters(string q) // Updated method name
        {
            return q.Replace("\\", string.Empty)
                     .Replace("\"", string.Empty)
                     .Replace("\t", string.Empty);
        }
        [HttpPost]
        [Route("FnClosing")]
        public IHttpActionResult FnClosing([FromBody] ClosingRequest request)
        {
            if (request == null || request.Drs == null || request.Crs == null)
            {
                return BadRequest("Invalid input data.");
            }

            if (request.Drs.Count != request.Crs.Count)
            {
                return BadRequest("Debit and Credit lists must be of the same length.");
            }

            var result = CalculateClosing(request.Drs, request.Crs);
            return Ok(result);
        }

        private List<double> CalculateClosing(List<double> drs, List<double> crs)
        {
            var closings = new List<double>();
            double closing = 0;

            for (int i = 0; i < drs.Count; i++)
            {
                closing = i == 0 ? drs[i] - crs[i] : closing - crs[i] + drs[i];
                closings.Add(closing);
            }

            return closings;
        }
        #endregion

        #region Data Module 2 Developed By: Prem Prakash Dhakal Date: 09-19-2024
        [HttpGet]
        [Route("api/branch/filter")]
        public IHttpActionResult BranchFilter(string companyCode, List<string> branchCode, string userNo)
        {
            if (branchCode == null || branchCode.Count == 0)
            {
                // Construct the SQL query for the case when branchCode is empty
                string query = $@"SELECT branch_code FROM SC_BRANCH_CONTROL 
                              WHERE user_no = @UserNo 
                              AND company_code IN ({companyCode})";

                // Execute the query and return the result
                using (var connection = new OracleConnection(_connection))
                {
                    connection.Open();
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserNo", userNo);
                        var reader = command.ExecuteReader();
                        var branches = new List<string>();
                        while (reader.Read())
                        {
                            branches.Add(reader["branch_code"].ToString());
                        }
                        return Ok(branches);
                    }
                }
            }
            else if (branchCode.Count == 1)
            {
                branchCode.Add("0");
            }

            // Convert to tuple-like format (C# doesn't have tuples like Python)
            return Ok(branchCode);
        }
        [HttpGet]
        [Route("api/branch/names")]
        public IHttpActionResult BranchFilterNames(List<string> sendedBranchCode, string userNo, string companyCode)
        {
            string query;
            if (sendedBranchCode == null || sendedBranchCode.Count == 0)
            {
                query = $@"
                SELECT branch_edesc 
                FROM FA_BRANCH_SETUP 
                WHERE branch_code IN 
                (SELECT branch_code FROM SC_BRANCH_CONTROL WHERE user_no = @UserNo AND company_code IN ({companyCode}))";
            }
            else if (sendedBranchCode.Count == 1)
            {
                sendedBranchCode.Add("0");
                var branchCodes = string.Join(",", sendedBranchCode.Select(b => $"'{b}'"));
                query = $@"
                SELECT branch_edesc 
                FROM FA_BRANCH_SETUP 
                WHERE branch_code IN ({branchCodes}) AND company_code IN ({companyCode})";
            }
            else
            {
                var branchCodes = string.Join(",", sendedBranchCode.Select(b => $"'{b}'"));
                query = $@"
                SELECT branch_edesc 
                FROM FA_BRANCH_SETUP 
                WHERE branch_code IN ({branchCodes}) AND company_code IN ({companyCode})";
            }

            using (var connection = new OracleConnection(_connection))
            {
                connection.Open();
                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserNo", userNo); // Use parameter for userNo
                    var reader = command.ExecuteReader();
                    var branches = new List<string>();

                    while (reader.Read())
                    {
                        branches.Add(reader["branch_edesc"].ToString());
                    }
                    return Ok(branches);
                }
            }
        }
        public Dictionary<string, dynamic> GetMultiQty(string unitType, string companyCode)
        {
            var multiQtyDict = new Dictionary<string, dynamic>();
            string query = $@"
            SELECT ITEM_CODE, MU_CODE, RATIO
            FROM MultiQtyTable
            WHERE UNIT_TYPE = @UnitType AND COMPANY_CODE = @CompanyCode";
            using (var connection = new OracleConnection(_connection))
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    // Add parameters to prevent SQL injection
                    command.Parameters.AddWithValue("@UnitType", unitType);
                    command.Parameters.AddWithValue("@CompanyCode", companyCode);
                    // Execute the command and read the results
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string itemCode = reader["ITEM_CODE"].ToString();
                            string muCode = reader["MU_CODE"].ToString();
                            double ratio = Convert.ToDouble(reader["RATIO"]);
                            // Add to the dictionary
                            multiQtyDict[itemCode] = new
                            {
                                mu_code = muCode,
                                ratio = ratio
                            };
                        }
                    }
                }
            }
            return multiQtyDict;
        }
        private SalesRecord ProcessCodes(OracleDataReader reader)
        {
            try
            {
                var salesRecord = new SalesRecord
                {
                    Code = reader.GetString(0), 
                    Description = reader.GetString(1), 
                    Amount = reader.GetDouble(2) 
                                                 
                };
                return salesRecord;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [HttpGet]
        [Route("GetProductWiseSalesReport")]
        public Dictionary<string, object> GetProductWiseSalesReport(string companyCode,string branchCode,
          string userNo, string fromDate, string toDate, string grossNetStr,
          string dealerCode, string customerStr, string itemStringFilter,
          List<string> itemCode,
          string customerCode,
          string months,
          string brand,
          string cat,
          string grp,
          bool withEngDate,
          string cusGrp,
          string cusGrpStr,
          string repType,
          string unitType,
          string grossNet)
        {
            var companyCodeTuple = companyCode + "','0";
            customerCode = customerCode.TrimStart();
            grossNetStr = grossNet == "net" ? "NET_SALES_RATE" : "NET_GROSS_RATE";
            string brandStr = "";
            if (!string.IsNullOrEmpty(brand))
            {
                brandStr = $@" and item_code in (select item_code from iP_ITEM_SPEC_SETUP where brand_name='{brand}' and company_code in {companyCode}) ";
            }
            string grpStr = "";
            if (!string.IsNullOrEmpty(grp))
            {
                grpStr = $@" and item_code in (select item_code from ip_item_master_setup where pre_item_code like '{grp}%' and company_code in {companyCode}) ";
            }

             cusGrpStr = "";
            string subCodeCusStr = "";

            if (!string.IsNullOrEmpty(cusGrp))
            {
                cusGrpStr = $@" and customer_code in (select customer_code from sa_customer_setup where pre_customer_code like '{cusGrp}%' and company_code in {companyCode}) ";
                subCodeCusStr = $@" and sub_code in (select distinct('C'||customer_code) from sa_customer_setup where pre_customer_code like '{cusGrp}%' and company_code in {companyCode}) ";
            }

            string catStr = "";
            if (!string.IsNullOrEmpty(cat))
            {
                catStr = $@" and item_code in (select item_code from ip_item_master_setup where category_code in ('{cat}') and company_code in {companyCode}) ";
            }

             itemStringFilter = "";

            if (itemCode.Count == 0)
            {
                itemStringFilter = $@" and item_code in (select item_code from ip_item_master_setup where company_code in {companyCode} {brandStr} {grpStr} {catStr})";
            }
            else if (itemCode.Count == 1)
            {
                itemCode.Add("0"); // assuming itemCode is a List<string>
                var itemCodeTuple = string.Join(",", itemCode.Select(ic => $"'{ic}'"));
                itemStringFilter = $@" and item_code in ({itemCodeTuple}) ";
                itemStringFilter += $@" and item_code in (select item_code from ip_item_master_setup where item_code in ({itemCodeTuple}) and company_code in {companyCode} {brandStr} {grpStr} {catStr}) ";
            }
            else if (itemCode.Count != 0)
            {
                var itemCodeTuple = string.Join(",", itemCode.Select(ic => $"'{ic}'"));
                itemStringFilter = $@" and item_code in ({itemCodeTuple}) and company_code in {companyCode} ";
            }
            if (string.IsNullOrEmpty(cusGrp) && string.IsNullOrEmpty(cat) && string.IsNullOrEmpty(brand) && itemCode.Count == 0)
            {
                itemStringFilter = "";
            }
             customerStr = "";
            string subCodeStr = "";
             grossNetStr = "NET_GROSS_RATE";
            if (grossNet == "net")
            {
                grossNetStr = "NET_SALES_RATE";
            }

            if (!string.IsNullOrEmpty(customerCode))
            {
                customerStr = $@" AND A.customer_code ='{customerCode}' ";
                subCodeStr = $@" and sub_code='C{customerCode}' ";
            }
            if (repType == "product_wise_sales")
            {

                // Initialize multiUnitD as an empty dictionary.
                Dictionary<string, dynamic> multiUnitD = new Dictionary<string, dynamic>();

                // Check if unitType is provided and assign the result of GetMultiQty.
                if (!string.IsNullOrEmpty(unitType))
                {
                    var result = GetMultiQty(unitType, companyCode);

                    // Ensure the result is not null before assigning it to multiUnitD.
                    if (result != null)
                    {
                        multiUnitD = result;
                    }
                }
                // SQL query construction (with placeholders for data).
                string query = $@"
                    SELECT ITEM_EDESC, INDEX_MU_CODE, SALES_ROLL_QTY, SALES_QTY, SALES_VALUE, SALES_RET_ROLL_QTY, 
                           SALES_RET_QTY, SALES_RET_VALUE, 
                           SALES_ROLL_QTY - SALES_RET_ROLL_QTY AS NET_ROLL_QTY,
                           SALES_QTY - SALES_RET_QTY AS NET_SALES_QTY, 
                           SALES_VALUE - SALES_RET_VALUE AS NET_SALES_VALUE, ITEM_CODE
                    FROM 
                    ( 
                        SELECT D.ITEM_EDESC, D.INDEX_MU_CODE, SUM(A.SALES_ROLL_QTY) AS SALES_ROLL_QTY, 
                               SUM(A.SALES_QTY) AS SALES_QTY, SUM(A.SALES_VALUE) AS SALES_VALUE, 
                               SUM(A.SALES_RET_ROLL_QTY) AS SALES_RET_ROLL_QTY, SUM(A.SALES_RET_QTY) AS SALES_RET_QTY, 
                               SUM(A.SALES_RET_VALUE) AS SALES_RET_VALUE, A.ITEM_CODE 
                        FROM 
                        ( 
                            SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.ITEM_CODE, 
                                   SUM(ISNULL(A.ROLL_QTY, 0)) AS SALES_ROLL_QTY, 
                                   SUM(ISNULL(A.QUANTITY, 0)) AS SALES_QTY, 
                                   SUM(ISNULL(A.QUANTITY * A.{grossNetStr}, 0)) AS SALES_VALUE, 
                                   0 AS SALES_RET_ROLL_QTY, 0 AS SALES_RET_QTY, 0 AS SALES_RET_VALUE  
                            FROM SA_SALES_INVOICE A 
                            WHERE A.DELETED_FLAG = 'N' {customerStr} 
                                  AND A.BRANCH_CODE IN {GetBranchFilter(companyCode, branchCode, userNo)} 
                                  {itemStringFilter} 
                                  AND A.COMPANY_CODE IN {companyCode} 
                                  AND A.SALES_DATE BETWEEN '{fromDate}' AND '{toDate}' 
                            GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.ITEM_CODE 
                    
                            UNION ALL 
                    
                            SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.ITEM_CODE, 
                                   0 AS SALES_ROLL_QTY, 0 AS SALES_QTY, 0 AS SALES_VALUE, 
                                   SUM(ISNULL(A.ROLL_QTY, 0)) AS SALES_RET_ROLL_QTY, 
                                   SUM(ISNULL(A.QUANTITY, 0)) AS SALES_RET_QTY, 
                                   SUM(ISNULL(A.QUANTITY * A.{grossNetStr}, 0)) AS SALES_RET_VALUE  
                            FROM SA_SALES_RETURN A 
                            WHERE A.DELETED_FLAG = 'N' 
                                  AND A.COMPANY_CODE IN {companyCode} 
                                  AND A.BRANCH_CODE IN {GetBranchFilter(companyCode, branchCode, userNo)} 
                                  {itemStringFilter} {customerStr} 
                                  AND A.RETURN_DATE BETWEEN '{fromDate}' AND '{toDate}' 
                            GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.ITEM_CODE 
                        ) A 
                        JOIN IP_ITEM_MASTER_SETUP D ON A.COMPANY_CODE = D.COMPANY_CODE AND A.ITEM_CODE = D.ITEM_CODE 
                        GROUP BY A.ITEM_CODE, D.ITEM_EDESC, D.INDEX_MU_CODE 
                    ) 
                    ORDER BY ITEM_EDESC";

                var productData = new List<Dictionary<string, object>>();
                var salesQty = new List<double>();
                var salesAmt = new List<double>();
                var salesRetQty = new List<double>();
                var salesRetAmt = new List<double>();
                var netSalesQty = new List<double>();
                var netSalesAmt = new List<double>();

                // Execute SQL query.
                using (var connection = new OracleConnection(_connection))
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string itemCodes = reader["ITEM_CODE"].ToString();
                                double qty = Convert.ToDouble(reader["SALES_QTY"]);
                                double qtyRet = Convert.ToDouble(reader["SALES_RET_QTY"]);
                                double qtyNet = Convert.ToDouble(reader["NET_SALES_QTY"]);
                                string unit = reader["INDEX_MU_CODE"].ToString();

                                // Check if unitType is provided and adjust quantities based on multiUnitD ratios.
                                if (!string.IsNullOrEmpty(unitType) && multiUnitD != null && multiUnitD.ContainsKey(itemCodes))
                                {
                                    qty *= multiUnitD[itemCodes]["ratio"];
                                    qtyRet *= multiUnitD[itemCodes]["ratio"];
                                    qtyNet *= multiUnitD[itemCodes]["ratio"];
                                    unit = multiUnitD[itemCodes]["mu_code"];
                                }

                                // Add the calculated values to lists.
                                salesQty.Add(qty);
                                salesAmt.Add(Convert.ToDouble(reader["SALES_VALUE"]));
                                salesRetQty.Add(qtyRet);
                                salesRetAmt.Add(Convert.ToDouble(reader["SALES_RET_VALUE"]));
                                netSalesQty.Add(qtyNet);
                                netSalesAmt.Add(Convert.ToDouble(reader["NET_SALES_VALUE"]));

                                // Add each record to the productData list.
                                productData.Add(new Dictionary<string, object>
                                {
                                    { "item", reader["ITEM_EDESC"].ToString() },
                                    { "unit", unit },
                                    { "sales_qty", qty },
                                    { "sales_amt", Convert.ToDouble(reader["SALES_VALUE"]) },
                                    { "sales_ret_qty", qtyRet },
                                    { "sales_ret_amt", Convert.ToDouble(reader["SALES_RET_VALUE"]) },
                                    { "net_sales_qty", qtyNet },
                                    { "net_sales_amt", Convert.ToDouble(reader["NET_SALES_VALUE"]) }
                                });
                            }
                        }
                    }
                }
                // Prepare the final report.
                var productWiseReport = new Dictionary<string, object>
                    {
                        { "product_wise_report", productData },
                        { "branches", GetBranchFilter(branchCode, userNo, companyCode) },
                        { "Total_sales_qty", salesQty.Sum() },
                        { "Total_sales_amt", salesAmt.Sum() },
                        { "Total_sales_ret_qty", salesRetQty.Sum() },
                        { "Total_sales_ret_amt", salesRetAmt.Sum() },
                        { "Total_net_sales_qty", netSalesQty.Sum() },
                        { "Total_net_sales_amt", netSalesAmt.Sum() }
                    };

                return productWiseReport;
            }
            else if(repType == "dealer_customer_wise_sales")
            {
                // First Query
                string dealerCodeFilter = string.IsNullOrEmpty(dealerCode) ? "" : $" AND A.PARTY_TYPE_CODE='{dealerCode}'";

                string firstQuery = $@"
                    SELECT ITEM_EDESC, INDEX_MU_CODE, SALES_ROLL_QTY, SALES_QTY, SALES_VALUE, 
                           SALES_RET_ROLL_QTY, SALES_RET_QTY, SALES_RET_VALUE, 
                           SALES_ROLL_QTY - SALES_RET_ROLL_QTY AS NET_ROLL_QTY,
                           SALES_QTY - SALES_RET_QTY AS NET_SALES_QTY, 
                           SALES_VALUE - SALES_RET_VALUE AS NET_SALES_VALUE, 
                           ITEM_CODE
                    FROM (
                        SELECT D.ITEM_EDESC, D.INDEX_MU_CODE, SUM(A.SALES_ROLL_QTY) AS SALES_ROLL_QTY, 
                               SUM(A.SALES_QTY) AS SALES_QTY, SUM(A.SALES_VALUE) AS SALES_VALUE, 
                               SUM(SALES_RET_ROLL_QTY) AS SALES_RET_ROLL_QTY, 
                               SUM(A.SALES_RET_QTY) AS SALES_RET_QTY,
                               SUM(A.SALES_RET_VALUE) AS SALES_RET_VALUE, A.ITEM_CODE 
                        FROM (
                            SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.ITEM_CODE, 
                                   SUM(ISNULL(A.ROLL_QTY, 0)) AS SALES_ROLL_QTY, 
                                   SUM(ISNULL(A.QUANTITY, 0)) AS SALES_QTY, 
                                   SUM(ISNULL(A.QUANTITY * A.{grossNetStr}, 0)) AS SALES_VALUE,
                                   0 AS SALES_RET_ROLL_QTY, 0 AS SALES_RET_QTY, 0 AS SALES_RET_VALUE  
                            FROM SA_SALES_INVOICE A 
                            WHERE A.DELETED_FLAG = 'N' {customerStr}
                              AND A.BRANCH_CODE IN ({GetBranchFilter(companyCode, branchCode, userNo)})
                              AND A.COMPANY_CODE IN ({companyCode})                        
                              AND TRUNC(A.SALES_DATE) BETWEEN '{fromDate}' AND '{toDate}'
                            GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.ITEM_CODE 
                        
                            UNION ALL 
                        
                            SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.ITEM_CODE, 0 AS SALES_ROLL_QTY, 
                                   0 AS SALES_QTY, 0 AS SALES_VALUE, 
                                   SUM(ISNULL(A.ROLL_QTY, 0)) AS SALES_RET_ROLL_QTY, 
                                   SUM(ISNULL(A.QUANTITY, 0)) AS SALES_RET_QTY,  
                                   SUM(ISNULL(A.QUANTITY * A.{grossNetStr}, 0)) AS SALES_RET_VALUE  
                            FROM SA_SALES_RETURN A
                            WHERE A.DELETED_FLAG = 'N' 
                              AND A.PARTY_TYPE_CODE='{dealerCode}' 
                              AND A.COMPANY_CODE IN ({companyCode})
                              AND A.BRANCH_CODE IN ({GetBranchFilter(companyCode, branchCode, userNo)})
                              {customerStr}
                              AND TRUNC(A.RETURN_DATE) BETWEEN '{fromDate}' AND '{toDate}' 
                            GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.ITEM_CODE
                        ) A, IP_ITEM_MASTER_SETUP D 
                        WHERE A.COMPANY_CODE = D.COMPANY_CODE AND A.ITEM_CODE = D.ITEM_CODE 
                        GROUP BY A.ITEM_CODE, D.ITEM_EDESC, D.INDEX_MU_CODE  
                    ) ORDER BY ITEM_EDESC 
                ";

                // Execute the first query
                var firstResults = ExecuteOracleQuery(firstQuery);

                // Second Query
                string secondQuery = $@"
                    SELECT PARTY_TYPE_EDESC, CUSTOMER_EDESC, SUM(SALES_QTY), SUM(SALES_VALUE), 
                           SUM(SALES_RET_QTY), SUM(SALES_RET_VALUE),
                           SUM(SALES_QTY) - SUM(SALES_RET_QTY) AS NET_SALES_QTY, 
                           SUM(SALES_VALUE) - SUM(SALES_RET_VALUE) AS NET_SALES_VALUE, 
                           PARTY_TYPE_CODE
                    FROM (
                        SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.PARTY_TYPE_CODE, A.CUSTOMER_CODE, A.ITEM_CODE, 
                               SUM(ISNULL(A.QUANTITY, 0)) AS SALES_QTY, 
                               SUM(ISNULL(A.QUANTITY * A.NET_SALES_RATE, 0)) AS SALES_VALUE, 
                               0 AS SALES_RET_QTY, 0 AS SALES_RET_VALUE  
                        FROM SA_SALES_INVOICE A 
                        WHERE A.DELETED_FLAG = 'N' 
                          AND A.BRANCH_CODE IN ({GetBranchFilter(companyCode, branchCode, userNo)})                        
                          AND A.COMPANY_CODE IN ({companyCode})  
                          {dealerCodeFilter}                      
                          AND TRUNC(A.SALES_DATE) BETWEEN '{fromDate}' AND '{toDate}' 
                        GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.PARTY_TYPE_CODE, A.CUSTOMER_CODE, A.ITEM_CODE 
                    
                        UNION ALL 
                    
                        SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.PARTY_TYPE_CODE, A.CUSTOMER_CODE, A.ITEM_CODE, 
                               0 AS SALES_QTY, 0 AS SALES_VALUE, 
                               SUM(ISNULL(A.QUANTITY, 0)) AS SALES_RET_QTY,  
                               SUM(ISNULL(A.QUANTITY * A.NET_SALES_RATE, 0)) AS SALES_RET_VALUE  
                        FROM SA_SALES_RETURN A 
                        WHERE A.DELETED_FLAG = 'N' 
                          AND A.BRANCH_CODE IN ({GetBranchFilter(companyCode, branchCode, userNo)})
                          AND A.COMPANY_CODE IN ({companyCode}) 
                          {dealerCodeFilter}                                              
                          AND TRUNC(A.RETURN_DATE) BETWEEN '{fromDate}' AND '{toDate}' 
                        GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.PARTY_TYPE_CODE, A.CUSTOMER_CODE, A.ITEM_CODE
                    ) a
                    LEFT JOIN (
                        SELECT PARTY_TYPE_CODE b_code, PARTY_TYPE_EDESC FROM IP_PARTY_TYPE_CODE 
                        WHERE COMPANY_CODE IN ({companyCode}) AND DELETED_FLAG='N'
                    ) b ON a.PARTY_TYPE_CODE = b.b_code
                    LEFT JOIN (
                        SELECT CUSTOMER_CODE c_code, CUSTOMER_EDESC FROM SA_CUSTOMER_SETUP 
                        WHERE COMPANY_CODE IN ({companyCode}) AND DELETED_FLAG='N'
                    ) c ON a.CUSTOMER_CODE = c.c_code
                    LEFT JOIN (
                        SELECT ITEM_CODE d_code, ITEM_EDESC FROM IP_ITEM_MASTER_SETUP 
                        WHERE COMPANY_CODE IN ({companyCode}) AND DELETED_FLAG='N'
                    ) d ON a.ITEM_CODE = d.d_code
                    GROUP BY PARTY_TYPE_CODE, PARTY_TYPE_EDESC, CUSTOMER_EDESC
                    ORDER BY 1, 3
                ";

                // Execute the second query
                var secondResults = ExecuteOracleQuery(secondQuery);

                // Combine results from both queries if needed
                var combinedResults = new Dictionary<string, object>
                {
                    { "firstQueryResults", firstResults },
                    { "secondQueryResults", secondResults }
                };
                return combinedResults;
            }
            else if(repType == "customer_wise_sales")
            {
                string query = $@"
                    SELECT CUSTOMER_EDESC, master_customer_code, pre_customer_code, group_sku_flag,
                        ISNULL(SALES_QTY, 0), ISNULL(sales_value, 0), ISNULL(SALES_RET_QTY, 0), 
                        ISNULL(SALES_RET_VALUE, 0), ISNULL(NET_SALES_QTY, 0), ISNULL(NET_SALES_VALUE, 0)
                    FROM (
                        SELECT * FROM (
                            SELECT A.CUSTOMER_CODE, A.CUSTOMER_EDESC, A.master_customer_code, 
                                   A.pre_customer_code, A.group_sku_flag 
                            FROM SA_CUSTOMER_SETUP A 
                            WHERE A.COMPANY_CODE IN ('01', '02', '0')
                            GROUP BY A.CUSTOMER_CODE, A.CUSTOMER_EDESC, 
                                     A.master_customer_code, A.pre_customer_code, 
                                     A.group_sku_flag
                        ) A
                        LEFT OUTER JOIN (
                            SELECT COD, NET_SALES_VALUE, SALES_QTY, sales_value, 
                                   SALES_RET_QTY, SALES_RET_VALUE, NET_SALES_QTY 
                            FROM (
                                SELECT cus_name, Cod, SALES_ROLL_QTY, SALES_QTY, 
                                       SALES_VALUE, SALES_RET_ROLL_QTY,
                                       SALES_RET_QTY, SALES_RET_VALUE, 
                                       SALES_ROLL_QTY - SALES_RET_ROLL_QTY AS NET_ROLL_QTY,
                                       SALES_QTY - SALES_RET_QTY AS NET_SALES_QTY,
                                       SALES_VALUE - SALES_RET_VALUE AS NET_SALES_VALUE 
                                FROM (
                                    SELECT D.CUS_name, D.cod, 
                                           SUM(A.SALES_ROLL_QTY) AS SALES_ROLL_QTY, 
                                           SUM(A.SALES_QTY) AS SALES_QTY,
                                           SUM(A.SALES_VALUE) AS SALES_VALUE, 
                                           SUM(SALES_RET_ROLL_QTY) AS SALES_RET_ROLL_QTY, 
                                           SUM(A.SALES_RET_QTY) AS SALES_RET_QTY,
                                           SUM(A.SALES_RET_VALUE) AS SALES_RET_VALUE 
                                    FROM (
                                        SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.customer_code,
                                               SUM(ISNULL(A.ROLL_QTY, 0)) AS SALES_ROLL_QTY, 
                                               SUM(ISNULL(A.QUANTITY, 0)) AS SALES_QTY,
                                               SUM(ISNULL(A.QUANTITY * A.NET_sales_RATE, 0)) AS SALES_VALUE,
                                               0 AS SALES_RET_ROLL_QTY, 
                                               0 AS SALES_RET_QTY, 
                                               0 AS SALES_RET_VALUE  
                                        FROM SA_SALES_INVOICE A 
                                        WHERE A.DELETED_FLAG = 'N' 
                                              AND A.COMPANY_CODE IN {companyCode}
                                              {customerStr} {cusGrpStr}
                                              AND A.BRANCH_CODE IN {GetBranchFilter(companyCode, branchCode, userNo)}
                                              AND A.COMPANY_CODE IN {companyCode}
                                              AND CAST(a.SALES_DATE AS DATE) BETWEEN '{fromDate}' AND '{toDate}'
                                        GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.customer_CODE

                                        UNION ALL

                                        SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.customer_code, 
                                               0 AS SALES_ROLL_QTY, 
                                               0 AS SALES_QTY, 
                                               0 AS SALES_VALUE,
                                               SUM(ISNULL(A.ROLL_QTY, 0)) AS SALES_RET_ROLL_QTY, 
                                               SUM(ISNULL(A.QUANTITY, 0)) AS SALES_RET_QTY,
                                               SUM(ISNULL(A.QUANTITY * A.NET_sales_RATE, 0)) AS SALES_RET_VALUE
                                        FROM SA_SALES_RETURN A 
                                        WHERE A.DELETED_FLAG = 'N'
                                              AND A.COMPANY_CODE IN {companyCode}
                                              AND A.BRANCH_CODE IN {GetBranchFilter(companyCode, branchCode, userNo)} 
                                              {customerStr} {cusGrpStr}
                                              AND CAST(A.RETURN_DATE AS DATE) BETWEEN '{fromDate}' AND '{toDate}' 
                                        GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.customer_CODE
                                    ) A
                                    LEFT OUTER JOIN (
                                        SELECT customer_code AS cod, customer_edesc AS cus_name 
                                        FROM SA_CUSTOMER_SETUP 
                                        WHERE 1 = 1 AND COMPANY_CODE IN {companyCode} 
                                        GROUP BY customer_code, customer_edesc
                                    ) D ON D.cod = A.CUSTOMER_CODE
                                    GROUP BY D.cod, D.cus_name
                                ) ORDER BY Cus_name
                            ) B ON A.customer_code = b.cod
                        ) 
                        ORDER BY 2
                ";

                //Console.WriteLine(query);
                List<object[]> allCodes = ExecuteQueryList(query);
                var vals = new List<Dictionary<string, object>>();
                var totalValues = new double[6];
                foreach (var i in allCodes)
                {
                    var result = ProcessCodes(allCodes, i[0].ToString(), i[1].ToString(), i[2].ToString(),
                                              i[3].ToString(), Convert.ToDouble(i[4]), Convert.ToDouble(i[5]),
                                              Convert.ToDouble(i[6]), Convert.ToDouble(i[7]),
                                              Convert.ToDouble(i[8]), Convert.ToDouble(i[9]));

                    if (result != null)
                    {
                        vals.Add(result);
                        if (i[3].ToString() == "I")
                        {
                            totalValues[0] += Convert.ToDouble(i[4]);
                            totalValues[1] += Convert.ToDouble(i[5]);
                            totalValues[2] += Convert.ToDouble(i[6]);
                            totalValues[3] += Convert.ToDouble(i[7]);
                            totalValues[4] += Convert.ToDouble(i[8]);
                            totalValues[5] += Convert.ToDouble(i[9]);
                        }
                    }
                }
                var dd = new Dictionary<string, object>
                {
                    { "detail", vals },
                    { "total1", totalValues[0] },
                    { "total2", totalValues[1] },
                    { "total3", totalValues[2] },
                    { "total4", totalValues[3] },
                    { "total5", totalValues[4] },
                    { "total6", totalValues[5] }
                };

                return dd;
            }
            else if(repType == "group_wise_sales")
            {
                string query = $@"
                        SELECT cd, item_edesc, master_item_code, pre_item_code, group_sku_flag, xx, item_code, index_mu_code,
                            sales_roll_qty, NVL(sales_qty, 0), NVL(sales_value, 0), NVL(sales_ret_roll_qty, 0), 
                            NVL(sales_ret_qty, 0), NVL(sales_ret_value, 0), NVL(net_sales_qty, 0), NVL(net_sales_value, 0)
                        FROM (
                            SELECT ITEM_CODE CD, ITEM_EDESC, MASTER_ITEM_CODE, PRE_ITEM_CODE, group_sku_flag 
                            FROM IP_ITEM_MASTER_SETUP 
                            WHERE COMPANY_CODE IN ({companyCode}) AND deleted_flag = 'N'
                            GROUP BY ITEM_CODE, ITEM_EDESC, MASTER_ITEM_CODE, PRE_ITEM_CODE, group_sku_flag
                        ) A
                        LEFT OUTER JOIN (
                            SELECT ITEM_EDESC xx, ITEM_CODE, INDEX_MU_CODE, SALES_ROLL_QTY, SALES_QTY, SALES_VALUE, 
                                SALES_RET_ROLL_QTY, SALES_RET_QTY, SALES_RET_VALUE,
                                SALES_ROLL_QTY - SALES_RET_ROLL_QTY AS NET_ROLL_QTY,
                                SALES_QTY - SALES_RET_QTY AS NET_SALES_QTY,
                                SALES_VALUE - SALES_RET_VALUE AS NET_SALES_VALUE
                            FROM (
                                SELECT D.ITEM_EDESC, A.ITEM_CODE, D.INDEX_MU_CODE, 
                                    SUM(A.SALES_ROLL_QTY) AS SALES_ROLL_QTY,
                                    SUM(A.SALES_QTY) AS SALES_QTY,
                                    SUM(A.SALES_VALUE) AS SALES_VALUE,
                                    SUM(SALES_RET_ROLL_QTY) AS SALES_RET_ROLL_QTY,
                                    SUM(A.SALES_RET_QTY) AS SALES_RET_QTY,
                                    SUM(A.SALES_RET_VALUE) AS SALES_RET_VALUE
                                FROM (
                                    SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.ITEM_CODE,
                                        SUM(NVL(A.ROLL_QTY, 0)) AS SALES_ROLL_QTY,
                                        SUM(NVL(A.QUANTITY, 0)) AS SALES_QTY,
                                        SUM(NVL(A.QUANTITY * A.{grossNetStr}, 0)) AS SALES_VALUE,
                                        0 AS SALES_RET_ROLL_QTY, 
                                        0 AS SALES_RET_QTY, 
                                        0 AS SALES_RET_VALUE
                                    FROM SA_SALES_INVOICE A
                                    WHERE A.DELETED_FLAG = 'N' {customerStr} 
                                        AND A.BRANCH_CODE IN {GetBranchFilter(companyCode, branchCode, userNo)} 
                                        AND A.COMPANY_CODE IN ({companyCode}) 
                                        AND TRUNC(a.SALES_DATE) BETWEEN '{fromDate}' AND '{toDate}'
                                    GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.ITEM_CODE

                                    UNION ALL

                                    SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.ITEM_CODE, 
                                        0 AS SALES_ROLL_QTY, 
                                        0 AS SALES_QTY, 
                                        0 AS SALES_VALUE,
                                        SUM(NVL(A.ROLL_QTY, 0)) AS SALES_RET_ROLL_QTY,
                                        SUM(NVL(A.QUANTITY, 0)) AS SALES_RET_QTY,
                                        SUM(NVL(A.QUANTITY * A.{grossNetStr}, 0)) AS SALES_RET_VALUE
                                    FROM SA_SALES_RETURN A
                                    WHERE A.DELETED_FLAG = 'N' {customerStr} 
                                        AND A.BRANCH_CODE IN {GetBranchFilter(companyCode, branchCode, userNo)} 
                                        AND A.COMPANY_CODE IN ({companyCode}) 
                                        AND TRUNC(A.RETURN_DATE) BETWEEN '{fromDate}' AND '{toDate}'
                                    GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.ITEM_CODE
                                ) A, IP_ITEM_MASTER_SETUP D
                                WHERE A.COMPANY_CODE = D.COMPANY_CODE 
                                    AND A.ITEM_CODE = D.ITEM_CODE
                                GROUP BY A.ITEM_CODE, D.ITEM_EDESC, D.INDEX_MU_CODE
                            ) 
                            ORDER BY ITEM_EDESC
                        ) B ON B.ITEM_CODE = A.CD
                        ORDER BY A.MASTER_ITEM_CODE";

                        var allCodes = ExecuteQuery(query);
                        var vals = new List<Dictionary<string, object>>();
                        var Arrtotal1 = new List<double>();
                        var Arrtotal2 = new List<double>();
                        var Arrtotal3 = new List<double>();
                        var Arrtotal4 = new List<double>();
                        var Arrtotal5 = new List<double>();
                        var Arrtotal6 = new List<double>();
                
                                //foreach (var i in allCodes)
                                //{
                                //    if (i is Dictionary<string, object> array && array.Count > 15)
                                //    {
                                //        double total1 = 0, total2 = 0, total3 = 0, total4 = 0, total5 = 0, total6 = 0;
                                //        if (array["group_flag"].ToString() == "G")
                                //        {
                                //            bool addOn = false;
                                //            foreach (var z in allCodes)
                                //            {
                                //                if (z is Dictionary<string, object> subArray && subArray.Count > 15 && subArray["group_flag"].ToString() == "I") // Use the appropriate key
                                //                {
                                //                    string commonPrefix = subArray["item"].ToString()
                                //                        .Substring(0, Math.Min(subArray["item"].ToString().Length, array["item"].ToString().Length)); // Use appropriate keys
                                //                    if (array["item"].ToString() == commonPrefix)
                                //                    {
                                //                        addOn = true;
                                //                        try
                                //                        {
                                //                            total1 += Convert.ToDouble(subArray["sales_qty"]);
                                //                            total2 += Convert.ToDouble(subArray["sales_amt"]);
                                //                            total3 += Convert.ToDouble(subArray["sales_ret_qty"]);
                                //                            total4 += Convert.ToDouble(subArray["sales_ret_amt"]);
                                //                            total5 += Convert.ToDouble(subArray["net_sales_qty"]);
                                //                            total6 += Convert.ToDouble(subArray["net_sales_amt"]);
                                //                        }
                                //                        catch (FormatException)
                                //                        {

                                //                        }
                                //                    }
                                //                    else if (addOn)
                                //                    {
                                //                        break;
                                //                    }
                                //                }
                                //            }

                                //            if (total1 > 0 || total2 > 0 || total3 > 0 || total4 > 0 || total5 > 0 || total6 > 0)
                                //            {
                                //                var cals = new Dictionary<string, object>
                                //                                    {
                                //                                        { "item", array["item"] },
                                //                                        { "group_flag", array["group_flag"] },
                                //                                        { "unit", array["unit"] },
                                //                                        { "sales_qty", total1 },
                                //                                        { "sales_amt", total2 },
                                //                                        { "sales_ret_qty", total3 },
                                //                                        { "sales_ret_amt", total4 },
                                //                                        { "net_sales_qty", total5 },
                                //                                        { "net_sales_amt", total6 },
                                //                                        { "level", array["item"].ToString().Count(c => c == '.') },
                                //                                        { "master", array["item"] },
                                //                                        { "pre", array["pre"] }
                                //                                    };
                                //                vals.Add(cals);
                                //            }
                                //        }
                                //        else if (array["group_flag"].ToString() == "I")
                                //        {
                                //            try
                                //            {
                                //                Arrtotal1.Add(Convert.ToDouble(array["sales_qty"]));
                                //                Arrtotal2.Add(Convert.ToDouble(array["sales_amt"]));
                                //                Arrtotal3.Add(Convert.ToDouble(array["sales_ret_qty"]));
                                //                Arrtotal4.Add(Convert.ToDouble(array["sales_ret_amt"]));
                                //                Arrtotal5.Add(Convert.ToDouble(array["net_sales_qty"]));
                                //            }
                                //            catch (FormatException)
                                //            {

                                //            }
                                //        }
                                //    }
                                //}
                            //foreach (var i in allCodes)
                            //{
                            //    if (i is Dictionary<string, object> array && array.Count > 15)
                            //    {
                            //        double total1 = 0, total2 = 0, total3 = 0, total4 = 0, total5 = 0, total6 = 0;
                            //        if (array.ContainsKey("group_flag") && array["group_flag"].ToString() == "G")
                            //        {
                            //            bool addOn = false;
                            //            foreach (var z in allCodes)
                            //            {
                            //                if (z is Dictionary<string, object> subArray && subArray.Count > 15
                            //                    && subArray.ContainsKey("group_flag")
                            //                    && subArray["group_flag"].ToString() == "I")
                            //                {
                            //                    string commonPrefix = subArray.ContainsKey("item") && array.ContainsKey("item")
                            //                        ? subArray["item"].ToString().Substring(0, Math.Min(subArray["item"].ToString().Length, array["item"].ToString().Length))
                            //                        : string.Empty; 

                            //                    if (array.ContainsKey("item") && array["item"].ToString() == commonPrefix)
                            //                    {
                            //                        addOn = true;
                            //                        try
                            //                        {
                            //                            total1 += Convert.ToDouble(subArray["sales_qty"]);
                            //                            total2 += Convert.ToDouble(subArray["sales_amt"]);
                            //                            total3 += Convert.ToDouble(subArray["sales_ret_qty"]);
                            //                            total4 += Convert.ToDouble(subArray["sales_ret_amt"]);
                            //                            total5 += Convert.ToDouble(subArray["net_sales_qty"]);
                            //                            total6 += Convert.ToDouble(subArray["net_sales_amt"]);
                            //                        }
                            //                        catch (FormatException)
                            //                        {
                                            
                            //                        }
                            //                    }
                            //                    else if (addOn)
                            //                    {
                            //                        break;
                            //                    }
                            //                }
                            //            }

                            //            if (total1 > 0 || total2 > 0 || total3 > 0 || total4 > 0 || total5 > 0 || total6 > 0)
                            //            {
                            //                var cals = new Dictionary<string, object>
                            //                {
                            //                    { "item", array["item"] },
                            //                    { "group_flag", array["group_flag"] },
                            //                    { "unit", array["unit"] },
                            //                    { "sales_qty", total1 },
                            //                    { "sales_amt", total2 },
                            //                    { "sales_ret_qty", total3 },
                            //                    { "sales_ret_amt", total4 },
                            //                    { "net_sales_qty", total5 },
                            //                    { "net_sales_amt", total6 },
                            //                    { "level", array["item"].ToString().Count(c => c == '.') },
                            //                    { "master", array["item"] },
                            //                    { "pre", array["pre"] }
                            //                };
                            //                vals.Add(cals);
                            //            }
                            //        }
                            //        else if (array.ContainsKey("group_flag") && array["group_flag"].ToString() == "I")
                            //        {
                            //            try
                            //            {
                            //                Arrtotal1.Add(Convert.ToDouble(array["sales_qty"]));
                            //                Arrtotal2.Add(Convert.ToDouble(array["sales_amt"]));
                            //                Arrtotal3.Add(Convert.ToDouble(array["sales_ret_qty"]));
                            //                Arrtotal4.Add(Convert.ToDouble(array["sales_ret_amt"]));
                            //                Arrtotal5.Add(Convert.ToDouble(array["net_sales_qty"]));
                            //            }
                            //            catch (FormatException)
                            //            {
                                
                            //            }
                            //        }
                            //    }
                            //}
                            var productWiseReport = new Dictionary<string, object>
                                    {
                                        { "product_wise_report", vals },
                                        { "branches", GetBranchFilter(branchCode, userNo, companyCode) },
                                        { "Total_sales_qty", Arrtotal1.Sum() },
                                        { "Total_sales_amt", Arrtotal2.Sum() },
                                        { "Total_sales_ret_qty", Arrtotal3.Sum() },
                                        { "Total_sales_ret_amt", Arrtotal4.Sum() },
                                        { "Total_net_sales_qty", Arrtotal5.Sum() },
                                        { "Total_net_sales_amt", Arrtotal6.Sum() }
                                    };
                                return productWiseReport;
                            }
            else if(repType == "cat_wise_sales")
            {
                string query = $@"
                SELECT cat_name, CATEGORY_CODE, SALES_ROLL_QTY, SALES_QTY, SALES_VALUE, SALES_RET_ROLL_QTY, 
                       SALES_RET_QTY, SALES_RET_VALUE, SALES_ROLL_QTY - SALES_RET_ROLL_QTY AS NET_ROLL_QTY, 
                       SALES_QTY - SALES_RET_QTY AS NET_SALES_QTY, SALES_VALUE - SALES_RET_VALUE AS NET_SALES_VALUE 
                FROM (
                    SELECT e.cat_name, D.CATEGORY_CODE, SUM(A.SALES_ROLL_QTY) AS SALES_ROLL_QTY, SUM(A.SALES_QTY) AS SALES_QTY, 
                           SUM(A.SALES_VALUE) AS SALES_VALUE, SUM(SALES_RET_ROLL_QTY) AS SALES_RET_ROLL_QTY, SUM(A.SALES_RET_QTY) AS SALES_RET_QTY,
                           SUM(A.SALES_RET_VALUE) AS SALES_RET_VALUE 
                    FROM (
                        SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.ITEM_CODE, 
                               SUM(ISNULL(A.ROLL_QTY, 0)) AS SALES_ROLL_QTY, SUM(ISNULL(A.QUANTITY, 0)) AS SALES_QTY, 
                               SUM(ISNULL(A.QUANTITY * A.{grossNetStr}, 0)) AS SALES_VALUE, 0 AS SALES_RET_ROLL_QTY, 
                               0 AS SALES_RET_QTY, 0 AS SALES_RET_VALUE  
                        FROM SA_SALES_INVOICE A 
                        WHERE A.DELETED_FLAG = 'N' 
                        {customerStr} 
                        AND A.BRANCH_CODE IN {GetBranchFilter(companyCode, branchCode, userNo)} 
                        {itemStringFilter} 
                        AND A.COMPANY_CODE IN {companyCode} 
                        AND CAST(A.SALES_DATE AS DATE) BETWEEN '{fromDate}' AND '{toDate}' 
                        GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.ITEM_CODE 

                        UNION ALL 

                        SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.ITEM_CODE, 0 AS SALES_ROLL_QTY, 0 AS SALES_QTY, 
                               0 AS SALES_VALUE, SUM(ISNULL(A.ROLL_QTY, 0)) AS SALES_RET_ROLL_QTY, 
                               SUM(ISNULL(A.QUANTITY, 0)) AS SALES_RET_QTY,  
                               SUM(ISNULL(A.QUANTITY * A.{grossNetStr}, 0)) AS SALES_RET_VALUE 
                        FROM SA_SALES_RETURN A 
                        WHERE A.DELETED_FLAG = 'N' 
                        AND A.COMPANY_CODE IN {companyCode} 
                        AND A.BRANCH_CODE IN {GetBranchFilter(companyCode, branchCode, userNo)} 
                        {itemStringFilter} 
                        {customerStr} 
                        AND CAST(A.RETURN_DATE AS DATE) BETWEEN '{fromDate}' AND '{toDate}' 
                        GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.ITEM_CODE 
                    ) A 
                    INNER JOIN IP_ITEM_MASTER_SETUP D ON A.COMPANY_CODE = D.COMPANY_CODE AND A.ITEM_CODE = D.ITEM_CODE 
                    LEFT JOIN (
                        SELECT category_code AS cod, category_edesc AS cat_name 
                        FROM ip_category_code 
                        WHERE COMPANY_CODE IN {companyCode} 
                        GROUP BY category_code, category_edesc 
                    ) e ON D.category_code = e.cod 
                    GROUP BY e.cat_name, CATEGORY_CODE 
                ) 
                ORDER BY CATEGORY_CODE";
                var connection = new OracleConnection(_connection);
                OracleCommand command = new OracleCommand(query, connection);
                command.CommandType = CommandType.Text;
                //command.CommandText = query;
                command.ExecuteNonQuery();
                //Console.WriteLine(query);

                List<Dictionary<string, object>> productData = new List<Dictionary<string, object>>();
                List<double> salesQty = new List<double>();
                List<double> salesAmt = new List<double>();
                List<double> salesRetQty = new List<double>();
                List<double> salesRetAmt = new List<double>();
                List<double> netSalesQty = new List<double>();
                List<double> netSalesAmt = new List<double>();

                connection.Open();
                OracleDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    salesQty.Add(reader.GetDouble(3));
                    salesAmt.Add(reader.GetDouble(4));
                    salesRetQty.Add(reader.GetDouble(6));
                    salesRetAmt.Add(reader.GetDouble(7));
                    netSalesQty.Add(reader.GetDouble(9));
                    netSalesAmt.Add(reader.GetDouble(10));

                    productData.Add(new Dictionary<string, object>
                {
                    { "item", reader.GetString(0) },
                    { "unit", reader.GetString(1) },
                    { "sales_qty", reader.GetDouble(3) },
                    { "sales_amt", reader.GetDouble(4) },
                    { "sales_ret_qty", reader.GetDouble(6) },
                    { "sales_ret_amt", reader.GetDouble(7) },
                    { "net_sales_qty", reader.GetDouble(9) },
                    { "net_sales_amt", reader.GetDouble(10) }
                });
                }
                reader.Close();
                connection.Close();
                var productWiseReport = new Dictionary<string, object>
                    {
                        { "product_wise_report", productData },
                        { "branches", GetBranchFilter(branchCode, userNo, companyCode) },
                        { "Total_sales_qty", salesQty.Sum() },
                        { "Total_sales_amt", salesAmt.Sum() },
                        { "Total_sales_ret_qty", salesRetQty.Sum() },
                        { "Total_sales_ret_amt", salesRetAmt.Sum() },
                        { "Total_net_sales_qty", netSalesQty.Sum() },
                        { "Total_net_sales_amt", netSalesAmt.Sum() }
                    };

                return productWiseReport;
            }
            else if(repType == "cat_grp_wise_sales")
            {
                string query = $@"
                SELECT company_code, branch_code, item_code, sales_roll_qty,
                       sales_qty, sales_value, sales_ret_roll_qty, sales_ret_qty,
                       sales_ret_value, item_edesc, i_code, category_code,
                       c_code, 
                       CASE WHEN category_edesc IS NULL THEN 'Not defined' ELSE category_edesc END AS category_edesc,
                       NVL(sales_qty, 0) - NVL(sales_ret_qty, 0) AS qt,
                       NVL(sales_value, 0) - NVL(sales_ret_value, 0) AS am
                FROM (
                    SELECT * FROM (
                        SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.ITEM_CODE,
                               SUM(NVL(A.ROLL_QTY, 0)) AS SALES_ROLL_QTY, 
                               SUM(NVL(A.QUANTITY, 0)) AS SALES_QTY, 
                               SUM(NVL(A.QUANTITY * A.{grossNetStr}, 0)) AS SALES_VALUE,
                               0 AS SALES_RET_ROLL_QTY, 0 AS SALES_RET_QTY, 0 AS SALES_RET_VALUE  
                        FROM SA_SALES_INVOICE A 
                        WHERE A.DELETED_FLAG = 'N'
                        AND A.COMPANY_CODE IN {companyCode}
                        AND CAST(a.SALES_DATE AS DATE) BETWEEN '{fromDate}' AND '{toDate}' 
                        {customerStr} 
                        {itemStringFilter}
                        GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.ITEM_CODE
                        
                        UNION ALL

                        SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.ITEM_CODE, 
                               0 AS SALES_ROLL_QTY, 0 AS SALES_QTY, 0 AS SALES_VALUE,
                               SUM(NVL(A.ROLL_QTY, 0)) AS SALES_RET_ROLL_QTY, 
                               SUM(NVL(A.QUANTITY, 0)) AS SALES_RET_QTY,
                               SUM(NVL(A.QUANTITY * A.{grossNetStr}, 0)) AS SALES_RET_VALUE
                        FROM SA_SALES_RETURN A
                        WHERE A.DELETED_FLAG = 'N'
                        AND A.COMPANY_CODE IN {companyCode}
                        AND CAST(A.RETURN_DATE AS DATE) BETWEEN '{fromDate}' AND '{toDate}' 
                        {customerStr} 
                        {itemStringFilter}
                        GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.ITEM_CODE 
                    ) A 
                    LEFT OUTER JOIN (
                        SELECT item_edesc, item_code AS i_code, category_code 
                        FROM ip_item_master_setup 
                        WHERE COMPANY_CODE IN {companyCode}
                    ) B ON A.ITEM_CODE = B.i_code 
                    LEFT OUTER JOIN (
                        SELECT category_code AS c_code, category_edesc 
                        FROM ip_category_code 
                        WHERE COMPANY_CODE IN {companyCode}
                    ) C ON B.category_code = C.c_code
                )
                ORDER BY 13, 10 ASC";
                var connection = new OracleConnection(_connection);
                OracleCommand command = new OracleCommand(query, connection);
                command.CommandType = CommandType.Text;
                //command.CommandText = query;
                command.ExecuteNonQuery();
               
                //Console.WriteLine(query);

                List<Dictionary<string, object>> companyList = new List<Dictionary<string, object>>();
                List<double> totalSalesQty = new List<double>();
                List<double> totalSalesAmt = new List<double>();
                double salesQtySum = 0;
                double salesAmtSum = 0;

                connection.Open();
                OracleDataReader reader = command.ExecuteReader();

                string changedCompany = null;
                List<Dictionary<string, object>> branches = new List<Dictionary<string, object>>();
                List<double> v1 = new List<double>();
                List<double> v2 = new List<double>();

                while (reader.Read())
                {
                    string currentCompany = reader[13].ToString();

                    if (changedCompany != currentCompany)
                    {
                        if (changedCompany != null)
                        {
                            companyList.Add(new Dictionary<string, object>
                        {
                            { "detail", branches },
                            { "name", changedCompany },
                            { "sales_qty", salesQtySum },
                            { "sales_amt", salesAmtSum }
                        });
                        }

                        changedCompany = currentCompany;
                        branches = new List<Dictionary<string, object>>();
                        salesQtySum = 0;
                        salesAmtSum = 0;
                    }
                    var branchData = new Dictionary<string, object>
                    {
                        { "name", reader[9].ToString() },
                        { "sales_qty", Convert.ToDouble(reader[14]) },
                        { "sales_amt", Convert.ToDouble(reader[15]) }
                    };
                    branches.Add(branchData);
                    v1.Add(Convert.ToDouble(reader[14]));
                    v2.Add(Convert.ToDouble(reader[15]));
                    salesQtySum += Convert.ToDouble(reader[14]);
                    salesAmtSum += Convert.ToDouble(reader[15]);
                    totalSalesQty.Add(Convert.ToDouble(reader[14]));
                    totalSalesAmt.Add(Convert.ToDouble(reader[15]));
                }
                if (changedCompany != null)
                {
                    companyList.Add(new Dictionary<string, object>
                    {
                        { "detail", branches },
                        { "name", changedCompany },
                        { "sales_qty", salesQtySum },
                        { "sales_amt", salesAmtSum }
                    });
                }
                reader.Close();
                connection.Close();
                var productWiseReport = new Dictionary<string, object>
                {
                    { "detail", companyList },
                    { "sales_qty", totalSalesQty.Sum() },
                    { "sales_amt", totalSalesAmt.Sum() }
                };
                return productWiseReport;
            }
            else if(repType == "monthly_sales_vs_collection")
            {
                var connection = new OracleConnection(_connection);
                connection.Open();
                List<float> opening = new List<float>();

                // Opening Balance Query
                string openingQuery = $@"
                    SELECT NVL(SUM(dr_amount) - SUM(cr_amount), 0) OPENING
                    FROM V$VIRTUAL_SUB_LEDGER
                    WHERE COMPANY_CODE IN ({companyCode})
                    AND deleted_flag = 'N'
                    AND form_code = '0'
                    AND SUBSTR(sub_code, 1, 1) = 'C'";

                using (var command = new OracleCommand(openingQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            opening.Add(reader.IsDBNull(0) ? 0 : reader.GetFloat(0));
                        }
                    }
                }
                // Monthly Sales vs Collection Query
                string salesVsCollectionQuery = $@"
                SELECT CASE 
                    WHEN mahina = 'Shrawan' THEN 'Bhadra'
                    WHEN mahina = 'Bhadra' THEN 'Ashoj'
                    WHEN mahina = 'Ashoj' THEN 'Kartik'
                    WHEN mahina = 'Kartik' THEN 'Mangsir'
                    WHEN mahina = 'Mangsir' THEN 'Poush'
                    WHEN mahina = 'Poush' THEN 'Magh'
                    WHEN mahina = 'Magh' THEN 'Falgun'
                    WHEN mahina = 'Falgun' THEN 'Chaitra'
                    WHEN mahina = 'Chaitra' THEN 'Baishakh'
                    WHEN mahina = 'Baishakh' THEN 'Jestha'
                    WHEN mahina = 'Jestha' THEN 'Ashadh'
                    ELSE 'undefined'
                END mahina,
                SUM(balance), startdate
                FROM 
                (
                    SELECT ACC_CODE, COMPANY_CODE, b.startdate, B.RANGENAME MAHINA,  
                    CASE 
                        WHEN (SELECT TRANSACTION_TYPE FROM FA_CHART_OF_ACCOUNTS_SETUP WHERE ACC_CODE = A.ACC_CODE AND COMPANY_CODE = A.COMPANY_CODE) = 'DR' 
                        THEN SUM(NVL(DR_AMOUNT, 0) - NVL(CR_AMOUNT, 0)) 
                        ELSE SUM(NVL(CR_AMOUNT, 0) - NVL(DR_AMOUNT, 0)) 
                    END BALANCE  
                    FROM V$VIRTUAL_GENERAL_LEDGER A, V_DATE_RANGE B  
                    WHERE VOUCHER_DATE <= ENDDATE  
                    AND SORTORDER = 1 
                    AND ENDDATE <= (SELECT ENDDATE FROM V_DATE_RANGE  
                                    WHERE ENDDATE > (SELECT MAX(VOUCHER_DATE) FROM V$VIRTUAL_GENERAL_LEDGER 
                                                     WHERE COMPANY_CODE IN ({companyCode})
                                                     AND TRUNC(VOUCHER_DATE) <= '{toDate}') 
                                    AND ROWNUM = 1)  
                    AND COMPANY_CODE IN ({companyCode})                
                    AND DELETED_FLAG = 'N'  
                    AND ACC_CODE IN (SELECT ACC_CODE FROM FA_CHART_OF_ACCOUNTS_SETUP 
                                     WHERE COMPANY_CODE IN ({companyCode}) 
                                     AND DELETED_FLAG = 'N' 
                                     AND acc_nature IN ('AE'))  
                    GROUP BY ACC_CODE, b.startdate, B.RANGENAME, COMPANY_CODE
                ) 
                GROUP BY mahina, startdate 
                ORDER BY 3";

                using (var command = new OracleCommand(salesVsCollectionQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        int x = 0;
                        while (reader.Read())
                        {
                            if (reader.IsDBNull(1)) continue;
                            opening.Add(reader.GetFloat(1)); // Adjust index as needed
                            x++;
                        }
                    }
                }
                // Final Sales and Collection Query
                string finalQuery = $@"
                    SELECT rangename, NVL(sales, 0), NVL(collection, 0) 
                    FROM 
                    (
                        SELECT * 
                        FROM 
                        (
                            SELECT RANGENAME  
                            FROM V_DATE_RANGE 
                            WHERE SORTORDER = 1 
                            AND startdate <= TRUNC(SYSDATE) 
                            ORDER BY STARTDATE
                        ) A
                        LEFT OUTER JOIN 
                        (
                            SELECT RANGENAME rs, VAL SALES  
                            FROM 
                            (
                                SELECT rangename, SUM(sales_qty - sales_ret_qty), SUM(sales_value - sales_ret_value) VAL 
                                FROM
                                (
                                    SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.SALES_DATE,
                                    SUM(NVL(A.ROLL_QTY, 0)) SALES_ROLL_QTY, 
                                    SUM(NVL(A.QUANTITY, 0)) SALES_QTY, 
                                    SUM(NVL(A.QUANTITY * A.NET_sales_RATE, 0)) SALES_VALUE,
                                    0 SALES_RET_ROLL_QTY, 
                                    0 SALES_RET_QTY, 
                                    0 SALES_RET_VALUE  
                                    FROM SA_SALES_INVOICE A 
                                    WHERE A.DELETED_FLAG = 'N'
                                    AND A.BRANCH_CODE IN ({branchCode})
                                    AND A.COMPANY_CODE IN ({companyCode}) {cusGrpStr}
                                    AND TRUNC(a.SALES_DATE) BETWEEN '{fromDate}' AND '{toDate}' {customerStr} {itemStringFilter}                             
                                    GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.SALES_DATE
                                    UNION ALL
                                    SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.RETURN_DATE, 
                                    0 SALES_ROLL_QTY, 
                                    0 SALES_QTY, 
                                    0 SALES_VALUE,
                                    SUM(NVL(A.ROLL_QTY, 0)) SALES_RET_ROLL_QTY, 
                                    SUM(NVL(A.QUANTITY, 0)) SALES_RET_QTY,
                                    SUM(NVL(A.QUANTITY * A.NET_sales_RATE, 0)) SALES_RET_VALUE  
                                    FROM SA_SALES_RETURN A
                                    WHERE A.DELETED_FLAG = 'N'
                                    AND A.COMPANY_CODE IN ({companyCode}) {cusGrpStr}
                                    AND A.BRANCH_CODE IN ({branchCode})
                                    AND TRUNC(A.RETURN_DATE) BETWEEN '{fromDate}' AND '{toDate}' {customerStr} {itemStringFilter}
                                    GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.RETURN_DATE 
                                ) a
                                LEFT OUTER JOIN 
                                (SELECT * FROM V_DATE_RANGE WHERE SORTORDER = '1') b
                                ON a.SALES_DATE BETWEEN b.STARTDATE AND b.ENDDATE    
                                WHERE RANGENAME IS NOT NULL 
                                GROUP BY STARTDATE, RANGENAME 
                            ) B 
                            ON A.RANGENAME = B.rs
                            LEFT OUTER JOIN 
                            (
                                SELECT RANGENAME ms, SUM(amt) collection 
                                FROM 
                                (
                                    SELECT * 
                                    FROM
                                    (
                                        SELECT voucher_date, NVL(SUM(NVL(CR_AMOUNT, 0) * NVL(EXCHANGE_RATE, 1)), 0) amt
                                        FROM V$VIRTUAL_SUB_LEDGER b
                                        WHERE (COMPANY_CODE, Voucher_NO) IN
                                        (
                                            SELECT COMPANY_CODE, A.voucher_no
                                            FROM V$VIRTUAL_GENERAL_LEDGER A
                                            WHERE A.ACC_CODE IN 
                                            (
                                                SELECT ACC_CODE 
                                                FROM FA_CHART_OF_ACCOUNTS_SETUP 
                                                WHERE ACC_NATURE IN ('AB', 'AC', 'LC') 
                                                AND COMPANY_CODE = A.COMPANY_CODE
                                            )
                                            AND A.TRANSACTION_TYPE = 'DR'                                        
                                            AND A.COMPANY_CODE IN ({companyCode})                                                                                                                     
                                            AND A.BRANCH_CODE IN ({branchCode})
                                            {subCodeStr} {subCodeCusStr}                                                                                                                   
                                            AND A.DELETED_FLAG = 'N'
                                            AND A.Voucher_NO != '0'
                                        )
                                        AND SUBSTR(sub_code, 1, 1) = 'C'
                                        AND TRANSACTION_TYPE = 'CR'                                                                                                                            
                                        GROUP BY voucher_date
                                    ) a
                                    LEFT OUTER JOIN 
                                    (SELECT * FROM V_DATE_RANGE WHERE SORTORDER = '1') b
                                    ON a.voucher_date BETWEEN b.STARTDATE AND b.ENDDATE
                                )  
                                WHERE RANGENAME IS NOT NULL 
                                GROUP BY STARTDATE, RANGENAME
                            ) C
                            ON A.RANGENAME = C.ms
                        )
                    )";

                List<Dictionary<string, object>> reportDetails = new List<Dictionary<string, object>>();
                using (var command = new OracleCommand(finalQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        int x = 0;
                        while (reader.Read())
                        {
                            reportDetails.Add(new Dictionary<string, object>
                    {
                        { "title", reader.GetString(0) },
                        { "sales", reader.IsDBNull(1) ? 0 : reader.GetFloat(1) },
                        { "collection", reader.IsDBNull(2) ? 0 : reader.GetFloat(2) },
                        { "abbr", reader.GetString(0).Substring(0, 4) },
                        { "opening", opening[x] }
                    });
                            x++;
                        }
                    }
                }
                // Calculate totals
                float totalSales = 0f;
                float totalCollection = 0f;
                foreach (var record in reportDetails)
                {
                    totalSales += Convert.ToSingle(record["sales"]);
                    totalCollection += Convert.ToSingle(record["collection"]);
                }
                return new Dictionary<string, object>
                {
                    { "detail", reportDetails },
                    { "total", new Dictionary<string, object> { { "sales", totalSales }, { "collection", totalCollection }, { "abbr", "Bak" } } }
                };
            }
            else if(repType == "weekly_sales_vs_collection")
            {
                string QRY = @"
                    select rangename, nvl(sales, 0), nvl(collection, 0), sortorder from
                    (
                        SELECT * FROM 
                        (
                            select trunc(sysdate - n) STARTDATE, trunc(sysdate - n) ENDDATE,
                                substr(TO_CHAR(sysdate - n, 'DAY'), 1, 3) || '-' || EXTRACT(MONTH FROM TO_DATE(sysdate - n, 'DD-Mon-YYYY')) || '/'
                                || EXTRACT(day FROM TO_DATE(sysdate - n, 'DD-Mon-YYYY')) RANGENAME,
                                rownum SORTORDER 
                            from (select rownum n from dual connect by level <= 7) 
                            where n >= 1
                        ) A
                        LEFT OUTER JOIN 
                        (
                            select RANGENAME rs, VAL SALES from 
                            (
                                select rangename, sum(sales_qty - sales_ret_qty), sum(sales_value - sales_ret_value) VAL 
                                from 
                                (
                                    select * from 
                                    (
                                        SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.sales_date,
                                            SUM(NVL(A.ROLL_QTY, 0)) SALES_ROLL_QTY, SUM(NVL(A.QUANTITY, 0)) SALES_QTY, SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) SALES_VALUE,
                                            0 SALES_RET_ROLL_QTY, 0 SALES_RET_QTY, 0 SALES_RET_VALUE  
                                        FROM SA_SALES_INVOICE A 
                                        WHERE A.DELETED_FLAG = 'N'
                                        GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.sales_date
                                        Union All
                                        SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.return_date,
                                            0 SALES_ROLL_QTY, 0 SALES_QTY, 0 SALES_VALUE,
                                            SUM(NVL(A.ROLL_QTY, 0)) SALES_RET_ROLL_QTY, SUM(NVL(A.QUANTITY, 0)) SALES_RET_QTY,
                                            SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) SALES_RET_VALUE  
                                        FROM SA_SALES_RETURN A 
                                        WHERE A.DELETED_FLAG = 'N'
                                        GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.return_date 
                                    ) a
                                    LEFT OUTER JOIN 
                                    (
                                        select trunc(sysdate - n) STARTDATE, trunc(sysdate - n) ENDDATE,
                                            substr(TO_CHAR(sysdate - n, 'DAY'), 1, 3) || '-' || EXTRACT(MONTH FROM TO_DATE(sysdate - n, 'DD-Mon-YYYY')) || '/'
                                            || EXTRACT(day FROM TO_DATE(sysdate - n, 'DD-Mon-YYYY')) RANGENAME,
                                            rownum SORTORDER 
                                        from (select rownum n from dual connect by level <= 7) 
                                        where n >= 1
                                    ) b
                                    on a.sales_date between b.startdate and b.enddate
                                )
                                WHERE RANGENAME IS NOT NULL 
                                group by startdate, rangename
                            )
                        ) B 
                        ON A.RANGENAME = B.rs
                        LEFT OUTER JOIN 
                        (
                            select rangename ms, sum(amt) collection 
                            from 
                            (
                                select * from 
                                (
                                    select voucher_date, CR_AMOUNT amt from V$FA_SUB_LEDGER 
                                    where voucher_no IN
                                    (
                                        select voucher_no from V$VIRTUAL_GENERAL_LEDGER 
                                        where company_CODE = '01'
                                        and ACC_CODE IN (SELECT ACC_CODE FROM FA_CHART_OF_ACCOUNTS_SETUP WHERE ACC_NATURE IN ('AB', 'AC') AND company_code = '01')
                                        AND deleted_flag = 'N'
                                        AND voucher_no != '0'
                                        AND TRANSACTION_TYPE = 'DR'
                                    ) 
                                    AND TRANSACTION_TYPE = 'CR'
                                    and company_code = '01'
                                ) a
                                LEFT OUTER JOIN 
                                (
                                    select trunc(sysdate - n) STARTDATE, trunc(sysdate - n) ENDDATE,
                                        substr(TO_CHAR(sysdate - n, 'DAY'), 1, 3) || '-' || EXTRACT(MONTH FROM TO_DATE(sysdate - n, 'DD-Mon-YYYY')) || '/'
                                        || EXTRACT(day FROM TO_DATE(sysdate - n, 'DD-Mon-YYYY')) RANGENAME,
                                        rownum SORTORDER 
                                    from (select rownum n from dual connect by level <= 7) 
                                    where n >= 1
                                ) b
                                on a.voucher_date between b.startdate and b.enddate
                            )
                            where rangename is not null 
                            group by startdate, rangename
                        ) C 
                        ON A.RANGENAME = C.ms
                    ) 
                    order by sortorder";
                var connection = new OracleConnection(_connection);
                connection.Open();
                using (OracleCommand command = new OracleCommand(QRY, connection))
                {
                    OracleDataReader reader = command.ExecuteReader();
                    List<Dictionary<string, object>> asd = new List<Dictionary<string, object>>();

                    while (reader.Read())
                    {
                        asd.Add(new Dictionary<string, object>
                {
                    { "title", reader.GetString(0) },
                    { "sales", Convert.ToDouble(reader.GetDecimal(1)) },
                    { "collection", Convert.ToDouble(reader.GetDecimal(2)) },
                    { "abbr", reader.GetString(0).Substring(0, 4) }
                });
                    }

                    double totalSales = 0.0;
                    double totalCollection = 0.0;

                    foreach (var record in asd)
                    {
                        totalSales += Convert.ToDouble(record["sales"]);
                        totalCollection += Convert.ToDouble(record["collection"]);
                    }

                    return new Dictionary<string, object>
                    {
                        { "detail", asd },
                        { "total", new Dictionary<string, object>
                            {
                                { "sales", totalSales },
                                { "collection", totalCollection },
                                { "abbr", "Bak" }
                            }
                        }
                    };
                }
            }
            else if(repType == "product_sales_vs_collection")
            {
                var detail = new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object> { { "title", "Baisakh" }, { "sales", 1738971.0 }, { "collection", 263532.0 }, { "abbr", "Bak" } },
                    new Dictionary<string, object> { { "title", "Jestha" }, { "sales", 1738971.0 }, { "collection", 263532.0 }, { "abbr", "Bak" } },
                    new Dictionary<string, object> { { "title", "Ashad" }, { "sales", 1738971.0 }, { "collection", 263532.0 }, { "abbr", "Bak" } },
                    new Dictionary<string, object> { { "title", "Shrawan" }, { "sales", 1738971.0 }, { "collection", 263532.0 }, { "abbr", "Bak" } },
                    new Dictionary<string, object> { { "title", "Bhadra" }, { "sales", 1738971.0 }, { "collection", 263532.0 }, { "abbr", "Bak" } },
                    new Dictionary<string, object> { { "title", "Ashoj" }, { "sales", 1738971.0 }, { "collection", 263532.0 }, { "abbr", "Bak" } },
                    new Dictionary<string, object> { { "title", "Kartik" }, { "sales", 1738971.0 }, { "collection", 263532.0 }, { "abbr", "Bak" } }
                };
                var total = new Dictionary<string, object>
                {
                    { "sales", 1738971.0 },
                    { "collection", 126378.0 },
                    { "abbr", "Bak" }
                };
                var result = new Dictionary<string, object>
                {
                    { "detail", detail },
                    { "total", total }
                };
                return result;
            }
            else if(repType == "customer_sales_vs_collection")
            {
                var qry = $@"
                SELECT CUSTOMER_EDESC, NVL(NET_SALES_VALUE, 0), NVL(COLLECTION, 0), NVL(OPENING, 0) FROM 
                (
                    SELECT * FROM 
                    (
                        SELECT A.CUSTOMER_CODE, A.CUSTOMER_EDESC FROM SA_CUSTOMER_SETUP A 
                        WHERE A.COMPANY_CODE IN {companyCode} {customerStr}
                        GROUP BY A.CUSTOMER_CODE, A.CUSTOMER_EDESC
                    ) A
                    LEFT OUTER JOIN 
                    (
                        SELECT COD, NET_SALES_VALUE FROM 
                        (
                            SELECT cus_name, Cod, SALES_ROLL_QTY, SALES_QTY, SALES_VALUE, SALES_RET_ROLL_QTY,
                                SALES_RET_QTY, SALES_RET_VALUE, SALES_ROLL_QTY - SALES_RET_ROLL_QTY NET_ROLL_QTY,
                                SALES_QTY - SALES_RET_QTY NET_SALES_QTY, SALES_VALUE - SALES_RET_VALUE NET_SALES_VALUE
                            FROM 
                            ( 
                                -- Add the nested query here for sales data
                            ) A
                            LEFT OUTER JOIN 
                            (
                                SELECT customer_code cod, customer_edesc cus_name FROM SA_CUSTOMER_SETUP 
                                WHERE COMPANY_CODE IN {companyCode}
                            ) D
                            ON D.cod = A.CUSTOMER_CODE
                            GROUP BY D.cod, D.cus_name
                        )
                    ) B ON A.customer_code = B.cod
                    LEFT OUTER JOIN 
                    (
                        SELECT sub_code, NVL(SUM(NVL(CR_AMOUNT, 0) * NVL(EXCHANGE_RATE, 1)), 0) amt
                        FROM V$VIRTUAL_SUB_LEDGER b
                        WHERE (COMPANY_CODE, Voucher_NO) IN 
                        (
                            -- Add subledger query here
                        )
                    ) C ON 'C' || A.customer_code = C.SUB_CODE
                    LEFT OUTER JOIN 
                    (
                        SELECT sub_code D_CODE, NVL(SUM(dr_amount) - SUM(cr_amount), 0) OPENING
                        FROM V$VIRTUAL_SUB_LEDGER
                        WHERE company_code IN '{companyCode}' 
                        AND deleted_flag = 'N' 
                        AND (form_code = '0' OR voucher_date < '{fromDate}')
                        GROUP BY sub_code
                    ) D ON 'C' || A.customer_code = D.D_CODE
                )
                WHERE NVL(NET_SALES_VALUE, 0) || NVL(COLLECTION, 0) != '00' 
                ORDER BY CUSTOMER_EDESC";

                var salesRecords = new List<SalesRecord>();
                double totalSales = 0.0;
                double totalCollection = 0.0;
                using (OracleConnection connection = new OracleConnection(_connection))
                {
                    connection.OpenAsync();
                    using (OracleCommand command = new OracleCommand(qry, connection)) 
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var salesRecord = new SalesRecord
                                {
                                    Title = reader.GetString(0),   
                                    Sales = reader.GetDouble(1),   
                                    Collection = reader.GetDouble(2), 
                                    Abbr = reader.GetString(0).Substring(0, 4)  
                                };

                                salesRecords.Add(salesRecord);
                                totalSales += salesRecord.Sales;
                                totalCollection += salesRecord.Collection;
                            }
                        }
                    }
                }
                var response = new Dictionary<string, object>
                {
                    { "Detail", salesRecords },
                    { "Total", new Total 
                        {
                            Sales = totalSales,
                            Collection = totalCollection,
                            Abbr = "Bak"
                        }
                    }
                };

                return response;
            }
            else if(repType == "grp_customer_sales_vs_collection")
            {
                var vals = new List<SalesRecord>();
                var arrTotal1 = new List<double>();
                var arrTotal2 = new List<double>();
                var arrTotal3 = new List<double>();
                string query = $@"
                    SELECT CUSTOMER_EDESC, master_customer_code, pre_customer_code, group_sku_flag, 
                           NVL(NET_SALES_VALUE, 0), NVL(COLLECTION, 0), NVL(OPENING, 0) 
                    FROM 
                    (
                        SELECT * FROM 
                        (
                            SELECT A.CUSTOMER_CODE, A.CUSTOMER_EDESC, A.master_customer_code, 
                                   A.pre_customer_code, A.group_sku_flag 
                            FROM SA_CUSTOMER_SETUP A 
                            WHERE A.COMPANY_CODE IN ({companyCode}) 
                            GROUP BY A.CUSTOMER_CODE, A.CUSTOMER_EDESC, A.master_customer_code, 
                                     A.pre_customer_code, A.group_sku_flag
                        ) A
                        LEFT OUTER JOIN 
                        (
                            SELECT COD, NET_SALES_VALUE 
                            FROM 
                            (
                                SELECT cus_name, Cod, SALES_ROLL_QTY, SALES_QTY, SALES_VALUE, 
                                       SALES_RET_ROLL_QTY, SALES_RET_QTY, SALES_RET_VALUE, 
                                       SALES_ROLL_QTY - SALES_RET_ROLL_QTY NET_ROLL_QTY,
                                       SALES_QTY - SALES_RET_QTY NET_SALES_QTY, 
                                       SALES_VALUE - SALES_RET_VALUE NET_SALES_VALUE
                                FROM 
                                (
                                    SELECT D.CUS_name, D.cod, 
                                           SUM(NVL(A.SALES_ROLL_QTY, 0)) SALES_ROLL_QTY, 
                                           SUM(NVL(A.SALES_QTY, 0)) SALES_QTY,
                                           SUM(NVL(A.SALES_VALUE, 0)) SALES_VALUE, 
                                           SUM(SALES_RET_ROLL_QTY) SALES_RET_ROLL_QTY, 
                                           SUM(A.SALES_RET_QTY) SALES_RET_QTY,
                                           SUM(NVL(A.SALES_RET_VALUE, 0)) SALES_RET_VALUE 
                                    FROM
                                    (
                                        SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.customer_code,
                                               SUM(NVL(A.ROLL_QTY, 0)) SALES_ROLL_QTY, 
                                               SUM(NVL(A.QUANTITY, 0)) SALES_QTY, 
                                               SUM(NVL(A.QUANTITY * A.NET_sales_RATE, 0)) SALES_VALUE,
                                               0 SALES_RET_ROLL_QTY, 0 SALES_RET_QTY, 0 SALES_RET_VALUE  
                                        FROM SA_SALES_INVOICE A 
                                        WHERE A.DELETED_FLAG = 'N' 
                                        AND A.COMPANY_CODE IN ({companyCode})
                                        {customerStr} {cusGrpStr}
                                        AND A.BRANCH_CODE IN ({GetBranchFilter(companyCode, branchCode, userNo)})
                                        AND TRUNC(a.SALES_DATE) BETWEEN '{fromDate}' AND '{toDate}'
                                        GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.customer_CODE
                                        UNION ALL
                                        SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.customer_code, 0 SALES_ROLL_QTY, 
                                               0 SALES_QTY, 0 SALES_VALUE, 
                                               SUM(NVL(A.ROLL_QTY, 0)) SALES_RET_ROLL_QTY, 
                                               SUM(NVL(A.QUANTITY, 0)) SALES_RET_QTY,
                                               SUM(NVL(A.QUANTITY * A.NET_sales_RATE, 0)) SALES_RET_VALUE
                                        FROM SA_SALES_RETURN A
                                        WHERE A.DELETED_FLAG = 'N'
                                        AND A.COMPANY_CODE IN ({companyCode})
                                        AND A.BRANCH_CODE IN ({GetBranchFilter(companyCode, branchCode, userNo)}) 
                                        {customerStr} {cusGrpStr}
                                        AND TRUNC(A.RETURN_DATE) BETWEEN '{fromDate}' AND '{toDate}' 
                                        GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.customer_CODE 
                                        ORDER BY 1                                                 
                                    ) A                          
                                    LEFT OUTER JOIN 
                                    (
                                        SELECT customer_code cod, customer_edesc cus_name 
                                        FROM SA_CUSTOMER_SETUP 
                                        WHERE 1=1 AND COMPANY_CODE IN ({companyCode}) 
                                        GROUP BY customer_code, customer_edesc
                                    ) D ON D.cod = A.CUSTOMER_CODE                                                 
                                    GROUP BY D.cod, D.cus_name                                                                            
                                ) 
                                ORDER BY Cus_name
                            ) B ON A.customer_code = b.cod
                        )
                        LEFT OUTER JOIN 
                        (
                            SELECT sub_code, NVL(SUM(NVL(CR_AMOUNT, 0) * NVL(EXCHANGE_RATE, 1)), 0) collection
                            FROM V$VIRTUAL_SUB_LEDGER b
                            WHERE (COMPANY_CODE, Voucher_NO) IN
                            (
                                SELECT COMPANY_CODE, A.voucher_no
                                FROM V$VIRTUAL_GENERAL_LEDGER A
                                WHERE A.ACC_CODE IN 
                                (
                                    SELECT ACC_CODE 
                                    FROM FA_CHART_OF_ACCOUNTS_SETUP 
                                    WHERE ACC_NATURE IN ('AB', 'AC', 'LC') 
                                    AND COMPANY_CODE = A.COMPANY_CODE
                                )
                                AND A.TRANSACTION_TYPE = 'DR'   
                                AND COMPANY_CODE IN ({companyCode})
                                AND BRANCH_CODE IN ({GetBranchFilter(companyCode, branchCode, userNo)})                                                    

                                AND TRUNC(voucher_DATE) BETWEEN '{fromDate}' AND '{toDate}'                                                                                                                                                                                                                                                                                                                               
                                AND A.DELETED_FLAG = 'N'                                                    

                                AND A.Voucher_NO != '0'                                                       
                            )
                            AND SUBSTR(sub_code, 1, 1) = 'C'
                            {subCodeCusStr} {subCodeStr}
                            AND TRANSACTION_TYPE = 'CR'                                                                                                                           
                            GROUP BY sub_code                                                            
                        ) c ON 'C' || A.customer_code = C.SUB_CODE
                        LEFT OUTER JOIN
                        (
                            SELECT sub_code D_CODE, NVL(SUM(dr_amount) - SUM(cr_amount), 0) OPENING
                            FROM V$VIRTUAL_SUB_LEDGEr
                            WHERE company_code IN ({companyCode})
                            AND deleted_flag = 'N'                                                 
                            AND (form_code = '0' OR voucher_date < '{fromDate}')
                            AND substr(sub_code, 1, 1) = 'C'
                             {subCodeCusStr} {subCodeStr}
                            GROUP BY sub_code
                        ) D ON 'C' || A.customer_code = D.D_CODE
                    ) 
                    ORDER BY 2, 1";

                using (OracleConnection connection = new OracleConnection(_connection))
                {
                     connection.OpenAsync(); 
                     using (var command = new OracleCommand(query, connection))
                    {
                        using (var reader =  command.ExecuteReader()) 
                        {
                            while ( reader.Read()) 
                            {
                                var result = ProcessCodes(reader);
                                if (result != null)
                                {
                                    vals.Add(result);
                                    if (reader.GetString(3) == "I") 
                                    {
                                        arrTotal1.Add(reader.GetDouble(4)); 
                                        arrTotal2.Add(reader.GetDouble(5)); 
                                        arrTotal3.Add(reader.GetDouble(6)); 
                                    }
                                }
                            }
                        }
                    }
                }
                var dd = new Dictionary<string, object>
                {
                    { "Detail", vals },
                    { "Total1", arrTotal1.Sum() },
                    { "Total2", arrTotal2.Sum() },
                    { "Total3", arrTotal3.Sum() }
                };
                return dd;
            }
            else if(repType == "brand_wise_sales")
            {
                string query = $@"
                    SELECT NVL(brand_name, 'undefined'), '', SALES_ROLL_QTY, SALES_QTY, SALES_VALUE, 
                           SALES_RET_ROLL_QTY, SALES_RET_QTY, SALES_RET_VALUE, 
                           SALES_ROLL_QTY - SALES_RET_ROLL_QTY AS NET_ROLL_QTY, 
                           SALES_QTY - SALES_RET_QTY AS NET_SALES_QTY, 
                           SALES_VALUE - SALES_RET_VALUE AS NET_SALES_VALUE 
                    FROM (
                        SELECT e.brand_name, 
                               SUM(A.SALES_ROLL_QTY) AS SALES_ROLL_QTY, 
                               SUM(A.SALES_QTY) AS SALES_QTY, 
                               SUM(A.SALES_VALUE) AS SALES_VALUE, 
                               SUM(SALES_RET_ROLL_QTY) AS SALES_RET_ROLL_QTY, 
                               SUM(A.SALES_RET_QTY) AS SALES_RET_QTY, 
                               SUM(A.SALES_RET_VALUE) AS SALES_RET_VALUE 
                        FROM (
                            SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.ITEM_CODE, 
                                   SUM(NVL(A.ROLL_QTY, 0)) AS SALES_ROLL_QTY, 
                                   SUM(NVL(A.QUANTITY, 0)) AS SALES_QTY, 
                                   SUM(NVL(A.QUANTITY * A.{grossNetStr}, 0)) AS SALES_VALUE,
                                   0 AS SALES_RET_ROLL_QTY, 
                                   0 AS SALES_RET_QTY, 
                                   0 AS SALES_RET_VALUE  
                            FROM SA_SALES_INVOICE A 
                            WHERE A.DELETED_FLAG = 'N' 
                                  {customerStr} 
                                  AND A.BRANCH_CODE IN {GetBranchFilter(companyCode, branchCode, userNo)} 
                                  {itemStringFilter} 
                                  AND A.COMPANY_CODE IN {companyCode} 
                                  AND TRUNC(a.SALES_DATE) BETWEEN '{fromDate}' AND '{toDate}' 
                            GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.ITEM_CODE 
                            UNION ALL 
                            SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.ITEM_CODE, 
                                   0 AS SALES_ROLL_QTY, 
                                   0 AS SALES_QTY, 
                                   0 AS SALES_VALUE, 
                                   SUM(NVL(A.ROLL_QTY, 0)) AS SALES_RET_ROLL_QTY, 
                                   SUM(NVL(A.QUANTITY, 0)) AS SALES_RET_QTY,  
                                   SUM(NVL(A.QUANTITY * A.{grossNetStr}, 0)) AS SALES_RET_VALUE 
                            FROM SA_SALES_RETURN A 
                            WHERE A.DELETED_FLAG = 'N' 
                                  AND A.COMPANY_CODE IN {companyCode} 
                                  AND A.BRANCH_CODE IN {GetBranchFilter(companyCode, branchCode, userNo)} 
                                  {itemStringFilter} 
                                  {customerStr} 
                                  AND TRUNC(A.RETURN_DATE) BETWEEN '{fromDate}' AND '{toDate}' 
                            GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.ITEM_CODE 
                            ORDER BY 1
                        ) A 
                        JOIN IP_ITEM_MASTER_SETUP D 
                            ON A.COMPANY_CODE = D.COMPANY_CODE 
                            AND A.ITEM_CODE = D.ITEM_CODE 
                        LEFT OUTER JOIN (
                            SELECT item_code AS cod, brand_name 
                            FROM IP_ITEM_SPEC_SETUP 
                            WHERE COMPANY_CODE IN {companyCode}
                        ) e 
                            ON D.ITEM_CODE = e.cod 
                        GROUP BY e.brand_name
                    ) 
                    ORDER BY brand_name";
                var productData = new List<ProductData>();
                double totalSalesQty = 0;
                double totalSalesAmt = 0;
                double totalSalesRetQty = 0;
                double totalSalesRetAmt = 0;
                double totalNetSalesQty = 0;
                double totalNetSalesAmt = 0;
                using (var connection = new OracleConnection(_connection))
                {
                    connection.OpenAsync();
                    using (var command = new OracleCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var item = reader.GetString(0);
                                var unit = reader.GetString(1);
                                var salesQty = reader.GetDouble(3);
                                var salesAmt = reader.GetDouble(4);
                                var salesRetQty = reader.GetDouble(6);
                                var salesRetAmt = reader.GetDouble(7);
                                var netSalesQty = reader.GetDouble(9);
                                var netSalesAmt = reader.GetDouble(10);

                                productData.Add(new ProductData
                                {
                                    Item = item,
                                    Unit = unit,
                                    SalesQty = salesQty,
                                    SalesAmt = salesAmt,
                                    SalesRetQty = salesRetQty,
                                    SalesRetAmt = salesRetAmt,
                                    NetSalesQty = netSalesQty,
                                    NetSalesAmt = netSalesAmt
                                });

                                totalSalesQty += salesQty;
                                totalSalesAmt += salesAmt;
                                totalSalesRetQty += salesRetQty;
                                totalSalesRetAmt += salesRetAmt;
                                totalNetSalesQty += netSalesQty;
                                totalNetSalesAmt += netSalesAmt;
                            }
                        }
                    }
                }
                var report = new Dictionary<string, object>
                    {
                        { "ProductWiseData", productData },
                        { "Branches", GetBranchFilter(branchCode, userNo, companyCode) },
                        { "TotalSalesQty", totalSalesQty },
                        { "TotalSalesAmt", totalSalesAmt },
                        { "TotalSalesRetQty", totalSalesRetQty },
                        { "TotalSalesRetAmt", totalSalesRetAmt },
                        { "TotalNetSalesQty", totalNetSalesQty },
                        { "TotalNetSalesAmt", totalNetSalesAmt }
                    };
                return report;
            }
            else if(repType == "monthly_wise_sales")
            {
                string query = $@"
                    SELECT a.*, 
                           CASE 
                               WHEN a.rangename = 'Ashoj' THEN SUBSTR(a.rangename, 1, 4) 
                               ELSE SUBSTR(a.rangename, 1, 3) 
                           END aa 
                    FROM (
                        SELECT startdate, 
                               rangename, 
                               SUM(sales_qty - sales_ret_qty) AS qty, 
                               SUM(sales_value - sales_ret_value) AS amt 
                        FROM (
                            SELECT * FROM (
                                SELECT A.COMPANY_CODE, 
                                       A.BRANCH_CODE, 
                                       A.sales_date,
                                       SUM(NVL(A.ROLL_QTY, 0)) AS SALES_ROLL_QTY, 
                                       SUM(NVL(A.QUANTITY, 0)) AS SALES_QTY, 
                                       SUM(NVL(A.QUANTITY * A.{grossNetStr}, 0)) AS SALES_VALUE,
                                       0 AS SALES_RET_ROLL_QTY, 
                                       0 AS SALES_RET_QTY, 
                                       0 AS SALES_RET_VALUE 
                                FROM SA_SALES_INVOICE A 
                                WHERE A.DELETED_FLAG = 'N' 
                                      AND A.BRANCH_CODE IN {GetBranchFilter(companyCode, branchCode, userNo)} 
                                      AND A.COMPANY_CODE IN {companyCode} 
                                      AND TRUNC(a.SALES_DATE) BETWEEN '{fromDate}' AND '{toDate}' 
                                      {customerStr}  
                                      {itemStringFilter}                             
                                GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.sales_date 
                                UNION ALL 
                                SELECT A.COMPANY_CODE, 
                                       A.BRANCH_CODE, 
                                       A.return_date, 
                                       0 AS SALES_ROLL_QTY, 
                                       0 AS SALES_QTY, 
                                       0 AS SALES_VALUE, 
                                       SUM(NVL(A.ROLL_QTY, 0)) AS SALES_RET_ROLL_QTY, 
                                       SUM(NVL(A.QUANTITY, 0)) AS SALES_RET_QTY,
                                       SUM(NVL(A.QUANTITY * A.{grossNetStr}, 0)) AS SALES_RET_VALUE  
                                FROM SA_SALES_RETURN A 
                                WHERE A.DELETED_FLAG = 'N' 
                                      AND A.COMPANY_CODE IN {companyCode} 
                                      AND A.BRANCH_CODE IN {GetBranchFilter(companyCode, branchCode, userNo)} 
                                      AND TRUNC(A.RETURN_DATE) BETWEEN '{fromDate}' AND '{toDate}' 
                                      {customerStr} 
                                      {itemStringFilter} 
                                GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.return_date 
                                ORDER BY sales_date 
                            ) a 
                            LEFT OUTER JOIN (
                                SELECT * 
                                FROM V_DATE_RANGE 
                                WHERE SORTORDER = '1'
                            ) b ON a.sales_date BETWEEN b.startdate AND b.enddate
                        ) 
                        GROUP BY startdate, rangename 
                    ) a 
                    ORDER BY startdate";

                var vals = new List<MonthlySalesData>();
                double totalAmount = 0;
                double totalQuantity = 0;

                using (OracleConnection connection = new OracleConnection(_connection))
                {
                     connection.OpenAsync();

                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        using (var reader =  command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var qty = reader.GetDouble(2);
                                var amt = reader.GetDouble(3);
                                var month = reader.GetString(1);
                                var abbr = reader.GetString(4);

                                vals.Add(new MonthlySalesData
                                {
                                    Month = month,
                                    Qty = qty,
                                    Amt = amt,
                                    Abbr = abbr
                                });

                                totalAmount += amt;
                                totalQuantity += qty;
                            }
                        }
                    }
                }
                var total = new Dictionary<string, object>
                    {
                        { "Month", "This Year" },
                        { "Qty", totalQuantity },
                        { "Amt", totalAmount },
                        { "Details", vals },
                        { "TotalAmt", totalAmount },
                        { "TotalQty", totalQuantity }
                    };

                return total; 
            }
            else if(repType == "monthly_wise_sales_eng")
            {
                string query = $@"
                    SELECT a.*, 
                           CASE 
                               WHEN rangename = 'JULY-22' THEN LOWER(SUBSTR(rangename, 1, 4)) || LOWER(SUBSTR(rangename, -2)) 
                               ELSE UPPER(SUBSTR(rangename, 1, 3)) || '-' || LOWER(SUBSTR(rangename, -2)) 
                           END aa 
                    FROM (
                        SELECT startdate, 
                               rangename, 
                               SUM(sales_qty - sales_ret_qty) AS qty, 
                               SUM(sales_value - sales_ret_value) AS amt 
                        FROM (
                            SELECT * FROM (
                                SELECT A.COMPANY_CODE, 
                                       A.BRANCH_CODE, 
                                       A.sales_date,
                                       SUM(NVL(A.ROLL_QTY, 0)) AS SALES_ROLL_QTY, 
                                       SUM(NVL(A.QUANTITY, 0)) AS SALES_QTY, 
                                       SUM(NVL(A.QUANTITY * A.{grossNetStr}, 0)) AS SALES_VALUE,
                                       0 AS SALES_RET_ROLL_QTY, 
                                       0 AS SALES_RET_QTY, 
                                       0 AS SALES_RET_VALUE 
                                FROM SA_SALES_INVOICE A 
                                WHERE A.DELETED_FLAG = 'N' 
                                      AND A.BRANCH_CODE IN {GetBranchFilter(companyCode, branchCode, userNo)} 
                                      AND A.COMPANY_CODE IN {companyCode} 
                                      AND TRUNC(a.SALES_DATE) BETWEEN '{fromDate}' AND '{toDate}' 
                                      {customerStr}  
                                      {itemStringFilter}                             
                                GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.sales_date 
                                UNION ALL 
                                SELECT A.COMPANY_CODE, 
                                       A.BRANCH_CODE, 
                                       A.return_date, 
                                       0 AS SALES_ROLL_QTY, 
                                       0 AS SALES_QTY, 
                                       0 AS SALES_VALUE, 
                                       SUM(NVL(A.ROLL_QTY, 0)) AS SALES_RET_ROLL_QTY, 
                                       SUM(NVL(A.QUANTITY, 0)) AS SALES_RET_QTY,
                                       SUM(NVL(A.QUANTITY * A.{grossNetStr}, 0)) AS SALES_RET_VALUE  
                                FROM SA_SALES_RETURN A 
                                WHERE A.DELETED_FLAG = 'N' 
                                      AND A.COMPANY_CODE IN {companyCode} 
                                      AND A.BRANCH_CODE IN {GetBranchFilter(companyCode, branchCode, userNo)} 
                                      AND TRUNC(A.RETURN_DATE) BETWEEN '{fromDate}' AND '{toDate}' 
                                      {customerStr} 
                                      {itemStringFilter} 
                                GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.return_date 
                                ORDER BY sales_date 
                            ) a 
                            LEFT OUTER JOIN (
                                SELECT * 
                                FROM V_DATE_RANGE_ENG
                            ) b ON a.sales_date BETWEEN b.startdate AND b.enddate
                        ) 
                        GROUP BY startdate, rangename 
                        ORDER BY startdate
                    ) a 
                    ORDER BY startdate";
                var vals = new List<MonthlySalesData>();
                double totalAmount = 0;
                double totalQuantity = 0;
                using (var connection = new OracleConnection(_connection))
                {
                    connection.OpenAsync(); 

                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        using (OracleDataReader reader= command.ExecuteReader()) 
                        {
                            while (reader.Read()) 
                            {
                                var qty = reader.GetDouble(2);
                                var amt = reader.GetDouble(3);
                                var month = reader.GetString(1);
                                var abbr = reader.GetString(4);
                                // Add the monthly sales data to the list
                                vals.Add(new MonthlySalesData
                                {
                                    Month = month,
                                    Qty = qty,
                                    Amt = amt,
                                    Abbr = abbr
                                });

                                totalAmount += amt;
                                totalQuantity += qty;
                            }
                        }
                    }
                }
                var total = new MonthlyWiseSalesReport
                {
                    Month = "This Year",
                    Qty = totalQuantity,
                    Amt = totalAmount,
                    Details = vals,
                    TotalAmt = totalAmount,
                    TotalQty = totalQuantity
                };

                var result = new Dictionary<string, object>
                {
                    { "total_qty", totalQuantity },
                    { "total_amt", totalAmount },
                    { "details", vals } 
                };

                return result;
            }
            //else if(repType == "graph_wise_sales")
            //{
            //    var graph = new Dictionary<string, object>();
            //    graph["branch_wise_sales"] = GetBranchWiseSales(companyCode, branchCode, fromDate, toDate, customerStr, itemStringFilter, grossNetStr);
            //    graph["com_wise_sales"] = GetCompanyWiseSales(companyCode, fromDate, toDate, customerStr, grossNetStr);
            //    graph["all_companies"] = GetAllCompanies(fromDate, toDate, customerStr);
            //    return graph;
            //}
            else if(repType == "graph_wise_sales")
            {
                var graph = new Dictionary<string, object>();

                string query = $@"
                SELECT branch_edesc, qty, amt, abbr FROM 
                (
                    SELECT branch_code, SUM(sales_qty - sales_ret_qty) AS qty, SUM(sales_value - sales_ret_value) AS amt FROM 
                    (
                        SELECT A.COMPANY_CODE, A.BRANCH_CODE,
                        SUM(NVL(A.ROLL_QTY, 0)) AS SALES_ROLL_QTY, SUM(NVL(A.QUANTITY, 0)) AS SALES_QTY, SUM(NVL(A.QUANTITY * A.{grossNetStr}, 0)) AS SALES_VALUE,
                        0 AS SALES_RET_ROLL_QTY, 0 AS SALES_RET_QTY, 0 AS SALES_RET_VALUE 
                        FROM SA_SALES_INVOICE A 
                        WHERE A.DELETED_FLAG = 'N'  
                        AND A.BRANCH_CODE IN {GetBranchFilter(companyCode, branchCode, userNo)}
                        AND A.COMPANY_CODE IN {companyCode}
                        AND TRUNC(a.SALES_DATE) BETWEEN '{fromDate}' AND '{toDate}' {customerStr} {itemStringFilter}                             
                        GROUP BY A.COMPANY_CODE, A.BRANCH_CODE

                        UNION ALL

                        SELECT A.COMPANY_CODE, A.BRANCH_CODE, 0 AS SALES_ROLL_QTY, 0 AS SALES_QTY, 0 AS SALES_VALUE,
                        SUM(NVL(A.ROLL_QTY, 0)) AS SALES_RET_ROLL_QTY, SUM(NVL(A.QUANTITY, 0)) AS SALES_RET_QTY,
                        SUM(NVL(A.QUANTITY * A.{grossNetStr}, 0)) AS SALES_RET_VALUE  
                        FROM SA_SALES_RETURN A
                        WHERE A.DELETED_FLAG = 'N'             
                        AND A.BRANCH_CODE IN {GetBranchFilter(companyCode, branchCode, userNo)}
                        AND A.COMPANY_CODE IN {companyCode}
                        AND TRUNC(a.return_DATE) BETWEEN '{fromDate}' AND '{toDate}' {customerStr} {itemStringFilter}                             
                        GROUP BY A.COMPANY_CODE, A.BRANCH_CODE
                    ) 
                    GROUP BY branch_code 
                    ORDER BY branch_code
                ) a
                LEFT OUTER JOIN 
                (
                    SELECT branch_code AS code, branch_edesc, CASE WHEN abbr_code IS NULL THEN branch_edesc ELSE abbr_code END AS abbr   
                    FROM FA_BRANCH_SETUP 
                    WHERE company_code IN {companyCode} AND GROUP_SKU_FLAG = 'I'
                ) b 
                ON a.branch_code = b.code   
                ORDER BY branch_edesc";

                OracleConnection connection = new OracleConnection(_connection);
                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    var reader = command.ExecuteReader();
                    var vals = new List<Dictionary<string, object>>();
                    var qty = new List<double>();
                    var amt = new List<double>();
                    while (reader.Read())
                    {
                        qty.Add(reader.GetDouble(1));
                        amt.Add(reader.GetDouble(2));
                        vals.Add(new Dictionary<string, object>
                            {
                                { "name", reader.GetString(0) },
                                { "qty", reader.GetDouble(1) },
                                { "amt", reader.GetDouble(2) },
                                { "abr", reader.GetString(3) }
                            });
                    }
                    var total = new
                    {
                        total_qty = qty.Sum(),
                        total_amt = amt.Sum(),
                        details = vals
                    };
                    graph["branch_wise_sales"] = total;
                }
                query = $@"
                SELECT company_edesc, qty, amt, abbr FROM
                (
                    SELECT company_code, SUM(sales_qty - sales_ret_qty) AS qty, SUM(sales_value - sales_ret_value) AS amt FROM
                    (                                                                                                
                        SELECT A.COMPANY_CODE,
                        SUM(NVL(A.ROLL_QTY, 0)) AS SALES_ROLL_QTY, SUM(NVL(A.QUANTITY, 0)) AS SALES_QTY, SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) AS SALES_VALUE,
                        0 AS SALES_RET_ROLL_QTY, 0 AS SALES_RET_QTY, 0 AS SALES_RET_VALUE  
                        FROM SA_SALES_INVOICE A 
                        WHERE A.DELETED_FLAG = 'N'                                                                
                        AND TRUNC(a.SALES_DATE) BETWEEN '{fromDate}' AND '{toDate}' {customerStr}
                        GROUP BY A.COMPANY_CODE

                        UNION ALL

                        SELECT A.COMPANY_CODE, 0 AS SALES_ROLL_QTY, 0 AS SALES_QTY, 0 AS SALES_VALUE,
                        SUM(NVL(A.ROLL_QTY, 0)) AS SALES_RET_ROLL_QTY, SUM(NVL(A.QUANTITY, 0)) AS SALES_RET_QTY,
                        SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) AS SALES_RET_VALUE  
                        FROM SA_SALES_RETURN A
                        WHERE A.DELETED_FLAG = 'N'                                                                
                        AND TRUNC(a.return_DATE) BETWEEN '{fromDate}' AND '{toDate}' {customerStr}
                        GROUP BY A.COMPANY_CODE                                                                                                                                 
                    ) 
                    GROUP BY company_code 
                    ORDER BY company_code                                                                
                ) a
                LEFT OUTER JOIN
                (
                    SELECT company_code AS code, company_edesc, CASE WHEN abbr_code IS NULL THEN company_edesc ELSE abbr_code END AS abbr   
                    FROM company_SETUP 
                ) b 
                ON a.company_code = b.code   
                ORDER BY company_edesc";

                //Console.WriteLine(query);
                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    var reader = command.ExecuteReader();
                    var vals = new List<Dictionary<string, object>>();
                    var qty = new List<double>();
                    var amt = new List<double>();
                    while (reader.Read())
                    {
                        qty.Add(reader.GetDouble(1));
                        amt.Add(reader.GetDouble(2));
                        vals.Add(new Dictionary<string, object>
                        {
                            { "name", reader.GetString(0) },
                            { "qty", reader.GetDouble(1) },
                            { "amt", reader.GetDouble(2) },
                            { "abr", reader.GetString(3) }
                        });
                    }
                    var total = new
                    {
                        total_qty = qty.Sum(),
                        total_amt = amt.Sum(),
                        details = vals
                    };
                    graph["com_wise_sales"] = total;
                }

                query = $@"
                    SELECT COALESCE(brand_name, 'not defined') AS brand, SUM(sales_qty - sales_ret_qty) AS qty, SUM(sales_value - sales_ret_value) AS amt 
                    FROM (
                    SELECT * FROM (
                        SELECT A.COMPANY_CODE, A.BRANCH_CODE, a.item_code,
                        SUM(NVL(A.ROLL_QTY, 0)) AS SALES_ROLL_QTY, SUM(NVL(A.QUANTITY, 0)) AS SALES_QTY, 
                        SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) AS SALES_VALUE,
                        0 AS SALES_RET_ROLL_QTY, 0 AS SALES_RET_QTY, 0 AS SALES_RET_VALUE  
                        FROM SA_SALES_INVOICE A 
                        WHERE A.DELETED_FLAG = 'N'  
                        AND A.BRANCH_CODE IN {GetBranchFilter(companyCode, branchCode, userNo)}
                        AND A.COMPANY_CODE IN {companyCode}
                        AND TRUNC(a.SALES_DATE) BETWEEN '{fromDate}' AND '{toDate}' {customerStr} {itemStringFilter}                             
                        GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, a.item_code
            
                        UNION ALL
            
                        SELECT A.COMPANY_CODE, A.BRANCH_CODE, a.item_code, 0 AS SALES_ROLL_QTY, 0 AS SALES_QTY, 0 AS SALES_VALUE,
                        SUM(NVL(A.ROLL_QTY, 0)) AS SALES_RET_ROLL_QTY, SUM(NVL(A.QUANTITY, 0)) AS SALES_RET_QTY,
                        SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) AS SALES_RET_VALUE  
                        FROM SA_SALES_RETURN A 
                        WHERE A.DELETED_FLAG = 'N'             
                        AND A.BRANCH_CODE IN {GetBranchFilter(companyCode, branchCode, userNo)}
                        AND A.COMPANY_CODE IN {companyCode}
                        AND TRUNC(a.return_DATE) BETWEEN '{fromDate}' AND '{toDate}' {customerStr} {itemStringFilter}                             
                        GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, a.item_code
                        ) a 
                        LEFT OUTER JOIN (
                            SELECT item_code AS code, brand_name 
                            FROM ip_item_spec_setup 
                            WHERE COMPANY_CODE IN {companyCode}
                        ) b ON a.item_code = b.code
                    ) 
                    GROUP BY brand_name 
                    ORDER BY brand_name";

                Console.WriteLine(query);
                using (var cmd = new OracleCommand(query, connection))
                {
                    var allCodes = cmd.ExecuteReader();
                    var vals = new List<Dictionary<string, object>>();
                    var qty = new List<double>();
                    var amt = new List<double>();
                    while (allCodes.Read())
                    {
                        qty.Add(allCodes.GetDouble(1));
                        amt.Add(allCodes.GetDouble(2));
                        vals.Add(new Dictionary<string, object>
                        {
                            { "name", allCodes.GetString(0) },
                            { "qty", allCodes.GetDouble(1) },
                            { "amt", allCodes.GetDouble(2) }
                        });
                    }
                    var total = new Dictionary<string, object>
                    {
                        { "total_qty", qty.Sum() },
                        { "total_amt", amt.Sum() },
                        { "details", vals }
                    };
                    graph["brand_wise_sales"] = total;
                    // Category-wise sales query
                    query = $@"
                    SELECT category_edesc, qty, amt 
                    FROM (
                        SELECT COALESCE(category_code, 'not defined') AS cat, SUM(sales_qty - sales_ret_qty) AS qty, SUM(sales_value - sales_ret_value) AS amt 
                        FROM (
                            SELECT * FROM (
                                SELECT A.COMPANY_CODE, A.BRANCH_CODE, a.item_code,
                                SUM(NVL(A.ROLL_QTY, 0)) AS SALES_ROLL_QTY, SUM(NVL(A.QUANTITY, 0)) AS SALES_QTY, 
                                SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) AS SALES_VALUE,
                                0 AS SALES_RET_ROLL_QTY, 0 AS SALES_RET_QTY, 0 AS SALES_RET_VALUE  
                                FROM SA_SALES_INVOICE A 
                                WHERE A.DELETED_FLAG = 'N'  
                                AND A.BRANCH_CODE IN {GetBranchFilter(companyCode, branchCode, userNo)}
                                AND A.COMPANY_CODE IN {companyCode} 
                                AND TRUNC(a.SALES_DATE) BETWEEN '{fromDate}' AND '{toDate}' {customerStr} {itemStringFilter}                             
                                GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, a.item_code
                                UNION ALL
                                SELECT A.COMPANY_CODE, A.BRANCH_CODE, a.item_code, 0 AS SALES_ROLL_QTY, 0 AS SALES_QTY, 0 AS SALES_VALUE,
                                SUM(NVL(A.ROLL_QTY, 0)) AS SALES_RET_ROLL_QTY, SUM(NVL(A.QUANTITY, 0)) AS SALES_RET_QTY,
                                SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) AS SALES_RET_VALUE  
                                FROM SA_SALES_RETURN A 
                                WHERE A.DELETED_FLAG = 'N'             
                                AND A.BRANCH_CODE IN {GetBranchFilter(companyCode, branchCode, userNo)}
                                AND A.COMPANY_CODE IN {companyCode}
                                AND TRUNC(a.return_DATE) BETWEEN '{fromDate}' AND '{toDate}' {customerStr} {itemStringFilter}                             
                                GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, a.item_code
                            ) a 
                            LEFT OUTER JOIN (
                                SELECT item_code AS code, category_code 
                                FROM IP_ITEM_MASTER_SETUP 
                                WHERE COMPANY_CODE IN {companyCode}
                            ) b ON a.item_code = b.code
                        ) 
                        GROUP BY category_code
                    ) a
                    LEFT OUTER JOIN (
                        SELECT category_code AS code, category_edesc 
                        FROM IP_CATEGORY_CODE 
                        WHERE COMPANY_CODE IN {companyCode}
                    ) b ON a.cat = b.code 
                    ORDER BY category_edesc";
                    using (OracleCommand cmdCode = new OracleCommand(query, connection))
                    {
                        allCodes = cmdCode.ExecuteReader();
                        vals = new List<Dictionary<string, object>>();
                        qty = new List<double>();
                        amt = new List<double>();
                        while (allCodes.Read())
                        {
                            qty.Add(allCodes.GetDouble(1));
                            amt.Add(allCodes.GetDouble(2));
                            vals.Add(new Dictionary<string, object>
                            {
                                { "name", allCodes.GetString(0) },
                                { "qty", allCodes.GetDouble(1) },
                                { "amt", allCodes.GetDouble(2) }
                            });
                        }
                        total = new Dictionary<string, object>
                        {
                            { "total_qty", qty.Sum() },
                            { "total_amt", amt.Sum() },
                            { "details", vals }
                        };
                        graph["cat_wise_sales"] = total;

                        // Final query
                        query = @"
                        SELECT * FROM (
                            SELECT * FROM (
                                SELECT A.*, ROWNUM RM FROM (
                                    SELECT ITEM_EDESC, SUM(sales_qty - sales_ret_qty) AS qty, SUM(sales_value - sales_ret_value) AS amt 
                                    FROM (
                                        SELECT * FROM (
                                            SELECT A.COMPANY_CODE, A.BRANCH_CODE, a.item_code,
                                            SUM(NVL(A.ROLL_QTY, 0)) AS SALES_ROLL_QTY, SUM(NVL(A.QUANTITY, 0)) AS SALES_QTY, 
                                            SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) AS SALES_VALUE,
                                            0 AS SALES_RET_ROLL_QTY, 0 AS SALES_RET_QTY, 0 AS SALES_RET_VALUE  
                                            FROM SA_SALES_INVOICE A 
                                            WHERE A.DELETED_FLAG = 'N'  
                                            AND A.BRANCH_CODE IN {BranchFilter(company_code, branch_code, user_no)}
                                            AND A.COMPANY_CODE IN {company_code}
                                            GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, a.item_code
                                            UNION ALL
                                            SELECT A.COMPANY_CODE, A.BRANCH_CODE, a.item_code, 0 AS SALES_ROLL_QTY, 0 AS SALES_QTY, 0 AS SALES_VALUE,
                                            SUM(NVL(A.ROLL_QTY, 0)) AS SALES_RET_ROLL_QTY, SUM(NVL(A.QUANTITY, 0)) AS SALES_RET_QTY,
                                            SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) AS SALES_RET_VALUE  
                                            FROM SA_SALES_RETURN A 
                                            WHERE A.DELETED_FLAG = 'N'             
                                            AND A.BRANCH_CODE IN {BranchFilter(company_code, branch_code, user_no)}
                                            AND A.COMPANY_CODE IN {company_code}
                                            GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, a.item_code
                                        ) a 
                                    ) 
                                ) 
                            ) 
                        )";
                        query = @"
                        SELECT * FROM (
                            SELECT * FROM (
                                SELECT A.*, ROWNUM RM FROM (
                                    SELECT ITEM_EDESC, SUM(sales_qty - sales_ret_qty) AS qty, SUM(sales_value - sales_ret_value) AS amt 
                                    FROM (
                                        SELECT * FROM (
                                            SELECT A.COMPANY_CODE, A.BRANCH_CODE, a.item_code,
                                            SUM(NVL(A.ROLL_QTY, 0)) AS SALES_ROLL_QTY, SUM(NVL(A.QUANTITY, 0)) AS SALES_QTY, 
                                            SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) AS SALES_VALUE,
                                            0 AS SALES_RET_ROLL_QTY, 0 AS SALES_RET_QTY, 0 AS SALES_RET_VALUE  
                                            FROM SA_SALES_INVOICE A 
                                            WHERE A.DELETED_FLAG = 'N'  
                                            AND A.BRANCH_CODE IN {BranchFilter(company_code, branch_code, user_no)}
                                            AND A.COMPANY_CODE IN {company_code}
                                            GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, a.item_code
                                            UNION ALL
                                            SELECT A.COMPANY_CODE, A.BRANCH_CODE, a.item_code, 0 AS SALES_ROLL_QTY, 0 AS SALES_QTY, 0 AS SALES_VALUE,
                                            SUM(NVL(A.ROLL_QTY, 0)) AS SALES_RET_ROLL_QTY, SUM(NVL(A.QUANTITY, 0)) AS SALES_RET_QTY,
                                            SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) AS SALES_RET_VALUE  
                                            FROM SA_SALES_RETURN A 
                                            WHERE A.DELETED_FLAG = 'N'             
                                            AND A.BRANCH_CODE IN {BranchFilter(company_code, branch_code, user_no)}
                                            AND A.COMPANY_CODE IN {company_code}
                                            GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, a.item_code
                                        ) a 
                                    ) 
                                    GROUP BY item_code
                                ) A 
                                WHERE ROWNUM <= 10  -- Limit the result to the top 10 items
                            )
                        ) ORDER BY amt DESC";  
                        Console.WriteLine(query);
                        using (var cmdQuery = new OracleCommand(query, connection))
                        {
                            allCodes = cmdQuery.ExecuteReader();
                            vals = new List<Dictionary<string, object>>();
                            qty = new List<double>();
                            amt = new List<double>();
                            while (allCodes.Read())
                            {
                                vals.Add(new Dictionary<string, object>
                                {
                                    { "item_name", allCodes.GetString(0) }, 
                                    { "qty", allCodes.GetDouble(1) },        
                                    { "amt", allCodes.GetDouble(2) }        
                                });
                            }
                            // Prepare the final data structure for the result
                            var finalResult = new Dictionary<string, object>
                            {
                                { "top_items", vals },
                                { "total_qty", vals.Sum(v => (double)v["qty"]) },
                                { "total_amt", vals.Sum(v => (double)v["amt"]) }
                            };
                            graph["top_items_sales"] = finalResult;
                        }
                        return graph;
                    }
                }
            }
            else if (repType == "period_wise_sales")
            {
                var cals = new Dictionary<string, object>();

                string query;

                if (withEngDate == true)
                {
                    query = $@"
                        SELECT item_edesc, this_year_qty, this_year_amt, last_month_qty, last_month_amt, this_month_qty, this_month_amt, index_mu_code, ss 
                        FROM (
                            SELECT rangename, qty, amt, item_edesc, index_mu_code, ss 
                            FROM (
                                SELECT startdate, rangename, item_code, SUM(sales_qty - sales_ret_qty) AS qty, SUM(sales_value - sales_ret_value) AS amt 
                                FROM (
                                    SELECT * FROM (
                                        SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.sales_date, a.item_code,
                                            SUM(NVL(A.ROLL_QTY, 0)) AS SALES_ROLL_QTY,
                                            SUM(NVL(A.QUANTITY, 0)) AS SALES_QTY,
                                            SUM(NVL(A.QUANTITY * A.{grossNetStr}, 0)) AS SALES_VALUE,
                                            0 AS SALES_RET_ROLL_QTY,
                                            0 AS SALES_RET_QTY,
                                            0 AS SALES_RET_VALUE 
                                        FROM SA_SALES_INVOICE A 
                                        WHERE A.DELETED_FLAG = 'N'
                                            AND A.BRANCH_CODE IN {GetBranchFilter(companyCode, branchCode, userNo)}
                                            AND A.COMPANY_CODE IN {companyCode} 
                                            {customerStr} 
                                            {itemStringFilter}
                                            AND item_code IN (
                                                SELECT item_code FROM ip_item_master_setup WHERE company_code IN {companyCode}
                                            )
                                        GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.sales_date, a.item_code
                                        UNION ALL
                                        SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.return_date, a.item_code,
                                            0 AS SALES_ROLL_QTY, 0 AS SALES_QTY, 0 AS SALES_VALUE,
                                            SUM(NVL(A.ROLL_QTY, 0)) AS SALES_RET_ROLL_QTY,
                                            SUM(NVL(A.QUANTITY, 0)) AS SALES_RET_QTY,
                                            SUM(NVL(A.QUANTITY * A.{grossNetStr}, 0)) AS SALES_RET_VALUE 
                                        FROM SA_SALES_RETURN A 
                                        WHERE A.DELETED_FLAG = 'N'
                                            AND A.BRANCH_CODE IN {GetBranchFilter(companyCode, branchCode, userNo)}
                                            AND A.COMPANY_CODE IN {companyCode} 
                                            {customerStr} 
                                            {itemStringFilter}
                                            AND item_code IN (
                                                SELECT item_code FROM ip_item_master_setup WHERE company_code IN {companyCode}
                                            )
                                        GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.return_date, a.item_code 
                                        ORDER BY sales_date
                                    ) a
                                    LEFT OUTER JOIN (
                                        SELECT * FROM V_DATE_RANGE WHERE rangename IN ('This Month', 'Last Month', 'This Year')
                                    ) b ON a.sales_date BETWEEN b.startdate AND b.enddate
                                ) 
                                GROUP BY startdate, rangename, item_code 
                                ORDER BY startdate
                            ) a
                            LEFT OUTER JOIN (
                                SELECT item_code AS ss, item_edesc, index_mu_code 
                                FROM ip_item_master_setup WHERE company_code IN {companyCode} 
                                GROUP BY item_code, item_edesc, index_mu_code
                            ) b ON a.item_code = b.ss
                        )
                        PIVOT (
                            SUM(qty) AS qty, SUM(amt) AS amt
                            FOR rangename IN ('This Year' AS This_Year, 'Last Month' AS Last_Month, 'This Month' AS This_Month)
                        ) 
                        ORDER BY ITEM_EDESC";
                }
                else
                {
                    query = $@"
                        SELECT item_edesc, this_year_qty, this_year_amt, last_month_qty, last_month_amt, this_month_qty, this_month_amt, index_mu_code, ss 
                        FROM (
                            SELECT rangename, qty, amt, item_edesc, index_mu_code, ss 
                            FROM (
                                SELECT startdate, rangename, item_code, SUM(sales_qty - sales_ret_qty) AS qty, SUM(sales_value - sales_ret_value) AS amt 
                                FROM (
                                    SELECT * FROM (
                                        SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.sales_date, a.item_code,
                                            SUM(NVL(A.ROLL_QTY, 0)) AS SALES_ROLL_QTY,
                                            SUM(NVL(A.QUANTITY, 0)) AS SALES_QTY,
                                            SUM(NVL(A.QUANTITY * A.{grossNetStr}, 0)) AS SALES_VALUE,
                                            0 AS SALES_RET_ROLL_QTY,
                                            0 AS SALES_RET_QTY,
                                            0 AS SALES_RET_VALUE 
                                        FROM SA_SALES_INVOICE A 
                                        WHERE A.DELETED_FLAG = 'N'
                                            AND A.BRANCH_CODE IN {GetBranchFilter(companyCode, branchCode, userNo)}
                                            AND A.COMPANY_CODE IN {companyCode} 
                                            {customerStr} 
                                            {itemStringFilter}
                                            AND item_code IN (
                                                SELECT item_code FROM ip_item_master_setup WHERE company_code IN {companyCode}
                                            )
                                        GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.sales_date, a.item_code
                                        UNION ALL
                                        SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.return_date, a.item_code,
                                            0 AS SALES_ROLL_QTY, 0 AS SALES_QTY, 0 AS SALES_VALUE,
                                            SUM(NVL(A.ROLL_QTY, 0)) AS SALES_RET_ROLL_QTY,
                                            SUM(NVL(A.QUANTITY, 0)) AS SALES_RET_QTY,
                                            SUM(NVL(A.QUANTITY * A.{grossNetStr}, 0)) AS SALES_RET_VALUE 
                                        FROM SA_SALES_RETURN A 
                                        WHERE A.DELETED_FLAG = 'N'
                                            AND A.BRANCH_CODE IN {GetBranchFilter(companyCode, branchCode, userNo)}
                                            AND A.COMPANY_CODE IN {companyCode} 
                                            {customerStr} 
                                            {itemStringFilter}
                                            AND item_code IN (
                                                SELECT item_code FROM ip_item_master_setup WHERE company_code IN {companyCode}
                                            )
                                        GROUP BY A.COMPANY_CODE, A.BRANCH_CODE, A.return_date, a.item_code 
                                        ORDER BY sales_date
                                    ) a
                                    LEFT OUTER JOIN (
                                        SELECT TRUNC(SYSDATE, 'MONTH') AS startdate, 
                                               TRUNC(LAST_DAY(SYSDATE)) AS enddate, 'This Month' AS rangename 
                                        FROM dual
                                        UNION ALL
                                        SELECT TRUNC(ADD_MONTHS(SYSDATE, -1), 'MONTH') AS startdate, 
                                               TRUNC(LAST_DAY(ADD_MONTHS(SYSDATE, -1))) AS enddate, 'Last Month' AS rangename 
                                        FROM dual
                                        UNION ALL
                                        SELECT startdate, enddate, rangename FROM V_DATE_RANGE WHERE rangename IN ('This Year')
                                    ) b ON a.sales_date BETWEEN b.startdate AND b.enddate
                                ) 
                                GROUP BY startdate, rangename, item_code 
                                ORDER BY startdate
                            ) a
                            LEFT OUTER JOIN (
                                SELECT item_code AS ss, item_edesc, index_mu_code 
                                FROM ip_item_master_setup WHERE company_code IN {companyCode} 
                                GROUP BY item_code, item_edesc, index_mu_code
                            ) b ON a.item_code = b.ss
                        )
                        PIVOT (
                            SUM(qty) AS qty, SUM(amt) AS amt
                            FOR rangename IN ('This Year' AS This_Year, 'Last Month' AS Last_Month, 'This Month' AS This_Month)
                        ) 
                        ORDER BY ITEM_EDESC";
                }
                var results = new List<Dictionary<string, object>>();
                using (var connection = new OracleConnection(_connection))
                {
                    using (var command = new OracleCommand(query, connection))
                    {
                        connection.Open();
                        using (var adapter = new OracleDataAdapter(command))
                        {
                            var dataTable = new DataTable();
                            adapter.Fill(dataTable);

                            foreach (DataRow row in dataTable.Rows)
                            {
                                var rowDict = new Dictionary<string, object>();
                                foreach (DataColumn column in dataTable.Columns)
                                {
                                    rowDict[column.ColumnName] = row[column];
                                }
                                results.Add(rowDict);
                            }
                        }
                    }
                }
                var THIS_YEAR_QTY = new List<double>();
                var THIS_YEAR_AMT = new List<double>();
                var LAST_MONTH_QTY = new List<double>();
                var LAST_MONTH_AMT = new List<double>();
                var THIS_MONTH_QTY = new List<double>();
                var THIS_MONTH_AMT = new List<double>();
                var multi_unit_d = new Dictionary<string, dynamic>();

                if (!string.IsNullOrEmpty(unitType))
                {
                    multi_unit_d = GetMultiQty(unitType, companyCode); // Assuming GetMultiQty returns a dictionary
                }
                foreach (var i in results)
                {
                    double t1 = i.ContainsKey("this_year_qty") && i["this_year_qty"] != null ? Convert.ToDouble(i["this_year_qty"]) : 0;
                    double t2 = i.ContainsKey("this_year_amt") && i["this_year_amt"] != null ? Convert.ToDouble(i["this_year_amt"]) : 0;
                    double t3 = i.ContainsKey("last_month_qty") && i["last_month_qty"] != null ? Convert.ToDouble(i["last_month_qty"]) : 0;
                    double t4 = i.ContainsKey("last_month_amt") && i["last_month_amt"] != null ? Convert.ToDouble(i["last_month_amt"]) : 0;
                    double t5 = i.ContainsKey("this_month_qty") && i["this_month_qty"] != null ? Convert.ToDouble(i["this_month_qty"]) : 0;
                    double t6 = i.ContainsKey("this_month_amt") && i["this_month_amt"] != null ? Convert.ToDouble(i["this_month_amt"]) : 0;

                    double q1=0, q2=0, q3=0;
                    string unit = "";
                    double ratio=0;
                    if (!string.IsNullOrEmpty(unitType))
                    {
                        if (i.ContainsKey("item_code") && i["item_code"] != null)
                        {
                            string itemCodes = i["item_code"].ToString(); // Safely cast to string
                            if (multi_unit_d.ContainsKey(itemCodes))
                            {
                                var ratioObj = multi_unit_d[itemCodes]["ratio"];
                                var muCodeObj = multi_unit_d[itemCodes]["mu_code"];
                                if (ratioObj != null && double.TryParse(ratioObj.ToString(), out ratio))
                                {
                                    q1 = t1 * ratio;
                                    q2 = t3 * ratio;
                                    q3 = t5 * ratio;
                                }
                                else
                                {
                                    q1 = q2 = q3 = 0; // Default to 0 if ratio parsing fails
                                }

                                unit = muCodeObj != null ? muCodeObj.ToString() : string.Empty; // Safely set unit to empty if mu_code is null
                            }
                        }
                        else
                        {
                            q1 = q2 = q3 = 0;
                        }
                    }
                    else
                    {
                        q1 = t1;
                        q2 = t3;
                        q3 = t5;
                    }
                    THIS_YEAR_QTY.Add(q1);
                    THIS_YEAR_AMT.Add(t2);
                    LAST_MONTH_QTY.Add(q2);
                    LAST_MONTH_AMT.Add(t4);
                    THIS_MONTH_QTY.Add(q3);
                    THIS_MONTH_AMT.Add(t6);
                }
                cals.Add("this_year_qty", THIS_YEAR_QTY);
                cals.Add("this_year_amt", THIS_YEAR_AMT);
                cals.Add("last_month_qty", LAST_MONTH_QTY);
                cals.Add("last_month_amt", LAST_MONTH_AMT);
                cals.Add("this_month_qty", THIS_MONTH_QTY);
                cals.Add("this_month_amt", THIS_MONTH_AMT);
                cals.Add("item_edesc", results); 
                return cals; 
            }
            else
            {
                return null;
            }
        }
        private Dictionary<string, object> GetBranchWiseSales(string companyCode, string branchCode, string fromDate, string toDate, string customerStr, string itemStringFilter, string grossNetStr)
        {
            var branchSales = new Dictionary<string, object>();
            string query = $@"
            SELECT branch_edesc, qty, amt, abbr FROM (
                SELECT branch_code, SUM(sales_qty - sales_ret_qty) AS qty, SUM(sales_value - sales_ret_value) AS amt FROM (
                    SELECT A.COMPANY_CODE, A.BRANCH_CODE,
                        SUM(ISNULL(A.ROLL_QTY, 0)) AS SALES_ROLL_QTY,
                        SUM(ISNULL(A.QUANTITY, 0)) AS SALES_QTY,
                        SUM(ISNULL(A.QUANTITY * A.{grossNetStr}, 0)) AS SALES_VALUE,
                        0 AS SALES_RET_ROLL_QTY,
                        0 AS SALES_RET_QTY,
                        0 AS SALES_RET_VALUE
                    FROM SA_SALES_INVOICE A
                    WHERE A.DELETED_FLAG = 'N'
                        AND A.BRANCH_CODE IN ({GetBrandWiseItem(companyCode, branchCode)})
                        AND A.COMPANY_CODE IN ({companyCode})
                        AND CAST(A.SALES_DATE AS DATE) BETWEEN '{fromDate}' AND '{toDate}' {customerStr} {itemStringFilter}
                    GROUP BY A.COMPANY_CODE, A.BRANCH_CODE
                    UNION ALL
                    SELECT A.COMPANY_CODE, A.BRANCH_CODE, 0 AS SALES_ROLL_QTY, 0 AS SALES_QTY, 0 AS SALES_VALUE,
                        SUM(ISNULL(A.ROLL_QTY, 0)) AS SALES_RET_ROLL_QTY,
                        SUM(ISNULL(A.QUANTITY, 0)) AS SALES_RET_QTY,
                        SUM(ISNULL(A.QUANTITY * A.{grossNetStr}, 0)) AS SALES_RET_VALUE
                    FROM SA_SALES_RETURN A
                    WHERE A.DELETED_FLAG = 'N'
                        AND A.BRANCH_CODE IN ({GetBrandWiseItem(companyCode, branchCode)})
                        AND A.COMPANY_CODE IN ({companyCode})
                        AND CAST(A.RETURN_DATE AS DATE) BETWEEN '{fromDate}' AND '{toDate}' {customerStr} {itemStringFilter}
                    GROUP BY A.COMPANY_CODE, A.BRANCH_CODE
                ) AS grouped_sales
                GROUP BY branch_code 
            ) AS a
            LEFT JOIN (
                SELECT branch_code AS code, branch_edesc, CASE WHEN abbr_code IS NULL THEN branch_edesc ELSE abbr_code END AS abbr
                FROM FA_BRANCH_SETUP WHERE company_code IN ({companyCode}) AND GROUP_SKU_FLAG = 'I'
            ) AS b ON a.branch_code = b.code 
            ORDER BY branch_edesc";
            var connection = new OracleConnection(_connection);
            using (var command = new OracleCommand(query, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    var vals = new List<Dictionary<string, object>>();
                    var qty = new List<double>();
                    var amt = new List<double>();

                    while (reader.Read())
                    {
                        qty.Add(reader.GetDouble(1));
                        amt.Add(reader.GetDouble(2));
                        vals.Add(new Dictionary<string, object>
                    {
                        { "name", reader.GetString(0) },
                        { "qty", reader.GetDouble(1) },
                        { "amt", reader.GetDouble(2) },
                        { "abr", reader.GetString(3) }
                       });
                    }
                    //var total = new Dictionary<string, double>
                    //{
                    //    { "total_qty", qty.Sum() },
                    //    { "total_amt", amt.Sum() },
                    //    { "details", vals }
                    //};
                    //branchSales = total;

                    var total = new Dictionary<string, object>
                    {
                        { "total_qty", qty.Sum() },  
                        { "total_amt", amt.Sum() },  
                        { "details", vals }          
                    };
                    branchSales = total;
                }
                connection.Close();
            }
            return branchSales;
        }
        private Dictionary<string, object> GetCompanyWiseSales(string companyCode, string fromDate, string toDate, string customerStr, string grossNetStr)
        {
            var companySales = new Dictionary<string, object>();
            string query = $@"
            SELECT company_edesc, qty, amt, abbr FROM (
                SELECT company_code, SUM(sales_qty - sales_ret_qty) AS qty, SUM(sales_value - sales_ret_value) AS amt FROM (
                    SELECT A.COMPANY_CODE,
                        SUM(ISNULL(A.ROLL_QTY, 0)) AS SALES_ROLL_QTY,
                        SUM(ISNULL(A.QUANTITY, 0)) AS SALES_QTY,
                        SUM(ISNULL(A.QUANTITY * A.NET_GROSS_RATE, 0)) AS SALES_VALUE,
                        0 AS SALES_RET_ROLL_QTY,
                        0 AS SALES_RET_QTY,
                        0 AS SALES_RET_VALUE
                    FROM SA_SALES_INVOICE A
                    WHERE A.DELETED_FLAG = 'N'
                        AND CAST(A.SALES_DATE AS DATE) BETWEEN '{fromDate}' AND '{toDate}' {customerStr}
                    GROUP BY A.COMPANY_CODE
                    UNION ALL
                    SELECT A.COMPANY_CODE, 0 AS SALES_ROLL_QTY, 0 AS SALES_QTY, 0 AS SALES_VALUE,
                        SUM(ISNULL(A.ROLL_QTY, 0)) AS SALES_RET_ROLL_QTY,
                        SUM(ISNULL(A.QUANTITY, 0)) AS SALES_RET_QTY,
                        SUM(ISNULL(A.QUANTITY * A.NET_GROSS_RATE, 0)) AS SALES_RET_VALUE
                    FROM SA_SALES_RETURN A
                    WHERE A.DELETED_FLAG = 'N'
                        AND CAST(A.RETURN_DATE AS DATE) BETWEEN '{fromDate}' AND '{toDate}' {customerStr}
                    GROUP BY A.COMPANY_CODE
                ) AS sales_data
                GROUP BY company_code 
            ) AS a
            LEFT JOIN (
                SELECT company_code AS code, company_edesc, CASE WHEN abbr_code IS NULL THEN company_edesc ELSE abbr_code END AS abbr
                FROM company_setup
            ) AS b ON a.company_code = b.code 
            ORDER BY company_edesc";
            var connection = new OracleConnection(_connection);
            using (var command = new OracleCommand(query, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    var vals = new List<Dictionary<string, object>>();
                    var qty = new List<double>();
                    var amt = new List<double>();

                    while (reader.Read())
                    {
                        qty.Add(reader.GetDouble(1));
                        amt.Add(reader.GetDouble(2));
                        vals.Add(new Dictionary<string, object>
                    {
                        { "name", reader.GetString(0) },
                        { "qty", reader.GetDouble(1) },
                        { "amt", reader.GetDouble(2) },
                        { "abr", reader.GetString(3) }
                    });
                    }
                   //var total = new Dictionary<string, double>
                   // {
                   //     { "total_qty", qty.Sum() },
                   //     { "total_amt", amt.Sum() },
                   //     { "details", vals }
                   // };
                   // companySales = total;
                }
                connection.Close();
            }

            return companySales;
        }
        private List<Dictionary<string, object>> GetAllCompanies(string fromDate, string toDate, string customerStr)
        {
            var allCompanies = new List<Dictionary<string, object>>();
            string query = "SELECT company_code, abbr_code, company_edesc FROM company_setup WHERE deleted_flag = 'N'";
            var connection = new OracleConnection(_connection);
            using (var command = new OracleCommand(query, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var company = new Dictionary<string, object>
                    {
                        { "code", reader.GetString(0) },
                        { "name", reader.GetString(2) },
                        { "abbr", reader.GetString(1) }
                    };
                        allCompanies.Add(company);
                    }
                }
                connection.Close();
            }

            return allCompanies;
        }
        public Dictionary<string, object> ProcessCodes(List<object[]> allCodes, string name, string motherCode, string parentCode, string groupFlag, params double[] totals)
        {
            double[] totalSums = new double[totals.Length];
            bool addOn = false;

            if (groupFlag == "G")
            {
                foreach (var z in allCodes)
                {
                    if (z[3].ToString() == "I")
                    {
                        string asd = z[1].ToString().Substring(0, Math.Min(motherCode.Length, z[1].ToString().Length));

                        if (motherCode.Contains(asd))
                        {
                            addOn = true;

                            for (int idx = 0; idx < totals.Length; idx++)
                            {
                                totalSums[idx] += Convert.ToDouble(z[4 + idx]);
                            }
                        }
                        else if (addOn)
                        {
                            break;
                        }
                    }
                }

                if (totalSums.Any(ts => ts != 0))
                {
                    var result = new Dictionary<string, object>
            {
                { "name", name },
                { "level", motherCode.Count(c => c == '.') },
                { "GSKU", groupFlag },
                { "master", motherCode },
                { "pre", parentCode }
            };

                    for (int idx = 0; idx < totals.Length; idx++)
                    {
                        result[$"total{idx + 1}"] = totalSums[idx];
                    }

                    return result;
                }
            }
            else if (groupFlag == "I")
            {
                var result = new Dictionary<string, object>
                {
                    { "name", name },
                    { "level", motherCode.Count(c => c == '.') },
                    { "GSKU", groupFlag },
                    { "master", motherCode },
                    { "pre", parentCode }
                };

                for (int idx = 0; idx < totals.Length; idx++)
                {
                    result[$"total{idx + 1}"] = totals[idx];
                }

                if (totals.Any(t => t != 0))
                {
                    return result;
                }
            }

            return null;
        }
        private object ProcessCodes(IEnumerable<SalesData> salesData, string customerDescription, string masterCustomerCode, string preCustomerCode, string groupSkuFlag,
        decimal salesQty, decimal salesValue, decimal salesRetQty, decimal salesRetValue, decimal netSalesQty, decimal netSalesValue)
        {
            return new
            {
                name = customerDescription,
                master_customer_code = masterCustomerCode,
                pre_customer_code = preCustomerCode,
                group_sku_flag = groupSkuFlag,
                sales_qty = salesQty,
                sales_val = salesValue,
                sales_ret_qty = salesRetQty,
                sales_ret_val = salesRetValue,
                sales_net_qty = netSalesQty,
                sales_net_val = netSalesValue
            };
        }
        #endregion

        #region Data Execution of ADO.NET Engin Developed By: Prem Prakash Dhakal Date: 09-19-2024
        public async Task<DataTable> ExecuteQueryAsync(string query)
        {
            using (var connection = new OracleConnection(_connection))
            {
                await connection.OpenAsync();
                using (var command = new OracleCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var dataTable = new DataTable();
                        dataTable.Load(reader);
                        return dataTable;
                    }
                }
            }
        }
        private List<object[]> ExecuteQueryList(string query)
        {
            var results = new List<object[]>();
            using (OracleConnection connection = new OracleConnection(_connection))
            {
                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    connection.Open();
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new object[reader.FieldCount];
                            reader.GetValues(row);
                            results.Add(row);
                        }
                    }
                    connection.Close();
                }
            }
            return results;
        }
        public IHttpActionResult ExecuteOracleQuery(string query)
        {
            DataTable resultTable = new DataTable();

            try
            {
                using (OracleConnection connection = new OracleConnection(_connection))
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(resultTable);
                        }
                    }
                }

                // Return the result as a JSON response
                return Ok(resultTable);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        public async Task<DataTable> ExecuteQueryAsync(string query, Dictionary<string, object> parameters)
        {
            var dataTable = new DataTable();

            using (var connection = new OracleConnection(_connection))
            {
                await connection.OpenAsync();

                using (var command = new OracleCommand(query, connection))
                {
                    foreach (var param in parameters)
                    {
                        command.Parameters.Add(new OracleParameter(param.Key, param.Value ?? DBNull.Value));
                    }

                    using (var adapter = new OracleDataAdapter(command))
                    {
                        await Task.Run(() => adapter.Fill(dataTable));
                    }
                }
            }
            return dataTable;
        }
        public async Task<decimal> ExecuteScalarAsync(OracleConnection connection, string query)
        {
            using (var command = new OracleCommand(query, connection))
            {
                var result = await command.ExecuteScalarAsync();
                return Convert.ToDecimal(result);
            }
        }
        // Method to execute query and return results as List of Dictionaries
        public List<Dictionary<string, object>> ExecuteQuery(string query)
        {
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

            try
            {
                using (OracleConnection connection = new OracleConnection(_connection))
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            DataTable resultTable = new DataTable();
                            adapter.Fill(resultTable);  // Fill the result in a DataTable

                            // Iterate over the rows in the DataTable
                            foreach (DataRow row in resultTable.Rows)
                            {
                                Dictionary<string, object> rowData = new Dictionary<string, object>();

                                // Iterate over each column and add to the dictionary
                                foreach (DataColumn column in resultTable.Columns)
                                {
                                    rowData[column.ColumnName] = row[column];
                                }

                                rows.Add(rowData);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error executing query: " + ex.Message, ex);
            }

            return rows;
        }
        #endregion
    }
    #region For Dashboard and Report Class Developed By: Prem Prakash Dhakal Date: 09-19-2024
    public class MonthlyWiseSalesReport
    {
        public string Month { get; set; }
        public double Qty { get; set; }
        public double Amt { get; set; }
        public List<MonthlySalesData> Details { get; set; }
        public double TotalAmt { get; set; }
        public double TotalQty { get; set; }
    }
    public class MonthlySalesData
    {
        public string Month { get; set; }
        public double Qty { get; set; }
        public double Amt { get; set; }
        public string Abbr { get; set; }
    }
    public class ClosingRequest
    {
        public List<double> Drs { get; set; }
        public List<double> Crs { get; set; }
    }
    public class LoanOverview
    {
        public List<LoanGroupDetail> Rep { get; set; }
        public string Name { get; set; }
        public decimal Total { get; set; }
        public string Detail { get; set; }
        public string Topic { get; set; }
    }
    public class LoanGroupDetail
    {
        public string Item { get; set; }
        public string GroupFlag { get; set; }
        public decimal Total { get; set; }
        public int Level { get; set; }
        public string Master { get; set; }
        public string Pre { get; set; }
        public string Code { get; set; }
    }
    public class OverviewRequestModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string CompanyCode { get; set; }
    }
    public class ApproveData
    {
        public string VoucherNo { get; set; }
        public string Step { get; set; }
    }
    public class SupplierAgingReport
    {
        public string CustomerCode { get; set; }
        public string CustomerEdesc { get; set; }
        public string GroupSkuFlag { get; set; }
        public string MasterCustomerCode { get; set; }
        public string PreCustomerCode { get; set; }
        public double Total { get; set; }
        public double Total_030 { get; set; }
        public List<dynamic> Total_030_Detail { get; set; } = new List<dynamic>();
        public double Total_3160 { get; set; }
        public List<dynamic> Total_3160_Detail { get; set; } = new List<dynamic>();
        public double Total_6190 { get; set; }
        public List<dynamic> Total_6190_Detail { get; set; } = new List<dynamic>();
        public double Total_91120 { get; set; }
        public List<dynamic> Total_91120_Detail { get; set; } = new List<dynamic>();
        public double Total_120Plus { get; set; }
        public List<dynamic> Total_120Plus_Detail { get; set; } = new List<dynamic>();
    }
    public class VirtualSubLedger
    {
        public string SubCode { get; set; }
        public string VoucherNo { get; set; }
        public DateTime VoucherDate { get; set; }
        public double DrAmount { get; set; }
        public double CrAmount { get; set; }
        public string CompanyCode { get; set; }
        public string FormCode { get; set; }
    }
    public class SupplierSetup
    {
        public string SupplierCode { get; set; }
        public string SupplierEdesc { get; set; }
        public string GroupSkuFlag { get; set; }
        public string MasterSupplierCode { get; set; }
        public string PreSupplierCode { get; set; }
        public string CompanyCode { get; set; }
        public bool DeletedFlag { get; set; }
    }
    public class RequestData
    {
        public int Interval { get; set; }
        public List<CodeData> AllCodes { get; set; }
    }
    public class CodeData
    {
        public string SubCode { get; set; }
        public string VNo { get; set; }
        public float Balance { get; set; }
        public int Days { get; set; }
    }
    public class BalanceDetails
    {
        public float Amt { get; set; }
        public string VNo { get; set; }
        public int Days { get; set; }
    }
    public class CustomerSummary
    {
        public string SubCode { get; set; }
        public float Total { get; set; }
        public float First030 { get; set; }
        public List<BalanceDetails> First030Details { get; set; }
        public float Third3160 { get; set; }
        public List<BalanceDetails> Third3160Details { get; set; }
        public float Fourth6190 { get; set; }
        public List<BalanceDetails> Fourth6190Details { get; set; }
        public float Sixth91120 { get; set; }
        public List<BalanceDetails> Sixth91120Details { get; set; }
        public float Seventh120 { get; set; }
        public List<BalanceDetails> Seventh120Details { get; set; }
    }
    public class CustomerDetail
    {
        public string CustomerCode { get; set; }
        public float Total { get; set; }
        public float First030 { get; set; }
        public List<BalanceDetails> First030Details { get; set; }
        public float Third3160 { get; set; }
        public List<BalanceDetails> Third3160Details { get; set; }
        public float Fourth6190 { get; set; }
        public List<BalanceDetails> Fourth6190Details { get; set; }
        public float Sixth91120 { get; set; }
        public List<BalanceDetails> Sixth91120Details { get; set; }
        public float Seventh120 { get; set; }
        public List<BalanceDetails> Seventh120Details { get; set; }
    }
    public class AgeingRequest
    {
        public List<string> CompanyCode { get; set; }
        public string Interval { get; set; }
        public string GrpCode { get; set; }
        public string CusCode { get; set; }
        public List<CodeData> AllCodes { get; set; }
    }
    public class SupplierDetail
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Pan { get; set; }
        public int CreditDays { get; set; }
        public float CreditLimit { get; set; }
    }
    public class ItemHistory
    {
        public string InvoiceNo { get; set; }
        public string ItemCode { get; set; }
        public float Rate { get; set; }
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
    }
    public class SubLedgerReport
    {
        public DateTime VoucherDate { get; set; }
        public string VoucherNo { get; set; }
        public float DebitAmount { get; set; }
        public float CreditAmount { get; set; }
        public string Particulars { get; set; }
        public string CreatedBy { get; set; }
        public string CurrencyCode { get; set; }
        public float ExchangeRate { get; set; }
        public string Miti { get; set; }
    }
    public class DealerResponse
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
    public class SubLedgerTransactionResponse
    {
        public string LinkSubCode { get; set; }
        public string SubName { get; set; }
    }
    public class SubLedgerResponse
    {
        public string LinkSubCode { get; set; }
        public string SubName { get; set; }
    }
    public class CompanyResponse
    {
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string TpinVatNo { get; set; }
        public List<BranchResponse> Branches { get; set; }
    }
    public class BranchResponse
    {
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string BranchAddress { get; set; }
    }
    public class UserAccessInfo
    {
        public string Password { get; set; }
        public string CompanyCode { get; set; }
        public int UserNo { get; set; }
        public string MobileUsers { get; set; }
        public string MobileAdminUsers { get; set; }
    }
    public class AssetDetail
    {
        public string Topic { get; set; }
        public decimal Amount { get; set; }
    }
    public class CurrentAssetsOverview
    {
        public decimal Amt { get; set; }
        public string Topic { get; set; }
        public List<AssetDetail> Detail { get; set; }
    }
    public class LoanDetail
    {
        public string Topic { get; set; }
        public decimal Amount { get; set; }
    }
    public class ExpenseGroupDetail
    {
        public string Item { get; set; }
        public string GroupFlag { get; set; }
        public decimal Total { get; set; }
        public int Level { get; set; }
        public string Master { get; set; }
        public string Pre { get; set; }
        public string Code { get; set; }
    }
    public class ExpenseOverview
    {
        public List<ExpenseGroupDetail> Rep { get; set; }
        public decimal Total { get; set; }
        public string Name { get; set; }
    }
    public class QueryRequest
    {
        public string Query { get; set; }
        public string Question { get; set; }
    }
    public class SalesReportResults
    {
        public string ITEM_EDESC { get; set; }
        public string INDEX_MU_CODE { get; set; }
        public decimal SALES_ROLL_QTY { get; set; }
        public decimal SALES_QTY { get; set; }
        public decimal SALES_VALUE { get; set; }
        public decimal SALES_RET_ROLL_QTY { get; set; }
        public decimal SALES_RET_QTY { get; set; }
        public decimal SALES_RET_VALUE { get; set; }
        public decimal NET_ROLL_QTY { get; set; }
        public decimal NET_SALES_QTY { get; set; }
        public decimal NET_SALES_VALUE { get; set; }
        public string ITEM_CODE { get; set; }
        public string PARTY_TYPE_EDESC { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
    }
    public class SalesData
    {
        public string CustomerDescription { get; set; }
        public string MasterCustomerCode { get; set; }
        public string PreCustomerCode { get; set; }
        public string GroupSkuFlag { get; set; }
        public decimal SalesQty { get; set; }
        public decimal SalesValue { get; set; }
        public decimal SalesRetQty { get; set; }
        public decimal SalesRetValue { get; set; }
        public decimal NetSalesQty { get; set; }
        public decimal NetSalesValue { get; set; }
    }
    public class CustomerSalesVsCollection
    {
        public string CustomerDesc { get; set; }
        public string MasterCustomerCode { get; set; }
        public string PreCustomerCode { get; set; }
        public string GroupSkuFlag { get; set; }
        public double Sales { get; set; }
        public double Collection { get; set; }
        public double Opening { get; set; }
    }
    public class BrandWiseSales
    {
        public string Item { get; set; }
        public string Unit { get; set; }
        public double SalesQty { get; set; }
        public double SalesAmt { get; set; }
        public double SalesRetQty { get; set; }
        public double SalesRetAmt { get; set; }
        public double NetSalesQty { get; set; }
        public double NetSalesAmt { get; set; }
    }
    public class BrandWiseSalesReport
    {
        public List<BrandWiseSales> ProductWiseReport { get; set; }
        public List<string> Branches { get; set; }
        public double TotalSalesQty { get; set; }
        public double TotalSalesAmt { get; set; }
        public double TotalSalesRetQty { get; set; }
        public double TotalSalesRetAmt { get; set; }
        public double TotalNetSalesQty { get; set; }
        public double TotalNetSalesAmt { get; set; }
    }
    public class MonthlySales
    {
        public string Month { get; set; }
        public double Qty { get; set; }
        public double Amt { get; set; }
        public string Abbr { get; set; }
    }
    public class MonthlySalesReport
    {
        public string Month { get; set; }
        public double Qty { get; set; }
        public double Amt { get; set; }
        public List<MonthlySales> Details { get; set; }
        public double TotalAmt { get; set; }
        public double TotalQty { get; set; }
    }
    public class SalesCollectionResponse
    {
        public List<CustomerSalesVsCollection> Detail { get; set; }
        public double Total1 { get; set; }
        public double Total2 { get; set; }
        public double Total3 { get; set; }
    }
    public class MonthlySalesEng
    {
        public string Month { get; set; }
        public double Qty { get; set; }
        public double Amt { get; set; }
        public string Abbr { get; set; }
    }
    public class MonthlySalesReportEng
    {
        public string Month { get; set; }
        public double Qty { get; set; }
        public double Amt { get; set; }
        public List<MonthlySalesEng> Details { get; set; }
        public double TotalAmt { get; set; }
        public double TotalQty { get; set; }
    }
    //public class MonthlySalesData
    //{
    //    public string Month { get; set; }
    //    public float Qty { get; set; }
    //    public float Amt { get; set; }
    //    public string Abr { get; set; }
    //}
    public class SalesGraph
    {
        public TotalSales BranchWiseSales { get; set; }
        public TotalSales ComWiseSales { get; set; }
        public List<CompanyMonthlySales> ComMonthlyWiseSales { get; set; }
        public Dictionary<string, object> ComMonthlyWiseSalesTable { get; set; }
        public TotalSales BrandWiseSales { get; set; }
        public TotalSales CatWiseSales { get; set; }
        public List<SalesData> ItemWiseSales { get; set; }
    }
    public class TotalSales
    {
        public float TotalQty { get; set; }
        public float TotalAmt { get; set; }
        public List<SalesData> Details { get; set; }
    }
    public class CompanyMonthlySales
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Abbr { get; set; }
        public MonthlySalesData Detail { get; set; }
    }
    public class FinancialData
    {
        public string Topic { get; set; }
        public decimal Amount { get; set; }
    }
    public class CategorySummary
    {
        public string CategoryDesc { get; set; }
        public int Qty { get; set; }
        public decimal Amt { get; set; }
    }
    public class DirectExpenseDetail
    {
        public string Topic { get; set; }
        public decimal Amount { get; set; }
    }
    public class DirectExpenseSummary
    {
        public string Topic { get; set; }
        public decimal Amount { get; set; }
        public List<DirectExpenseDetail> Details { get; set; }
    }
    public class IndirectExpenseDetail
    {
        public string Topic { get; set; }
        public decimal Amount { get; set; }
    }
    public class IndirectExpenseSummary
    {
        public string Topic { get; set; }
        public decimal Amount { get; set; }
        public List<IndirectExpenseDetail> Details { get; set; }
    }
    public class LiabilityGroupDetail
    {
        public string Item { get; set; }
        public string GroupFlag { get; set; }
        public decimal Total { get; set; }
        public int Level { get; set; }
        public string Master { get; set; }
        public string Pre { get; set; }
        public string Code { get; set; }
    }
    public class LiabilityOverview
    {
        public List<LiabilityGroupDetail> Rep { get; set; }
        public decimal Total { get; set; }
        public string Name { get; set; }
    }
    public class AssetGroupDetail
    {
        public string Item { get; set; }
        public string GroupFlag { get; set; }
        public decimal Total { get; set; }
        public int Level { get; set; }
        public string Master { get; set; }
        public string Pre { get; set; }
        public string Code { get; set; }
    }
    public class AssetOverview
    {
        public List<AssetGroupDetail> Rep { get; set; }
        public decimal Total { get; set; }
        public string Name { get; set; }
    }
    public class SalesRecord
    {
        public string Title { get; set; }
        public double Sales { get; set; }
        public double Collection { get; set; }
        public string Abbr { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
    }
    public class Total
    {
        public double Sales { get; set; }
        public double Collection { get; set; }
        public string Abbr { get; set; }
    }
    public class SalesResponse
    {
        public List<SalesRecord> Detail { get; set; }
        public Total Total { get; set; }
        public double Total1 { get; set; }
        public double Total2 { get; set; }
        public double Total3 { get; set; }


    }
    public class ProductWiseReport
    {
        public List<ProductData> ProductWiseData { get; set; }
        public List<string> Branches { get; set; }
        public double TotalSalesQty { get; set; }
        public double TotalSalesAmt { get; set; }
        public double TotalSalesRetQty { get; set; }
        public double TotalSalesRetAmt { get; set; }
        public double TotalNetSalesQty { get; set; }
        public double TotalNetSalesAmt { get; set; }
    }
    public class ProductData
    {
        public string Item { get; set; }
        public string Unit { get; set; }
        public double SalesQty { get; set; }
        public double SalesAmt { get; set; }
        public double SalesRetQty { get; set; }
        public double SalesRetAmt { get; set; }
        public double NetSalesQty { get; set; }
        public double NetSalesAmt { get; set; }
    }
    #endregion
}
