angular.module("umbraco").controller("BlenderGridEditor.ColorPicker.controller",
    function ($scope, assetsService, $http, dialogService, mediaHelper, $element, angularHelper) {

        assetsService.load([
            "/umbraco/lib/spectrum/spectrum.js"
        ]).then(function () {

            var elem = $element.find("input");
            elem.spectrum({
                color: $scope.property.value,
                allowEmpty: true,
                showInitial: false,
                chooseText: "choose",
                cancelText: "cancel",
                preferredFormat: "hex",
                showInput: true,
                clickoutFiresChange: true,
                change: function (color) {
                    angularHelper.safeApply($scope, function () {
                        if (color) {
                            $scope.property.value = color.toHexString(); 
                        }
                        else {
                            $scope.property.value = '';
                        }
                    });
                }
            });

        });

        assetsService.loadCss("/umbraco/lib/spectrum/spectrum.css");

    });