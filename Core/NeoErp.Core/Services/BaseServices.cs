using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NeoErp.Core.Integration;
using System.Collections;
using System.Web.Mvc;
using NeoErp.Core.Models;
using NeoErp.Data;



namespace NeoErp.Core.Services
{
    public class BaseServices
    {
        private NeoErpCoreEntity db = new NeoErpCoreEntity();
        //private NeoErpCoreEntity db = NeoErpCoreEntity.CreateForSelectedDatabase();

        #region Get List
       
        public static int GetNewID(string FieldName, string TableName, bool IsNextID =true)
        {
            DbConnBase _db = new DbConnBase();
            string sql = "select max(" + FieldName + ") as ID from " + TableName;
            var data = _db.ExecuteScalar(sql);
            int newid = 0;

            int.TryParse(Convert.ToString(data), out newid);

            //if (data != null)
            //{
            //    newid= Convert.ToInt32(data);

            //}
            if (IsNextID == true)
                newid += 1;

            return newid;
        }

        public static string GetSelectionQry(Dictionary<string, string> SelectionQry, string PreFix = "", string SkipField = "")
        {
            string Param = "( 1=1 ";
            if (SelectionQry != null)
            {
                foreach (KeyValuePair<string, string> item in SelectionQry)
                {
                    if (SkipField.ToUpper().Contains(item.Key.Trim().ToUpper()) == true)
                        continue;
                    if (string.IsNullOrEmpty(item.Value.ToString().Trim()))
                        continue;

                    Param += " and   " + PreFix + item.Key.Trim() + " in ( " + item.Value.ToString() + ")";
                }
            }
            Param += ")";

            return Param;
        }

        #endregion




    }
}