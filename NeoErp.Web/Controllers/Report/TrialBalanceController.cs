using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NeoErp.Models.Report;
using NeoErp.Models;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using NeoErp.Models.Common;
using System.Data;
using NeoErp.Core;
using NeoErp.Core.Models;
using NeoErp.Data;
using NeoErp.Core.Caching;
using NeoErp.Models.Mobiles;

namespace NeoErp.Controllers.Report
{
    public class TrialBalanceController : Controller
    {
        private IWorkContext _workContext;
        private NeoErpCoreEntity _objectEntity;
        private IDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private IMenuModel _menuService;
       
        public TrialBalanceController(IWorkContext workContext,NeoErpCoreEntity objectEntity,IDbContext dbContext,ICacheManager cacheManager, IMenuModel menuModel)
        {
            this._workContext = workContext;
            this._objectEntity = objectEntity;
            this._dbContext = dbContext;
            this._cacheManager = cacheManager;
            this._menuService = menuModel;
        }
        public ActionResult TryTestTrialBalance()
        {
           
           // var abc = _objectEntity.COMPANY_USERS.ToList();
           
            if (Session["UserNo"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
          //  ViewData["ModuleMenu"] = _menuService.GetModule();
            return View();
        }
        public JsonResult TrialBalanceBarMasterGrid()
        {
           
            var jsonData = TrialBalanceModel.GetTrialBalanceGroupBy().AsQueryable();
            return Json(jsonData, JsonRequestBehavior.AllowGet);

        }
       // TrialBalanceModel.GetTrialBalanceSubGroupBy(id).AsQueryable()
        public JsonResult GetTrialBalanceBarMasterGrid()
        {
            string Query = @"

select account_head, TreeLevel, Pre_Acc_Code, Master_ACC_Code , acc_code, DR_OPENING, CR_OPENING, dr_amt, cr_amt , nvl(DR_OPENING,0) +  nvl(dr_amt,0) as DR_Closing, nvl(CR_OPENING,0) + nvl(cr_amt,0) as cr_closing, Child_rec from (
select account_head, TreeLevel, Pre_Acc_Code, Master_ACC_Code , acc_code,   
CASE WHEN dr_amt_opening-cr_amt_opening >= 0  THEN dr_amt_opening-cr_amt_opening 
    ELSE 0
end as DR_OPENING,
CASE WHEN dr_amt_opening-cr_amt_opening < 0  THEN ABS(dr_amt_opening-cr_amt_opening )
    ELSE 0
end as CR_OPENING,
dr_amt, cr_amt , Child_rec from(
select  (lpad(' ',2*(level-1)) || acc_edesc) account_head, LEVEL AS TreeLevel, Pre_Acc_Code, Master_ACC_Code , acc_code, (select count(*) from FA_CHART_OF_ACCOUNTS_SETUP where pre_acc_code = am.master_acc_code) as Child_rec ,
decode(
(
select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER
where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE < '01-Sep-2014'
), 
null,
(select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER
where  acc_code = am.acc_code  and VOUCHER_DATE < '01-Sep-2014')
,
(select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER
where  pre_acc_code like (am.master_acc_code||'%')  and VOUCHER_DATE < '01-Sep-2014')
)  dr_amt_opening,
decode
((select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER
where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE < '01-Sep-2014') 
,null,
(select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER
where   acc_code = am.acc_code and VOUCHER_DATE < '01-Sep-2014')
,
(select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER
where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE < '01-Sep-2014')
) cr_amt_opening,
decode(
(
select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER
where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE between '01-Sep-2014' and '31-Dec-2016' 
), 
null,
(select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER
where  acc_code = am.acc_code  and VOUCHER_DATE between '01-Sep-2014' and '31-Dec-2016' )
,
(select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER
where  pre_acc_code like (am.master_acc_code||'%')  and VOUCHER_DATE between '01-Sep-2014' and '31-Dec-2016' )
)  dr_amt ,
decode
((select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER
where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE between '01-Sep-2014' and '31-Dec-2016'  ) 
,null,
(select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER
where   acc_code = am.acc_code and VOUCHER_DATE between '01-Sep-2014' and '31-Dec-2016' )
,
(select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER
where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE between '01-Sep-2014' and '31-Dec-2016' )
) cr_amt
from FA_CHART_OF_ACCOUNTS_SETUP am
where deleted_flag = 'N'
and company_code = '01'
and pre_acc_code='00'
connect by prior master_acc_code =pre_acc_code
)
)";
           
          var jsonData =  _cacheManager.Get("NeoErp", () => _dbContext.SqlQuery<TrialBalanceViewModel>(Query).ToList());
           // var jsonData = TrialBalanceModel.GetTrialBalanceGroupBy().AsQueryable();
            return Json(jsonData, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetTrialBalanceBarGroup(string id)
        {
            if (id != null)
                id = id.Replace("__", "&");
            string Query = @"
select account_head, TreeLevel, Pre_Acc_Code, Master_ACC_Code , acc_code, DR_OPENING, CR_OPENING, dr_amt, cr_amt , nvl(DR_OPENING,0) +  nvl(dr_amt,0) as DR_Closing, nvl(CR_OPENING,0) + nvl(cr_amt,0) as cr_closing, Child_rec from (
select account_head, TreeLevel, Pre_Acc_Code, Master_ACC_Code , acc_code,   
CASE WHEN dr_amt_opening-cr_amt_opening >= 0  THEN dr_amt_opening-cr_amt_opening 
    ELSE 0
end as DR_OPENING,
CASE WHEN dr_amt_opening-cr_amt_opening < 0  THEN ABS(dr_amt_opening-cr_amt_opening )
    ELSE 0
end as CR_OPENING,
dr_amt, cr_amt , Child_rec from(
select  (lpad(' ',2*(level-1)) || acc_edesc) account_head, LEVEL AS TreeLevel, Pre_Acc_Code, Master_ACC_Code , acc_code, (select count(*) from FA_CHART_OF_ACCOUNTS_SETUP where pre_acc_code = am.master_acc_code) as Child_rec ,
decode(
(
select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER
where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE < '01-Sep-2014'
), 
null,
(select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER
where  acc_code = am.acc_code  and VOUCHER_DATE < '01-Sep-2014')
,
(select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER
where  pre_acc_code like (am.master_acc_code||'%')  and VOUCHER_DATE < '01-Sep-2014')
)  dr_amt_opening,
decode
((select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER
where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE < '01-Sep-2014') 
,null,
(select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER
where   acc_code = am.acc_code and VOUCHER_DATE < '01-Sep-2014')
,
(select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER
where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE < '01-Sep-2014')
) cr_amt_opening,
decode(
(
select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER
where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE between '01-Sep-2014' and '31-Dec-2016' 
), 
null,
(select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER
where  acc_code = am.acc_code  and VOUCHER_DATE between '01-Sep-2014' and '31-Dec-2016' )
,
(select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER
where  pre_acc_code like (am.master_acc_code||'%')  and VOUCHER_DATE between '01-Sep-2014' and '31-Dec-2016' )
)  dr_amt ,
decode
((select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER
where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE between '01-Sep-2014' and '31-Dec-2016'  ) 
,null,
(select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER
where   acc_code = am.acc_code and VOUCHER_DATE between '01-Sep-2014' and '31-Dec-2016' )
,
(select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER
where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE between '01-Sep-2014' and '31-Dec-2016' )
) cr_amt
from FA_CHART_OF_ACCOUNTS_SETUP am
where deleted_flag = 'N'
and company_code = '01'   
 and Pre_Acc_Code='" + id + @"' and Level=2
connect by prior master_acc_code =pre_acc_code
)
)";

           
           
            var jsonData = _cacheManager.Get("NeoErp", () => _dbContext.SqlQuery<TrialBalanceViewModel>(Query).ToList());
             // var jsonData = TrialBalanceModel.GetTrialBalanceSubGroupBy(id).AsQueryable();
            //var test = TrialBalanceModel.GetTrialBalanceDetails(id);
            return Json(jsonData, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Index()
        {
            if (Session["UserNo"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            ViewData["ModuleMenu"] = _menuService.GetModule();
            return View();
        }
        public ActionResult Tree()
        {
            ViewData["ModuleMenu"] = _menuService.GetModule();
            return View();
        }
        public ActionResult PopUp()
        {
            ViewData["ModuleMenu"] = _menuService.GetModule();
            return View();
        }
        public ActionResult GroupBy()
        {
            ViewData["ModuleMenu"] = _menuService.GetModule();
            return View();
        }
        public ActionResult Consolidated()
        {
            
            var userINfo = _workContext.CurrentUserinformation;
            ViewData["figureAmount"] = 1;
            ViewData["roundupAmount"] = "0.00";
            ViewData["ModuleMenu"] = _menuService.GetModule();
            return View();
        }
        public ActionResult Graph(string graphType)
        {
            ViewData["ModuleMenu"] = _menuService.GetModule();
            return View();
        }
        public JsonResult OnTrialBalanceGroupByRequested(int page, int rows, bool _search, string searchField, string searchOper, string searchString)//int page, int rows, bool _search, string searchField, string searchOper, string searchString)
        {
            var gridModel = new CustomerJQGridModel();
            return gridModel.CustomerGrid.DataBind(TrialBalanceModel.GetTrialBalanceGroupBy().AsQueryable());
        }

        public JsonResult TrialBalanceBarChart()
        {
            var jsonData = TrialBalanceModel.GetTrialBalanceGroupBy().AsQueryable();
            return Json(jsonData, JsonRequestBehavior.AllowGet);

        }
        public JsonResult OnTrialBalanceSubGroupByRequested(string id)
        {
            if (id != null)
                id = id.Replace("__", "&");
            var gridModel = new CustomerJQGridModel();
            return gridModel.CustomerGrid.DataBind(TrialBalanceModel.GetTrialBalanceSubGroupBy(id).AsQueryable());
        }
        public JsonResult OnTrialBalanceLedgerRequested(string id)
        {
            if (id != null)
                id = id.Replace("__", "&");
            var gridModel = new CustomerJQGridModel();
            return gridModel.CustomerGrid.DataBind(TrialBalanceModel.GetTrialBalanceLedger(id).AsQueryable());
        }

        public JsonResult OnTrialBalanceDetailRequested(string id)
        {
            if (id != null)
                id = id.Replace("__", "&");
            var gridModel = new CustomerJQGridModel();
            return gridModel.CustomerGrid.DataBind(TrialBalanceModel.GetTrialBalanceDetails(id).AsQueryable());
        }

        public JsonResult OnTrialBalanceVoucherRequested(string id)
        {
            if (id != null)
                id = id.Replace("__", "&");
            var gridModel = new CustomerJQGridModel();
            return gridModel.CustomerGrid.DataBind(TrialBalanceModel.GetTrialBalanceVoucher(id).AsQueryable());
        }
        public JsonResult TrialBalanceGroupByGridData()
        {
            var gridModel = new TrailBalanceGroupJQGridMOdel();
            return gridModel.TrailBalanceGroupJQGrid.DataBind(TrialBalanceModel.GetGroupByTrailBalanceList().AsQueryable());
        }
        public string TrialBalanceTreeGridData(string FromDate, string ToDate, string figureAmount, string roundupAmount)
        {
            
            switch (figureAmount)
            {
                case "lakh": ViewData["figureAmount"] = 100000;
                    Session["figureAmount"] = 100000;
                    break;
                case "thousand": ViewData["figureAmount"] = 1000;
                    Session["figureAmount"] = 1000;
                    break;
                case "crore": ViewData["figureAmount"] = 10000000;
                    Session["figureAmount"] = 10000000;
                    break;
                case "Actual": ViewData["figureAmount"] = 1;
                    Session["figureAmount"] = 1;
                    break;
            }
            ViewData["roundupAmount"] = roundupAmount;
            string Company = Session["Company"].ToString();
            string Branch = Session["Branch"].ToString();
            return TrialBalanceModel.GetTreeData(FromDate, ToDate, Company, Branch);
        }
        public JsonResult OnTrialBalanceDateWiseRequested(string Id, string From, string To,string figureAmount,string roundupAmount)
        {
            //if (Id != null)
            //    Id = Id.Replace("__", "&").Trim();
            //var gridModel = new CustomerJQGridModel();
            //string Company = Session["Company"].ToString();
            //string Branch = Session["Branch"].ToString();
            //return gridModel.CustomerGridDetails.DataBind(TrialBalanceModel.GetTrialBalanceDateWise(Id, From, To, Company, Branch).AsQueryable());
        //}




         string Company = Session["Company"].ToString();
             string Branch = Session["Branch"].ToString();
            if (Session["figureAmount"] == null)
            {
                Session["figureAmount"] = "1";
                Session["roundupAmount"] = "0.00";
            }
            var gridModel = new CustomerJQGridModel();
            figureAmount = Session["figureAmount"].ToString();
            if (Session["roundupAmount"] == null)
            {
              
                Session["roundupAmount"] = "0.00";
            }
            roundupAmount = Session["roundupAmount"].ToString();
            switch (figureAmount)
            {
                case "lakh": ViewData["figureAmount"] = 100000;
                    Session["figureAmount"] = 100000;
                    break;
                case "thousand": ViewData["figureAmount"] = 1000;
                    Session["figureAmount"] = 1000;
                    break;
                case "crore": ViewData["figureAmount"] = 10000000;
                    Session["figureAmount"] = 10000000;
                    break;
                case "Actual": ViewData["figureAmount"] = 1;
                    Session["figureAmount"] = 1;
                    break;

            }
            if (roundupAmount == "0")
            {
                roundupAmount = "0.0";
            }
            string a = Convert.ToString(ViewData["figureAmount"]);
            if (Id != null)
            {
                Session["Ledger"] = Id;
                Session["From"] = From;
                Session["To"] = To;
                Session["figureAmount"] = figureAmount;
                Session["roundupAmount"] = roundupAmount;
                if (Id != null && a != "")
                {
                    Session["Ledger"] = Id;
                    Session["From"] = From;
                    Session["To"] = To;
                    Session["figureAmount"] = a;
                    Session["roundupAmount"] = roundupAmount;
                }
            }
            try
            {
                if (Session["roundupAmount"] == null && Session["figureAmount"] == null)
                {
                    return gridModel.CustomerGridDetails.DataBind(TrialBalanceModel.GetTrialBalanceDateWise(Id, From, To, Company, Branch).AsQueryable());
                    //return gridModel.CustomerGridDetails.DataBind(TrialBalanceModel.GetTrialBalanceDateWise(Session["Ledger"].ToString(), Session["From"].ToString(), Session["To"].ToString(), Company, Branch).AsQueryable());
                }
                else
                {                    
                    return gridModel.CustomerGridDetails.DataBind(TrialBalanceModel.GetTrialBalanceDateWisePref(Id, From, To, Company, Branch, figureAmount, roundupAmount).AsQueryable());
                }
            }
            catch
            {
                return null;
            }
}
        public JsonResult OnTrialBalanceDateWiseDetailsRequested(string id, string figureAmount, string roundupAmount)
        {
            //if (id != null)
            //    id = id.Replace("__", "&").Trim();
            var gridModel = new CustomerJQGridModel();
            string Company = Session["Company"].ToString();
            string Branch = Session["Branch"].ToString();

            if (figureAmount == "actual")
            {
                Session["figureAmount"] = "Actual";
            }
            if (Session["figureAmount"] == null && id == null)
            {
                Session["figureAmount"] = "Actual";
                Session["roundupAmount"] = "0.00";
            }
            figureAmount = Session["figureAmount"].ToString();
            roundupAmount = Session["roundupAmount"].ToString();
            switch (figureAmount)
            {
                case "lakh": figureAmount = "100000";
                    Session["figureAmount"] = 100000;
                    break;
                case "thousand": figureAmount = "1000";
                    Session["figureAmount"] = 1000;
                    break;
                case "crore": figureAmount = "10000000";
                    Session["figureAmount"] = 10000000;
                    break;
                case "Actual": figureAmount = "1";
                    Session["figureAmount"] = 1;
                    break;

            }
            if (roundupAmount == "0")
            {
                roundupAmount = "0.0";
            }

            string a = Convert.ToString(figureAmount);
            if (id != null)
            {
                Session["Ledger"] = id;
                if (id != null && a != "")
                {
                    Session["Ledger"] = id;
                    Session["figureAmount"] = a;
                    Session["roundupAmount"] = roundupAmount;
                }
            }
            try
            {
                if (Session["roundupAmount"] == null && Session["figureAmount"] == null)
                {
                    return gridModel.CustomerGridVoucher.DataBind(TrialBalanceModel.GetTrialBalanceDateWiseDetails(id, Company, Branch).AsQueryable());
                }
                else
                {
                    return gridModel.CustomerGridVoucher.DataBind(TrialBalanceModel.GetTrialBalanceDateWiseDetailsPref(id, Company, Branch,a,roundupAmount).AsQueryable());
                }
            }
            catch
            {
                return null;
            }



            //return gridModel.CustomerGridVoucher.DataBind(TrialBalanceModel.GetTrialBalanceDateWiseDetails(id, Company, Branch).AsQueryable());
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult getfigdata(string prefCode)
        {
            IEnumerable<TrialBalanceModel> data = TrialBalanceModel.getfigdata(prefCode);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult getsubprefdata(string prefCode)
        {
            IEnumerable<TrialBalanceModel> data = TrialBalanceModel.getsubprefdata(prefCode);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult getledgerprefdata(string prefCode)
        {
            IEnumerable<TrialBalanceModel> data = TrialBalanceModel.getledgerprefdata(prefCode);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //public string getfigdata(string prefCode)
        //{getledgerprefdata
        //    //return TrialBalanceModel.getfigdata(prefCode);
        //    return TrialBalanceModel.getfigdata(prefCode);
        //}
        public void ExportToExcel(string id, string level, string Type)
        {
            GridView gv = new GridView();
            gv.DataSource = TrialBalanceExportModel.GetExportToExcelData(id, level);
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            if (Type.Equals("xls"))
            {
                Response.AddHeader("content-disposition", "attachment; filename=TrialBalance.xls");
                Response.ContentType = "application/ms-excel";
            }
            else
            {
                Response.AddHeader("content-disposition", "attachment; filename=TrialBalance.doc");
                Response.ContentType = "application/ms-word";
            }
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
        }
        public ActionResult SubLedgerSummary()
        {
            ViewData["ModuleMenu"] = _menuService.GetModule();
            return View();
        }
        public string OnSubLedgerRequested(string Id, string From, string To, string figureAmount, string roundupAmount)
        {
            string Company = Session["Company"].ToString();
            string Branch = Session["Branch"].ToString();
            switch (figureAmount)
            {
                case "lakh": ViewData["figureAmount"] = 100000;
                    Session["figureAmount"] = 100000;
                    break;
                case "thousand": ViewData["figureAmount"] = 1000;
                    Session["figureAmount"] = 1000;
                    break;
                case "crore": ViewData["figureAmount"] = 10000000;
                    Session["figureAmount"] = 10000000;
                    break;
                case "Actual": ViewData["figureAmount"] = 1;
                    Session["figureAmount"] = 1;
                    break;

            }
            switch (figureAmount)
            {
                case "100000": ViewData["figureAmount"] = 100000;
                    Session["figureAmount"] = 100000;
                    break;
                case "1000": ViewData["figureAmount"] = 1000;
                    Session["figureAmount"] = 1000;
                    break;
                case "10000000": ViewData["figureAmount"] = 10000000;
                    Session["figureAmount"] = 10000000;
                    break;
                case "1": ViewData["figureAmount"] = 1;
                    Session["figureAmount"] = 1;
                    break;

            }
            if (roundupAmount == "0")
            {
                roundupAmount = "0.0";
            }
            string a = Convert.ToString(ViewData["figureAmount"]);
            if (Id != null)
            {
                Session["Ledger"] = Id;
                Session["From"] = From;
                Session["To"] = To;
                //Session["figureAmount"] = figureAmount;
                //Session["roundupAmount"] = roundupAmount;
                if (Id != null && a != "")
                {
                    Session["Ledger"] = Id;
                    Session["From"] = From;
                    Session["To"] = To;
                    Session["figureAmount"] = a;
                    Session["roundupAmount"] = roundupAmount;
                }
            }
            try
            {
                if (Session["roundupAmount"] == null && Session["figureAmount"] == null)
                {
                    return TrialBalanceModel.GetSubLedger(Session["Ledger"].ToString(), Session["From"].ToString(), Session["To"].ToString(), Company, Branch);
                    //return gridModel.CustomerGridDetails.DataBind(TrialBalanceModel.GetTrialBalanceDateWise(Session["Ledger"].ToString(), Session["From"].ToString(), Session["To"].ToString(), Company, Branch).AsQueryable());
                }
                else
                {
                    return TrialBalanceModel.GetSubLedgerPref(Session["Ledger"].ToString(), Session["From"].ToString(), Session["To"].ToString(), Company, Branch, Session["figureAmount"].ToString(), Session["roundupAmount"].ToString());
                }
            }
            catch
            {
                return null;
            }
           // return TrialBalanceModel.GetSubLedger(Session["Ledger"].ToString(), Session["From"].ToString(), Session["To"].ToString(), Company, Branch);
        }
        public string OnDailySummaryRequested(string Id, string From, string To, string figureAmount, string roundupAmount)
        {
            string Company = Session["Company"].ToString();
            string Branch = Session["Branch"].ToString();
            switch (figureAmount)
            {
                case "lakh": ViewData["figureAmount"] = 100000;
                    Session["figureAmount"] = 100000;
                    break;
                case "thousand": ViewData["figureAmount"] = 1000;
                    Session["figureAmount"] = 1000;
                    break;
                case "crore": ViewData["figureAmount"] = 10000000;
                    Session["figureAmount"] = 10000000;
                    break;
                case "Actual": ViewData["figureAmount"] = 1;
                    Session["figureAmount"] = 1;
                    break;

            }
            switch (figureAmount)
            {
                case "100000": ViewData["figureAmount"] = 100000;
                    Session["figureAmount"] = 100000;
                    break;
                case "1000": ViewData["figureAmount"] = 1000;
                    Session["figureAmount"] = 1000;
                    break;
                case "10000000": ViewData["figureAmount"] = 10000000;
                    Session["figureAmount"] = 10000000;
                    break;
                case "1": ViewData["figureAmount"] = 1;
                    Session["figureAmount"] = 1;
                    break;

            }
            if (roundupAmount == "0")
            {
                roundupAmount = "0.0";
            }
            string a = Convert.ToString(ViewData["figureAmount"]);
            if (Id != null)
            {
                Session["Ledger"] = Id;
                Session["From"] = From;
                Session["To"] = To;
                //Session["figureAmount"] = figureAmount;
                //Session["roundupAmount"] = roundupAmount;
                if (Id != null && a != "")
                {
                    Session["Ledger"] = Id;
                    Session["From"] = From;
                    Session["To"] = To;
                    Session["figureAmount"] = a;
                    Session["roundupAmount"] = roundupAmount;
                }
            }
            try
            {
                if (Session["roundupAmount"] == null && Session["figureAmount"] == null)
                {
                    return TrialBalanceModel.GetDailySummaryStatement(Session["Ledger"].ToString(), Session["From"].ToString(), Session["To"].ToString(), Company, Branch);
                    //return gridModel.CustomerGridDetails.DataBind(TrialBalanceModel.GetTrialBalanceDateWise(Session["Ledger"].ToString(), Session["From"].ToString(), Session["To"].ToString(), Company, Branch).AsQueryable());
                }
                else
                {
                    return TrialBalanceModel.GetDailySummaryStatementPref(Session["Ledger"].ToString(), Session["From"].ToString(), Session["To"].ToString(), Company, Branch, Session["figureAmount"].ToString(), Session["roundupAmount"].ToString());
                }
            }
            catch
            {
                return null;
            }
        }
        public string GetLedgerName(string Id, string From, string To)
        {
            if (Id != null)
            {
                Session["Ledger"] = Id;
            }
            return TrialBalanceModel.GetLedgerName(Session["Ledger"].ToString());
        }
        public ActionResult LedgerStatement()
        {
            string figureAmount=null;
            string roundupAmount=null;
            ViewData["ModuleMenu"] = _menuService.GetModule();
            string AccountHead = Request.QueryString["Ah"];
            if (AccountHead == "1")
            {
                AccountHead = null;
            }
            if (AccountHead != null)
            {
                if (figureAmount == null)
                {
                    Session["figureAmount"] = "1";
                    Session["roundupAmount"] = "0.00";
                }
               figureAmount=Session["figureAmount"].ToString();
               roundupAmount = Session["roundupAmount"].ToString();
                Session["Ledger"] = AccountHead;
                Session["From"] = Request.QueryString["F"];
                Session["To"] = Request.QueryString["T"];
                string a = Session["Ledger"].ToString();
                string b = Session["From"].ToString();
                string c = Session["To"].ToString();
                 var gridModel = new CustomerJQGridModel();
                    string Company = Session["Company"].ToString();
                    string Branch = Session["Branch"].ToString();
                    if (figureAmount == null)
                    {
                        return View(gridModel.CustomerGridDetails.DataBind(TrialBalanceModel.GetTrialBalanceDateWise(Session["Ledger"].ToString(), Session["From"].ToString(), Session["To"].ToString(), Company, Branch).AsQueryable()));
                    }
                    else
                    {
                        return View(gridModel.CustomerGridDetails.DataBind(TrialBalanceModel.GetTrialBalanceDateWisePref(Session["Ledger"].ToString(), Session["From"].ToString(), Session["To"].ToString(), Company, Branch, figureAmount, roundupAmount).AsQueryable()));

                    }
                
                
            }
            else
                return View();
        }
        public ActionResult MonthwiseSummary()
        {
            string id = Request.QueryString["Ah"];
            
            if (id != null)
            {
                Session["Id"] = id;
                Session["FromDate"] = Request.QueryString["F"];
                Session["ToDate"] = Request.QueryString["T"];
               
            }
            if (!string.IsNullOrEmpty(Session["FromDate"] as string) && !string.IsNullOrEmpty(Session["ToDate"] as string))
            {
                string fromDate = Session["FromDate"].ToString();
                string toDate = Session["ToDate"].ToString(); 
                id = Session["Id"].ToString();
                if (Session["roundupAmount"] == null)
                {
                    ViewData["roundupAmount"] = "0.00";
                    ViewData["figureAmount"] = 1;
                }
                else {
                    ViewData["roundupAmount"] = Session["roundupAmount"].ToString();
                    ViewData["figureAmount"] = Session["figureAmount"].ToString();
                }
                
                ViewData["FromDate"] = Convert.ToDateTime(fromDate).ToString("yyyy-M-dd");
                ViewData["ToDate"] = Convert.ToDateTime(toDate).ToString("yyyy-M-dd");
                DataTable monthlyData = new DataTable();
                monthlyData = TrialBalanceModel.GetMonthwiseSummary(fromDate, toDate, id);
                if (monthlyData.Rows.Count<1)
                {
                    ViewData.Model = null;
                }
                else {
                    ViewData.Model = monthlyData.AsEnumerable();
                }
            }
            ViewData["ModuleMenu"] = _menuService.GetModule();
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthwiseSummary(string From, string To, string Id, string figureAmount, string roundupAmount)
        {
            //Session["FromDate"] = From;
            //Session["ToDate"] = To;
            //Session["Id"] = Id;
            //return View();

            Session["FromDate"] = From;
            Session["ToDate"] = To;
            Session["Id"] = Id;
            ViewData["Id"] = Id;
            switch (figureAmount)
            {
                case "lakh": ViewData["figureAmount"] = 100000;
                    Session["figureAmount"] = 100000;
                    break;
                case "thousand": ViewData["figureAmount"] = 1000;
                    Session["figureAmount"] = 1000;
                    break;
                case "crore": ViewData["figureAmount"] = 10000000;
                    Session["figureAmount"] = 10000000;
                    break;
                case "Actual": ViewData["figureAmount"] = 1;
                    Session["figureAmount"] = 1;
                    break;

            }
            Session["roundupAmount"] = roundupAmount;
            ViewData["roundupAmount"] = roundupAmount;
            ViewData["FromDate"] = Convert.ToDateTime(From).ToString("yyyy-M-dd");
            ViewData["ToDate"] = Convert.ToDateTime(To).ToString("yyyy-M-dd");
            DataTable monthlyData = new DataTable();
            monthlyData = TrialBalanceModel.GetMonthwiseSummary(From, To, Id);
            ViewData.Model = monthlyData.AsEnumerable();
            ViewData["ModuleMenu"] = _menuService.GetModule();
            return View();
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            return RedirectToAction(returnUrl, "TrialBalance");
        }
        public ActionResult DailySummaryReport()
        {
            ViewData["ModuleMenu"] = _menuService.GetModule();
            return View();
        }
        public JsonResult OnLedgerRequested(string Id, string From, string To, string figureAmount,string roundupAmount)
        {
            var gridModel = new CustomerJQGridModel();
            string Company = Session["Company"].ToString();
            string Branch = Session["Branch"].ToString();
            switch (figureAmount)
            {
                case "lakh": ViewData["figureAmount"] = 100000;
                    Session["figureAmount"] = 100000;
                    break;
                case "thousand": ViewData["figureAmount"] = 1000;
                    Session["figureAmount"] = 1000;
                    break;
                case "crore": ViewData["figureAmount"] = 10000000;
                    Session["figureAmount"] = 10000000;
                    break;
                case "Actual": ViewData["figureAmount"] = 1;
                    Session["figureAmount"] = 1;
                    break;

            }
            switch (figureAmount)
            {
                case "100000": ViewData["figureAmount"] = 100000;
                    Session["figureAmount"] = 100000;
                    break;
                case "1000": ViewData["figureAmount"] = 1000;
                    Session["figureAmount"] = 1000;
                    break;
                case "10000000": ViewData["figureAmount"] = 10000000;
                    Session["figureAmount"] = 10000000;
                    break;
                case "1": ViewData["figureAmount"] = 1;
                    Session["figureAmount"] = 1;
                    break;

            }
            if (roundupAmount == "0")
            {
                roundupAmount = "0.0";
            }
            
            string a = Convert.ToString(ViewData["figureAmount"]);
            //if (figureAmount == null && roundupAmount == null)
            //{
            //    //figureAmount = Session["figureAmount"].ToString();
            //    //roundupAmount = Session["roundupAmount"].ToString();
            //    ViewData["roundupAmount"] = Session["roundupAmount"].ToString();
            //}
           // roundupAmount=Session["roundupAmount"].ToString();

            if (Id != null)
            {
                Session["Ledger"] = Id;
                Session["From"] = From;
                Session["To"] = To;
                if (Id != null && a != "")
                {
                    Session["Ledger"] = Id;
                    Session["From"] = From;
                    Session["To"] = To;
                    Session["figureAmount"] = a;
                    Session["roundupAmount"] = roundupAmount;
                }
            }
            try
            {
                if (Session["roundupAmount"] == null && Session["figureAmount"] == null)
                {
                    return gridModel.CustomerGridDetails.DataBind(TrialBalanceModel.GetTrialBalanceDateWise(Session["Ledger"].ToString(), Session["From"].ToString(), Session["To"].ToString(), Company, Branch).AsQueryable());
                }
                else
                {
                    return gridModel.CustomerGridDetails.DataBind(TrialBalanceModel.GetTrialBalanceDateWisePref(Session["Ledger"].ToString(), Session["From"].ToString(), Session["To"].ToString(), Company, Branch, Session["figureAmount"].ToString(), Session["roundupAmount"].ToString()).AsQueryable());
                }
                }
            catch
            {
                return null;
            }
        }
        public JsonResult OnSubLedgerDetailRequested(string Id, string From, string To,string figureAmount,string roundupAmount)
        {
            string Company = Session["Company"].ToString();
            string Branch = Session["Branch"].ToString();
            if (Session["figureAmount"] == null)
            {
                Session["figureAmount"] = "1";
                Session["roundupAmount"] = "0.00";
            }
            var gridModel = new CustomerJQGridModel();
            figureAmount = Session["figureAmount"].ToString();
            roundupAmount = Session["roundupAmount"].ToString();
            switch (figureAmount)
            {
                case "lakh": ViewData["figureAmount"] = 100000;
                    Session["figureAmount"] = 100000;
                    break;
                case "thousand": ViewData["figureAmount"] = 1000;
                    Session["figureAmount"] = 1000;
                    break;
                case "crore": ViewData["figureAmount"] = 10000000;
                    Session["figureAmount"] = 10000000;
                    break;
                case "Actual": ViewData["figureAmount"] = 1;
                    Session["figureAmount"] = 1;
                    break;

            }
            if (roundupAmount == "0")
            {
                roundupAmount = "0.0";
            }
            string a = Convert.ToString(ViewData["figureAmount"]);
            if (Id != null)
            {
                Session["Ledger"] = Id;
                Session["From"] = From;
                Session["To"] = To;
                Session["figureAmount"] = figureAmount;
                Session["roundupAmount"] = roundupAmount;
                if (Id != null && a != "")
                {
                    Session["Ledger"] = Id;
                    Session["From"] = From;
                    Session["To"] = To;
                    Session["figureAmount"] = a;
                    Session["roundupAmount"] = roundupAmount;
                }
            }
            try
            {
                if (Session["roundupAmount"] == null && Session["figureAmount"] == null)
                {
                    return gridModel.CustomerGridDetails.DataBind(TrialBalanceModel.GetSubLedgerDateWise(Id, From, To, Company, Branch).AsQueryable());
                    //return gridModel.CustomerGridDetails.DataBind(TrialBalanceModel.GetTrialBalanceDateWise(Session["Ledger"].ToString(), Session["From"].ToString(), Session["To"].ToString(), Company, Branch).AsQueryable());
                }
                else
                {
                    return gridModel.CustomerGridDetails.DataBind(TrialBalanceModel.GetSubLedgerDateWisePref(Id, From, To, Company, Branch,figureAmount,roundupAmount).AsQueryable());
                }
            }
            catch
            {
                return null;
            }



            //if (Id != null)
            //    Id = Id.Replace("__", "&").Trim();
            //var gridModel = new CustomerJQGridModel();
            //string Company = Session["Company"].ToString();
            //string Branch = Session["Branch"].ToString();
            //return gridModel.CustomerGridDetails.DataBind(TrialBalanceModel.GetSubLedgerDateWise(Id, From, To, Company, Branch).AsQueryable());
        }
        public JsonResult OnSubLedgerDateWiseDetailsRequested(string id,string figureAmount,string roundupAmount)
        {
            //if (id != null)
            //    id = id.Replace("__", "&").Trim();
            //var gridModel = new CustomerJQGridModel();
            //string Company = Session["Company"].ToString();
            //string Branch = Session["Branch"].ToString();
            //return gridModel.CustomerGridVoucher.DataBind(TrialBalanceModel.GetSubLedgerDateWiseDetails(id, Company, Branch).AsQueryable());

            
            if (figureAmount == "actual") {
                Session["figureAmount"] = "Actual";
            }
            if (figureAmount == null && id==null)
            {
                Session["figureAmount"] = "Actual";
                Session["roundupAmount"] = "0.00";
            }
            figureAmount = Session["figureAmount"].ToString();
            roundupAmount = Session["roundupAmount"].ToString();
            string Company = Session["Company"].ToString();
            string Branch = Session["Branch"].ToString();
            var gridModel = new CustomerJQGridModel();
            switch (figureAmount)
            {
                case "lakh": figureAmount = "100000";
                    Session["figureAmount"] = 100000;
                    break;
                case "thousand": figureAmount = "1000";
                    Session["figureAmount"] = 1000;
                    break;
                case "crore": figureAmount = "10000000";
                    Session["figureAmount"] = 10000000;
                    break;
                case "Actual": figureAmount = "1";
                    Session["figureAmount"] = 1;
                    break;

            }
            if (roundupAmount == "0")
            {
                roundupAmount = "0.0";
            }
            string a = Convert.ToString(figureAmount);
            if (id != null)
            {
                Session["Ledger"] = id;
                Session["figureAmount"] = figureAmount;
                Session["roundupAmount"] = roundupAmount;
                if (id != null && a != "")
                {
                    Session["Ledger"] = id;
                    Session["figureAmount"] = a;
                    Session["roundupAmount"] = roundupAmount;
                }
            }
            try
            {
                if (Session["roundupAmount"] == null && Session["figureAmount"] == null)
                {
                    return gridModel.CustomerGridVoucher.DataBind(TrialBalanceModel.GetSubLedgerDateWiseDetails(id, Company, Branch).AsQueryable());
                    //return gridModel.CustomerGridDetails.DataBind(TrialBalanceModel.GetTrialBalanceDateWise(Session["Ledger"].ToString(), Session["From"].ToString(), Session["To"].ToString(), Company, Branch).AsQueryable());
                }
                else
                {
                    return gridModel.CustomerGridVoucher.DataBind(TrialBalanceModel.GetSubLedgerDateWiseDetailsPref(id, Company, Branch, a, roundupAmount).AsQueryable());
                }
            }
            catch
            {
                return null;
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetSubLedgerList(string AccountHead)
        {
            IEnumerable<TrialBalanceModel> data = TrialBalanceModel.getSubLedger();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetLedgerList(string AccountHead)
        {
            IEnumerable<TrialBalanceModel> data = TrialBalanceModel.getLedger();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Consolidated(string Id, string From, string To, string figureAmount, string roundupAmount, string SetDefault, string PreferenceName)
        {

            ViewData["FromDate"] = Convert.ToDateTime(From).ToString("yyyy-M-dd");
            ViewData["ToDate"] = Convert.ToDateTime(To).ToString("yyyy-M-dd");
            ViewData["DateStep"] = Id;
            switch (figureAmount)
            {
                case "lakh": ViewData["figureAmount"] = 100000;
                    Session["figureAmount"] = 100000;
                    break;
                case "thousand": ViewData["figureAmount"] = 1000;
                    Session["figureAmount"] = 1000;
                    break;
                case "crore": ViewData["figureAmount"] = 10000000;
                    Session["figureAmount"] = 10000000;
                    break;
                case "Actual": ViewData["figureAmount"] = 1;
                    Session["figureAmount"] = 1;
                    break;
            }
            ViewData["roundupAmount"] = roundupAmount;
            string a = ViewData["roundupAmount"].ToString();
            string b = ViewData["figureAmount"].ToString();
            ViewData["FromDate"] = Convert.ToDateTime(From).ToString("yyyy-M-dd");
            ViewData["ToDate"] = Convert.ToDateTime(To).ToString("yyyy-M-dd");
            if (PreferenceName != null)
            {
                TrialBalanceModel.SavePreference(PreferenceName, Id, From, To, Convert.ToString(ViewData["figureAmount"]), roundupAmount, SetDefault);
            }
            ViewData["ModuleMenu"] = _menuService.GetModule();
            return View();

        }

        public string ConsolidatedAjax()
        {
            string AjaxDt = TrialBalanceModel.GetDataForConsolidatedAjax();
            ViewData["AjaxDt"] = AjaxDt;
            return AjaxDt;
        }
        public string ConsolidatedAjaxComplete(string rowKey)
        {

            string PrefernceSetup = TrialBalanceModel.GetAllPreferencesVariable(rowKey);
            return PrefernceSetup;
        }
        public void ConsolidatedAjaxRemoveRow(string rowKey)
        {
            TrialBalanceModel.RemoveSelectedRow(rowKey);
        }
        public string GetPreferenceNameList()
        {
            string PreferenceList = TrialBalanceModel.GetPreferenceList();
            return PreferenceList;
        }
        [HttpPost]
        public ActionResult DailySummaryReport3(string Id, string From, string To, string figureAmount, string roundupAmount, string PreferenceName, string SetDefault, string ledgerP)
        {

            ViewData["FromDate"] = Convert.ToDateTime(From).ToString("yyyy-M-dd");
            ViewData["ToDate"] = Convert.ToDateTime(To).ToString("yyyy-M-dd");
            ViewData["DateStep"] = Id;
            switch (figureAmount)
            {
                case "lakh": ViewData["figureAmount"] = 100000;
                    Session["figureAmount"] = 100000;
                    break;
                case "thousand": ViewData["figureAmount"] = 1000;
                    Session["figureAmount"] = 1000;
                    break;
                case "crore": ViewData["figureAmount"] = 10000000;
                    Session["figureAmount"] = 10000000;
                    break;
                case "Actual": ViewData["figureAmount"] = 1;
                    Session["figureAmount"] = 1;
                    break;

            }
            ViewData["roundupAmount"] = roundupAmount;
            ViewData["FromDate"] = Convert.ToDateTime(From).ToString("yyyy-M-dd");
            ViewData["ToDate"] = Convert.ToDateTime(To).ToString("yyyy-M-dd");
            if (PreferenceName != null)
            {
                TrialBalanceModel.SavePreferenceLedgerStatement(PreferenceName, Id, From, To, Convert.ToString(ViewData["figureAmount"]), roundupAmount, SetDefault, ledgerP);
            }
            ViewData["ModuleMenu"] = _menuService.GetModule();
            return View();

        }
        

        [HttpPost]
        public ActionResult DailySummaryReport(string Id, string From, string To, string figureAmount, string roundupAmount, string PreferenceName, string SetDefault, string ledgerP)
        {

            ViewData["FromDate"] = Convert.ToDateTime(From).ToString("yyyy-M-dd");
            ViewData["ToDate"] = Convert.ToDateTime(To).ToString("yyyy-M-dd");
            ViewData["DateStep"] = Id;
            switch (figureAmount)
            {
                case "lakh": ViewData["figureAmount"] = 100000;
                    Session["figureAmount"] = 100000;
                    break;
                case "thousand": ViewData["figureAmount"] = 1000;
                    Session["figureAmount"] = 1000;
                    break;
                case "crore": ViewData["figureAmount"] = 10000000;
                    Session["figureAmount"] = 10000000;
                    break;
                case "Actual": ViewData["figureAmount"] = 1;  
                    Session["figureAmount"] = 1;
                    break;

            }
            ViewData["roundupAmount"] = roundupAmount;
            ViewData["FromDate"] = Convert.ToDateTime(From).ToString("yyyy-M-dd");
            ViewData["ToDate"] = Convert.ToDateTime(To).ToString("yyyy-M-dd");
            if (PreferenceName != null)
            {
                TrialBalanceModel.SavePreferenceLedgerStatement(PreferenceName, Id, From, To, Convert.ToString(ViewData["figureAmount"]), roundupAmount, SetDefault, ledgerP);
            }
            ViewData["ModuleMenu"] = _menuService.GetModule();
            return View();

        }
        [HttpPost]
        public ActionResult LedgerStatement(string Id, string From, string To,string figureAmount, string roundupAmount, string PreferenceName, string SetDefault, string ledgerP)
        {

            ViewData["FromDate"] = Convert.ToDateTime(From).ToString("yyyy-M-dd");
            ViewData["ToDate"] = Convert.ToDateTime(To).ToString("yyyy-M-dd");
            ViewData["DateStep"] = Id;
            string Company = Session["Company"].ToString();
            string Branch = Session["Branch"].ToString();
            switch (figureAmount)
            {
                case "lakh": ViewData["figureAmount"] = 100000;
                    Session["figureAmount"] = 100000;
                    break;
                case "thousand": ViewData["figureAmount"] = 1000;
                    Session["figureAmount"] = 1000;
                    break;
                case "crore": ViewData["figureAmount"] = 10000000;
                    Session["figureAmount"] = 10000000;
                    break;
                case "Actual": ViewData["figureAmount"] = 1;
                    Session["figureAmount"] = 1;
                    break;

            }
            
            ViewData["roundupAmount"] = roundupAmount;
            ViewData["FromDate"] = Convert.ToDateTime(From).ToString("yyyy-M-dd");
            ViewData["ToDate"] = Convert.ToDateTime(To).ToString("yyyy-M-dd");
            if (PreferenceName != null)
            {
                TrialBalanceModel.SavePreferenceLedgerStatement(PreferenceName, Id, From, To, Convert.ToString(ViewData["figureAmount"]), roundupAmount, SetDefault, ledgerP);
            }
            
            ViewData["ModuleMenu"] = _menuService.GetModule();
            string a=Convert.ToString(ViewData["figureAmount"]);
            IEnumerable<TrialBalanceModel> data = (TrialBalanceModel.GetTrialBalanceDateWisePref(ledgerP, From, To, Company, Branch, a,roundupAmount));
            //return Json(data, JsonRequestBehavior.AllowGet);
            return View(data);

        }
        public string LedgerAjax()
        {
            string AjaxDt = TrialBalanceModel.GetDataForLedgerAjax();
            ViewData["AjaxDt"] = AjaxDt;
            return AjaxDt;
        }
        public string LedgerAjaxComplete(string rowKey)
        {

            string PrefernceSetup = TrialBalanceModel.GetAllPreferencesVariableLedger(rowKey);
            return PrefernceSetup;
        }
        public void LedgerAjaxRemoveRow(string rowKey)
        {
            TrialBalanceModel.RemoveSelectedRowLedger(rowKey);
        }
        public string GetPreferenceNameListLedger()
        {
            string PreferenceList = TrialBalanceModel.GetPreferenceListLedger();
            return PreferenceList;
        }
        [HttpPost]
        public ActionResult SubLedgerSummary1(string Id, string From, string To, string figureAmount, string roundupAmount, string PreferenceName, string SetDefault, string SubledgerP)
        {

            ViewData["FromDate"] = Convert.ToDateTime(From).ToString("yyyy-M-dd");
            ViewData["ToDate"] = Convert.ToDateTime(To).ToString("yyyy-M-dd");
            ViewData["DateStep"] = Id;
            switch (figureAmount)
            {
                case "lakh": ViewData["figureAmount"] = 100000;
                    Session["figureAmount"] = 100000;
                    break;
                case "thousand": ViewData["figureAmount"] = 1000;
                    Session["figureAmount"] = 1000;
                    break;
                case "crore": ViewData["figureAmount"] = 10000000;
                    Session["figureAmount"] = 10000000;
                    break;
                case "Actual": ViewData["figureAmount"] = 1;
                    Session["figureAmount"] = 1;
                    break;

            }
            ViewData["roundupAmount"] = roundupAmount;
            ViewData["FromDate"] = Convert.ToDateTime(From).ToString("yyyy-M-dd");
            ViewData["ToDate"] = Convert.ToDateTime(To).ToString("yyyy-M-dd");
            if (PreferenceName != null)
            {
                TrialBalanceModel.SavePreferenceSubLedgerStatement(PreferenceName, Id, From, To, Convert.ToString(ViewData["figureAmount"]), roundupAmount, SetDefault, SubledgerP);
            }
            ViewData["ModuleMenu"] = _menuService.GetModule();
            return View();

        }
        public string SubLedgerAjax()
        {
            string AjaxDt = TrialBalanceModel.GetDataForSubLedgerAjax();
            ViewData["AjaxDt"] = AjaxDt;
            return AjaxDt;
        }
        public string SubLedgerAjaxComplete(string rowKey)
        {

            string PrefernceSetup = TrialBalanceModel.GetAllPreferencesVariableSubLedger(rowKey);
            return PrefernceSetup;
        }
        public void SubLedgerAjaxRemoveRow(string rowKey)
        {
            TrialBalanceModel.RemoveSelectedRowSubLedger(rowKey);
        }
        public string GetPreferenceNameListSubLedger()
        {
            string PreferenceList = TrialBalanceModel.GetPreferenceListSubLedger();
            return PreferenceList;
        }
    }
}
