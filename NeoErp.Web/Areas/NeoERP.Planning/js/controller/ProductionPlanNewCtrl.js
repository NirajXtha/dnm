/*eslint-disable*/
planningModule.controller('ProductionPlanNewCtrl', function ($scope, $routeParams, $rootScope, $http) {
    debugger;
    $scope.pageName = "Production Plan New";
    checkedItems = [];
    $scope.firstView = "Y";
    $scope.secondView = "N";
    $scope.thirdView = "N";
    checkedItemsDist = [];
    $scope.checkedItemsorgi = [];
    $scope.checkedItemsDistReset = [];
    $scope.checkedItemsDisplay = [{
        order_no: "",
        item_code: "",
        ietm_edesc:"",
        customer_code: "",
        qty: 0,
        Pqty: "",
       
    }];
    $scope.viewSummeryQty = function () {
        $scope.secondView = "Y";
        debugger;
        //var multiSelectVal = $("#customerList").data("kendoMultiSelect").value();

        pCode = $.grep($("#customerList").data("kendoMultiSelect").value(), function (v) { return (v) }).toString();
        var startDate = $("#FromDateVoucher").val();
        var endDate = $("#ToDateVoucher").val();
        $scope.bindproddetailsGrid(pCode, startDate, endDate);
    }
    $scope.bindproddetailsGrid = function (customerlist, startdate, enddate) {
        var proddetailsUrl = window.location.protocol + "//" + window.location.host + "/api/ProductionPlanApi/GetPorductionDetails?startDate=" + startdate + "&endDate=" + enddate + "&customers=" + pCode;
        $scope.productDetailsGridOptions = {

            dataSource: {
                type: "json",
                transport: {
                    read: proddetailsUrl,
                },
                pageSize: 50,
                //serverPaging: true,
                serverSorting: true
            },
            scrollable: true,
            height: 450,
            sortable: true,
            pageable: true,
            dataBound: function (e) {
               
                var griddata = $("#kGrid").data("kendoGrid");
                var dataItems = griddata.dataSource._pristineData;
                //$('#header-chb').on("change", selectallRow);
                $(".row-checkbox").on("click", selectRow);
                debugger;
               

            },
            columns: [
                {
                    //title: 'Select All',
                    //headerTemplate: "<input type='checkbox' id='header-chb' class='checkbox header-checkbox'><label class='k-checkbox-label' for='header-chb'></label>",
                    template: function (dataItem) {
                        return "<input type='checkbox' id='row-checkbox_" + dataItem.customer_code + "' class='row-checkbox'><label class='k-checkbox-label' for='${dataItem.customer_code}'></label>"
                    },
                    width: 50
                },
                {
                    field: "order_no",
                    title: "Order No",
                    width: 150
                },

                {
                    field: "item_edesc",
                    title: "Item",
                    width: 150
                },
                {
                    field: "customer_edesc",
                    title: "Customer",
                    width: 150
                },
                {
                    field: "qty",
                    title: "Quantity",
                    width: 150
                },
               
                //{
                //    template: '<a class="print glyphicon glyphicon-print" title="Click to Print" ng-dblclick="printdata()"  style="color:grey;"><span class="sr-only"></span> </a>',

                //    title: "Print",
                //    width: "40px"
                //}
                //{
                //    command: [
                //        {
                //            "name": "print",
                //            "buttonType": "Image",
                //            "text": " ",
                //            "title": "Click to Print",
                //            "imageClass": "fa-print",
                //            "iconClass": "fa",
                //            "click": function (e) {
                //                debugger;
                //                var btn = $(e.currentTarget).closest("tr").find("a.k-icon.k-i-expand")[0];
                //                $(btn).trigger('click');

                //                var data = this.dataItem($(e.currentTarget).closest("tr"));
                //                var detailGrid = $(e.currentTarget).closest('tr').next('tr').find('.orders').data("kendoGrid");
                //                var items = detailGrid.dataSource.data();
                //                data.DetailList = items;
                //                e.preventDefault();
                //                var template = kendo.template($("#InterestCalcPrintTemplate").html());
                //                var result = template(data);
                //                $("#SalesPrintFormBody").html(result);
                //                $("#SalesPrintWindow").modal('show');
                //            },

                //            visible: function (dataItem) {
                //                return true;
                //                //if (dataItem.APPROVED_FLAG == "A") return true;
                //                //else return false;
                //            }
                //        }
                //    ]
                //},
            ]


        };
        function selectallRow() {
            debugger;

            var checked = this.checked;
            if (checked) {
                checkedItems = [];
                var grid = $("#kGrid").data("kendoGrid");
                var dataItem = datas = grid.dataSource._pristineData;
                for (var i = 0; i < dataItem.length; i++) {
                    debugger;
                    checkedItems.push({
                        "order_no": dataItem[i].order_no,
                        "item_code": dataItem[i].item_code,
                        "customer_code": dataItem[i].item_code,
                        "qty": dataItem[i].qty
                        
                    });

                }
                $('.row-checkbox').attr('checked', true);
            }
            else {
                checkedItems = [];
                $('.row-checkbox').attr('checked', false);
            }

        }

        //function selectRow() {
        //    debugger;
        //    var checked = this.checked;
        //    row = $(this).closest("tr"),
        //        grid = $("#kGrid").data("kendoGrid"),
        //        dataItem = grid.dataItem(row);
        //    if (checked) {
        //        checkedItems.push({
        //            "order_no": dataItem[i].order_no,
        //            "item_code": dataItem[i].CREDIT_DAYS,
        //            "customer_code": dataItem[i].item_code,
        //            "qty": dataItem[i].qty
        //        });
        //    }
        //    else {
        //        for (var i = 0; i < checkedItems.length; i++) {
        //            if (checkedItems[i].CUSTOMER_CODE == dataItem.CUSTOMER_CODE) {
        //                checkedItems.splice(i, 1);
        //            }
        //        }
        //        row.removeClass("k-state-selected");
        //    }
        //}
        function selectRow() {
        
            debugger;
            var checked = this.checked;
            row = $(this).closest("tr"),
                grid = $("#kGrid").data("kendoGrid"),
                dataItem = grid.dataItem(row);
            if (checked) {
                checkedItems.push({
                    "order_no": dataItem.order_no,
                    "item_code": dataItem.item_code,
                    "item_edesc": dataItem.item_edesc,
                    "customer_code": dataItem.item_code,
                    "qty": dataItem.qty
                });
            }
            else {
                for (var i = 0; i < checkedItems.length; i++) {
                    if (checkedItems[i].CUSTOMER_CODE == dataItem.CUSTOMER_CODE) {
                        checkedItems.splice(i, 1);
                    }
                }
                row.removeClass("k-state-selected");
            }
            //if (checkedItems.length > 0) {
              
            //}
           
        }
        
    }

    $scope.viewDistinctQty = function () {
        debugger;
      
        $scope.checkedItemsorgi = [];
        for (var i = 0; i < checkedItems.length; i++) {
           
            $scope.checkedItemsorgi.push({
                "order_no": checkedItems[i].order_no,
                "item_code": checkedItems[i].item_code,
                "item_edesc": checkedItems[i].item_edesc,
                "customer_code": checkedItems[i].customer_code,
                "qty": checkedItems[i].qty
            });

        };


        $scope.checkedItemsDist = $scope.checkedItemsorgi;

        var unique = [];
        var tempArr = [];
        $scope.checkedItemsDist.forEach((value, index) => {
            if (unique.indexOf(value.item_code) === -1) {
                unique.push(value.item_code);
            } else {
                tempArr.push(index);
            }
        });
        tempArr.reverse();
        tempArr.forEach(ele => {
            $scope.checkedItemsDist.splice(ele, 1);
        });
       
        $scope.calculateQtyDist(checkedItems);
    }
    $scope.calculateQtyDist = function (checkedItems) {
        $scope.checkedItemsDistReset = [];
       
        for (var i = 0; i < $scope.checkedItemsDist.length; i++) {
            
            $scope.checkedItemsDistReset.push({
                "order_no": $scope.checkedItemsDist[i].order_no,
                "item_code": $scope.checkedItemsDist[i].item_code,
                "item_edesc": $scope.checkedItemsDist[i].item_edesc,
                "customer_code": $scope.checkedItemsDist[i].customer_code,
                "qty": 0,
                "Pqty": 0

            });

        };

        $.each($scope.checkedItemsDistReset, function (it, val) {
            debugger;
            $.each(checkedItems, function (i, v) {
                debugger;
                if (val.item_code === v.item_code) {
                    val.qty = val.qty + v.qty;
                    $scope.checkedItemsDistReset[it].qty = val.qty;
                }
                //else {
                //    val.qty = v.qty;
                //    $scope.checkedItemsDistReset[it].qty = val.qty;
                //}
            });
        });
    }

    $scope.viewStock = function () {
        debugger;
        var s = $scope.checkedItemsDistReset;
    }
});