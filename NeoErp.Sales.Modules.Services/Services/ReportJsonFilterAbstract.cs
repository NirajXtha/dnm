using NeoErp.Core.Models;
using NeoErp.Sales.Modules.Services.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Services
{
    public abstract class ReportJsonFilterAbstract
    {         

        public IEnumerable<string> getSelectedCustomerFromJsonData(NeoErpCoreEntity _entity,ReportFiltersModel reportFilters)
        {
            string path = System.Web.HttpContext.Current.Server.MapPath("~/App_Files/json/SA_CUSTOMER_SETUP.json");
            IEnumerable<CustomerModel> customerData;
            if (File.Exists(path))
            {
                using (StreamReader r = new StreamReader(path))
                {
                    string jsonData = r.ReadToEnd();
                    customerData = Newtonsoft.Json.JsonConvert.DeserializeObject<EnumerableQuery<CustomerModel>>(jsonData);
                }

            }
            else
            {
                string query = $@"SELECT DISTINCT INITCAP(CS.CUSTOMER_EDESC)AS CustomerName, CS.CUSTOMER_CODE AS CustomerCode,
                                        CS.GROUP_SKU_FLAG, CS.MASTER_CUSTOMER_CODE, CS.PRE_CUSTOMER_CODE
                                        FROM SA_CUSTOMER_SETUP CS
                                        WHERE CS.DELETED_FLAG = 'N'                                           
                                        ORDER BY MASTER_CUSTOMER_CODE, PRE_CUSTOMER_CODE";
                customerData = _entity.SqlQuery<CustomerModel>(query);
                //save to jsonfile
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(customerData, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(System.Web.HttpContext.Current.Server.MapPath("~/App_Files/json/SA_CUSTOMER_SETUP.json"), json);
            }


            //now get selected customer data
            //first get value of i customer
            var custoemrFilter = customerData
                                      .Where(x => (x.GROUP_SKU_FLAG == "I" && reportFilters.CustomerFilter.Contains(x.CustomerCode)))
                                      .Select(x => x.CustomerCode);

            //get mastercode of g customer
            IEnumerable<string> getSelectedMastercustomerCode = customerData
                                        .Where(x => x.GROUP_SKU_FLAG == "G" && reportFilters.CustomerFilter.Contains(x.CustomerCode))
                                        .Select(x => x.MASTER_CUSTOMER_CODE);

            //now get value of g customer               
            var temp = customerData.Where(x => getSelectedMastercustomerCode.Any(y => x.MASTER_CUSTOMER_CODE.StartsWith(y)))
                         .Select(x => x.CustomerCode);
            if (temp.Count() > 0)
                custoemrFilter = custoemrFilter.Concat(temp).ToList();
            return custoemrFilter;

        }
        public IEnumerable<string> getSelectedSuplierFromJsonData(NeoErpCoreEntity _entity,ReportFiltersModel reportFilters)
        {
            string path = System.Web.HttpContext.Current.Server.MapPath("~/App_Files/json/IP_SUPPLIER_SETUP.json");
            IEnumerable<SupplierModel> supplierData;
            if (File.Exists(path))
            {
                using (StreamReader r = new StreamReader(path))
                {
                    string jsonData = r.ReadToEnd();
                    supplierData = Newtonsoft.Json.JsonConvert.DeserializeObject<EnumerableQuery<SupplierModel>>(jsonData);
                }

            }
            else
            {
                string query = @"SELECT LEVEL, COMPANY_CODE,DELETED_FLAG,
                                    INITCAP(SUPPLIER_EDESC) AS SupplierName,
                                    SUPPLIER_CODE AS SupplierCode,
                                    MASTER_SUPPLIER_CODE AS MasterItemCode, 
                                    PRE_SUPPLIER_CODE AS PreItemCode, 
                                    GROUP_SKU_FLAG AS GroupFlag
                                    FROM IP_SUPPLIER_SETUP fs
                                    WHERE fs.DELETED_FLAG = 'N'                                                                     
                                    START WITH PRE_SUPPLIER_CODE = '00'
                                    CONNECT BY PRIOR MASTER_SUPPLIER_CODE = PRE_SUPPLIER_CODE";
                supplierData = _entity.SqlQuery<SupplierModel>(query);
                //save to jsonfile
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(supplierData, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(System.Web.HttpContext.Current.Server.MapPath("~/App_Files/json/IP_SUPPLIER_SETUP.json"), json);
            }

            //now get selected supplier data
            //first get value of i supplier
            IEnumerable<string> supplierFilter = Enumerable.Empty<string>();
            if (reportFilters.SupplierFilter.Count() > 0)
            {
                supplierFilter = supplierData
                                      .Where(x => x.GroupFlag == "I" && reportFilters.SupplierFilter.Contains(x.SupplierCode))
                                      .Select(x => x.SupplierCode);

                //get mastercode of g supplier
                IEnumerable<string> getSelectedMastersupplierCode = supplierData
                                            .Where(x => x.GroupFlag == "G" && reportFilters.SupplierFilter.Contains(x.SupplierCode))
                                            .Select(x => x.MasterItemCode);

                //now get value of g supplier               
                var temp = supplierData.Where(x => getSelectedMastersupplierCode.Any(y => x.MasterItemCode.StartsWith(y)))
                             .Select(x => x.SupplierCode);
                if (temp.Count() > 0)
                    supplierFilter = supplierFilter.Concat(temp);
            }
            return supplierFilter;

        }
        public IEnumerable<string> getSelectedProductFromJsonData(NeoErpCoreEntity _entity,ReportFiltersModel reportFilters)
        {
            string path = System.Web.HttpContext.Current.Server.MapPath("~/App_Files/json/IP_ITEM_MASTER_SETUP.json");
            IEnumerable<ProductModel> productData;
            if (File.Exists(path))
            {
                using (StreamReader r = new StreamReader(path))
                {
                    string jsonData = r.ReadToEnd();
                    productData = Newtonsoft.Json.JsonConvert.DeserializeObject<EnumerableQuery<ProductModel>>(jsonData);
                }

            }
            else
            {
                string query = @"SELECT DISTINCT LEVEL, 
                                    INITCAP(ITEM_EDESC) AS ItemName,
                                    ITEM_CODE AS ItemCode,
                                    MASTER_ITEM_CODE AS MasterItemCode, 
                                    PRE_ITEM_CODE AS PreItemCode, 
                                    GROUP_SKU_FLAG AS GroupFlag
                                    FROM IP_ITEM_MASTER_SETUP ims
                                    WHERE ims.DELETED_FLAG = 'N'                      
                                    START WITH PRE_ITEM_CODE = '00'
                                    CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE";
                productData = _entity.SqlQuery<ProductModel>(query);
                //save to jsonfile
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(productData, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(System.Web.HttpContext.Current.Server.MapPath("~/App_Files/json/IP_ITEM_MASTER_SETUP.json"), json);
            }

            //now get selected product data
            //first get value of i product
            IEnumerable<string> productFilter =Enumerable.Empty<string>();
            if (reportFilters.ProductFilter.Count() > 0)
            {
                productFilter = productData
                                      .Where(x => x.GroupFlag == "I" && reportFilters.ProductFilter.Contains(x.ItemCode))
                                      .Select(x => x.ItemCode);

                //get mastercode of g product
                IEnumerable<string> getSelectedMasterproductCode = productData
                                            .Where(x => x.GroupFlag == "G" && reportFilters.ProductFilter.Contains(x.ItemCode))
                                            .Select(x => x.MasterItemCode);

                //now get value of g product               
                var temp = productData.Where(x => getSelectedMasterproductCode.Any(y => x.MasterItemCode.StartsWith(y)))
                             .Select(x => x.ItemCode).ToList();
                if (temp.Count() > 0)
                    productFilter = productFilter.Concat(temp);
            }
            return productFilter;

        }
    }
}
