using NeoErp.Core.Caching;
using NeoErp.Core.Helpers;
using NeoErp.Transaction.Service.Models;
using NeoErp.Transaction.Service.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoErp.Transaction.Controllers
{
    public class TransactionController : ApiController
    {
        
        private ITransactionServiceRepository _TransactionServiceRepo;
        public TransactionController(ITransactionServiceRepository TransactionServiceRepo)
        {            
            this._TransactionServiceRepo = TransactionServiceRepo;
        }
        [HttpGet] 
        public Consumption GetAllTransactionSetup()
        {
            var item = this._TransactionServiceRepo.GetConsumptionSetup();
            return item;
        }        

        [HttpGet]
        public List<Employee> GetAllEmployeeSetup(string filter)
        {
            if (string.IsNullOrEmpty(filter))
                return new  List<Employee>();
            return this._TransactionServiceRepo.GetAllEmployeeSetup(filter);
        }

        
        [HttpGet]
        public List<Customers> GetAllCustomerSetup(string filter)
        {
            return this._TransactionServiceRepo.GetAllCustomerSetup(filter);
        }

        [HttpGet]
        public List<Department> GetAllDepartmentSetup(string filter)
        {
            return this._TransactionServiceRepo.GetAllDepartmentSetup(filter);
        }

        [HttpGet]
        public List<Location> GetAllLocationSetup(string filter)
        {
            return this._TransactionServiceRepo.GetAllLocationSetup(filter);
        }

        [HttpGet]
        public List<Products> GetAllProducts()
        {
            return this._TransactionServiceRepo.GetAllProducts(string.Empty);
        }
        [HttpGet]
        public List<Products> GetAllProducts(string filter)
        {
            return this._TransactionServiceRepo.GetAllProducts(filter);
        }

        [HttpGet]
        public List<CostCenter> GetAllCostCenter(string filter)
        {
            return this._TransactionServiceRepo.GetAllCostCenter(filter);
        }

        [HttpPost]
        public string PostConsumptionIssue(ConsumptionIssue issue)
        {
            try
            {
                string actionMessage = "success";
                this._TransactionServiceRepo.SaveConsumptionIssue(issue);
                return actionMessage;
            }
            catch (Exception ex)
            {
                throw;
            }            
        }
    }
}
