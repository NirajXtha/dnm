using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Models
{
    public class TestTemplate
    {
        public string name { get; set; }
    }

    public class TesteeTemplate
    {
        public string FIRST { get; set; }
    }

    public class CustomerDataBinding
    {
        List<string> customerId { get; set; }
        List<string> masterCustomerCode { get; set; }
    }
}
