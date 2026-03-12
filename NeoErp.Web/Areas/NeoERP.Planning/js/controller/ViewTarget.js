planningModule.controller('ViewTarget', function ($scope, $http, $route, $routeParams) {

    $scope.targetId = $routeParams.targetId;
    $scope.targetData = {};

    // Load Target by ID
    $scope.loadTargetById = function (targetId) {
        $http.get("/api/DistributionPlaningApi/GetTargetById", { params: { targetId: targetId } })
            .then(function (response) {
                var data = response.data;
                if (!data) return;

                $scope.targetData = {
                    TARGET_ID: data.TARGET_ID || 0,
                    TARGET_NAME: data.TARGET_NAME || 0,
                    TARGET_TYPE: data.TARGET_TYPE || 0,
                    TARGET_QUANTITY: data.TARGET_QUANTITY || 0,
                    TARGET_AMOUNT: data.TARGET_AMOUNT || 0,
                    TARGET_SETUP_TYPE: data.TARGET_SETUP_TYPE || 0,
                    SUB_TARGET_TYPE: data.SUB_TARGET_TYPE || 0,
                    DATE_FILTER: data.DATE_FILTER || 0,
                    FROM_DATE: data.FROM_DATE || 0,
                    END_DATE: data.END_DATE || 0,
                    MASTER_CODE: data.MASTER_CODE || 0,
                    MASTER_NAME: data.MASTER_NAME || 0,
                    EMP_GROUP: (data.EMP_GROUP && data.EMP_GROUP.length > 0) ? data.EMP_GROUP.join(', ') : 0,
                    ASSIGN_EMPLOYEE: (data.ASSIGN_EMPLOYEE && data.ASSIGN_EMPLOYEE.length > 0) ? data.ASSIGN_EMPLOYEE.join(', ') : 0,
                    CUSTOMER_GROUP: (data.CUSTOMER_GROUP && data.CUSTOMER_GROUP.length > 0) ? data.CUSTOMER_GROUP.join(', ') : 0,
                    ITEM_GROUP: (data.ITEM_GROUP && data.ITEM_GROUP.length > 0) ? data.ITEM_GROUP.join(', ') : 0,
                    ITEMS: (data.ITEMS && data.ITEMS.length > 0)
                        ? data.ITEMS.map(i => (i.ITEM_CODE || 0) + " (" + (i.MU_CODE || 0) + ")").join(', ')
                        : 0
                };
            })
            .catch(function (error) {
                console.error("Error loading target:", error);
            });
    };


    $scope.loadTargetById($scope.targetId);

});
