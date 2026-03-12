/* eslint-disable */
app.controller("mvcCRUDCtrl", function ($scope, crudAJService) {
    //var getData = crudAJService.getInstalledPlugInData()
    //getData.then(function (response) {
    //    debugger;
    //    $scope.installedPlugInList = response.data;
    //});

    //var getData = crudAJService.GetDistributionMneu()
    //getData.then(function (response) {
    //    $scope.DistributionMenus = response.data[0].Items[3].MENU_EDESC;
    //    $scope.VIRTUAL_PATH = response.data[0].Items[3].VIRTUAL_PATH;
    //    $scope.MODULE_ABBR = response.data[0].Items[3].MODULE_ABBR;
    //    $scope.COLOR = response.data[0].Items[3].COLOR;
    //    $scope.DESCRIPTION = response.data[0].Items[3].DESCRIPTION;
    //});

    $scope.init = function () {
      //  alert($scope.mytest)
        var Module_Code = $scope.mytest;        
        var Getdata = crudAJService.getGlobalDashboardMenu(Module_Code)
        Getdata.then(function (response) {
            
            if (response.data == undefined  && response.data.length == 0) {
                false
            } else if ($scope.dynamicMenu == undefined) {
                $scope.dynamicMenu =response.data;
            }
            else {
                $.each(response.data, function (i, obj) {
                    $scope.dynamicMenu.unshift(obj);
                });
               
            }
            
           
          //  $scope.dynamicMenu.push({ Module_Code: Module_Code })
            //$timeout(function () {
            //    DataTableGrid();

        });
    };
    $scope.modelABBRColor = function (MODULE_ABBR) {
        if (MODULE_ABBR == 'DB')
            return "#43a12e";
        else if (MODULE_ABBR == 'AC')
            return "#3c763d";
        else if (MODULE_ABBR == 'AR')
            return "#4480a4";
        else if (MODULE_ABBR == 'PR')
            return "#666";
        else if (MODULE_ABBR == 'ST')
            return "#31708f";
        else if (MODULE_ABBR == 'FA')
            return "#5aa9d7";
        else if (MODULE_ABBR == 'SM')
            return "#45b6b6";
        else if (MODULE_ABBR == 'NA')
            return "#45b6b6";
        else
            return "#b6a845";
    }

    $scope.menuColor = function (MODULE_ABBR) {
        
        if(MODULE_ABBR=='DB')
        {
            return "#5aa9d7";
        }
        
        else if (MODULE_ABBR == 'SM') {
            return "#4480a4";
        }
        else if (MODULE_ABBR == 'AC') {
            return "#3c763d";
        }
        
        else if(MODULE_ABBR=='AR')
        {
            return "#666";
        }
        else if (MODULE_ABBR == 'PR') {
            return "#31708f";
        }
        else if (MODULE_ABBR == 'ST') {
            return "#45b6b6";
        }

        else if (MODULE_ABBR == 'NA') {
            return "#b6a845";
        }
        else {
            return "45b6b8";
        }
    }



});
