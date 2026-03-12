using NeoERP.DocumentTemplate.Service.Interface;
using NeoERP.DocumentTemplate.Service.Models;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace NeoERP.DocumentTemplate.Controllers.Api
{
    public class SubLedgerMgmtApiController : ApiController
    {
        private ISubLegerMappingSetup _subLegerMappingSetup;

        public SubLedgerMgmtApiController(ISubLegerMappingSetup subLegerMappingSetup)
        {
            _subLegerMappingSetup = subLegerMappingSetup;
        }

        [HttpGet]
        public List<SubLedgerMapDetail> GetSubLedgerMappingForGrid(string subCode = "0", string masterCode = "0", string preCode = "0")
        {
            try
            {
                var mappedDataForGrid = _subLegerMappingSetup.GetSubLedgerMappingForGrid(subCode, masterCode);
                return mappedDataForGrid;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }


        [HttpGet]
        public List<SubLedgerMappingModal> GetCharOfAccountTree()
        {
            try
            {
                var accountTreeData = _subLegerMappingSetup.GetCharOfAccountTree();
                return accountTreeData;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public List<CustomerSubLedgerModal> GetCustomerSubLedger()
        {
            try
            {
                var customerLeder = _subLegerMappingSetup.GetCustomerSubLedger();
                return customerLeder;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public List<EmployeeSubLedgerModal> GetEmployeeSubLedger()
        {
            try
            {
                var employeeLeder = _subLegerMappingSetup.GetEmployeeSubLedger();
                return employeeLeder;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public string GetMiscSubLedger()
        {
            return "Not Implemented";
        }

        [HttpGet]
        public List<ChartOfItemModel> GetItemSubLedger()
        {
            try
            {
                var itemLedger = _subLegerMappingSetup.GetChatOfItemSubLedger();
                return itemLedger;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public List<SupplierSubLederModal> GetSupplierSubLedger()
        {
            try
            {
                var supplierLedger = _subLegerMappingSetup.GetSupplierSubLedger();
                return supplierLedger;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public string SaveSubLedgerMapping(ModalToSaveSubLedgerMapping modal)
        {
            try
            {
                var saveRes = _subLegerMappingSetup.SaveSubLedgerMapping(modal);
                return saveRes;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public List<CostCenterSubLedgerModal> GetCostCenterSubLedger()
        {
            try
            {
                var employeeLeder = _subLegerMappingSetup.GetCostCenterSubLedger();
                return employeeLeder;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        public string SaveCostCenterMapping(ModalToSaveSubLedgerMapping modal)
        {
            try
            {
                var saveRes = _subLegerMappingSetup.SaveCostCenterMapping(modal);
                return saveRes;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public List<CostCenterSubLedgerModal> GetCostCenterMappingForGrid(string subCode = "0", string masterCode = "0", string preCode = "0")
        {
            try
            {
                var mappedDataForGrid = _subLegerMappingSetup.GetCostCenterMappingForGrid(subCode, masterCode);
                return mappedDataForGrid;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }  
    }
}
