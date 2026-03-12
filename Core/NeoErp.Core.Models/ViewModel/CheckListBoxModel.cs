using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CheckBoxList.ViewModel
{
    // view model for checklistbox
    public class CheckBoxListViewModel
    {
        public string Name { get; set; }
        public List<CheckListBoxItem> Items { get; set; }

        // the name of this list should be the same as of the CheckBoxes otherwise you will not get any result after post
        public List<string> SelectedValues { get; set; }
    }

    // represents single check box item
    public class CheckListBoxItem
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }
}