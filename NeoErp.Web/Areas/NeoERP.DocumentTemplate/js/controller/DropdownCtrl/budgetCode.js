DTModule.controller('budgetCtrl', function ($scope, $http, $routeParams, $window, $filter) {

    // Function to create data source with dynamic URL based on row index
    $scope.createBudgetDataSource = function (isFirstRow) {
        return {
            serverFiltering: true,
            transport: {
                read: {
                    // Use different API for first row vs other rows
                    url: isFirstRow ? "/api/TemplateApi/GetAllMasterBudgetCenterByFilter" : "/api/TemplateApi/GetAllBudgetCenterByFilter",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    type: "GET"
                },
                parameterMap: function (data, action) {
                    var newParams;
                    if (data.filter != undefined) {
                        if (data.filter.filters[0] != undefined) {
                            newParams = {
                                filter: data.filter.filters[0].value,
                                accCode: accCode
                            };
                            return newParams;
                        }
                        else {
                            newParams = {
                                filter: "",
                                accCode: accCode
                            };
                            return newParams;
                        }
                    }
                    else {
                        newParams = {
                            filter: "",
                            accCode: accCode
                        };
                        return newParams;
                    }
                }
            }
        };
    };

    // Default data source (for backward compatibility)
    $scope.budgetCenterDataSource = $scope.createBudgetDataSource(false);

    $scope.budgetCenterOption = {
        dataSource: $scope.budgetCenterDataSource, // Default data source
        dataTextField: "BUDGET_EDESC",
        dataValueField: "BUDGET_CODE",
        filter: "contains",
        suggest: true,
        autoBind: false, // Don't load data until user opens dropdown
        open: function (e) {
            // Set the correct data source when dropdown is opened (before data is loaded)
            var element = this.element;
            if (element && element[0] && element[0].attributes['budget-index']) {
                var budgetIndex = parseInt(element[0].attributes['budget-index'].value);

                // If it's the first row (index 0), use the first row API
                if (budgetIndex === 0) {
                    // Check if we haven't already set the first row data source
                    if (!element.data('first-row-ds-set')) {
                        this.setDataSource($scope.createBudgetDataSource(true));
                        element.data('first-row-ds-set', true);
                    }
                }
            }
        },
        select: function (e) {
            var selectedBudgetCode = e.dataItem.BUDGET_CODE;
            var budgetKey = this.element[0].attributes['budget-key'].value;
            var budgetIndex = this.element[0].attributes['budget-index'].value;

            // Get parent scope (assuming this is called from within budget modal)
            var parentScope = angular.element(this.element).scope();

            if (parentScope && parentScope.dynamicModalData && parentScope.dynamicModalData[budgetKey]) {
                var budgetList = parentScope.dynamicModalData[budgetKey].BUDGET;

                // Check if this budget center is already selected in another row
                var isDuplicate = false;
                for (var i = 0; i < budgetList.length; i++) {
                    if (i != budgetIndex && budgetList[i].BUDGET_CODE === selectedBudgetCode) {
                        isDuplicate = true;
                        break;
                    }
                }

                if (isDuplicate) {
                    parentScope.budgetCodeValidation = "Budget Center '" + selectedBudgetCode + "' is already selected. Please choose a different one.";
                    parentScope.budgetcount = true;
                    // Clear the selection
                    budgetList[budgetIndex].BUDGET_CODE = "";
                    this.value("");
                    parentScope.$apply();
                    return false;
                } else {
                    parentScope.budgetCodeValidation = "";
                    parentScope.budgetcount = false;
                    budgetList[budgetIndex].BUDGET_CODE = selectedBudgetCode;
                    parentScope.$apply();
                }
            }
        }
    };
});

