angular.module("umbraco").controller("BlenderGrid.ParameterConfig.Controller",
    function ($scope, assetsService, $http, BlenderGridRequestHelper, dialogService) {

        $scope.defaultParameterList = [
	        {
	            name: "Boolean",
	            view: "Boolean"
	        },
	        {
	            name: "Color Picker",
	            view: "colorPicker"
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
                name: "layers",
                view: "layers",
            }
        ]

        $scope.isCustom = function () {
            var isCustom = true;
            angular.forEach($scope.defaultParameterList, function (item, index) {
                if ($scope.model.value.view != "" && item.view === $scope.model.value.view) {
                    isCustom = false;
                }
            });
            return isCustom;
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