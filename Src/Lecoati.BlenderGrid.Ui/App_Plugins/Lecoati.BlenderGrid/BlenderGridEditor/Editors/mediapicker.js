angular.module("umbraco").controller("BlenderGridEditor.Mediapicker.controller",
    function ($scope, assetsService, $http, dialogService, mediaHelper) {

        $scope.pick = function (service) {
            dialogService.mediaPicker({
                multiPicker: false,
                callback: function (data) {
                    $scope.property.value = mediaHelper.resolveFile(data, false);
                }
            });
        };

    });