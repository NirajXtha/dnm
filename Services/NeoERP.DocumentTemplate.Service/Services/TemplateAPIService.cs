using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeoErp.Core;
using NeoErp.Core.Models;
using NeoERP.DocumentTemplate.Service.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace NeoERP.DocumentTemplate.Service.Services
{
    public  class TemplateAPIService
    {
        private static IWorkContext _workContext;
        private static NeoErpCoreEntity _objectEntity;

        public TemplateAPIService(IWorkContext workContext)
        {
            _workContext = workContext;
            _objectEntity = new NeoErpCoreEntity();
           // _objectEntity = NeoErpCoreEntity.CreateForSelectedDatabase();
        }

        public static StringBuilder MapMasterColumn(JObject masterColumnList)
        {
            StringBuilder MasterColumnBuilder = new StringBuilder();
            if(masterColumnList.Count > 0)
            {
                foreach(var masterCol in masterColumnList)
                {
                    if(masterCol.Key.ToString() == "CURRENCY_CODE") MasterColumnBuilder.Append(masterCol.Key.ToString()).Append(",");
                    else if(masterCol.Key.ToString()== "EXCHANGE_RATE" ) MasterColumnBuilder.Append(masterCol.Key.ToString()).Append(",");
                    else MasterColumnBuilder.Append(masterCol.Key.ToString()).Append(",");
                }
            }
            return MasterColumnBuilder;
        }

        public static StringBuilder MapMasterColumn(string orderNumber,string voucherNoForEdit, string primaryDateColumn,string primaryColumnName,
            JObject masterColumn,out string primaryDate,out string primaryColumn,out string validation_voucher_date,out decimal exchangeRate)
        {
            StringBuilder valuesBuilder = new StringBuilder();
            string pDate = string.Empty, pColumn = string.Empty;

            dynamic salesObject = new JObject();

            foreach (var v in masterColumn)
            {
                if (v.Key == primaryDateColumn)
                {
                    pDate = v.Value.ToString();
                }
                if (v.Key == primaryColumnName)
                {
                    pColumn = v.Value.ToString();
                }
                string lastName = v.Key.Split('_').Last();
                if (lastName == "DATE")
                {
                    if (v.Value.ToString() == "")
                    {
                        valuesBuilder.Append("''").Append(",");
                    }
                    else
                    {
                        if (v.Key == primaryDateColumn)
                        {
                            //VoucherDate = "trunc(TO_DATE(" + "'" + v.Value + "'" + ",'DD-MON-YYYY hh24:mi:ss'))";
                            //validation_voucher_date = v.Value.ToString();

                            salesObject.VoucherDate = "trunc(TO_DATE(" + "'" + v.Value + "'" + ",'DD-MON-YYYY hh24:mi:ss'))";
                            salesObject.validation_voucher_date = v.Value.ToString();
                        }
                        valuesBuilder.Append("TO_DATE(" + "'" + v.Value + "'" + ",'DD-MON-YYYY hh24:mi:ss')").Append(",");
                    }
                }
                else if (v.Key.ToString() == "ORDER_NO" || v.Key.ToString() == "SALES_NO" || v.Key.ToString() == "CHALAN_NO" || v.Key.ToString() == "RETURN_NO")
                {
                    if (orderNumber == "undefined")
                    {
                        //valuesbuilder.Append("'" + newOrderNo + "'").Append(",");
                        if (voucherNoForEdit == "")
                        {
                            valuesBuilder.Append("'" + v.Value + "'").Append(",");
                            salesObject.newvoucherNo = v.Value.ToString();
                        }
                        else
                        {
                            valuesBuilder.Append("'" + voucherNoForEdit + "'").Append(",");
                            salesObject.newvoucherNo = voucherNoForEdit;
                        }
                    }
                    else
                    {
                        valuesBuilder.Append("'" + v.Value + "'").Append(",");
                    }
                }
                else if (v.Key.ToString() == "MANUAL_NO")
                {
                    valuesBuilder.Append("'" + v.Value + "'").Append(",");
                    salesObject.manualno = v.Value.ToString();
                }
                else if (v.Key.ToString() == "CURRENCY_CODE")
                {

                    valuesBuilder.Append("'" + v.Value + "'").Append(",");
                    salesObject.currencyformat = v.Value.ToString();
                }
                else if (v.Key.ToString() == "EXCHANGE_RATE")
                {
                    valuesBuilder.Append("'" + v.Value + "'").Append(",");
                    salesObject.exchangrate = Convert.ToDecimal(v.Value.ToString());
                }
                else { valuesBuilder.Append("'" + v.Value + "'").Append(","); }
            }

            primaryDate = pDate;
            primaryColumn = pColumn;
            validation_voucher_date = salesObject.validation_voucher_date;
            exchangeRate = salesObject.exchagerate;
            return valuesBuilder;
        }


        public static StringBuilder MapChildColumn(dynamic childcolumnvalues)
        {
            StringBuilder childvaluesbuilder = new StringBuilder();
            //StringBuilder masterchildvaluesbuilder = new StringBuilder();
            foreach (var item in childcolumnvalues)
            {
                var itemArray = JsonConvert.DeserializeObject(item.ToString());
                //var itemArray = item;
                foreach (var data in itemArray)
                {
                    var dataname = data.Name.ToString();
                    string[] datanamesplit = dataname.Split('_');
                    string datalastName = datanamesplit.Last();
                    var datavalue = data.Value;
                    if (datalastName == "DATE")
                    {
                        if (datavalue.Value.ToString() == "")
                        {
                            childvaluesbuilder.Append("''").Append(",");
                        }
                        else
                        {
                            childvaluesbuilder.Append("TO_DATE(" + "'" + datavalue.Value + "'" + ",'MM-DD-YYYY')").Append(",");
                        }
                    }
                    else if (datalastName == "PRICE")
                    {
                        if (datavalue.Value == null)
                        {
                            childvaluesbuilder.Append("''").Append(",");
                        }
                        else
                        {
                            childvaluesbuilder.Append(datavalue.Value).Append(",");
                        }
                    }
                    else if (datalastName == "QUANTITY")
                    {
                        try
                        {
                            datavalue.Value = Convert.ToDecimal(datavalue.Value);
                        }
                        catch (Exception ex)
                        {
                            datavalue.Value = null;
                        }
                        if (datavalue.Value == null)
                        {
                            childvaluesbuilder.Append("''").Append(",");
                        }
                        else
                        {
                            childvaluesbuilder.Append(datavalue.Value).Append(",");
                        }
                    }
                    else if (dataname == "MANUAL_NO")
                    {
                        if (datavalue.Value.ToString() == "")
                        {
                            childvaluesbuilder.Append("' '").Append(",");
                        }
                        else
                        {
                            childvaluesbuilder.Append("'" + datavalue.Value + "'").Append(",");
                        }
                    }
                    else if (dataname == "STOCK_BLOCK_FLAG")
                    {
                        if (datavalue.Value.ToString() == "")
                        {
                            childvaluesbuilder.Append("'N'").Append(",");
                        }
                        else
                        {
                            childvaluesbuilder.Append("'" + datavalue.Value + "'").Append(",");
                        }
                    }
                    else
                    {
                        if (datavalue.Value.ToString() == "")
                        {
                            childvaluesbuilder.Append("' '").Append(",");
                        }
                        else
                        {
                            childvaluesbuilder.Append("'" + datavalue.Value + "'").Append(",");
                        }
                    }
                }
            }
            return childvaluesbuilder;
        }

        public static void GetFormReference(FormDetails model,string primaryColumnName,string voucherNumberGeneratedNo)
        {
            var refInsQry = $@"SELECT * FROM {model.Table_Name} WHERE {primaryColumnName}='{voucherNumberGeneratedNo}'";
            var refResult = _objectEntity.SqlQuery<COMMON_COLUMN>(refInsQry).ToList();
            if (refResult.Count > 0)
            {
                var srNo = 1;
                foreach (var it in refResult)
                {
                    foreach (var item in model.REF_MODEL)
                    {
                        if (it.ITEM_CODE == item.ITEM_CODE)
                        {
                            var maxRefNoQry = $@"SELECT TO_CHAR(SYSDATE,'RRRRMMDD')||'.'||LPAD(MAX(REGEXP_SUBSTR(TRANSACTION_NO,'[^.]+', 1, 2))+1,11,'0')TRANSACTION_NO FROM REFERENCE_DETAIL";
                            var maxRefNo = _objectEntity.SqlQuery<string>(maxRefNoQry).FirstOrDefault();
                            var getRefFormCodeQry = $@"SELECT REF_FORM_CODE FROM FORM_SETUP WHERE FORM_CODE ='{model.Form_Code}' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                            var REF_FORM_CODE = _objectEntity.SqlQuery<string>(getRefFormCodeQry).FirstOrDefault();
                            var refInsQuery = $@"INSERT INTO REFERENCE_DETAIL (TRANSACTION_NO,VOUCHER_NO,FORM_CODE,COMPANY_CODE,SERIAL_NO,REFERENCE_NO,REFERENCE_FORM_CODE,REFERENCE_ITEM_CODE,
                                                     REFERENCE_QUANTITY,REFERENCE_MU_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,REFERENCE_UNIT_PRICE,REFERENCE_TOTAL_PRICE,REFERENCE_CALC_UNIT_PRICE,REFERENCE_CALC_TOTAL_PRICE,                                                        REFERENCE_REMARKS,REFERENCE_DATE,BRANCH_CODE,REFERENCE_BRANCH_CODE,REFERENCE_SERIAL_NO,SYN_ROWID,BATCH_NO,REFERENCE_BATCH_NO,VOUCHER_DATE) 
                                                   VALUES('{maxRefNo}','{voucherNumberGeneratedNo}','{model.Form_Code}','{_workContext.CurrentUserinformation.company_code}','{it.SERIAL_NO}','{item.VOUCHER_NO}','{REF_FORM_CODE}','{it.ITEM_CODE}','{it.QUANTITY}','{it.MU_CODE}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,
                                                          'N','{it.UNIT_PRICE}','{it.TOTAL_PRICE}','{it.CALC_UNIT_PRICE}','{it.CALC_TOTAL_PRICE}','{it.REMARKS}',SYSDATE,'{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.branch_code}','{srNo}','','','',SYSDATE)";
                            _objectEntity.ExecuteSqlCommand(refInsQuery);
                        }
                    }
                    srNo++;
                }
            }
        }

        public static void CalculateChargeForSalesOrder()
        {

        }

        public static void GetInv_Item_Charge_Value()
        {

        }
    }
}
