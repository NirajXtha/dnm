using NeoERP.ProjectManagement.Service.Models;
using NeoERP.ProjectManagement.Service.Interface;
using NeoERP.DocumentTemplate.Service.Models;
using NeoERP.DocumentTemplate.Service.Interface;
using System;
using NeoERP.ProjectManagement.Service;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using NeoErp.Core;
using NeoErp.Core.Caching;
using System.Data.SqlClient;
using NeoErp.Core.Models;
using NeoErp.Core.Models.Log4NetLoggin;
using System.Web.Http;
using NeoErp.Data;
using NeoErp.Core.Services.CommonSetting;

namespace NeoERP.ProjectManagement.Controllers.Api
{
    public class EntryApiController : ApiController
    {
        private const string PM = "PROJECT_MANAGEMENT";
        private IEntryRepo _entryRepo;
        private IDbContext _dbContext;
        private IFormTemplateRepo _FormTemplateRepo;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        private NeoErpCoreEntity _objectEntity;
        private readonly ILogErp _logErp;
        private ISettingService _settingService;
        private DefaultValueForLog _defaultValueForLog;
        public EntryApiController(IEntryRepo _IEntryRepo, IDbContext dbContext, IFormTemplateRepo FormTemplateRepo, IWorkContext workContext, ICacheManager cacheManager, NeoErpCoreEntity objectEntity)
        {
            this._entryRepo = _IEntryRepo;
            this._FormTemplateRepo = FormTemplateRepo;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            this._logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }
        [HttpGet]
        public List<FormControlModels> GetFormControlByFormCode(string formcode)
        {

            _logErp.InfoInFile("Get Form Control by Form code " + formcode + " is started");
            try
            {
                var userid = _workContext.CurrentUserinformation.User_id;
                var company_code = _workContext.CurrentUserinformation.company_code;
                var branch_code = _workContext.CurrentUserinformation.branch_code;
                var response = new List<FormControlModels>();
                if (this._cacheManager.IsSet($"GetFormControlByFormCode_{userid}_{company_code}_{branch_code}_{formcode}"))
                {
                    var data = _cacheManager.Get<List<FormControlModels>>($"GetFormControlByFormCode_{userid}_{company_code}_{branch_code}_{formcode}");
                    _logErp.InfoInFile("Form control by form code is fetched from cache");
                    response = data;
                }
                else
                {
                    var getFormControlByFormCodeList = this._entryRepo.GetFormControls(formcode);
                    _logErp.InfoInFile(getFormControlByFormCodeList.Count() + " form controls is fetched using : " + formcode);
                    this._cacheManager.Set($"GetFormControlByFormCode_{userid}_{company_code}_{branch_code}_{formcode}", getFormControlByFormCodeList, 20);
                    response = getFormControlByFormCodeList;
                }
                return response;

            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting form control by " + formcode + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        public FormDetailRefrence GetRefrenceFlag(string formCode)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<FORM_SETUP_REFERENCE>();
            //if (this._cacheManager.IsSet($"GetRefrenceFlag_{userid}_{company_code}_{branch_code}_{formCode}"))
            //{
            //    var data = _cacheManager.Get<List<FORM_SETUP_REFERENCE>>($"GetRefrenceFlag_{userid}_{company_code}_{branch_code}_{formCode}");
            //    response = data;
            //}
            //else
            //{
            var getRefrenceFlagList = this._entryRepo.GetRefrenceFlag(formCode);
            var vatRegistrationFlag = this._entryRepo.GetFormControls(formCode);
            var dataList = new FormDetailRefrence();
            dataList.FormSetupRefrence = getRefrenceFlagList;
            dataList.FormControlModels = vatRegistrationFlag;
            return dataList;

        }
        [HttpGet]
        public List<ChargeOnSales> GetChargeDataForEdit(string formCode, string voucherNo)
        {
            _logErp.InfoInFile("GetChargeDataForEdit based on formcode: " + formCode + " and voucher number : " + voucherNo + " started=====");
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<ChargeOnSales>();
            //if (this._cacheManager.IsSet($"GetChargeDataForEdit_{userid}_{company_code}_{branch_code}_{formCode}_{voucherNo}"))
            //{
            //    var data = _cacheManager.Get<List<ChargeOnSales>>($"GetChargeDataForEdit_{userid}_{company_code}_{branch_code}_{formCode}_{voucherNo}");
            //    _logErp.InfoInFile("GetChargeDataForEdit fetched successfully from cached!!!!");
            //    response = data;
            //}
            //else
            //{
            //    var ChargeDataForEdit = this._entryRepo.GetChargesData(formCode, voucherNo);
            //    _logErp.InfoInFile("charge Data for edit : " + ChargeDataForEdit);
            //    this._cacheManager.Set($"GetChargeDataForEdit_{userid}_{company_code}_{branch_code}_{formCode}_{voucherNo}", ChargeDataForEdit, 20);
            //    response = ChargeDataForEdit;
            //}
            var ChargeDataForEdit = this._entryRepo.GetChargesData(formCode, voucherNo);
            response = ChargeDataForEdit;
            return response;
        }
        [HttpGet]
        public List<ChargeOnSales> GetChargeData(string formCode)
        {
            _logErp.InfoInFile("GetCharge Data for " + formCode + " form code started==========");
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<ChargeOnSales>();
            //if (this._cacheManager.IsSet($"GetChargeData_{userid}_{company_code}_{branch_code}_{formCode}"))
            //{
            //    var data = _cacheManager.Get<List<ChargeOnSales>>($"GetChargeData_{userid}_{company_code}_{branch_code}_{formCode}");
            //    _logErp.InfoInFile(data.Count() + " records of charge data has been fetched from cached");
            //    response = data;
            //}
            //else
            //{
            //    var ChargeData = this._FormTemplateRepo.GetChargesData(formCode);
            //    _logErp.InfoInFile("Charge data for form code contains : " + ChargeData);
            //    this._cacheManager.Set($"GetChargeData_{userid}_{company_code}_{branch_code}_{formCode}", ChargeData, 20);
            //    response = ChargeData;
            //}
            var ChargeData = this._entryRepo.GetChargesData(formCode);
            response = ChargeData;
            return response;
        }
        [HttpGet]
        public List<FinancialBudgetTransaction> GetDataForBudgetModal(string VOUCHER_NO)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<FinancialBudgetTransaction>();
            //if (this._cacheManager.IsSet($"GetDataForBudgetModal_{userid}_{company_code}_{branch_code}_{VOUCHER_NO}"))
            //{
            //    var data = _cacheManager.Get<List<FinancialBudgetTransaction>>($"GetDataForBudgetModal_{userid}_{company_code}_{branch_code}_{VOUCHER_NO}");
            //    response = data;
            //}
            //else
            //{
            //    var financialBudgetTransactionList = this._entryRepo.Getbudgetdetail(VOUCHER_NO);
            //    this._cacheManager.Set($"GetDataForBudgetModal_{userid}_{company_code}_{branch_code}_{VOUCHER_NO}", financialBudgetTransactionList, 20);
            //    response = financialBudgetTransactionList;
            //}
            var financialBudgetTransactionList = this._entryRepo.Getbudgetdetail(VOUCHER_NO);
            response = financialBudgetTransactionList;
            return response;
            //return this._entryRepo.Getbudgetdetail(VOUCHER_NO);
        }
        [HttpGet]
        public List<BATCH_TRANSACTION_DATA> GetDataForBatchTrackingModal(string VOUCHER_NO)
        {
            var response = new List<BATCH_TRANSACTION_DATA>();

            var batchTransactionList = this._entryRepo.Getbatchtrackingdetail(VOUCHER_NO);
            response = batchTransactionList;
            return response;
        }
        [HttpGet]
        public List<BATCHTRANSACTIONDATA> GetDataForBatchModal(string VOUCHER_NO)
        {
            var response = new List<BATCHTRANSACTIONDATA>();

            var batchTransactionList = this._entryRepo.Getbatchdetail(VOUCHER_NO);
            response = batchTransactionList;
            return response;
        }
        [HttpGet]
        public string GetLoactionNameByCode(string locationcode)
        {
            var result = this._entryRepo.GetLoactionNameByCode(locationcode);
            return result;
        }
        [HttpGet]
        public List<CITY> GetAllCityDtlsByFilter(string filter)
        {

            var response = _entryRepo.GetAllCityDetailsByFilter(filter);
            return response;
        }
        [HttpGet]
        public List<VECHILES> GetAllVechileDtlsByFilter(string filter)
        {

            var response = _entryRepo.GetAllVechDetailsByFilter(filter);
            return response;
        }
        [HttpGet]
        public List<TRANSPORTER> GetAllTransporterDtlsByFilter(string filter)
        {

            var response = _entryRepo.GetAllTransporterDetailsByFilter(filter);
            return response;
        }
        //[HttpGet]
        //public string GetNewOrderNo(string companycode, string formcode, string currentdate, string tablename, string isSequence = "false")
        //{
        //    bool UseSequenceInTransaction = false;
        //    var setting = _settingService.LoadSetting<WebPrefrenceSetting>(Constants.WebPrefranceSetting);
        //    if (setting != null)
        //        bool.TryParse(setting.UseSequenceInTransaction, out UseSequenceInTransaction);
        //    if (UseSequenceInTransaction)
        //    {
        //        var result = this._entryRepo.GetNewSequence();
        //        return result;
        //    }
        //    else
        //    {
        //        var result = this._entryRepo.NewVoucherNo(companycode, formcode, currentdate, tablename);
        //        return result;
        //    }
        //    //  return "Error No";

        //}
        //[HttpGet]
        public bool getItemCountResult(string code)
        {
            var result = this._entryRepo.ItemNoExistsOrNot(code);
            return result;
        }
        [HttpGet]
        public bool getBatchItemCountResult(string code)
        {
            var result = this._entryRepo.BatchItemNoExistsOrNot(code);
            return result;
        }
        [HttpGet]
        public List<Division> GetdivisionListByFilter(string filter)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var Filterdata = new List<Division>();
            if (filter == "!@$")
                return Filterdata;
            if (filter == null)
                filter = "";
            if (filter.Contains("#"))
            {
                var colName = filter.Split('#')[0].ToString().ToLower();
                filter = filter.Split('#')[1].ToString().ToLower();
                if (colName == "code")
                {
                    if (this._cacheManager.IsSet($"GetdivisionListByFilter_{userid}_{company_code}_{branch_code}_{filter}_{colName}"))
                    {
                        Filterdata = this._cacheManager.Get<List<Division>>($"GetdivisionListByFilter_{userid}_{company_code}_{branch_code}_{filter}_{colName}");
                    }
                    else
                    {
                        var AllFilterDivision = this._entryRepo.GetAllDivisionSetup(filter);
                        var DivisionCodes = AllFilterDivision.Where(x => x.DIVISION_CODE.ToLower().Contains(filter));
                        var StartWithDivisionCode = DivisionCodes.Where(x => x.DIVISION_CODE.ToLower().StartsWith(filter.Trim())).ToList();
                        var startWithCodes = StartWithDivisionCode.Select(x => x.DIVISION_CODE.ToLower()).ToList();
                        var EndWithDivisionCode = DivisionCodes.Where(x => !startWithCodes.Contains(x.DIVISION_CODE.ToLower()) && x.DIVISION_CODE.ToLower().EndsWith(filter.Trim()));
                        var endWithCodes = EndWithDivisionCode.Select(x => x.DIVISION_EDESC.ToLower()).ToList();
                        var ContainsDivisionCode = DivisionCodes.Where(x => !startWithCodes.Contains(x.DIVISION_CODE.ToLower()) && !endWithCodes.Contains(x.DIVISION_CODE.ToLower()));

                        Filterdata.AddRange(StartWithDivisionCode);
                        Filterdata.AddRange(ContainsDivisionCode);
                        Filterdata.AddRange(EndWithDivisionCode);
                        Filterdata.ForEach(s => s.Type = "Code");
                        this._cacheManager.Set($"GetdivisionListByFilter_{userid}_{company_code}_{branch_code}_{filter}_{colName}", Filterdata, 30);
                        return Filterdata;
                    }
                }
                if (colName == "name")
                {
                    if (this._cacheManager.IsSet($"GetdivisionListByFilter_{userid}_{company_code}_{branch_code}_{filter}_{colName}"))
                    {
                        Filterdata = this._cacheManager.Get<List<Division>>($"GetdivisionListByFilter_{userid}_{company_code}_{branch_code}_{filter}_{colName}");
                    }
                    else
                    {
                        var AllFilterDivision = this._entryRepo.GetAllDivisionSetup(filter);
                        var DivisionNames = AllFilterDivision.Where(x => x.DIVISION_EDESC.ToLower().Contains(filter));
                        var StartWithDivisionName = DivisionNames.Where(x => x.DIVISION_EDESC.ToLower().StartsWith(filter.Trim())).ToList();
                        var startWithNames = StartWithDivisionName.Select(x => x.DIVISION_EDESC.ToLower()).ToList();
                        var EndWithDivisionName = DivisionNames.Where(x => !startWithNames.Contains(x.DIVISION_EDESC.ToLower()) && x.DIVISION_EDESC.ToLower().EndsWith(filter.Trim()));
                        var endWithNames = EndWithDivisionName.Select(x => x.DIVISION_EDESC.ToLower()).ToList();
                        var ContainsDivisionName = DivisionNames.Where(x => !startWithNames.Contains(x.DIVISION_EDESC.ToLower()) && !endWithNames.Contains(x.DIVISION_EDESC.ToLower()));
                        StartWithDivisionName.ForEach(s => s.Type = "Name");
                        EndWithDivisionName.ToList().ForEach(s => s.Type = "Name");
                        ContainsDivisionName.ToList().ForEach(s => s.Type = "Name");
                        Filterdata.AddRange(StartWithDivisionName);
                        Filterdata.AddRange(ContainsDivisionName);
                        Filterdata.AddRange(EndWithDivisionName);
                        this._cacheManager.Set($"GetdivisionListByFilter_{userid}_{company_code}_{branch_code}_{filter}_{colName}", Filterdata, 30);
                        return Filterdata;
                    }
                }
                if (colName == "address")
                {
                    if (this._cacheManager.IsSet($"GetdivisionListByFilter_{userid}_{company_code}_{branch_code}_{filter}_{colName}"))
                    {
                        Filterdata = this._cacheManager.Get<List<Division>>($"GetdivisionListByFilter_{userid}_{company_code}_{branch_code}_{filter}_{colName}");
                    }
                    else
                    {
                        var AllFilterAddress = this._entryRepo.GetAllDivisionSetup(filter);
                        var Address = AllFilterAddress.Where(x => x.ADDRESS.ToLower().Contains(filter));
                        var StartWithAddressName = Address.Where(x => x.ADDRESS.ToLower().StartsWith(filter.Trim())).ToList();
                        var startWithAddress = StartWithAddressName.Select(x => x.ADDRESS.ToLower()).ToList();
                        var EndWithAddressName = Address.Where(x => !startWithAddress.Contains(x.ADDRESS.ToLower()) && x.ADDRESS.ToLower().EndsWith(filter.Trim()));
                        var endWithAddress = EndWithAddressName.Select(x => x.ADDRESS.ToLower()).ToList();
                        var ContainsAddressName = Address.Where(x => !startWithAddress.Contains(x.ADDRESS.ToLower()) && !endWithAddress.Contains(x.ADDRESS.ToLower()));
                        StartWithAddressName.ForEach(s => s.Type = "Addr");
                        EndWithAddressName.ToList().ForEach(s => s.Type = "Addr");
                        ContainsAddressName.ToList().ForEach(s => s.Type = "Addr");
                        Filterdata.AddRange(StartWithAddressName);
                        Filterdata.AddRange(ContainsAddressName);
                        Filterdata.AddRange(EndWithAddressName);
                        this._cacheManager.Set($"GetdivisionListByFilter_{userid}_{company_code}_{branch_code}_{filter}_{colName}", Filterdata, 30);
                        return Filterdata;
                    }
                }
                if (colName == "phoneno")
                {
                    if (this._cacheManager.IsSet($"GetdivisionListByFilter_{userid}_{company_code}_{branch_code}_{filter}_{colName}"))
                    {
                        Filterdata = this._cacheManager.Get<List<Division>>($"GetdivisionListByFilter_{userid}_{company_code}_{branch_code}_{filter}_{colName}");
                    }
                    else
                    {
                        var AllFilterPhoneNo = this._entryRepo.GetAllDivisionSetup(filter);
                        var PhoneNoNames = AllFilterPhoneNo.Where(x => x.TELEPHONE_NO.ToLower().Contains(filter));
                        var StartWithPhoneNoName = PhoneNoNames.Where(x => x.TELEPHONE_NO.ToLower().StartsWith(filter.Trim())).ToList();
                        var startWithNames = StartWithPhoneNoName.Select(x => x.TELEPHONE_NO.ToLower()).ToList();
                        var EndWithPhoneNoName = PhoneNoNames.Where(x => !startWithNames.Contains(x.TELEPHONE_NO.ToLower()) && x.TELEPHONE_NO.ToLower().EndsWith(filter.Trim()));
                        var endWithNames = EndWithPhoneNoName.Select(x => x.TELEPHONE_NO.ToLower()).ToList();
                        var ContainsPhoneNoName = PhoneNoNames.Where(x => !startWithNames.Contains(x.TELEPHONE_NO.ToLower()) && !endWithNames.Contains(x.TELEPHONE_NO.ToLower()));
                        StartWithPhoneNoName.ForEach(s => s.Type = "Ph");
                        EndWithPhoneNoName.ToList().ForEach(s => s.Type = "Ph");
                        ContainsPhoneNoName.ToList().ForEach(s => s.Type = "Ph");
                        Filterdata.AddRange(StartWithPhoneNoName);
                        Filterdata.AddRange(ContainsPhoneNoName);
                        Filterdata.AddRange(EndWithPhoneNoName);
                        this._cacheManager.Set($"GetdivisionListByFilter_{userid}_{company_code}_{branch_code}_{filter}_{colName}", Filterdata, 30);
                        return Filterdata;
                    }
                }
            }
            else
            {
                var AllFilterDivision = this._entryRepo.GetAllDivisionSetup(filter);
                if (AllFilterDivision.Count >= 1)
                {
                    if (this._cacheManager.IsSet($"GetdivisionListByFilter_{userid}_{company_code}_{branch_code}_{filter}"))
                    {
                        Filterdata = this._cacheManager.Get<List<Division>>($"GetdivisionListByFilter_{userid}_{company_code}_{branch_code}_{filter}");
                    }
                    else
                    {
                        var DivisionCodes = AllFilterDivision.Where(x => x.DIVISION_CODE.ToLower().Contains(filter));
                        var StartWithDivisionCode = DivisionCodes.Where(x => x.DIVISION_CODE.ToLower().StartsWith(filter.Trim())).ToList();
                        var startWithCodes = StartWithDivisionCode.Select(x => x.DIVISION_CODE.ToLower()).ToList();
                        var EndWithDivisionCode = DivisionCodes.Where(x => !startWithCodes.Contains(x.DIVISION_CODE.ToLower()) && x.DIVISION_CODE.ToLower().EndsWith(filter.Trim()));
                        var endWithCodes = EndWithDivisionCode.Select(x => x.DIVISION_EDESC.ToLower()).ToList();
                        var ContainsDivisionCode = DivisionCodes.Where(x => !startWithCodes.Contains(x.DIVISION_CODE.ToLower()) && !endWithCodes.Contains(x.DIVISION_CODE.ToLower()));
                        Filterdata.AddRange(StartWithDivisionCode);
                        Filterdata.AddRange(ContainsDivisionCode);
                        Filterdata.AddRange(EndWithDivisionCode);
                        Filterdata.ForEach(s => s.Type = "Code");
                        var Removedata = AllFilterDivision.RemoveAll(x => x.DIVISION_CODE.ToLower().Contains(filter));
                        var DivisionNames = AllFilterDivision.Where(x => x.DIVISION_EDESC.ToLower().Contains(filter));
                        var StartWithDivisionName = DivisionNames.Where(x => x.DIVISION_EDESC.ToLower().StartsWith(filter.Trim())).ToList();
                        var startWithNames = StartWithDivisionName.Select(x => x.DIVISION_EDESC.ToLower()).ToList();
                        var EndWithDivisionName = DivisionNames.Where(x => !startWithNames.Contains(x.DIVISION_EDESC.ToLower()) && x.DIVISION_EDESC.ToLower().EndsWith(filter.Trim()));
                        var endWithNames = EndWithDivisionName.Select(x => x.DIVISION_EDESC.ToLower()).ToList();
                        var ContainsDivisionName = DivisionNames.Where(x => !startWithNames.Contains(x.DIVISION_EDESC.ToLower()) && !endWithNames.Contains(x.DIVISION_EDESC.ToLower()));
                        StartWithDivisionName.ForEach(s => s.Type = "Name");
                        EndWithDivisionName.ToList().ForEach(s => s.Type = "Name");
                        ContainsDivisionName.ToList().ForEach(s => s.Type = "Name");
                        Filterdata.AddRange(StartWithDivisionName);
                        Filterdata.AddRange(ContainsDivisionName);
                        Filterdata.AddRange(EndWithDivisionName);
                        AllFilterDivision.RemoveAll(x => x.DIVISION_EDESC.ToLower().Contains(filter));
                        var Addresses = AllFilterDivision.Where(x => x.ADDRESS.ToLower().Contains(filter));
                        var StartWithAddress = Addresses.Where(x => x.ADDRESS.ToLower().StartsWith(filter.Trim())).ToList();
                        var startWithAddress = StartWithAddress.Select(x => x.ADDRESS.ToLower()).ToList();
                        var EndWithAddress = Addresses.Where(x => !startWithAddress.Contains(x.ADDRESS.ToLower()) && x.ADDRESS.ToLower().EndsWith(filter.Trim()));
                        var endWithAddress = EndWithAddress.Select(x => x.ADDRESS.ToLower()).ToList();
                        var ContainsAddress = Addresses.Where(x => !startWithAddress.Contains(x.ADDRESS.ToLower()) && !endWithAddress.Contains(x.ADDRESS.ToLower()));
                        StartWithAddress.ForEach(s => s.Type = "Addr");
                        EndWithAddress.ToList().ForEach(s => s.Type = "Addr");
                        ContainsAddress.ToList().ForEach(s => s.Type = "Addr");
                        Filterdata.AddRange(StartWithAddress);
                        Filterdata.AddRange(ContainsAddress);
                        Filterdata.AddRange(EndWithAddress);
                        AllFilterDivision.RemoveAll(x => x.ADDRESS.ToLower().Contains(filter));
                        var PhoneNoNames = AllFilterDivision.Where(x => x.TELEPHONE_NO.ToLower().Contains(filter));
                        var StartWithPhoneNoName = PhoneNoNames.Where(x => x.TELEPHONE_NO.ToLower().StartsWith(filter.Trim())).ToList();
                        var startWithPhone = StartWithPhoneNoName.Select(x => x.TELEPHONE_NO.ToLower()).ToList();
                        var EndWithPhoneNoName = PhoneNoNames.Where(x => !startWithPhone.Contains(x.TELEPHONE_NO.ToLower()) && x.TELEPHONE_NO.ToLower().EndsWith(filter.Trim()));
                        var endWithPhone = EndWithPhoneNoName.Select(x => x.TELEPHONE_NO.ToLower()).ToList();
                        var ContainsPhoneNoName = PhoneNoNames.Where(x => !startWithPhone.Contains(x.TELEPHONE_NO.ToLower()) && !endWithPhone.Contains(x.TELEPHONE_NO.ToLower()));
                        StartWithPhoneNoName.ForEach(s => s.Type = "Ph");
                        EndWithPhoneNoName.ToList().ForEach(s => s.Type = "Ph");
                        ContainsPhoneNoName.ToList().ForEach(s => s.Type = "Ph");
                        Filterdata.AddRange(StartWithPhoneNoName);
                        Filterdata.AddRange(ContainsPhoneNoName);
                        Filterdata.AddRange(EndWithPhoneNoName);
                        this._cacheManager.Set($"GetdivisionListByFilter_{userid}_{company_code}_{branch_code}_{filter}", Filterdata, 30);
                        return Filterdata;
                    }
                }
                return AllFilterDivision;
            }
            return Filterdata;
        }
        [HttpGet]
        public List<Customers> GetAllSupplierForReferenceByFilter(string filter)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var Filterdata = new List<Customers>();
            var ShowAdvanceAutoComplete = false;
            var setting = _settingService.LoadSetting<WebPrefrenceSetting>(Constants.WebPrefranceSetting);
            if (setting != null)
                bool.TryParse(setting.ShowAdvanceAutoComplete, out ShowAdvanceAutoComplete);
            if (ShowAdvanceAutoComplete == false)
            {
                if (this._cacheManager.IsSet($"AllSupplierForReferenceByCode_{userid}_{company_code}_{branch_code}_{filter}_{ShowAdvanceAutoComplete}"))
                {
                    var data = _cacheManager.Get<List<Customers>>($"AllSupplierForReferenceByCode_{userid}_{company_code}_{branch_code}_{filter}_{ShowAdvanceAutoComplete}");
                    Filterdata = data;
                    return Filterdata;
                }
                else
                {
                    var AllFilterSupplier = this._entryRepo.getALLSupplierListByFlterForReference(filter);
                    this._cacheManager.Set($"AllSupplierForReferenceByCode_{userid}_{company_code}_{branch_code}_{filter}_{ShowAdvanceAutoComplete}", Filterdata, 20);
                    return Filterdata;
                }
            }
            if (filter == "!@$")
                return Filterdata;
            if (filter == null)
                return this._entryRepo.getALLSupplierListByFlterForReference(filter);
            if (filter.Contains("#"))
            {
                var colName = filter.Split('#')[0].ToString().ToLower();
                filter = filter.Split('#')[1].ToString().ToLower();
                if (colName == "code")
                {
                    if (this._cacheManager.IsSet($"AllSupplierForReferenceByCode_{userid}_{company_code}_{branch_code}_{filter}"))
                    {
                        var data = _cacheManager.Get<List<Customers>>($"AllSupplierForReferenceByCode_{userid}_{company_code}_{branch_code}_{filter}");
                        Filterdata = data;
                    }
                    else
                    {
                        var AllFilterSupplier = this._entryRepo.getALLSupplierListByFlterForReference(filter);
                        var SupplierCodes = AllFilterSupplier.Where(x => x.CustomerCode.ToLower().Contains(filter));
                        var StartWithSupplierCode = SupplierCodes.Where(x => x.CustomerCode.ToLower().StartsWith(filter.Trim())).ToList();
                        var startWithCodes = StartWithSupplierCode.Select(x => x.CustomerCode.ToLower()).ToList();
                        var EndWithSupplierCode = SupplierCodes.Where(x => !startWithCodes.Contains(x.CustomerCode.ToLower()) && x.CustomerCode.ToLower().EndsWith(filter.Trim()));
                        var endWithCodes = EndWithSupplierCode.Select(x => x.CustomerCode.ToLower()).ToList();
                        var ContainsSupplierCode = SupplierCodes.Where(x => !startWithCodes.Contains(x.CustomerCode.ToLower()) && !endWithCodes.Contains(x.CustomerCode.ToLower()));

                        Filterdata.AddRange(StartWithSupplierCode);
                        Filterdata.AddRange(ContainsSupplierCode);
                        Filterdata.AddRange(EndWithSupplierCode);
                        Filterdata.ForEach(s => s.Type = "SupplierCode");
                        this._cacheManager.Set($"AllCustomerSetupByCode_{userid}_{company_code}_{branch_code}_{filter}", Filterdata, 20);
                    }
                    return Filterdata;
                }
                if (colName == "name")
                {
                    if (this._cacheManager.IsSet($"AllSupplierForReferenceByName_{userid}_{company_code}_{branch_code}_{filter}"))
                    {
                        var data = _cacheManager.Get<List<Customers>>($"AllSupplierForReferenceByName_{userid}_{company_code}_{branch_code}_{filter}");
                        Filterdata = data;
                    }
                    else
                    {
                        var AllFilterSupplier = this._entryRepo.getALLSupplierListByFlterForReference(filter);
                        var SupplierNames = AllFilterSupplier.Where(x => x.CustomerName.ToLower().Contains(filter));
                        var StartWithSupplierName = SupplierNames.Where(x => x.CustomerName.ToLower().StartsWith(filter.Trim())).ToList();
                        var startWithNames = StartWithSupplierName.Select(x => x.CustomerName.ToLower()).ToList();
                        var EndWithSupplierName = SupplierNames.Where(x => !startWithNames.Contains(x.CustomerName.ToLower()) && x.CustomerName.ToLower().EndsWith(filter.Trim()));
                        var endWithNames = EndWithSupplierName.Select(x => x.CustomerName.ToLower()).ToList();
                        var ContainsSupplierName = SupplierNames.Where(x => !startWithNames.Contains(x.CustomerName.ToLower()) && !endWithNames.Contains(x.CustomerName.ToLower()));
                        StartWithSupplierName.ForEach(s => s.Type = "SupplierName");
                        EndWithSupplierName.ToList().ForEach(s => s.Type = "SupplierName");
                        ContainsSupplierName.ToList().ForEach(s => s.Type = "SupplierName");
                        Filterdata.AddRange(StartWithSupplierName);
                        Filterdata.AddRange(ContainsSupplierName);
                        Filterdata.AddRange(EndWithSupplierName);
                        this._cacheManager.Set($"AllSupplierForReferenceByName_{userid}_{company_code}_{branch_code}_{filter}", Filterdata, 20);
                    }
                    return Filterdata;
                }
                if (colName == "address")
                {
                    if (this._cacheManager.IsSet($"AllSupplierForReferenceByName_{userid}_{company_code}_{branch_code}_{filter}"))
                    {
                        var data = _cacheManager.Get<List<Customers>>($"AllSupplierForReferenceByName_{userid}_{company_code}_{branch_code}_{filter}");
                        Filterdata = data;
                    }
                    else
                    {
                        var AllFilterAddress = this._entryRepo.getALLSupplierListByFlterForReference(filter);
                        var Address = AllFilterAddress.Where(x => x.REGD_OFFICE_EADDRESS.ToLower().Contains(filter));
                        var StartWithAddressName = Address.Where(x => x.REGD_OFFICE_EADDRESS.ToLower().StartsWith(filter.Trim())).ToList();
                        var startWithAddress = StartWithAddressName.Select(x => x.REGD_OFFICE_EADDRESS.ToLower()).ToList();
                        var EndWithAddressName = Address.Where(x => !startWithAddress.Contains(x.REGD_OFFICE_EADDRESS.ToLower()) && x.REGD_OFFICE_EADDRESS.ToLower().EndsWith(filter.Trim()));
                        var endWithAddress = EndWithAddressName.Select(x => x.REGD_OFFICE_EADDRESS.ToLower()).ToList();
                        var ContainsAddressName = Address.Where(x => !startWithAddress.Contains(x.REGD_OFFICE_EADDRESS.ToLower()) && !endWithAddress.Contains(x.REGD_OFFICE_EADDRESS.ToLower()));
                        StartWithAddressName.ForEach(s => s.Type = "Address");
                        EndWithAddressName.ToList().ForEach(s => s.Type = "Address");
                        ContainsAddressName.ToList().ForEach(s => s.Type = "Address");
                        Filterdata.AddRange(StartWithAddressName);
                        Filterdata.AddRange(ContainsAddressName);
                        Filterdata.AddRange(EndWithAddressName);
                        this._cacheManager.Set($"AllSupplierForReferenceByaddress_{userid}_{company_code}_{branch_code}_{filter}", Filterdata, 20);
                    }
                    return Filterdata;
                }
                if (colName == "phoneno")
                {
                    if (this._cacheManager.IsSet($"AllSupplierForReferenceByphoneno_{userid}_{company_code}_{branch_code}_{filter}"))
                    {
                        var data = _cacheManager.Get<List<Customers>>($"AllSupplierForReferenceByphoneno_{userid}_{company_code}_{branch_code}_{filter}");
                        Filterdata = data;
                    }
                    else
                    {
                        var AllFilterPhoneNo = this._entryRepo.getALLSupplierListByFlterForReference(filter);
                        var PhoneNoNames = AllFilterPhoneNo.Where(x => x.TEL_MOBILE_NO1.ToLower().Contains(filter));
                        var StartWithPhoneNoName = PhoneNoNames.Where(x => x.TEL_MOBILE_NO1.ToLower().StartsWith(filter.Trim())).ToList();
                        var startWithNames = StartWithPhoneNoName.Select(x => x.TEL_MOBILE_NO1.ToLower()).ToList();
                        var EndWithPhoneNoName = PhoneNoNames.Where(x => !startWithNames.Contains(x.TEL_MOBILE_NO1.ToLower()) && x.TEL_MOBILE_NO1.ToLower().EndsWith(filter.Trim()));
                        var endWithNames = EndWithPhoneNoName.Select(x => x.TEL_MOBILE_NO1.ToLower()).ToList();
                        var ContainsPhoneNoName = PhoneNoNames.Where(x => !startWithNames.Contains(x.TEL_MOBILE_NO1.ToLower()) && !endWithNames.Contains(x.TEL_MOBILE_NO1.ToLower()));
                        StartWithPhoneNoName.ForEach(s => s.Type = "PhoneNo");
                        EndWithPhoneNoName.ToList().ForEach(s => s.Type = "PhoneNo");
                        ContainsPhoneNoName.ToList().ForEach(s => s.Type = "PhoneNo");
                        Filterdata.AddRange(StartWithPhoneNoName);
                        Filterdata.AddRange(ContainsPhoneNoName);
                        Filterdata.AddRange(EndWithPhoneNoName);
                        this._cacheManager.Set($"AllSupplierForReferenceByphoneno_{userid}_{company_code}_{branch_code}_{filter}", Filterdata, 20);
                    }
                    return Filterdata;
                }
            }
            else
            {
                if (this._cacheManager.IsSet($"AllFilterSupplierForReference_{userid}_{company_code}_{branch_code}_{filter}"))
                {
                    var data = _cacheManager.Get<List<Customers>>($"AllFilterSupplierForReference_{userid}_{company_code}_{branch_code}_{filter}");
                    Filterdata = data;
                }
                else
                {
                    var AllFilterSupplier = this._entryRepo.getALLSupplierListByFlterForReference(filter);
                    if (filter == null)
                        return AllFilterSupplier;
                    if (AllFilterSupplier.Count >= 1)
                    {
                        var SupplierCodes = AllFilterSupplier.Where(x => x.CustomerCode.ToLower().Contains(filter));
                        var StartWithSupplierCode = SupplierCodes.Where(x => x.CustomerCode.ToLower().StartsWith(filter.Trim())).ToList();
                        var startWithCodes = StartWithSupplierCode.Select(x => x.CustomerCode.ToLower()).ToList();
                        var EndWithSupplierCode = SupplierCodes.Where(x => !startWithCodes.Contains(x.CustomerCode.ToLower()) && x.CustomerCode.ToLower().EndsWith(filter.Trim()));
                        var endWithCodes = EndWithSupplierCode.Select(x => x.CustomerCode.ToLower()).ToList();
                        var ContainsSupplierCode = SupplierCodes.Where(x => !startWithCodes.Contains(x.CustomerCode.ToLower()) && !endWithCodes.Contains(x.CustomerCode.ToLower()));
                        Filterdata.AddRange(StartWithSupplierCode);
                        Filterdata.AddRange(ContainsSupplierCode);
                        Filterdata.AddRange(EndWithSupplierCode);
                        Filterdata.ForEach(s => s.Type = "SupplierCode");
                        AllFilterSupplier.RemoveAll(x => x.CustomerCode.Contains(filter));
                        var SupplierNames = AllFilterSupplier.Where(x => x.CustomerName.ToLower().Contains(filter));
                        var StartWithSupplierName = SupplierNames.Where(x => x.CustomerName.ToLower().StartsWith(filter.Trim())).ToList();
                        var startWithNames = StartWithSupplierName.Select(x => x.CustomerName.ToLower()).ToList();
                        var EndWithSupplierName = SupplierNames.Where(x => !startWithNames.Contains(x.CustomerName.ToLower()) && x.CustomerName.ToLower().EndsWith(filter.Trim()));
                        var endWithNames = EndWithSupplierName.Select(x => x.CustomerName.ToLower()).ToList();
                        var ContainsSupplierName = SupplierNames.Where(x => !startWithNames.Contains(x.CustomerName.ToLower()) && !endWithNames.Contains(x.CustomerName.ToLower()));
                        StartWithSupplierName.ForEach(s => s.Type = "SupplierName");
                        EndWithSupplierName.ToList().ForEach(s => s.Type = "SupplierName");
                        ContainsSupplierName.ToList().ForEach(s => s.Type = "SupplierName");
                        Filterdata.AddRange(StartWithSupplierName);
                        Filterdata.AddRange(ContainsSupplierName);
                        Filterdata.AddRange(EndWithSupplierName);
                        AllFilterSupplier.RemoveAll(x => x.CustomerName.ToLower().Contains(filter));
                        var Address = AllFilterSupplier.Where(x => x.REGD_OFFICE_EADDRESS.ToLower().Contains(filter));
                        var StartWithAddressName = Address.Where(x => x.REGD_OFFICE_EADDRESS.ToLower().StartsWith(filter.Trim())).ToList();
                        var startWithAddress = StartWithAddressName.Select(x => x.REGD_OFFICE_EADDRESS.ToLower()).ToList();
                        var EndWithAddressName = Address.Where(x => !startWithAddress.Contains(x.REGD_OFFICE_EADDRESS.ToLower()) && x.REGD_OFFICE_EADDRESS.ToLower().EndsWith(filter.Trim()));
                        var endWithAddress = EndWithAddressName.Select(x => x.REGD_OFFICE_EADDRESS.ToLower()).ToList();
                        var ContainsAddressName = Address.Where(x => !startWithAddress.Contains(x.REGD_OFFICE_EADDRESS.ToLower()) && !endWithAddress.Contains(x.REGD_OFFICE_EADDRESS.ToLower()));
                        StartWithAddressName.ForEach(s => s.Type = "Address");
                        EndWithAddressName.ToList().ForEach(s => s.Type = "Address");
                        ContainsAddressName.ToList().ForEach(s => s.Type = "Address");
                        Filterdata.AddRange(StartWithAddressName);
                        Filterdata.AddRange(ContainsAddressName);
                        Filterdata.AddRange(EndWithAddressName);
                        AllFilterSupplier.RemoveAll(x => x.REGD_OFFICE_EADDRESS.ToLower().Contains(filter));
                        var PhoneNoNames = AllFilterSupplier.Where(x => x.TEL_MOBILE_NO1.ToLower().Contains(filter));
                        var StartWithPhoneNoName = PhoneNoNames.Where(x => x.TEL_MOBILE_NO1.ToLower().StartsWith(filter.Trim())).ToList();
                        var startWithPhone = StartWithPhoneNoName.Select(x => x.TEL_MOBILE_NO1.ToLower()).ToList();
                        var EndWithPhoneNoName = PhoneNoNames.Where(x => !startWithPhone.Contains(x.TEL_MOBILE_NO1.ToLower()) && x.TEL_MOBILE_NO1.ToLower().EndsWith(filter.Trim()));
                        var endWithPhone = EndWithPhoneNoName.Select(x => x.TEL_MOBILE_NO1.ToLower()).ToList();
                        var ContainsPhoneNoName = PhoneNoNames.Where(x => !startWithPhone.Contains(x.TEL_MOBILE_NO1.ToLower()) && !endWithPhone.Contains(x.TEL_MOBILE_NO1.ToLower()));
                        StartWithPhoneNoName.ForEach(s => s.Type = "PhoneNo");
                        EndWithPhoneNoName.ToList().ForEach(s => s.Type = "PhoneNo");
                        ContainsPhoneNoName.ToList().ForEach(s => s.Type = "PhoneNo");
                        Filterdata.AddRange(StartWithPhoneNoName);
                        Filterdata.AddRange(ContainsPhoneNoName);
                        Filterdata.AddRange(EndWithPhoneNoName);
                        this._cacheManager.Set($"AllFilterSupplierForReference_{userid}_{company_code}_{branch_code}_{filter}", Filterdata, 20);
                        return Filterdata;
                    }
                    return AllFilterSupplier;
                }
            }
            return Filterdata;
        }
        [HttpGet]
        public List<Suppliers> GetAllSupplierListByFilter(string filter)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var Filterdata = new List<Suppliers>();
            var ShowAdvanceAutoComplete = false;
            var setting = _settingService.LoadSetting<WebPrefrenceSetting>(Constants.WebPrefranceSetting);
            if (setting != null)
                bool.TryParse(setting.ShowAdvanceAutoComplete, out ShowAdvanceAutoComplete);
            if (ShowAdvanceAutoComplete == false)
            {
                if (this._cacheManager.IsSet($"AllFilterSupplier_{userid}_{company_code}_{branch_code}_{filter}_{ShowAdvanceAutoComplete}"))
                {
                    var data = _cacheManager.Get<List<Suppliers>>($"AllFilterSupplier_{userid}_{company_code}_{branch_code}_{filter}_{ShowAdvanceAutoComplete}");
                    Filterdata = data;
                    return Filterdata;
                }
                else
                {
                    var AllFilterSupplier = this._entryRepo.getALLSupplierListByFlter(filter);
                    this._cacheManager.Set($"AllFilterSupplier_{userid}_{company_code}_{branch_code}_{filter}_{ShowAdvanceAutoComplete}", AllFilterSupplier, 20);
                    return AllFilterSupplier;
                }

            }
            if (filter == "!@$")
                return Filterdata;
            if (filter == null)
                return this._entryRepo.getALLSupplierListByFlter(filter);
            if (filter.Contains("#"))
            {
                var colName = filter.Split('#')[0].ToString().ToLower();
                filter = filter.Split('#')[1].ToString().ToLower();
                if (colName == "code")
                {
                    if (this._cacheManager.IsSet($"AllSupplierSetupByCode_{userid}_{company_code}_{branch_code}_{filter}"))
                    {
                        var data = _cacheManager.Get<List<Suppliers>>($"AllSupplierSetupByCode_{userid}_{company_code}_{branch_code}_{filter}");
                        Filterdata = data;
                    }
                    else
                    {
                        var AllFilterSupplier = this._entryRepo.getALLSupplierListByFlter(filter);
                        var SupplierCodes = AllFilterSupplier.Where(x => x.SUPPLIER_CODE.ToLower().Contains(filter));
                        var StartWithSupplierCode = SupplierCodes.Where(x => x.SUPPLIER_CODE.ToLower().StartsWith(filter.Trim())).ToList();
                        var startWithCodes = StartWithSupplierCode.Select(x => x.SUPPLIER_CODE.ToLower()).ToList();
                        var EndWithSupplierCode = SupplierCodes.Where(x => !startWithCodes.Contains(x.SUPPLIER_CODE.ToLower()) && x.SUPPLIER_CODE.ToLower().EndsWith(filter.Trim()));
                        var endWithCodes = EndWithSupplierCode.Select(x => x.SUPPLIER_CODE.ToLower()).ToList();
                        var ContainsSupplierCode = SupplierCodes.Where(x => !startWithCodes.Contains(x.SUPPLIER_CODE.ToLower()) && !endWithCodes.Contains(x.SUPPLIER_CODE.ToLower()));
                        Filterdata.AddRange(StartWithSupplierCode);
                        Filterdata.AddRange(ContainsSupplierCode);
                        Filterdata.AddRange(EndWithSupplierCode);
                        Filterdata.ForEach(s => s.Type = "SupplierCode");
                        this._cacheManager.Set($"AllSupplierSetupByCode_{userid}_{company_code}_{branch_code}_{filter}", Filterdata, 20);
                    }
                    return Filterdata;
                }
                if (colName == "name")
                {
                    if (this._cacheManager.IsSet($"AllSupplierSetupByName_{userid}_{company_code}_{branch_code}_{filter}"))
                    {
                        var data = _cacheManager.Get<List<Suppliers>>($"AllSupplierSetupByName_{userid}_{company_code}_{branch_code}_{filter}");
                        Filterdata = data;
                    }
                    else
                    {
                        var AllFilterSupplier = this._entryRepo.getALLSupplierListByFlter(filter);
                        var SupplierNames = AllFilterSupplier.Where(x => x.SUPPLIER_EDESC.ToLower().Contains(filter));
                        var StartWithSupplierName = SupplierNames.Where(x => x.SUPPLIER_EDESC.ToLower().StartsWith(filter.Trim())).ToList();
                        var startWithNames = StartWithSupplierName.Select(x => x.SUPPLIER_EDESC.ToLower()).ToList();
                        var EndWithSupplierName = SupplierNames.Where(x => !startWithNames.Contains(x.SUPPLIER_EDESC.ToLower()) && x.SUPPLIER_EDESC.ToLower().EndsWith(filter.Trim()));
                        var endWithNames = EndWithSupplierName.Select(x => x.SUPPLIER_EDESC.ToLower()).ToList();
                        var ContainsSupplierName = SupplierNames.Where(x => !startWithNames.Contains(x.SUPPLIER_EDESC.ToLower()) && !endWithNames.Contains(x.SUPPLIER_EDESC.ToLower()));
                        StartWithSupplierName.ForEach(s => s.Type = "SupplierName");
                        EndWithSupplierName.ToList().ForEach(s => s.Type = "SupplierName");
                        ContainsSupplierName.ToList().ForEach(s => s.Type = "SupplierName");
                        Filterdata.AddRange(StartWithSupplierName);
                        Filterdata.AddRange(ContainsSupplierName);
                        Filterdata.AddRange(EndWithSupplierName);
                        this._cacheManager.Set($"AllSupplierSetupByName_{userid}_{company_code}_{branch_code}_{filter}", Filterdata, 20);
                    }
                    return Filterdata;
                }
                if (colName == "address")
                {
                    if (this._cacheManager.IsSet($"AllSupplierSetupByAddress_{userid}_{company_code}_{branch_code}_{filter}"))
                    {
                        var data = _cacheManager.Get<List<Suppliers>>($"AllSupplierSetupByAddress_{userid}_{company_code}_{branch_code}_{filter}");
                        Filterdata = data;
                    }
                    else
                    {
                        var AllFilterAddress = this._entryRepo.getALLSupplierListByFlter(filter);
                        var Address = AllFilterAddress.Where(x => x.REGD_OFFICE_EADDRESS.ToLower().Contains(filter));
                        var StartWithAddressName = Address.Where(x => x.REGD_OFFICE_EADDRESS.ToLower().StartsWith(filter.Trim())).ToList();
                        var startWithAddress = StartWithAddressName.Select(x => x.REGD_OFFICE_EADDRESS.ToLower()).ToList();
                        var EndWithAddressName = Address.Where(x => !startWithAddress.Contains(x.REGD_OFFICE_EADDRESS.ToLower()) && x.REGD_OFFICE_EADDRESS.ToLower().EndsWith(filter.Trim()));
                        var endWithAddress = EndWithAddressName.Select(x => x.REGD_OFFICE_EADDRESS.ToLower()).ToList();
                        var ContainsAddressName = Address.Where(x => !startWithAddress.Contains(x.REGD_OFFICE_EADDRESS.ToLower()) && !endWithAddress.Contains(x.REGD_OFFICE_EADDRESS.ToLower()));
                        StartWithAddressName.ForEach(s => s.Type = "Address");
                        EndWithAddressName.ToList().ForEach(s => s.Type = "Address");
                        ContainsAddressName.ToList().ForEach(s => s.Type = "Address");
                        Filterdata.AddRange(StartWithAddressName);
                        Filterdata.AddRange(ContainsAddressName);
                        Filterdata.AddRange(EndWithAddressName);
                        this._cacheManager.Set($"AllSupplierSetupByAddress_{userid}_{company_code}_{branch_code}_{filter}", Filterdata, 20);
                    }
                    return Filterdata;
                }
                if (colName == "phoneno")
                {
                    if (this._cacheManager.IsSet($"AllSupplierSetupByPhoneno_{userid}_{company_code}_{branch_code}_{filter}"))
                    {
                        var data = _cacheManager.Get<List<Suppliers>>($"AllSupplierSetupByPhoneno_{userid}_{company_code}_{branch_code}_{filter}");
                        Filterdata = data;
                    }
                    else
                    {
                        var AllFilterPhoneNo = this._entryRepo.getALLSupplierListByFlter(filter);
                        var PhoneNoNames = AllFilterPhoneNo.Where(x => x.TEL_MOBILE_NO1.ToLower().Contains(filter));
                        var StartWithPhoneNoName = PhoneNoNames.Where(x => x.TEL_MOBILE_NO1.ToLower().StartsWith(filter.Trim())).ToList();
                        var startWithNames = StartWithPhoneNoName.Select(x => x.TEL_MOBILE_NO1.ToLower()).ToList();
                        var EndWithPhoneNoName = PhoneNoNames.Where(x => !startWithNames.Contains(x.TEL_MOBILE_NO1.ToLower()) && x.TEL_MOBILE_NO1.ToLower().EndsWith(filter.Trim()));
                        var endWithNames = EndWithPhoneNoName.Select(x => x.TEL_MOBILE_NO1.ToLower()).ToList();
                        var ContainsPhoneNoName = PhoneNoNames.Where(x => !startWithNames.Contains(x.TEL_MOBILE_NO1.ToLower()) && !endWithNames.Contains(x.TEL_MOBILE_NO1.ToLower()));
                        StartWithPhoneNoName.ForEach(s => s.Type = "PhoneNo");
                        EndWithPhoneNoName.ToList().ForEach(s => s.Type = "PhoneNo");
                        ContainsPhoneNoName.ToList().ForEach(s => s.Type = "PhoneNo");
                        Filterdata.AddRange(StartWithPhoneNoName);
                        Filterdata.AddRange(ContainsPhoneNoName);
                        Filterdata.AddRange(EndWithPhoneNoName);
                        this._cacheManager.Set($"AllSupplierSetupByPhoneno_{userid}_{company_code}_{branch_code}_{filter}", Filterdata, 20);
                    }
                    return Filterdata;
                }
            }
            else
            {
                if (this._cacheManager.IsSet($"AllFilterSupplier_{userid}_{company_code}_{branch_code}_{filter}"))
                {
                    var data = _cacheManager.Get<List<Suppliers>>($"AllFilterSupplier_{userid}_{company_code}_{branch_code}_{filter}");
                    Filterdata = data;
                }
                else
                {
                    var AllFilterSupplier = this._entryRepo.getALLSupplierListByFlter(filter);
                    if (filter == null)
                        return AllFilterSupplier;
                    if (AllFilterSupplier.Count >= 1)
                    {
                        var SupplierCodes = AllFilterSupplier.Where(x => x.SUPPLIER_CODE.ToLower().Contains(filter));
                        var StartWithSupplierCode = SupplierCodes.Where(x => x.SUPPLIER_CODE.ToLower().StartsWith(filter.Trim())).ToList();
                        var startWithCodes = StartWithSupplierCode.Select(x => x.SUPPLIER_CODE.ToLower()).ToList();
                        var EndWithSupplierCode = SupplierCodes.Where(x => !startWithCodes.Contains(x.SUPPLIER_CODE.ToLower()) && x.SUPPLIER_CODE.ToLower().EndsWith(filter.Trim()));
                        var endWithCodes = EndWithSupplierCode.Select(x => x.SUPPLIER_CODE.ToLower()).ToList();
                        var ContainsSupplierCode = SupplierCodes.Where(x => !startWithCodes.Contains(x.SUPPLIER_CODE.ToLower()) && !endWithCodes.Contains(x.SUPPLIER_CODE.ToLower()));
                        Filterdata.AddRange(StartWithSupplierCode);
                        Filterdata.AddRange(ContainsSupplierCode);
                        Filterdata.AddRange(EndWithSupplierCode);
                        Filterdata.ForEach(s => s.Type = "SupplierCode");
                        AllFilterSupplier.RemoveAll(x => x.SUPPLIER_CODE.Contains(filter));
                        var SupplierNames = AllFilterSupplier.Where(x => x.SUPPLIER_EDESC.ToLower().Contains(filter));
                        var StartWithSupplierName = SupplierNames.Where(x => x.SUPPLIER_EDESC.ToLower().StartsWith(filter.Trim())).ToList();
                        var startWithNames = StartWithSupplierName.Select(x => x.SUPPLIER_EDESC.ToLower()).ToList();
                        var EndWithSupplierName = SupplierNames.Where(x => !startWithNames.Contains(x.SUPPLIER_EDESC.ToLower()) && x.SUPPLIER_EDESC.ToLower().EndsWith(filter.Trim()));
                        var endWithNames = EndWithSupplierName.Select(x => x.SUPPLIER_EDESC.ToLower()).ToList();
                        var ContainsSupplierName = SupplierNames.Where(x => !startWithNames.Contains(x.SUPPLIER_EDESC.ToLower()) && !endWithNames.Contains(x.SUPPLIER_EDESC.ToLower()));
                        StartWithSupplierName.ForEach(s => s.Type = "SupplierName");
                        EndWithSupplierName.ToList().ForEach(s => s.Type = "SupplierName");
                        ContainsSupplierName.ToList().ForEach(s => s.Type = "SupplierName");
                        Filterdata.AddRange(StartWithSupplierName);
                        Filterdata.AddRange(ContainsSupplierName);
                        Filterdata.AddRange(EndWithSupplierName);
                        AllFilterSupplier.RemoveAll(x => x.SUPPLIER_EDESC.ToLower().Contains(filter));
                        var Address = AllFilterSupplier.Where(x => x.REGD_OFFICE_EADDRESS.ToLower().Contains(filter));
                        var StartWithAddressName = Address.Where(x => x.REGD_OFFICE_EADDRESS.ToLower().StartsWith(filter.Trim())).ToList();
                        var startWithAddress = StartWithAddressName.Select(x => x.REGD_OFFICE_EADDRESS.ToLower()).ToList();
                        var EndWithAddressName = Address.Where(x => !startWithAddress.Contains(x.REGD_OFFICE_EADDRESS.ToLower()) && x.REGD_OFFICE_EADDRESS.ToLower().EndsWith(filter.Trim()));
                        var endWithAddress = EndWithAddressName.Select(x => x.REGD_OFFICE_EADDRESS.ToLower()).ToList();
                        var ContainsAddressName = Address.Where(x => !startWithAddress.Contains(x.REGD_OFFICE_EADDRESS.ToLower()) && !endWithAddress.Contains(x.REGD_OFFICE_EADDRESS.ToLower()));
                        StartWithAddressName.ForEach(s => s.Type = "Address");
                        EndWithAddressName.ToList().ForEach(s => s.Type = "Address");
                        ContainsAddressName.ToList().ForEach(s => s.Type = "Address");
                        Filterdata.AddRange(StartWithAddressName);
                        Filterdata.AddRange(ContainsAddressName);
                        Filterdata.AddRange(EndWithAddressName);
                        AllFilterSupplier.RemoveAll(x => x.REGD_OFFICE_EADDRESS.ToLower().Contains(filter));
                        var PhoneNoNames = AllFilterSupplier.Where(x => x.TEL_MOBILE_NO1.ToLower().Contains(filter));
                        var StartWithPhoneNoName = PhoneNoNames.Where(x => x.TEL_MOBILE_NO1.ToLower().StartsWith(filter.Trim())).ToList();
                        var startWithPhone = StartWithPhoneNoName.Select(x => x.TEL_MOBILE_NO1.ToLower()).ToList();
                        var EndWithPhoneNoName = PhoneNoNames.Where(x => !startWithPhone.Contains(x.TEL_MOBILE_NO1.ToLower()) && x.TEL_MOBILE_NO1.ToLower().EndsWith(filter.Trim()));
                        var endWithPhone = EndWithPhoneNoName.Select(x => x.TEL_MOBILE_NO1.ToLower()).ToList();
                        var ContainsPhoneNoName = PhoneNoNames.Where(x => !startWithPhone.Contains(x.TEL_MOBILE_NO1.ToLower()) && !endWithPhone.Contains(x.TEL_MOBILE_NO1.ToLower()));
                        StartWithPhoneNoName.ForEach(s => s.Type = "PhoneNo");
                        EndWithPhoneNoName.ToList().ForEach(s => s.Type = "PhoneNo");
                        ContainsPhoneNoName.ToList().ForEach(s => s.Type = "PhoneNo");
                        Filterdata.AddRange(StartWithPhoneNoName);
                        Filterdata.AddRange(ContainsPhoneNoName);
                        Filterdata.AddRange(EndWithPhoneNoName);
                        this._cacheManager.Set($"AllFilterSupplier_{userid}_{company_code}_{branch_code}_{filter}", Filterdata, 20);
                        return Filterdata;
                    }
                    return AllFilterSupplier;
                }
            }
            return Filterdata;
        }
        [HttpGet]
        public List<Currency> GetCurrencyListByFlter(string filter)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<Currency>();
            if (this._cacheManager.IsSet($"GetCurrencyListByFlter_{userid}_{company_code}_{branch_code}_{filter}"))
            {
                var data = _cacheManager.Get<List<Currency>>($"GetCurrencyListByFlter_{userid}_{company_code}_{branch_code}_{filter}");
                response = data;
            }
            else
            {
                var CurrencyList = this._entryRepo.getCurrencyListByFlter(filter);
                this._cacheManager.Set($"GetCurrencyListByFlter_{userid}_{company_code}_{branch_code}_{filter}", CurrencyList, 20);
                response = CurrencyList;
            }
            return response;
        }
        [HttpGet]
        public List<AccountSetup> GetAllAccountSetupByFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
                return new List<AccountSetup>();

            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var Filterdata = new List<AccountSetup>();
            var ShowAdvanceAutoComplete = false;
            //Constants.
            var setting = _settingService.LoadSetting<WebPrefrenceSetting>(Constants.WebPrefranceSetting);
            if (setting != null)
                bool.TryParse(setting.ShowAdvanceAutoComplete, out ShowAdvanceAutoComplete);
            if (ShowAdvanceAutoComplete == false)
            {

                    if (this._cacheManager.IsSet($"{PM}_AllFilterAccount_{userid}_{company_code}_{branch_code}_{filter}_{ShowAdvanceAutoComplete}"))
                    {
                    var data = _cacheManager.Get<List<AccountSetup>>($"{PM}_AllFilterAccount_{userid}_{company_code}_{branch_code}_{filter}_{ShowAdvanceAutoComplete}");
                    Filterdata = data;
                    return Filterdata;
                }
                else
                {
                    var AllFilterAccount = this._entryRepo.getALLAccountSetupByFlter(filter);
                    this._cacheManager.Set($"{PM}_AllFilterAccount_{userid}_{company_code}_{branch_code}_{filter}_{ShowAdvanceAutoComplete}", AllFilterAccount, 20);
                    return AllFilterAccount;
                }

            }
            if (filter == "!@$")
            {
                return Filterdata;
            }
            if (filter == null)
                filter = "";
            if (filter.Contains("#"))
            {
                var colName = filter.Split('#')[0].ToString().ToLower();
                filter = filter.Split('#')[1].ToString().ToLower();
                if (colName == "code")
                {
                    if (this._cacheManager.IsSet($"AllAccountSetupByCode_{userid}_{company_code}_{branch_code}_{filter}"))
                    {
                        var data = _cacheManager.Get<List<AccountSetup>>($"AllAccountSetupByCode_{userid}_{company_code}_{branch_code}_{filter}");
                        Filterdata = data;
                    }
                    else
                    {
                        var AllFilterAccount = this._entryRepo.getALLAccountSetupByFlter(filter);
                        var AccountCodes = AllFilterAccount.Where(x => x.ACC_CODE.ToLower().Contains(filter));
                        var StartWithAccountCode = AccountCodes.Where(x => x.ACC_CODE.ToLower().StartsWith(filter.Trim())).ToList();
                        var startWithCodes = StartWithAccountCode.Select(x => x.ACC_CODE.ToLower()).ToList();
                        var EndWithAccountCode = AccountCodes.Where(x => !startWithCodes.Contains(x.ACC_CODE.ToLower()) && x.ACC_CODE.ToLower().EndsWith(filter.Trim()));
                        var endWithCodes = EndWithAccountCode.Select(x => x.ACC_EDESC.ToLower()).ToList();
                        var ContainsAccountCode = AccountCodes.Where(x => !startWithCodes.Contains(x.ACC_CODE.ToLower()) && !endWithCodes.Contains(x.ACC_CODE.ToLower()));
                        Filterdata.AddRange(StartWithAccountCode);
                        Filterdata.AddRange(ContainsAccountCode);
                        Filterdata.AddRange(EndWithAccountCode);
                        Filterdata.ForEach(s => s.Type = "AccountCode");
                        this._cacheManager.Set($"AllAccountSetupByCode_{userid}_{company_code}_{branch_code}_{filter}", Filterdata, 20);
                    }

                    return Filterdata;
                }
                if (colName == "name")
                {

                        if (this._cacheManager.IsSet($"{PM}_AllAccountSetupByName_{userid}_{company_code}_{branch_code}_{filter}"))
                        {
                        var data = _cacheManager.Get<List<AccountSetup>>($"AllAccountSetupByName_{userid}_{company_code}_{branch_code}_{filter}");
                        Filterdata = data;
                    }
                    else
                    {
                        var AllFilterAccount = this._entryRepo.getALLAccountSetupByFlter(filter);
                        var AccountNames = AllFilterAccount.Where(x => x.ACC_EDESC.ToLower().Contains(filter));
                        var StartWithAccountName = AccountNames.Where(x => x.ACC_EDESC.ToLower().StartsWith(filter.Trim())).ToList();
                        var startWithNames = StartWithAccountName.Select(x => x.ACC_EDESC.ToLower()).ToList();
                        var EndWithAccountName = AccountNames.Where(x => !startWithNames.Contains(x.ACC_EDESC.ToLower()) && x.ACC_EDESC.ToLower().EndsWith(filter.Trim()));
                        var endWithNames = EndWithAccountName.Select(x => x.ACC_EDESC.ToLower()).ToList();
                        var ContainsAccountName = AccountNames.Where(x => !startWithNames.Contains(x.ACC_EDESC.ToLower()) && !endWithNames.Contains(x.ACC_EDESC.ToLower()));
                        StartWithAccountName.ForEach(s => s.Type = "AccountName");
                        EndWithAccountName.ToList().ForEach(s => s.Type = "AccountName");
                        ContainsAccountName.ToList().ForEach(s => s.Type = "AccountName");
                        Filterdata.AddRange(StartWithAccountName);
                        Filterdata.AddRange(ContainsAccountName);
                        Filterdata.AddRange(EndWithAccountName);
                        this._cacheManager.Set($"AllAccountSetupByName_{userid}_{company_code}_{branch_code}_{filter}", Filterdata, 20);
                    }

                    return Filterdata;
                }
            }
            else
            {
                if (this._cacheManager.IsSet($"{PM}_AllFilterAccount_{userid}_{company_code}_{branch_code}_{filter}"))
                {
                    var data = _cacheManager.Get<List<AccountSetup>>($"AllFilterAccount_{userid}_{company_code}_{branch_code}_{filter}");
                    Filterdata = data;
                }
                else
                {
                    var AllFilterAccount = this._entryRepo.getALLAccountSetupByFlter(filter);
                    if (AllFilterAccount.Count >= 1)
                    {
                        var AccountCodes = AllFilterAccount.Where(x => x.ACC_CODE.ToLower().Contains(filter));
                        var StartWithAccountCode = AccountCodes.Where(x => x.ACC_CODE.ToLower().StartsWith(filter.Trim())).ToList();
                        var startWithCodes = StartWithAccountCode.Select(x => x.ACC_CODE.ToLower()).ToList();
                        var EndWithAccountCode = AccountCodes.Where(x => !startWithCodes.Contains(x.ACC_CODE.ToLower()) && x.ACC_CODE.ToLower().EndsWith(filter.Trim()));
                        var endWithCodes = EndWithAccountCode.Select(x => x.ACC_CODE.ToLower()).ToList();
                        var ContainsAccountCode = AccountCodes.Where(x => !startWithCodes.Contains(x.ACC_CODE.ToLower()) && !endWithCodes.Contains(x.ACC_CODE.ToLower()));
                        Filterdata.AddRange(StartWithAccountCode);
                        Filterdata.AddRange(ContainsAccountCode);
                        Filterdata.AddRange(EndWithAccountCode);
                        Filterdata.ForEach(s => s.Type = "AccountCode");
                        var Removedata = AllFilterAccount.RemoveAll(x => x.ACC_CODE.ToLower().Contains(filter));
                        var AccountNames = AllFilterAccount.Where(x => x.ACC_EDESC.ToLower().Contains(filter));
                        var StartWithAccountName = AccountNames.Where(x => x.ACC_EDESC.ToLower().StartsWith(filter.Trim())).ToList();
                        var startWithNames = StartWithAccountName.Select(x => x.ACC_EDESC.ToLower()).ToList();
                        var EndWithAccountName = AccountNames.Where(x => !startWithNames.Contains(x.ACC_EDESC.ToLower()) && x.ACC_EDESC.ToLower().EndsWith(filter.Trim()));
                        var endWithNames = EndWithAccountName.Select(x => x.ACC_EDESC.ToLower()).ToList();
                        var ContainsAccountName = AccountNames.Where(x => !startWithNames.Contains(x.ACC_EDESC.ToLower()) && !endWithNames.Contains(x.ACC_EDESC.ToLower()));
                        StartWithAccountName.ForEach(s => s.Type = "AccountName");
                        EndWithAccountName.ToList().ForEach(s => s.Type = "AccountName");
                        ContainsAccountName.ToList().ForEach(s => s.Type = "AccountName");
                        Filterdata.AddRange(StartWithAccountName);
                        Filterdata.AddRange(ContainsAccountName);
                        Filterdata.AddRange(EndWithAccountName);
                        this._cacheManager.Set($"AllFilterAccount_{userid}_{company_code}_{branch_code}_{filter}", Filterdata, 20);
                        return Filterdata;
                    }
                    return AllFilterAccount;
                }
            }
            return Filterdata;
        }
        [HttpGet]
        public List<AccountSetup> GetAllAccountForBud()
        {
            var result = this._entryRepo.getALLAccountForInvBudgetTrans();
            return result;
        }
        [HttpGet]
        public string getSubledgerCodeByAccCode(string accCode)
        {
            var result = this._entryRepo.getSubledgerCodeByAccCode(accCode);
            return result;
        }
        [HttpGet]
        public List<BudgetCenter> GetAllBudgetCenterForLocationByFilter(string filter)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<BudgetCenter>();
            if (this._cacheManager.IsSet($"GetAllBudgetCenterForLocationByFilter_{userid}_{company_code}_{branch_code}_{filter}"))
            {
                var data = _cacheManager.Get<List<BudgetCenter>>($"GetAllBudgetCenterForLocationByFilter_{userid}_{company_code}_{branch_code}_{filter}");
                response = data;
            }
            else
            {
                var AllBudgetCenterForLocationByFilterList = this._entryRepo.GetAllBudgetCenterForLocationByFilter(filter);
                this._cacheManager.Set($"GetAllBudgetCenterForLocationByFilter_{userid}_{company_code}_{branch_code}_{filter}", AllBudgetCenterForLocationByFilterList, 20);
                response = AllBudgetCenterForLocationByFilterList;
            }
            return response;
        }
        [HttpGet]
        public List<BudgetCenter> GetAllBudgetCenterChildByFilter(string filter, string accCode)
        {
            var a = new List<BudgetCenter>();
            return a;
        }
        [HttpGet]
        public List<SubLedger> GetAllSubLedgerByFilter(string filter, string accCode)
        {
            var Filterdata = new List<SubLedger>();
            if (filter == null)
                filter = "";
            if (filter.Contains("#"))
            {
                var colName = filter.Split('#')[0].ToString().ToLower();
                filter = filter.Split('#')[1].ToString().ToLower();
                if (colName == "code")
                {
                    var AllFilterSubledger = this._entryRepo.GetAllSubLedgerByFilter(filter, accCode);
                    var SubledgerCodes = AllFilterSubledger.Where(x => x.SUB_EDESC.ToLower().Contains(filter));
                    var StartWithSubledgerCode = SubledgerCodes.Where(x => x.SUB_CODE.ToLower().StartsWith(filter.Trim())).ToList();
                    var startWithCodes = StartWithSubledgerCode.Select(x => x.SUB_CODE.ToLower()).ToList();
                    var EndWithSubledgerCode = SubledgerCodes.Where(x => !startWithCodes.Contains(x.SUB_CODE.ToLower()) && x.SUB_CODE.ToLower().EndsWith(filter.Trim()));
                    var endWithCodes = EndWithSubledgerCode.Select(x => x.SUB_EDESC.ToLower()).ToList();
                    var ContainsSubledgerCode = SubledgerCodes.Where(x => !startWithCodes.Contains(x.SUB_CODE.ToLower()) && !endWithCodes.Contains(x.SUB_CODE.ToLower()));

                    Filterdata.AddRange(StartWithSubledgerCode);
                    Filterdata.AddRange(ContainsSubledgerCode);
                    Filterdata.AddRange(EndWithSubledgerCode);
                    Filterdata.ForEach(s => s.Type = "Code");
                    return Filterdata;
                }
                if (colName == "name")
                {
                    var AllFilterSubledger = this._entryRepo.GetAllSubLedgerByFilter(filter, accCode);
                    var SubledgerNames = AllFilterSubledger.Where(x => x.SUB_EDESC.ToLower().Contains(filter));
                    var StartWithSubledgerName = SubledgerNames.Where(x => x.SUB_EDESC.ToLower().StartsWith(filter.Trim())).ToList();
                    var startWithNames = StartWithSubledgerName.Select(x => x.SUB_EDESC.ToLower()).ToList();
                    var EndWithSubledgerName = SubledgerNames.Where(x => !startWithNames.Contains(x.SUB_EDESC.ToLower()) && x.SUB_EDESC.ToLower().EndsWith(filter.Trim()));
                    var endWithNames = EndWithSubledgerName.Select(x => x.SUB_EDESC.ToLower()).ToList();
                    var ContainsSubledgerName = SubledgerNames.Where(x => !startWithNames.Contains(x.SUB_EDESC.ToLower()) && !endWithNames.Contains(x.SUB_EDESC.ToLower()));
                    StartWithSubledgerName.ForEach(s => s.Type = "Name");
                    EndWithSubledgerName.ToList().ForEach(s => s.Type = "Name");
                    ContainsSubledgerName.ToList().ForEach(s => s.Type = "Name");
                    Filterdata.AddRange(StartWithSubledgerName);
                    Filterdata.AddRange(ContainsSubledgerName);
                    Filterdata.AddRange(EndWithSubledgerName);
                    return Filterdata;
                }
            }
            else
            {
                var AllFilterSubledger = this._entryRepo.GetAllSubLedgerByFilter(filter, accCode);
                if (AllFilterSubledger.Count >= 1)
                {
                    var SubledgerCodes = AllFilterSubledger.Where(x => x.SUB_CODE.ToLower().Contains(filter));
                    var StartWithSubledgerCode = SubledgerCodes.Where(x => x.SUB_CODE.ToLower().StartsWith(filter.Trim())).ToList();
                    var startWithCodes = StartWithSubledgerCode.Select(x => x.SUB_CODE.ToLower()).ToList();
                    var EndWithSubledgerCode = SubledgerCodes.Where(x => !startWithCodes.Contains(x.SUB_CODE.ToLower()) && x.SUB_CODE.ToLower().EndsWith(filter.Trim()));
                    var endWithCodes = EndWithSubledgerCode.Select(x => x.SUB_EDESC.ToLower()).ToList();
                    var ContainsSubledgerCode = SubledgerCodes.Where(x => !startWithCodes.Contains(x.SUB_CODE.ToLower()) && !endWithCodes.Contains(x.SUB_CODE.ToLower()));
                    Filterdata.AddRange(StartWithSubledgerCode);
                    Filterdata.AddRange(ContainsSubledgerCode);
                    Filterdata.AddRange(EndWithSubledgerCode);
                    Filterdata.ForEach(s => s.Type = "Code");
                    var Removedata = AllFilterSubledger.RemoveAll(x => x.SUB_CODE.ToLower().Contains(filter));
                    var SubledgerNames = AllFilterSubledger.Where(x => x.SUB_EDESC.ToLower().Contains(filter));
                    var StartWithSubledgerName = SubledgerNames.Where(x => x.SUB_EDESC.ToLower().StartsWith(filter.Trim())).ToList();
                    var startWithNames = StartWithSubledgerName.Select(x => x.SUB_EDESC.ToLower()).ToList();
                    var EndWithSubledgerName = SubledgerNames.Where(x => !startWithNames.Contains(x.SUB_EDESC.ToLower()) && x.SUB_EDESC.ToLower().EndsWith(filter.Trim()));
                    var endWithNames = EndWithSubledgerName.Select(x => x.SUB_EDESC.ToLower()).ToList();
                    var ContainsSubledgerName = SubledgerNames.Where(x => !startWithNames.Contains(x.SUB_EDESC.ToLower()) && !endWithNames.Contains(x.SUB_EDESC.ToLower()));
                    StartWithSubledgerName.ForEach(s => s.Type = "Name");
                    EndWithSubledgerName.ToList().ForEach(s => s.Type = "Name");
                    ContainsSubledgerName.ToList().ForEach(s => s.Type = "Name");
                    Filterdata.AddRange(StartWithSubledgerName);
                    Filterdata.AddRange(ContainsSubledgerName);
                    Filterdata.AddRange(EndWithSubledgerName);
                    return Filterdata;
                }
                return AllFilterSubledger;
            }
            return Filterdata;
        }
        [HttpGet]
        public List<TemplateDraftModel> GetDraftList(string moduleCode, string formCode)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<TemplateDraftModel>();
            if (this._cacheManager.IsSet($"GetDraftList_{userid}_{company_code}_{branch_code}_{moduleCode}_{formCode}"))
            {
                var data = _cacheManager.Get<List<TemplateDraftModel>>($"GetDraftList_{userid}_{company_code}_{branch_code}_{moduleCode}_{formCode}");
                response = data;
            }
            else
            {
                var DraftList = this._entryRepo.GetDraftList(moduleCode, formCode);
                this._cacheManager.Set($"GetDraftList_{userid}_{company_code}_{branch_code}_{moduleCode}_{formCode}", DraftList, 20);
                response = DraftList;
            }
            return response;
        }
        [HttpPost]
        public HttpResponseMessage insertQuickSetup(QuickSetupModel model)
        {
            try
            {

                var result = this._entryRepo.InsertQuickSetup(model);
                if (result == "C_SUCCESS")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetChildOfCustomerByGroup");
                    keystart.Add("getAllItemsForCustomerStock");
                    keystart.Add("getAllComboEmployees");
                    keystart.Add("getAllComboDealers");
                    keystart.Add("GetCustomers");
                    keystart.Add("AllCustomerSetupByCode");
                    keystart.Add("AllCustomerSetupByName");
                    keystart.Add("AllCustomerSetupByAddress");
                    keystart.Add("AllCustomerSetupByPhoneno");
                    keystart.Add("AllFilterCustomer");
                    keystart.Add("customerDropDownForGroupPopup");
                    keystart.Add("GetCustomerListByCustomerCode");
                    keystart.Add("GetAllCustomerSetupByFilter");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "C_SUCCESS", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (result == "I_SUCCESS")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetChildOfItemByGroup");
                    keystart.Add("GetProducts");
                    keystart.Add("GetItemChargeDataSavedValueWise");
                    keystart.Add("GetItemChargeDataSavedQuantityWise");
                    keystart.Add("GetInvItemChargesData");
                    keystart.Add("GetMUCodeByProductId");
                    keystart.Add("GetMuCode");
                    keystart.Add("GetProductListByItemCode");
                    keystart.Add("GetAllProductsListByFilter");
                    keystart.Add("getProductCodeWithChild");
                    keystart.Add("GetGroupProducts");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "I_SUCCESS", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (result == "S_SUCCESS")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetChildOfsupplierByGroup");
                    keystart.Add("GetSuppliers");
                    keystart.Add("AllSupplierForReferenceByCode");
                    keystart.Add("AllSupplierForReferenceByName");
                    keystart.Add("AllSupplierForReferenceByaddress");
                    keystart.Add("AllSupplierForReferenceByphoneno");
                    keystart.Add("AllFilterSupplierForReference");
                    keystart.Add("AllSupplierSetupByCode");
                    keystart.Add("AllSupplierSetupByName");
                    keystart.Add("AllSupplierSetupByAddress");
                    keystart.Add("AllSupplierSetupByPhoneno");
                    keystart.Add("AllFilterSupplier");
                    keystart.Add("getsupplierCodeWithChild");
                    keystart.Add("GetSupplierListBySupplierCode");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "S_SUCCESS", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpPost]
        public HttpResponseMessage SaveInventoryFormData(FormDetails model)
        {
            _logErp.InfoInFile("Inventory voucher Form data save method started=============");
            using (var trans = _objectEntity.Database.BeginTransaction())
            {
                try
                {
                    var voucherno = model.Order_No;

                    // This For Get Primary Date Column from all table
                    var primarydatecolumn = _entryRepo.GetPrimaryDateByTableName(model.Table_Name);

                    // This For Ger Primary column Etc VoucherNo,Sales_no
                    var primarycolname = _entryRepo.GetPrimaryColumnByTableName(model.Table_Name);

                    string primarydate = string.Empty, primarycolumn = string.Empty, today = DateTime.Now.ToString("dd-MMM-yyyy"), createddatestring = "SYSDATE", todaystring = System.DateTime.Now.ToString("yyyyMMddHHmmss"), manualno = string.Empty, currencyformat = "NRS", VoucherDate = createddatestring, grandtotal = model.Grand_Total;
                    // Get New VoucherNo from Database
                    string newvoucherNo = _FormTemplateRepo.NewVoucherNo(this._workContext.CurrentUserinformation.company_code, model.Form_Code, today, model.Table_Name);
                    decimal exchangrate = 1;
                    var VoucherNumberGeneratedNo = string.Empty;
                    //DeserializeObject Master 
                    var masterColumn = _entryRepo.MapMasterColumnWithValue(model.Master_COLUMN_VALUE);
                    //DeserializeObject Child 
                    var childColumn = _entryRepo.MapChildColumnWithValue(model.Child_COLUMN_VALUE);
                    //var customTran = _entryRepo.MapCustomTransactionWithValue(model.Custom_COLUMN_VALUE);
                    var customColumn = new Newtonsoft.Json.Linq.JObject();
                    var customColList = new List<CustomOrderColumn>();
                    //DeserializeObject Customer value
                    if (model.Custom_COLUMN_VALUE != null)
                    {

                        customColList = _entryRepo.MapCustomTransactionWithValue(model.Custom_COLUMN_VALUE);
                    }
                    var budgetTransaction = _entryRepo.MapBudgetTransactionColumnValue(model.BUDGET_TRANS_VALUE);
                    var batchTransaction = _entryRepo.MapBatchTransactionValue(model.SERIAL_TRACKING_VALUE);
                    var batchTransValues = _entryRepo.MapBatchTransValue(model.BATCH_TRACKING_VALUE);
                    var charges = _entryRepo.MapChargesColumnWithValue(model.CHARGES);
                    string createdDateForEdit = string.Empty, createdByForEdit = string.Empty, voucherNoForEdit = string.Empty;
                    bool insertedToChild = false;
                    bool insertedToMaster = false;
                    var msg = string.Empty;
                    var sessionRowIDForedit = 0;
                    var shippingDetailValues = _entryRepo.MapShippingDetailsColumnValue(model.SHIPPING_DETAILS_VALUE);
                    _logErp.WarnInDB("Shipping details for sales order : " + shippingDetailValues);
                    if (voucherno == "undefined")
                    {
                        //string updateTransactionNo = _entryRepo.NewVoucherNo(this._workContext.CurrentUserinformation.company_code, model.Form_Code, DateTime.Now.ToString("dd-MMM-yyyy"), model.Table_Name);
                        VoucherNumberGeneratedNo = newvoucherNo;
                        var commonValue = new CommonFieldsForInventory
                        {
                            FormCode = model.Form_Code,
                            TableName = model.Table_Name,
                            ExchangeRate = exchangrate,
                            CurrencyFormat = currencyformat,
                            VoucherNumber = voucherno,
                            NewVoucherNumber = VoucherNumberGeneratedNo,
                            TempCode = model.TempCode,
                            VoucherDate = VoucherDate,
                            Grand_Total = model.Grand_Total,
                            FormRef = model.FROM_REF,
                            ManualNumber = masterColumn.MANUAL_NO,
                            MODULE_CODE = model.MODULE_CODE
                        };
                        masterColumn.CREATED_BY = _workContext.CurrentUserinformation.login_code.ToUpper();
                        if (childColumn.Count > 0)
                        {
                            insertedToChild = _entryRepo.SaveChildColumnValue(childColumn, masterColumn, commonValue, model, primarydatecolumn, primarycolname, _objectEntity);

                        }
                        if (insertedToChild)
                        {
                            insertedToMaster = _entryRepo.SaveMasterColumnValue(masterColumn, commonValue, _objectEntity);
                        }
                        if (commonValue.FormRef)
                        {
                            foreach (var refa in model.REF_MODEL)
                            {
                                var childData = childColumn.Where(x => x.ITEM_CODE == refa.ITEM_CODE).FirstOrDefault();
                                //refa.SERIAL_NO = childData.SERIAL_NO.ToString();
                                refa.Qty = childData.QUANTITY;
                                refa.calc_qty = childData.CALC_QUANTITY;
                                refa.cal_unit_price = childData.CALC_UNIT_PRICE;
                                refa.cal_total_price = childData.CALC_TOTAL_PRICE;
                                refa.Total_price = childData.TOTAL_PRICE ?? 0;
                                refa.SUB_PROJECT_CODE = childData.SUB_PROJECT_CODE;
                            }

                            //_entryRepo.GetFormReference(childColumn, masterColumn, commonValue, model, primarydatecolumn, primarycolname, _objectEntity);
                            _entryRepo.GetFormReference(commonValue, model.REF_MODEL, _objectEntity);
                        }
                        if (budgetTransaction != null)
                        {
                            for (int i = 0; i < budgetTransaction.Count; i++)
                            {
                                budgetTransaction[i].SUB_PROJECT_CODE = childColumn[i].SUB_PROJECT_CODE;
                            }

                            _entryRepo.SaveBudgetTransactionColumnValue(budgetTransaction, commonValue, _objectEntity);
                            //_entryRepo.SaveBudgetTransactionColumnValue(budgetTransaction, commonValue, _objectEntity);
                        }

                        if (batchTransaction != null)
                        {
                            _entryRepo.SaveBatchTransactionValues(masterColumn, batchTransaction, commonValue, _objectEntity);
                        }
                        if (batchTransValues != null)
                        {
                            _entryRepo.SaveBatchTransValues(childColumn, masterColumn, batchTransValues, commonValue, _objectEntity);
                        }
                        if (charges != null)
                        {
                            _entryRepo.SaveChargeColumnValue(charges, commonValue, _objectEntity);
                        }
                        if (customColList.Count > 0)
                        {
                            _entryRepo.SaveCustomTransaction(customColList, commonValue, _objectEntity);
                        }
                        if (shippingDetailValues != null)
                        {
                            _entryRepo.SaveShippingDetailsColumnValue(shippingDetailValues, commonValue, _objectEntity);
                        }
                        #region CLEAR CACHE
                        List<string> keystart = new List<string>();
                        keystart.Add("GetAllMenuItems");
                        keystart.Add("GetAllSalesOrderDetails");
                        keystart.Add("GetSalesOrderDetailFormDetailByFormCodeAndOrderNo");
                        keystart.Add("GetAllOrederNoByFlter");
                        keystart.Add("GetSubMenuList");
                        keystart.Add("GetSubMenuDetailList");
                        List<string> Record = new List<string>();
                        Record = this._cacheManager.GetAllKeys();
                        this._cacheManager.RemoveCacheByKey(keystart, Record);
                        #endregion
                        if (model.Save_Flag == "0" && insertedToChild == true && insertedToMaster == true)
                        {
                            trans.Commit();
                            msg = "INSERTED";
                        }
                        else if (model.Save_Flag == "3" && insertedToChild == true && insertedToMaster == true)
                        {
                            trans.Commit();
                            msg = "SAVEANDPRINT";
                        }
                        else
                        {
                            trans.Commit();
                            msg = "INSERTEDANDCONTINUE";
                        }
                    }
                    //for updates 
                    else
                    {
                        bool updatedChild = false;
                        bool updatedMaster = false;

                        var defaultData = _entryRepo.GetMasterTransactionByVoucherNo(voucherno);
                        foreach (var def in defaultData)
                        {
                            voucherNoForEdit = def.VOUCHER_NO.ToString();
                            createdDateForEdit = "TO_DATE('" + def.CREATED_DATE.ToString() + "', 'DD-MON-YY hh12:mi:ss PM')";
                            createdByForEdit = def.CREATED_BY.ToString().ToUpper();
                            //sessionRowIDForedit = Convert.ToInt32(def.SESSION_ROWID);
                        }
                        var commonUpdateValue = new CommonFieldsForInventory
                        {
                            FormCode = model.Form_Code,
                            TableName = model.Table_Name,
                            ExchangeRate = exchangrate,
                            CurrencyFormat = currencyformat,
                            VoucherNumber = voucherno,
                            NewVoucherNumber = voucherno,
                            TempCode = model.TempCode,
                            VoucherDate = createdDateForEdit,
                            PrimaryColumn = primarycolname,
                            PrimaryDateColumn = primarydatecolumn,
                            DrTotal = Convert.ToDecimal(model.Grand_Total),
                            ManualNumber = masterColumn.MANUAL_NO,
                            MODULE_CODE = model.MODULE_CODE

                        };
                        masterColumn.MODIFY_DATE = VoucherDate;
                        masterColumn.MODIFY_BY = _workContext.CurrentUserinformation.login_code.ToUpper();
                        masterColumn.CREATED_BY = createdByForEdit;
                        _entryRepo.DeleteChildTransaction(commonUpdateValue, _objectEntity);
                        updatedChild = _entryRepo.SaveChildColumnValue(childColumn, masterColumn, commonUpdateValue, model, primarydatecolumn, primarycolname, _objectEntity);

                        if (updatedChild)
                        {
                            updatedMaster = _entryRepo.UpdateMasterTransaction(commonUpdateValue, _objectEntity);
                        }
                        if (budgetTransaction != null)
                        {
                            _entryRepo.DeleteBudgetTransaction(commonUpdateValue.VoucherNumber, _objectEntity);
                            _entryRepo.SaveBudgetTransactionColumnValue(budgetTransaction, commonUpdateValue, _objectEntity);
                        }
                        if (batchTransaction != null)
                        {
                            _entryRepo.DeleteBatchTransaction(commonUpdateValue.VoucherNumber, _objectEntity);
                            _entryRepo.SaveBatchTransactionValues(masterColumn, batchTransaction, commonUpdateValue, _objectEntity);
                        }
                        if (batchTransValues != null)
                        {
                            _entryRepo.DeleteBatchTransaction(commonUpdateValue.VoucherNumber, _objectEntity);
                            _entryRepo.SaveBatchTransValues(childColumn, masterColumn, batchTransValues, commonUpdateValue, _objectEntity);
                        }
                        if (charges != null)
                        {
                            _entryRepo.DeleteChargeTransaction(commonUpdateValue.VoucherNumber, _objectEntity);
                            _entryRepo.SaveChargeColumnValue(charges, commonUpdateValue, _objectEntity);
                        }
                        if (customColList.Count > 0)
                        {
                            _entryRepo.DeleteCustomTransaction(commonUpdateValue.VoucherNumber, _objectEntity);
                            _entryRepo.SaveCustomTransaction(customColList, commonUpdateValue, _objectEntity);
                        }
                        if (shippingDetailValues != null)
                        {
                            _entryRepo.UpdateShippingDetailsColumnValue(shippingDetailValues, commonUpdateValue, _objectEntity);
                        }
                        #region CLEAR CACHE
                        List<string> keystart = new List<string>();
                        keystart.Add("GetAllMenuItems");
                        keystart.Add("GetAllSalesOrderDetails");
                        keystart.Add("GetSalesOrderDetailFormDetailByFormCodeAndOrderNo");
                        keystart.Add("GetAllOrederNoByFlter");
                        keystart.Add("GetSubMenuList");
                        keystart.Add("GetSubMenuDetailList");
                        List<string> Record = new List<string>();
                        Record = this._cacheManager.GetAllKeys();
                        this._cacheManager.RemoveCacheByKey(keystart, Record);
                        #endregion
                        if (model.Save_Flag == "0" && updatedChild == true && updatedMaster == true)
                        {
                            trans.Commit();
                            msg = "UPDATED";
                        }
                        else if (model.Save_Flag == "4" && updatedChild == true && updatedMaster == true)
                        {
                            trans.Commit();
                            msg = "UPDATEDANDPRINT";
                        }
                        else
                        {
                            trans.Rollback();
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                        }
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = msg, STATUS_CODE = (int)HttpStatusCode.OK, VoucherNo = VoucherNumberGeneratedNo, SessionNo = newvoucherNo, VoucherDate = primarydate, FormCode = model.Form_Code });
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    _logErp.ErrorInDB("Error while saving Inventory voucher: " + ex.Message);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                    //throw new Exception(ex.Message);
                }
            }
        }
        [HttpGet]
        public List<ShippingDetailsViewModel> GetAllShippingDtlsByFilter(string FormCode, string VoucherNo)
        {

            var response = _entryRepo.GetShippingData(FormCode, VoucherNo);
            return response;
        }
        [HttpGet]
        public List<Products> GetMUCodeByProductId(string productId)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<Products>();
            //if (this._cacheManager.IsSet($"GetMUCodeByProductId_{userid}_{company_code}_{branch_code}_{productId}"))
            //{
            //    var data = _cacheManager.Get<List<Products>>($"GetMUCodeByProductId_{userid}_{company_code}_{branch_code}_{productId}");
            //    response = data;
            //}
            //else
            //{
            //    var MUCodeByProductId = this._entryRepo.GetProductDataByProductCode(productId);
            //    this._cacheManager.Set($"GetMUCodeByProductId_{userid}_{company_code}_{branch_code}_{productId}", MUCodeByProductId, 20);
            //    response = MUCodeByProductId;
            //}
            var MUCodeByProductId = this._entryRepo.GetProductDataByProductCode(productId);
            response = MUCodeByProductId;
            return response;
        }
        [HttpGet]
        public List<FormCustomSetup> GetFormCustomSetup(string formCode, string voucherNo)
        {
            _logErp.InfoInFile("GetFormCustom Setup for formcode: " + formCode + " and voucher no : " + voucherNo + " started====");
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<FormCustomSetup>();
            if (this._cacheManager.IsSet($"GetFormCustomSetup_{userid}_{company_code}_{branch_code}_{formCode}_{voucherNo}"))
            {
                var data = _cacheManager.Get<List<FormCustomSetup>>($"GetFormCustomSetup_{userid}_{company_code}_{branch_code}_{formCode}_{voucherNo}");
                _logErp.InfoInFile(data.Count() + " custom setup has been fetched for " + formCode + " formcode and " + voucherNo + " voucher number from cached");
                response = data;
            }
            else
            {
                var FormCustomSetup = this._entryRepo.GetFormCustomSetup(formCode, voucherNo);
                _logErp.InfoInFile(FormCustomSetup.Count() + " custom setup has been fetched for " + formCode + " formcode and " + voucherNo + " voucher number");
                this._cacheManager.Set($"GetFormCustomSetup_{userid}_{company_code}_{branch_code}_{formCode}_{voucherNo}", FormCustomSetup, 20);
                response = FormCustomSetup;
            }
            return response;
        }
        [HttpPost]
        public HttpResponseMessage SaveWebPrefrence(WebPrefrence model)
        {
            try
            {

                var result = this._entryRepo.SaveWebPrefrence(model);
                //  if(result.Success)
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result.Success, STATUS_CODE = (int)HttpStatusCode.OK });


                //    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });


            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpPost]
        public HttpResponseMessage updateAreaSetup(AreaModels model)
        {
            try
            {
                var result = this._entryRepo.updateAreaSetup(model);
                if (result == "UPDATED")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetAllAreaCode");
                    keystart.Add("GetAllAreaSetupByFilter");
                    keystart.Add("getAreaCodeWithChild");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        public List<FinanceVoucherReference> GetReferenceList(string formcode)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<FinanceVoucherReference>();
            //if (this._cacheManager.IsSet($"GetReferenceList_{userid}_{company_code}_{branch_code}_{formcode}"))
            //{
            //    var data = _cacheManager.Get<List<FinanceVoucherReference>>($"GetReferenceList_{userid}_{company_code}_{branch_code}_{formcode}");
            //    response = data;
            //}
            //else
            //{
            //    var financeVoucherReferenceList = this._Reference.GetFinanceVoucherReferenceList(formcode);
            //    this._cacheManager.Set($"GetReferenceList_{userid}_{company_code}_{branch_code}_{formcode}", financeVoucherReferenceList, 20);
            //    response = financeVoucherReferenceList;
            //}
            var financeVoucherReferenceList = this._entryRepo.GetFinanceVoucherReferenceList(formcode);
            //this._cacheManager.Set($"GetReferenceList_{userid}_{company_code}_{branch_code}_{formcode}", financeVoucherReferenceList, 20);
            response = financeVoucherReferenceList;
            return response;
            // return this._Reference.GetFinanceVoucherReferenceList(formcode);
        }
        [HttpGet]
        public List<REFERENCE_DETAILS> getRefDetails(string VoucherNo, string formcode)
        {
            var response =_entryRepo.GetReference_Details_For_VoucherNo(VoucherNo, formcode);
            return response;
        }
        [HttpGet]
        public List<PartyType> GetPartyTypes()
        {
            var AllPartyTypesList = this._entryRepo.GetAllPartyTypes();
            return AllPartyTypesList;
        }
        [HttpPost]
        public List<COMMON_COLUMN> GetVoucherDetailForPRoduction(ProductionRefrence model)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<COMMON_COLUMN>();
            decimal ProductQty = 0;
            decimal.TryParse(model.Production_Qty, out ProductQty);
            var result = this._entryRepo.GetProductionFormDetail(model.FormCode, model.TableName, model.RoutingName, ProductQty);
            return result;
        }
        [HttpGet]
        public string getCustEdesc(string code)
        {
            var result = this._entryRepo.GetcustomerNameByCode(code);
            return result;
        }

        [HttpGet]
        public string getItemEdesc(string code)
        {
            var result = this._entryRepo.GetItemNameByCode(code);
            return result;
        }
        [HttpPost]
        public HttpResponseMessage DeleteInvVoucher(string voucherno, string formcode)
        {
            try
            {
                bool isposted = _entryRepo.CheckVoucherNoPosted(voucherno);
                if (isposted == true)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "POSTED", STATUS_CODE = (int)HttpStatusCode.OK, VoucherNo = voucherno });
                }


                string checkreferenced = _entryRepo.CheckVoucherNoReferenced(voucherno);
                if (checkreferenced != "" && checkreferenced != null && checkreferenced != "undefined")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "REFERENCED", STATUS_CODE = (int)HttpStatusCode.OK, VoucherNo = checkreferenced });
                }
                else
                {
                    var primarycolumn = string.Empty;
                    var fomdetails = _entryRepo.GetFormDetailSetup(formcode);
                    if (fomdetails.Count > 0)
                    {
                        primarycolumn = _entryRepo.GetPrimaryColumnByTableName(fomdetails[0].TABLE_NAME);
                    }
                    bool deleteres = _entryRepo.deletevouchernoInv(fomdetails[0].TABLE_NAME, formcode, voucherno, primarycolumn);

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "DELETED", STATUS_CODE = (int)HttpStatusCode.OK });
                }


            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpGet]
        public List<FormDetailSetup> GetFormDetailSetup(string formCode)
        {
            _logErp.InfoInFile("Get Form Details Setup for : " + formCode + " formcode");
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;

            var response = new List<FormDetailSetup>();
            //if (this._cacheManager.IsSet($"fromdetailsetup_{_workContext.CurrentUserinformation.User_id}_{company_code}_{branch_code}_{formCode}"))
            //{

            //    var data = _cacheManager.Get<List<FormDetailSetup>>($"fromdetailsetup_{_workContext.CurrentUserinformation.User_id}_{company_code}_{branch_code}_{formCode}");
            //    _logErp.InfoInFile(data.Count() + " Form Details setup has been fetched from cached for " + formCode + " formcode");
            //    response = data;
            //}
            //else
            //{
                var formDetailList = this._entryRepo.GetFormDetailSetup(formCode);
                this._cacheManager.Set($"fromdetailsetup_{_workContext.CurrentUserinformation.User_id}_{company_code}_{branch_code}_{formCode}", formDetailList, 20);
                _logErp.InfoInFile(formDetailList.Count() + " form details setup has beed fetched for " + formCode + " formcode");
                response = formDetailList;
            //}
            return response;
        }
        [HttpGet]
        public List<DraftFormModel> GetDraftFormDetail(string formCode)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<DraftFormModel>();
            //if (this._cacheManager.IsSet($"draftformdetail_{userid}_{company_code}_{branch_code}_{formCode}"))
            //{
            //    var data = _cacheManager.Get<List<DraftFormModel>>($"draftformdetail_{userid}_{company_code}_{branch_code}_{formCode}");
            //    response = data;
            //}
            //else
            //{
            //    var draftDetailList = this._FormTemplateRepo.GetDraftFormDetailSetup(formCode);
            //    this._cacheManager.Set($"draftformdetail_{userid}_{company_code}_{branch_code}_{formCode}", draftDetailList, 20);
            //    response = draftDetailList;
            //}
            var draftDetailList = this._entryRepo.GetDraftFormDetailSetup(formCode);
            response = draftDetailList;
            return response;
        }
        [HttpGet]
        public List<FormSetup> GetFormSetupByFormCode(string formCode)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<FormSetup>();
            if (this._cacheManager.IsSet($"GetFormSetupByFormCode_{userid}_{company_code}_{branch_code}_{formCode}"))
            {
                var data = _cacheManager.Get<List<FormSetup>>($"GetFormSetupByFormCode_{userid}_{company_code}_{branch_code}_{formCode}");
                response = data;
            }
            else
            {
                var fromsetup = this._entryRepo.GetFormSetupByFormCode(formCode);
                this._cacheManager.Set($"GetFormSetupByFormCode_{userid}_{company_code}_{branch_code}_{formCode}", fromsetup, 20);
                response = fromsetup;
            }
            return response;
        }
        [HttpGet]
        public List<COMMON_COLUMN> GetSalesOrderDetailFormDetailByFormCodeAndOrderNo(string formCode, string orderno)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<COMMON_COLUMN>();
            //if (this._cacheManager.IsSet($"GetSalesOrderDetailFormDetailByFormCodeAndOrderNo_{userid}_{company_code}_{branch_code}_{formCode}_{orderno}"))
            //{
            //    var data = _cacheManager.Get<List<COMMON_COLUMN>>($"GetSalesOrderDetailFormDetailByFormCodeAndOrderNo_{userid}_{company_code}_{branch_code}_{formCode}_{orderno}");
            //    response = data;
            //}
            //else
            //{
            //    var SalesOrderDetailFormDetailByFormCodeAndOrderNo = this._FormTemplateRepo.GetSalesOrderFormDetail(formCode, orderno);
            //    this._cacheManager.Set($"GetSalesOrderDetailFormDetailByFormCodeAndOrderNo_{userid}_{company_code}_{branch_code}_{formCode}_{orderno}", SalesOrderDetailFormDetailByFormCodeAndOrderNo, 20);
            //    response = SalesOrderDetailFormDetailByFormCodeAndOrderNo;
            //}
            var SalesOrderDetailFormDetailByFormCodeAndOrderNo = this._entryRepo.GetSalesOrderFormDetail(formCode, orderno);
            response = SalesOrderDetailFormDetailByFormCodeAndOrderNo;
            return response;
        }
        public List<BudgetCenter> getBudgetCodeByAccCode(string accCode)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<BudgetCenter>();
            if (this._cacheManager.IsSet($"getBudgetCodeByAccCode_{userid}_{company_code}_{branch_code}_{accCode}"))
            {
                var data = _cacheManager.Get<List<BudgetCenter>>($"getBudgetCodeByAccCode_{userid}_{company_code}_{branch_code}_{accCode}");
                response = data;
            }
            else
            {
                var BudgetCodeByAccCodeList = this._entryRepo.getBudgetCodeByAccCode(accCode);
                this._cacheManager.Set($"getBudgetCodeByAccCode_{userid}_{company_code}_{branch_code}_{accCode}", BudgetCodeByAccCodeList, 20);
                response = BudgetCodeByAccCodeList;
            }
            return response;
        }
        [HttpGet]
        public List<DraftFormModel> getDraftDataByFormCodeAndTempCode(string formCode, string TempCode)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<DraftFormModel>();
            if (this._cacheManager.IsSet($"getDraftDataByFormCodeAndTempCode_{userid}_{company_code}_{branch_code}_{formCode}_{TempCode}"))
            {
                var data = _cacheManager.Get<List<DraftFormModel>>($"getDraftDataByFormCodeAndTempCode_{userid}_{company_code}_{branch_code}_{formCode}_{TempCode}");
                response = data;
            }
            else
            {
                var DraftDataByFormCodeAndTempCode = this._entryRepo.getDraftDataByFormCodeAndTempCode(formCode, TempCode);
                this._cacheManager.Set($"getDraftDataByFormCodeAndTempCode_{userid}_{company_code}_{branch_code}_{formCode}_{TempCode}", DraftDataByFormCodeAndTempCode, 20);
                response = DraftDataByFormCodeAndTempCode;
            }
            return response;
        }
        public IHttpActionResult GetVouchersCount(string FORM_CODE, string TABLE_NAME)
        {
            try
            {
                var rslt = _entryRepo.GetTotalVoucher(FORM_CODE, TABLE_NAME);
                return Ok(rslt);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }


        }
        [HttpGet]
        public List<SubProjectData> GetSubProjectList()
        {
            List<SubProjectData> response = new List<SubProjectData>();
            try
            {
                response = _entryRepo.GetSubProjectList();
                return response;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

    }
}