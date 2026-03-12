

DTModule.controller('ThirdPartyApiCtrl', function ($scope, $rootScope, $http,$filter) {
   

        $scope.hello = "laxman";
        $scope.OperaModalOpen = function () {
            $("#saveAndPrintModal").modal("toggle");

        };

    $scope.CallShymphonyApi = function () {
      

        //var ShymphonyApi = "/api/ShymphonyApi/Read";
        var ShymphonyApi = "/api/OperaApi/Read";
        $http.get(ShymphonyApi).then(function (res) {
            $("#saveAndPrintModal").modal("toggle");
           // $scope.masterModelDataFn();
        });


    }

});

