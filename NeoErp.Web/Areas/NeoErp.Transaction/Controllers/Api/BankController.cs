using NeoErp.Core;
using NeoErp.Transaction.Service.Models;
using NeoErp.Transaction.Service.Services.BankSetup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoErp.Transaction.Controllers.Api
{
    public class BankController : ApiController
    {
        public IBankSetupService _bankService;
        private IWorkContext _workContext;

        public BankController(IBankSetupService _bankService, IWorkContext _workContext)
        {
            this._bankService = _bankService;
            this._workContext = _workContext;
        }
        [HttpPost]
        public string BankSetup(BankSetupModel BankSetup)
        {
            var bank = this._bankService.GetBank(BankSetup.BankCode);
            if (bank != null)
                return this._bankService.UpdateBank(BankSetup);

            return this._bankService.SaveBank(BankSetup);
        }
        [HttpGet]
        public string DeleteBank(int bankCode)
        {
            return this._bankService.DeleteBank(bankCode);
        }
        [HttpPost]
        public string BankLimitSetup(BankLimitModel Setup)
        {
            var limit = this._bankService.GetLimitByTransactionNumber(Setup.TransactionNumber, Setup.Type);
            if (limit != null)
                return this._bankService.UpdateLimit(Setup);

            return this._bankService.SaveLimit(Setup);
        }
        [HttpGet]
        public string UpdateLimit(int id,DateTime date)
        {
            return this._bankService.RenewByid(id, date);
        }
        public List<BankSetupModel> GetAllBanks()
        {
            return this._bankService.GetAllBanks();
        }
        public BankSetupModel GetBankByBankCode(string bankCode)
        {
            return this._bankService.GetBank(int.Parse(bankCode));
        }
        public List<BankLimitModel> GetAllBankLimit(string type="B")
        {
            return this._bankService.GetLimitList(type);
        }
        public List<BankLimitModel> GetHistory(int transNo, int sn, string type)
        {
            return this._bankService.GetHistory(transNo, sn, type);
        }
        public BankLimitModel GetFundedByTranNo(int transNo, string type = "B")
        {
            return this._bankService.GetLimitByTransactionNumber(transNo, type);
        }
        [HttpPost]
        public string CategorySetup(LoanCategoryModel model)
        {
            return this._bankService.SaveCategory(model);
        }
        public List<LoanCategoryModel> GetAllCategories()
        {
            return this._bankService.GetLoanCategories();
        }
        public LoanCategoryModel GetLoanCategory(int Id)
        {
            return this._bankService.GetLoanCategory(Id);
        }
        [HttpGet]
        public string DeleteLoanCategory(int Id)
        {
            return this._bankService.DeleteCategory(Id);
        }
    }
}
