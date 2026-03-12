using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NeoErp.Models.QueryBuilder
{
    public class NotificationBuilderModel
    {
        public string ResultValue { get; set; }
        public int NotificationId { get; set; }
        [Display(Name = "Notification Name")]
        public string NotificationName { get; set; }
        [Display(Name = "Notification Result")]
        public string NotificationResult { get; set; }
        [Display(Name = "Module")]
        public string ModuleCode { get; set; }
        [Display(Name = "Users")]
        public string Users { get; set; }
        public string SqlQuery { get; set; }
        [Display(Name = "Min Result")]
        public double MinResult { get; set; }
        [Display(Name = "Max Result")]
        public double MaxResult { get; set; }
        [Display(Name = "Type")]
        public string NotificationType { get; set; }
        [Display(Name = "Template")]
        public string NotificationTemplate { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        [Display(Name = "Append Text")]
        public string AppendText { get; set; }
        [Display(Name = "Position")]
        public string AppendPosition { get; set; }
    }

    public class ReordingReport
    {
        public string widgetsId { get; set; }
        public string ReportName { get; set; }
        public string XAxis { get; set; }
        public string YAxis { get; set; }
        public string Height { get; set; }
        public string Width { get; set; }

    }
 
}