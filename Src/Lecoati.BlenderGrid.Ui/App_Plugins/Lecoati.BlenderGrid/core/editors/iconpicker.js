
angular.module("umbraco").controller("BlenderGrid.Editor.IconPicker.controller",
    function ($scope, assetsService, $http, dialogService, mediaHelper) {

        $scope.config = {
            cssPath: "",
            prefixe: "",
            defaultClass: ""
        }

        if ($scope.property.$editor.config) {
            angular.extend($scope.config, $scope.property.$editor.config);
        }

        $scope.openIconDialog = function () {
            var dialog = dialogService.open({
                template: '/App_Plugins/Lecoati.BlenderGrid/core/dialogs/iconpickereditor.html',
                show: true,
                dialogData: {
                    icon: $scope.property.value,
                    config: $scope.config
                },
                callback: function (data) {
                    $scope.property.value = data;
                }
            });
        }
        
        // Load css asset
        assetsService.loadCss("/App_Plugins/Lecoati.BlenderGrid/core/assets/iconpicker.css");
        assetsService.loadCss($scope.config.cssPath);

    });