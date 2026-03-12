//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web.Http;
//using System.Data;
//using System.Data.OracleClient;
//using System.Configuration;
//using System.Web.Mvc;
//namespace NeoErp.Controllers.Dashboard.Api
//{
//    public class StockReportController : ApiController
//    {
//        private readonly OracleConnection _conn;
//        private readonly string _dbUser;
//        private readonly string connection;
//        public StockReportController(string dbUser, string dbPass)
//        {
//            //var host = "your_host";
//            //var port = "your_port";
//            //var service = "your_service";
//            //var dsn = $"(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={host})(PORT={port})))(CONNECT_DATA=(SERVICE_NAME={service})))";
//            //_conn = new OracleConnection($"User Id={dbUser};Password={dbPass};Data Source={dsn};");
//            connection = ConfigurationManager.ConnectionString["NeoErpCoreEntity"].ToString(); _conn = new OracleConnection($"User Id={dbUser};Password={dbPass};Data Source={dsn};");
//            _conn = new OracleConnection(connection);
//            _dbUser = dbUser;
//            _conn.Open();
//        }
        
      
//        public ActionResult GetAllSubLedger(string companyCode)
//        {
//            var result = new Dictionary<string, object>();

//            // Query 1 - Fetching Categories
//            var categoryQuery = "SELECT category_code, CATEGORY_EDESC FROM IP_CATEGORY_CODE ORDER BY CATEGORY_EDESC ASC";
//            result["cat"] = ExecuteQueryToDict(categoryQuery);

//            // Query 2 - Fetching Item Groups
//            var itemGroupQuery = $"SELECT ITEM_EDESC, master_ITEM_CODE FROM IP_ITEM_MASTER_SETUP WHERE GROUP_SKU_FLAG = 'G' AND company_code = '{companyCode}' ORDER BY ITEM_EDESC";
//            result["grp"] = ExecuteQueryToDict(itemGroupQuery);

//            // Query 3 - Fetching Brands
//            var brandQuery = $"SELECT DISTINCT brand_name FROM ip_item_spec_setup WHERE brand_name IS NOT NULL AND brand_name NOT IN (' ') AND company_code = '{companyCode}' ORDER BY brand_name";
//            result["brand"] = ExecuteQueryToList(brandQuery);

//            // Query 5 - Fetching Customer Groups
//            var customerGroupQuery = $"SELECT CUSTOMER_EDESC, master_CUSTOMER_CODE FROM SA_CUSTOMER_SETUP WHERE GROUP_SKU_FLAG = 'G' AND company_code = '{companyCode}' ORDER BY CUSTOMER_EDESC";
//            result["cus_grp"] = ExecuteQueryToDict(customerGroupQuery);

//            // Query 6 - Fetching Supplier Groups
//            var supplierGroupQuery = $"SELECT supplier_edesc, master_supplier_code FROM ip_supplier_setup WHERE GROUP_SKU_FLAG = 'G' AND company_code = '{companyCode}' ORDER BY supplier_EDESC";
//            result["sup_grp"] = ExecuteQueryToDict(supplierGroupQuery);

//            // Query 4 - Fetching Units
//            var unitQuery = @"SELECT DISTINCT index_mu_code FROM 
//                              (SELECT item_code, index_mu_code, 1 ratio FROM ip_item_master_setup 
//                               WHERE index_mu_code IS NOT NULL AND index_mu_code != 'None' AND GROUP_SKU_FLAG = 'I' AND deleted_flag = 'N'
//                               UNION ALL
//                               SELECT item_code, mu_code, CASE WHEN conversion_factor = 0 THEN 0 ELSE NVL(conversion_factor, 0) / NVL(fraction, 0) END ratio 
//                               FROM IP_ITEM_UNIT_SETUP WHERE serial_no = '1' AND deleted_flag = 'N')";
//            result["unit"] = ExecuteQueryToList(unitQuery);
//            return Ok(result);
//        }

      
//        public ActionResult GetCatWiseGroup(string companyCode, string cat)
//        {
//            var query = $"SELECT ITEM_EDESC, master_ITEM_CODE FROM IP_ITEM_MASTER_SETUP WHERE GROUP_SKU_FLAG = 'G' AND company_code = '{companyCode}' AND category_code = '{cat}' ORDER BY ITEM_EDESC";
//            var result = ExecuteQueryToDict(query);
//            return Ok(new { grp = result });
//        }

       
//        public ActionResult GetCatWiseBrand(string companyCode, string cat)
//        {
//            var query = $@"
//                SELECT DISTINCT brand_name FROM 
//                (SELECT * FROM (SELECT item_code, brand_name FROM IP_ITEM_SPEC_SETUP WHERE company_code = '{companyCode}') a
//                 LEFT OUTER JOIN (SELECT item_code code, item_edesc, category_code, group_sku_flag, master_item_code, pre_item_code FROM ip_item_master_setup WHERE company_code = '{companyCode}') b
//                 ON a.item_code = b.code) WHERE GROUP_SKU_FLAG = 'G' AND category_code = '{cat}' AND brand_name IS NOT NULL ORDER BY brand_name";
//            var result = ExecuteQueryToList(query);
//            return Ok(result);
//        }

//        // Helper functions
//        private Dictionary<string, string> ExecuteQueryToDict(string query)
//        {
//            var result = new Dictionary<string, string>();
//            using (var cmd = new OracleCommand(query, _conn))
//            {
//                using (var reader = cmd.ExecuteReader())
//                {
//                    while (reader.Read())
//                    {
//                        result[reader.GetString(1)] = reader.GetString(0);
//                    }
//                }
//            }
//            return result;
//        }

//        private List<string> ExecuteQueryToList(string query)
//        {
//            var result = new List<string>();
//            using (var cmd = new OracleCommand(query, _conn))
//            {
//                using (var reader = cmd.ExecuteReader())
//                {
//                    while (reader.Read())
//                    {
//                        result.Add(reader.GetString(0));
//                    }
//                }
//            }
//            return result;
//        }
//    }
//}
