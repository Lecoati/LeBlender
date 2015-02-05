angular.module("umbraco").controller("BlenderGrid.Editor.Mediapicker.controller",
    function ($scope, assetsService, $http, dialogService, mediaHelper) {

        $scope.pick = function (service) {
            dialogService.mediaPicker({
                multiPicker: false,
                callback: function (data) {
                    $scope.property.value = [];
                    $scope.property.value.push({
                        id: data.id,
                        url: mediaHelper.resolveFile(data, false),
                    });
                }
            });
        };

        $scope.getUrl = function () {

            if ($scope.property.value && $scope.property.value.length > 0) {
                return $scope.property.value[0].url;
            }

        }

    });