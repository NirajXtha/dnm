using NeoErp.Core.Helpers;
using NeoErp.Core.Models.CustomModels;
using NeoErp.Data;
using NeoErp.Sales.Modules.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoErp.Core.Domain;

namespace NeoErp.Sales.Modules.Services.Services
{
    public class JournalVoucherService:IJournalVoucherService
    {

        private IDbContext _dbContext;

        public JournalVoucherService(IDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public List<JournalVoucherDataModel> GetJournalVoucher(filterOption model)
        {
            string query = string.Format(@"SELECT VBD.VOUCHER_DATE, VBD.ACC_CODE, VBD.ACC_EDESC, VBD.FORM_CODE, FS.FORM_EDESC, VBD.MANUAL_NO, VBD.VOUCHER_NO, VBD.IN_MU_CODE, VBD.OUT_MU_CODE, Round(NVL(VBD.DR_AMOUNT,0)/{2},{3}) DR_AMOUNT,
             ROUND(NVL(VBD.CR_AMOUNT,0)/{4},{5}) CR_AMOUNT,SERIAL_NO,VBD.PARTICULARS, VBD.REMARKS, NVL(VBD.QUANTITY,0) QUANTITY, BS_DATE(VBD.VOUCHER_DATE) MITI, ROUND(NVL(VBD.IN_QUANTITY,0)/{6},{7}) IN_QUANTITY, ROUND(NVL(VBD.OUT_QUANTITY,0)/{8},{9}) OUT_QUANTITY,VBD.TABLE_NAME FROM V$VIRTUAL_BOOK_DEFAULT VBD INNER JOIN FORM_SETUP FS
              on VBD.FORM_CODE = FS.FORM_CODE
              WHERE VBD.COMPANY_CODE ='01' 
                AND BRANCH_CODE='01.01' AND VOUCHER_DATE >= TO_DATE('{0}', 'yyyy-mm-dd') AND VOUCHER_DATE <= TO_DATE('{1}', 'yyyy-mm-dd') 
                ORDER BY VBD.VOUCHER_DATE,VBD.TABLE_NAME,VBD.FORM_CODE,VBD.VOUCHER_NO, VBD.DR_AMOUNT", model.ReportFilters.FromDate, model.ReportFilters.ToDate, ReportFilterHelper.FigureFilterValue(model.ReportFilters.AmountFigureFilter)
                ,ReportFilterHelper.RoundUpFilterValue(model.ReportFilters.AmountRoundUpFilter), ReportFilterHelper.FigureFilterValue(model.ReportFilters.AmountFigureFilter)
                , ReportFilterHelper.RoundUpFilterValue(model.ReportFilters.AmountRoundUpFilter),ReportFilterHelper.FigureFilterValue(model.ReportFilters.QuantityFigureFilter)
                ,ReportFilterHelper.RoundUpFilterValue(model.ReportFilters.QuantityRoundUpFilter),ReportFilterHelper.FigureFilterValue(model.ReportFilters.QuantityFigureFilter)
                , ReportFilterHelper.RoundUpFilterValue(model.ReportFilters.QuantityRoundUpFilter));
            var salesRegisterCustomers = this._dbContext.SqlQuery<JournalVoucherDataModel>(query).ToList();
            return salesRegisterCustomers;
        }

        public List<JournalVoucherDataModel> GetJournalVoucher(filterOption model, User userinfo)
        {
            //var companyCode = string.Join(",", model.ReportFilters.CompanyFilter);
            //if (string.IsNullOrEmpty(companyCode))
            //    companyCode = userinfo.company_code;
            var companyCode = string.Empty;
            foreach (var company in model.ReportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }
                
            companyCode = companyCode == "" ? $@"'{userinfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            //companyCode = companyCode == null ? userinfo.company_code : companyCode;
            //old one 
            //string query = string.Format(@"SELECT VBD.VOUCHER_DATE, VBD.ACC_CODE, VBD.ACC_EDESC, VBD.FORM_CODE, FS.FORM_EDESC, VBD.MANUAL_NO ,VBD.TRACKING_NO,VBD.VOUCHER_NO, VBD.IN_MU_CODE, VBD.OUT_MU_CODE, Round(NVL(FN_CONVERT_CURRENCY(NVL(VBD.DR_AMOUNT,0),'NRS',VBD.VOUCHER_DATE),0)/{2},{3}) DR_AMOUNT,
            // ROUND(NVL(FN_CONVERT_CURRENCY(NVL(VBD.CR_AMOUNT,0),'NRS',VBD.VOUCHER_DATE),0)/{4},{5}) CR_AMOUNT,SERIAL_NO,VBD.PARTICULARS, VBD.REMARKS, NVL(VBD.QUANTITY,0) QUANTITY, BS_DATE(VBD.VOUCHER_DATE) MITI, ROUND(NVL(VBD.IN_QUANTITY,0)/{6},{7}) IN_QUANTITY, ROUND(NVL(VBD.OUT_QUANTITY,0)/{8},{9}) OUT_QUANTITY,VBD.TABLE_NAME FROM V$VIRTUAL_BOOK_DEFAULT VBD INNER JOIN FORM_SETUP FS
            //  on VBD.FORM_CODE = FS.FORM_CODE
            //  WHERE VBD.COMPANY_CODE IN(" + companyCode+ @") AND FS.COMPANY_CODE IN(" + companyCode 

            //  +@") AND VOUCHER_DATE >= TO_DATE('{0}', 'yyyy-mm-dd') AND VOUCHER_DATE <= TO_DATE('{1}', 'yyyy-mm-dd')", model.ReportFilters.FromDate, model.ReportFilters.ToDate, ReportFilterHelper.FigureFilterValue(model.ReportFilters.AmountFigureFilter)
            //    , ReportFilterHelper.RoundUpFilterValue(model.ReportFilters.AmountRoundUpFilter), ReportFilterHelper.FigureFilterValue(model.ReportFilters.AmountFigureFilter)
            //    , ReportFilterHelper.RoundUpFilterValue(model.ReportFilters.AmountRoundUpFilter), ReportFilterHelper.FigureFilterValue(model.ReportFilters.QuantityFigureFilter)
            //    , ReportFilterHelper.RoundUpFilterValue(model.ReportFilters.QuantityRoundUpFilter), ReportFilterHelper.FigureFilterValue(model.ReportFilters.QuantityFigureFilter)
            //    , ReportFilterHelper.RoundUpFilterValue(model.ReportFilters.QuantityRoundUpFilter));


            //new one Removed VBD.TRACKING_NO
            string query = string.Format(@"SELECT VBD.VOUCHER_DATE, VBD.ACC_CODE, VBD.ACC_EDESC, VBD.FORM_CODE, FS.FORM_EDESC, VBD.MANUAL_NO ,VBD.VOUCHER_NO, VBD.IN_MU_CODE, VBD.OUT_MU_CODE, Round(NVL(FN_CONVERT_CURRENCY(NVL(VBD.DR_AMOUNT,0),'NRS',VBD.VOUCHER_DATE),0)/{2},{3}) DR_AMOUNT,
             ROUND(NVL(FN_CONVERT_CURRENCY(NVL(VBD.CR_AMOUNT,0),'NRS',VBD.VOUCHER_DATE),0)/{4},{5}) CR_AMOUNT,SERIAL_NO,VBD.PARTICULARS, VBD.REMARKS, NVL(VBD.QUANTITY,0) QUANTITY, BS_DATE(VBD.VOUCHER_DATE) MITI, ROUND(NVL(VBD.IN_QUANTITY,0)/{6},{7}) IN_QUANTITY, ROUND(NVL(VBD.OUT_QUANTITY,0)/{8},{9}) OUT_QUANTITY,VBD.TABLE_NAME FROM V$VIRTUAL_BOOK_DEFAULT VBD INNER JOIN FORM_SETUP FS
              on VBD.FORM_CODE = FS.FORM_CODE
              WHERE VBD.COMPANY_CODE IN(" + companyCode+ @") AND FS.COMPANY_CODE IN(" + companyCode 
              
              +@") AND VOUCHER_DATE >= TO_DATE('{0}', 'yyyy-mm-dd') AND VOUCHER_DATE <= TO_DATE('{1}', 'yyyy-mm-dd')", model.ReportFilters.FromDate, model.ReportFilters.ToDate, ReportFilterHelper.FigureFilterValue(model.ReportFilters.AmountFigureFilter)
                , ReportFilterHelper.RoundUpFilterValue(model.ReportFilters.AmountRoundUpFilter), ReportFilterHelper.FigureFilterValue(model.ReportFilters.AmountFigureFilter)
                , ReportFilterHelper.RoundUpFilterValue(model.ReportFilters.AmountRoundUpFilter), ReportFilterHelper.FigureFilterValue(model.ReportFilters.QuantityFigureFilter)
                , ReportFilterHelper.RoundUpFilterValue(model.ReportFilters.QuantityRoundUpFilter), ReportFilterHelper.FigureFilterValue(model.ReportFilters.QuantityFigureFilter)
                , ReportFilterHelper.RoundUpFilterValue(model.ReportFilters.QuantityRoundUpFilter));


            if (model.ReportFilters.BranchFilter.Count > 0)
            {
                query += string.Format(@" AND BRANCH_CODE IN ('{0}')", string.Join("','", model.ReportFilters.BranchFilter).ToString());
            }

            query += "ORDER BY VBD.VOUCHER_DATE,VBD.TABLE_NAME,VBD.FORM_CODE,VBD.VOUCHER_NO, VBD.DR_AMOUNT";

            var salesRegisterCustomers = this._dbContext.SqlQuery<JournalVoucherDataModel>(query).ToList();
            return salesRegisterCustomers;
        }

        public List<JournalVoucherDataModel> GetJournalsUBLEDGERVoucher(filterOption model, User userinfo)
        {
            //var companyCode = string.Join(",", model.ReportFilters.CompanyFilter);
            //if (string.IsNullOrEmpty(companyCode))
            //    companyCode = userinfo.company_code;
            var companyCode = string.Empty;
            foreach (var company in model.ReportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userinfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            //companyCode = companyCode == null ? userinfo.company_code : companyCode;
            var Query = $@"SELECT FN_FETCH_DESC(a.COMPANY_CODE,'FORM_SETUP',A.FORM_CODE) AS FORM_EDESC,a.form_code,A.VOUCHER_NO, A.VOUCHER_DATE, B.ACC_CODE, D.ACC_EDESC, B.PARTICULARS               ,  C.SUB_CODE, E.SUB_EDESC, C.DR_AMOUNT, C.CR_AMOUNT ,a.REFERENCE_NO AS MANUAL_NO,b.TRACKING_NO, b.serial_no

                                            FROM MASTER_TRANSACTION A, FA_DOUBLE_VOUCHER B, FA_VOUCHER_SUB_DETAIL C, FA_CHART_OF_ACCOUNTS_SETUP D, FA_SUB_LEDGER_SETUP E

                                            WHERE A.VOUCHER_NO = B.VOUCHER_NO

                                            AND B.VOUCHER_NO = C.VOUCHER_NO

                                            AND A.FORM_CODE = B.FORM_CODE

                                            AND A.FORM_CODE = C.FORM_CODE

                                            AND B.ACC_CODE = C.ACC_CODE

                                            AND B.SERIAL_NO = C.SERIAL_NO

                                            AND A.COMPANY_CODE = B.COMPANY_CODE

                                            AND B.COMPANY_CODE = C.COMPANY_CODE

                                            AND B.ACC_CODE = D.ACC_CODE

                                            AND B.COMPANY_CODE = D.COMPANY_CODE

                                            AND C.SUB_CODE = E.SUB_CODE

                                            AND C.COMPANY_CODE = E.COMPANY_CODE and
                                             A.COMPANY_CODE IN({companyCode}) AND A.COMPANY_CODE IN({companyCode}) AND A.VOUCHER_DATE >= TO_DATE('{model.ReportFilters.FromDate}', 'yyyy-mm-dd') AND A.VOUCHER_DATE <= TO_DATE('{model.ReportFilters.ToDate}', 'yyyy-mm-dd')   
                                               ";


            if (model.ReportFilters.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND A.BRANCH_CODE IN ('{0}')", string.Join("','", model.ReportFilters.BranchFilter).ToString());
            }

            Query += "ORDER BY A.VOUCHER_DATE,A.VOUCHER_NO, c.DR_AMOUNT";

            var salesRegisterCustomers = this._dbContext.SqlQuery<JournalVoucherDataModel>(Query).ToList();
            return salesRegisterCustomers;
        }
    }
}
