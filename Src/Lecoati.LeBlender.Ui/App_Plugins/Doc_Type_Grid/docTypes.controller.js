
angular.module("umbraco").controller("docTypes.controller",
    function ($scope, $rootScope, assetsService, $http, LeBlenderRequestHelper, dialogService) {

        // Add doctype
        $scope.add = function (parameter) {
            $scope.model.value.push("");
        }

        // Remove a doctype
        $scope.remove = function ($index) {
            $scope.model.value.splice($index, 1);
        }

        // Init Model
        if (!$scope.model.value) {
            $scope.model.value = [];
        }

        // Init render with the value of viewPath
        // but render always have to be = /App_Plugins/DocTypeGridEditor/Render/DocTypeGridEditor.cshtml
        $scope.model.parentValue.render = $scope.model.parentValue.config.viewPath ? $scope.model.parentValue.config.viewPath : "";
        $scope.$on('editorConfigSaving', function () {
            $scope.model.parentValue.config.viewPath = $scope.model.parentValue.render;
            $scope.model.parentValue.render = "/App_Plugins/DocTypeGridEditor/Render/DocTypeGridEditor.cshtml";
        });

    });