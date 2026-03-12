using NeoErp.Core;
using NeoErp.Core.Domain;
using NeoErp.Core.Helpers;
using NeoErp.Sales.Modules.Services.Models.TrialBalance;
using NeoErp.Sales.Modules.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoErp.sales.Module.Controllers.Api
{
    public class TrialBalanceController : ApiController
    {
        public ITrialBalanceService _trialBalanceRegister { get; set; }
        private IWorkContext _workContext { get; set; }
        public IVoucherService _voucherRegister { get; set; }
        public TrialBalanceController(ITrialBalanceService trialBalanceRegister,IWorkContext workContext,IVoucherService voucherService)
        {
            this._trialBalanceRegister = trialBalanceRegister;
            this._workContext = workContext;
            this._voucherRegister = voucherService;
        }

        [HttpGet]
        public List<TrialBalanceViewModel> GetParentLedger(string formDate, string toDate)
        {
            var trialBalances = _trialBalanceRegister.GetTrialBalanceMasterGrid(formDate, toDate);
            return trialBalances;
        }
        public List<TrialBalanceViewModel> GetchildLedgers(string formDate, string toDate, string id)
        {
            var trialBalances = _trialBalanceRegister.GetTrialBalanceChildLedger(formDate, toDate, id);
            return trialBalances;
        }

        [HttpPost]
        public List<TrialBalanceViewModel> GetchildLedgersTrial(TrialBalancefilterOption model)
        {
            //int id = 0;
            //string parentId = "0";
            //string subledger = "0";
            //string datatype = "DG";
            var subledgerCount = Convert.ToInt16(model.subledger);

            if(subledgerCount>0)
            {
                var UniqueId = 99910+ subledgerCount;
                var subLedgerTrialBalance = _trialBalanceRegister.GETSubLedgerTrialBalance(model).ToList();
                foreach(var subledgercontent in subLedgerTrialBalance)
                {
                    subledgercontent.Id = UniqueId + subledgercontent.Id;
                    UniqueId++;

                }
                return subLedgerTrialBalance;
            }

            var trialBalances = _trialBalanceRegister.GetTrialBalanceChildLedgerTrial(model, _workContext.CurrentUserinformation,model.id, model.datatype);
            int i = 89;
            foreach(var trail in trialBalances.Where(x=>x.MASTER_ACC_CODE.EndsWith("00")))
                {
                trail.Id = trail.Id + i;
                i++;
            }
            return trialBalances;
        }
        [HttpPost]
        public List<PLLedgerViewModel> GetchildLedgers_SF_PL(TrialBalancefilterOption model)
        {
            //int id = 0;
            //string parentId = "0";
            //string subledger = "0";
            //string datatype = "DG";
            var subledgerCount = Convert.ToInt16(model.subledger);
      
            User userinfo = this._workContext.CurrentUserinformation;
            var data = _voucherRegister.GeneratePLLedger(model.formDate, model.formDate, userinfo.company_code, userinfo.branch_code, userinfo);
            var PLTree = new List<PLLedgerViewModel>();
            foreach(var pldata in data.OrderBy(x => x.SEGMENT_CODE).GroupBy(x => x.SEGMENT_NAME))
            {
                var getsegment = data.Where(x => x.SEGMENT_NAME == pldata.Key).ToList();
                var tree = new PLLedgerViewModel();
                tree.DESCRIPTION = getsegment.FirstOrDefault().SEGMENT_NAME;
                tree.PL_code = getsegment.FirstOrDefault().SEGMENT_CODE;
                tree.id = Convert.ToInt16(getsegment.FirstOrDefault().SEGMENT_CODE);
                tree.parentId = 0;
                double TotalAmount = 0;
                foreach(var segment in getsegment.OrderBy(x=>x.INDEXING))
                {
                    var pLLedgerView = new PLLedgerViewModel();
                    if (segment.END_SEGMENT == "Y")
                    {
                        continue;
                    }
                    tree.hasChildren = true;
                    var plcode = 0;
                     int.TryParse(segment.PL_CODE ,out plcode);
                    pLLedgerView.DESCRIPTION = segment.PL_EDESC;
                    pLLedgerView.PL_code = segment.PL_CODE;
                    pLLedgerView.id = plcode + 102;
                    pLLedgerView.parentId = tree.id;
                    pLLedgerView.BalanceAmt = segment.Balance;
                    if (segment.OPERATE_TYPE == "+")
                    {
                        pLLedgerView.signoperater = true;
                        TotalAmount = TotalAmount + segment.Balance ?? 0;
                    }
                    else if (segment.OPERATE_TYPE == "-")
                    {
                        pLLedgerView.signoperater = false;
                        TotalAmount = TotalAmount - segment.Balance ?? 0;
                    }

                    pLLedgerView.Sign = segment.OPERATE_TYPE;

                    PLTree.Add(pLLedgerView);
                    foreach( var acc in segment.ChildSummary)
                    {
                        var accountSummary = new PLLedgerViewModel();
                        int accCode = 0;
                        int.TryParse(acc.ACC_CODE, out accCode);
                        accountSummary.DESCRIPTION = acc.ACC_EDESC;
                        accountSummary.PL_code = acc.ACC_CODE;
                        accountSummary.id = accCode + 102;
                        accountSummary.parentId = pLLedgerView.id;
                        accountSummary.BalanceAmt = acc.Balance;
                        accountSummary.hasChildren = false;
                        pLLedgerView.hasChildren = true;
                        PLTree.Add(accountSummary);
                    }
                }
                tree.BalanceAmt = TotalAmount;
                if (TotalAmount < 0)
                {
                    tree.signoperater = false;
                    tree.Sign = "-";
                }

                PLTree.Add(tree);
               
                var Heading = getsegment.Where(x => x.END_SEGMENT == "Y").FirstOrDefault();
                if(Heading!=null)
                {
                    var lastHeadingSegment = new PLLedgerViewModel();
                    var plid = 0;
                    int.TryParse(Heading.PL_CODE, out plid);
                    lastHeadingSegment.DESCRIPTION = Heading.PL_EDESC;
                    lastHeadingSegment.PL_code = Heading.PL_CODE;
                    lastHeadingSegment.id = plid + 102;
                    lastHeadingSegment.parentId = tree.id;
                    lastHeadingSegment.BalanceAmt = TotalAmount;
                    PLTree.Add(lastHeadingSegment);
                }
                   
            }
            //if (subledgerCount > 0)
            //{
            //    var UniqueId = 99910 + subledgerCount;
            //    var subLedgerTrialBalance = _trialBalanceRegister.GETSubLedgerTrialBalance(model).ToList();
            //    foreach (var subledgercontent in subLedgerTrialBalance)
            //    {
            //        subledgercontent.Id = UniqueId + subledgercontent.Id;
            //        UniqueId++;

            //    }
            //    return subLedgerTrialBalance;
            //}

            //var trialBalances = _trialBalanceRegister.GetTrialBalanceChildLedgerTrial(model, _workContext.CurrentUserinformation, model.id, model.datatype);
            //int i = 89;
            //foreach (var trail in trialBalances.Where(x => x.MASTER_ACC_CODE.EndsWith("00")))
            //{
            //    trail.Id = trail.Id + i;
            //    i++;
            //}
            return PLTree.OrderBy(x=>x.id).ToList();
        }
        [HttpPost]
        public List<TrialBalanceViewModel> GetTreeView()
        {
            var trialBalances = _trialBalanceRegister.GetTrialBalanceMasterGrid("2015/1/1", "2016/1/1");
            // var abc = new TrialBalanceViewModel();
            // abc.MASTER_ACC_CODE = "00";
            //// abc.PRE_ACC_CODE = "00";
            // trialBalances.Insert(0, abc);
            return trialBalances;
        }
    }

}
