angular.module("umbraco").
    directive('blenderEditorWrapper', function () {
        return {
            scope: {
                property: "=",
                item: "=",
            },
            restrict: 'E',
            replace: true,
            template: '<div ng-include="model.view"></div>',

            controller: function ($scope) {
              
                var initEditorPath = function (property) {
                    if (property && property.$editor && property.$editor.propretyType) {
                        if (property.$editor.propretyType.view && _.indexOf(property.$editor.propretyType.view, "/") >= 0) {
                            return property.$editor.propretyType.view;
                        } else {
                            return "/App_Plugins/Lecoati.BlenderGrid/core/editors/" + property.$editor.propretyType.view + ".html";
                        }
                    }
                };

                $scope.model = {
                    alias: $scope.property.$editor ? $scope.property.$editor.alias : "",
                    label: $scope.property.$editor ? $scope.property.$editor.name : "",
                    config: $scope.property.$editor ? $scope.property.$editor.propretyType.config : {},
                    value: $scope.property.value,
                    view: initEditorPath($scope.property)
                }

                $scope.$watch("model.value", function (newValue, oldValue) {
                    $scope.property.value = newValue;
                }, true);

            }

        };
    });