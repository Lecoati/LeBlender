
angular.module("umbraco").controller("LeBlender.Editor.IconPicker.controller",
    function ($scope, assetsService, $http, dialogService, mediaHelper) {

        $scope.config = {
            cssPath: "",
            prefix: "",
            defaultClass: ""
        }

        if ($scope.model.config) {
            angular.extend($scope.config, $scope.model.config);
        }

        $scope.openIconDialog = function () {
            var dialog = dialogService.open({
                template: '/App_Plugins/Lecoati.LeBlender/core/dialogs/iconpickereditor.html',
                show: true,
                dialogData: {
                    icon: $scope.model.value,
                    config: $scope.config
                },
                callback: function (data) {
                    $scope.model.value = data;
                }
            });
        }
        
        // Load css asset
        assetsService.loadCss("/App_Plugins/Lecoati.LeBlender/core/assets/iconpicker.css");
        assetsService.loadCss($scope.config.cssPath);

    });