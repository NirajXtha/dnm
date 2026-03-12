/// <reference path="~/Scripts/_references.js" />
/// <reference path="../module/planningModule.js" />

planningModule.controller('subPlanCtrl', function ($scope, plansubplanservice) {
    $scope.pageName = "Sub Plan Index";
    $scope.planCode = '85';
    $scope.columnNumber = 0;
    $scope.columns = [];
    $scope.subPlanItemTotal = [];
    $scope.formElements = {};
    $scope.week = {};
    $scope.ChangedValue = function (e,item_code) {
    }
    //update_wizard(2);
    var singlecol = [{ field: "ITEM_EDESC", title: "Item Name", width: "150px", locked: true },
                    {
                        field: "ITEM_CODE", title: "ITEM_CODE", width: "150px", locked: true,
                        template: "<input class='autoFillNum_' name='autoFillNum_{{dataItem.ITEM_CODE}}' dir='rtl' ng-change='ChangedValue(this)' ng-model='autoFillNum' style='width:70px !important' type='number' />",
                    }, ];
    function columnStructure(num) {
        for (var i = 1; i < 5 ; i++) {
            var weekname = "week_" + i;
            singlecol.push({ field: "week_"+i, title: "Week "+i, width: "150px", template: "<input ng-model='"+weekname+"' name='week_{{dataItem.ITEM_CODE}}' ng-change='ChangedValue(this,{{dataItem.ITEM_CODE}})'  type='text' style='width:100%' />" });
        }
        $scope.columns = singlecol;
        return singlecol;
    }
    //var getcolumnNumber = plansubplanservice.getFrequencyForCoulum($scope.planCode).then(function (result) {
    //    $scope.columnNumber = result.data;
    //    columnStructure($scope.columnNumber);
    //    getTreeList();
    //}, function (errorStatus) {
    //    // error on httpget request.
    //});
   
    var id = 10;
    //kendo treelist
    function getTreeList() {
        var mainurl = window.location.protocol + "//" + window.location.host + "/api/PlanSetupApi/getItemByCode";
        var viewdetail = {
            PLAN_CODE: '85',
            SUBPLAN_EDESC: '',
            TIME_FRAME_CODE: '',
            TIME_FRAME_EDESC: '',
        }

        $scope.treelistOptions = {
            dataSource: {
                transport: {
                    read: {
                        url: mainurl,
                        type: 'GET',
                        data: function (data, evt) {
                            return {
                                itemCode: viewdetail.PLAN_CODE,
                                prodId: '00',
                                level: '1',
                                masterCode: '00'
                            }
                        }
                    },
                },
                schema: {
                    model: {
                        id: "MASTER_ITEM_CODE",
                        parentId: "PRE_ITEM_CODE",
                        fields: {
                            ITEM_CODE: { field: "ITEM_CODE", type: "string" },
                            ITEM_EDESC: { field: "ITEM_EDESC", type: "string" },
                            PRE_ITEM_CODE: { field: "PRE_ITEM_CODE", type: "string", defaultValue: '00' },
                        },
                    }
                }
            },
            sortable: true,
            editable: true,
            columns: singlecol,
            dataBound: function (e) {
                e.preventDefault();
            }
           
        };
    }
    // submit
    $scope.SaveSubPlan = function (form) {
        
    }

    $scope.Cancel = function () {
        window.location.href = "/Planning/Home/Setup#!Planning/SubplanList";
    }
});
