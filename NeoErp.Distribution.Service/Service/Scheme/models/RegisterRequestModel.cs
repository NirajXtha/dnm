using System;

namespace NeoErp.Distribution.Service.Service.Scheme.models
{
    public class RegisterRequestModel
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string MobileNo { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Profession { get; set; }
        public string EmailId { get; set; }
        public string Password { get; set; }
    }
}
