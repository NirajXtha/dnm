DTModule.controller('customerCtrl', function ($scope, $http, $routeParams, $window, $filter) {
    $scope.columnwiseSearch = true;
    $scope.customerDataSource = {
        type: "json",
        serverFiltering: true,
        //suggest: true,
        //highlightFirst: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetAllCustomerSetupByFilter",
                success: function (data) {
                    $scope.columnwiseSearch = true;
                    alert(data);
                },
                error: function (xhr, error) {
                    console.debug(xhr); console.debug(error);
                    $scope.columnwiseSearch = true;
                    alert('error');
                }
            },
            parameterMap: function (data, action) {
                //
                var newParams;
                var checkvalueColumnSearch = true;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        if (data.filter.filters[0].value.match("^#")) {
                            if (data.filter.filters[0].value.length <= 1)
                                return false;
                            if (data.filter.filters[0].value.match("#$")) {
                                $scope.columnwiseSearch = false;
                            }
                            if ($scope.columnwiseSearch == false) {
                                newParams = {
                                    filter: data.filter.filters[0].value
                                };
                                return newParams;
                            }
                            return false;
                            
                        }
                        newParams = {
                            filter: data.filter.filters[0].value
                        };
                        return newParams;
                    }
                    else {
                        return false;
                    }
                }
                else {
                    return false;
                }
            }
        },
          group: { field: "Type" }
    };
    //$scope.customerCodeOnChange = function (kendoEvent) {
    //    
    //    if (kendoEvent.sender.dataItem() == undefined) {
    //        $scope.customererror = "Please Enter Valid Code."
    //        $('#customers').data("kendoComboBox").value([]);
    //    }
    //    else {
    //        $scope.customererror = "";
    //    }
    //}
    $scope.custoCodeOnChange = function (kendoEvent) {
        console.log(kendoEvent);
       // 
        //if (kendoEvent.sender.dataItem() == undefined) {
        //    $scope.custoererror = "Please Enter Valid Code."
        //    $('#customer').data("kendoComboBox").value([]);
        //}
        //else {
        //    $scope.custoererror = "";
        //}
    }
});