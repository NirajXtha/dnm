using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoErp.Core.Helpers
{
    public class EnumHelper
    {
        public static DataTable EnumToDataTable(Type typEnum)
        {
            string[] ddlTypnames = Enum.GetNames(typEnum);
            Array arrddlTypVals = Enum.GetValues(typEnum);

            DataTable dt = new DataTable();
            DataRow dr = default(DataRow);

            dt.Columns.Add(new DataColumn("Value", typeof(Int32)));
            dt.Columns.Add(new DataColumn("Display", typeof(string)));
            int i2 = 0;
            for (i2 = 0; i2 <= ddlTypnames.Length - 1; i2++)
            {
                dr = dt.NewRow();

                dr[0] = Convert.ToInt32(arrddlTypVals.GetValue(i2));
                dr[1] = ddlTypnames[i2];

                dt.Rows.Add(dr);
            }

            return dt;
        }
    }

    public static class EnumExtensions
    {
        public static IEnumerable<SelectListItem> ToSelectList(this Enum enumValue)
        {
            return from Enum e in Enum.GetValues(enumValue.GetType())
                   select new SelectListItem
                   {
                       Selected = e.Equals(enumValue),
                       Text = e.ToDescription(),
                       Value = (Convert.ToInt32(e)).ToString()
                   };
        }

        public static string ToDescription(this Enum value)
        {
            var attributes = (DescriptionAttribute[])value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public static IEnumerable<SelectListItem> ToSelectListNameSameAsValue(this Enum enumValue)
        {
            return from Enum e in Enum.GetValues(enumValue.GetType())
                   select new SelectListItem
                   {
                       Selected = e.Equals(enumValue),
                       Text = (Convert.ToInt32(e)).ToString(),
                       Value = (Convert.ToInt32(e)).ToString()
                   };
        }

    }
}