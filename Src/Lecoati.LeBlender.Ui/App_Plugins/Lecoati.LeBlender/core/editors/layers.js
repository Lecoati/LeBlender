angular.module("umbraco").controller("LeBlender.Editor.Layers.controller",
    function ($scope, assetsService, $http, dialogService, mediaHelper) {

        $scope.openLayersEditor = function () {

            var dialog = dialogService.open({
                template: '/App_Plugins/Lecoati.LeBlender/core/dialogs/layerseditor.html',
                show: true,
                dialogData: {
                    value: $scope.model.value,
                    slide: {
                        image: $scope.item["image"] && $scope.item["image"].value && $scope.item["image"].value.length > 0 ? $scope.item["image"].value[0].url : "",
                        color: $scope.item["color"] && $scope.item["color"].value ? $scope.item["color"].value : ""
                    }
                },
                callback: function (data) {
                    $scope.model.value = data
                }
            });

        }

    });