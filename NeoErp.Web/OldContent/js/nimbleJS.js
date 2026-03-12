
var viewPage = function (path) {
    //   alert(path);
    if (path != null) {
        var win = window.open(path, '_blank');
        win.focus();
        //window.location.href = path;
    }
    else
        return false;
}

//Auto Hide Error/Success Message
$(function () {
    // setTimeout() function will be fired after page is loaded
    // it will wait for 5 sec. and then will fire
    // $("#successMessage").hide() function
    setTimeout(function () {
        $(".alert-message").hide('blind', {}, 500)
    }, 15000);
});

$(document).ready(function () {
    $(".highlight").each(function () {
        var originalWidth = $(this).width();
        var idealWidth = Math.max(originalWidth, $(this).find("pre").contents().width());
        $(this).hover(function () {
            $(this).animate({ width: idealWidth });
        }, function () {
            $(this).animate({ width: originalWidth });
        });
    });
});


function printdiv(printpage) {
    var headstr = "<html><head><title></title></head><body>";
    var footstr = "</body>";
    var newstr = document.all.item(printpage).innerHTML;
    var oldstr = document.body.innerHTML;
    document.body.innerHTML = headstr + newstr + footstr;
    window.print();
    document.body.innerHTML = oldstr;
    return false;
};

var tableToExcel = (function () {
    var uri = 'data:application/vnd.ms-excel;base64,'
        , template = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel"  xmlns="http://www.w3.org/TR/REC-html40"><head> <!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets> <x:ExcelWorksheet><x:Name>{worksheet}</x:Name> <x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions>  </x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook> </xml><![endif]--></head><body> <table>{table}</table></body></html>'
        , base64 = function (s) { return window.btoa(unescape(encodeURIComponent(s))) }
        , format = function (s, c) { return s.replace(/{(\w+)}/g, function (m, p) { return c[p]; }) }
    return function (table, name, filename) {
        if (!table.nodeType) table = document.getElementById(table)
        var ctx = { worksheet: name || 'Worksheet', table: table.innerHTML }
        if (filename == null)
            filename = "Report";

        document.getElementById("dlink").href = uri + base64(format(template, ctx));
        document.getElementById("dlink").download = filename + ".xls";
        document.getElementById("dlink").click();

        //window.location.href = uri + base64(format(template, ctx))
    }
})();

//Initialize  Date picker
$(document).ready(function () {
    //var datepicker = $.fn.datepicker.noConflict(); // return $.fn.datepicker to previously assigned value
    //$.fn.bootstrapDP = datepicker;                 // give $().bootstrapDP the bootstrap-datepicker functionality

    $('.datepicker').datepicker(
        {
            format: 'dd/mm/yyyy',
            //  autoclose: true,
            calendarWeeks: true,
            todayBtn: true
            // todayHighlight: true
            // changeYear: true, 
            //yearRange: '1970:2020', 
            //changeMonth: true
        }).inputmask("dd/mm/yyyy", { "placeholder": "dd/mm/yyyy" });;
});



function GetSelectedEmpFromDataTable() {
    //Collecting checked nodes value and set in a variable with comma seperated
    var SelectedEmps = "";
    $("input:checked").each(function () {
        if ($(this).val() != 'on') {
            if (SelectedEmps != "") SelectedEmps = SelectedEmps + ",";
            SelectedEmps = SelectedEmps + $(this).val();
        }
    });

    //set checked collected value in a hidden control having id='Sel'
    $("#Sel").val(SelectedEmps);

    if ($("#UDSel") != undefined)
        $("#UDSel").val(SelectedEmps);

    return SelectedEmps;
}

/* checkbox select all */
$(document).ready(function () {
    $("#selectall").click(function () {
        $(".checkbox").prop("checked", $("#selectall").prop("checked"))
    })
});

var CheckToggle = function (classname, checkboxID) {
    $("." + classname).prop("checked", $("#" + checkboxID).prop("checked"))
}

var Confirm = function (msg) {
    if (msg == '' || msg == undefined) msg == "Sure to Delete ?";
    var status = confirm(msg);
    return status;

}

function registerCloseEvent() {

    $(".closeTab").click(function () {

        //there are multiple elements which has .closeTab icon so close the tab whose close icon is clicked
        var tabContentId = $(this).parent().attr("href");
        $(this).parent().parent().remove(); //remove li of tab
        $('#tabs a:last').tab('show'); // Select first tab
        $(tabContentId).remove(); //remove respective tab content

    });
}

var IndexMenuAction = function (id,action) {
    var url = id.href;
    if (action === 'create')
    {
        $.get(url, function (data) {
            var nextTab = $('#tabs li').size() + 1;
            if ($('#tabs li a[href="#tab_create"]').length > 0)
                return false;

            $('<li><a href="#tab_create" data-toggle="tab"><i class="fa fa-plus"></i>Add New<button class="close closeTab" type="button" style="padding-left: 13px;" >×</button></a></li>').appendTo('#tabs');

            // create the tab content
            $('<div class="tab-pane" id="tab_create">' + data + '</div>').appendTo('.tab-content');

            // make the new tab active
            $('#tabs a:last').tab('show');
            registerCloseEvent();
        });
        return false;
    }
    var checkedValues = $('.material-switch input:checkbox:checked').map(function () {
        return this.id;
    }).get();
    if (checkedValues.length <= 0)
        displayPopupNotification('Please Select Grid value', 'info')
    checkedValues.forEach(function (entry) {
        var id = entry.split("_")[1];
        var BaseUrl = "";
        if (url.indexOf("?") == -1) {
            BaseUrl = url + "?id=" + id;
            url = url.split("?")[0];
        }

        else
            BaseUrl = url + "&id=" + id;
        $.get(BaseUrl, function (data) {
            url = url.split("?")[0];
            var nextTab = $('#tabs li').size() + 1;
            if ($('#tabs li a[href="#tab' + id + '"]').length > 0)
                return false;
          
            $('<li><a href="#tab' + id + '" data-toggle="tab">' + $(data).find('#ProjectName').val() + '<button class="close closeTab" type="button" style="padding-left: 13px;" >×</button></a></li>').appendTo('#tabs');

            // create the tab content
            $('<div class="tab-pane" id="tab' + id + '">' + data + '</div>').appendTo('.tab-content');

            // make the new tab active
            $('#tabs a:last').tab('show');
            registerCloseEvent();
        });

       
    });
    checkedValues = [];
    return false;
   
};
//Created for: Perform Update Delete operation for highlighted row in grid mvc table.  It can be used for any operation for selected/highlighted row for grid mvc table.
var UDAction = function (id) {

    var url = id.href;
    var checkedValues = $('.material-switch input:checkbox:checked').map(function () {
        return this.id;
    }).get();
    console.log(checkedValues);
    checkedValues.forEach(function (entry) {
        var id = entry.split("_")[1];
        var BaseUrl = "";
        if (url.indexOf("?") == -1) {
            BaseUrl = url + "?id=" + id;
            url = url.split("?")[0];
        }

        else
            BaseUrl = url + "&id=" + id;
        $.get(BaseUrl, function (data) {
            url = url.split("?")[0];
            var nextTab = $('#tabs li').size() + 1;
            if ($('#tabs li a[href="#tab' + id + '"]').length > 0)
                return false;
            // create the tab
            $('<li><a href="#tab' + id + '" data-toggle="tab">' + $(data).find('#ProjectName').val() + '<button class="close closeTab" type="button" style="padding-left: 13px;" >×</button></a></li>').appendTo('#tabs');

            // create the tab content
            $('<div class="tab-pane" id="tab' + id + '">' + data + '</div>').appendTo('.tab-content');

            // make the new tab active
            $('#tabs a:last').tab('show');
            registerCloseEvent();
        });

        console.log(entry);
    });
    checkedValues = [];
    return false;
    if ($("#id").val() != undefined && $("#id").val() != "") {

        if (url.indexOf("?") == -1)
            url = url + "?id=" + $("#id").val();
        else
            url = url + "&id=" + $("#id").val();
    }
    else {
        alert("Select (Highlight) record to modify.");
        return false;
    }
    // window.location = url;
    return false;
}

function SubmitForm(formName, actionName, target) {

    GetSelectedEmpFromDataTable();
    var form = $("#" + formName);

    if (actionName != null && actionName != '') {
        $(form).attr("action", actionName);
    }
    if (target != null && target != '') {
        $(form).attr("target", "_blank");
    }

    //alert($(form).attr("name"));
    form.submit();
    return true;
}

function SetFocus(controlID) {
    return document.getElementById(controlID).focus();
}

//Initialize Nepali Date picker
$(document).ready(function () {
    $('.nepali-calendar').nepaliDatePicker();
});

function AddThead() {
    var $table = $('table.TableStyle,table.grid-table');
    $table.floatThead({
        scrollContainer: function ($table) {
            return $table.closest('.scroll,.scrollRep');
        },
        useAbsolutePositioning: true
    });
    $table.floatThead('reflow');
}

function RemoveThead() {
    var $table = $('table.TableStyle,table.grid-table');
    $table.floatThead('destroy');
}

function ToogleHead() {
    if ($("#AddHead").is(':checked'))
        AddThead();
    else
        RemoveThead();
}


//Add items inside class "toolbar_item" into .btn-toolbar. utilized in index page to add buttons into common toolbar. In index page keep buttons inside a span with class "toolbar_item", it will add these buttons in toolbar.
$(document).ready(function () {
    $(".toolbar_item").appendTo(".btn-toolbar");
    $(".toolbar_item_out").appendTo(".btn-toolbar-out");

    $(".toolbar_item").removeClass("toolbar_item");
    $(".toolbar_item_out").removeClass("toolbar_item_out");

    if ($(".btn-toolbar").length) {
        var val = $(".btn-toolbar").html().trim();
        if (val == "") {
            $(".btn-toolbar").parent().attr("style", "display:none");
        }
    }
});

// bootstrap wysi editor
$(document).ready(function () {
    $(".editor").wysihtml5();
})

// ajax request
ajaxRequest = function (url, data) {
    return $.ajax({
        url: url,
        type: 'POST',
        data: data
    });
}

var cmsCoreScript = (function () {
    ajaxRequest = function (url, data) {
        return $.ajax({
            url: url,
            type: 'POST',
            data: data
        });
    }

    tooltip = $(".content").on("hover", ".tt", function () {
        console.log("hovered");
    })

    return {
        ajaxRequest: ajaxRequest,
        tooltip: tooltip
    }
}(cmsCoreScript || {}));


//Utilized the function to Save and Add new functionality in a form. Keep a "hidden text box with ID:IsNew" in form and Add submit button with onsubmit="return IsNew1();".
var IsNew1 = function () {
    $("#IsNew").val("1");
    return true;
}

var IsGet1 = function () {
    $("#IsGet").val("1");
    return true;
}
//================================================================================
// Generic Add More Functions Begin
//================================================================================

var addMoreRow = function (tableId, sourceUrl) {
    addTableRow($("#" + tableId), sourceUrl);
    return false;
};

var deleteMoreRow = function (eventObject) {
    $(eventObject).closest("tr").remove();
    return false;
};

function addTableRow(table, url) {
    var ul = $(table).find("tbody tr:first");
    $.ajax({
        url: url,
        success: function (html) {
            $(html).appendTo($(table).find("tbody")); //Pending check binding events. not working now.          
            var form = ul.closest('form');
            form.removeData('validator');
            form.removeData('unobtrusiveValidation');
            $.validator.unobtrusive.parse(form);
        }
    });
    return false;
}

//================================================================================
// Generic Add More Functions End
//================================================================================


//Fixes for Bootstarp datepicker error on chrome.
jQuery.validator.methods["date"] = function (value, element) { return true; }