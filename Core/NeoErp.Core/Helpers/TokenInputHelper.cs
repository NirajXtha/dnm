using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Core.Helpers
{
    public class TokenInputHelper
    {
        public static string PreDataForTokenInput(string key, string value)
        {
            if (key == null || key == "" || value == null || value == "") return string.Empty;

            string data = string.Empty;
            string[] keyList = key.Split(',');
            string[] valueList = value.Split(',');

            if (keyList.Length != valueList.Length) return string.Empty;

            data = "[";
            for (int i = 0; i < keyList.Length; i++)
            {
                data += data != "[" ? "," : "";
                data += "{\"id\":\"" + keyList[i] + "\",\"Value\":\"" + valueList[i] + "\"}";
            }
            data += "]";
            return data;
        }


        public static string PreDataForTokenInput<T>(List<KeyValueModule<T>> values)
        {
            if (values.Count<=0) return string.Empty;

            string data = string.Empty;
           
            data = "[";
            for (int i = 0; i < values.Count; i++)
            {
                data += data != "[" ? "," : "";
                data += "{\"id\":\"" + values[i].id + "\",\"Value\":\"" + values[i].Value + "\"}";
            }
            data += "]";
            return data;
        }




    }
}