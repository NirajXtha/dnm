(function (window, angular) {
    'use strict';
    // Stub to avoid 404s if the real controller is not needed on current pages
    try {
        var app = angular.module('distributionModule');
        app.controller('TermsAndConditionSetupCtrl', ['$scope', function ($scope) {
            // TODO: implement if required
            $scope.__stub = true;
        }]);
    } catch (e) {
        // Angular application/module might not be present on this page; that's okay.
        // This file exists just to prevent 404 errors referenced from some layouts.
    }
})(window, window.angular);
