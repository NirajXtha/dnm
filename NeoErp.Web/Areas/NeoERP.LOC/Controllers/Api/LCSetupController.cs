using NeoErp.LOC.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NeoERP.LOC.Models;
using NeoErp.LOC.Services.Models;

namespace NeoERP.LOC.Controllers.Api
{
    public class LCSetupController : ApiController
    {
        private ILcSetupService _lcsetupservice { get; set; }
        public LCSetupController(ILcSetupService lcsetupservice)
        {
            this._lcsetupservice = lcsetupservice;
        }

        #region LC Bank Setup
        [HttpPost]
        public IHttpActionResult CreateLCBank(LCBankModels bankdetails)
        {
            try
            {
                _lcsetupservice.AddUpdateLcBank(bankdetails);
                return Ok();
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        public IHttpActionResult GetAllBanks()
        {
            try
            {
                var result = _lcsetupservice.getAllLCBanks();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }

        }

        [HttpGet]
        public IHttpActionResult DeleteLCBanks(int BankCode)
        {
            try
            {
                _lcsetupservice.deleteLCBanks(BankCode);
                return Ok();
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }

        }

        #endregion

        #region LC Term Setup
        [HttpPost]
        public HttpResponseMessage CreateLCTerm(LCTermModels lcterm)
        {
            if (ModelState.IsValid)
            {
                _lcsetupservice.AddUpdateLcTerm(lcterm);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Error", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        public List<LCTermModels> GetAllTerms()
        {
            return _lcsetupservice.getAllLCTerms();
        }



        [HttpGet]
        public HttpResponseMessage DeleteLCTerms(int TermCode)
        {
            _lcsetupservice.deleteLCTerm(TermCode);
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
        }
        #endregion

        #region LCPayment Term Setup

        [HttpPost]
        public HttpResponseMessage CreateLCPTerm(LCPTermModels lcpterm)
        {
            if (ModelState.IsValid)
            {
                _lcsetupservice.AddUpdatePLcTerm(lcpterm);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Error", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        public List<LCPTermModels> GetAllPTerms()
        {
            return _lcsetupservice.getAllLCPTerms();
        }


        [HttpGet]
        public HttpResponseMessage DeleteLCPTerm(int PTermCode)
        {
            _lcsetupservice.deleteLCPTerm(PTermCode);
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
        }


        #endregion

        #region LC Status Setup

        [HttpPost]
        public HttpResponseMessage CreateLCStatus(LCStatusModels lcstatus)
        {
            if (ModelState.IsValid)
            {
                _lcsetupservice.AddUpdateLcStatus(lcstatus);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Error", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        public List<LCStatusModels> GetAllStatus()
        {
            return _lcsetupservice.getAllLCStatus();
        }

        [HttpGet]
        public HttpResponseMessage DeleteLCStatus(int StatusCode)
        {
            _lcsetupservice.deleteLCStatus(StatusCode);
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
        }

        //Beneficiary start here
        [HttpPost]
        public HttpResponseMessage CreateLCBeneficiary(LCBeneficiaryModels lcbeneficiary)
        {
            if (ModelState.IsValid)
            {
                _lcsetupservice.AddUpdateLcBeneficiary(lcbeneficiary);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Error", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        public List<LCBeneficiaryModels> GetAllBeneficiary()
        {
            return _lcsetupservice.GetAllLCBeneficiary();
        }

        [HttpGet]
        public HttpResponseMessage DeleteLCBeneficiary(int BNF_CODE)
        {
            _lcsetupservice.DeleteLCBeneficiary(BNF_CODE);
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });

        }
        #endregion

        #region LCHS Setup
        // start hs here
        [HttpPost]
        public HttpResponseMessage CreateHs(LCHSModals lchs)
        {
            bool result;
            if (ModelState.IsValid)
            {
                try
                {
                    result = _lcsetupservice.AddUpdateHs(lchs);
                    if (result)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success" });

                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Exist"});

                    }

                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }

            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Error"});
            }
        }

        public List<LCHSModals> Getallhs()
        {
            return _lcsetupservice.getallhs();
        }

        [HttpGet]
        public HttpResponseMessage DeleteHs(string HS_CODE)
        {
            _lcsetupservice.deleteHs(HS_CODE);
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
        }
        #endregion

        #region Lc Location Setup
        [HttpGet]
        public IHttpActionResult GetAllLocations()
        {
            try
            {
                var result = _lcsetupservice.getAllLocations();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }

        }

        [HttpPost]
        public IHttpActionResult AddLocation(LocationModels locationdetails)
        {
            bool result;
            try
            {
            result= _lcsetupservice.AddLocation(locationdetails);
                if (result)
                {
                    return Ok(new {MESSAGE="Success"});
                }
                else
                {
                    return Ok(new { MESSAGE = "Exist" });
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage DeleteLocation(string locationCode)
        {
            _lcsetupservice.deleteLocations(locationCode);
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
        }
        #endregion

        #region Document Setup 

        public IHttpActionResult GetAllDocuments()
        {
            try
            {
                var result = _lcsetupservice.getAllDocumentInfo();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }

        }

        [HttpPost]
        public IHttpActionResult AddDocumentInfo(DocumentModels documentdetails)
        {
            try
            {
                _lcsetupservice.AddDocumentInfo(documentdetails);
                return Ok();
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage DeleteDocumentInfo(string documentCode)
        {
            _lcsetupservice.deleteDocumentInfo(documentCode);
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
        }

        #endregion

        #region comments
        //public LCPTermModels GetPTermByPTermCode(int PTermCode)
        //{
        //    var data = _lcsetupservice.getPtermByPTermCode(PTermCode);
        //    return data;
        //}
        //public LCStatusModels GetStatusByStatusCode(int StatusCode)
        //{
        //    var data = _lcsetupservice.getStatusByStatusCode(StatusCode);
        //    return data;
        //}
        //public LCTermModels GetTermByTermCode(int TermCode)
        //{
        //    var data = _lcsetupservice.getTermByTermCode(TermCode);
        //    return data;
        //}

        //public LCBankModels GetBankByBankCode(int BankCode)
        //{
        //    var data = _lcsetupservice.getBankByBankCode(BankCode);
        //    return data;
        //}
        #endregion

        #region ISSUING CARRIER
        public HttpResponseMessage CreateIc(IssuingCarrierModels lcic)
        {
            if (ModelState.IsValid)
            {
                string message = _lcsetupservice.AddUpdateIc(lcic);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = message, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Error", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        public List<IssuingCarrierModels> Getallic()
        {
            return _lcsetupservice.getallic();
        }
        [HttpGet]
        public HttpResponseMessage DeleteIc(string CARRIER_CODE)
        {
            _lcsetupservice.deleteIc(CARRIER_CODE);
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
        }

        #endregion

        #region container
        public HttpResponseMessage Createcontainer(ContainerModels lcc)
        {
            if (ModelState.IsValid)
            {
                string message = _lcsetupservice.AddUpdateContainer(lcc);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = message, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Error", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        public List<ContainerModels> Getallcontainer()
        {
            return _lcsetupservice.getallc();
        }
        [HttpGet]
        public HttpResponseMessage Deletecontainer(string CONTAINER_CODE)
        {
            _lcsetupservice.deletec(CONTAINER_CODE);
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
        }
        #endregion

        #region Applicant Bank

        public IHttpActionResult getApplicantBank()
        {
            try
            {
                var list = _lcsetupservice.GetLC_ApplicantBank();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }

        }

        [HttpPost]
        public IHttpActionResult CreateLCApplicantBank(ApplicantBankModel applicantBankModel)
        {
            try
            {
                _lcsetupservice.AddUpdateLcApplicantBank(applicantBankModel);
                return Ok("Saved");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }
        public IHttpActionResult GetallApplicantBankList()
        {
            try
            {
                var list = _lcsetupservice.GetallApplicantBankList();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }


        [HttpGet]
        public HttpResponseMessage DeleteApplicantBank(string BANK_CODE)
        {
            _lcsetupservice.DeleteApplicantBank(BANK_CODE);
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
        }

        [HttpGet]
        public HttpResponseMessage BankAccountExists(string BANK_NAME, string BANK_ACC_NO)
        {   bool result = false;
            result= _lcsetupservice.ApplicantBankAccountNumberExists(BANK_NAME, BANK_ACC_NO);
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
        }



        #endregion

        #region LC Contractor

        //public IHttpActionResult getLC_Contractor()
        //{
        //    try
        //    {
        //        var list = _lcsetupservice.GetLC_ApplicantBank();
        //        return Ok(list);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content(HttpStatusCode.NotFound, ex.Message);
        //    }

        //}

        [HttpPost]
        public IHttpActionResult CreateLc_Contractor(LC_ContractorModel lcContractorModel)
        {
            try
            {
                _lcsetupservice.AddUpdateLC_Contractor(lcContractorModel);
                return Ok("Saved");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }
        public IHttpActionResult Getall_Lc_ContractorList()
        {
            try
            {
                var list = _lcsetupservice.getAll_LC_ContractorList();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

      
        [HttpGet]
        public HttpResponseMessage DeleteLc_Contractor(string CONTRACTOR_CODE)
        {
            _lcsetupservice.DeleteLC_Contractor(CONTRACTOR_CODE);
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
        }

        //[HttpGet]
        //public HttpResponseMessage BankAccountExists(string BANK_NAME, string BANK_ACC_NO)
        //{
        //    bool result = false;
        //    result = _lcsetupservice.ApplicantBankAccountNumberExists(BANK_NAME, BANK_ACC_NO);
        //    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
        //}



        #endregion

        #region LC Clearing Agent



        [HttpPost]
        public IHttpActionResult CreateLcClearingAgent(LC_ClearingAgentModel lcClearingAgentModel)
        {
            try
            {
                _lcsetupservice.AddUpdateLcClearingAgent(lcClearingAgentModel);
                return Ok("Saved");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }
        public IHttpActionResult GetAllLcClearingAgentList()
        {
            try
            {
                var list = _lcsetupservice.GetAllLcClearingAgentList();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }


        [HttpGet]
        public HttpResponseMessage DeleteLcClearingAgent(string CAGENT_CODE)
        {
            _lcsetupservice.DeleteLcClearingAgent(CAGENT_CODE);
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
        }
        #endregion

        #region Preference
        
        [HttpPost]
        public IHttpActionResult SavePreferenceSetup(string Beneficiary, string GetPOfromERP)
        {
            try
            {
                var response =_lcsetupservice.SavePreferenceSetup(Beneficiary, GetPOfromERP);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        
        [HttpGet]
         public IHttpActionResult GetPreferenceSetup()
         {
            try
            {
                var response = _lcsetupservice.GetPreferenceSetup();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }

         }
        #endregion
     
    
     

    }
}
