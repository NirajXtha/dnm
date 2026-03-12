using System;
using System.Collections.Generic;
using NeoErp.Services;
using NeoErp.Data;
using NeoErp.Core.Quotation;
using System.Web.Http;
using System.Web;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace NeoErp.Controllers.Api
{
    public class ApiQuotationController : ApiController
    {
        private IDbContext _dbContext;
        private IQuotation _quoTemplate;

        public ApiQuotationController(IDbContext dbContext, IQuotation quoTemplate)
        {
            this._dbContext = dbContext;
            _quoTemplate = quoTemplate;

        }
        public List<Quotation> GetQuotationDetails(string id)
        {
            List<Quotation> quotations = new List<Quotation>();
            try
            {
                quotations = _quoTemplate.GetQuotationDetails(id);
                return quotations;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        public List<Supplier> getSupplierDetails(string panNo, string supCode, string company)
        {
            List<Supplier> supplierDetails = _quoTemplate.GetSupplierDetails(panNo, supCode, company);
            return supplierDetails;
        }
        [HttpGet]
        public List<CurrencyQuotationModel> getCurrency(string company)
        {
            List<CurrencyQuotationModel> data = _quoTemplate.getCurrency(company);
            return data;
        }
        [HttpGet]
        public List<PartyDetails> partyDetails(string id)
        {
            List<PartyDetails> data = _quoTemplate.partyDetails(id);
            return data;
        }
        [HttpGet]
        public List<TermsAndConditions> termsAndConditions(string id, string supCode)
        {
            List<TermsAndConditions> data = _quoTemplate.termsAndConditions(id, supCode);
            return data;
        }
        [HttpGet]
        public List<PartyDetailsItems> partyDetailsItems(string id, string supCode)
        {
            List<PartyDetailsItems> data = _quoTemplate.partyDetailsItems(id, supCode);
            return data;
        }
        [HttpGet]
        public int? checkPAN(string panNo,string tenderNo, string supCode)
        {
            int? count = _quoTemplate.GetSupplierCount(panNo, tenderNo, supCode);
            return count;
        }
        public List<Company> GetCompanyDetails(string id)
        {
            List<Company> company = _quoTemplate.GetCompanyDetails(id);
            return company;
        }
        [HttpPost]
        public IHttpActionResult SaveFormData(Quotation_Details formData)
        {
            try
            {
                int? isPosted = _quoTemplate.InsertQuotationDetails(formData);

                if (isPosted.HasValue && isPosted > 0)
                {
                    return Ok(new { success = true, message = "Quotation Details saved successfully.", data = new { quotationNo = isPosted, tenderNo = formData.TENDER_NO } });
                }
                else
                {
                    return Ok(new { success = false, message = "Failed to save Quotation Details." });
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpGet]
        public List<FormDetailSetup> GetFormDetailSetup(string companyCode)
        {
            var thrdQty = 0;
            var sndQty = 0;
            var response = new List<FormDetailSetup>();
            var formDetailList = this._quoTemplate.GetFormDetailSetup(companyCode);
            var formCode = formDetailList[0].FORM_CODE;
            //response = formDetailList;
            var checkchargelist = $@"select * from charge_setup where form_code = '{formCode}' and company_code ='{companyCode}'";
            List<ChargeSetup> checkchargelistentity = this._dbContext.SqlQuery<ChargeSetup>(checkchargelist).ToList();

            if (formDetailList.Where(x => x.COLUMN_NAME == "THIRD_QUANTITY").Count() > 0)
                thrdQty = formDetailList.Where(x => x.COLUMN_NAME == "THIRD_QUANTITY").FirstOrDefault().SERIAL_NO = formDetailList.Where(x => x.COLUMN_NAME == "QUANTITY").Select(p => p.SERIAL_NO).FirstOrDefault();
            if (formDetailList.Where(x => x.COLUMN_NAME == "SECOND_QUANTITY").Count() > 0)
                sndQty = formDetailList.Where(x => x.COLUMN_NAME == "SECOND_QUANTITY").FirstOrDefault().SERIAL_NO = formDetailList.Where(x => x.COLUMN_NAME == "QUANTITY").Select(p => p.SERIAL_NO).FirstOrDefault();
            formDetailList.RemoveAll(x => checkchargelistentity.Any(t => t.CHARGE_CODE == x.COLUMN_NAME));
            string columname = "";
            columname = $@"select distinct ICH.charge_edesc,CH.charge_code,CH.charge_type_flag,CH.VALUE_PERCENT_FLAG
                                ,CH.VALUE_PERCENT_AMOUNT,CH.ON_ITEM,CH.priority_index_no,ch.manual_calc_charge from charge_setup CH
                                INNER JOIN ip_charge_code ICH ON ICH.charge_code = CH.charge_code
                                where CH.form_code = '{formCode}' and CH.company_code ='{companyCode}' and CH.ON_ITEM ='Y' order by CH.priority_index_no";

            List<IPChargeEdesc> columnameentity = this._dbContext.SqlQuery<IPChargeEdesc>(columname).ToList();

            for (int i = 0; i < columnameentity.Count; i++)
            {
                var columncharge = new FormDetailSetup();
                columncharge.SERIAL_NO = columnameentity[i].CHARGE_CODE == "VT" ? 37 :
                                         columnameentity[i].CHARGE_CODE == "DC" ? 33 :35;
                columncharge.DISPLAY_FLAG = "Y";
                columncharge.FORM_CODE = formCode;
                columncharge.DELETED_FLAG = "N";
                columncharge.COLUMN_WIDTH =100;
                columncharge.COLUMN_HEADER = columnameentity[i].CHARGE_EDESC;
                columncharge.COLUMN_NAME = columnameentity[i].CHARGE_CODE;
                columncharge.MASTER_CHILD_FLAG = "C";
                columncharge.LEFT_POSITION = 1600;
                columncharge.CHARGE_TYPE_FLAG = columnameentity[i].CHARGE_TYPE_FLAG;
                columncharge.VALUE_PERCENT_FLAG = columnameentity[i].VALUE_PERCENT_FLAG;
                columncharge.VALUE_PERCENT_AMOUNT = columnameentity[i].VALUE_PERCENT_AMOUNT;
                columncharge.MANUAL_CALC_CHARGE = columnameentity[i].MANUAL_CALC_CHARGE;
                columncharge.ON_ITEM = columnameentity[i].ON_ITEM;
                if (!formDetailList.Select(x => x.COLUMN_HEADER).Contains(columncharge.COLUMN_HEADER))
                    formDetailList.Add(columncharge);
            }
            response = formDetailList;
            return response;
        }
        [HttpGet]
        public List<COMMON_COLUMN> GetQuotationDetailByOrderno(string id,string companyCode)
        {
            var response = new List<COMMON_COLUMN>();
            var SalesOrderDetailFormDetailByFormCodeAndOrderNo = this._quoTemplate.GetQuestOrderFormDetail(id, companyCode);
            response = SalesOrderDetailFormDetailByFormCodeAndOrderNo;
            return response;
       }
    }
}
