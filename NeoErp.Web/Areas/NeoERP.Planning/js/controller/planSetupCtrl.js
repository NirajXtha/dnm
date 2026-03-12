/// <reference path="~/Scripts/_references.js" />
/// <reference path="../module/planningModule.js" />

planningModule.controller('planSetupCtrl', function ($scope, plansetupservice, $routeParams, $rootScope) {
    $rootScope.reservedData;
    
    $scope.p_code = $routeParams.plancode;
    $scope.param = $rootScope.reservedData;

    $scope.pageName = "Planning setup Index";
    $scope.treelistOptions = [];
    $scope.Frequency = [];
    $scope.ITEM = [];
    $scope.Plan = [];
    $scope.startDate = [];
    $scope.endDate = [];

    $scope.gotoBack = function () {
        
        $rootScope.reservedData;
        if (confirm("All input data will lost, Are you sure to go back?")) {
            if ($routeParams.plancode != '' && $routeParams.plancode != undefined && $routeParams.plancode!='undefined') {
                window.location = "/Planning/Home/Index#!Planning/CreatePlan/"+$routeParams.plancode;
            }
            else {
                window.location = "/Planning/Home/Index#!Planning/CreatePlan";
            }
        }
    }

   

    //$scope.DynamicCol = [];
    //update_wizard(1);
    //var getItemByCodeUrl = window.location.protocol + "//" + window.location.host + "/api/PlanSetupApi/getItemByCode";

    //$scope.treelistOptions = {
    //    dataSource: {
    //        transport: {
    //            read: {
    //                url: getItemByCodeUrl,
    //                type: 'GET',
    //                data: function (data, evt) {
    //                    var treeList = $("#treelist").data("kendoTreeList");
    //                    var dataSource = treeList.dataSource;
    //                    if (data.id) {
    //                        var parentNode = dataSource.get(data.id);
    //                        return {
    //                            itemCode: code,
    //                            prodId: parentNode.ITEM_CODE,
    //                            level: parentNode.Level,
    //                            masterCode: parentNode.MASTER_ITEM_CODE
    //                        }
    //                    }
    //                    else {
    //                        return {
    //                            itemCode: code,
    //                            prodId: '00',
    //                            level: '1',
    //                            masterCode: '00'
    //                        }
    //                    }
    //                }
    //            },

    //        },

    //        schema: {
    //            model: {
    //                id: "MASTER_ITEM_CODE",
    //                parentId: "PRE_ITEM_CODE",
    //                fields: {
    //                    ITEM_CODE: { field: "ITEM_CODE", type: "string" },
    //                    ITEM_EDESC: { field: "ITEM_EDESC", type: "string" },
    //                    PRE_ITEM_CODE: { field: "PRE_ITEM_CODE", type: "string", defaultValue: '00' },
    //                }
    //            }
    //        }
    //    },
    //    sortable: true,
    //    editable: true,
    //    columns: $scope.DynamicCol
    //};

    //function getTitleValue(planCode) {
    //    var getTitle = plansetupservice.getTitleValue(planCode);
    //    getTitle.then(function (data) {
    //        
    //        $scope.ITEM = data.data[0].ITEM_EDESC;
    //        $scope.Frequency = data.data[0].TIME_FRAME_EDESC;
    //        $scope.Plan = data.data[0].PLAN_TYPE;
    //        $scope.startDate = data.data[0].START_DATE;
    //        $scope.endDate = data.data[0].END_DATE;

    //        //$scope.planSetup.selectedItem = data.data[0].ITEM_CODE;
    //        //$scope.planSetup.selectedPlan = data.data[0].PLAN_TYPE;
    //        //$scope.planSetup.selectedFrequency = data.data[0].TIME_FRAME_EDESC;
    //    }, function (error) {
    //        
    //    });
    //};

    //$scope.planType = {
    //    type: "json",
    //    transport: {
    //        read: {
    //            url: "/api/PlanSetupApi/getPlanType"
    //        }
    //    },

    //};

    //$scope.frequencyType = {
    //    type: "json",
    //    transport: {
    //        read: {
    //            url: "/api/PlanSetupApi/getFrequencyType"
    //        }
    //    },
    //    //dataBound: function (e) {
    //    //    
    //    //    $scope.Frequency
    //    //}
    //};

    $scope.cancelForm = function () {
        window.location.href = "/Planning/Home/Setup#!Planning/";
    }
});
