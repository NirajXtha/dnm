DTModule.controller('draftCtrl', function ($scope, $http, $routeParams, $window, $filter) {

    $scope.draftList = [];
    $scope.orientation = "horizontal";
    $scope.formcode = "";
 
    $scope.moduleCode = $routeParams.module;
    if ($scope.moduleCode == "01") {
        $scope.modulename = "Financial Accounting Draft"
    }
    else if ($scope.moduleCode == "02") {
        $scope.modulename = "Inventory & Procurement Draft"
    }
    else if ($scope.moduleCode == "04") {
        $scope.modulename = "Sales & Revenue Draft"
    }
    var formcode = "";


    $scope.BackFromMenu = function () {
        $window.location.href = '/DocumentTemplate/Home/Dashboard';
    }

    $scope.BindDetailGrid = function (formcode, tableName, formname) {
        $scope.formcode = formcode;
        
        $scope.formname = formname;
        $("#kGrid").html("");
        BindGrid(formcode, tableName);
        setTimeout(function () {
            $('[data-toggle="tooltip"]').tooltip();
        }, 10)
        // get Splitter object
        //var splitter = $("#splitter").data("kendoSplitter");
        // modify the size of the first pane
        //splitter.options.panes[0].size = "0px";
        // force layout readjustment
        //splitter.resize(true);
    }


    
    var req = "/api/TemplateApi/GetDraftList?moduleCode=" + $scope.moduleCode + "&formCode=" + $scope.formcode;
    $http.get(req).then(function (response) {
        $scope.draftList = response.data;

    });

    function BindGrid(formCode, tableName) {
       
        $scope.mainGridOptions = {
            dataSource: {
                type: "json",
                transport: {

                    read: "/api/TemplateApi/GetDraftDetailList?formcode=" + formCode
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
                $("#kGrid tbody tr").css("cursor", "pointer");
                $("#kGrid tbody tr").on('dblclick', function () {
                    var voucherNo = $(this).find('td span').html()
                    $scope.doSomething(voucherNo);
                })
            },
            columns: [

                {
                    field: "VOUCHER_NO",
                    title: "Document No.",
                    filterable: true
                   // width: "10"
                }, {
                    field: "VOUCHER_DATE",
                    title: "Date",
                    template: "#=kendo.toString(kendo.parseDate(VOUCHER_DATE),'dd MMM yyyy') == null?'':kendo.toString(kendo.parseDate(VOUCHER_DATE),'dd MMM yyyy') #",
                    //width: "10"
                }, {
                    field: "VOUCHER_AMOUNT",
                    title: "Amount",
                    //width: "10",
                    attributes: { style: "text-align:right;" },
                    template: '#= kendo.format("{0:n}",VOUCHER_AMOUNT) #'
                },
                 {
                     title: "Manual No.",
                     //width: "10"
                 },
            {
                field: "CREATED_BY",
                title: "Prepared By",
                //width: "10"
            }, {
                field: "CREATED_DATE",
                title: "Prepared Date & Time",
                template: "#= kendo.toString(kendo.parseDate(CREATED_DATE),'dd MMM yyyy hh:mm:ss') #",
                //width: "10"
            },
             {
                 field: "CHECKED_BY",
                 title: "Checked By",
                 //width: "10",
                  hidden: true,
             },
             {
                 field: "CHECKED_DATE",
                 title: "Checked Date",
                 //template: "#= kendo.toString(kendo.parseDate(CHECKED_DATE),'dd MMM yyyy') #",
                 template: "#=kendo.toString(kendo.parseDate(CHECKED_DATE),'dd MMM yyyy') == null?'':kendo.toString(kendo.parseDate(CHECKED_DATE),'dd MMM yyyy') #",
                 //width: "8",
                  hidden: true,
             },
              {
                  field: "AUTHORISED_BY",
                  title: "Authorised By",
                  //width: "8",
                  hidden: true,
              },
               {
                   field: "POSTED_DATE",
                   title: "Posted Date",
                   //template: "#= kendo.toString(kendo.parseDate(POSTED_DATE),'dd MMM yyyy') #",
                   template: "#=kendo.toString(kendo.parseDate(POSTED_DATE),'dd MMM yyyy') == null?'':kendo.toString(kendo.parseDate(POSTED_DATE),'dd MMM yyyy') #",
                   //width: "7",
                  hidden: true,
               },
               {
                   field: "MODIFY_DATE",
                   title: "Modified Date & Time",

                   //template: "#= kendo.toString(kendo.parseDate(MODIFY_DATE),'dd MMM yyyy') #",
                   template: "#=kendo.toString(kendo.parseDate(MODIFY_DATE),'dd MMM yyyy hh:mm:ss') == null?'':kendo.toString(kendo.parseDate(MODIFY_DATE),'dd MMM yyyy hh:mm:ss') #",
                   //width: "8"
               },
                {
                    field: "SYN_RowID",
                    title: "SYN ROWID",
                    //width: "5",
                    hidden: true,
                },
             {
                 //command: [{
                 template: '<a class="edit glyphicon glyphicon-edit" title="Edit" ng-click="doSomething(dataItem.VOUCHER_NO)" style="color:grey;"><span class="sr-only"></span> </a>',
                 //template: '#= redirectEditOrder(ORDER_NO)#',
                 //}],
                 title: " ",
                 width: "40px"
             }
            ],
            //dataBound: function (e) {
            //    
            //    var that = this;
            //    $(that.tbody).on("click", "tr", function (e) {
            //        window.location.href = $(this).find('td:first a').attr('href');
            //    });
            //}

        };

        $scope.doSomething = function (orderNo) {
          
            showloader();
            if ($scope.moduleCode == "01")
                window.location.href = "/DocumentTemplate/Home/Index#!DT/FinanceVoucher/" + formCode + "/" + orderNo + ""
            else if ($scope.moduleCode === "02")
                window.location.href = "/DocumentTemplate/Home/Index#!DT/Inventory/" + formCode + "/" + orderNo + ""
            else if ($scope.moduleCode == "03")
                window.location.href = "/DocumentTemplate/Home/Index#!DT/Production/" + formCode + "/" + orderNo + ""
            else if ($scope.moduleCode == "04")
                window.location.href = "/DocumentTemplate/Home/Index#!DT/formtemplates/" + formCode + "/" + orderNo + ""
            // window.location.href = "/DocumentTemplate/Home/Index#!DT/formtemplate/" + formCode + "/" + voucherNo + "/" + tableName + ""
        }

    }
    //$scope.onsiteSearch = function ($this) {
    //    
    //    var q = $("#txtSearchString").val();
    //    var grid = $("#kGrid").data("kendogrid");
    //    grid.dataSource.query({
    //        page: 1,
    //        pageSize: 50,
    //        filter: {
    //            logic: "or",
    //            filters: [
    //              { field: "VOUCHER_NO", operator: "contains", value: q },
    //              { field: "VOUCHER_DATE", operator: "contains", value: q },
    //              { field: "VOUCHER_AMOUNT", operator: "contains", value: q },
    //              { field: "CREATED_BY", operator: "contains", value: q },
    //              { field: "CREATED_DATE", operator: "contains", value: q },
    //              { field: "CHECKED_BY", operator: "contains", value: q },
    //              { field: "CHECKED_DATE", operator: "contains", value: q },
    //              { field: "AUTHORISED_BY", operator: "contains", value: q },
    //              { field: "POSTED_DATE", operator: "contains", value: q },
    //              { field: "MODIFY_DATE", operator: "contains", value: q },
    //              { field: "SYN_RowID", operator: "contains", value: q },




    //            ]
    //        }
    //    });
    //};




    //$scope.setHeight = function () {
    //    kendo.resize($($scope.splitter.wrapper[0]));
    //}
});
