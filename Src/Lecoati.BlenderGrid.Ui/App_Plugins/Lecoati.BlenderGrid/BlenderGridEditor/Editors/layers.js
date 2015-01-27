angular.module("umbraco").controller("BlenderGridEditor.Layers.controller",
    function ($scope, assetsService, $http, dialogService, mediaHelper) {

        $scope.openLayersEditor = function () {

            var dialog = dialogService.open({
                template: '/App_Plugins/Lecoati.BlenderGrid/BlenderGridEditor/Editors/LayersEditor.html',
                show: true,
                dialogData: {
                    value: $scope.property.value,
                    slide: {
                        image: $scope.item["image"] && $scope.item["image"].value ? $scope.item["image"].value : "",
                        color: $scope.item["color"] && $scope.item["color"].value ? $scope.item["color"].value : ""
                    }
                },
                callback: function (data) {
                    $scope.property.value = data
                }
            });

        }

    });