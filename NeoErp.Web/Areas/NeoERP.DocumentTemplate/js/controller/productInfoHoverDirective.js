// Product Info Hover Directive - Click to show product info tooltip
(function () {
    'use strict';

    angular.module('DTModule')
        .directive('productInfoHover', ['productInfoService', '$document', '$compile', productInfoHover]);

    function productInfoHover(productInfoService, $document, $compile) {
        return {
            restrict: 'A',
            scope: {
                itemCode: '@productInfoHover'
            },
            template: '<i class="fa fa-info-circle" style="font-size: 14px; color: #3498db;" title="Product Info"></i>' +
                '<div class="product-info-tooltip">' +
                '<div ng-if="loading" style="padding: 20px; text-align: center;">' +
                '<i class="fa fa-spinner fa-spin" style="font-size: 24px; color: #3498db;"></i>' +
                '<p>Loading...</p>' +
                '</div>' +
                '<div ng-if="error && !loading" style="padding: 20px; text-align: center; color: #e74c3c;">' +
                '<i class="fa fa-exclamation-triangle"></i>' +
                '<p>Error loading info</p>' +
                '</div>' +
                '<div ng-if="!loading && !error && productData">' +
                '<div class="info-section">' +
                '<div class="section-header product-header">Current Product Info</div>' +
                '<div class="product-name-header">{{productData.ProductInfo.ProductName || "N/A"}}</div>' +
                '<table class="info-table">' +
                '<tr><td>Product Code / ID</td><td>{{productData.ProductInfo.ProductCode || "N/A"}}</td></tr>' +
                '<tr><td>Unit</td><td>{{productData.ProductInfo.Unit || "N/A"}}</td></tr>' +
                '<tr><td>Category</td><td>{{productData.ProductInfo.Category || "N/A"}}</td></tr>' +
                '<tr><td>HS Code</td><td>{{productData.ProductInfo.HsCode || "N/A"}}</td></tr>' +
                '<tr><td>Created By</td><td>{{productData.ProductInfo.CreatedBy || "N/A"}}</td></tr>' +
                '</table>' +
                '</div>' +
                '<div class="info-section">' +
                '<div class="section-header sales-header">Closing Stock - Location wise</div>' +
                '<table class="info-table">' +
                '<tr ng-repeat="stock in productData.StockByLocation"><td>{{stock.LocationName}}</td><td>{{stock.Stock | number:2}}</td></tr>' +
                '<tr ng-if="!productData.StockByLocation.length"><td colspan="2" style="text-align:center;color:#999;">No stock data</td></tr>' +
                '</table>' +
                '<div class="subsection-header">Sales Rate History</div>' +
                '<table class="info-table">' +
                '<tr ng-repeat="sale in productData.SalesHistory | limitTo:3">' +
                '<td>{{sale.CustomerName}}</td><td>{{sale.SalesPrice | number:2}}</td>' +
                '</tr>' +
                '<tr ng-if="!productData.SalesHistory.length"><td colspan="2" style="text-align:center;color:#999;">No sales history</td></tr>' +
                '</table>' +
                '</div>' +
                '</div>' +
                '</div>',
            link: function (scope, element, attrs) {
                var loaded = false;
                var lastItemCode = null;
                var isOpen = false;

                element.addClass('product-info-hover');

                // Toggle on click
                element.on('click', function (e) {
                    e.stopPropagation();
                    e.preventDefault();
                    var currentCode = scope.itemCode;
                    if (!currentCode) return;

                    if (isOpen) {
                        closeTooltip();
                        scope.$apply();
                    } else {
                        // Close all others
                        var allActive = document.querySelectorAll('.product-info-hover.active');
                        angular.forEach(allActive, function (el) {
                            angular.element(el).removeClass('active');
                        });
                        openTooltip(currentCode);
                    }
                });

                function openTooltip(itemCode) {
                    isOpen = true;
                    element.addClass('active');
                    if (!loaded || itemCode !== lastItemCode) {
                        loaded = true;
                        lastItemCode = itemCode;
                        loadProductInfo(itemCode);
                    }
                }

                function closeTooltip() {
                    isOpen = false;
                    element.removeClass('active');
                }

                function loadProductInfo(itemCode) {
                    scope.$apply(function () {
                        scope.loading = true;
                        scope.error = false;
                        scope.productData = null;
                    });

                    productInfoService.getProductInfo(itemCode)
                        .then(function (data) {
                            scope.productData = data;
                            scope.loading = false;
                        })
                        .catch(function () {
                            scope.loading = false;
                            scope.error = true;
                        });
                }

                // Close on outside click
                function onDocumentClick(e) {
                    if (isOpen && !element[0].contains(e.target)) {
                        scope.$apply(function () {
                            closeTooltip();
                        });
                    }
                }
                $document.on('click', onDocumentClick);

                scope.$on('$destroy', function () {
                    element.off('click');
                    $document.off('click', onDocumentClick);
                });
            }
        };
    }
})();
