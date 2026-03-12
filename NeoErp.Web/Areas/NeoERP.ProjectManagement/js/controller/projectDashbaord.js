PMModule.controller('projectDashbaord', function ($scope,  $rootScope, $http, $filter, $timeout) {

            $scope.projectCountList = [];
            $scope.dataforBarChart = [];
            $scope.completedProjects = [];
            $scope.startedProjects = [];

            $http.get("/Api/ProjectApi/GetProjectCount").then(function (results) {
                $scope.projectCountList = results.data;
            });

            $http.get("/api/ProjectApi/CountBarGraph").then(function (results) {
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
                            label: 'Project Details by Percentage',
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
                                    text: 'Project Details'
                                }
                            },
                            y: {
                                beginAtZero: true,
                                title: {
                                    display: true,
                                    text: 'Percentage'
                                }
                            }
                        }
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
                            label: 'Project Count',
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

            $scope.tab = 'started';

            $http.get("/api/ProjectApi/GetCompletedProject").then(function (results) {
                results.data.forEach(function (item) {
                    item['END_DT'] = formatDate(item['END_DT']);
                });
                $scope.completedProjects = results.data;
            });

            $http.get("/api/ProjectApi/GetStartedProject").then(function (results) {
                results.data.forEach(function (item) {
                    item['START_DT'] = formatDate(item['START_DT']);
                })
                $scope.startedProjects = results.data;
            });

            $scope.setTab = function (tabName, event) {
                event.preventDefault();
                $scope.tab = tabName;
            };

            function formatDate(dateString) {
                var date = new Date(dateString);
                return $filter('date')(date, 'dd-MMM-yyyy');
            }
        });
