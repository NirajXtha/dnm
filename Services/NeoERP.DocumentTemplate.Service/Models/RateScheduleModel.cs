using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NeoERP.DocumentTemplate.Service.Models
{
    public class IndividualsByMasterCodesRequest
    {
        public string PartyType { get; set; }
        public List<string> MasterCodes { get; set; }
    }
}
