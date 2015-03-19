angular.module("umbraco").controller("LeBlender.Dialog.ParameterConfig.Prevalues.Controller",
    function ($scope, assetsService, $http, LeBlenderRequestHelper, dialogService) {

        $scope.defaultParameterList = [
            {
                name: "Textstring",
                view: "textstring",
            },
            {
	            name: "Textarea",
	            view: "textarea",
            },
            {
                name: "Rich Text Editor",
                view: "rte"
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
                    multiPicker: "0",
                    showEditButton: "0"
                }
            },
            {
                name: "Multi Content Picker",
                view: "/umbraco/Views/propertyeditors/contentpicker/contentpicker.html",
                config: {
                    multiPicker: "1",
                    showEditButton: "0"
                }
            },
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

        $scope.autoPopulateAlias = function (name) {
            var s = name.replace(/[^a-zA-Z0-9\s\.-]+/g, '');
            return s.toCamelCase();
        }

        var toCamelCase = function (name) {
            var s = name.toPascalCase();
            if ($.trim(s) == "")
                return "";
            if (s.length > 1)
                s = s.substr(0, 1).toLowerCase() + s.substr(1);
            else
                s = s.toLowerCase();
            return s;
        };

        var toPascalCase = function (name) {
            var s = "";
            angular.each($.trim(name).split(/[\s\.-]+/g), function (val, idx) {
                if ($.trim(val) == "")
                    return;
                if (val.length > 1)
                    s += val.substr(0, 1).toUpperCase() + val.substr(1);
                else
                    s += val.toUpperCase();
            });
            return s;
        };

        $scope.init();

        if ($scope.model.value.name === "" && $scope.model.value.name === "") {
            $scope.$watch("model.value.name", function () {
                $scope.model.value.alias = $scope.autoPopulateAlias($scope.model.value.name);
            })
        }

    });