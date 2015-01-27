angular.module("umbraco").controller("uSky.GridSettings.backgroundColorPicker",
    function ($scope, assetsService, $http) {

        $scope.colors = $scope.model.approvedColors ? $scope.model.approvedColors.split(",") : [];

        $scope.setStyle = function (color) {
            return {
                "background-color": color
            }
        }

        // Load css asset
        assetsService.loadCss("/App_Plugins/Lecoati.GridStarterKit/GridSettings/assets/backgroundColorPicker.css");

    });