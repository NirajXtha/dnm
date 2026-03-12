PMModule.controller('requisitionPendingReport', function ($scope,  $window, $http, $filter, $timeout) {

    var response = $http.get('/api/ProjectApi/GetRequisitionPendingDetails?formcode=160');

    response.then(function (budgetResult) {
        if (budgetResult.data != "") {
            $scope.dynamicModalData = budgetResult.data;
            initializeKendoGrid();
        }
    });

    function initializeKendoGrid() {
        var dataSource = new kendo.data.DataSource({
            data: $scope.dynamicModalData
        });

        $("#kGrid").kendoGrid({
            dataSource: dataSource,
            height: 400,
            sortable: true,
            pageable: true,
            toolbar: ["excel"],
            excel: {
                fileName: "Requisition Pending Report.xlsx",
                allPages: true
            },
            columns: [
                { field: "VOUCHER_NO", title: "Document No.", width: 130 },
                { field: "VOUCHER_DATE", title: "Date", template: "#=kendo.toString(kendo.parseDate(VOUCHER_DATE),'dd MMM yyyy') == null?'':kendo.toString(kendo.parseDate(VOUCHER_DATE),'dd MMM yyyy') #", width: 130 },
                { field: "COMPANY_EDESC", title: "Company Name", width: 120 },
                { field: "BRANCH_EDESC", title: "Branch Name", width: 130 },
                { field: "ITEM_EDESC", title: "Product Description", width: 150 },
                { field: "LOCATION_EDESC", title: "Godown Location", width: 200 },
                { field: "MU_CODE", title: "Unit", width: 80 },
                { field: "CALC_QUANTITY", title: "Quantity", attributes: { style: "text-align:right;" }, template: '#= kendo.format("{0:n}",CALC_QUANTITY) #', width: 80 },
                { field: "CALC_UNIT_PRICE", title: "Rate", attributes: { style: "text-align:right;" }, template: '#= kendo.format("{0:n}",CALC_UNIT_PRICE) #', width: 80 },
                { field: "CALC_TOTAL_PRICE", title: "Total", attributes: { style: "text-align:right;" }, template: '#= kendo.format("{0:n}",CALC_TOTAL_PRICE) #', width: 80 },
                //{
                //    title: "Cost Centre",
                //    attributes: { style: "text-align: center;" },
                //    columns: [{
                //        field: "BUDGET_FLAG",
                //        title: "Budget",
                //        template: "<span>#: (BUDGET_FLAG == null) ? '-' : BUDGET_FLAG #</span>",
                //        width: 80
                //    },
                //    {
                //        field: "BUDGET_AMOUNT",
                //        title: "Budget Amount",
                //        attributes: { style: "text-align: right;" },
                //        template: '#= kendo.format("{0:n}",BUDGET_AMOUNT) #',
                //        width: 100
                //    }]
                //},
                { field: "PROJECT_NAME", title: "Project Name", width: 130 },
                { field: "SUB_PROJECT_NAME", title: "Sub Project Name", width: 130 },
                { field: "CREATED_BY", title: "Prepared By ", width: 130 },
                { field: "CREATED_DATE", title: "Prepared Date & Time", template: "#= kendo.toString(kendo.parseDate(CREATED_DATE),'dd MMM yyyy hh:mm:ss') #", width: 150 },
            ]
        });

        // Function to filter data
        $("#itemtxtSearchString").keyup(function () {
            var val = $(this).val().toLowerCase(); // Get the search input value
            dataSource.filter({
                logic: "or",
                filters: [
                    { field: "PROJECT_NAME", operator: "contains", value: val },
                    { field: "CALC_TOTAL_PRICE", operator: "eq", value: parseInt(val) || null },
                    { field: "CALC_QUANTITY", operator: "eq", value: parseInt(val) || null },
                    { field: "CALC_UNIT_PRICE", operator: "eq", value: parseInt(val) || null },
                    { field: "BUDGET_AMOUNT", operator: "eq", value: parseInt(val) || null },
                    { field: "VOUCHER_NO", operator: "contains", value: val },
                    { field: "VOUCHER_DATE", operator: "contains", value: val },
                    { field: "COMPANY_EDESC", operator: "contains", value: val },
                    { field: "BRANCH_EDESC", operator: "contains", value: val },
                    { field: "ITEM_EDESC", operator: "contains", value: val },
                    { field: "LOCATION_EDESC", operator: "contains", value: val },
                    { field: "PROJECT_NAME", operator: "contains", value: val },
                    { field: "SUB_PROJECT_NAME", operator: "contains", value: val },
                    { field: "BUDGET_FLAG", operator: "contains", value: val },
                    { field: "MU_CODE", operator: "contains", value: val }
                ]
            });
        });
    }
    });
