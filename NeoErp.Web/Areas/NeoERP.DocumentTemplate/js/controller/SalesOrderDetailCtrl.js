DTModule.controller('salesorderdetailCtrl', function ($scope, $http, $filter, $q, salesorderdetailservice, salesorderdetailfactory) {
  
    $scope.formcode = "177";
    $scope.tableName = "SA_SALES_INVOICE";
    document.ttt = $scope.formcode;
    function getUrlVars() {
       
        var vars = [], hash;
        var hashes = window.location.href.slice(window.location.href.indexOf('!') + 1).split('/');
        for (var i = 0; i < hashes.length; i++) {
 
            hash = hashes[i].split('=');
            vars.push(hash[0]);
            vars[hash[0]] = hash[1];
        }
        return vars;
    }

    $scope.sodata = function () {
        salesorderdetailservice.GetAllSalesOrderData().then(function (d) {
            $scope.salesorderdata = d.data;

        }, function () { alert("Error at: Get All Sales Order Data") });
        function isEmpty(obj) {
            return Object.keys(obj).length === 0;
        }
    };
    $scope.sodata();
    $scope.salesorderdata = null;
    
    var tableName = $scope.tableName.split('_');
    var lastName = tableName[tableName.length - 1];
   
    $scope.mainGridOptions = {

        dataSource: {
            type: "json",
            transport: {

                read: "/api/TemplateApi/GetAllSalesOrderDetails"
            },
            pageSize: 50,
            //serverPaging: true,
            serverSorting: true
        },
        scrollable: true,
        height: 450,
        sortable: true,
        pageable: true,
        //columns: col,
        columns: [
            {
                hidden: true,
                field: "FORM_CODE",

            },
            {
            field: "VOUCHER_NO",
            title: "Document No",
            width: "120px"
        }, {
            field: "ORDER_DATE",
            title: "ORDER DATE",
            template: "#= kendo.toString(kendo.parseDate(ORDER_DATE),'dd MMM yyyy') #",
            width: "120px"
        }, {
            field: "CREATED_BY",
            title: "CREATED BY",
            width: "120px"
        }, {
            field: "CREATED_DATE",
            title: "CREATED DATE",
            template: "#= kendo.toString(kendo.parseDate(CREATED_DATE),'dd MMM yyyy') #",
            width: "120px"
        },
         {
             //command: [{
             template: "<span class='k-button' data-checkfor='#= ORDER_NO#' ng-click='doSomething(this)'>Edit</span>",
             //template: '#= redirectEditOrder(ORDER_NO)#',
             //}],
             title: " ",
             width: "50px"
         }
        ]

    };

    $scope.onsiteSearch = function ($this) {

        var q = $("#txtSearchString").val();
        var grid = $("#kGrid").data("kendo-grid");
        grid.dataSource.query({
            page: 1,
            pageSize: 50,
            filter: {
                logic: "or",
                filters: [
                  { field: "ORDER_NO", operator: "contains", value: q },
                  { field: "ORDER_DATE", operator: "contains", value: q },
                  { field: "CREATED_BY", operator: "contains", value: q }
                ]
            }
        });
    }

    $scope.doSomething = function ($this) {
      
        var formcode = $this.dataItem.FORM_CODE;
        var orderNo = $this.dataItem.ORDER_NO.replace("/", "_");
     

        window.location.href = "/DocumentTemplate/Home/Index#!DT/formtemplates/" + formcode + "/" + orderNo + ""
    }



});