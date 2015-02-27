
angular.module("umbraco").controller("BlenderGrid.Editor.Contentpicker.controller",
    function ($scope, assetsService, $http, dialogService, mediaHelper) {

        if (!$scope.model.value) {
            $scope.model.value = [];
        }

        $scope.config = {
            multiPicker: false,
            startNodeId: -1,
            maxNumber:1
        }

        if ($scope.model.config) {
            angular.extend($scope.config, $scope.model.config);
        }

        $scope.openContentPicker = function () {
            dialogService.contentPicker({
                multiPicker: $scope.config.multiPicker,
                startNodeId: $scope.config.startNodeId,
                callback: function (data) {
                    if ($scope.multiPicker) {
                        data.forEach(function (data) {
                            var existsNode = $scope.model.value.some(function (node) {
                                return node.id === data.id;
                            });

                            if (!existsNode) {
                                $scope.model.value.push({
                                    id: data.id,
                                    name: data.name,
                                    icon: data.icon
                                });
                            }
                        });
                    }
                    else {
                        $scope.model.value.push({
                            name: data.name,
                            id: data.id,
                            icon: data.icon
                        });
                    }   
                }
            });
        };

        $scope.remove = function (index) {
            $scope.model.value.splice(index, 1);
        };

        $scope.sortableOptions = {
            handle: ".icon-navigation",
            axis: "y",
            delay: 150,
            distance: 5
        };

    });