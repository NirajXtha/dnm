PMModule.controller('currencyCtrl', function ($scope, $http, $routeParams, $window, $filter) {

    $scope.currencyDataSource = {

        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetCurrencyListByFlter",

            },
            parameterMap: function (data, action) {
                var newParams;
               
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        newParams = {
                            filter: data.filter.filters[0].value
                        };
                        return newParams;
                    }
                    else {
                        newParams = {
                            filter: ""
                        };
                        return newParams;
                    }
                }
                else {
                    newParams = {
                        filter: ""
                    };
                    return newParams;
                }
            }
        }
    };
    
    $scope.CurrencyCodeOnChange = function (kendoEvent) {
        if (kendoEvent.sender.dataItem() == undefined) {
            $scope.currencyrerror = "Please Enter Valid Code."
            $('#currency').data("kendoComboBox").value([]);
            $(kendoEvent.sender.element[0]).addClass('borderRed');
        }
        else {
            var previousCurrency = previous;
            var previousExchangeRate = previousExchange;
            $scope.currencyrerror = "";
            if (kendoEvent.sender.dataItem().EXCHANGE_RATE == null) {
                $scope.masterModels["EXCHANGE_RATE"] = 1;
            }
            else {
                $scope.masterModels["EXCHANGE_RATE"] = kendoEvent.sender.dataItem().EXCHANGE_RATE;

                // old

                //var total_price = 0;
                //var amount = 0;
                //var calcUnitPrice = 0;
                //var calcTotalPrice = 0;
                //var unitPrice = 0;
                //var drTotal = 0;
                //var crTotal = 0;
                //var diffAmount = 0;
                //var selectedCurrency = $("#currency").data("kendoDropDownList").value();
                //var selectedCurrencyq = $("#currency").data("kendoDropDownList")._old;
                //$scope.masterModels["EXCHANGE_RATE"] = kendoEvent.sender.dataItem().EXCHANGE_RATE;
                //exchangeRate = $scope.masterModels["EXCHANGE_RATE"];

                //drTotal = $scope.accsummary.drTotal !== undefined || null ? $scope.accsummary.drTotal : 0;
                //crTotal = $scope.accsummary.crTotal !== undefined || null ? $scope.accsummary.crTotal : 0;
                //diffAmount = $scope.accsummary.diffAmount !== undefined || null ? $scope.accsummary.diffAmount : 0;

                //angular.isDefined($scope.accsummary.drTotal)

                //function calCulation() {
                //    angular.forEach($scope.childModels, function (value, index) {
                       
                //        if ($scope.childModels[index].hasOwnProperty("AMOUNT" && " PARTICULARS" && "BUDGET_FLAG" && "TRANSACTION_TYPE")) {
                           

                //            amount = ($scope.childModels[index].AMOUNT * previousExchangeRate) / exchangeRate;
                //            $scope.childModels[index].AMOUNT = Math.round(parseFloat(amount.toFixed(2)) * 100) / 100;
                //            $scope.accsummary.drTotal = Math.round(parseFloat((drTotal * previousExchangeRate / exchangeRate).toFixed(2)) * 100) / 100;
                //            $scope.accsummary.crTotal = Math.round(parseFloat((crTotal * previousExchangeRate / exchangeRate).toFixed(2)) * 100) / 100;
                //            $scope.accsummary.diffAmount = Math.round(parseFloat((diffAmount * previousExchangeRate / exchangeRate).toFixed(2)) * 100) / 100;
                //        }
                //        if ($scope.childModels[0].hasOwnProperty("CALC_UNIT_PRICE" && " CALC_TOTAL_PRICE" && "UNIT_PRICE" && "TOTAL_PRICE")) {
                //            if ($scope.childModels[0].UNIT_PRICE === undefined) {
                                
                //                calcUnitPrice = ($scope.childModels[index].CALC_UNIT_PRICE * previousExchangeRate) / exchangeRate;
                //                $scope.childModels[index].CALC_UNIT_PRICE = Math.round(parseFloat(calcUnitPrice.toFixed(2)) * 100) / 100;

                //                calcTotalPrice = ($scope.childModels[index].CALC_TOTAL_PRICE * previousExchangeRate) / exchangeRate;
                //                $scope.childModels[index].CALC_TOTAL_PRICE = Math.round(parseFloat(calcTotalPrice.toFixed(2)) * 100) / 100;


                //                total_price += $scope.childModels[index].CALC_TOTAL_PRICE;
                //            }
                //            else {
                                
                //                amount = ($scope.childModels[index].TOTAL_PRICE * previousExchangeRate) / exchangeRate;
                //                $scope.childModels[index].TOTAL_PRICE = Math.round(parseFloat(amount.toFixed(2)) * 100) / 100;

                //                calcUnitPrice = ($scope.childModels[index].CALC_UNIT_PRICE * previousExchangeRate) / exchangeRate;
                //                $scope.childModels[index].CALC_UNIT_PRICE = Math.round(parseFloat(calcUnitPrice.toFixed(2)) * 100) / 100;

                //                calcTotalPrice = ($scope.childModels[index].CALC_TOTAL_PRICE * previousExchangeRate) / exchangeRate;
                //                $scope.childModels[index].CALC_TOTAL_PRICE = Math.round(parseFloat(calcTotalPrice.toFixed(2)) * 100) / 100;

                //                unitPrice = ($scope.childModels[index].UNIT_PRICE * previousExchangeRate) / exchangeRate;
                //                $scope.childModels[index].UNIT_PRICE = Math.round(parseFloat(unitPrice.toFixed(2)) * 100) / 100;

                //                total_price += $scope.childModels[index].TOTAL_PRICE;
                //            }
                //        }



                //    });
                    
                //    if (drTotal === null && crTotal === null && diffAmount === null) {
                //        $scope.summary.grandTotal = Math.round(parseFloat(total_price.toFixed(2)) * 100) / 100;
                //        $scope.GrandtotalCalution();
                //    }

                //};
                //if (previousCurrency === "NRS") {
                //    calCulation();
                //}
                //if (previousCurrency === "INR") {
                //    calCulation();
                //}
                //if (previousCurrency === "USD") {

                //    angular.forEach($scope.childModels, function (value, index) {

                //        if ($scope.childModels[index].hasOwnProperty("AMOUNT" && " PARTICULARS" && "BUDGET_FLAG" && "TRANSACTION_TYPE")) {                        
                //            amount = ($scope.childModels[index].AMOUNT * previousExchangeRate) / exchangeRate;
                //            $scope.childModels[index].AMOUNT = Math.round(parseFloat(amount.toFixed(2)));
                //            $scope.accsummary.drTotal = Math.round(parseFloat(((drTotal * previousExchangeRate) / exchangeRate).toFixed(2)));
                //            $scope.accsummary.crTotal = Math.round(parseFloat(((crTotal * previousExchangeRate) / exchangeRate).toFixed(2)));
                //            $scope.accsummary.diffAmount = Math.round(parseFloat(((diffAmount * previousExchangeRate) / exchangeRate).toFixed(2)));
                //        }
                //        if ($scope.childModels[0].hasOwnProperty("CALC_UNIT_PRICE" && " CALC_TOTAL_PRICE" && "UNIT_PRICE" && "TOTAL_PRICE")) {

                //            if ($scope.childModels[0].UNIT_PRICE === undefined) {
                //                calcUnitPrice = ($scope.childModels[index].CALC_UNIT_PRICE * previousExchangeRate) / exchangeRate;
                //                $scope.childModels[index].CALC_UNIT_PRICE = Math.round(parseFloat(calcUnitPrice.toFixed(2)));

                //                calcTotalPrice = ($scope.childModels[index].CALC_TOTAL_PRICE * previousExchangeRate) / exchangeRate;
                //                $scope.childModels[index].CALC_TOTAL_PRICE = Math.round(parseFloat(calcTotalPrice.toFixed(2)));

                //                total_price += $scope.childModels[index].CALC_TOTAL_PRICE;
                //            }
                //            else {
                               
                //                amount = ($scope.childModels[index].TOTAL_PRICE * previousExchangeRate) / exchangeRate;
                //                $scope.childModels[index].TOTAL_PRICE = Math.round(parseFloat(amount.toFixed(2)));
                //                calcUnitPrice = ($scope.childModels[index].CALC_UNIT_PRICE * previousExchangeRate) / exchangeRate;
                //                $scope.childModels[index].CALC_UNIT_PRICE = Math.round(parseFloat(calcUnitPrice.toFixed(2)));

                //                calcTotalPrice = ($scope.childModels[index].CALC_TOTAL_PRICE * previousExchangeRate) / exchangeRate;
                //                $scope.childModels[index].CALC_TOTAL_PRICE = Math.round(parseFloat(calcTotalPrice.toFixed(2)));
                //                unitPrice = ($scope.childModels[index].UNIT_PRICE * previousExchangeRate) / exchangeRate;

                //                $scope.childModels[index].UNIT_PRICE = Math.round(parseFloat(unitPrice.toFixed(2)));
                //                total_price += $scope.childModels[index].TOTAL_PRICE;
                //            }
                //        }


                //    });
                //    if (drTotal === null && crTotal === null && diffAmount === null) {
                //        $scope.summary.grandTotal = Math.round(parseFloat(total_price.toFixed(2)) * 100) / 100;
                //        $scope.GrandtotalCalution();
                //    }
                //}
                //if (previousCurrency === "EUR") {
                //    calCulation();
                //}
            }
            $(kendoEvent.sender.element[0]).removeClass('borderRed');
        }
    };
    $scope.CurrencyCodeOnSelect = function (kendoEvent) {         
        previous = $("#currency").val();
        previousExchange = $scope.masterModels["EXCHANGE_RATE"];
    };
});
