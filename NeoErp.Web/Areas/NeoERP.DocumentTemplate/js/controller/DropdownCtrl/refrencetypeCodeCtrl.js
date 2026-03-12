
DTModule.controller('refrenceCodeCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter, formtemplateservice) {

    var a = document.docname;
    

    $scope.refrenceCodeDataSource = {
        serverFiltering: true,
        transport: {
            read: {
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                type: "GET",
                url: "/api/TemplateApi/GetAllOrederNoByFlter"
            },
            parameterMap: function (data, action) {
                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        newParams = {
                            FormCode: $scope.FormCode,
                            filter: data.filter.filters[0].value,
                            Table_name: $rootScope.RefTableName,
                        };
                        return newParams;
                    }
                    else {
                        newParams = {
                            FormCode: $scope.FormCode,
                            filter: "",
                            Table_name: $scope.RefTableName,
                        };
                        return newParams;
                    }
                }
                else {
                    newParams = {

                        FormCode: $scope.FormCode,
                        filter: "",
                        Table_name: $scope.RefTableName,
                    };
                    return newParams;
                }
            }
        },
    };
    $scope.refrenceCodeOption = {
        dataSource: $scope.refrenceCodeDataSource,
        //template: '<span>{{dataItem.ORDER_EDESC}}</span>  --- ' +
        //'<span>{{dataItem.Type}}</span>',
        dataTextField: 'ORDER_EDESC',
        dataValueField: 'ORDER_CODE',
        filter: 'contains',
        select: function (e) {
            
            if (e.dataItem == "undefined" || e.dataItem == "" || e.dataItem == null) {
                return;
            }
            else {
                
                var orderNo = e.dataItem.ORDER_CODE;
                var defered = $.Deferred();
                var saleOrderformDetail = formtemplateservice.getSalesOrderDetail_ByFormCodeAndOrderNo($scope.FormCode, orderNo, defered);
                $.when(defered).done(function (result) {
                    var response = [];
                    response = result;
                    $scope.refrenceFn(response);

                });
            }
        },
       
      
    }
    $scope.refrenceCodeOnChange = function (kendoEvent) {
        
        if (kendoEvent.sender.dataItem() == undefined) {
            $scope.refrenceerror = "Please Enter Valid Code."
            $('#refrencetype').data("kendoComboBox").value([]);
            $(kendoEvent.sender.element[0]).addClass('borderRed');
        }
        else {
            $scope.refrenceerror = "";
            $(kendoEvent.sender.element[0]).removeClass('borderRed');
        }
    };

});

