using NeoErp.Core;
using NeoErp.Data;
using NeoERP.DocumentTemplate.Service.Interface;
using NeoERP.DocumentTemplate.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Repository
{
    public class ValidationRepo : IValidationRepo
    {
        private IWorkContext _workcontext;
        private IDbContext _dbcontext;
        public ValidationRepo(IWorkContext workcontext, IDbContext dbcontext)
        {
            _workcontext = workcontext;
            _dbcontext = dbcontext;
        }

        public bool ValidateCreditLimiCustomerWise(string formcode, string customercode, decimal totalamount, bool isConsolidated, out decimal DrCrTotal, out decimal actualbalance, string companycode)
        {
           
            if (isConsolidated == true)
            {
                string Query = $@"SELECT CUSTOMER_CODE,CUSTOMER_EDESC,LINK_SUB_CODE,SUM(CREDIT_LIMIT) AS CREDIT_LIMIT,SUM(BALANCE) AS BALANCE,EXCEED_LIMT_PERCENTAGE
                    FROM(SELECT SUM(VS.DR_AMOUNT) AS DR_TOTAL,SUM(VS.CR_AMOUNT) AS CR_TOTAL,SA.CUSTOMER_CODE,
                    SA.CUSTOMER_EDESC,SA.LINK_SUB_CODE,SA.CREDIT_LIMIT,NVL(SA.EXCEED_LIMIT_PERCENTAGE,0) AS EXCEED_LIMT_PERCENTAGE,NVL((SUM (VS.DR_AMOUNT) - SUM (VS.CR_AMOUNT)),0) 
                    BALANCE FROM SA_CUSTOMER_SETUP SA 
                INNER JOIN V$VIRTUAL_SUB_LEDGER VS ON SA.LINK_SUB_CODE = VS.SUB_CODE AND SA.COMPANY_CODE=VS.COMPANY_CODE
                WHERE SA.CUSTOMER_CODE = '{customercode}'
                 GROUP BY SA.CUSTOMER_CODE,SA.CUSTOMER_EDESC,SA.LINK_SUB_CODE,SA.EXCEED_LIMIT_PERCENTAGE,SA.CREDIT_LIMIT)
                 GROUP BY CUSTOMER_CODE,LINK_SUB_CODE,EXCEED_LIMT_PERCENTAGE,CUSTOMER_EDESC";
                CreditLimitValidations entity = this._dbcontext.SqlQuery<CreditLimitValidations>(Query).FirstOrDefault();
                decimal totalAmt = entity.BALANCE;
                //DrCrTotal = totalAmt;
                int exceedlimitpercentage = entity.EXCEED_LIMIT_PERCENTAGE;
                string PdcQuery = $@"SELECT NVL(SUM(NVL(PDC_AMOUNT,0)),0) BalAmt FROM FA_PDC_RECEIPTS 
             WHERE CUSTOMER_CODE = '{customercode}' AND COMPANY_CODE IN(SELECT COMPANY_CODE FROM COMPANY_SETUP WHERE CONSOLIDATE_FLAG ='Y') AND (VG_DATE IS NULL OR VG_DATE='') AND (BOUNCE_FLAG='N' OR BOUNCE_FLAG IS NULL) 
                    AND TO_DATE(TO_CHAR(CHEQUE_DATE,'DD-MON-YYYY')) >=TO_DATE(SYSDATE)";
                decimal pdcamount = this._dbcontext.SqlQuery<decimal>(PdcQuery).FirstOrDefault();
                decimal CurrentAmount = totalAmt - pdcamount;
                string pendingSalesQuery = $@"SELECT NVL(SUM(NVL(DUE_QTY,0) * NVL(UNIT_PRICE,0)),0) BalAmt FROM V$SALES_ORDER_ANALYSIS 
                        WHERE CUSTOMER_CODE = '{customercode}'AND ORDER_NO NOT IN('01') AND COMPANY_CODE IN(SELECT COMPANY_CODE FROM COMPANY_SETUP WHERE CONSOLIDATE_FLAG ='Y')";
                decimal pendingsalesamount = this._dbcontext.SqlQuery<decimal>(pendingSalesQuery).FirstOrDefault();
                decimal dblAmount = CurrentAmount + pendingsalesamount;
                var extraCreditlimit = 0;
                if (exceedlimitpercentage > 0)
                {
                    extraCreditlimit = entity.CREDIT_LIMIT * exceedlimitpercentage / 100;
                }
                actualbalance = dblAmount + extraCreditlimit;
                DrCrTotal = actualbalance + totalamount;
                if (DrCrTotal > totalamount)
                {
                    return false;
                }
                else
                {
                    return true;
                }
                
            }
            else
            {
                string Query = $@"SELECT SUM(VS.DR_AMOUNT) AS DR_TOTAL,SUM(VS.CR_AMOUNT) AS CR_TOTAL,                           SA.CUSTOMER_CODE,SA.CUSTOMER_EDESC,SA.LINK_SUB_CODE,SA.CREDIT_LIMIT,NVL(SA.EXCEED_LIMIT_PERCENTAGE,0) AS EXCEED_LIMT_PERCENTAGE,NVL((SUM (VS.DR_AMOUNT) - SUM (VS.CR_AMOUNT)),0) BALANCE FROM SA_CUSTOMER_SETUP SA 
                LEFT JOIN V$VIRTUAL_SUB_LEDGER VS ON SA.LINK_SUB_CODE = VS.SUB_CODE AND SA.COMPANY_CODE=VS.COMPANY_CODE
                WHERE SA.CUSTOMER_CODE = '{customercode}' AND SA.COMPANY_CODE = '{companycode}' GROUP BY SA.CUSTOMER_CODE,SA.CUSTOMER_EDESC,SA.LINK_SUB_CODE,SA.CREDIT_LIMIT,SA.EXCEED_LIMIT_PERCENTAGE";
                CreditLimitValidations entity = this._dbcontext.SqlQuery<CreditLimitValidations>(Query).FirstOrDefault();
                decimal totalAmt = entity.BALANCE;
                //DrCrTotal = totalAmt;
                int exceedlimitpercentage = entity.EXCEED_LIMIT_PERCENTAGE;
                string PdcQuery = $@"SELECT NVL(SUM(NVL(PDC_AMOUNT,0)),0) BalAmt FROM FA_PDC_RECEIPTS 
                WHERE CUSTOMER_CODE = '{customercode}' AND COMPANY_CODE IN('{companycode}') AND (VG_DATE IS NULL OR VG_DATE='') AND (BOUNCE_FLAG='N' OR BOUNCE_FLAG IS NULL) 
                AND TO_DATE(TO_CHAR(CHEQUE_DATE,'DD-MON-YYYY')) >=TO_DATE(SYSDATE)";
                decimal pdcamount = this._dbcontext.SqlQuery<decimal>(PdcQuery).FirstOrDefault();
                decimal CurrentAmount = totalAmt - pdcamount;
                string pendingSalesQuery = $@"SELECT NVL(SUM(NVL(DUE_QTY,0) * NVL(UNIT_PRICE,0)),0) BalAmt FROM V$SALES_ORDER_ANALYSIS 
                        WHERE CUSTOMER_CODE = '{customercode}'AND ORDER_NO NOT IN('01') AND COMPANY_CODE = '{companycode}'";
                decimal pendingsalesamount = this._dbcontext.SqlQuery<decimal>(pendingSalesQuery).FirstOrDefault();
                decimal dblAmount = CurrentAmount + pendingsalesamount;
                var extraCreditlimit = 0;
                if (exceedlimitpercentage > 0)
                {
                    extraCreditlimit = entity.CREDIT_LIMIT * exceedlimitpercentage / 100;
                }
                actualbalance = dblAmount + extraCreditlimit;
                DrCrTotal = actualbalance + totalamount;
                if (DrCrTotal > totalamount)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
