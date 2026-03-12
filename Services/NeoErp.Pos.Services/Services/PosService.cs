
using NeoErp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Pos.Services.Services
{
    public class PosService : IPosService
    {
        private IDbContext _dbContext;

        public PosService(IDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public CustomerViewModel CreateCustomers(CustomerViewModel customerdetail)
        {

            var selectquery = "select max(CAST(CUSTOMER_CODE AS INT)) AS CustomerCode from SA_CUSTOMER_SETUP";
            var CustomerCode = _dbContext.SqlQuery<int>(selectquery).FirstOrDefault();
            CustomerCode = CustomerCode + 1;
            var insertQuery = string.Format(@"INSERT INTO SA_CUSTOMER_SETUP(CUSTOMER_CODE,CUSTOMER_EDESC, REGD_OFFICE_EADDRESS, TEL_MOBILE_NO1, TEL_MOBILE_NO2, EMAIL,GROUP_SKU_FLAG,COMPANY_CODE,CREATED_BY,MASTER_CUSTOMER_CODE,PRE_CUSTOMER_CODE,CREATED_DATE,DELETED_FLAG)VALUES('{0}', '{1}', '{2}', '{3}', '{4}','{5}','{6}','{7}','{8}','{9}','{10}',TO_DATE('{11}', 'mm/dd/yyyy hh24:mi:ss'),'{12}')", CustomerCode, customerdetail.CustomerName, customerdetail.Address, customerdetail.Mobile, customerdetail.Phone, customerdetail.Email, "I", "01", "ADMIN", "01.01.03.00", "01.01.03", DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss"), "N");
            var rowCount = _dbContext.ExecuteSqlCommand(insertQuery);
            _dbContext.SaveChanges();
            customerdetail.CustomerCode = CustomerCode.ToString();
            customerdetail.CustomerName = customerdetail.CustomerName;
            return customerdetail;
        }

        public List<CustomerViewModel> GetAllCustomers(string filter)
        {
            var sqlquery = "select  CUSTOMER_CODE as CustomerCode,CUSTOMER_EDESC as CustomerName from SA_CUSTOMER_SETUP where group_sku_flag='I' and deleted_flag='N'";
            sqlquery += "AND lower(CUSTOMER_EDESC) LIKE ('%" + filter + "%') OR lower(REGD_OFFICE_NADDRESS) LIKE ('%" + filter + "%')";
            var customers = _dbContext.SqlQuery<CustomerViewModel>(sqlquery).ToList();
            return customers;
        }

        public List<ProductViewModel> GetAllProducts()
        {
            var sqlquery = "select  Item_code as ProductCode,Item_edesc as ProductName,nvl(sales_price,10) as Price, IMAGE_FILE_NAME as ImageName  from ip_item_master_setup where group_sku_flag='I' and deleted_flag='N'";
            var products = _dbContext.SqlQuery<ProductViewModel>(sqlquery).ToList();
            return products;
        }

        public List<ProductViewModel> GetProductsByValue(string value)
        {
            var sqlquery = "select  Item_code as ProductCode,Item_edesc as ProductName,nvl(sales_price,10) as Price  from ip_item_master_setup where group_sku_flag='I' and deleted_flag='N'";
            sqlquery += "AND ITEM_CODE LIKE ('" + value + "') OR ITEM_EDESC LIKE ('" + value + "')";
            var products = _dbContext.SqlQuery<ProductViewModel>(sqlquery).ToList();
            return products;
        }

    }
}