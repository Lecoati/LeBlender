angular.module("umbraco").controller("LeBlender.Dialog.ParameterConfig.Prevalues.Controller",
    function ($scope, editorService) {

		var guidEmpty = "00000000-0000-0000-0000-000000000000";
		$scope.name = "Property Setting";

        /***************************************/
        /* legacy adaptor 0.9.15 */
        /***************************************/

		var parameter = $scope.model.dialogData.parameter;

        if (parameter && parameter.propretyType) {

            switch (parameter.propretyType.name) {
                case "Textstring":
                    parameter.dataType = "0cc0eba1-9960-42c9-bf9b-60e150b429ae";
                    parameter.propretyType = {};
                    break;
                case "Textarea":
                    parameter.dataType = "c6bac0dd-4ab9-45b1-8e30-e4b619ee5da3";
                    parameter.propretyType = {};
                    break;
                case "Rich Text Editor":
                    parameter.dataType = "ca90c950-0aff-4e72-b976-a30b1ac57dad";
                    parameter.propretyType = {};
                    break;
                case "Boolean":
                    parameter.dataType = "92897bc6-a5f3-4ffe-ae27-f2e7e33dda49";
                    parameter.propretyType = {};
                    break;
                case "Media Picker":
                    parameter.dataType = "93929b9a-93a2-4e2a-b239-d99334440a59";
                    parameter.propretyType = {};
                    break;
                case "Multi Media Picker":
                    parameter.dataType = "7e3962cc-ce20-4ffc-b661-5897a894ba7e";
                    parameter.propretyType = {};
                    break;
                case "Content Picker":
                    parameter.dataType = "a6857c73-d6e9-480c-b6e6-f15f6ad11125";
                    parameter.propretyType = {};
                    break;
                case "Multi Content Picker":
                    parameter.dataType = "";
                    break;
            }

        }

        /***************************************/
        /* properties */
        /***************************************/

        // Change property type
        $scope.change = function () {
            $scope.model.value.propretyType = $scope.selectedPropertyType;
            if ($scope.model.value.propretyType.config) {
                $scope.textAreaconfig = JSON.stringify($scope.model.value.propretyType.config, null, 4);
            }
            else {
                $scope.textAreaconfig = "";
            }
        };

        // Control if the property is custom 
        $scope.isCustom = function () {
            if ($scope.model.value.dataType === guidEmpty) {
                return true;
            }
            else {
                return false;
            }
        };

        // Stringify the current config
        $scope.init = function () {

            if (!$scope.model.value.propretyType) {
                $scope.model.value.propretyType = {};
            }

            if (!$scope.model.value.dataType) {
                // find "Textstring" datatype from guid and select on init - fallback to first element
                var datatypeToSelect = _.findWhere($scope.availableDataTypes, { guid: '0cc0eba1-9960-42c9-bf9b-60e150b429ae' }) || $scope.availableDataTypes[0];
                $scope.model.value.dataType = datatypeToSelect.guid;
            }

            if (!$scope.model.value.dataType && $scope.model.value.propretyType) {
                $scope.textAreaconfig = JSON.stringify($scope.model.value.propretyType.config, null, 4);
            }

            $scope.$watch('textAreaconfig', function () {
                try {
                    $scope.model.value.propretyType.config = JSON.parse($scope.textAreaconfig);
                } catch (exp) {
                    delete $scope.model.value.propretyType.config;
                }
            });

		};

        $scope.close = function () {
            editorService.close();
        };

        // Save current property
        $scope.save = function () {
            if ($scope.model.value && $scope.model.value.name && $scope.model.value.alias) {
                if (parameter) {
                    parameter.name = $scope.model.value.name;
                    parameter.alias = $scope.model.value.alias;
                    parameter.dataType = $scope.model.value.dataType;
                }
                else {
                    $scope.model.submit($scope.model.value);
                }
            }
            editorService.close();
        };

        /***************************************/
        /* autoPopulateAlias */
        /***************************************/

        // main method for autoPopulateAlias
        $scope.autoPopulateAlias = function (name) {
            var s = name.replace(/[^a-zA-Z0-9\s\.-]+/g, '');
            return s.toCamelCase();
        };

        // init autoPopulateAlias
        $scope.initAutoPopulateAlias = function () {
            if ($scope.model.value.name === "") {
                $scope.$watch("model.value.name", function () {
                    $scope.model.value.alias = $scope.autoPopulateAlias($scope.model.value.name);
                });
            }
        };

        // toCamelCase
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

        // toPascalCase
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

        /***************************************/
        /* init */
        /***************************************/

        // Init availableDataTypes
        $scope.availableDataTypes = angular.copy($scope.model.dialogData.availableDataTypes);
        $scope.availableDataTypes.unshift({
            guid: guidEmpty,
            name: "- - custom - -"
        });

		if (parameter)
			$scope.model.value = parameter;

        if (!$scope.model.value) {
            $scope.model.value = {
                name: "",
                alias: ""
            };
        }

        $scope.init();
        $scope.initAutoPopulateAlias();

    });