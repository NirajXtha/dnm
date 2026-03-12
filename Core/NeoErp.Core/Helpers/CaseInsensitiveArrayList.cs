using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Core.Helpers
{
    public class CaseInsensitiveArrayList : ArrayList
    {

        bool IgnoreCase = true;
        public CaseInsensitiveArrayList(bool IsIgnoreCase = true) : base()
        {
            IgnoreCase = IsIgnoreCase;
        }

        private string SearchEntry(object value)
        {
            string SearchValue = null;
            if (IgnoreCase == true)
            {
                SearchValue = Convert.ToString(value).ToLower();
            }
            else
            {
                SearchValue = Convert.ToString(value);
                // Convert.ToString(value).ToLower
            }


            foreach (string Entry in this)
            {
                if (Entry.ToLower() == SearchValue)
                {
                    return Entry;
                }
            }

            return null;
        }

        public override int IndexOf(object value)
        {
            string Entry = SearchEntry(value);

            if (Entry == null)
            {
                return -1;
            }
            else
            {
                return base.IndexOf(Entry);
            }
        }

        public override int IndexOf(object value, int startIndex)
        {
            string Entry = SearchEntry(value);

            if (Entry == null)
            {
                return -1;
            }
            else
            {
                return base.IndexOf(Entry, startIndex);
            }
        }

        public override int IndexOf(object value, int startIndex, int count)
        {
            string Entry = SearchEntry(value);

            if (Entry == null)
            {
                return -1;
            }
            else
            {
                return base.IndexOf(Entry, startIndex, count);
            }
        }

        public override int LastIndexOf(object value)
        {
            string Entry = SearchEntry(value);

            if (Entry == null)
            {
                return -1;
            }
            else
            {
                return base.LastIndexOf(Entry);
            }
        }

        public override int LastIndexOf(object value, int startIndex)
        {
            string Entry = SearchEntry(value);

            if (Entry == null)
            {
                return -1;
            }
            else
            {
                return base.LastIndexOf(Entry, startIndex);
            }
        }

        public override int LastIndexOf(object value, int startIndex, int count)
        {
            string Entry = SearchEntry(value);

            if (Entry == null)
            {
                return -1;
            }
            else
            {
                return base.LastIndexOf(Entry, startIndex, count);
            }
        }

        public override void Remove(object value)
        {
            string Entry = SearchEntry(value);

            if (Entry == null)
                return;
            base.Remove(Entry);
        }

        public object ArrayListToString(string Seperator, string EnclosedBy = "'")
        {
            string Str = "";
            foreach (object Item in this)
            {
                if (Str.Length > 0)
                    Str += ",";
                Str += EnclosedBy + Item + EnclosedBy;
            }
            return Str;
        }

        public ArrayList RemoveDuplicate()
        {
            ArrayList list = new ArrayList();
            foreach (string item in this)
            {
                if (!list.Contains(item))
                {
                    list.Add(item);
                }
            }
            return list;
        }

    }
}