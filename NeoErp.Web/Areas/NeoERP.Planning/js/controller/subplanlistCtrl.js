planningModule.controller('subplanlistCtrl', function ($scope) {
    
    $scope.planlistGridOptions = {
        
        dataSource: {
            type: "jsonp",
            transport: {
                read: window.location.protocol + "//" + window.location.host + "/api/PlanApi/GetPlanList"
            },
            schema: {
                model: {
                    fields: {
                        START_DATE: { type: "date" },
                        END_DATE: { type: "date" },
                    },
                },
            },
            pageSize: 10,
            serverPaging: true,
            serverSorting: true
        },
        sortable: true,
        pageable: true,
        dataBound: function () {
            var $this = this;
            //$.each($this.tbody.find("tr.k-master-row"), function (i, v) {
            //    $this.expandRow(v);
            //})
            
            $this.expandRow(this.tbody.find("tr.k-master-row").first());
            var thead = $($('table')[2]).find('thead').addClass('table-head-color');
        },
        columns: [
            //{
            //    field: "PLAN_CODE",
            //    title: "Plan code",
            //    width: "80px"
            //},
            {
                field: "PLAN_EDESC",
                title: "Plan Name",
            },
            {
                field: "START_DATE",
                title: 'Start Date',
                format: "{0:yyyy-MMM-dd}",
                width: "120px", 
            },
            {
                field: "END_DATE",
                title: "End Date",
                format: "{0:yyyy-MMM-dd}",
                width:"120px",
            }
        ]
    };

    $scope.subplanGridOptions = function (dataItem) {
        
        return {
            dataSource: {
                type: "jsonp",
                transport: {
                    read: window.location.protocol + "//" + window.location.host + "/api/SubPlanApi/ViewSubPlans?plancode=" + dataItem.PLAN_CODE
                },
                serverPaging: true,
                serverSorting: true,
                serverFiltering: true,
                pageSize: 5,
            },
            scrollable: false,
            columns: [
            {
                field: "PLAN_CODE",
                title: "Plan Code",
                width: "80px"
            },
            {
                field: "SUBPLAN_EDESC",
                title: "Sub-plan Name",
            }]
        };
    };
});