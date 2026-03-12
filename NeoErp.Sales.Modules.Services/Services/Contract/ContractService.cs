using NeoErp.Core;
using NeoErp.Core.Helpers;
using NeoErp.Core.Models;
using NeoErp.Sales.Modules.Services.Models.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Services.Contract
{
    public class ContractService : IContractService
    {
        private NeoErpCoreEntity _objectEntity { get; set; }
        private IWorkContext _workContext { get; set; }
        public ContractService(NeoErpCoreEntity objectEntity, IWorkContext workContext)
        {
            this._objectEntity = objectEntity;
            this._workContext = workContext;

        }
        public List<ContractViewModel> GetAllContractInfo(filterOption model, string FincalYear)
        {
            var companyCode = string.Empty;
            foreach (var company in model.ReportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{_workContext.CurrentUserinformation.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            var contractTypeFilter = string.Empty;
            if (model.ReportFilters.ContractStatusTypeFilter.Count > 0)
            {
                foreach (var contractfilter in model.ReportFilters.ContractStatusTypeFilter)
                {
                    contractTypeFilter += string.Format("LOWER(TRIM('{0}'))", contractfilter) + ",";
                }
                contractTypeFilter = contractTypeFilter.TrimEnd(',');
            }


            var Query = string.Format(@"select SC.service_start_date,SC.EXPIRY_DATE,BS_DATE(service_start_date) as BsStartDate , SC.CONTRACT_NO,SC.CONTRACT_DATE,SC.CUSTOMER_CODE,C.CUSTOMER_EDESC,(SELECT CUSTOMER_EDESC FROM SA_CUSTOMER_SETUP WHERE MASTER_CUSTOMER_CODE=C.PRE_CUSTOMER_CODE AND DELETED_FLAG='N') AS PRE_CUSTOMER_EDESC
                        , SC.BANDWIDTH,SC.CONTRACT_TYPE ,SC.DISTRICT_CODE ,D.DISTRICT_EDESC
                        ,SC.CITY_CODE,CC.CITY_EDESC,SC.PAYMENT_LOCATION
                        ,SC.HOSTNAME,SC.STATUS_TYPE,SC.PAYMENT_BASIS,SC.REMARKS,SC.RE_PAYMENT_AMOUNT,
                        BS.BUSINESS_SECTOR,BS.BUSINESS_SECTOR_EDESC,C.CUSTOMER_ID 
                     
                         from sa_contract_info sc, sa_customer_setup C,DISTRICT_CODE D, CITY_CODE CC
                         , BUSINESS_SECTOR BS,HR_FISCAL_YEAR_CODE FY
                        WHERE C.DELETED_FLAG = 'N'
                        AND C.CUSTOMER_CODE = SC.CUSTOMER_CODE
                        AND SC.COMPANY_CODE = C.COMPANY_CODE
                        AND SC.DISTRICT_CODE = D.DISTRICT_CODE(+)
                        AND SC.CITY_CODE = CC.CITY_CODE(+)
                       
                        AND SC.BUSINESS_SECTOR = BS.BUSINESS_SECTOR
                           AND TRIM(FY.FISCAL_YEAR_CODE)=TRIM('" + FincalYear + @"')
                             AND SC.COMPANY_CODE=FY.COMPANY_CODE
                           AND BS.COMPANY_CODE=SC.COMPANY_CODE {0} AND SC.EXPIRY_DATE >= FY.START_DATE ", string.IsNullOrEmpty(contractTypeFilter) ? "" : " and lower(trim(sc.status_type)) in (" + contractTypeFilter + ") AND SC.COMPANY_CODE IN (" + companyCode + ") ");
            var data = _objectEntity.SqlQuery<ContractViewModel>(Query).ToList();
            return data;
        }
    }
}
