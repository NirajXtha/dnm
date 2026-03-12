var SchedularBuilder = (function (schedularBuilder, $, ko) {
    debugger;
    "use strict";

    schedularBuilder.Create = function () {
        debugger;
        var config = {
            QueryCheckerButton: ".buttonQueryChecker",
           
        };

        var viewModel = {
            schedularname: ko.observable(),
            email: ko.observable(),
            spCode: ko.observable(),
            reportName: ko.observable(),
            Subject: ko.observable(),
            sqlquery: ko.observable(),
            Template: ko.observable(),
            sqltype:ko.observable(),
            fequencytype:ko.observable(),
            startdate:ko.observable(),
            Days: ko.observable(1),
            messagetype: ko.observable(),
            summaryReport: ko.observable(false),
            reportType: ko.observable('QB'),
            checkQuery: function () {
              //  alert(viewModel.sqlQuery());
                var settings = {
                    url: window.virtualPath + "api/QueryBuilder/CheckQuery?query="+viewModel.sqlQuery(),
                    type: "POST",
                    contentType: "application/json",
                    data: {
                        "query": viewModel.sqlQuery(),
                    },
                    success: function (data) {
                        
                        toastr["success"]("Successfully Run Query","Success");
                    },
                    statusCode: {
                        401: function () {
                        
                        },
                        403: function () {
                         
                        },
                        500: function (data) {
                          
                            toastr["error"](data.responseJSON.MESSAGE, "Sql Query is failed to run.");
                        }
                    }
                };
                $.ajax(settings);
            },
            saveData: function (evt) {
                debugger;
                var isTemplate = false;
                if (evt == "1")
                    isTemplate = true;
                //alert($("#MessageTemplate").val());
                //evt.preventDefault();
                var strVal = $.trim(viewModel.email());
                var lastChar = strVal.slice(-1);
                if (lastChar == ',') {
                    strVal = strVal.slice(0, -1);
                }
                if (viewModel.reportName() == "" && viewModel.reportType() == "RB") {
                    toastr["warning"]("Please Select report", "warning");
                    return;
                }
                if (viewModel.fequencytype() == "fequencyrange") {
                    if (viewModel.Days() == 0)
                        viewModel.Days(1);
                }
                var param = {
                    schedularName: viewModel.schedularname(),
                    email: strVal,
                    Subject: viewModel.Subject(),
                    sqlQuery: viewModel.reportType()=="QB"?viewModel.sqlquery():"",
                    Template: $(".note-editable").html(),
                    sqltype: viewModel.sqltype(),
                    fequencytype: viewModel.fequencytype(),
                    startdate: viewModel.startdate(),
                    Days: viewModel.Days(),
                    summaryReport: viewModel.summaryReport(),
                    reportType: viewModel.reportType(),
                    reportName: viewModel.reportType() == "QB" ?"":viewModel.reportName(),
                    employeeCode: viewModel.reportType() == "QB" ? [] : $("#employeeFilterOption").data("kendoMultiSelect").value(),
                    isTemplate: isTemplate
                }
                var settings = {
                    url: window.virtualPath + "api/Scheduler/CreateSchedular",
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify(param),
                    //data: JSON.stringify({
                    //    schedularName: viewModel.schedularname(),
                    //    email: strVal,
                    //    Subject: viewModel.Subject(),
                    //    sqlQuery: viewModel.sqlquery(),
                    //    Template: $(".note-editable").html(),
                    //    sqltype: viewModel.sqltype(),
                    //    fequencytype: viewModel.fequencytype(),
                    //    startdate: viewModel.startdate(),
                    //    Days: viewModel.startdate(),
                    //    summaryReport: viewModel.summaryReport(),
                    //    reportType: viewModel.reportType(),
                    //    reportName: viewModel.reportName(),
                    //    employeeCode: $("#employeeFilterOption").data("kendoMultiSelect").value(),
                    //}),
                    success: function (data) {

                        toastr["success"]("Widget Is Created Successfully", "Success");
                    },
                    statusCode: {
                        401: function () {

                        },
                        403: function () {

                        },
                        500: function (data) {

                            toastr["error"](data.responseJSON.MESSAGE, "Sql Query is failed to run.");
                        }
                    }
                };
                $.ajax(settings);
            },
            selectData: function (evt) {
            },
            updateData: function (evt) {
                //evt.preventDefault();
                var settings = {
                    url: window.virtualPath + "api/QueryBuilder/UpdateWidzed",
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify({
                        widgetsName: viewModel.reportname(),
                        WidgetsBGColor: viewModel.selectedBgcolorChooser(),
                        sqlQuery: viewModel.sqlQuery(),
                        widgetsColor: viewModel.selectedfontcolorChooser(),
                        widgetsTitle: viewModel.widgetTitle(),
                        //AA
                        widgetFontIcon: viewModel.selectedIcon(),
                        Isactive: viewModel.Active(),
                        OrderNo: viewModel.Order(),
                        MidBGColor: viewModel.MidselectedBgcolorChooser(),
                        MidFontColor: viewModel.MidselectedfontcolorChooser(),
                        IsMidBlink: viewModel.MidenableBlink(),
                        MaxBGColor: viewModel.MaxselectedBgcolorChooser(),
                        MaxFontColor: viewModel.MaxselectedfontcolorChooser(),
                        IsMaxBlink: viewModel.MaxenableBlink(),
                        MaxValue: viewModel.highestValue(),
                        MinValue: viewModel.lowestValue(),
                        IsBlink: viewModel.enableBlink(),
                        LABELPOSITION: viewModel.selectedlabelPositions(),
                        SPEEDOMETERMAXVALUE: viewModel.speedometerMaxValue(),
                        MAXVALUEQUERY: viewModel.sqlHigestQuery(),
                        MINVALUEQUERY: viewModel.sqlLowestQuery(),
                        ChartType: viewModel.chosenChart(),
                        WidgetsId: viewModel.reportId(),
                    }),
                    success: function (data) {

                        toastr["success"]("Successfully Run Query", "Success");
                    },
                    statusCode: {
                        401: function () {

                        },
                        403: function () {

                        },
                        500: function (data) {

                            toastr["error"](data.responseJSON.MESSAGE, "Sql Query is failed to run.");
                        }
                    }
                };
                $.ajax(settings);
            },
        };
      
        return {
            config: config,
            viewModel: viewModel,
            init: function () {
                
                ko.applyBindings(viewModel);

                var reportList = [{ text: "Daily Sales Report", value: "DailySalesReport" }, { text: "Bonus Statement Dealer", value: "BonusStatement" }, { text: "Bank Guaranty (Party)", value: "BankGuarantyByParty" }, { text: "Bank Guaranty (Sales Person)", value: "BankGuarantyBySalesPerson" }, { text: "Bank Guaranty (Dealer)", value: "BankGuarantyByDealer" }, { text: "Dealerwise Discount Ledger", value: "DealerwiseDiscountLedger" }];
                $(".reportbased").css({ "display": "none" });

                $('input[type=radio][name=reportType]').change(function () {
                    if (this.value == 'RB') {
                        $(".reportbased").css({ "display": "block" });
                        $(".querybased").css({ "display": "none" });
                    }
                    else if (this.value == 'QB') {
                        $(".reportbased").css({ "display": "none" });
                        $(".querybased").css({ "display": "block" });
                    }
                });

                var autoCompleteurl = window.location.protocol + "//" + window.location.host + "/api/Scheduler/GetEmployeeListForScheduler";
                var autoCompletedataSource = new kendo.data.DataSource({
                    transport: {
                        read: {
                            url: autoCompleteurl,
                            dataType: "json",
                            contentType: "application/json; charset=utf-8"
                        }
                    }
                });

                var employeeurl = window.location.protocol + "//" + window.location.host + "/api/Scheduler/GetEmployeeList";
                var emploeeDataSource = new kendo.data.DataSource({
                    transport: {
                        read: {
                            url: employeeurl,
                            dataType: "json",
                            contentType: "application/json; charset=utf-8"
                        }
                    }
                });

                $("#reportList").kendoDropDownList({
                    dataTextField: "text",
                    dataValueField: "value",
                    optionLabel: "Please Select...",
                    dataSource: reportList,
                    index: 0,
                    select: function (e) {
                        var report_name = e.dataItem.value;
                        viewModel.reportName(report_name);
                    }
                });

                $('#employeeMultiSelect').kendoMultiSelect({
                    dataSource: autoCompletedataSource,
                    dataTextField: "EmployeeName",
                    dataValueField: "EmployeeCode",
                    height: 600,
                    headerTemplate: '<div class="col-md-offset-4"><strong>Employee...</strong></div>',
                    placeholder: "Select Employee...",
                    select: onSelect,
                    deselect: onDeselect,
                    dataBound: function (e) {
                        var current = this.value();
                        this._savedOld = current.slice(0);
                    }
                });

                $('#employeeFilterOption').kendoMultiSelect({
                    dataSource: emploeeDataSource,
                    dataTextField: "EmployeeName",
                    dataValueField: "EmployeeCode",
                    autoClose: false,
                    height: 600,
                    headerTemplate: '<div class="col-md-offset-4"><strong>Employee...</strong></div>',
                    placeholder: "---Select Employee---",
                    select: onFilterSelect,
                    deselect: onFilterDeselect,
                    dataBound: function () {
                        var current = this.value();
                        this._savedOld = current.slice(0);
                    }
                });

                var customerurl = window.location.protocol + "//" + window.location.host + "/api/Scheduler/GetAllCustomerswithoutlet";
                var customerDataSource = new kendo.data.DataSource({
                    transport: {
                        read: {
                            url: customerurl,
                            dataType: "json",
                            contentType: "application/json; charset=utf-8"
                        }
                    }
                });

                customerDataSource.fetch(function () {
                    
                   var custDataSource = _.filter(this.data(), function (item, index) { return item.EMAIL != null })
                   $('#customerOption').kendoMultiSelect({
                       dataSource: custDataSource,
                       dataTextField: "CustomerName",
                       dataValueField: "CustomerCode",
                       autoClose: false,
                       height: 600,
                       headerTemplate: '<div class="col-md-offset-4"><strong>Customer...</strong></div>',
                       placeholder: "---Select Customer---",
                       select: onCustSelect,
                       deselect: onCustDeselect,
                       dataBound: function () {
                           var current = this.value();
                           this._savedOld = current.slice(0);
                       }
                   });
                   var email = "";
                   function onCustSelect(e) {
                       debugger;
                       //if ($("#customerOption").data("kendoMultiSelect").value().length > 1)
                       //{
                       //    if (e.dataItem.CustomerName == "ALL" && e.dataItem.CustomerCode == "0") {
                       //        e.preventDefault();
                       //    }
                       //}
                       //if (e.dataItem.CustomerName == "ALL" && e.dataItem.CustomerCode == "0" && $("#customerOption").data("kendoMultiSelect").value().length <= 1)
                       //{
                           
                       //    $(".custchk").css("display", "block");
                       //    $("#customerOption").data("kendoMultiSelect").options.maxSelectedItems = 1;
                       //    return;
                       //}
                       var personalEmailAddress = $.trim(e.dataItem.EMAIL);
                       //var ifEmailExists = $('#email').val();
                       if(viewModel.email())
                         email = viewModel.email();
                       if (personalEmailAddress != null && personalEmailAddress != "" && personalEmailAddress != undefined) {
                           email += personalEmailAddress + ",";
                           viewModel.email(email);
                           // $('#email').val(email);
                       }
                   };
                   function onCustDeselect(e) {
                       debugger;
                       //if (e.dataItem.CustomerName == "ALL" && e.dataItem.CustomerCode == "0") {
                       //    $("#customerOption").data("kendoMultiSelect").options.maxSelectedItems = 1000;
                       //    $(".custchk").css("display", "none");
                       //    $('#email').val("");                          
                       //    $("#Customer").prop("checked", false);
                       //    $("#Outlet").prop("checked", false);
                       //    return;
                       //}
                       var personalEmailAddress = $.trim(e.dataItem.EMAIL);
                       var ifEmailExists = viewModel.email();
                        var ifEmailExists = $('#email').val();
                       if ($.trim(ifEmailExists) != "") {
                           var list = ifEmailExists.split(",");
                           list.splice(list.length - 1, 1);
                           $.each(list, function (key, val) {
                               if (personalEmailAddress == val) {
                                   var emailtobereplaced = val + ",";
                                   var replacedEmail = ifEmailExists.replace(emailtobereplaced, "");
                                   $('#email').val(replacedEmail);
                                   viewModel.email(replacedEmail);
                                   // $('#email').val(replacedEmail);
                                   email = "";
                               }
                           })
                       }
                   };
                });


        

                $("#Outlet").change(function () {
                    debugger;
                    if (this.checked) {
                        if ($("#email").val() == "") {
                            //$("#email").val(this.value);
                            viewModel.email(this.value);
                        }
                        else {
                            //$("#email").val($("#email").val() + this.value)
                            viewModel.email($("#email").val() + this.value);
                           
                        }
                        var multiselect = $("#customerOption").data("kendoMultiSelect");
                        multiselect.enable(false);
                    }
                    else {
                        if ($("#email").val() != "") {
                            var str1 = $("#email").val();
                            var str2 = "outlet";
                            if (str1.indexOf(str2) != -1) {
                                var str3=str1.replace(str2, "");
                                //$("#email").val(str3);
                                viewModel.email(str3);

                            }
                        }
                        if ($('#Customer').is(':checked')) {
                            return;
                        }
                        else {
                            var multiselect = $("#customerOption").data("kendoMultiSelect");
                            multiselect.enable(true);
                        }

                       
                    }

                });
                $("#Customer").change(function () {
                    debugger;
                    if (this.checked) {
                        if ($("#email").val() == "") {
                            viewModel.email(this.value);//$("#email").val(this.value);
                        }
                        else {
                            //$("#email").val($("#email").val() + this.value)
                            viewModel.email($("#email").val() + this.value);
                        }
                        var multiselect = $("#customerOption").data("kendoMultiSelect");
                        multiselect.enable(false);
                    }
                    else {
                        if ($("#email").val() != "") {
                            var str1 = $("#email").val();
                            var str2 = "Customer";
                            if (str1.indexOf(str2) != -1) {
                                var str3 = str1.replace(str2, "");
                                //$("#email").val(str3);
                                viewModel.email(str3);

                            }
                        }
                        if ($('#Outlet').is(':checked')) {
                            return;
                        }
                        else
                        {
                            var multiselect = $("#customerOption").data("kendoMultiSelect");
                            multiselect.enable(true);
                        }
                       
                    }
                });

                var email = "";
                function onSelect(e) {

                    var personalEmailAddress = $.trim(e.dataItem.PersonalEmail);
                   // var ifEmailExists = $('#email').val();
                    var ifEmailExists = viewModel.email();
                    //if (viewModel.email())
                    //    email = viewModel.email();
                    if (personalEmailAddress != null && personalEmailAddress != "" && personalEmailAddress != undefined) {
                        email += personalEmailAddress + ",";
                        viewModel.email(email);
                       // $('#email').val(email);
                    }
                };
                function onDeselect(e) {

                    var personalEmailAddress = $.trim(e.dataItem.PersonalEmail);
                    var ifEmailExists = viewModel.email();
                   // var ifEmailExists = $('#email').val();
                    if ($.trim(ifEmailExists) != "") {
                        var list = ifEmailExists.split(",");
                        list.splice(list.length - 1, 1);
                        $.each(list, function (key, val) {
                            if (personalEmailAddress == val) {
                                var emailtobereplaced = val + ",";
                                var replacedEmail = ifEmailExists.replace(emailtobereplaced, "");
                                $('#email').val(replacedEmail);
                               // $('#email').val(replacedEmail);
                                email = "";
                            }
                        })
                    }
                };

                
                var SpCode = "";
                function onFilterSelect(e) {
                    debugger;
                    var sp_code = $.trim(e.dataItem.EmployeeCode);
                    if (sp_code != null && sp_code != "" && sp_code != undefined) {
                        SpCode += sp_code + ",";
                        viewModel.spCode(SpCode);
                        // $('#email').val(email);
                    }
                };
                function onFilterDeselect(e) {

                    var personalEmailAddress = $.trim(e.dataItem.PersonalEmail);
                    var ifEmailExists = viewModel.email();
                    // var ifEmailExists = $('#email').val();
                    if ($.trim(ifEmailExists) != "") {
                        var list = ifEmailExists.split(",");
                        list.splice(list.length - 1, 1);
                        $.each(list, function (key, val) {
                            if (personalEmailAddress == val) {
                                var emailtobereplaced = val + ",";
                                var replacedEmail = ifEmailExists.replace(emailtobereplaced, "");
                                $('#email').val(replacedEmail);
                                // $('#email').val(replacedEmail);
                                email = "";
                            }
                        })
                    }
                };

                $('#employeeMultiSelect_listbox').slimScroll({
                    //height: '100px',

                    alwaysVisible: true

                });
            },
            render: function () {
                $("#gauge").kendoRadialGauge({
                    //gaugeArea: {
                    //    width: "230px",
                    //    height: "130",
                    //},
                    
                    pointer: {
                        value: 65
                    },

                    scale: {
                        minorUnit: 5,
                        startAngle: -30,
                        endAngle: 210,
                        max: 180,
                        labels: {
                            position: "inside"
                        },
                        ranges: [
                            {
                                from: 80,
                                to: 120,
                                color: '#ff7a00'
                            }, {
                                from: 120,
                                to: 150,
                                color: "#ff7a00"
                            }, {
                                from: 150,
                                to: 180,
                                color: "#c20000"
                            }
                        ]
                    }
                });
              
            },
         
        };
    };

    return schedularBuilder;
}(SchedularBuilder || {}, jQuery, ko));

