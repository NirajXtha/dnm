using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Core.Helpers
{
    public enum NimNotificationTypes
    {
        Success = 1,
        Error =2,
        Warning=3
    }

    public partial class Display
    {
        #region Grid Display
        private static int itemPerPage = 35;

        public static int ItemPerPage
        {
            get
            {
                return itemPerPage;
            }
            set { itemPerPage = value; }
        }

        public static string GridItemsName
        {
            get
            {
                return "Displaying Data";
            }
            set
            {

            }
        }

        public static string CrudTitle
        {
            get
            {
                return "Options";
            }
            set
            {

            }
        }

        public static string SelectText
        {
            get
            {
                return "--Select Item--";
            }
            set
            {

            }
        }

        public static string SelectTextAll
        {
            get
            {
                return "All";
            }
            set
            {

            }
        }


        #endregion

        //#region Page Display
        //public static string FaLayoutPage
        //{
        //    get
        //    {
        //        return "~/Areas/FixedAsset/Views/Shared/_ProfileLayout.cshtml";
        //    }
        //    set
        //    {

        //    }
        //}


        //public static string FormLayoutPage
        //{
        //    get
        //    {
        //        return "~/Areas/Grievance/Views/Shared/FormTemplate.cshtml";
        //    }
        //    set
        //    {

        //    }
        //}


        //#endregion

    }

    public class SelectListModel<T, T1>
    {
        public T Value { get; set; }
        public T1 Text { get; set; }
    }

    public class KeyValueModule<T>
    {
        public T id { get; set; }
        public string Value { get; set; }
    }

}