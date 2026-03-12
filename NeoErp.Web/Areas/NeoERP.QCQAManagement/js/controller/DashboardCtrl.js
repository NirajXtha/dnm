QCQAModule.controller('DashboardQCQACtrl', function ($scope, $rootScope, $http, $filter, $timeout) {

    $scope.projectCountList = [];
    $scope.dataforBarChart = [];
    $scope.completedProjects = [];
    $scope.startedProjects = [];
    $scope.formCode = 497;
    //$http.get("/Api/QCQAApi/QCQADetails").then(function (results) {
    //    $scope.quotationCountList = results.data;
    //    $scope.dataforBarChart = results.data;
    //});

    $scope.ShowIcon = function (searchBox) {
        search = searchBox.toUpperCase();
        var noresult = 0;
        if (search == "") {
            $('.SearchIcon > li').show();
            noresult = 1;

        } else {
            $('.SearchIcon > li').each(function (index) {
                var text = $($('.SearchIcon > li > a > h6')[index]).text().toUpperCase();;
                var match = text.indexOf(search);
                if (match >= 0) {
                    $(this).show();
                    noresult = 1;
                } else {
                    $(this).hide();
                }
            });
        };
    };
});