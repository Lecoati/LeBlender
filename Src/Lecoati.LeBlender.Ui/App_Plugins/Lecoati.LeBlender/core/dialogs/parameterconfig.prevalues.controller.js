angular.module("umbraco").controller("LeBlender.Dialog.ParameterConfig.Prevalues.Controller",
    function ($scope, assetsService, $http, LeBlenderRequestHelper, dialogService) {

        $scope.availableDataTypes = angular.copy($scope.dialogData.availableDataTypes);
        $scope.availableDataTypes.push({
            guid: "",
            name: "- - custom - -"
        })

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
            if ($scope.model.value.dataType === "") {
                return true;
            }
            else {
                return false;
            }
        }

        // Stringify the current config config
        $scope.init = function () {

            if (!$scope.model.value.propretyType) {
                $scope.model.value.propretyType = {};
            }

            switch ($scope.model.value.propretyType.name) {
                case "Textstring": $scope.model.value.dataType = "0cc0eba1-9960-42c9-bf9b-60e150b429ae"; 
                    $scope.model.value.propretyType = {};
                    break;
                case "Textarea": $scope.model.value.dataType = "c6bac0dd-4ab9-45b1-8e30-e4b619ee5da3";
                    $scope.model.value.propretyType = {};
                    break;
                case "Rich Text Editor": $scope.model.value.dataType = "ca90c950-0aff-4e72-b976-a30b1ac57dad";
                    $scope.model.value.propretyType = {};
                    break;
                case "Boolean": $scope.model.value.dataType = "92897bc6-a5f3-4ffe-ae27-f2e7e33dda49";
                    $scope.model.value.propretyType = {};
                    break;
                case "Media Picker": $scope.model.value.dataType = "93929b9a-93a2-4e2a-b239-d99334440a59";
                    $scope.model.value.propretyType = {};
                    break;
                case "Multi Media Picker": $scope.model.value.dataType = "7e3962cc-ce20-4ffc-b661-5897a894ba7e";
                    $scope.model.value.propretyType = {};
                    break;
                case "Content Picker": $scope.model.value.dataType = "a6857c73-d6e9-480c-b6e6-f15f6ad11125";
                    $scope.model.value.propretyType = {};       
                    break;
                case "Multi Content Picker":
                    $scope.model.value.dataType = "";
                    break;
            }

            if (!$scope.model.value.dataType && $scope.model.value.propretyType) {
                $scope.textAreaconfig = JSON.stringify($scope.model.value.propretyType.config, null, 4);
            }

            $scope.$watch('textAreaconfig', function () {
                try {
                    $scope.model.value.propretyType.config = JSON.parse($scope.textAreaconfig);
                } catch (exp) {
                    delete $scope.model.value.propretyType.config;
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

//$scope.defaultParameterList = [
//    {
//        name: "Textstring",
//        view: "textstring",
//    },
//    {
//        name: "Textarea",
//        view: "textarea",
//    },
//    {
//        name: "Rich Text Editor",
//        view: "rte"
//    },
//    {
//        name: "Boolean",
//        view: "boolean"
//    },
//    {
//        name: "Media Picker",
//        view: "/umbraco/Views/propertyeditors/mediapicker/mediapicker.html",
//        config: {
//            multiPicker: false
//        }
//    },
//    {
//        name: "Multi Media Picker",
//        view: "/umbraco/Views/propertyeditors/mediapicker/mediapicker.html",
//        config: {
//            multiPicker: true
//        }
//    },
//    {
//        name: "Content Picker",
//        view: "/umbraco/Views/propertyeditors/contentpicker/contentpicker.html",
//        config: {
//            multiPicker: "0",
//            showEditButton: "0"
//        }
//    },
//    {
//        name: "Multi Content Picker",
//        view: "/umbraco/Views/propertyeditors/contentpicker/contentpicker.html",
//        config: {
//            multiPicker: "1",
//            showEditButton: "0"
//        }
//    },
//    {
//        name: "- - custom - -",
//    }
//]