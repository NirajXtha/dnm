using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model
{
   public class ResellerRegistration
    {
        public int? Id { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string Lat { get; set; }
        public string Longt { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string Approved_Flag { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Registration_Type { get; set; }
        public string Delete_Flag { get; set; }
        public string Mode { get; set; }
        public string Reseller_Contact { get; set; }
        public string Reseller_Code { get; set; }
        public List<string> Image_Name { get; set; }
        public string Latitude_Mst { get; set; }
        public string Longitude_Mst { get; set; }
        public string Image_Path { get; set; }
    }
}
