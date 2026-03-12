distributionModule.controller("DivisionLimitCtrl", function ($scope, $timeout, crudAJService) {

    $("#DivisionGrid").kendoGrid({
            dataSource: {
                dataType: "json",
                transport: {
                    read: window.location.protocol + "//" + window.location.host + "/api/Distributor/GetDivisionWiseCreditLimitList",
                },
                pageSize: 20,
                group: { field: "CUSTOMER_EDESC", aggregates: [{ field: "CREDIT_LIMIT", aggregate: "sum" }, { field: "UTI", aggregate: "sum" }, { field: "BALANCE", aggregate: "sum" }] },
            },
            height: 550,
            toolbar: kendo.template($("#toolbar-template").html()),
            excel: {
                fileName: "Division Wise Credit Limit",
                allPages: true,
            },
            groupable: true,
            sortable: true,
            pageable: {
                refresh: true,
                pageSizes: true,
                buttonCount: 5
            },
            columns: [
                {
                    field: "CUSTOMER_EDESC",
                    title: "Customer Name",
                    hidden: true,
                },
                {
                    field: "DIVISION_EDESC",
                    title: "Division Name",
                    groupFooterTemplate:"Total"
                },
                {
                    field: "CREDIT_LIMIT",
                    title: "Credit Limit",
                    aggregates: ["sum"],
                    attributes:
                    {
                        style: "text-align:right;"
                    },
                    groupFooterTemplate: "#= kendo.toString(sum, 'n')#",
                },
                {
                    field: "UTI",
                    title: "Utilization",
                    aggregates: ["sum"],
                    attributes:
                    {
                        style: "text-align:right;"
                    },
                    groupFooterTemplate: "#= kendo.toString(sum, 'n')#",
                }, 
                {
                    field: "BALANCE",
                    title: "Outstanding",
                    aggregates: ["sum"],
                    attributes:
                    {
                        style: "text-align:right;"
                    },
                    groupFooterTemplate: "#= kendo.toString(sum, 'n')#",
                },
                {
                    field: "REMARKS",
                    title: "Remarks",
                }]
        });
   



});