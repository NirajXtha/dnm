using NeoErp.Pos.Services;
using NeoErp.Pos.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;



namespace NeoErp.Pos.Controllers.Api
{
    public class PosHomeController : ApiController
    {
        private IPosService _posservice { get; set; }
        public PosHomeController(IPosService posservice)
        {
            this._posservice = posservice;
        }
        public string GetMessage()
        {
            return "done";
        }

        public List<ProductViewModel> GetProducts()
        {
            return _posservice.GetAllProducts();

        }
        public List<ProductViewModel> GetProductsByValue(string value)
        {
            return _posservice.GetProductsByValue(value);

        }

      
        public List<CustomerViewModel> GetCustomers(string filter)
        {
            return _posservice.GetAllCustomers(filter);
        }

        [HttpPost]
        public CustomerViewModel CreateCustomer(CustomerViewModel customerdetail)
        {
            if (ModelState.IsValid)
            {
              
                var customercode = _posservice.CreateCustomers(customerdetail);
               
                return customerdetail;
            }
            else
            {
                CustomerViewModel customer = new CustomerViewModel();
                return customer;
            }
        }
    }
}
