using System;
using NeoErp.Data;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.DynamicData;
using NeoErp.Core;
using NeoErp.LOC.Services.Models;

namespace NeoErp.LOC.Services.Services
{
    public class LOCService : ILOCService
    {
        private IDbContext _dbContext;
        private IWorkContext _workcontext;
        public LOCService(IDbContext dbContext, IWorkContext _iWorkContext)
        {
            this._workcontext = _iWorkContext;
            this._dbContext = dbContext;
        }                

        public List<ItemDetails> GetAllItemsByOrderCode(string OrderCode)
        {

            var sqlquery = $@"SELECT IPO.ORDER_NO, IPO.ITEM_CODE,IMS.ITEM_EDESC,IPO.CALC_QUANTITY,IPO.CALC_UNIT_PRICE,IPO.CALC_TOTAL_PRICE, IPO.MU_CODE,IPO.CURRENCY_CODE FROM IP_PURCHASE_ORDER IPO 
                JOIN IP_ITEM_MASTER_SETUP IMS ON IPO.ITEM_CODE = IMS.ITEM_CODE AND IPO.COMPANY_CODE  = IMS.COMPANY_CODE WHERE IPO.ORDER_NO = '{OrderCode}' AND IPO.DELETED_FLAG = 'N'  AND IPO.COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}'";
            var result = _dbContext.SqlQuery<ItemDetails>(sqlquery).ToList();
            return result;
        }

        public List<CountryModels> GetAllCountry(string filter)
      {
            if (string.IsNullOrEmpty(filter) || filter == "undefined")
            {
                filter = string.Empty;
            }
            var sqlquery = string.Format(@"SELECT COALESCE(COUNTRY_CODE ||'-'|| COUNTRY_EDESC,' ') COUNTRY_EDESC, COALESCE(COUNTRY_CODE,' ') COUNTRY_CODE FROM COUNTRY_SETUP WHERE (UPPER(COUNTRY_CODE) LIKE '%{0}%' OR UPPER(COUNTRY_EDESC) LIKE '%{0}%')", filter.ToUpperInvariant());
            var result = _dbContext.SqlQuery<CountryModels>(sqlquery).ToList();
            return result;
        }


         public List<SupplierModel> GetAllSuppliersByFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter) || filter == "undefined")
            {
                filter = string.Empty;
            }
            var sqlquery = string.Format(@"SELECT SUPPLIER_EDESC,SUPPLIER_CODE FROM ip_supplier_setup WHERE (UPPER(SUPPLIER_EDESC) LIKE '%{0}%' OR UPPER(SUPPLIER_CODE) LIKE '%{0}%')", filter.ToUpperInvariant());
            var result = _dbContext.SqlQuery<SupplierModel>(sqlquery).ToList();
            return result;
        }


        public List<HSModels> GetAllHsCodes(string filter)
      {
            if (string.IsNullOrEmpty(filter) || filter == "undefined")
            {
                filter = string.Empty;
            }
            var sqlquery = string.Format(@"SELECT COALESCE(HS_CODE,' ') HS_CODE, COALESCE(HS_EDESC,' ') HS_EDESC FROM LC_HS WHERE (UPPER(HS_CODE) LIKE '%{0}%' OR UPPER(HS_EDESC) LIKE '%{0}%') AND DELETED_FLAG='N'", filter.ToUpperInvariant());
            var result = _dbContext.SqlQuery<HSModels>(sqlquery).ToList();
            return result;
        }

        public List<BeneficiaryModels> GetAllBeneficiary(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                filter = string.Empty;
            }
            var sqlquery =$@"SELECT BNF_CODE, COALESCE(BNF_EDESC,' ') BNF_EDESC,ADDRESS FROM LC_BENEFICIARY WHERE (BNF_CODE LIKE '%{filter.ToUpperInvariant()}%' OR UPPER(BNF_EDESC) LIKE '%{filter.ToUpperInvariant()}%') AND DELETED_FLAG='N' AND COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}'";
            var result = _dbContext.SqlQuery<BeneficiaryModels>(sqlquery).ToList();
            return result;
        }


    }
}