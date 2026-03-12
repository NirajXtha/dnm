DTModule.controller('DocumentReferenceCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter) {
    //document reference 
    $scope.Name = "sd";
    $scope.DocumentReference = [];
    $scope.DocumentReference = [
        {
            DOCUMENT: "",
            TEMPLATE: "",
            FROM_DATE:"",
            TO_DATE:"",
            NAME: "",
            ITEM_DESC: "",
            VOUCHER_NO: ""
        }];

    //Row
    $scope.ROW = {
        radio: 'nonrefrence'
    };
    $scope.ROW = {
        radio: 'incomplete'
    };
    $scope.ROW = {
        radio: 'all'
    };

    //REFERENCE_QUALITY
    $scope.REFERENCE_QUALITY = {
        radio: 'nonrefrence'
    };
    $scope.REFERENCE_QUALITY = {
        radio: 'incomplete'
    };
        //document reference end
    
});

