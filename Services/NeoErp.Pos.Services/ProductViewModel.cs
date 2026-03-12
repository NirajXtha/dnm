using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NeoErp.Pos.Services
{
    public class ProductViewModel
    {
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public Decimal? Price { get; set; }
        public string ImageName { get; set; }
    }

    public class CustomerViewModel
    {
        public string CustomerCode { get; set; }
        [Required]
        public string CustomerName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Mobile { get; set; }
        [Required]
        public string Address { get; set; }
      
    }
}