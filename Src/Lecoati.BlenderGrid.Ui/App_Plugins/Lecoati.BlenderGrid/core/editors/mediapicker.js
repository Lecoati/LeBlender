angular.module("umbraco").controller("BlenderGrid.Editor.Mediapicker.controller",
    function ($scope, assetsService, $http, dialogService, mediaHelper) {

        $scope.pick = function (service) {
            dialogService.mediaPicker({
                multiPicker: false,
                callback: function (data) {
                    $scope.model.value = [];
                    $scope.model.value.push({
                        id: data.id,
                        url: mediaHelper.resolveFile(data, false),
                    });
                }
            });
        };

        $scope.getUrl = function () {

            if ($scope.model.value && $scope.model.value.length > 0) {
                return $scope.model.value[0].url;
            }

        }

    });