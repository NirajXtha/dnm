angular.module('distributionModule')
    .controller('Schemectrl', function ($scope, $http, distributorService, $routeParams) {

    $scope.fetchUsers = function () {
        // debugger;

        var url = '';
        url = window.location.protocol + "//" + window.location.host + "/api/mobileDistribution/SchemeApi/";
        $http({
            method: 'POST',
            url: url,
            data: {
                "action": "getUsers",
                "data": {
                    "company_code": "01",
                    "branch_code": "01.01"
                }
            }
        }).then(
            function successCallback(response) {
                // debugger;
                console.log(response.data.result);
                if (response.data.response) {
                    console.log("Success response")
                }
                $scope.initKendoGrid(response.data.result.Data)
            },
            function errorCallback(response) {
                // debugger;
                console.error(response);
            }
        );

        };


        $scope.initKendoGrid = function (data) {
            debugger;
            $("#grid").kendoGrid({
                dataSource: {
                    data: data,
                    pageSize: 20
                },
                //height: 550,
                scrollable: true,
                sortable: true,
                filterable: true,
                pageable: {
                    refresh: true,
                    pageSizes: true,
                    buttonCount: 5
                },
                columns: [
                    { field: "Id", title: "ID", width: "70px" },
                    {
                        field: "Profile_Picture",
                        title: "Profile Picture",
                        width: "100px",
                        template: function (dataItem) {
                            if (dataItem.Profile_Picture) {
                                return `<img src="${window.location.protocol}//${window.location.host}/${dataItem.Profile_Picture}" style="width:60px;border-radius:50%;" />`;

                            } else {
                                return "--";
                            }
                        }
                    },
                    { field: "First_Name", title: "First Name", width: "120px" },
                    { field: "Middle_Name", title: "Middle Name", width: "120px" },
                    { field: "Last_Name", title: "Last Name", width: "120px" },
                    { field: "Address", title: "Address", width: "150px" },
                    { field: "Mobile_No", title: "Mobile No", width: "120px" },
                    { field: "Khalti_Account", title: "Khalti Account", width: "120px" },
                    {
                        field: "Date_Of_Birth",
                        title: "Date of Birth",
                        width: "120px",
                        template: function (dataItem) {
                            if (dataItem.Date_Of_Birth) {
                                return kendo.toString(new Date(dataItem.Date_Of_Birth), "yyyy-MM-dd");
                            } else {
                                return "";
                            }
                        }
                    },
                    { field: "Profession", title: "Profession", width: "120px" },
                    { field: "Email_Id", title: "Email", width: "150px" }                   
                ]
            });
        };


    $scope.fetchUsers();
    // debugger;

});
