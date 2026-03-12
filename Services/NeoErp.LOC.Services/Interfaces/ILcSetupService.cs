using NeoErp.LOC.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.LOC.Services.Services
{
    public interface ILcSetupService
    {
        void AddUpdateLcBank(LCBankModels lcbank);
        List<LCBankModels> getAllLCBanks();
        void deleteLCBanks(int BankCode);
        List<LCTermModels> getAllLCTerms();
        void AddUpdateLcTerm(LCTermModels lcterm);
        void deleteLCTerm(int TermCode);
        List<LCPTermModels> getAllLCPTerms();
        void AddUpdatePLcTerm(LCPTermModels lcpterm);
        void deleteLCPTerm(int PTermCode);
        List<LCStatusModels> getAllLCStatus();
        void AddUpdateLcStatus(LCStatusModels lcstatus);
        void deleteLCStatus(int StatusCode);
        List<LCBeneficiaryModels> GetAllLCBeneficiary();
        void AddUpdateLcBeneficiary(LCBeneficiaryModels lcbeneficiary);
        void DeleteLCBeneficiary(int BNF_CODE);
        bool AddUpdateHs(LCHSModals lchs);
        List<LCHSModals> getallhs();
        void deleteHs(string HS_CODE);
        bool AddLocation(LocationModels locationdetails);
        List<LocationModels> getAllLocations();
        void deleteLocations(string LocationCode);
        void AddDocumentInfo(DocumentModels documentdetails);
        List<DocumentModels> getAllDocumentInfo();
        void deleteDocumentInfo(string documentCode);

        string AddUpdateIc(IssuingCarrierModels lcic);
        List<IssuingCarrierModels> getallic();
        void deleteIc(string CARRIER_CODE);

        string AddUpdateContainer(ContainerModels lcc);
        List<ContainerModels> getallc();
        void deletec(string CONTAINER_CODE);

        List<LCApplicantBank> GetLC_ApplicantBank();

        List<ApplicantBankModel> GetallApplicantBankList();
        void AddUpdateLcApplicantBank(ApplicantBankModel applicantBankModel);
        void DeleteApplicantBank(string BANK_CODE);
        bool ApplicantBankAccountNumberExists(string BANK_NAME, string BANK_ACC_NO);

        #region LC Contractor Repo
        List<LC_ContractorModel> getAll_LC_ContractorList();
        void AddUpdateLC_Contractor(LC_ContractorModel lcContractor);
        void DeleteLC_Contractor(string CONTRACTOR_CODE);
        //bool IsLC_ContractorAlreadyExist();
        #endregion

        #region LC Clearing Agent
        List<LC_ClearingAgentModel> GetAllLcClearingAgentList();
        void AddUpdateLcClearingAgent(LC_ClearingAgentModel lcClearingAgentModel);
        void DeleteLcClearingAgent(string CAGENT_CODE);
        //bool IsLC_ContractorAlreadyExist();
        #endregion

        #region lc Preference setup
        LC_PREFERENCE_SETUP SavePreferenceSetup(string Beneficiary, string GetPOfromERP);
        LC_PREFERENCE_SETUP GetPreferenceSetup();
        
        #endregion
    }
}
