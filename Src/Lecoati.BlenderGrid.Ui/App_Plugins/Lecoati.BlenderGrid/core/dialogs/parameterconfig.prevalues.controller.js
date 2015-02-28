angular.module("umbraco").controller("BlenderGrid.Dialog.ParameterConfig.Prevalues.Controller",
    function ($scope, assetsService, $http, BlenderGridRequestHelper, dialogService) {

        $scope.defaultParameterList = [
	        {
	            name: "Textarea",
	            view: "textarea",
	        },
            {
                name: "Textstring",
                view: "textbox",
            },
            {
            	name: "Boolean",
            	view: "boolean"
            },
            //{
            //    name: "Integer",
            //    view: "/umbraco/Views/propertyeditors/integer/integer.html"
            //},
            //{
            //    name: "datepicker",
            //    view: "/umbraco/Views/propertyeditors/datepicker/datepicker.html"
            //},
            {
                name: "Media Picker",
                view: "/umbraco/Views/propertyeditors/mediapicker/mediapicker.html",
                config: {
                    multiPicker: false
                }
            },
            {
                name: "Multi Media Picker",
                view: "/umbraco/Views/propertyeditors/mediapicker/mediapicker.html",
                config: {
                    multiPicker: true
                }
            },
            {
                name: "Content Picker",
                view: "/umbraco/Views/propertyeditors/contentpicker/contentpicker.html",
                config: {
                    multiPicker: false,
                    minNumber: 1,
                    maxNumber: 1
                }
            },
            //{
            //    name: "Multi Content Picker",
            //    view: "/umbraco/Views/propertyeditors/contentpicker/contentpicker.html",
            //    config: {
            //        multiPicker: true,
            //        minNumber: 1,
            //        maxNumber: 100
            //    }
            //},
            {
                name: "- - custom - -",
            }





            //{
            //    name: "multivalues",
            //    view: "/umbraco/Views/prevalueeditors/multivalues.html"
            //},
            //{
            //	name: "Simple Media Picker",
            //	view: "/umbraco/Views/propertyeditors/mediapicker/mediapicker.html",
            //	config: {
            //	    multiPicker: true
            //	}
            //},
            //{
            //	name: "Boolean",
            //	view: "boolean"
            //},
            //{
            //    name: "datepicker",
            //    view: "datepicker"
            //},
            //{
            //    name: "integer",
            //    view: "integer"
            //},
            //{
            //    name: "markdowneditor",
            //    view: "markdowneditor"
            //},
	        //{
	        //    name: "Color Picker",
	        //    view: "colorpicker"
	        //},
            //{
            //    name: "Icon Picker",
            //    view: "iconpicker",
            //},
	        //{
	        //    name: "Content Picker",
	        //    view: "contentpicker"
	        //},
	        //{
	        //    name: "Media Picker",
	        //    view: "/umbraco/Views/propertyeditors/mediapicker/mediapicker.html",
            //    config: {
	        //        multiPicker: false
            //    }
	        //},
	        //{
	        //    name: "Textarea",
	        //    view: "/umbraco/Views/propertyeditors/textarea/textarea.html",
	        //},
            //{
            //    name: "Textstring",
            //    view: "/umbraco/Views/propertyeditors/textbox/textbox.html",
            //},
            //{
            //    name: "Layers",
            //    view: "layers",
            //}

        ]

        // Change property type
        $scope.change = function () {
            $scope.model.value.propretyType = $scope.selectedPropertyType;
            if ($scope.model.value.propretyType.config) {
                $scope.textAreaconfig = JSON.stringify($scope.model.value.propretyType.config, null, 4);
            }
            else {
                $scope.textAreaconfig = "";
            }
        }

        // Control if the property is custom 
        $scope.isCustom = function () {
            if ($scope.model.value.propretyType) {
                return $scope.model.value.propretyType.name === "- - custom - -";
            }
            else {
                return false;
            }
            
        }

        // Stringify the current config config
        $scope.init = function () {

            if (!$scope.model.value.propretyType) {
                $scope.model.value.propretyType = $scope.defaultParameterList[0];
            }

            $scope.selectedPropertyType = _.find($scope.defaultParameterList, function (item) {
                return item.view === $scope.model.value.propretyType.view &&
                       item.name === $scope.model.value.propretyType.name
                       JSON.stringify(item.config) === JSON.stringify($scope.model.value.propretyType.config)
            });

            $scope.textAreaconfig = JSON.stringify($scope.model.value.propretyType.config, null, 4);

            $scope.$watch('textAreaconfig', function () {
                try {
                    $scope.model.value.propretyType.config = JSON.parse($scope.textAreaconfig);
                } catch (exp) {
                    $scope.model.value.propretyType.config = undefined;
                };
            });

        };

        // Save current property
        $scope.save = function () {
            angular.extend($scope.dialogData.parameter, $scope.model.value);
            $scope.close();
        }

        // Extend model
        angular.extend($scope, {
            model: {
                value: angular.copy($scope.dialogData.parameter)
            }    
        });

        $scope.init();

    });