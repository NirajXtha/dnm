using System;

namespace NeoErp.Print.Service.Models
{
    public class FormDetailSetupModel
    {
        public int SERIAL_NO { get; set; }
        public string TABLE_NAME { get; set; }
        public string COLUMN_NAME { get; set; }
        public string COLUMN_HEADER { get; set; }
        public string MASTER_CHILD_FLAG { get; set; }
        public string FORM_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
    }
}
