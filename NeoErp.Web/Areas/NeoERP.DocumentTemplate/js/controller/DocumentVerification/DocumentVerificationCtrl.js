DTModule.controller('DocumentVerificationCtrl', function ($scope, $http, $routeParams, $window, $filter) { 
    debugger;
    $scope.subMenuList = [];
    $scope.orientation = "horizontal";
    $scope.formcode = "";
    $scope.formControlsInfo = [];
    var dataFillDefered = $.Deferred();
    $scope.moduleCode = $routeParams.module;
    $scope.docVer = "";
    $scope.query = {}['$'];
    var words = $scope.moduleCode.split('_');
  
        var MOduleCode = words[0];
        $scope.moduleCode = MOduleCode;
        var docuementType = words[1];
   
        if ($scope.moduleCode == "01") {
            $scope.modulename = "Financial Accounting";
            $scope.docVer = docuementType;
        }
        else if ($scope.moduleCode == "02") {
            $scope.modulename = "Inventory & Procurement";
            $scope.docVer = docuementType;
        }
        else if ($scope.moduleCode == "04" || $scope.moduleCode == "05" || $scope.moduleCode == "06") {
            $scope.modulename = "Sales & Revenue";
            if ($scope.moduleCode == "04") {
                //  $scope.docVer = "Check";
                $scope.docVer = docuementType;
            }
            if ($scope.moduleCode == "05") {
                $scope.docVer = "Authorise";
                $scope.moduleCode = "04";
            }
            if ($scope.moduleCode == "06") {
                $scope.docVer = "Post";
                $scope.moduleCode = "04";
            }
        }
        else {
            $scope.docVer = docuementType;
        }
    var formcode = "";
    $scope.BackFromMenu = function () {
        $window.location.href = '/DocumentTemplate/Home/Dashboard';
    };
    $scope.BindDetailGrid = function (formcode, tableName, formname, docVer) {
        debugger;
        $scope.formcode = formcode;
        $scope.formname = formname;
        $("#kGridVer").html("");
        $("#txtSearchString").show();
        BindGrid(formcode, tableName, docVer);
        setTimeout(function () {
            $('[data-toggle="tooltip"]').tooltip();
        }, 10)
    };
    //var req = "/api/TemplateApi/GetSubMenuList?moduleCode=" + $scope.moduleCode;
    var req = "/api/TemplateApi/getSalesVerificationFormcodeWise?moduleCode=" + $scope.moduleCode + "&docVer=" + $scope.docVer;
    return $http.get(req).then(function (response) {
        $("#txtSearchString").hide();
        //old

        //if ($scope.docVer == "Post") {
        //    for (var i = 0; i < response.data.length; i++) {
        //        if (response.data[i].TABLE_NAME == "SA_SALES_ORDER") {
        //            response.data.splice(i, 1);
        //        }
        //    }
        //    $scope.subMenuList = response.data;
        //}
        //else {
        //    $scope.subMenuList = response.data;
        //}

        //new
        if (response.data.length == 0) {
            bootbox.alert("Data Empty.");
        }
        else {
            $scope.subMenuList = response.data;
        }
    });
    function BindGrid(formCode, tableName, docVer) {
        debugger;
        $scope.mainGridOptionsVer = {
            dataSource: {
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                type: "GET",
                transport: {
                    read: "/api/TemplateApi/GetSubMenuDetailListVer?formcode=" + formCode + "&docVer=" + docVer
                },
                pageSize: 50,
                serverPaging: false,
                serverSorting: true,
                schema: {
                    model: {
                        fields: {
                            VOUCHER_NO: { type: "string" },
                            VOUCHER_DATE: { type: "date" },
                            CREATED_DATE: { type: "date" },
                            CHECKED_DATE: { type: "date" },
                            POSTED_DATE: { type: "date" },
                            MODIFY_DATE: { type: "date" },
                            VOUCHER_AMOUNT: { type: "number" }
                        }
                    }
                },
            },
            scrollable: true,
            height: 427,
            sortable: true,
            resizable: true,
            pageable: true,
            filterable: {
                extra: false,
                operators: {
                    number: {
                        eq: "Is equal to",
                        neq: "Is not equal to",
                        gte: "is greater than or equal to	",
                        gt: "is greater than",
                        lte: "is less than or equal",
                        lt: "is less than",
                    },
                    string: {
                        eq: "Is equal to",
                        neq: "Is not equal to",
                        startswith: "Starts with",
                        contains: "Contains",
                        doesnotcontain: "Does not contain",
                        endswith: "Ends with",
                    },
                    date: {
                        eq: "Is equal to",
                        neq: "Is not equal to",
                        gte: "Is after or equal to",
                        gt: "Is after",
                        lte: "Is before or equal to",
                        lt: "Is before",
                    }
                }
            },
            columnMenu: true,
            columnMenuInit: function (e) {
                wordwrapmenu(e);
            },
            dataBound: function (e) {                
                $("#kGridVer tbody tr").css("cursor", "pointer");
                $("#kGridVer tbody tr td:not(:last-child)").on('dblclick', function () {
                    debugger;
                    var grid = $("#kGridVer").data("kendoGrid");
                    var row = $(this).closest('tr');
                    var data = grid.dataItem(row);
                    var voucherNo = $(this).closest('tr').find('td span').html()
                    $scope.doSomething(voucherNo, data.MODULE_CODE);
                });
                var mode = docVer;
                $(".checkbox").bind("change", function (e) {
                    var grid = $("#kGridVer").data("kendoGrid");
                    var row = $(e.target).closest("tr");
                    var data = grid.dataItem(row);
                    var attr = $("#chkAll").attr('disabled');
                    if (row.is(":not(:checked)")) {
                        bootbox.confirm({
                            title: mode,
                            message: "Do you want to " + mode + " data?",
                            buttons: {
                                confirm: {
                                    label: 'Yes',
                                    className: 'btn-success',
                                    label: '<i class="fa fa-check"></i> Yes',
                                },
                                cancel: {
                                    label: 'No',
                                    className: 'btn-danger',
                                    label: '<i class="fa fa-times"></i> No',
                                }
                            },
                            callback: function (result) {
                                if (result == true) {
                                    $.ajax({
                                        type: "POST",
                                        url: window.location.protocol + "//" + window.location.host + '/api/TemplateApi/UpdateMasterTranasactionForVerification?VoucherNo=' + data.VOUCHER_NO + '&formcode=' + data.Form_CODE + '&mode=' + docVer,
                                        contentType: "application/json; charset-utf-8",
                                        datatype: JSON,
                                        success: function (response) {
                                            debugger;
                                            if (response.MESSAGE == "SUCCESS") {
                                                $(row.context).attr("disabled", true);
                                                alert("i am here ", mode);    
                                                bootbox.alert('Data ' + mode + ' successfull.');
                                                $("#kGridVer").data("kendoGrid").dataSource.read();
                                            }
                                           else if (response.MESSAGE == "INVALIDTRIGGER") {
                                                $(row.context).prop('checked', false);
                                                bootbox.alert('<span style="background-color: red;">Data ' + mode + ' does not success due to invalid trigger.</span>');
                                            }
                                            else {
                                                alert("i am here 123 ", mode);    
                                                $(row.context).prop('checked', false);
                                                bootbox.alert('<span style="background-color: red;">Data ' + mode + ' does not success.</span>');
                                            }
                                        },
                                        failure: function (response) {
                                            debugger;
                                            if (response.MESSAGE == "INVALIDTRIGGER") {
                                                $(row.context).prop('checked', false);
                                                bootbox.alert('<span style="background-color: red;">Data ' + mode + ' does not success due to invalid trigger.</span>');
                                            }
                                                else {
                                                bootbox.alert('Fail! Please try next time.');
                                                $(row.context).prop('checked', false)
                                            }
                                          ;
                                        },
                                        error: function (response) {
                                            debugger;
                                            bootbox.alert('Fail! Please try next time.');
                                            $(row.context).prop('checked', false);
                                        }
                                    });
                                }
                                else {
                                    $(row.context).prop('checked', false);
                                }
                            }
                        });
                    }
                });
            },
            columns: [
                {
                    title: "Select All",
                    headerTemplate: "<input type='checkbox' id='chkAll' onclick=checkAll('" + $scope.docVer + "') class='checkAllCls' title='" + $scope.docVer + " All'/><label for='check-all' style='margin-left:5px;'>" + $scope.docVer + " All</label> ",
                    filterable: false,
                    sortable: false,
                    width: "10%",
                    template: '<input type="checkbox" class="checkbox" type="checkbox"/><label for="check - all"></label>'
                },
                {
                    field: "VOUCHER_NO",
                    title: "Document No.",
                    filterable: true
                }, {
                    field: "VOUCHER_DATE",
                    title: "Date",
                    template: "#=kendo.toString(kendo.parseDate(VOUCHER_DATE),'dd MMM yyyy') == null?'':kendo.toString(kendo.parseDate(VOUCHER_DATE),'dd MMM yyyy') #"
                }, {
                    field: "VOUCHER_AMOUNT",
                    title: "Amount",
                    attributes: { style: "text-align:right;" },
                    template: '#= kendo.format("{0:n}",VOUCHER_AMOUNT) #'
                },
                {
                    field: "REFERENCE_NO",
                    title: "Manual No.",
                    template: "#=kendo.toString(REFERENCE_NO) == null ? (kendo.toString(SYN_ROWID)==null ? '' : kendo.toString(SYN_ROWID)): kendo.toString(REFERENCE_NO) #"
                },
                {
                    field: "CREATED_BY",
                    title: "Prepared By"
                }, {
                    field: "CREATED_DATE",
                    title: "Prepared Date & Time",
                    template: "#= kendo.toString(kendo.parseDate(CREATED_DATE),'dd MMM yyyy hh:mm:ss') #",
                },
                {
                    field: "CHECKED_BY",
                    title: "Checked By",
                    hidden: true,
                },
                {
                    field: "CHECKED_DATE",
                    title: "Checked Date",
                    template: "#=kendo.toString(kendo.parseDate(CHECKED_DATE),'dd MMM yyyy') == null?'':kendo.toString(kendo.parseDate(CHECKED_DATE),'dd MMM yyyy') #",
                    hidden: true,
                },
                {
                    field: "AUTHORISED_BY",
                    title: "Authorised By",
                    hidden: true,
                },
                {
                    field: "POSTED_DATE",
                    title: "Posted Date",
                    template: "#=kendo.toString(kendo.parseDate(POSTED_DATE),'dd MMM yyyy') == null?'':kendo.toString(kendo.parseDate(POSTED_DATE),'dd MMM yyyy') #",
                    hidden: true,
                },
                {
                    field: "MODIFY_DATE",
                    title: "Modified Date",
                    template: "#=kendo.toString(kendo.parseDate(MODIFY_DATE),'dd MMM yyyy') == null?'':kendo.toString(kendo.parseDate(MODIFY_DATE),'dd MMM yyyy') #"
                },
                {
                    field: "SYN_RowID",
                    title: "SYN ROWID",
                    hidden: true,
                }
            ]
        };
        $scope.doSomething = function (orderNo, MODULE_CODE) {  
            debugger;
            showloader();
            var voucherno = orderNo.split(new RegExp('/', 'i')).join('_');
            debugger;
            if (MODULE_CODE == "01")
                window.location.href = "/DocumentTemplate/Home/Index#!DT/FinanceVoucher/" + formCode + "/" + voucherno + ""
            else if (MODULE_CODE === "02")
                window.location.href = "/DocumentTemplate/Home/Index#!DT/Inventory/" + formCode + "/" + voucherno + ""
            else if (MODULE_CODE == "03")
                window.location.href = "/DocumentTemplate/Home/Index#!DT/Inventory/" + formCode + "/" + voucherno + ""
            else if (MODULE_CODE == "04")
                window.location.href = "/DocumentTemplate/Home/Index#!DT/formtemplates/" + formCode + "/" + voucherno + ""
            //window.location.href = "/DocumentTemplate/Home/Index#!DT/formtemplate/" + formCode + "/" + voucherNo + "/" + tableName + ""
        }
    };
});
function checkAll(mode) {
    debugger;
    var grid = $("#kGridVer").data("kendoGrid");
    var ds = grid.dataSource.view();
    var model = [];
    if (ds.length > 0) {
        bootbox.confirm({
            title: mode,
            message: "Do you want to " + mode + " all data?",
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-success',
                    label: '<i class="fa fa-check"></i> Yes',
                },
                cancel: {
                    label: 'No',
                    className: 'btn-danger',
                    label: '<i class="fa fa-times"></i> No',
                }
            },
            callback: function (result) {
                debugger;
                if (result == true) {
                    var formcode = ds[0].Form_CODE;
                    for (var i = 0, j = 0; i < ds.length; i++) {
                        if (ds[i].VOUCHER_NO != 'undefined' || ds[i].VOUCHER_NO != '' || ds[i].VOUCHER_NO != null) {
                            model[j] = ds[i].VOUCHER_NO;
                            j++;
                        }
                    }
                    if (model.length > 0 && model.length == ds.length) {
                        $.ajax({
                            type: "POST",
                            url: window.location.protocol + "//" + window.location.host + '/api/TemplateApi/BulkUpdateMasterTranasactionForVerification?formcode=' + formcode + '&mode=' + mode,
                            contentType: "application/json; charset-utf-8",
                            datatype: JSON,
                            data: JSON.stringify(model),
                            success: function (response) {
                                debugger;
                                if (response.status == true) {
                                    $('#chkAll').prop('checked', false);
                                    bootbox.alert(response.message);
                                    $("#kGridVer").data("kendoGrid").dataSource.read();
                                }
                                if (response.MESSAGE == "INVALIDTRIGGER") {
                                    $('#chkAll').prop('checked', false);
                                    bootbox.alert('<span style="background-color: red;">Does not success due to invalid trigger.</span>');
                                }
                                else {
                                    bootbox.alert(response.message);
                                    $('#chkAll').prop('checked', false);
                                }
                            },
                            failure: function (response) {
                                debugger;
                                if (response.MESSAGE == "INVALIDTRIGGER") {
                                    $('#chkAll').prop('checked', false);
                                    bootbox.alert('<span style="background-color: red;">Does not success due to invalid trigger.</span>');
                                }
                                else {
                                    bootbox.alert('Fail! Please try next time.');
                                    $('#chkAll').prop('checked', false);
                                }
                            },
                            error: function (response) {
                                debugger;
                                bootbox.alert('Fail! Please try next time.');
                                $('#chkAll').prop('checked', false);
                            }
                        });
                    }
                    else {
                        bootbox.alert("Nothing to " + mode + " or voucher number count doesnot match.");
                        $('#chkAll').prop('checked', false);
                    }
                }
                else {
                    $('#chkAll').prop('checked', false);
                }
            }
        });
    }
    else {
        bootbox.alert("List empty.");
        $('#chkAll').prop('checked', false);
    }
}