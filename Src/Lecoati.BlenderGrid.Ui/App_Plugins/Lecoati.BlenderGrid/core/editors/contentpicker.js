
angular.module("umbraco").controller("BlenderGrid.Editor.Contentpicker.controller",
    function ($scope, assetsService, $http, dialogService, mediaHelper) {

        if (!$scope.property.value) {
            $scope.property.value = [];
        }

        $scope.config = {
            multiPicker: false,
            startNodeId: -1,
            maxNumber:1
        }

        if ($scope.property.$editor.config) {
            angular.extend($scope.config, $scope.property.$editor.config);
        }

        $scope.openContentPicker = function () {
            dialogService.contentPicker({
                multiPicker: $scope.config.multiPicker,
                startNodeId: $scope.config.startNodeId,
                callback: function (data) {
                    if ($scope.multiPicker) {
                        data.forEach(function (data) {
                            var existsNode = $scope.property.value.some(function (node) {
                                return node.id === data.id;
                            });

                            if (!existsNode) {
                                $scope.property.value.push({
                                    id: data.id,
                                    name: data.name,
                                    icon: data.icon
                                });
                            }
                        });
                    }
                    else {
                        $scope.property.value.push({
                            name: data.name,
                            id: data.id,
                            icon: data.icon
                        });
                    }   
                }
            });
        };

        $scope.remove = function (index) {
            $scope.property.value.splice(index, 1);
        };

        $scope.sortableOptions = {
            handle: ".icon-navigation",
            axis: "y",
            delay: 150,
            distance: 5
        };

    });