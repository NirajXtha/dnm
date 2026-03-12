using NeoErp.Core;
using NeoErp.Core.Domain;
using NeoErp.Core.Helpers;
using NeoErp.Core.Models;
using NeoErp.Sales.Modules.Services.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Services.Consumption
{
    public class ConsumptionService : ReportJsonFilterAbstract, IConsumptionService
    {

        private NeoErpCoreEntity _objectEntity;
        private IWorkContext _workContext;

        public ConsumptionService(NeoErpCoreEntity objectEntity, IWorkContext workContext)
        {
            this._objectEntity = objectEntity;
            this._workContext = workContext;
        }


        public IEnumerable<ConsumptionIssueRegisterDetailModel> GetConsumptionIssueRegister(ReportFiltersModel reportFilters, User userInfo, bool liveData)
        {

            var companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }
            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            IEnumerable<ConsumptionIssueRegisterDetailModel> data = Enumerable.Empty<ConsumptionIssueRegisterDetailModel>();
            string path = System.Web.HttpContext.Current.Server.MapPath("~/App_Files/json/ConsumptionIssueRegister.json");
            if (liveData == false && File.Exists(path))
            {
                using (StreamReader r = new StreamReader(path))
                {
                    string jsonData = r.ReadToEnd();
                    data = Newtonsoft.Json.JsonConvert.DeserializeObject<EnumerableQuery<ConsumptionIssueRegisterDetailModel>>(jsonData);
                }
            }
            else
            {
                var Query = $@"select A.ISSUE_NO,TO_DATE(A.ISSUE_DATE) ISSUE_DATE,A.MANUAL_NO,A.ISSUE_TYPE_CODE,A.FROM_LOCATION_CODE,A.TO_LOCATION_CODE,A.TO_BUDGET_FLAG,
                                 A.ITEM_CODE,FN_FETCH_DESC(A.COMPANY_CODE,'IP_ITEM_MASTER_SETUP',A.ITEM_CODE) ITEM_EDESC,
                                 A.SERIAL_NO,A.MU_CODE,A.REQ_QUANTITY,A.QUANTITY,A.UNIT_PRICE,A.TOTAL_PRICE,A.CALC_QUANTITY,A.CALC_UNIT_PRICE,
                                 A.CALC_TOTAL_PRICE,A.COMPLETED_QUANTITY,A.REMARKS,A.FORM_CODE,A.CURRENCY_CODE,A.EXCHANGE_RATE,
                                 A.SYN_ROWID,A.SESSION_ROWID,A.BATCH_NO, 
                                 A.CUSTOMER_CODE,FN_FETCH_DESC(A.COMPANY_CODE,'SA_CUSTOMER_SETUP',A.CUSTOMER_CODE) CUSTOMER_EDESC,
                                 A.EMPLOYEE_CODE,E.EMPLOYEE_EDESC,
                                 A.SUPPLIER_CODE,FN_FETCH_DESC(A.COMPANY_CODE,'IP_SUPPLIER_SETUP',A.SUPPLIER_CODE) SUPPLIER_EDESC,
                                 A.DEPARTMENT_CODE,D.DEPARTMENT_EDESC,
                                 A.ISSUE_TO,A.DIVISION_CODE  ,B.DIVISION_EDESC,
                                A.REFERENCE_NO,A.ISSUE_SLIP_NO,
                                SUBSTR(FN_FETCH_GROUP_DESC(A.COMPANY_CODE,'IP_ITEM_MASTER_SETUP', T.PRE_ITEM_CODE),1,100) ITEM_GROUP_EDESC,
                                SUBSTR(FN_FETCH_PRE_DESC(A.COMPANY_CODE,'IP_ITEM_MASTER_SETUP', T.PRE_ITEM_CODE),1,100) ITEM_SUBGROUP_EDESC,
                                A.COMPANY_CODE, FN_FETCH_DESC(A.COMPANY_CODE,'COMPANY_SETUP',A.COMPANY_CODE) COMPANY_EDESC,
                                A.BRANCH_CODE,FN_FETCH_DESC(A.COMPANY_CODE,'FA_BRANCH_SETUP',A.BRANCH_CODE) BRANCH_EDESC
                                from IP_GOODS_ISSUE A,HR_EMPLOYEE_SETUP E,HR_DEPARTMENT_CODE D,FA_DIVISION_SETUP B ,IP_ITEM_MASTER_SETUP T  
                                WHERE 
                                A.COMPANY_CODE = E.COMPANY_CODE (+)
                                AND A.COMPANY_CODE = D.COMPANY_CODE (+)
                                AND A.COMPANY_CODE = B.COMPANY_CODE (+)
                                AND A.COMPANY_CODE = T.COMPANY_CODE (+)
                                AND A.EMPLOYEE_CODE = E.EMPLOYEE_CODE (+)
                                AND A.DEPARTMENT_CODE = D.DEPARTMENT_CODE (+)
                                AND A.DIVISION_CODE = B.DIVISION_CODE (+)
                                AND A.ITEM_CODE = T.ITEM_CODE (+)
                                AND A.DELETED_FLAG = 'N'
                                AND A.COMPANY_CODE IN({companyCode})";
                data = this._objectEntity.SqlQuery<ConsumptionIssueRegisterDetailModel>(Query).ToList();
                //save to jsonfile
                string file = System.Web.HttpContext.Current.Server.MapPath("~/App_Files/json/ConsumptionIssueRegister.json");
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
                //File.WriteAllText(file, json);

                //save property               
                //// var fs = File.GetAccessControl(file);
                //// var sid = fs.GetOwner(typeof(SecurityIdentifier));                
                //// var ntAccount = sid.Translate(typeof(NTAccount)).ToString();

                //// var setting = new GoodsReceiptNotesReportJsonSetting()
                //// {
                ////     SID = sid.ToString(),
                ////     CreatedBy = ntAccount,
                ////     CreatedDate = File.GetCreationTime(file),
                ////     ModifyDate = File.GetLastWriteTime(file)
                //// };               
                ////_setting.SaveSetting(setting);

            }



            //****************************
            //CONDITIONS FITLER START HERE
            //****************************

            //companyFilter
            data = data.Where(x => x.COMPANY_CODE.Contains(companyCode));

            //branchFilter
            if (reportFilters.BranchFilter.Count() > 0)
            {
                data = data.Where(x => reportFilters.BranchFilter.Contains(x.BRANCH_CODE));
            }


            //supplierFilter
            if (reportFilters.SupplierFilter.Count() > 0)
            {
                var selectedSupplier = getSelectedSuplierFromJsonData(_objectEntity, reportFilters);
                data = data.Where(x => selectedSupplier.Contains(x.SUPPLIER_CODE));

            }

            //productFilter
            if (reportFilters.ProductFilter.Count() > 0)
            {
                var selectedItems = getSelectedProductFromJsonData(_objectEntity, reportFilters);
                data = data.Where(x => selectedItems.Contains(x.ITEM_CODE));

            }

            //categoryFilter
            if (reportFilters.CategoryFilter.Count() > 0)
            {
                data = data.Where(x => reportFilters.CategoryFilter.Contains(x.CATEGORY_CODE));
            }




            //range filter
            int min = 0, max = 0;
            ReportFilterHelper.RangeFilterValue(reportFilters.QuantityRangeFilter, out min, out max);
            if (!(min == 0 && max == 0))
            {
                data = data.Where(x => x.TOTAL_PRICE >= min && x.TOTAL_PRICE <= max);
                data = data.Where(x => x.QUANTITY >= min && x.QUANTITY <= max);
            }




            //dateFilter
            if (!string.IsNullOrEmpty(reportFilters.FromDate))
            {
                DateTime fromDate = Convert.ToDateTime(reportFilters.FromDate);
                data = data.Where(x => x.ISSUE_DATE >= fromDate);
            }
            if (!string.IsNullOrEmpty(reportFilters.ToDate))
            {
                DateTime toDate = Convert.ToDateTime(reportFilters.ToDate);
                data = data.Where(x => x.ISSUE_DATE <= toDate);
            }

            //amountFormat
            var temp = ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter);


            ////****************************
            ////CONDITIONS FITLER END HERE
            ////****************************



            return data;
        }
    }
}
