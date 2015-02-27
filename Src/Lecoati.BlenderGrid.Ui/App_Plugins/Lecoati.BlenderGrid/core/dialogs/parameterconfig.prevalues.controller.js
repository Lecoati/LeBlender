angular.module("umbraco").controller("BlenderGrid.Dialog.ParameterConfig.Prevalues.Controller",
    function ($scope, assetsService, $http, BlenderGridRequestHelper, dialogService) {

        $scope.defaultParameterList = [,
            {
                name: "multivalues",
                view: "/umbraco/Views/prevalueeditors/multivalues.html"
            },
            {
            	name: "Simple Media Picker",
            	view: "/umbraco/Views/propertyeditors/mediapicker/mediapicker.html",
            	config: {
            	    multiPicker: true
            	}
            },
            {
                name: "Multi Media Picker",
                view: "/umbraco/Views/propertyeditors/mediapicker/mediapicker.html",
                config: {
                    multiPicker: false
                }
            },
            {
            	name: "Boolean",
            	view: "boolean"
            },
	        {
	            name: "Color Picker",
	            view: "colorpicker"
	        },
            {
                name: "Icon Picker",
                view: "iconpicker",
            },
	        {
	            name: "Content Picker",
	            view: "contentpicker"
	        },
	        {
	            name: "Media Picker",
	            view: "mediapicker"
	        },
	        {
	            name: "Textarea",
	            view: "textarea",
	        },
            {
                name: "Textstring",
                view: "textstring",
            },
            {
                name: "Layers",
                view: "layers",
            }
        ]

        $scope.change = function () {
            var selectedParameterType = $scope.getParameter($scope.model.value.view)
            if (selectedParameterType && selectedParameterType.config) {
                $scope.textAreaconfig = JSON.stringify(selectedParameterType.config, null, 4);
            }
            else {
                $scope.textAreaconfig = "";
            }
        }

        $scope.getParameter = function (view) {
            return _.find($scope.defaultParameterList, function (item) {
                return item.view == view;
            });
        }

        $scope.isCustom = function () {
            var isCustom = true;
            angular.forEach($scope.defaultParameterList, function (item, index) {
                if ($scope.model.value.view != "" && item.view === $scope.model.value.view) {
                    isCustom = false;
                }
            });
            return isCustom;
        }

        $scope.hasConfig = function () {
            var hasConfig = false;
            angular.forEach($scope.defaultParameterList, function (item, index) {
                if ($scope.model.value.config != "" && item.view === $scope.model.value.view) {
                    hasConfig = true;
                }
            });
            return hasConfig;
        }

        $scope.getConfigAsText = function () {

            $scope.textAreaconfig = JSON.stringify($scope.model.value.config, null, 4);

            $scope.$watch('textAreaconfig', function () {
                try {
                    $scope.model.value.config = JSON.parse($scope.textAreaconfig);
                } catch (exp) {
                    //Exception handler
                };
            });

        };

        $scope.save = function () {
            angular.extend($scope.dialogData.parameter, $scope.model.value);
            $scope.close();
        }

        angular.extend($scope, {
            model: {
                value: angular.copy($scope.dialogData.parameter)
            }    
        });

        $scope.getConfigAsText();

    });