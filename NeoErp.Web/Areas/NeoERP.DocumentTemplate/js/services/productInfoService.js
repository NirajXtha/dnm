// Product Info Service - Handles dynamic data loading for product info hover tooltip
(function () {
    'use strict';

    angular.module('DTModule')
        .service('productInfoService', ['$http', '$q', productInfoService]);

    function productInfoService($http, $q) {
        var cache = {};
        var loadingPromises = {};

        this.getProductInfo = function (itemCode) {
            // Return cached data if available
            if (cache[itemCode]) {
                return $q.when(cache[itemCode]);
            }

            // Return existing promise if already loading
            if (loadingPromises[itemCode]) {
                return loadingPromises[itemCode];
            }

            // Make API call
            var url = '/api/TemplateApi/GetProductInfoData?itemCode=' + encodeURIComponent(itemCode);

            var promise = $http.get(url)
                .then(function (response) {
                    cache[itemCode] = response.data;
                    delete loadingPromises[itemCode];
                    return response.data;
                })
                .catch(function (error) {
                    console.error('Error loading product info for item:', itemCode, error);
                    delete loadingPromises[itemCode];
                    return $q.reject(error);
                });

            loadingPromises[itemCode] = promise;
            return promise;
        };

        // Clear cache if needed
        this.clearCache = function (itemCode) {
            if (itemCode) {
                delete cache[itemCode];
            } else {
                cache = {};
            }
        };
    }
})();
