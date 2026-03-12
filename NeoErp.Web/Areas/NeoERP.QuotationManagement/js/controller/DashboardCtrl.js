QMModule.controller('DashboardCtrl', function ($scope, $rootScope, $http, $filter, $timeout) {

    $scope.projectCountList = [];
    $scope.dataforBarChart = [];
    $scope.completedProjects = [];
    $scope.startedProjects = [];

    $http.get("/Api/QuotationApi/GetQuotationCount").then(function (results) {
        $scope.quotationCountList = results.data;
        $scope.dataforBarChart = results.data;
        createBarChart();
        createPieChart();
    });

    function createBarChart() {
        const ctx = document.getElementById('myChart');
        var totalCount = $scope.dataforBarChart.reduce(function (total, item) {
            return total + item.COUNT;
        }, 0);

        const percentages = $scope.dataforBarChart.map(function (item) {
            return ((item.COUNT / totalCount) * 100).toFixed(1);
        });

        const labels = $scope.dataforBarChart.map(item => item.HEADING);
        const colors = $scope.dataforBarChart.map(item => item.COLOR);

        new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Quotation Details by Percentage',
                    data: percentages,
                    backgroundColor: colors,
                    borderWidth: 1
                }]
            },
            options: {
                scales: {
                    x: {
                        title: {
                            display: true,
                            text: 'Quotation Details',
                            font: {
                                weight: 'bold'
                            }
                        },
                        ticks: {
                            font: {
                                size: '10px'
                            }
                        }
                    },
                    y: {
                        beginAtZero: true,
                        title: {
                            display: true,
                            text: 'Percentage',
                            font: {
                                weight: 'bold'
                            }
                        },
                        ticks: {
                            font: {
                                size: '10px'
                            }
                        }
                    }
                },
                responsive: true,
                maintainAspectRatio: false
            }
        });

    }

    function createPieChart() {
        const ctx = document.getElementById('myPieChart');

        const labels = $scope.dataforBarChart.map(item => item.HEADING);
        const data = $scope.dataforBarChart.map(item => item.COUNT);
        const colors = $scope.dataforBarChart.map(item => item.COLOR);

        new Chart(ctx, {
            type: 'pie',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Quotation Count',
                    data: data,
                    backgroundColor: colors,
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false
            }
        });
    }

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